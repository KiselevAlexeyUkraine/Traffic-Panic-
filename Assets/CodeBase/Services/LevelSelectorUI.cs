using UnityEngine;
using UnityEngine.UI;

namespace Codebase.Components.Ui
{
    public class LevelSelectorUI : MonoBehaviour
    {
        [SerializeField] private Button level1Button;
        [SerializeField] private Button level2Button;
        [SerializeField] private Button level3Button;
        [SerializeField] private Text selectedLevelText;

        private const string SelectedLevelKey = "SelectedLevelIndex";
        private int _selectedLevel = 2;

        private void Awake()
        {
            level1Button.onClick.AddListener(() => SelectLevel(2));
            level2Button.onClick.AddListener(() => SelectLevel(3));
            level3Button.onClick.AddListener(() => SelectLevel(4));

            _selectedLevel = PlayerPrefs.GetInt(SelectedLevelKey, 2);
            UpdateUI();
        }

        private void SelectLevel(int sceneIndex)
        {
            _selectedLevel = sceneIndex;
            PlayerPrefs.SetInt(SelectedLevelKey, _selectedLevel);
            PlayerPrefs.Save();
            Debug.Log("Level Selected: " + _selectedLevel);
            UpdateUI();
        }

        private void UpdateUI()
        {
            selectedLevelText.text = "Выбран уровень: " + (_selectedLevel - 1);

            level1Button.interactable = _selectedLevel != 2;
            level2Button.interactable = _selectedLevel != 3;
            level3Button.interactable = _selectedLevel != 4;
        }
    }
}
