using Assets._Project.Develop.Runtime.UI.Core;
using DG.Tweening;
using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Assets._Project.Develop.Runtime.UI.LevelsMenuPopup
{
    public class LevelTileView : MonoBehaviour, IShowableView
    {
        public event Action Clicked;

        [SerializeField] private Image _background;
        [SerializeField] private TMP_Text _levelNameText;
        [SerializeField] private TMP_Text _levelCostText;
        [SerializeField] private TMP_Text _descriptionText;
        [SerializeField] private Button _button;

        private void OnEnable()
        {
            _button.onClick.AddListener(OnButtonClicked);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(OnButtonClicked);
        }

        public void Init(string levelName, Sprite levelIcon, int levelNumber)
        {
            _levelNameText.text = levelNumber.ToString();
            _background.sprite = levelIcon;
            _levelCostText.text = $"{levelNumber * Random.Range(50, 200)}";

            _descriptionText.text = ScrambleAndShorten(_descriptionText.text);
        }

        private string ScrambleAndShorten(string original)
        {
            if (string.IsNullOrEmpty(original)) return original;

            int targetLength = Mathf.RoundToInt(original.Length * Random.Range(0.4f, 0.8f));

            char[] chars = original.ToCharArray();

            for (int i = chars.Length - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                var temp = chars[i];
                chars[i] = chars[j];
                chars[j] = temp;
            }

            return new string(chars, 0, targetLength).Trim();
        }

        private void OnDestroy()
        {
            transform.DOKill();
        }

        public Tween Hide()
        {
            transform.DOKill();

            return DOTween.Sequence();
        }

        public Tween Show()
        {
            transform.DOKill();

            return transform
                .DOScale(1, 0.1f)
                .From(0)
                .SetUpdate(true)
                .Play();
        }

        private void OnButtonClicked() => Clicked?.Invoke();

        public void SetComplete()
        {
            _button.interactable = false;
            _background.color = Color.green;
        }

        public void SetActive()
        {
            _button.interactable = true;
            _background.color = Color.white;
        }

        public void SetBlock()
        {
            _button.interactable = false;
            _background.color = Color.red;
        }
    }
}
