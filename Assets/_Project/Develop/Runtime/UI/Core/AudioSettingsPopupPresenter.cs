using Assets._Project.Develop.Runtime.UI.Core;
using Assets._Project.Develop.Runtime.Utilites.AudioManagement;
using Assets._Project.Develop.Runtime.Utilites.CoroutinesManagment;

namespace Assets._Project.Develop.Runtime.UI.AudioSettingsPopup
{
    public class AudioSettingsPopupPresenter : PopupPresenterBase
    {
        private readonly AudioSettingsPopupView _view;
        private readonly AudioService _audioService;

        private bool _masterMuted;
        private bool _musicMuted;
        private bool _sfxMuted;

        // Запоминаем значения до мута чтобы восстановить
        private float _masterBeforeMute = 1f;
        private float _musicBeforeMute = 1f;
        private float _sfxBeforeMute = 1f;

        protected override PopupViewBase PopupView => _view;

        public AudioSettingsPopupPresenter(
            AudioSettingsPopupView view,
            AudioService audioService,
            ICoroutinesPerformer coroutinesPerformer) : base(coroutinesPerformer)
        {
            _view = view;
            _audioService = audioService;
        }

        public override void Initialize()
        {
            base.Initialize();

            float masterVol = _audioService.GetMasterVolume();
            float musicVol = _audioService.GetMusicVolume();
            float sfxVol = _audioService.GetSFXVolume();

            // Запоминаем значения до мута (на случай если уже замьючено)
            _masterBeforeMute = masterVol > 0f ? masterVol : 1f;
            _musicBeforeMute = musicVol > 0f ? musicVol : 1f;
            _sfxBeforeMute = sfxVol > 0f ? sfxVol : 1f;

            // Определяем состояние мута по текущему значению
            _masterMuted = masterVol <= 0f;
            _musicMuted = musicVol <= 0f;
            _sfxMuted = sfxVol <= 0f;

            // Выставляем слайдеры по реальным значениям
            _view.SetMasterSliderSilent(masterVol);
            _view.SetMusicSliderSilent(musicVol);
            _view.SetSFXSliderSilent(sfxVol);

            // Подписываем слайдеры
            _view.MasterSlider.onValueChanged.AddListener(OnMasterSliderChanged);
            _view.MusicSlider.onValueChanged.AddListener(OnMusicSliderChanged);
            _view.SFXSlider.onValueChanged.AddListener(OnSFXSliderChanged);

            // Подписываем тоглы
            _view.MasterToggle.onClick.AddListener(OnMasterToggleClicked);
            _view.MusicToggle.onClick.AddListener(OnMusicToggleClicked);
            _view.SFXToggle.onClick.AddListener(OnSFXToggleClicked);

            // Иконки по реальному состоянию мута
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
            // Если двигаем слайдер вручную — снимаем мут
            if (_masterMuted && value > 0f)
            {
                _masterMuted = false;
                _view.SetMasterToggleIcon(false);
            }
        }

        private void OnMusicSliderChanged(float value)
        {
            _audioService.SetMusicVolume(value);
            if (_musicMuted && value > 0f)
            {
                _musicMuted = false;
                _view.SetMusicToggleIcon(false);
            }
        }

        private void OnSFXSliderChanged(float value)
        {
            _audioService.SetSFXVolume(value);
            if (_sfxMuted && value > 0f)
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
                _masterBeforeMute = _view.MasterSlider.value;
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
                _musicBeforeMute = _view.MusicSlider.value;
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
                _sfxBeforeMute = _view.SFXSlider.value;
                _audioService.SetSFXVolume(0f);
                _view.SetSFXSliderSilent(0f);
            }
            else
            {
                _audioService.SetSFXVolume(_sfxBeforeMute);
                _view.SetSFXSliderSilent(_sfxBeforeMute);
            }
            _view.SetSFXToggleIcon(_sfxMuted);
        }
    }
}