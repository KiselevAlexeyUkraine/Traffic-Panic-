using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Zenject;
using Codebase.Services;

namespace Codebase.Components.Ui.Pages.Menu
{
    public class MenuPage : BasePage
    {
        [SerializeField] private Button _start;
        [SerializeField] private Button _settings;
        [SerializeField] private Button _authors;
        [SerializeField] private Button _exit;

        private AudioService _audioService;

        [Inject]
        private void Construct(AudioService audioService)
        {
            _audioService = audioService;
        }

        private void Awake()
        {
            _start.onClick.AddListener(() => { _audioService.PlayClickSound(); SceneSwitcher.Instance.LoadNextScene(); });
            _settings.onClick.AddListener(() => { _audioService.PlayClickSound(); PageSwitcher.Open(PageName.Settings); });
            _authors.onClick.AddListener(() => { _audioService.PlayClickSound(); PageSwitcher.Open(PageName.Authors); });
            _exit.onClick.AddListener(() => { _audioService.PlayClickSound(); PageSwitcher.Open(PageName.Exit); });

            AddHoverSound(_start);
            AddHoverSound(_settings);
            AddHoverSound(_authors);
            AddHoverSound(_exit);
        }

        private void OnDestroy()
        {
            _start.onClick.RemoveAllListeners();
            _settings.onClick.RemoveAllListeners();
            _authors.onClick.RemoveAllListeners();
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
