using Codebase.Services.Inputs;
using System;
using System.Linq;
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

        [SerializeField] private float _maxXPosition = 6f;
        [SerializeField] private float _minXPosition = -6f;
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _smoothness = 0.1f;
        [SerializeField] private float _distanceOveraging = 3f;
        [SerializeField] private float _snapSpeedMultiplier = 2f; // Коэффициент ускорения при подстановке
        [SerializeField] private float _snapDistanceThreshold = 0.5f; // Порог расстояния для ускорения
        private float _currentXPosition = 0f;
        private float _targetXPosition = 0f;
        private IInput _playerInput;
        private Rigidbody _rigidbody;

        private bool _isMoving = false;
        private readonly float[] _lanes = new float[] { -6f, -3f, 0f, 3f, 6f }; // Фиксированные точки

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            _currentXPosition = _rigidbody.position.x;
            _targetXPosition = SnapToNearestLane(_currentXPosition); 
        }

        private void Update()
        {
            // Обновляем цель для движения
            if (_playerInput.Left && _currentXPosition > _minXPosition)
            {
                _targetXPosition = SnapToNearestLane(Mathf.Max(_currentXPosition - _distanceOveraging, _minXPosition));
                _isMoving = true;
                OnMovingLeft?.Invoke();
                Debug.Log("Нажата клавиша влево");
            }

            if (_playerInput.Right && _currentXPosition < _maxXPosition)
            {
                _targetXPosition = SnapToNearestLane(Mathf.Min(_currentXPosition + _distanceOveraging, _maxXPosition));
                _isMoving = true;
                OnMovingRight?.Invoke();
                Debug.Log("Нажата клавиша вправо");
            }

            // Плавное перемещение
            if (_isMoving)
            {
                Vector3 currentPosition = _rigidbody.position;
                Vector3 targetPosition = new Vector3(_targetXPosition, currentPosition.y, currentPosition.z);

                
                float distanceToTarget = Vector3.Distance(currentPosition, targetPosition);

               
                float lerpFactor = 1f - Mathf.Pow(1f - _smoothness, Time.deltaTime * _moveSpeed);

              
                if (distanceToTarget <= _snapDistanceThreshold)
                {
                    lerpFactor *= _snapSpeedMultiplier;
                }

               
                lerpFactor = Mathf.Clamp01(lerpFactor);

               
                Vector3 newPosition = Vector3.Lerp(currentPosition, targetPosition, lerpFactor);

               
                _rigidbody.MovePosition(newPosition);

              
                _currentXPosition = newPosition.x;

                
                if (distanceToTarget < 0.01f)
                {
                    _currentXPosition = _targetXPosition;
                    _isMoving = false;
                }
            }
            else
            {
               
                _targetXPosition = SnapToNearestLane(_currentXPosition);
                Vector3 finalPosition = new Vector3(_targetXPosition, _rigidbody.position.y, _rigidbody.position.z);
                _rigidbody.MovePosition(finalPosition);
                _currentXPosition = _targetXPosition;
            }
        }

        private float SnapToNearestLane(float xPosition)
        {
            
            return _lanes.OrderBy(lane => Mathf.Abs(lane - xPosition)).First();
        }
    }
}