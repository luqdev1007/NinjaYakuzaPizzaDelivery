using Assets._Project.Develop.Runtime.Meta.Infrastructure;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.Timeline;
using UnityEngine.UI;

namespace Assets._Project.Develop.Editor
{
    /// <summary>
    /// Пересобирает содержимое Intro.unity и IntroTimeline.playable с нуля.
    /// Идемпотентен: сносит прошлый канвас и прошлый .playable, строит заново,
    /// поэтому запускать можно сколько угодно раз.
    ///
    /// Приоритет — корректность биндингов и порядок кадров. Тайминг и кривые
    /// намеренно простые (линейные), доводить их предполагается руками в окне
    /// Timeline; повторный запуск генератора эту ручную правку затрёт.
    /// </summary>
    public static class IntroSceneBuilder
    {
        private const string ScenePath = "Assets/_Project/Scenes/Intro.unity";
        private const string FramesFolder = "Assets/_Project/Art/Intro";
        private const string TimelinePath = "Assets/_Project/Art/Intro/IntroTimeline.playable";

        private const string CanvasName = "IntroCanvas";
        private const int FramesCount = 6;

        private const float FrameDuration = 5f;
        private const float CrossfadeDuration = 0.5f;

        // Зум внутрь от 1.05: стартовый запас не даёт оголиться краям кадра,
        // если пропорции картинки не совпали с пропорциями экрана.
        private const float FrameZoomFrom = 1.05f;
        private const float FrameZoomTo = 1.12f;

        private const float CaptionDelay = 0.8f;
        private const float CaptionFade = 0.4f;
        private const float CaptionTailCut = 1.2f;

        private const float ClipFrameRate = 60f;

        // Те же шрифты, что у диалоговой подсказки и диалогового текста.
        private const string HintFontGuid = "18e5ede33e490485d9576cb16d82ad1e";
        private const string CaptionFontGuid = "27f54c9130c9748c9be53df11fa03382";

