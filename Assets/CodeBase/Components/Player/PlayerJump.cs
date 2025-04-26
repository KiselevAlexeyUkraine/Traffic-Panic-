using System;
using UnityEngine;

namespace Codebase.Components.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerJump : MonoBehaviour
    {
        [SerializeField] private float targetJumpHeight = 2f;

        public Action OnJumping;

        private PlayerCollisionHandler _playerCollisionHandler;
        private Rigidbody _rigidbody;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _playerCollisionHandler = GetComponent<PlayerCollisionHandler>();
            _playerCollisionHandler.OnPlayerJump += OnJump;
        }

        private void OnDestroy()
        {
            _playerCollisionHandler.OnPlayerJump -= OnJump;
        }

        private void OnJump()
        {
            OnJumping?.Invoke();

            Vector3 velocity = _rigidbody.linearVelocity;
            velocity.y = 0f;
            _rigidbody.linearVelocity = velocity;

            float currentY = transform.position.y;
            float remainingHeight = Mathf.Max(targetJumpHeight - currentY, 0f);

            float gravity = Physics.gravity.magnitude;
            float requiredVelocity = Mathf.Sqrt(2f * gravity * remainingHeight);

            _rigidbody.AddForce(Vector3.up * requiredVelocity * _rigidbody.mass, ForceMode.Impulse);
        }
    }
}
