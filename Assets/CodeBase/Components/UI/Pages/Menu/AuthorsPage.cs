using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Zenject;
using Codebase.Services;

namespace Codebase.Components.Ui.Pages.Menu
{
    public class AuthorsPage : BasePage
    {
        [SerializeField] private Button _back;

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

            AddHoverSound(_back);
        }

        private void OnDestroy()
        {
            _back.onClick.RemoveAllListeners();
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
