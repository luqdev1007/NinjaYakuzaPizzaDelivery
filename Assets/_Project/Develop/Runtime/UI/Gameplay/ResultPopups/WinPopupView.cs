using Assets._Project.Develop.Runtime.Gameplay.Features.LevelResultsFeature;
using Assets._Project.Develop.Runtime.UI.Core;
using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.ResultPopups
{
    public class WinPopupView : PopupViewBase
    {
        public event Action ContinueClicked;

        [Header("Common UI")]
        [SerializeField] private TMP_Text _title;

        [Header("Stars Configuration")]
        [SerializeField] private StarUIComponents _timeStar;
        [SerializeField] private StarUIComponents _styleStar;
        [SerializeField] private StarUIComponents _secretsStar;

        [Serializable]
        public struct StarUIComponents
        {
            public Transform StarFilled; // Объект "Star (Filled)" из твоей иерархии
            public TMP_Text InfoText;    // Текст внутри InfoLabel -> DescriptionText
        }

        public void SetTitle(string title) => _title.text = title;
        public void OnContinueClick() => ContinueClicked?.Invoke();

        public void SetupResults(LevelResultReport report)
        {
            // Настройка ВРЕМЕНИ (Orange)
            _timeStar.StarFilled.localScale = report.TimeStarEarned ? Vector3.one : Vector3.zero;
            _timeStar.InfoText.text = $"<color=\"orange\"><size=32>TIME</size></color>\n\n" +
                                      $"You achieved minimal goal for time on this level!\n\n" +
                                      $"Your best time:\n<size=48><color=\"orange\">{report.FinalTime:F1}s</color></size>";

            // Настройка СТИЛЯ (Red)
            _styleStar.StarFilled.localScale = report.StyleStarEarned ? Vector3.one : Vector3.zero;
            _styleStar.InfoText.text = $"<color=\"red\"><size=32>STYLE RANK</size></color>\n\n" +
                                       $"You achieved minimal goal for style points on this level!\n\n" +
                                       $"Your best rank:\n<size=48><color=\"red\">{report.StyleLetter}</color></size>";

            // Настройка СЕКРЕТОВ (Purple)
            _secretsStar.StarFilled.localScale = report.SecretStarEarned ? Vector3.one : Vector3.zero;
            _secretsStar.InfoText.text = $"<color=\"purple\"><size=32>SECRET CHESTS</size></color>\n\n" +
                                         $"You find <color=\"purple\">{report.CollectedSecrets}/{report.TotalSecrets}</color> secret chests on level\n\n" +
                                         $"Don't forget to unlock founded secret loot in Dojo";
        }

        protected override void ModifyShowAnimation(Sequence animation)
        {
            base.ModifyShowAnimation(animation);

            var starViews = new List<Transform>();
            if (_timeStar.StarFilled.localScale.x > 0.1f) starViews.Add(_timeStar.StarFilled);
            if (_styleStar.StarFilled.localScale.x > 0.1f) starViews.Add(_styleStar.StarFilled);
            if (_secretsStar.StarFilled.localScale.x > 0.1f) starViews.Add(_secretsStar.StarFilled);

            foreach (var star in starViews)
            {
                animation
                    .Append(star.DOScale(1.25f, 0.3f).SetEase(Ease.OutBack).From(0))
                    .Join(star.DOLocalRotate(Vector3.forward * 360, 0.3f, RotateMode.LocalAxisAdd)
                         .SetEase(Ease.OutCubic).From(Vector3.zero))
                    .Append(star.DOScale(1.25f, 0.1f)); 

                animation.AppendInterval(0.1f);
            }
        }
    }
}