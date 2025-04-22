using Codebase.Services;
using Zenject;

namespace Codebase.Components.Ui.Pages.Menu
{
    public class StartPage : BasePage
    {
        //private SceneService _sceneService;

        //[Inject]
        //private void Construct(SceneService sceneService)
        //{
        //    _sceneService = sceneService;
        //}

        private void Start()
        {
            Opened += () => { SceneSwitcher.Instance.LoadScene(1); };
        }
    }
}