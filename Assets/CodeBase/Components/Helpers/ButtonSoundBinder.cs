using Codebase.Services;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace Components.Helpers
{
    public class ButtonSoundBinder : MonoBehaviour
    {
        [SerializeField] private List<Button> _buttons;

        private AudioService _audioService;

        [Inject]
        private void Construct(AudioService audioService)
        {
            _audioService = audioService;
        }

        private void Awake()
        {
            foreach (var button in _buttons)
            {
                if (button == null) continue;

                button.onClick.AddListener(() => _audioService.PlayClickSound());
                AddHoverSound(button);
            }
        }

        private void AddHoverSound(Button button)
        {
            if (button == null)
                return;

            EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>()
                                   ?? button.gameObject.AddComponent<EventTrigger>();

            var entry = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
            entry.callback.AddListener((_) => _audioService.PlayHoverSound());

            trigger.triggers.Add(entry);
        }

        private void OnDestroy()
        {
            foreach (var button in _buttons)
            {
                if (button == null) continue;
                button.onClick.RemoveAllListeners();

                var trigger = button.GetComponent<EventTrigger>();
                if (trigger != null)
                {
                    trigger.triggers.RemoveAll(entry => entry.eventID == EventTriggerType.PointerEnter);
                }
            }
        }
    }
}