        // Без '&' в пути: Unity трактует его как модификатор горячей клавиши.
        [MenuItem("NYPD/Intro/Rebuild Intro Scene and Timeline")]
        public static void Rebuild()
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo() == false)
            {
                return;
            }

            Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);

            IntroBootstrap bootstrap = UnityEngine.Object.FindAnyObjectByType<IntroBootstrap>();

            if (bootstrap == null)
            {
                Debug.LogError($"[IntroSceneBuilder] {nameof(IntroBootstrap)} not found in {ScenePath}");
                return;
            }

            PlayableDirector director = bootstrap.GetComponent<PlayableDirector>();

            if (director == null)
            {
                Debug.LogError($"[IntroSceneBuilder] {nameof(PlayableDirector)} not found on {bootstrap.name}");
                return;
            }

            Sprite[] frames = LoadFrameSprites();

            if (frames == null)
            {
                return;
            }

            SetupCamera(scene);

            RemoveOldCanvas(scene);

            RectTransform canvasRoot = CreateCanvas();

            List<Animator> frameAnimators = new List<Animator>();
            List<Animator> captionAnimators = new List<Animator>();

            for (int i = 0; i < FramesCount; i++)
            {
                frameAnimators.Add(CreateFrame(canvasRoot, i, frames[i]));
            }

            for (int i = 0; i < FramesCount; i++)
            {
                captionAnimators.Add(CreateCaption(canvasRoot, i));
            }

            CreateSkipHint(canvasRoot);

            TimelineAsset timeline = BuildTimeline(director, frameAnimators, captionAnimators);

            director.playableAsset = timeline;
            director.playOnAwake = false;
            director.extrapolationMode = DirectorWrapMode.None;
            director.timeUpdateMode = DirectorUpdateMode.UnscaledGameTime;

            EditorUtility.SetDirty(director);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"[IntroSceneBuilder] Rebuilt {FramesCount} frames, total duration {timeline.duration:0.##}s");
        }

        private static Sprite[] LoadFrameSprites()
        {
            Sprite[] frames = new Sprite[FramesCount];

            for (int i = 0; i < FramesCount; i++)
            {
                string path = $"{FramesFolder}/{i + 1}.jpg";
                Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);

                if (sprite == null)
                {
                    Debug.LogError($"[IntroSceneBuilder] Frame sprite not found: {path}");
                    return null;
                }

                frames[i] = sprite;
            }

            return frames;
        }

        private static void SetupCamera(Scene scene)
        {
            // Кадры — Screen Space Overlay, но на фейдах и стыках под ними видно
            // фон камеры. Со скайбоксом это светлая заливка вместо затемнения.
            foreach (GameObject root in scene.GetRootGameObjects())
            {
                Camera camera = root.GetComponent<Camera>();

                if (camera == null)
                {
                    continue;
                }

                camera.clearFlags = CameraClearFlags.SolidColor;
                camera.backgroundColor = Color.black;

                EditorUtility.SetDirty(camera);
            }
        }

        private static void RemoveOldCanvas(Scene scene)
        {
            foreach (GameObject root in scene.GetRootGameObjects())
            {
                if (root.name == CanvasName)
                {
                    UnityEngine.Object.DestroyImmediate(root);
                }
            }
        }

        private static RectTransform CreateCanvas()
        {
            GameObject canvasObject = new GameObject(CanvasName, typeof(Canvas), typeof(CanvasScaler));

            Canvas canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            return canvasObject.GetComponent<RectTransform>();
        }

        private static Animator CreateFrame(RectTransform parent, int index, Sprite sprite)
        {
            RectTransform rect = CreateUIObject($"Frame_{index + 1}", parent);

            Stretch(rect);
            rect.localScale = new Vector3(FrameZoomFrom, FrameZoomFrom, 1f);

            Image image = rect.gameObject.AddComponent<Image>();
            image.sprite = sprite;
            image.type = Image.Type.Simple;
            image.preserveAspect = false;
            image.raycastTarget = false;

            AddHiddenCanvasGroup(rect);

            return AddTimelineAnimator(rect);
        }

        private static Animator CreateCaption(RectTransform parent, int index)
        {
            RectTransform rect = CreateUIObject($"Caption_{index + 1}", parent);

            rect.anchorMin = new Vector2(0.5f, 0f);
            rect.anchorMax = new Vector2(0.5f, 0f);
            rect.pivot = new Vector2(0.5f, 0f);
            rect.anchoredPosition = new Vector2(0f, 180f);
            rect.sizeDelta = new Vector2(1500f, 220f);

            TextMeshProUGUI text = rect.gameObject.AddComponent<TextMeshProUGUI>();
            text.text = $"Frame {index + 1} caption";
            text.fontSize = 48f;
            text.alignment = TextAlignmentOptions.Center;
            text.raycastTarget = false;

            TMP_FontAsset font = LoadFont(CaptionFontGuid);

            if (font != null)
            {
                text.font = font;
            }

            AddHiddenCanvasGroup(rect);

            return AddTimelineAnimator(rect);
        }

        private static void CreateSkipHint(RectTransform parent)
        {
            // Якорь зеркалит SkipLabelAnchor из DialogDisplayView.prefab:
            // нижний правый угол, (-60, 30), 225x60.
            RectTransform rect = CreateUIObject("SkipHint", parent);

            rect.anchorMin = new Vector2(1f, 0f);
            rect.anchorMax = new Vector2(1f, 0f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = new Vector2(-60f, 30f);
            rect.sizeDelta = new Vector2(225f, 60f);

            TextMeshProUGUI text = rect.gameObject.AddComponent<TextMeshProUGUI>();
            text.text = "[E] to skip";
            text.fontSize = 36f;
            text.alignment = TextAlignmentOptions.Center;
            text.raycastTarget = false;

            TMP_FontAsset font = LoadFont(HintFontGuid);

            if (font != null)
            {
                text.font = font;
            }
        }

        private static TimelineAsset BuildTimeline(
            PlayableDirector director,
            List<Animator> frameAnimators,
            List<Animator> captionAnimators)
        {
            AssetDatabase.DeleteAsset(TimelinePath);

            TimelineAsset timeline = ScriptableObject.CreateInstance<TimelineAsset>();
            timeline.name = "IntroTimeline";
            AssetDatabase.CreateAsset(timeline, TimelinePath);

            timeline.editorSettings.frameRate = ClipFrameRate;

            ClearSceneBindings(director);

            float step = FrameDuration - CrossfadeDuration;

            for (int i = 0; i < FramesCount; i++)
            {
                float frameStart = i * step;

                AnimationTrack frameTrack = CreateTrack(timeline, $"Frame {i + 1}");
                AnimationClip frameClip = CreateFrameClip($"Frame_{i + 1}_Clip");
                AttachClip(timeline, frameTrack, frameClip, frameStart, FrameDuration);
                director.SetGenericBinding(frameTrack, frameAnimators[i]);

                float captionDuration = FrameDuration - CaptionDelay - CaptionTailCut;

                AnimationTrack captionTrack = CreateTrack(timeline, $"Caption {i + 1}");
                AnimationClip captionClip = CreateCaptionClip($"Caption_{i + 1}_Clip", captionDuration);
                AttachClip(timeline, captionTrack, captionClip, frameStart + CaptionDelay, captionDuration);
                director.SetGenericBinding(captionTrack, captionAnimators[i]);
            }

            EditorUtility.SetDirty(timeline);
            AssetDatabase.SaveAssets();

            return timeline;
        }

        private static AnimationTrack CreateTrack(TimelineAsset timeline, string name)
        {
            AnimationTrack track = timeline.CreateTrack<AnimationTrack>(null, name);

            // Сцена — источник истины для позиции элемента: клипы двигают только
            // scale и alpha, но при ApplyTransformOffsets трек всё равно норовит
            // подставить собственный оффсет.
            track.trackOffset = TrackOffset.ApplySceneOffsets;

            return track;
        }

        private static void AttachClip(
            TimelineAsset timeline,
            AnimationTrack track,
            AnimationClip animationClip,
            double start,
            double duration)
        {
            AssetDatabase.AddObjectToAsset(animationClip, timeline);

            TimelineClip clip = track.CreateClip(animationClip);
            clip.start = start;
            clip.duration = duration;
            clip.displayName = animationClip.name;
        }

        private static AnimationClip CreateFrameClip(string name)
        {
            AnimationClip clip = new AnimationClip();
            clip.name = name;
            clip.frameRate = ClipFrameRate;

            AnimationCurve alpha = new AnimationCurve();
            alpha.AddKey(new Keyframe(0f, 0f));
            alpha.AddKey(new Keyframe(CrossfadeDuration, 1f));
            alpha.AddKey(new Keyframe(FrameDuration - CrossfadeDuration, 1f));
            alpha.AddKey(new Keyframe(FrameDuration, 0f));
            MakeLinear(alpha);

            AnimationCurve zoom = AnimationCurve.Linear(0f, FrameZoomFrom, FrameDuration, FrameZoomTo);
            AnimationCurve flat = AnimationCurve.Linear(0f, 1f, FrameDuration, 1f);

            SetCurve(clip, typeof(CanvasGroup), "m_Alpha", alpha);
            SetCurve(clip, typeof(RectTransform), "m_LocalScale.x", zoom);
            SetCurve(clip, typeof(RectTransform), "m_LocalScale.y", zoom);
            SetCurve(clip, typeof(RectTransform), "m_LocalScale.z", flat);

            return clip;
        }

        private static AnimationClip CreateCaptionClip(string name, float duration)
        {
            AnimationClip clip = new AnimationClip();
            clip.name = name;
            clip.frameRate = ClipFrameRate;

            AnimationCurve alpha = new AnimationCurve();
            alpha.AddKey(new Keyframe(0f, 0f));
            alpha.AddKey(new Keyframe(CaptionFade, 1f));
            alpha.AddKey(new Keyframe(duration - CaptionFade, 1f));
            alpha.AddKey(new Keyframe(duration, 0f));
            MakeLinear(alpha);

            SetCurve(clip, typeof(CanvasGroup), "m_Alpha", alpha);

            return clip;
        }

        private static void SetCurve(AnimationClip clip, Type componentType, string property, AnimationCurve curve)
        {
            EditorCurveBinding binding = EditorCurveBinding.FloatCurve(string.Empty, componentType, property);
            AnimationUtility.SetEditorCurve(clip, binding, curve);
        }

        private static void MakeLinear(AnimationCurve curve)
        {
            for (int i = 0; i < curve.length; i++)
            {
                AnimationUtility.SetKeyLeftTangentMode(curve, i, AnimationUtility.TangentMode.Linear);
                AnimationUtility.SetKeyRightTangentMode(curve, i, AnimationUtility.TangentMode.Linear);
            }
        }

        private static void ClearSceneBindings(PlayableDirector director)
        {
            // Биндинги живут на директоре и переживают удаление старого .playable,
            // оставляя записи с мёртвыми ключами. Чистим, чтобы пересборка не
            // копила мусор.
            SerializedObject serialized = new SerializedObject(director);
            SerializedProperty bindings = serialized.FindProperty("m_SceneBindings");

            if (bindings != null)
            {
                bindings.ClearArray();
                serialized.ApplyModifiedPropertiesWithoutUndo();
            }
        }

        private static RectTransform CreateUIObject(string name, RectTransform parent)
        {
            GameObject uiObject = new GameObject(name);
            RectTransform rect = uiObject.AddComponent<RectTransform>();
            rect.SetParent(parent, false);

            return rect;
        }

        private static void Stretch(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        private static void AddHiddenCanvasGroup(RectTransform rect)
        {
            CanvasGroup group = rect.gameObject.AddComponent<CanvasGroup>();
            group.alpha = 0f;
            group.interactable = false;
            group.blocksRaycasts = false;
        }

        private static Animator AddTimelineAnimator(RectTransform rect)
        {
            Animator animator = rect.gameObject.AddComponent<Animator>();

            // Без AlwaysAnimate Unity вправе выкинуть аниматор из апдейта, когда
            // считает объект невидимым — на alpha=0 это ровно наш случай.
            animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;

            return animator;
        }

        private static TMP_FontAsset LoadFont(string guid)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);

            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            return AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(path);
        }
    }
}
