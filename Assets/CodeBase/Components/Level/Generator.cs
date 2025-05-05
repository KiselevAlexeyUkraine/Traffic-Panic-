using System.Collections.Generic;
using UnityEngine;
using Codebase.Components.Player;
using Codebase.Services.Time;

namespace Codebase.Components.Level
{
    public class Generator : MonoBehaviour
    {
        [SerializeField] private List<Level> _levels;
        [Min(0)] [SerializeField] private int _levelsCount;
        [SerializeField] private float _zBound;
        [SerializeField] private float _speed;
        [SerializeField] private float _acceleration;
        [SerializeField] private PlayerMovement _playerMovement; // ������ �� PlayerMovement
        [SerializeField] private SpeedModifier _speedModifier; // ������ �� SpeedModifier
        [SerializeField] public bool IsFirstLevel = true; // ���� ������� ������

        private LinkedList<Level> _handledLevels = new();
        private int _levelIndex = 0;
        private bool _conditionsMet = false; // ���� ���������� �������
        private const int REQUIRED_LANE_CHANGES = 3; // ��������� ���������� ������������

        public bool ConditionsMet => _conditionsMet; // ��������� �������� ��� ProgressTimer

        private void Start()
        {
            _speed = 15f;
            if (IsFirstLevel)
            {
                // ������ �������: ������� _levelsCount ����� _levels[0]
                for (int i = 0; i < _levelsCount; i++)
                {
                    Level newLevel = Instantiate(_levels[0], transform); // ���������� ������ _levels[0]
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

                // ������������� �� ������� ��� �������� �������
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

                CheckConditions(); // ��������� ������� �� ������
            }
            else
            {
                // �� ������ �������: ����������� ���������
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

            // ������������ �� ������� ������ ��� ������� ������
            if (_playerMovement != null)
            {
                _playerMovement.OnLaneChanged -= CheckConditions;
            }
            if (_speedModifier != null)
            {
                _speedModifier.OnBoostUsed -= CheckConditions;
            }
        }

        private void Update()
        {
            foreach (Level level in _handledLevels)
            {
                level.Move(Vector3.back, _speed, _acceleration);
            }

            Level firstLevel = _handledLevels.First.Value;
            Vector3 firstLevelCenter = firstLevel.transform.TransformPoint(0f, 0f, firstLevel.Center);
            Vector3 firstLevelEdge = firstLevelCenter + new Vector3(0f, 0f, firstLevel.Extents);

            if (firstLevelEdge.z < transform.position.z + _zBound)
            {
                Level lastLevel = _handledLevels.Last.Value;
                Destroy(firstLevel.gameObject);
                _handledLevels.RemoveFirst();

                Level newLevel;
                if (IsFirstLevel && !_conditionsMet)
                {
                    // ������ �������, ������� �� ���������: ������� ����� _levels[0]
                    newLevel = Instantiate(_levels[0], transform);
                    Debug.Log("Respawning first segment (conditions not met).");
                }
                else
                {
                    // �� ������ ������� ��� ������� ���������: ����������� ���������
                    newLevel = Instantiate(GetNextLevel(), transform);
                    Debug.Log("Spawning next segment.");
                }

                newLevel.MoveToEdge(lastLevel, newLevel);
                _handledLevels.AddLast(newLevel);
            }
        }

        private void CheckConditions()
        {
            if (!IsFirstLevel) return;

            // ��������� �������: 3 ������������ � 1 ���������
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

        private Level GetNextLevel()
        {
            Level level = _levels[_levelIndex];
            _levelIndex = (_levelIndex + 1) % _levels.Count;
            return level;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            foreach (Level level in _handledLevels)
            {
                Vector3 center = level.transform.TransformPoint(new Vector3(0f, 0f, level.Center));
                Vector3 edge = center + new Vector3(0f, 0f, level.Extents);

                Gizmos.color = Color.white;
                Gizmos.DrawSphere(center, 0.2f);
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(edge, 0.3f);
            }

            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(transform.position + Vector3.forward * _zBound, 0.2f);
        }
#endif
    }
}