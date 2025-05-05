// SpeedModifier.cs
using UnityEngine;
using Codebase.Services.Inputs;
using Zenject;
using Codebase.Components.Player;
using Codebase.Components.Level;

namespace Codebase.Services.Time
{
    public class SpeedModifier : MonoBehaviour
    {
        [SerializeField] private float boostScale = 2f;
        [SerializeField] private float brakeScale = 0.5f;
        [SerializeField] private float effectDuration = 2f;
        [SerializeField] private float abilityUnlockDelay = 10f;
        [SerializeField] private Animator cameraAnimator;
        [SerializeField] private CameraShaker cameraShaker;
        [SerializeField] private float shakeInterval = 0.5f;

        public event System.Action<float, float> OnBoostSpeed;

        private IInput _playerInput;
        private float _timer;
        private bool _effectActive;
        private bool _isBoosting;
        private float _shakeTimer;
        private float _startTimer;
        private bool _abilitiesUnlocked;

        [Inject]
        private void Construct(DesktopInput desktopInput)
        {
            _playerInput = desktopInput;
        }

        private void Update()
        {
            if (!_abilitiesUnlocked)
            {
                _startTimer += UnityEngine.Time.unscaledDeltaTime;
                if (_startTimer >= abilityUnlockDelay)
                {
                    _abilitiesUnlocked = true;
                    Debug.Log("Speed abilities unlocked!");
                }
            }

            if (_abilitiesUnlocked)
            {
                if (_playerInput.Boost)
                {
                    if (!_effectActive)
                    {
                        TriggerBoost(boostScale);
                    }
                    else
                    {
                        _timer = effectDuration;
                    }

                    _isBoosting = true;
                    if (cameraAnimator != null) cameraAnimator.SetBool("IsBoosting", true);

                    _shakeTimer -= UnityEngine.Time.unscaledDeltaTime;
                    if (_shakeTimer <= 0f)
                    {
                        if (cameraShaker != null) cameraShaker.Shake();
                        _shakeTimer = shakeInterval;
                    }
                }
                else if (_playerInput.Drag)
                {
                    TriggerBoost(brakeScale);
                    _isBoosting = false;
                    if (cameraAnimator != null) cameraAnimator.SetBool("IsBoosting", false);
                }
            }

            if (_effectActive)
            {
                _timer -= UnityEngine.Time.unscaledDeltaTime;
                if (_timer <= 0f)
                {
                    _effectActive = false;
                    _isBoosting = false;
                    if (cameraAnimator != null) cameraAnimator.SetBool("IsBoosting", false);
                    OnBoostSpeed?.Invoke(1f, 0f); // Reset multiplier
                }
            }

            if (!_effectActive && !_playerInput.Boost)
            {
                _isBoosting = false;
                if (cameraAnimator != null) cameraAnimator.SetBool("IsBoosting", false);
            }
        }

        public void TriggerBoost(float scale)
        {
            if (!_abilitiesUnlocked)
            {
                Debug.Log("Abilities not yet unlocked!");
                return;
            }

            _timer = effectDuration;
            _effectActive = true;
            _shakeTimer = 0f;
            _isBoosting = true;
            if (cameraAnimator != null) cameraAnimator.SetBool("IsBoosting", true);
            OnBoostSpeed?.Invoke(scale, effectDuration);
        }
    }
}