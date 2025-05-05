using UnityEngine;
using Codebase.Services.Inputs;
using Zenject;
using Codebase.Components.Player;

namespace Codebase.Services.Time
{
    public class SpeedModifier : MonoBehaviour
    {
        [SerializeField] private float boostScale = 2f;
        [SerializeField] private float brakeScale = 0.5f;
        [SerializeField] private float effectDuration = 2f;
        [SerializeField] private float returnDuration = 1f; // Длительность возврата к нормальной скорости
        [SerializeField] private Animator cameraAnimator;
        [SerializeField] private CameraShaker cameraShaker;
        [SerializeField] private float shakeInterval = 0.5f;
        [SerializeField] private float abilityUnlockDelay = 10f;

        private IInput _playerInput;
        private float _timer;
        private bool _effectActive;
        private bool _isBoosting;
        private float _shakeTimer;
        private float _startTimer;
        private bool _abilitiesUnlocked;
        private bool _isReturning; // Флаг для состояния возврата
        private float _returnTimer; // Таймер для возврата
        private float _targetTimeScale; // Целевая скорость времени для интерполяции
        private float _startTimeScale; // Начальная скорость времени для интерполяции

        [Inject]
        private void Construct(DesktopInput desktopInput)
        {
            _playerInput = desktopInput;
        }

        private void Update()
        {
            // Обновляем таймер разблокировки способностей
            if (!_abilitiesUnlocked)
            {
                _startTimer += UnityEngine.Time.unscaledDeltaTime;
                if (_startTimer >= abilityUnlockDelay)
                {
                    _abilitiesUnlocked = true;
                    Debug.Log("Speed abilities unlocked!");
                }
            }

            // Обрабатываем ввод, если способности разблокированы
            if (_abilitiesUnlocked)
            {
                if (_playerInput.Boost)
                {
                    if (!_effectActive || UnityEngine.Time.timeScale != boostScale)
                    {
                        StartTimeEffect(boostScale);
                        if (cameraShaker != null) _shakeTimer = 0f;
                    }
                    else
                    {
                        _timer = effectDuration;
                    }
                    _isBoosting = true;
                    _isReturning = false; // Отключаем возврат при активном ускорении
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
                    StartTimeEffect(brakeScale);
                    _isBoosting = false;
                    _isReturning = false; // Отключаем возврат при активном замедлении
                    if (cameraAnimator != null) cameraAnimator.SetBool("IsBoosting", false);
                }
            }

            // Обновляем таймер эффекта
            if (_effectActive)
            {
                _timer -= UnityEngine.Time.unscaledDeltaTime;
                if (_timer <= 0f)
                {
                    StartReturnToNormalSpeed();
                }
            }

            // Обрабатываем плавный возврат к нормальной скорости
            if (_isReturning)
            {
                _returnTimer += UnityEngine.Time.unscaledDeltaTime;
                float t = _returnTimer / returnDuration;
                UnityEngine.Time.timeScale = Mathf.Lerp(_startTimeScale, 1f, t);

                if (_returnTimer >= returnDuration)
                {
                    UnityEngine.Time.timeScale = 1f;
                    _isReturning = false;
                    _effectActive = false;

                    if (!_playerInput.Boost)
                    {
                        _isBoosting = false;
                        if (cameraAnimator != null) cameraAnimator.SetBool("IsBoosting", false);
                    }
                }
            }

            // Сбрасываем состояние камеры, если эффект не активен и нет ускорения
            if (!_effectActive && !_isReturning && !_playerInput.Boost)
            {
                _isBoosting = false;
                if (cameraAnimator != null) cameraAnimator.SetBool("IsBoosting", false);
            }
        }

        private void StartTimeEffect(float scale)
        {
            UnityEngine.Time.timeScale = scale;
            _timer = effectDuration;
            _effectActive = true;
            _isReturning = false; // Отключаем возврат при старте нового эффекта
        }

        private void StartReturnToNormalSpeed()
        {
            _startTimeScale = UnityEngine.Time.timeScale;
            _returnTimer = 0f;
            _isReturning = true;
            _effectActive = false;
        }

        public void TriggerBoost(float scale)
        {
            if (!_abilitiesUnlocked)
            {
                Debug.Log("Abilities not yet unlocked!");
                return;
            }

            if (!_effectActive || UnityEngine.Time.timeScale != scale)
            {
                StartTimeEffect(scale);
                _shakeTimer = 0f;
            }
            else
            {
                _timer = effectDuration;
            }
            _isBoosting = true;
            _isReturning = false;
            if (cameraAnimator != null) cameraAnimator.SetBool("IsBoosting", true);
        }
    }
}