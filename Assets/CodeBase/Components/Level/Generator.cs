using System.Collections.Generic;
using UnityEngine;
using Codebase.Services.Time;
using Codebase.Components.Player;

namespace Codebase.Components.Level
{
    public class Generator : MonoBehaviour
    {
        [SerializeField] private List<Level> _levels;
        [Min(0)][SerializeField] private int _levelsCount;
        [SerializeField] private float _zBound;
        [SerializeField] private float _baseSpeed;
        [SerializeField] private SpeedModifier _speedModifier;
        [SerializeField] private PlayerCollisionHandler _playerCollisionHandler;

        private float _initialBaseSpeed;
        private float _speedMultiplier;
        private float _targetMultiplier;
        private LinkedList<Level> _handledLevels = new();
        private int _levelIndex;
        private float _boostTimer;

        private void Awake()
        {
            _initialBaseSpeed = _baseSpeed;
            ResetSpeedState();

            if (_speedModifier != null)
                _speedModifier.OnBoostSpeed += HandleBoost;

            if (_playerCollisionHandler != null)
                _playerCollisionHandler.OnNitro += HandleBoost;

            for (int i = 0; i < _levelsCount; i++)
            {
                Level newLevel = Instantiate(GetNextLevel(), transform);

                if (i == 0)
                    newLevel.transform.position = transform.position;
                else
                    newLevel.MoveToEdge(_handledLevels.Last.Value, newLevel);

                _handledLevels.AddLast(newLevel);
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
