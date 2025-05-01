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
        private enum SkillType { Armor, Magnet, Nitro }

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
            public int maxLevel = 10;
            public SkillType skillType;

            private string SkillKey => skillType.ToString();

            public int Level
            {
                get => PlayerPrefs.GetInt(SkillKey + "_Level", 0);
                set => PlayerPrefs.SetInt(SkillKey + "_Level", value);
            }

            public float Duration => baseDuration + durationIncrement * Level;
            public int UpgradeCost => baseCost + costIncrement * Level;
            public bool IsMaxLevel => Level >= maxLevel;

            public void Save() => PlayerPrefs.Save();
            public void Reset() => PlayerPrefs.DeleteKey(SkillKey + "_Level");
            public string Key => SkillKey;
        }

        [SerializeField] private SkillUpgrade[] skills;
        [SerializeField] private UiMainMenuCoins coinDisplay;
        [SerializeField] private Button resetButton;

        private void Start()
        {
            foreach (var skill in skills)
            {
                int capturedIndex = System.Array.IndexOf(skills, skill);
                skill.upgradeButton.onClick.AddListener(() => OnUpgradeSkill(capturedIndex));
                ApplySkillUpgrade(skill);
                UpdateUI(capturedIndex);
            }

            resetButton?.onClick.AddListener(ResetAllSkills);
            coinDisplay?.UpdateCoinsTotall();
        }

        private void OnUpgradeSkill(int index)
        {
            SkillUpgrade skill = skills[index];
            int coins = CoinStorage.GetCoins();

            if (coins >= skill.UpgradeCost && !skill.IsMaxLevel)
            {
                CoinStorage.AddCoins(-skill.UpgradeCost);
                skill.Level++;
                skill.Save();

                UpdateUI(index);
                ApplySkillUpgrade(skill);
                coinDisplay?.UpdateCoinsTotall();
            }
        }

        private void UpdateUI(int index)
        {
            SkillUpgrade skill = skills[index];

            if (skill.IsMaxLevel)
            {
                skill.costText.text = "MAX";
                skill.upgradeButton.interactable = false;
            }
            else
            {
                skill.costText.text = $"{skill.UpgradeCost}";
                skill.upgradeButton.interactable = true;
            }

            skill.progressSlider.value = skill.Level / (float)skill.maxLevel;
        }

        private void ApplySkillUpgrade(SkillUpgrade skill)
        {
            SkillProgressService.Instance.SetSkillData(skill.Key, skill.Duration);
        }

        private void ResetAllSkills()
        {
            foreach (var skill in skills)
            {
                skill.Reset();
                UpdateUI(System.Array.IndexOf(skills, skill));
                ApplySkillUpgrade(skill);
            }
            PlayerPrefs.Save();
            coinDisplay?.UpdateCoinsTotall();
        }
    }
}
