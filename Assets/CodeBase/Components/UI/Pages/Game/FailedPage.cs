using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Zenject;
using Codebase.Services;

namespace Codebase.Components.Ui.Pages.Game
{
    /// <summary>
    /// Класс отвечает за управление страницей "Неудачи", предоставляя кнопки для перезапуска уровня или выхода в меню.
    /// </summary>
    public class FailedPage : BasePage
    {
        [SerializeField] private Button _restart; // Кнопка для перезапуска текущего уровня.
        [SerializeField] private Button _exit; // Кнопка для перехода в главное меню.

        private AudioService _audioService;

        [Inject]
        private void Construct(AudioService audioService)
        {
            _audioService = audioService;
        }

        /// <summary>
        /// Подписываемся на события кнопок при инициализации объекта.
        /// </summary>
        private void Awake()
        {
            _restart.onClick.AddListener(() =>
            {
                _audioService.PlayClickSound();
                SceneSwitcher.Instance.LoadScene(SceneSwitcher.Instance.CurrentScene);
            });

            _exit.onClick.AddListener(() =>
            {
                _audioService.PlayClickSound();
                SceneSwitcher.Instance.LoadScene(1);
            });

            AddHoverSound(_restart);
            AddHoverSound(_exit);
        }

        /// <summary>
        /// Убираем подписки с событий кнопок при уничтожении объекта, чтобы избежать утечек памяти.
        /// </summary>
        private void OnDestroy()
        {
            _restart.onClick.RemoveAllListeners();
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
