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
                PageSwitcher.Open(PageName.Menu).Forget();
            });

            _masterVolume.onValueChanged.AddListener(OnMasterVolumeChanged);
            _soundsVolume.onValueChanged.AddListener(OnSoundsVolumeChanged);
            _musicVolume.onValueChanged.AddListener(OnMusicVolumeChanged);

            AddHoverSound(_back);

            ApplySavedSettings();
        }

        private void OnEnable()
        {
            ApplySavedSettings();
        }

        private void OnDisable()
        {
            SaveSettings();
        }

        private void ApplySavedSettings()
        {
            _masterVolume.SetValueWithoutNotify(_audioService.SavedMasterVolume);
            _soundsVolume.SetValueWithoutNotify(_audioService.SavedSoundsVolume);
            _musicVolume.SetValueWithoutNotify(_audioService.SavedMusicVolume);

            _audioService.SetMasterVolume(_audioService.SavedMasterVolume);
            _audioService.SetSoundsVolume(_audioService.SavedSoundsVolume);
            _audioService.SetMusicVolume(_audioService.SavedMusicVolume);
        }

        private void SaveSettings()
        {
            _audioService.SaveVolumes();
        }

        private void OnMasterVolumeChanged(float value)
        {
            _audioService.SetMasterVolume(value);
            _audioService.PlayClickSound();
        }

        private void OnSoundsVolumeChanged(float value)
        {
            _audioService.SetSoundsVolume(value);
            _audioService.PlayClickSound();
        }

        private void OnMusicVolumeChanged(float value)
        {
            _audioService.SetMusicVolume(value);
            _audioService.PlayClickSound();
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
