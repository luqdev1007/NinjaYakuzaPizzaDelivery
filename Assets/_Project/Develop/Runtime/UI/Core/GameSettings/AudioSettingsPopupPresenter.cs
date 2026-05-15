using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilities.AudioManagment;

namespace Assets._Project.Develop.Runtime.UI.Core.GameSettings
{
    public class AudioSettingsPopupPresenter : PopupPresenterBase
    {
        private readonly AudioSettingsPopupView _view;
        private readonly IAudioService _audioService;

        private bool _masterMuted;
        private bool _musicMuted;
        private bool _sfxMuted;

        private float _masterBeforeMute = 1f;
        private float _musicBeforeMute = 1f;
        private float _sfxBeforeMute = 1f;

        protected override PopupViewBase PopupView => _view;

        public AudioSettingsPopupPresenter(
            AudioSettingsPopupView view,
            ICoroutinesPerformer coroutinesPerformer,
            IAudioService audioService) : base(coroutinesPerformer)
        {
            _view = view;
            _audioService = audioService;
        }

        public override void Initialize()
        {
            base.Initialize();

            float masterVol = _audioService.GetMasterVolume();
            float musicVol = _audioService.GetMusicVolume();
            float sfxVol = _audioService.GetSfxVolume();

            _masterBeforeMute = masterVol > 0.001f ? masterVol : 1f;
            _musicBeforeMute = musicVol > 0.001f ? musicVol : 1f;
            _sfxBeforeMute = sfxVol > 0.001f ? sfxVol : 1f;

            _masterMuted = masterVol <= 0.001f;
            _musicMuted = musicVol <= 0.001f;
            _sfxMuted = sfxVol <= 0.001f;

            _view.SetMasterSliderSilent(masterVol);
            _view.SetMusicSliderSilent(musicVol);
            _view.SetSFXSliderSilent(sfxVol);

            _view.MasterSlider.onValueChanged.AddListener(OnMasterSliderChanged);
            _view.MusicSlider.onValueChanged.AddListener(OnMusicSliderChanged);
            _view.SFXSlider.onValueChanged.AddListener(OnSFXSliderChanged);

            _view.MasterToggle.onClick.AddListener(OnMasterToggleClicked);
            _view.MusicToggle.onClick.AddListener(OnMusicToggleClicked);
            _view.SFXToggle.onClick.AddListener(OnSFXToggleClicked);

            _view.SetMasterToggleIcon(_masterMuted);
            _view.SetMusicToggleIcon(_musicMuted);
            _view.SetSFXToggleIcon(_sfxMuted);
        }

        public override void Dispose()
        {
            base.Dispose();

            _view.MasterSlider.onValueChanged.RemoveListener(OnMasterSliderChanged);
            _view.MusicSlider.onValueChanged.RemoveListener(OnMusicSliderChanged);
            _view.SFXSlider.onValueChanged.RemoveListener(OnSFXSliderChanged);

            _view.MasterToggle.onClick.RemoveListener(OnMasterToggleClicked);
            _view.MusicToggle.onClick.RemoveListener(OnMusicToggleClicked);
            _view.SFXToggle.onClick.RemoveListener(OnSFXToggleClicked);
        }

        private void OnMasterSliderChanged(float value)
        {
            _audioService.SetMasterVolume(value);
            if (_masterMuted && value > 0.001f)
            {
                _masterMuted = false;
                _view.SetMasterToggleIcon(false);
            }
        }

        private void OnMusicSliderChanged(float value)
        {
            _audioService.SetMusicVolume(value);
            if (_musicMuted && value > 0.001f)
            {
                _musicMuted = false;
                _view.SetMusicToggleIcon(false);
            }
        }

        private void OnSFXSliderChanged(float value)
        {
            _audioService.SetSfxVolume(value);
            if (_sfxMuted && value > 0.001f)
            {
                _sfxMuted = false;
                _view.SetSFXToggleIcon(false);
            }
        }

        private void OnMasterToggleClicked()
        {
            _masterMuted = !_masterMuted;
            if (_masterMuted)
            {
                _masterBeforeMute = _view.MasterSlider.value > 0.001f ? _view.MasterSlider.value : 1f;
                _audioService.SetMasterVolume(0f);
                _view.SetMasterSliderSilent(0f);
            }
            else
            {
                _audioService.SetMasterVolume(_masterBeforeMute);
                _view.SetMasterSliderSilent(_masterBeforeMute);
            }
            _view.SetMasterToggleIcon(_masterMuted);
        }

        private void OnMusicToggleClicked()
        {
            _musicMuted = !_musicMuted;
            if (_musicMuted)
            {
                _musicBeforeMute = _view.MusicSlider.value > 0.001f ? _view.MusicSlider.value : 1f;
                _audioService.SetMusicVolume(0f);
                _view.SetMusicSliderSilent(0f);
            }
            else
            {
                _audioService.SetMusicVolume(_musicBeforeMute);
                _view.SetMusicSliderSilent(_musicBeforeMute);
            }
            _view.SetMusicToggleIcon(_musicMuted);
        }

        private void OnSFXToggleClicked()
        {
            _sfxMuted = !_sfxMuted;
            if (_sfxMuted)
            {
                _sfxBeforeMute = _view.SFXSlider.value > 0.001f ? _view.SFXSlider.value : 1f;
                _audioService.SetSfxVolume(0f);
                _view.SetSFXSliderSilent(0f);
            }
            else
            {
                _audioService.SetSfxVolume(_sfxBeforeMute);
                _view.SetSFXSliderSilent(_sfxBeforeMute);
            }
            _view.SetSFXToggleIcon(_sfxMuted);
        }
    }
}