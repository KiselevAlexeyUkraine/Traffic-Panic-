using UnityEngine;
using System;
using Codebase.Components.Level;

namespace Codebase.Progress
{
    public class ProgressLevel : MonoBehaviour
    {
        [SerializeField] private float duration = 60f;

        public float Progress { get; private set; }
        public event Action OnProgressComplete;

        private float _zStart;
        private bool _isRunning;

        private void OnEnable()
        {
            OnProgressCompleteStatic = OnProgressComplete;
            _elapsed = 0f;
            _isRunning = true;
            Progress = 0f;

            if (generator == null)
            {
                Debug.LogError("Generator is not assigned in ProgressTimer!");
            }
        }

        private void Update()
        {
            if (!_isRunning || generator == null) return;

            // ѕрогресс увеличиваетс€, если IsFirstLevel == false или услови€ выполнены
            if (!generator.IsFirstLevel || generator.ConditionsMet)
            {
                _elapsed += Time.deltaTime;
                Progress = Mathf.Clamp01(_elapsed / duration);
            float currentZ = transform.position.z;
            float distanceCovered = _zStart - currentZ;
            Progress = Mathf.Clamp01(distanceCovered / _zStart);

            if (currentZ <= 0f)
            {
                _isRunning = false;
                Progress = 1f;
                OnProgressComplete?.Invoke();
            }
        }
    }
}
                if (Progress >= 1f)
                {
                    _isRunning = false;
                    OnProgressComplete?.Invoke();
                }
            }
        }
    }
}