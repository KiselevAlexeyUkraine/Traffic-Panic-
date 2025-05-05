using UnityEngine;
using System;
using Codebase.Components.Level;

namespace Codebase.Progress
{
    public class ProgressTimer : MonoBehaviour
    {
        [SerializeField] private float duration = 60f;
        [SerializeField] private Generator generator; // Ссылка на Generator

        public float Progress { get; private set; }
        public event Action OnProgressComplete;
        public static Action OnProgressCompleteStatic;
        private float _elapsed;
        private bool _isRunning;

        private void OnEnable()
        {
            OnProgressCompleteStatic = OnProgressComplete;
            _elapsed = 0f;
            _isRunning = true;

            if (generator == null)
            {
                Debug.LogError("Generator is not assigned in ProgressTimer!");
            }
        }

        private void Update()
        {
            if (!_isRunning || generator == null) return;

            // Прогресс увеличивается, если IsFirstLevel == false или условия выполнены
            if (!generator.IsFirstLevel || generator.ConditionsMet)
            {
                _elapsed += Time.deltaTime;
                Progress = Mathf.Clamp01(_elapsed / duration);

                if (Progress >= 1f)
                {
                    _isRunning = false;
                    OnProgressComplete?.Invoke();
                }
            }
        }
    }
}