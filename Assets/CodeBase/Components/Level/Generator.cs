using System.Collections.Generic;
using UnityEngine;
using Codebase.Components.Player;


namespace Codebase.Components.Level
{
    public class Generator : MonoBehaviour
    {
        [SerializeField] private List<Level> _levels;
        [Min(0)]
        [SerializeField] private int _levelsCount = 3;
        [SerializeField] private float _zBound = -10f;
        [SerializeField] private float _speed = 15f;
        [SerializeField] private PlayerMovement _playerMovement;
        [SerializeField] private SpeedModifier _speedModifier;
        [SerializeField] private PlayerCollisionHandler _playerCollisionHandler;
        [SerializeField] public bool IsFirstLevel = true;

        private const int REQUIRED_LANE_CHANGES = 3;
        private float _baseSpeed;
        private float _initialBaseSpeed = 15f;
        private float _speedMultiplier = 1f;
        private float _targetMultiplier = 1f;
        private float _boostTimer;
        private bool _isNitroActive;
        private LinkedList<Level> _handledLevels = new();
        private int _levelIndex;
        public bool _conditionsMet;

        private void Awake()
        {
            _baseSpeed = _initialBaseSpeed;

            if (IsFirstLevel)
            {
                if (_levels == null || _levels.Count == 0)
                {
                    Debug.LogError("Levels list is empty or not assigned in Generator!");
                    return;
                }

                for (int i = 0; i < _levelsCount; i++)
                {
                    Level newLevel = Instantiate(_levels[0], transform);
                    if (i == 0)
                    {
                        newLevel.transform.position = transform.position;
                    }
                    else
                    {
                        Level lastLevel = _handledLevels.Last.Value;
                        newLevel.MoveToEdge(lastLevel, newLevel);
                    }
                    _handledLevels.AddLast(newLevel);
                }

                if (_playerMovement != null)
                {
                    _playerMovement.OnLaneChanged += CheckConditions;
                }
                else
                {
                    Debug.LogError("PlayerMovement is not assigned in Generator!");
                }

                if (_speedModifier != null)
                {
                    _speedModifier.OnBoostUsed += CheckConditions;
                    _speedModifier.OnBoostSpeed += HandleBoost;
                }
                else
                {
                    Debug.LogError("SpeedModifier is not assigned in Generator!");
                }

                CheckConditions();
            }
            else
            {
                if (_levels == null || _levels.Count == 0)
                {
                    Debug.LogError("Levels list is empty or not assigned in Generator!");
                    return;
                }

                for (int i = 0; i < _levelsCount; i++)
                {
                    Level newLevel = Instantiate(GetNextLevel(), transform);
                    if (i == 0)
                    {
                        newLevel.transform.position = transform.position;
                    }
                    else
                    {
                        Level lastLevel = _handledLevels.Last.Value;
                        newLevel.MoveToEdge(lastLevel, newLevel);
                    }
                    _handledLevels.AddLast(newLevel);
                }
            }
        }

        private void OnEnable()
        {
            _baseSpeed = _initialBaseSpeed;
            ResetSpeedState();
        }

        private void OnDestroy()
        {
            if (!IsFirstLevel) return;

            if (_playerMovement != null)
            {
                _playerMovement.OnLaneChanged -= CheckConditions;
            }

            if (_speedModifier != null)
            {
                _speedModifier.OnBoostUsed -= CheckConditions;
                _speedModifier.OnBoostSpeed -= HandleBoost;
            }

            ResetSpeedState();
        }

        private void ResetSpeedState()
        {
            _speedMultiplier = 1f;
            _targetMultiplier = 1f;
            _boostTimer = 0f;
            _isNitroActive = false;
        }

        public void DeactivateNitro()
        {
            _isNitroActive = false;
            _targetMultiplier = 1f;
            _boostTimer = 0f;
            Debug.Log("DeactivateNitro called, resetting isNitroActive and multiplier");
        }

        private void Update()
        {
            if (_boostTimer > 0f)
            {
                _boostTimer -= Time.deltaTime;
                Debug.Log($"Boost timer: {_boostTimer}, isNitroActive: {_isNitroActive}");
                if (_boostTimer <= 0f && !_isNitroActive)
                {
                    ResetSpeedMultiplier();
                    Debug.Log("Boost timer expired, resetting multiplier");
                }
            }

            _speedMultiplier = _targetMultiplier; // Мгновенное изменение скорости
            float speed = _baseSpeed * _speedMultiplier;
            Debug.Log($"Generator Update: speedMultiplier={_speedMultiplier}, targetMultiplier={_targetMultiplier}, speed={speed}");

            foreach (Level level in _handledLevels)
                level.Move(Vector3.back, speed, 0f);

            if (_handledLevels.Count == 0) return;

            Level firstLevel = _handledLevels.First.Value;
            Vector3 firstLevelCenter = firstLevel.transform.TransformPoint(0f, 0f, firstLevel.Center);
            Vector3 firstLevelEdge = firstLevelCenter + new Vector3(0f, 0f, firstLevel.Extents);

            if (firstLevelEdge.z < transform.position.z + _zBound)
            {
                Level lastLevel = _handledLevels.Last.Value;
                Destroy(firstLevel.gameObject);
                _handledLevels.RemoveFirst();

                Level newLevel = Instantiate(GetNextLevel(), transform);
                newLevel.MoveToEdge(lastLevel, newLevel);
                _handledLevels.AddLast(newLevel);
            }
        }

        public void SetSpeedMultiplier(float multiplier)
        {
            _targetMultiplier = Mathf.Max(_targetMultiplier, multiplier);
            _isNitroActive = multiplier >= 2.5f;
            Debug.Log($"SetSpeedMultiplier called, new target: {_targetMultiplier}, isNitroActive: {_isNitroActive}");
        }

        private void CheckConditions()
        {
            if (!IsFirstLevel) return;

            if (_playerMovement != null && _speedModifier != null &&
                _playerMovement.LaneChangeCount >= REQUIRED_LANE_CHANGES && _speedModifier.HasBoosted)
            {
                _conditionsMet = true;
                Debug.Log("Conditions met! Switching to standard level generation.");
            }
            else
            {
                Debug.Log($"Conditions not met. LaneChanges: {_playerMovement?.LaneChangeCount}/{REQUIRED_LANE_CHANGES}, Boosted: {_speedModifier?.HasBoosted}");
            }
        }

        public void ResetSpeedMultiplier()
        {
            if (_isNitroActive)
            {
                Debug.Log("Ignoring ResetSpeedMultiplier: Nitro is active");
                return;
            }
            _targetMultiplier = 1f;
            Debug.Log("ResetSpeedMultiplier applied, target: 1");
        }

        private void HandleBoost(float multiplier, float duration)
        {
            Debug.Log($"HandleBoost called with multiplier: {multiplier}, duration: {duration}");
            SetSpeedMultiplier(multiplier);
            if (multiplier >= 2.5f) // Для Nitro устанавливаем таймер явно
            {
                _boostTimer = duration;
            }
            else
            {
                _boostTimer = Mathf.Max(_boostTimer, duration);
            }
        }

        private Level GetNextLevel()
        {
            if (_levels == null || _levels.Count == 0)
            {
                Debug.LogError("No levels available in GetNextLevel!");
                return null;
            }

            Level level = _levels[_levelIndex];
            _levelIndex = (_levelIndex + 1) % _levels.Count;
            return level;
        }
    }
}