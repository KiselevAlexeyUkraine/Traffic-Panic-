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
        [SerializeField] private Animator cameraAnimator; // ������ �� Animator ������
        [SerializeField] private CameraShaker cameraShaker; // ������ �� CameraShaker
        [SerializeField] private float shakeInterval = 0.5f; // �������� ����� ��������
        [SerializeField] private float abilityUnlockDelay = 10f; // Delay before abilities are unlocked

        private IInput _playerInput;
        private float _timer;
        private bool _effectActive;
        private bool _isBoosting; // ���� ��� ������������ ��������� ���������
        private float _shakeTimer; // ������ ��� ��������� ������
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
                // ���������, ������ �� ������ ���������
                if (_playerInput.Boost)
                {
                    // ���� ��������� �������, ���������� ������, ����� ������ �����������
                    if (!_effectActive || UnityEngine.Time.timeScale != boostScale)
                    {
                        StartTimeEffect(boostScale);
                        if (cameraShaker != null) _shakeTimer = 0f; // ���������� ������ ������ ��� ������
                    }
                    else
                    {
                        _timer = effectDuration; // ���������� ������ ��� ������ �������
                    }
                    _isBoosting = true;

                    if (cameraAnimator != null) cameraAnimator.SetBool("IsBoosting", true); // ���������� �������� ���������

                    // ������������ ������ ������ � ����������
                    _shakeTimer -= UnityEngine.Time.unscaledDeltaTime;
                    if (_shakeTimer <= 0f)
                    {
                        if (cameraShaker != null) cameraShaker.Shake(); // �������� ������
                        _shakeTimer = shakeInterval; // ���������� ������ ������
                    }
                }
                // ���������, ������ �� ������ ����������
                else if (_playerInput.Drag)
                {
                    StartTimeEffect(brakeScale);
                    _isBoosting = false;
                    if (cameraAnimator != null) cameraAnimator.SetBool("IsBoosting", false); // ���������� ������ � �������� ���������
                }
            }

            // ��������� ������, ���� ������ �������
            if (_effectActive)
            {
                _timer -= UnityEngine.Time.unscaledDeltaTime;
                if (_timer <= 0f)
                {
                    UnityEngine.Time.timeScale = 1f; // ���������� ���������� ��������
                    _effectActive = false;

                    // ���� ����� ������ �� �������� ���������, ���������� ������
                    if (!_playerInput.Boost)
                    {
                        _isBoosting = false;
                        if (cameraAnimator != null) cameraAnimator.SetBool("IsBoosting", false);
                    }
                }
            }

            // ���� ������ �� ������� � ����� �� �������� ���������, ���������� ��������� ������
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

        // ��������� ����� ��� ������ ��������� �� ������ ��������
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
                _shakeTimer = 0f; // ���������� ������ ������
            }
            else
            {
                _timer = effectDuration; // ���������� ������
            }
            _isBoosting = true;
            if (cameraAnimator != null) cameraAnimator.SetBool("IsBoosting", true);
        }
    }
}