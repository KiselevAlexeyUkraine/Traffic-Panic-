using System;
using UnityEngine;

namespace Codebase.Components.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerJump : MonoBehaviour
    {
        [SerializeField] private float jumpForce = 10f;

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
            _rigidbody.linearVelocity = new Vector3(_rigidbody.linearVelocity.x, 0f, _rigidbody.linearVelocity.z);
            _rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
}