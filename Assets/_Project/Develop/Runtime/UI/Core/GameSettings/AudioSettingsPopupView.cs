using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Develop.Runtime.UI.Core.GameSettings
{
    public class AudioSettingsPopupView : PopupViewBase
    {
        [Header("Master")]
        [SerializeField] private Slider _masterSlider;
        [SerializeField] private Button _masterToggle;
        [SerializeField] private Image _masterToggleIcon;
        [SerializeField] private Sprite _masterUnmutedIcon;
        [SerializeField] private Sprite _masterMutedIcon;

        [Header("Music")]
        [SerializeField] private Slider _musicSlider;
        [SerializeField] private Button _musicToggle;
        [SerializeField] private Image _musicToggleIcon;
        [SerializeField] private Sprite _musicUnmutedIcon;
        [SerializeField] private Sprite _musicMutedIcon;

        [Header("SFX")]
        [SerializeField] private Slider _sfxSlider;
        [SerializeField] private Button _sfxToggle;
        [SerializeField] private Image _sfxToggleIcon;
        [SerializeField] private Sprite _sfxUnmutedIcon;
        [SerializeField] private Sprite _sfxMutedIcon;

        public Slider MasterSlider => _masterSlider;
        public Slider MusicSlider => _musicSlider;
        public Slider SFXSlider => _sfxSlider;

        public Button MasterToggle => _masterToggle;
        public Button MusicToggle => _musicToggle;
        public Button SFXToggle => _sfxToggle;

        public void SetMasterToggleIcon(bool muted)
            => _masterToggleIcon.sprite = muted ? _masterMutedIcon : _masterUnmutedIcon;

        public void SetMusicToggleIcon(bool muted)
            => _musicToggleIcon.sprite = muted ? _musicMutedIcon : _musicUnmutedIcon;

        public void SetSFXToggleIcon(bool muted)
            => _sfxToggleIcon.sprite = muted ? _sfxMutedIcon : _sfxUnmutedIcon;

        public void SetMasterSliderSilent(float value) => _masterSlider.SetValueWithoutNotify(value);
        public void SetMusicSliderSilent(float value) => _musicSlider.SetValueWithoutNotify(value);
        public void SetSFXSliderSilent(float value) => _sfxSlider.SetValueWithoutNotify(value);

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