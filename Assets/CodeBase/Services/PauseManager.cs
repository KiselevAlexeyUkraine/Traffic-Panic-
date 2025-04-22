using Codebase.Components.Player;
using Codebase.Services.Inputs;
using Codebase.Components.Ui.Pages;
using UnityEngine;

namespace Codebase.Services

{
    public class PauseManager : MonoBehaviour
    {
        [SerializeField]
        private PageSwitcher _pageSwitcher;

        private CursorToggle _cursorToggle = new();

        public bool IsPaused { get; private set; }

        [SerializeField] private PlayerMovement playerMovement;

        private IInput _desktopInput;

        private void Awake()
        {
            Play();
        }

        private void Start()
        {
            if (SceneSwitcher.Instance.CurrentScene == 2)
            {
                Pause();
            }
        }

        private void Update()
        {
            if (_desktopInput.Pause)
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
