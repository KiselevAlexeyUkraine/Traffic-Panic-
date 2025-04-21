using UnityEngine;
using UnityEngine.SceneManagement;

namespace Codebase.Services
{
    public class SceneSwitcher : MonoBehaviour
    {
        public static SceneSwitcher Instance;

        public int CurrentScene { get => SceneManager.GetActiveScene().buildIndex; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

        }
        private void Start()
        {
            if (SceneManager.GetActiveScene().buildIndex == 0)
            {
                SceneManager.LoadScene(1);
            }
        }

        public void LoadScene(int sceneIndex)
        {
            SceneManager.LoadScene(sceneIndex);
        }

        public void LoadNextScene()
        {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            int nextSceneIndex = currentSceneIndex + 1;

            if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(nextSceneIndex);
            }
            else
            {
                Debug.Log("Ёто последн€€ сцена в сборке.");
            }
        }

        public void LoadPreviousScene()
        {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            int previousSceneIndex = currentSceneIndex - 1;

            if (previousSceneIndex >= 0)
            {
                SceneManager.LoadScene(previousSceneIndex);
            }
            else
            {
                Debug.Log("Ёто перва€ сцена в сборке.");
            }
        }

        public void RestartCurrentScene()
        {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(currentSceneIndex);
        }

        public void ExitGame()
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}
