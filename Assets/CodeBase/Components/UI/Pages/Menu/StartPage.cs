using Codebase.Services;
using Zenject;

namespace Codebase.Components.Ui.Pages.Menu
{
    public class StartPage : BasePage
    {
        private SceneService _sceneService;

        [Inject]
        private void Construct(SceneService sceneService)
        {
            _sceneService = sceneService;
        }

        private void Awake()
        {
            Opened += () => { _sceneService.Load(); };
        }
    }
}