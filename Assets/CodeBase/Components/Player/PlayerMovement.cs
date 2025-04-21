using Codebase.Services.Inputs;
using System;
using UnityEngine;

namespace Codebase.Components.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMovement : MonoBehaviour
    {
        public Action OnMoving;

        [SerializeField] private float _maxXPosition = 2f;
        [SerializeField] private float _minXPosition = -2f;
        [SerializeField] private float _moveSpeed = 5f;

        private float _currentXPosition = 0f;
        private float _targetXPosition = 0f;
        private IInput _playerInput = new DesktopInput();
        private Rigidbody _rigidbody;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            _targetXPosition = _currentXPosition;
        }

        private void Update()
        {
            if (_playerInput.Left && _currentXPosition > _minXPosition)
            {
                _targetXPosition = _currentXPosition - 1f;
                OnMoving?.Invoke();
                Debug.Log("Нажата клавиша влево");
            }

            if (_playerInput.Right && _currentXPosition < _maxXPosition)
            {
                _targetXPosition = _currentXPosition + 1f;
                OnMoving?.Invoke();
                Debug.Log("Нажата клавиша вправо");
            }
        }

        private void FixedUpdate()
        {
            if (Mathf.Approximately(_currentXPosition, _targetXPosition))
                return;

            Vector3 currentPosition = _rigidbody.position;
            Vector3 targetPosition = new (_targetXPosition, currentPosition.y, currentPosition.z);
            Vector3 newPosition = Vector3.MoveTowards(currentPosition, targetPosition, _moveSpeed * Time.fixedDeltaTime);
            _rigidbody.MovePosition(newPosition);

            if (Mathf.Approximately(newPosition.x, _targetXPosition))
            {
                _currentXPosition = _targetXPosition;
            }

            Debug.Log("Вызов FixedUpdate");
        }
    }
}
