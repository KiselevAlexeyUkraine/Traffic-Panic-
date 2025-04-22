using UnityEngine;

namespace Codebase.Components.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerJump : MonoBehaviour
    {
        [SerializeField] private float jumpForce = 10f;
        [SerializeField] private LayerMask groundLayer;

        private PlayerCollisionHandler _playerCollisionHandler;
        private Rigidbody _rigidbody;
        private bool _isJumping;

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
            if (_isJumping) return;

            _isJumping = true;

            _rigidbody.linearVelocity = new Vector3(_rigidbody.linearVelocity.x, 0f, _rigidbody.linearVelocity.z);
            _rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            _rigidbody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!_isJumping)
                return;

            int otherLayerMask = 1 << collision.gameObject.layer;

            if ((groundLayer.value & otherLayerMask) != 0 && _rigidbody.linearVelocity.y <= 0.1f)
            {
                _isJumping = false;

                
                _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            }
        }
    }
}
