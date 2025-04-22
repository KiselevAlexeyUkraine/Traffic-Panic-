using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Zenject;
using Codebase.Services;

namespace Codebase.Components.Ui
{
    public class LinkButton : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private string _url;

        private AudioService _audioService;

        [Inject]
        private void Construct(AudioService audioService)
        {
            _audioService = audioService;
        }

        private void Awake()
        {
            _button.onClick.AddListener(() =>
            {
                _audioService.PlayClickSound();
                Application.OpenURL(_url);
            });

            AddHoverSound(_button);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveAllListeners();
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
