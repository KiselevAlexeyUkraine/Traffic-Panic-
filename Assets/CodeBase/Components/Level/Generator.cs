using System.Collections.Generic;
using UnityEngine;
using Codebase.Services.Time;
using Codebase.Components.Player;

namespace Codebase.Components.Level
{
    public class Generator : MonoBehaviour
    {
        [SerializeField] private List<Level> _levels;
        [Min(0)] [SerializeField] private int _levelsCount;
        [SerializeField] private float _zBound;
        [SerializeField] private float _speed;
        [SerializeField] private float _acceleration;
        [SerializeField] private PlayerMovement _playerMovement; // Ссылка на PlayerMovement
        [SerializeField] private SpeedModifier _speedModifier; // Ссылка на SpeedModifier
        [SerializeField] public bool IsFirstLevel = true; // Флаг первого уровня

        private float _initialBaseSpeed;
        private float _speedMultiplier;
        private float _targetMultiplier;
        private LinkedList<Level> _handledLevels = new();
        private int _levelIndex;
        private float _boostTimer;

        private void Awake()
        {
            _speed = 15f;
            if (IsFirstLevel)
            {
                // Первый уровень: спавним _levelsCount копий _levels[0]
                for (int i = 0; i < _levelsCount; i++)
                {
                    Level newLevel = Instantiate(_levels[0], transform); // Используем только _levels[0]
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

                // Подписываемся на события для проверки условий
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
                }
                else
                {
                    Debug.LogError("SpeedModifier is not assigned in Generator!");
                }

                CheckConditions(); // Проверяем условия на старте
            }
            else
            {
                // Не первый уровень: стандартная генерация
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

        private void OnDestroy()
        {
            if (!IsFirstLevel) return;

            // Отписываемся от событий только для первого уровня
            if (_playerMovement != null)
            {
                _playerMovement.OnLaneChanged -= CheckConditions;
            }
            if (_speedModifier != null)
            {
                _speedModifier.OnBoostUsed -= CheckConditions;
            }
        }

        private void OnEnable()
        {
            _baseSpeed = _initialBaseSpeed;
            ResetSpeedState();
        }

        private void ResetSpeedState()
        {
            _speedMultiplier = 1f;
            _targetMultiplier = 1f;
            _boostTimer = 0f;
        }

        private void Update()
        {
            if (_boostTimer > 0f)
            {
                _boostTimer -= Time.deltaTime;
                if (_boostTimer <= 0f)
                    ResetSpeedMultiplier();
            }

            _speedMultiplier = Mathf.Lerp(_speedMultiplier, _targetMultiplier, Time.deltaTime);
            float speed = _baseSpeed * _speedMultiplier;


            foreach (Level level in _handledLevels)
                level.Move(Vector3.back, speed, 0f);

            Level firstLevel = _handledLevels.First.Value;
            Vector3 firstLevelCenter = firstLevel.transform.TransformPoint(0f, 0f, firstLevel.Center);
            Vector3 firstLevelEdge = firstLevelCenter + new Vector3(0f, 0f, firstLevel.Extents);

            if (firstLevelEdge.z < transform.position.z + _zBound)
            {
                Level lastLevel = _handledLevels.Last.Value;
                Destroy(firstLevel.gameObject);
                _handledLevels.RemoveFirst();

                Level newLevel = Instantiate(GetNextLevel(), transform);
                newLevel.MoveToEdge(_handledLevels.Last.Value, newLevel);
                _handledLevels.AddLast(newLevel);
            }
        }

        public void SetSpeedMultiplier(float multiplier)
        {
            _targetMultiplier = multiplier;
        }

        private void CheckConditions()
        {
            if (!IsFirstLevel) return;

            // Проверяем условия: 3 перестроения и 1 ускорение
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
        public void ResetSpeedMultiplier()
        {
            _targetMultiplier = 1f;
        }

        private void HandleBoost(float multiplier, float duration)
        {
            SetSpeedMultiplier(multiplier);
            _boostTimer = duration;
        }

        private Level GetNextLevel()
        {
            Level level = _levels[_levelIndex];
            _levelIndex = (_levelIndex + 1) % _levels.Count;
            return level;
        }

        private void OnDestroy()
        {
            if (_speedModifier != null)
                _speedModifier.OnBoostSpeed -= HandleBoost;
            if (_playerCollisionHandler != null)
                _playerCollisionHandler.OnNitro -= HandleBoost;

            _baseSpeed = _initialBaseSpeed;
            ResetSpeedState();
        }
    }
}