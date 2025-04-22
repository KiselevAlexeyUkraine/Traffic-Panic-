using Codebase.Services;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Zenject;

namespace Codebase.Components.Ui.Pages.Menu
{
    public class SettingsPage : BasePage
    {
        [SerializeField] private Button _back;
        [SerializeField] private Slider _masterVolume;
        [SerializeField] private Slider _soundsVolume;
        [SerializeField] private Slider _musicVolume;

        private AudioService _audioService;
        private bool _isInitialized = false; // Флаг для предотвращения звука при загрузке

        [Inject]
        private void Construct(AudioService audioService)
        {
            _audioService = audioService;
        }

        private void Awake()
        {
            _back.onClick.AddListener(() =>
            {
                _audioService.PlayClickSound();
                PageSwitcher.Open(PageName.Menu);
            });

            _masterVolume.onValueChanged.AddListener(OnMasterVolumeChanged);
            _soundsVolume.onValueChanged.AddListener(OnSoundsVolumeChanged);
            _musicVolume.onValueChanged.AddListener(OnMusicVolumeChanged);

            AddHoverSound(_back);
            LoadSettings();

            _isInitialized = true; // Теперь можно воспроизводить звук
        }

        private void LoadSettings()
        {
            _isInitialized = false; // Запрещаем звуки при загрузке
            _masterVolume.value = _audioService.SavedMasterVolume;
            _soundsVolume.value = _audioService.SavedSoundsVolume;
            _musicVolume.value = _audioService.SavedMusicVolume;
        }

        private void OnMasterVolumeChanged(float value)
        {
            _audioService.SetMasterVolume(value);
            if (_isInitialized) _audioService.PlayClickSound();
        }

        private void OnSoundsVolumeChanged(float value)
        {
            _audioService.SetSoundsVolume(value);
            if (_isInitialized) _audioService.PlayClickSound();
        }

        private void OnMusicVolumeChanged(float value)
        {
            _audioService.SetMusicVolume(value);
            if (_isInitialized) _audioService.PlayClickSound();
        }

        private void OnDestroy()
        {
            _back.onClick.RemoveAllListeners();
            _masterVolume.onValueChanged.RemoveListener(OnMasterVolumeChanged);
            _soundsVolume.onValueChanged.RemoveListener(OnSoundsVolumeChanged);
            _musicVolume.onValueChanged.RemoveListener(OnMusicVolumeChanged);
        }

        private void AddHoverSound(Button button)
        {
            EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>() ?? button.gameObject.AddComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerEnter
            };
            entry.callback.AddListener((_) => _audioService.PlayHoverSound());
            trigger.triggers.Add(entry);
        }
    }
}
