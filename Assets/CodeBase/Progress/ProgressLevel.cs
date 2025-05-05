using UnityEngine;
using System;

namespace Codebase.Progress
{
    public class ProgressLevel : MonoBehaviour
    {
        public float Progress { get; private set; }
        public event Action OnProgressComplete;

        private float _zStart;
        private bool _isRunning;

        private void OnEnable()
        {
            _zStart = transform.position.z;
            _isRunning = true;
            Progress = 0f;
        }

        private void Update()
        {
            if (!_isRunning) return;

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
