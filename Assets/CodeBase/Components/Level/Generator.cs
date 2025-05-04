// File: Generator.cs
using System.Collections.Generic;
using UnityEngine;

namespace Codebase.Components.Level
{
    public class Generator : MonoBehaviour
    {
        [SerializeField]
        private List<Level> _levels;
        [Min(0)]
        [SerializeField]
        private int _levelsCount;
        [SerializeField]
        private float _zBound;
        [SerializeField]
        private float _speed;
        [SerializeField]
        private float _acceleration;

        private float _initialSpeed;
        private float _initialAcceleration;

        private LinkedList<Level> _handledLevels = new();
        private int _levelIndex = 0;

        private void Awake()
        {
            _initialSpeed = _speed;
            _initialAcceleration = _acceleration;
        }

        private void Start()
        {
            _speed = _initialSpeed;
            _acceleration = _initialAcceleration;

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

                Level newLevel = Instantiate(GetNextLevel(), transform);
                newLevel.MoveToEdge(lastLevel, newLevel);

                _handledLevels.AddLast(newLevel);
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
