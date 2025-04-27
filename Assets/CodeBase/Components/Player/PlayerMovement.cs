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

        [SerializeField] private float _maxXPosition = 2f;
        [SerializeField] private float _minXPosition = -2f;
        [SerializeField] private float _moveSpeed = 5f;

        private IInput _playerInput;
        private Rigidbody _rigidbody;

        private float _targetXPosition;
        private float _currentXPosition;
        private bool _isMoving = false;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            _currentXPosition = _rigidbody.position.x;
            _targetXPosition = _currentXPosition;
        }

        private void Update()
        {
            if (_isMoving)
                return;

            if (_playerInput.Left && _currentXPosition > _minXPosition)
            {
                _targetXPosition = Mathf.Max(_currentXPosition - 2f, _minXPosition);
                _isMoving = true;
                OnMovingLeft?.Invoke();
                Debug.Log("Нажата клавиша влево");
            }

            if (_playerInput.Right && _currentXPosition < _maxXPosition)
            {
                _targetXPosition = Mathf.Min(_currentXPosition + 2f, _maxXPosition);
                _isMoving = true;
                OnMovingRight?.Invoke();
                Debug.Log("Нажата клавиша вправо");
            }
        }

        private void FixedUpdate()
        {
            if (!_isMoving)
                return;

            Vector3 currentPosition = _rigidbody.position;
            Vector3 targetPosition = new Vector3(_targetXPosition, currentPosition.y, currentPosition.z);
            Vector3 newPosition = Vector3.MoveTowards(currentPosition, targetPosition, _moveSpeed * Time.fixedDeltaTime);
            _rigidbody.MovePosition(newPosition);

            if (Mathf.Approximately(newPosition.x, _targetXPosition))
            {
                _currentXPosition = _targetXPosition;
                _isMoving = false;
            }
        }
    }
}