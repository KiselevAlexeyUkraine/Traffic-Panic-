using UnityEngine;
using System;

namespace Codebase.Progress
{
    public class ProgressTimer : MonoBehaviour
    {
        [SerializeField] private float duration = 60f;

        public float Progress { get; private set; }
        public event Action OnProgressComplete;

        private float _elapsed;
        private bool _isRunning;

        private void OnEnable()
        {
            _elapsed = 0f;
            _isRunning = true;
        }

        private void Update()
        {
            if (!_isRunning) return;

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