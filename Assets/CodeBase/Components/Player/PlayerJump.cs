using System;
using UnityEngine;

namespace Codebase.Components.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerJump : MonoBehaviour
    {
        [SerializeField] private float jumpForce = 10f;
        [SerializeField] private LayerMask groundLayer;

        public bool IsJumping { get; private set; }

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
            if (IsJumping) return;

            IsJumping = true;
            OnJumping?.Invoke();
            _rigidbody.linearVelocity = new Vector3(_rigidbody.linearVelocity.x, 0f, _rigidbody.linearVelocity.z);
            _rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            _rigidbody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!IsJumping)
                return;

            int otherLayerMask = 1 << collision.gameObject.layer;

            if ((groundLayer.value & otherLayerMask) != 0 && _rigidbody.linearVelocity.y <= 0.1f)
            {
                IsJumping = false;
                _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            }
        }
    }
}
