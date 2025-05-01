using UnityEngine;
using UnityEngine.UI;
using Codebase.Services;

namespace Codebase.UI
{
    public class SkillSelector : MonoBehaviour
    {
        [SerializeField] private Button armorButton;
        [SerializeField] private Button magnetButton;
        [SerializeField] private Button nitroButton;

        private void Awake()
        {
            armorButton.onClick.AddListener(() => SelectSkill(SkillSelectorPersistent.SkillType.Armor));
            magnetButton.onClick.AddListener(() => SelectSkill(SkillSelectorPersistent.SkillType.Magnet));
            nitroButton.onClick.AddListener(() => SelectSkill(SkillSelectorPersistent.SkillType.Nitro));
        }

        private void Start()
        {
            UpdateButtonStates();
        }

        private void SelectSkill(SkillSelectorPersistent.SkillType skill)
        {
            SkillSelectorPersistent.Instance.SelectSkill(skill);
            UpdateButtonStates();
        }

        private void UpdateButtonStates()
        {
            var selected = SkillSelectorPersistent.Instance.SelectedSkill;

            armorButton.interactable = selected != SkillSelectorPersistent.SkillType.Armor;
            magnetButton.interactable = selected != SkillSelectorPersistent.SkillType.Magnet;
            nitroButton.interactable = selected != SkillSelectorPersistent.SkillType.Nitro;
        }
    }
}
