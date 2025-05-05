using UnityEngine;
using Codebase.Services.Inputs;
using Zenject;
using Codebase.Components.Player;
using Codebase.Components.Level;
using System;


    public class SpeedModifier : MonoBehaviour
    {
        [SerializeField] private float boostScale = 2f;
        [SerializeField] private float brakeScale = 0.5f;
        [SerializeField] private float effectDuration = 2f;
        [SerializeField] private float abilityUnlockDelay = 10f;
        [SerializeField] private Animator cameraAnimator;
        [SerializeField] private CameraShaker cameraShaker;
        [SerializeField] private float shakeInterval = 0.5f;
        [SerializeField] private PlayerMovement playerMovement;
        [SerializeField] private Generator generator;
        [SerializeField] private PlayerCollisionHandler playerCollisionHandler;
        [SerializeField] private bool isFirstLevel;

        public Action OnBoostUsed;
        public bool HasBoosted { get; private set; }
        public event Action<float, float> OnBoostSpeed;

        private IInput _playerInput;
        private float _timer;
        private bool _effectActive;
        private float _shakeTimer;
        private float _startTimer;
        private bool _abilitiesUnlocked;
        private bool _isBoosting;
        private const int REQUIRED_LANE_CHANGES = 3;

        [Inject]
        private void Construct(DesktopInput desktopInput)
        {
            _playerInput = desktopInput;
        }

        private void Awake()
        {
            if (playerMovement == null)
            {
                Debug.LogError("PlayerMovement is not assigned in SpeedModifier!");
            }
            if (generator == null)
            {
                Debug.LogError("Generator is not assigned in SpeedModifier!");
            }
            if (playerCollisionHandler == null)
            {
                Debug.LogError("PlayerCollisionHandler is not assigned in SpeedModifier!");
            }
        }

        private void OnEnable()
        {
            if (playerCollisionHandler != null)
            {
                playerCollisionHandler.OnNitroActivated += ResetOnNitro;
            }
        }

        private void OnDisable()
        {
            if (playerCollisionHandler != null)
            {
                playerCollisionHandler.OnNitroActivated -= ResetOnNitro;
            }
        }

        private void ResetOnNitro()
        {
            if (_effectActive)
            {
                ResetSpeed();
                Debug.Log("Nitro activated, resetting Boost/Brake effect");
            }
            _timer = 0f;
            _effectActive = false;
            _isBoosting = false;
            if (cameraAnimator != null) cameraAnimator.SetBool("IsBoosting", false);
        }

        private void Update()
        {
            // Пропускаем логику, если Nitro активно
            if (playerCollisionHandler != null && playerCollisionHandler.IsNitroActive)
            {
                return;
            }

            // Разблокировка способностей
            if (!_abilitiesUnlocked)
            {
                _startTimer += Time.deltaTime;
                if (_startTimer >= abilityUnlockDelay)
                {
                    _abilitiesUnlocked = true;
                    Debug.Log("Speed abilities unlocked!");
                }
            }

            // Обработка ввода Boost/Brake
            if (_abilitiesUnlocked)
            {
                if (_playerInput.Boost && CanBoost())
                {
                    if (!_effectActive)
                    {
                        StartSpeedEffect(boostScale);
                        if (cameraShaker != null) _shakeTimer = 0f;
                        if (!HasBoosted)
                        {
                            HasBoosted = true;
                            OnBoostUsed?.Invoke();
                            Debug.Log("Boost used!");
                        }
                    }
                    else if (_isBoosting)
                    {
                        _timer = Mathf.Max(_timer, effectDuration); // Обновляем таймер Boost
                        Debug.Log($"Updating Boost timer, new timer: {_timer}");
                    }
                    _isBoosting = true;
                    if (cameraAnimator != null) cameraAnimator.SetBool("IsBoosting", true);
                }
                else if (_playerInput.Drag && !_effectActive)
                {
                    StartSpeedEffect(brakeScale);
                    _isBoosting = false;
                    if (cameraAnimator != null) cameraAnimator.SetBool("IsBoosting", false);
                }
            }

            // Обновление активного эффекта
            if (_effectActive)
            {
                _shakeTimer -= Time.deltaTime;
                if (_shakeTimer <= 0f && _isBoosting)
                {
                    if (cameraShaker != null) cameraShaker.Shake();
                    _shakeTimer = shakeInterval;
                }

                _timer -= Time.deltaTime;
                if (_timer <= 0f)
                {
                    ResetSpeed();
                }
            }
        }

        private void StartSpeedEffect(float scale)
        {
            if (generator != null)
            {
                generator.SetSpeedMultiplier(scale);
                OnBoostSpeed?.Invoke(scale, effectDuration);
                Debug.Log($"Starting speed effect, multiplier: {scale}, duration: {effectDuration}");
            }
            _timer = effectDuration;
            _effectActive = true;
        }

        private void ResetSpeed()
        {
            if (generator != null)
            {
                generator.ResetSpeedMultiplier();
                OnBoostSpeed?.Invoke(1f, 0f);
                Debug.Log("Resetting speed multiplier to 1");
            }
            _effectActive = false;
            _isBoosting = false;
            if (cameraAnimator != null) cameraAnimator.SetBool("IsBoosting", false);
        }

        public void TriggerBoost(float scale)
        {
            if (!_abilitiesUnlocked)
            {
                Debug.Log("Abilities not yet unlocked!");
                return;
            }

            if (isFirstLevel)
            {
                if (!CanBoost())
                {
                    Debug.Log("Cannot boost: Complete 3 lane changes first!");
                    return;
                }
            }

            if (playerCollisionHandler != null && playerCollisionHandler.IsNitroActive)
            {
                Debug.Log("Cannot trigger boost: Nitro is active!");
                return;
            }

            StartSpeedEffect(scale);
            _shakeTimer = 0f;
            if (!HasBoosted)
            {
                HasBoosted = true;
                OnBoostUsed?.Invoke();
                Debug.Log("Boost used!");
            }
        }

        private bool CanBoost()
        {
            return playerMovement != null && playerMovement.LaneChangeCount >= REQUIRED_LANE_CHANGES;
        }
    }
