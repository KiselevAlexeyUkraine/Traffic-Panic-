using UnityEngine.SceneManagement;

namespace Codebase.Services
{
    public class SceneService
    {
        public int SceneToLoad { private get; set; } = DEFAULT_START_SCENE;
        private const int DEFAULT_START_SCENE = 2;

        public int CurrentScene => SceneManager.GetActiveScene().buildIndex;

        public void Load()
        {
            SceneManager.LoadScene(SceneToLoad);
            SceneToLoad = DEFAULT_START_SCENE;
        }

        public void Load(int index)
        {
            SceneManager.LoadScene(index);
        }

        public void Load(string name)
        {
            SceneManager.LoadScene(name);
        }

        public void LoadNextScene()
        {
            int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

            if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(nextSceneIndex);
            }
            else
            {
                SceneManager.LoadScene(1);
                UnityEngine.Debug.Log("Нажата клавиша вправо");
            }
        }

        public void LoadPreviousScene()
        {
            int previousSceneIndex = SceneManager.GetActiveScene().buildIndex - 1;

            if (previousSceneIndex >= 0)
            {
                SceneManager.LoadScene(previousSceneIndex);
            }
            else
            {
                UnityEngine.Debug.Log("Это первая сцена в сборке.");
            }
        }

        public void RestartCurrentScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void ExitGame()
        {
            UnityEngine.Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }

        public int GetCurrentScene()
        {
            return SceneManager.GetActiveScene().buildIndex;
        }

        public int GetScenesCount()
        {
            return SceneManager.sceneCountInBuildSettings;
        }
    }
}
