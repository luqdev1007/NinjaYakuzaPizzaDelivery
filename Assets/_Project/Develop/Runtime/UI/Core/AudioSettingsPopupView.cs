using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Assets._Project.Develop.Runtime.UI.Core;

namespace Assets._Project.Develop.Runtime.UI.AudioSettingsPopup
{
    public class AudioSettingsPopupView : PopupViewBase
    {
        [Header("Master")]
        [SerializeField] private Slider _masterSlider;
        [SerializeField] private Button _masterToggle;
        [SerializeField] private Image _masterToggleIcon;

        [Header("Music")]
        [SerializeField] private Slider _musicSlider;
        [SerializeField] private Button _musicToggle;
        [SerializeField] private Image _musicToggleIcon;

        [Header("SFX")]
        [SerializeField] private Slider _sfxSlider;
        [SerializeField] private Button _sfxToggle;
        [SerializeField] private Image _sfxToggleIcon;

        [Header("Icons")]
        [SerializeField] private Sprite _mutedIcon;
        [SerializeField] private Sprite _unmutedIcon;

        public Slider MasterSlider => _masterSlider;
        public Slider MusicSlider => _musicSlider;
        public Slider SFXSlider => _sfxSlider;

        public Button MasterToggle => _masterToggle;
        public Button MusicToggle => _musicToggle;
        public Button SFXToggle => _sfxToggle;

        public void SetMasterToggleIcon(bool muted)
            => _masterToggleIcon.sprite = muted ? _mutedIcon : _unmutedIcon;

        public void SetMusicToggleIcon(bool muted)
            => _musicToggleIcon.sprite = muted ? _mutedIcon : _unmutedIcon;

        public void SetSFXToggleIcon(bool muted)
            => _sfxToggleIcon.sprite = muted ? _mutedIcon : _unmutedIcon;

        // Двигаем слайдер без нотификации (чтоб не зациклить колбэки)
        public void SetMasterSliderSilent(float value) => SetSliderSilent(_masterSlider, value);
        public void SetMusicSliderSilent(float value) => SetSliderSilent(_musicSlider, value);
        public void SetSFXSliderSilent(float value) => SetSliderSilent(_sfxSlider, value);

        private void SetSliderSilent(Slider slider, float value)
        {
            slider.onValueChanged.RemoveAllListeners();
            slider.value = value;
            // listeners вернёт Presenter через Initialize
        }

        protected override void ModifyShowAnimation(Sequence animation)
        {
            base.ModifyShowAnimation(animation);
            animation
                .Append(_masterSlider.transform.DOScale(1f, 0.15f).From(0f).SetEase(Ease.OutBack))
                .Append(_musicSlider.transform.DOScale(1f, 0.15f).From(0f).SetEase(Ease.OutBack))
                .Append(_sfxSlider.transform.DOScale(1f, 0.15f).From(0f).SetEase(Ease.OutBack));
        }
    }
}