using Codebase.Components.Player;
using Codebase.Services.Inputs;
using Codebase.Components.Ui.Pages;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Codebase.Services

{
    public class PauseManager : MonoBehaviour
    {
        [SerializeField]
        private PageSwitcher _pageSwitcher;

        private CursorToggle _cursorToggle = new();

        public bool IsPaused { get; private set; }

        [SerializeField] private PlayerMovement playerMovement;

        private IInput _playerInput;

        [Inject]
        private void Construct(DesktopInput desktopInput)
        {
            _playerInput = desktopInput;
            Debug.Log("Пробросили зависимость");
        }

        private void Awake()
        {
            Play();
        }

        private void Update()
        {
            if (_playerInput.Pause)
            {
                SwitchState();
            }
        }

        public void Pause()
        {
            Time.timeScale = 0f;
            _cursorToggle.Enable();
            if (playerMovement != null)
            {
                playerMovement.enabled = false;
            }

        }

        public void Play()
        {
            Time.timeScale = 1f;
            _cursorToggle.Disable();

            if (playerMovement != null)
            {
                playerMovement.enabled = true;
            }

        }

        public void SwitchState()
        {
            if (IsPaused)
            {
                _pageSwitcher.Open(PageName.Stats);
                Play();
            }
            else
            {
                _pageSwitcher.Open(PageName.Pause);
                Pause();
            }

            IsPaused = !IsPaused;
        }
    }
}
