using UnityEngine;
using Codebase.Interfaces;

namespace Codebase.NPC
{
    public class NpcMover : MonoBehaviour, IWetMove
    {
        public enum MoveDirection
        {
            Right,
            Left,
            Stay,
            Random
        }

        [SerializeField] private float moveDuration = 1.2f;
        [SerializeField] private MoveDirection direction = MoveDirection.Stay;
        [SerializeField] private float minSpeedZ = 3f;
        [SerializeField] private float maxSpeedZ = 7f;
        [SerializeField] private float speedChangeInterval = 2f;
        [SerializeField] private float carSpeedX = 5f;
        [SerializeField] private Animator animator;

        private readonly float[] _positionsX = { -6f, -3f, 0f, 3f, 6f };

        private Vector3 _startPosition;
        private Vector3 _targetLocalPosition;
        private float _moveTimer;
        private bool _isMoving;
        private float _currentSpeedZ;
        private float _speedChangeTimer;
        private bool _speedChangeEnabled;
        private float _baseMinSpeedZ;
        private float _baseMaxSpeedZ;

        private void Start()
        {
            _targetLocalPosition = transform.localPosition;
            _baseMinSpeedZ = minSpeedZ;
            _baseMaxSpeedZ = maxSpeedZ;

            _currentSpeedZ = 0f;
            _speedChangeTimer = speedChangeInterval;
            _speedChangeEnabled = direction != MoveDirection.Random;

            if (_speedChangeEnabled)
            {
                _currentSpeedZ = Random.Range(minSpeedZ, maxSpeedZ);
            }
        }

        public void SetSpeedMultiplier(float multiplier)
        {
            minSpeedZ = _baseMinSpeedZ * multiplier;
            maxSpeedZ = _baseMaxSpeedZ * multiplier;
        }

        public void ResetSpeedMultiplier()
        {
            minSpeedZ = _baseMinSpeedZ;
            maxSpeedZ = _baseMaxSpeedZ;
        }

        private void Update()
        {
            if (_isMoving)
            {
                _moveTimer += Time.deltaTime;
                float t = Mathf.Clamp01(_moveTimer / moveDuration);
                Vector3 lerpedPosition = Vector3.Lerp(_startPosition, _targetLocalPosition, t);
                transform.localPosition = new Vector3(lerpedPosition.x, lerpedPosition.y, _startPosition.z);

                if (t >= 1f)
                    _isMoving = false;
            }

            if (_speedChangeEnabled && _currentSpeedZ != 0f)
            {
                _speedChangeTimer -= Time.deltaTime;
                if (_speedChangeTimer <= 0f)
                {
                    _currentSpeedZ = Random.Range(minSpeedZ, maxSpeedZ);
                    _speedChangeTimer = speedChangeInterval;
                }

                Vector3 currentPosition = transform.localPosition;
                currentPosition.z += _currentSpeedZ * Time.deltaTime;
                transform.localPosition = currentPosition;
            }
        }

        public void TriggerMove()
        {
            if (_isMoving) return;

            float currentX = Mathf.Round(transform.localPosition.x);
            int currentIndex = GetClosestIndex(currentX);
            if (currentIndex == -1) return;

            float? nextX = null;
            MoveDirection actualDirection = direction;

            switch (direction)
            {
                case MoveDirection.Right:
                    if (currentIndex < _positionsX.Length - 1)
                        nextX = _positionsX[currentIndex + 1];
                    break;

                case MoveDirection.Left:
                    if (currentIndex > 0)
                        nextX = _positionsX[currentIndex - 1];
                    break;

                case MoveDirection.Random:
                    int dir = Random.Range(0, 2);
                    if (dir == 0 && currentIndex > 0)
                    {
                        nextX = _positionsX[currentIndex - 1];
                        actualDirection = MoveDirection.Left;
                    }
                    else if (dir == 1 && currentIndex < _positionsX.Length - 1)
                    {
                        nextX = _positionsX[currentIndex + 1];
                        actualDirection = MoveDirection.Right;
                    }
                    break;

                case MoveDirection.Stay:
                    return;
            }

            if (nextX == null) return;

            if (actualDirection == MoveDirection.Right)
                animator.Play("Right");
            else if (actualDirection == MoveDirection.Left)
                animator.Play("Left");

            _startPosition = transform.localPosition;
            _targetLocalPosition = new Vector3(nextX.Value, _startPosition.y, _startPosition.z);
            _moveTimer = 0f;
            _isMoving = true;
        }

        private int GetClosestIndex(float value)
        {
            float minDiff = float.MaxValue;
            int closestIndex = -1;
            for (int i = 0; i < _positionsX.Length; i++)
            {
                float diff = Mathf.Abs(_positionsX[i] - value);
                if (diff < minDiff)
                {
                    minDiff = diff;
                    closestIndex = i;
                }
            }
            return closestIndex;
        }
    }
}
