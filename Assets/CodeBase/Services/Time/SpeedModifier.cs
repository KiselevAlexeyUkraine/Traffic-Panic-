using UnityEngine;
using Codebase.Services.Inputs;
using Zenject;

namespace Codebase.Services.Time
{
    public class SpeedModifier : MonoBehaviour
    {
        [SerializeField] private float boostScale = 2f;
        [SerializeField] private float brakeScale = 0.5f;
        [SerializeField] private float effectDuration = 2f;

        private IInput _playerInput;
        private float _timer;
        private bool _effectActive;

        [Inject]
        private void Construct(DesktopInput desktopInput)
        {
            _playerInput = desktopInput;
        }

        private void Update()
        {
            if (!_effectActive)
            {
                if (_playerInput.Boost)
                {
                    StartTimeEffect(boostScale);
                }
                else if (_playerInput.Drag)
                {
                    StartTimeEffect(brakeScale);
                }
            }

            if (_effectActive)
            {
                _timer -= UnityEngine.Time.unscaledDeltaTime;
                if (_timer <= 0f)
                {
                    UnityEngine.Time.timeScale = 1f;
                    _effectActive = false;
                }
            }
        }

        private void StartTimeEffect(float scale)
        {
            UnityEngine.Time.timeScale = scale;
            _timer = effectDuration;
            _effectActive = true;
        }
    }
}
