using UnityEngine;
using Unity.Cinemachine;
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
        [SerializeField] private Animator cameraAnimator; // Ссылка на Animator камеры
        [SerializeField] private CameraShaker cameraShaker; // Ссылка на CameraShaker
        [SerializeField] private float shakeInterval = 0.5f; // Интервал между трясками
        [SerializeField] private float abilityUnlockDelay = 10f; // Delay before abilities are unlocked

        private IInput _playerInput;
        private float _timer;
        private bool _effectActive;
        private bool _isBoosting; // Флаг для отслеживания состояния ускорения
        private float _shakeTimer; // Таймер для интервала тряски
        private float _startTimer; // Timer for tracking time since game start
        private bool _abilitiesUnlocked; // Flag to check if abilities are unlocked

        [Inject]
        private void Construct(DesktopInput desktopInput)
        {
            _playerInput = desktopInput;
        }

        private void Update()
        {
            // Update start timer until abilities are unlocked
            if (!_abilitiesUnlocked)
            {
                _startTimer += UnityEngine.Time.unscaledDeltaTime;
                if (_startTimer >= abilityUnlockDelay)
                {
                    _abilitiesUnlocked = true;
                    Debug.Log("Speed abilities unlocked!");
                }
            }

            // Process inputs only if abilities are unlocked
            if (_abilitiesUnlocked)
            {
                // Проверяем, нажата ли кнопка ускорения
                if (_playerInput.Boost)
                {
                    // Если ускорение активно, сбрасываем таймер, чтобы эффект продолжался
                    if (!_effectActive || UnityEngine.Time.timeScale != boostScale)
                    {
                        StartTimeEffect(boostScale);
                        if (cameraShaker != null) _shakeTimer = 0f; // Сбрасываем таймер тряски при старте
                    }
                    else
                    {
                        _timer = effectDuration; // Сбрасываем таймер при каждом нажатии
                    }
                    _isBoosting = true;

                    if (cameraAnimator != null) cameraAnimator.SetBool("IsBoosting", true); // Активируем анимацию опускания

                    // Обрабатываем тряску камеры с интервалом
                    _shakeTimer -= UnityEngine.Time.unscaledDeltaTime;
                    if (_shakeTimer <= 0f)
                    {
                        if (cameraShaker != null) cameraShaker.Shake(); // Вызываем тряску
                        _shakeTimer = shakeInterval; // Сбрасываем таймер тряски
                    }
                }
                // Проверяем, нажата ли кнопка торможения
                else if (_playerInput.Drag)
                {
                    StartTimeEffect(brakeScale);
                    _isBoosting = false;
                    if (cameraAnimator != null) cameraAnimator.SetBool("IsBoosting", false); // Возвращаем камеру в исходное состояние
                }
            }

            // Обновляем таймер, если эффект активен
            if (_effectActive)
            {
                _timer -= UnityEngine.Time.unscaledDeltaTime;
                if (_timer <= 0f)
                {
                    UnityEngine.Time.timeScale = 1f; // Возвращаем нормальную скорость
                    _effectActive = false;

                    // Если игрок больше не нажимает ускорение, возвращаем камеру
                    if (!_playerInput.Boost)
                    {
                        _isBoosting = false;
                        if (cameraAnimator != null) cameraAnimator.SetBool("IsBoosting", false);
                    }
                }
            }

            // Если эффект не активен и игрок не нажимает ускорение, сбрасываем состояние камеры
            if (!_effectActive && !_playerInput.Boost)
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
        }

        // Публичный метод для вызова ускорения из других скриптов
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
                _shakeTimer = 0f; // Сбрасываем таймер тряски
            }
            else
            {
                _timer = effectDuration; // Продлеваем эффект
            }
            _isBoosting = true;
            if (cameraAnimator != null) cameraAnimator.SetBool("IsBoosting", true);
        }
    }
}