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
    [SerializeField] private float shakeInterval = 0.5f;
    [SerializeField] private bool isFirstLevel;
    [SerializeField] private Animator cameraAnimator;
    [SerializeField] private CameraShaker cameraShaker;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private Generator generator;
    [SerializeField] private PlayerCollisionHandler playerCollisionHandler;

    public Action OnBoostUsed;
    public bool HasBoosted { get; private set; }
    public event Action<float, float> OnBoostSpeed;

    private IInput _playerInput;
    private float _effectTimer;
    private float _shakeTimer;
    private bool _effectActive;
    private bool _isBoosting;
    private bool _abilitiesUnlocked;
    private const int REQUIRED_LANE_CHANGES = 3;

    [Inject]
    private void Construct(DesktopInput desktopInput) => _playerInput = desktopInput;

    private void Awake()
    {
        ValidateReferences();
        InitializeAbilities();
    }

    private void OnEnable()
    {
        if (playerCollisionHandler != null)
            playerCollisionHandler.OnNitroActivated += ResetOnNitro;
    }

    private void OnDisable()
    {
        if (playerCollisionHandler != null)
            playerCollisionHandler.OnNitroActivated -= ResetOnNitro;
    }

    private void Update()
    {
        if (playerCollisionHandler != null && playerCollisionHandler.IsNitroActive)
            return;

        if (!_abilitiesUnlocked)
            return;

        HandleInput();
        UpdateEffect();
    }

    private void ValidateReferences()
    {
        if (playerMovement == null) Debug.LogError("PlayerMovement is not assigned!");
        if (generator == null) Debug.LogError("Generator is not assigned!");
        if (playerCollisionHandler == null) Debug.LogError("PlayerCollisionHandler is not assigned!");
    }

    private void InitializeAbilities()
    {
        _abilitiesUnlocked = !isFirstLevel;
    }

    private void HandleInput()
    {
        if (_playerInput.Boost && CanBoost())
        {
           
            if (!_effectActive)
            {
                StartSpeedEffect(boostScale);
                _shakeTimer = 0f;
                NotifyBoostUsed();
            }
            else if (_isBoosting)
            {
                _effectTimer = effectDuration;
                Debug.Log($"Updating Boost timer: {_effectTimer}");
            }
            _isBoosting = true;
            UpdateCameraAnimator(true);
        }
        else if (_playerInput.Drag && !_effectActive)
        {
            StartSpeedEffect(brakeScale);
            _isBoosting = false;
            UpdateCameraAnimator(false);
        }
    }

    private void UpdateEffect()
    {
        if (!_effectActive) return;

        UpdateShake();
        _effectTimer -= Time.deltaTime;
        if (_effectTimer <= 0f)
            ResetSpeed();
    }

    private void UpdateShake()
    {
        if (!_isBoosting) return;

        _shakeTimer -= Time.deltaTime;
        if (_shakeTimer <= 0f)
        {
            cameraShaker?.Shake();
            _shakeTimer = shakeInterval;
        }
    }

    private void StartSpeedEffect(float scale)
    {
        if (generator == null) return;

        generator.SetSpeedMultiplier(scale);
        OnBoostSpeed?.Invoke(scale, effectDuration);
        Debug.Log($"Starting speed effect, multiplier: {scale}, duration: {effectDuration}");
        _effectTimer = effectDuration;
        _effectActive = true;
    }

    private void ResetSpeed()
    {
        if (generator == null) return;

        generator.ResetSpeedMultiplier();
        OnBoostSpeed?.Invoke(1f, 0f);
        Debug.Log("Resetting speed multiplier to 1");
        _effectActive = false;
        _isBoosting = false;
        UpdateCameraAnimator(false);
    }

    private void ResetOnNitro()
    {
        if (_effectActive)
        {
            ResetSpeed();
            Debug.Log("Nitro activated, resetting Boost/Brake effect");
        }
        _effectTimer = 0f;
        _effectActive = false;
        _isBoosting = false;
        UpdateCameraAnimator(false);
    }

    private void UpdateCameraAnimator(bool isBoosting)
    {
        if (cameraAnimator != null)
            cameraAnimator.SetBool("IsBoosting", isBoosting);
    }

    private void NotifyBoostUsed()
    {
        if (!HasBoosted)
        {
            HasBoosted = true;
            OnBoostUsed?.Invoke();
            Debug.Log("Boost used!");
        }
    }

    public void TriggerBoost(float scale)
    {
        if (!_abilitiesUnlocked)
        {
            Debug.Log("Abilities not yet unlocked!");
            return;
        }

        if (isFirstLevel && !CanBoost())
        {
            Debug.Log("Cannot boost: Complete 3 lane changes first!");
            return;
        }

        if (playerCollisionHandler != null && playerCollisionHandler.IsNitroActive)
        {
            Debug.Log("Cannot trigger boost: Nitro is active!");
            return;
        }

        StartSpeedEffect(scale);
        _shakeTimer = 0f;
        NotifyBoostUsed();
    }

    private bool CanBoost() => playerMovement != null && (!isFirstLevel || playerMovement.LaneChangeCount >= REQUIRED_LANE_CHANGES);
}