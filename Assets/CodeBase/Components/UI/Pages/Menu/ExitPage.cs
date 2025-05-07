using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Zenject;
using Codebase.Services;

#if UNITY_EDITOR
using UnityEditor;
#endif

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
            _accept.onClick.AddListener(OnAcceptClick);
            _cancel.onClick.AddListener(OnCancelClick);

            AddHoverSound(_accept);
            AddHoverSound(_cancel);
        }

        private void OnDestroy()
        {
            _accept.onClick.RemoveAllListeners();
            _cancel.onClick.RemoveAllListeners();
        }

        private void OnAcceptClick()
        {
            _audioService.PlayClickSound();

#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private void OnCancelClick()
        {
            _audioService.PlayClickSound();
            PageSwitcher.Open(PageName.Menu).Forget();
        }

        private void AddHoverSound(Button button)
        {
            if (button == null)
                return;

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
