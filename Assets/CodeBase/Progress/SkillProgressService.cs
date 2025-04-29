using UnityEngine;
using System.Collections.Generic;

namespace Codebase.Services
{
    public class SkillProgressService : MonoBehaviour
    {
        public static SkillProgressService Instance { get; private set; }

        private readonly Dictionary<string, float> skillDurations = new();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public void SetSkillData(string skillKey, float duration)
        {
            skillDurations[skillKey] = duration;
        }

        public float GetSkillDuration(string skillKey, float defaultValue)
        {
            Debug.Log("Стринг ключ сохранения =  " + skillKey);
            float result = skillDurations.TryGetValue(skillKey, out var value) ? value : defaultValue;
            Debug.Log("Значение сохранения =  " + result);
            return result;
        }
    }
}
