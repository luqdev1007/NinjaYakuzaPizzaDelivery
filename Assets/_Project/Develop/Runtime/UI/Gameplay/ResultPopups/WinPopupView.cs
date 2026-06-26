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
        [SerializeField] private StarUIComponents _currencyStar;

        [Serializable]
        public struct StarUIComponents
        {
            public Transform StarFilled; 
            public TMP_Text InfoText;    
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

            // Настройка ВАЛЮТЫ (Green)
            _currencyStar.StarFilled.localScale = report.CurrencyStarEarned ? Vector3.one : Vector3.zero;
            _currencyStar.InfoText.text = $"<color=\"green\"><size=32>LOOT</size></color>\n\n" +
                                          $"Collect enough gold from props and soul shards from enemies!\n\n" +
                                          $"Gold:\n<size=24><color=\"green\">{report.CollectedGold}/{report.GoldThreshold}</color></size>\n" +
                                          $"Soul Shards:\n<size=24><color=\"green\">{report.CollectedShards}/{report.ShardThreshold}</color></size>";
        }

        protected override void ModifyShowAnimation(Sequence animation)
        {
            base.ModifyShowAnimation(animation);

            var starViews = new List<Transform>();

            if (_timeStar.StarFilled.localScale.x > 0.1f)
            {
                starViews.Add(_timeStar.StarFilled);
            }

            if (_styleStar.StarFilled.localScale.x > 0.1f)
            {
                starViews.Add(_styleStar.StarFilled);
            }

            if (_currencyStar.StarFilled.localScale.x > 0.1f)
            {
                starViews.Add(_currencyStar.StarFilled);
            }

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