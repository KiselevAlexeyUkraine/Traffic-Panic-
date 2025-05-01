using Codebase.Services;
using Zenject;
using UnityEngine;

namespace Codebase.Components.Ui.Pages.Menu
{
    public class StartPage : BasePage
    {
        private SceneService _sceneService;
        private const string SelectedLevelKey = "SelectedLevelIndex";

        [Inject]
        private void Construct(SceneService sceneService)
        {
            _sceneService = sceneService;
        }

        private void Awake()
        {
            Opened += () =>
            {
                int selectedScene = PlayerPrefs.GetInt(SelectedLevelKey, 2);
                _sceneService.SceneToLoad = selectedScene;
                _sceneService.Load();
            };
        }
    }
}
