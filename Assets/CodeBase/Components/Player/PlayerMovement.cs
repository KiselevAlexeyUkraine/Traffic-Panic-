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

        private float _currentXPosition = 0f;
        private float _targetXPosition = 0f;
        private IInput _playerInput;
        private Rigidbody _rigidbody;
        private PlayerJump _playerJump;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            _targetXPosition = _currentXPosition;

            _playerJump = GetComponent<PlayerJump>();
        }

        private void Update()
        {
            if (_playerJump != null && _playerJump.IsJumping)
                return; // запрет на смену позиции в воздухе

            if (_playerInput.Left && _currentXPosition > _minXPosition)
            {
                _targetXPosition = _currentXPosition - 2f;
                OnMovingLeft?.Invoke();
                Debug.Log("Нажата клавиша влево");
            }

            if (_playerInput.Right && _currentXPosition < _maxXPosition)
            {
                _targetXPosition = _currentXPosition + 2f;
                OnMovingRight?.Invoke();
                Debug.Log("Нажата клавиша вправо");
            }
        }

        private void FixedUpdate()
        {
            if (_playerJump != null && _playerJump.IsJumping)
                return;

            if (Mathf.Approximately(_currentXPosition, _targetXPosition))
                return;

            Vector3 currentPosition = _rigidbody.position;
            Vector3 targetPosition = new(_targetXPosition, currentPosition.y, currentPosition.z);
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
