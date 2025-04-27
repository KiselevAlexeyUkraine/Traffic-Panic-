using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Zenject;
using Codebase.Services;

namespace Codebase.Components.Ui.Pages.Game
{
    public class FailedPage : BasePage
    {
        [SerializeField] private Button _restart;
        [SerializeField] private Button _exit;

        private AudioService _audioService;
        private SceneService _sceneService;

        [Inject]
        private void Construct(AudioService audioService, SceneService sceneService)
        {
            _audioService = audioService;
            _sceneService = sceneService;
        }

        private void Awake()
        {
            _restart.onClick.AddListener(() =>
            {
                _audioService.PlayClickSound();
                _sceneService.RestartCurrentScene();
            });

            _exit.onClick.AddListener(() =>
            {
                _audioService.PlayClickSound();
                _sceneService.Load(1);
            });

            AddHoverSound(_restart);
            AddHoverSound(_exit);
        }

        private void OnDestroy()
        {
            _restart.onClick.RemoveAllListeners();
            _exit.onClick.RemoveAllListeners();
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