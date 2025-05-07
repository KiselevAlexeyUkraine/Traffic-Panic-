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
            Instance = this;
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
            LoadSkills();
        }

        public void SetSkillData(string skillKey, float duration)
        {
            Debug.Log($"[SkillProgressService] Set {skillKey} duration to {duration}");
            skillDurations[skillKey] = duration;
            PlayerPrefs.SetFloat(skillKey + "_Duration", duration);
            PlayerPrefs.Save();
        }

        public float GetSkillDuration(string skillKey, float defaultValue)
        {
            if (skillDurations.TryGetValue(skillKey, out var value))
            {
                Debug.Log($"[SkillProgressService] Loaded cached {skillKey} duration = {value}");
                return value;
            }

            float loaded = PlayerPrefs.GetFloat(skillKey + "_Duration", defaultValue);
            skillDurations[skillKey] = loaded;
            Debug.Log($"[SkillProgressService] Loaded saved {skillKey} duration = {loaded}");
            return loaded;
        }

        private void LoadSkills()
        {
            foreach (var key in new[] { "Armor", "Magnet", "Nitro" })
            {
                float duration = PlayerPrefs.GetFloat(key + "_Duration", 0f);
                skillDurations[key] = duration;
                Debug.Log($"[SkillProgressService] Init {key} with duration {duration}");
            }
        }
    }
}
