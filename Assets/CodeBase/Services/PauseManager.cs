using Codebase.Components.Player;
using Codebase.Services.Inputs;
using Codebase.Components.Ui.Pages;
using UnityEngine;
using Zenject;
using Codebase.Services.Time;

namespace Codebase.Services
{
    public class PauseManager : MonoBehaviour
    {
        [SerializeField] private PageSwitcher _pageSwitcher;
        [SerializeField] private PlayerMovement playerMovement;
        [SerializeField] private SpeedModifier speedModifier;

        public bool IsPaused { get; private set; }

        private IInput _playerInput;
        private CursorToggle _cursorToggle;

        [Inject]
        private void Construct(DesktopInput desktopInput, CursorToggle cursorToggle)
        {
            _playerInput = desktopInput;
            _cursorToggle = cursorToggle;
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
            UnityEngine.Time.timeScale = 0f;
            _cursorToggle.Enable();
            playerMovement.enabled = false;
            speedModifier.enabled = false;
        }

        public void Play()
        {
            UnityEngine.Time.timeScale = 1f;
            _cursorToggle.Disable();
            playerMovement.enabled = true;
            speedModifier.enabled = true;
        }

        public void SwitchState()
        {
            if (IsPaused)
            {
                _pageSwitcher.Open(PageName.Stats).Forget();
                Play();
            }
            else
            {
                _pageSwitcher.Open(PageName.Pause).Forget();
                Pause();
            }

            IsPaused = !IsPaused;
        }
    }
}
