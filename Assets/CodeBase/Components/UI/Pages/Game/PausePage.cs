using Codebase.Services;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Zenject;

namespace Codebase.Components.Ui.Pages.Game
{
    /// <summary>
    /// Класс отвечает за управление страницей паузы, предоставляя кнопки для продолжения игры, перезапуска, настроек и выхода.
    /// </summary>
    public class PausePage : BasePage
    {
        [SerializeField] private Button _continue; // Кнопка для продолжения игры.
        [SerializeField] private Button _restart; // Кнопка для перезапуска текущего уровня.
        [SerializeField] private Button _settings; // Кнопка для перехода в меню настроек.
        [SerializeField] private Button _exit; // Кнопка для выхода в главное меню.

        [SerializeField] private PauseManager _pauseManager; // Менеджер паузы для управления состоянием игры.

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
                SceneSwitcher.Instance.LoadScene(0);
            });

            AddHoverSound(_continue);
            AddHoverSound(_restart);
            AddHoverSound(_settings);
            AddHoverSound(_exit);
        }

        /// <summary>
        /// Убираем подписки с событий кнопок при уничтожении объекта, чтобы избежать утечек памяти.
        /// </summary>
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
