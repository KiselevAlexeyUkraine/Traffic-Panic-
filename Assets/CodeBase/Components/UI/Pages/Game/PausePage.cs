using Codebase.Services;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Zenject;

namespace Codebase.Components.Ui.Pages.Game
{
    public class PausePage : BasePage
    {
        [SerializeField] private Button _continue; 
        [SerializeField] private Button _restart;
        [SerializeField] private Button _settings;
        [SerializeField] private Button _exit; 

        [SerializeField] private PauseManager _pauseManager;

        private AudioService _audioService;

        [Inject]
        private void Construct(AudioService audioService)
        {
            _audioService = audioService;
        }

        private void Awake()
        {
            _continue.onClick.AddListener(() =>
            {
                _audioService.PlayClickSound();
                _pauseManager.SwitchState();
            });

            _restart.onClick.AddListener(() =>
            {
                _audioService.PlayClickSound();
                SceneSwitcher.Instance.RestartCurrentScene();
            });

            _settings.onClick.AddListener(() =>
            {
                _audioService.PlayClickSound();
                PageSwitcher.Open(PageName.GameSettings);
            });

            _exit.onClick.AddListener(() =>
            {
                _audioService.PlayClickSound();
                SceneSwitcher.Instance.LoadScene(1);
            });

            AddHoverSound(_continue);
            AddHoverSound(_restart);
            AddHoverSound(_settings);
            AddHoverSound(_exit);
        }

        private void OnDestroy()
        {
            _continue.onClick.RemoveAllListeners();
            _restart.onClick.RemoveAllListeners();
            _settings.onClick.RemoveAllListeners();
            _exit.onClick.RemoveAllListeners();
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
