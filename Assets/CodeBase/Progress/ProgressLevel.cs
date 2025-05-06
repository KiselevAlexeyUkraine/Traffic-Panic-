using UnityEngine;
using System;
using Codebase.Components.Level;

namespace Codebase.Progress
{
    public class ProgressLevel : MonoBehaviour
    {
        [SerializeField] private float targetDistance = 1000f;
        [SerializeField] private Generator generator;

        public float Progress { get; private set; }
        public event Action OnProgressComplete;

        private float _totalDistance;
        private bool _isRunning;

        private void OnEnable()
        {
            _totalDistance = 0f;
            _isRunning = true;
            Progress = 0f;

            ValidateReferences();
        }

        private void ValidateReferences()
        {
            if (generator == null)
                Debug.LogError("Generator is not assigned in ProgressLevel!");
        }

        private void Update()
        {
            if (!_isRunning || generator == null) return;

            if (!generator.IsFirstLevel || generator._conditionsMet)
            {
                float speed = GetGeneratorSpeed();
                _totalDistance += speed * Time.deltaTime;
                Progress = Mathf.Clamp01(_totalDistance / targetDistance);

                if (Progress >= 1f)
                {
                    _isRunning = false;
                    OnProgressComplete?.Invoke();
                }
            }
        }

        private float GetGeneratorSpeed()
        {
            return generator.GetCurrentSpeed();
        }
    }
}