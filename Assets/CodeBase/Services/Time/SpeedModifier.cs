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
        [SerializeField] private float returnDuration = 1f; // ������������ �������� � ���������� ��������
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
        private bool _isReturning; // ���� ��� ��������� ��������
        private float _returnTimer; // ������ ��� ��������
        private float _targetTimeScale; // ������� �������� ������� ��� ������������
        private float _startTimeScale; // ��������� �������� ������� ��� ������������

        [Inject]
        private void Construct(DesktopInput desktopInput)
        {
            _playerInput = desktopInput;
        }

        private void Update()
        {
            // ��������� ������ ������������� ������������
            if (!_abilitiesUnlocked)
            {
                _startTimer += UnityEngine.Time.unscaledDeltaTime;
                if (_startTimer >= abilityUnlockDelay)
                {
                    _abilitiesUnlocked = true;
                    Debug.Log("Speed abilities unlocked!");
                }
            }

            // ������������ ����, ���� ����������� ��������������
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
                    _isReturning = false; // ��������� ������� ��� �������� ���������
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
                    _isReturning = false; // ��������� ������� ��� �������� ����������
                    if (cameraAnimator != null) cameraAnimator.SetBool("IsBoosting", false);
                }
            }

            // ��������� ������ �������
            if (_effectActive)
            {
                _timer -= UnityEngine.Time.unscaledDeltaTime;
                if (_timer <= 0f)
                {
                    StartReturnToNormalSpeed();
                }
            }

            // ������������ ������� ������� � ���������� ��������
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

            // ���������� ��������� ������, ���� ������ �� ������� � ��� ���������
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
            _isReturning = false; // ��������� ������� ��� ������ ������ �������
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