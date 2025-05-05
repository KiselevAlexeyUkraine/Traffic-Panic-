using UnityEngine;
using System;
using Codebase.Components.Level;
namespace Codebase.Progress
{
    public class ProgressLevel : MonoBehaviour
    {
        [SerializeField] private float duration = 60f;[SerializeField] 
        private Generator generator; // Reference to the Generator component

        public float Progress { get; private set; }
        public event Action OnProgressComplete;

        private float _elapsed;
        private float _zStart;
        private bool _isRunning;

        private void OnEnable()
        {
            _elapsed = 0f;
            _isRunning = true;
            Progress = 0f;
            _zStart = transform.position.z; // Initialize _zStart, though not used in time-based progress

            if (generator == null)
            {
                Debug.LogError("Generator is not assigned in ProgressLevel!");
            }
        }

        private void Update()
        {
            if (!_isRunning || generator == null) return;

            // ѕрогресс увеличиваетс€, если IsFirstLevel == false или услови€ выполнены
            if (!generator.IsFirstLevel || generator._conditionsMet)
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