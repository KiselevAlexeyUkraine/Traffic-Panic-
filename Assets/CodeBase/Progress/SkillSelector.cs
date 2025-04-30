using UnityEngine;
using UnityEngine.UI;
using Codebase.Services;

namespace Codebase.UI
{
    public class SkillSelector : MonoBehaviour
    {
        [SerializeField] private Button armorButton;
        [SerializeField] private Button magnetButton;
        [SerializeField] private Button otherButton;

        private void Awake()
        {
            armorButton.onClick.AddListener(() => SelectSkill(SkillSelectorPersistent.SkillType.Armor));
            magnetButton.onClick.AddListener(() => SelectSkill(SkillSelectorPersistent.SkillType.Magnet));
            otherButton.onClick.AddListener(() => SelectSkill(SkillSelectorPersistent.SkillType.Other));
        }

        private void SelectSkill(SkillSelectorPersistent.SkillType skill)
        {
            SkillSelectorPersistent.Instance.SelectSkill(skill);
        }
    }
}
