using Codebase.Services;
using UnityEngine;
using Zenject;

namespace Codebase.Components.Ui.Pages.Menu
{
    public class MenuStart : BasePage
    {
        
        private CursorToggle cursorToggle = new();

        //[Inject]
        //private void Construct(PauseService pauseService)
        //{
        //    _pauseService = pauseService;
        //}

        private void Awake()
        {
            Time.timeScale = 1f;
            cursorToggle.Enable();
            Opened += () => { PageSwitcher.Open(PageName.Menu).Forget(); };
        }
    }
}