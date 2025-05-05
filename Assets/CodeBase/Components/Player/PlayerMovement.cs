using Codebase.Services.Inputs;
using System;
using UnityEngine;
using Zenject;

namespace Codebase.Components.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMovement : MonoBehaviour
    {
        [Inject]
        private void Construct(DesktopInput desktopInput)
        {
            _playerInput = desktopInput;
        }

        public Action OnMovingLeft;
        public Action OnMovingRight;
        public Action OnLaneChanged; // Новое событие для перестроения
        public int LaneChangeCount { get; private set; } // Счетчик перестроений

        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _smoothness = 0.1f;
        [SerializeField] private float _distanceOveraging = 3f;
        [SerializeField] private float _snapSpeedMultiplier = 2f;
        [SerializeField] private float _snapDistanceThreshold = 0.5f;
        [SerializeField] private float[] _lanes = new float[] { -6f, -3f, 0f, 3f, 6f };

        private const float DISTANCE_THRESHOLD = 0.01f;

        private float _minXPosition;
        private float _maxXPosition;
        private float _currentXPosition;
        private float _targetXPosition;
        private IInput _playerInput;
        private Rigidbody _rigidbody;
        private bool _isMoving;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;

            if (_playerInput == null)
            {
                Debug.LogError("PlayerInput is not initialized!");
                enabled = false;
                return;
            }

            if (_lanes == null || _lanes.Length == 0)
            {
                Debug.LogError("Lanes array is empty or null!");
                enabled = false;
                return;
            }

            _minXPosition = _lanes[0];
            _maxXPosition = _lanes[_lanes.Length - 1];
            _currentXPosition = _rigidbody.position.x;
            _targetXPosition = SnapToNearestLane(_currentXPosition);
        }

        private void Update()
        {
            HandleInput();
        }

        private void FixedUpdate()
        {
            Move();
        }

        private void HandleInput()
        {
            if (_playerInput.Left && _currentXPosition > _minXPosition && !(_isMoving && _targetXPosition == _minXPosition))
            {
                _targetXPosition = SnapToNearestLane(Mathf.Max(_currentXPosition - _distanceOveraging, _minXPosition));
                _isMoving = true;
                OnMovingLeft?.Invoke();
                OnLaneChanged?.Invoke(); // Вызываем событие перестроения
                LaneChangeCount++; // Увеличиваем счетчик
                Debug.Log("Moving left, LaneChangeCount: " + LaneChangeCount);
            }
            else if (_playerInput.Right && _currentXPosition < _maxXPosition && !(_isMoving && _targetXPosition == _maxXPosition))
            {
                _targetXPosition = SnapToNearestLane(Mathf.Min(_currentXPosition + _distanceOveraging, _maxXPosition));
                _isMoving = true;
                OnMovingRight?.Invoke();
                OnLaneChanged?.Invoke(); // Вызываем событие перестроения
                LaneChangeCount++; // Увеличиваем счетчик
                Debug.Log("Moving right, LaneChangeCount: " + LaneChangeCount);
            }
        }

        private void Move()
        {
            Vector3 currentPosition = _rigidbody.position;
            Vector3 targetPosition = new Vector3(_targetXPosition, currentPosition.y, currentPosition.z);

            if (_isMoving)
            {
                float distanceToTarget = Vector3.Distance(currentPosition, targetPosition);
                float lerpFactor = 1f - Mathf.Pow(1f - _smoothness, Time.fixedDeltaTime * _moveSpeed);

                if (distanceToTarget <= _snapDistanceThreshold)
                {
                    lerpFactor *= _snapSpeedMultiplier;
                }

                lerpFactor = Mathf.Clamp01(lerpFactor);
                Vector3 newPosition = Vector3.Lerp(currentPosition, targetPosition, lerpFactor);

                _rigidbody.MovePosition(newPosition);
                _currentXPosition = newPosition.x;

                if (distanceToTarget < DISTANCE_THRESHOLD)
                {
                    _currentXPosition = _targetXPosition;
                    _isMoving = false;
                }
            }
            else
            {
                _targetXPosition = SnapToNearestLane(_currentXPosition);
                _rigidbody.MovePosition(targetPosition);
                _currentXPosition = _targetXPosition;
            }
        }

        private float SnapToNearestLane(float xPosition)
        {
            float nearestLane = _lanes[0];
            float minDistance = Mathf.Abs(xPosition - nearestLane);

            for (int i = 1; i < _lanes.Length; i++)
            {
                float distance = Mathf.Abs(xPosition - _lanes[i]);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestLane = _lanes[i];
                }
            }

            return nearestLane;
        }
    }
}