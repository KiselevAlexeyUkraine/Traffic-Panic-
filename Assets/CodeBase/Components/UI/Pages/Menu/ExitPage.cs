using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Zenject;
using Codebase.Services;

namespace Codebase.Components.Ui.Pages.Menu
{
    public class ExitPage : BasePage
    {
        [SerializeField] private Button _accept;
        [SerializeField] private Button _cancel;

        private AudioService _audioService;

        [Inject]
        private void Construct(AudioService audioService)
        {
            _audioService = audioService;
        }

        private void Awake()
        {
            _accept.onClick.AddListener(() =>
            {
                _audioService.PlayClickSound();
                SceneSwitcher.Instance.ExitGame();
            });

            _cancel.onClick.AddListener(() =>
            {
                _audioService.PlayClickSound();
                PageSwitcher.Open(PageName.Menu);
            });

            AddHoverSound(_accept);
            AddHoverSound(_cancel);
        }

        private void OnDestroy()
        {
            _accept.onClick.RemoveAllListeners();
            _cancel.onClick.RemoveAllListeners();
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
