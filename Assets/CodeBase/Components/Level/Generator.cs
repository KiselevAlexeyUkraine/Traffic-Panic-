// Generator.cs
using System.Collections.Generic;
using UnityEngine;
using Codebase.Services.Time; // подключение SpeedModifier

namespace Codebase.Components.Level
{
    public class Generator : MonoBehaviour
    {
        [SerializeField] private List<Level> _levels;
        [Min(0)][SerializeField] private int _levelsCount;
        [SerializeField] private float _zBound;
        [SerializeField] private float _baseSpeed;
        [SerializeField] private float _acceleration;
        [SerializeField] private SpeedModifier _speedModifier; // ссылка на SpeedModifier

        private float _speedMultiplier = 1f;
        private float _targetMultiplier = 1f;
        private float _initialSpeed;
        private LinkedList<Level> _handledLevels = new();
        private int _levelIndex = 0;

        private void Awake()
        {
            _initialSpeed = _baseSpeed;

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
            if (_speedModifier != null)
                _speedModifier.OnBoostSpeed += HandleBoostSpeed;
        }

        private void OnDisable()
        {
            if (_speedModifier != null)
                _speedModifier.OnBoostSpeed -= HandleBoostSpeed;
        }

        private void HandleBoostSpeed(float multiplier, float duration)
        {
            StopAllCoroutines();
            StartCoroutine(ApplySpeedBoost(multiplier, duration));
        }

        private System.Collections.IEnumerator ApplySpeedBoost(float multiplier, float duration)
        {
            SetSpeedMultiplier(multiplier);
            yield return new WaitForSeconds(duration);
            ResetSpeedMultiplier();
        }

        private void Update()
        {
            _speedMultiplier = Mathf.Lerp(_speedMultiplier, _targetMultiplier, Time.deltaTime);
            float speed = _baseSpeed * _speedMultiplier;

            foreach (Level level in _handledLevels)
                level.Move(Vector3.back, speed, _acceleration);

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

        private Level GetNextLevel()
        {
            Level level = _levels[_levelIndex];
            _levelIndex = (_levelIndex + 1) % _levels.Count;
            return level;
        }
    }
}
