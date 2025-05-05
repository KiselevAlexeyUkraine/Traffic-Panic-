using UnityEngine;

namespace Codebase.Services
{
    public class SkillSelectorPersistent : MonoBehaviour
    {
        public enum SkillType
        {
           
            Armor,
            Magnet,
            Nitro
        }

        public static SkillSelectorPersistent Instance { get; private set; }

        public SkillType SelectedSkill { get; private set; } = SkillType.Nitro;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadSelectedSkill();
        }

        public void SelectSkill(SkillType skill)
        {
            SelectedSkill = skill;
            PlayerPrefs.SetInt("SelectedSkill", (int)skill);
            PlayerPrefs.Save();
            Debug.Log("[SkillSelector] Selected: " + skill);
        }

        private void LoadSelectedSkill()
        {
            SelectedSkill = (SkillType)PlayerPrefs.GetInt("SelectedSkill", 0);
        }
    }
}
