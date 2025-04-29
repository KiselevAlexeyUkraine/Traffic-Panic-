using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Codebase.Storage;
using Codebase.Services;
using Codebase.Components.Ui;

namespace Codebase.UI
{
    public class SkillUpgradeUI : MonoBehaviour
    {
        [System.Serializable]
        private enum SkillType { Armor, Magnet, Other }

        [System.Serializable]
        private class SkillUpgrade
        {
            public Button upgradeButton;
            public TMP_Text costText;
            public Slider progressSlider;
            public float baseDuration = 4f;
            public float durationIncrement = 2f;
            public int baseCost = 10;
            public int costIncrement = 10;
            public SkillType skillType;

            private string SkillKey => skillType.ToString();

            public int Level
            {
                get => PlayerPrefs.GetInt(SkillKey + "_Level", 0);
                set => PlayerPrefs.SetInt(SkillKey + "_Level", value);
            }

            public float Duration => baseDuration + durationIncrement * Level;
            public int UpgradeCost => baseCost + costIncrement * Level;

            public void Save() => PlayerPrefs.Save();
            public string Key => SkillKey;
        }

        [SerializeField] private SkillUpgrade[] skills;

        [SerializeField] private UiMainMenuCoins uiMainMenuCoins;

        private void Start()
        {
            foreach (var skill in skills)
            {
                int capturedIndex = System.Array.IndexOf(skills, skill);
                skill.upgradeButton.onClick.AddListener(() => OnUpgradeSkill(capturedIndex));
                UpdateUI(capturedIndex);
            }
        }

        private void OnUpgradeSkill(int index)
        {
            SkillUpgrade skill = skills[index];
            int coins = CoinStorage.GetCoins();

            if (coins >= skill.UpgradeCost)
            {
                CoinStorage.AddCoins(-skill.UpgradeCost);
                skill.Level++;
                skill.Save();
                uiMainMenuCoins.UpdateCoinsTotall();
                UpdateUI(index);
                ApplySkillUpgrade(skill);
            }
        }

        private void UpdateUI(int index)
        {
            SkillUpgrade skill = skills[index];
            skill.costText.text = $"{skill.UpgradeCost}";
            skill.progressSlider.value = skill.Level / 10f;
        }

        private void ApplySkillUpgrade(SkillUpgrade skill)
        {
            SkillProgressService.Instance.SetSkillData(skill.Key, skill.Duration);
        }
    }
}
