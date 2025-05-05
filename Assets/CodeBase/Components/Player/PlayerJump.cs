using UnityEngine;

namespace Codebase.Components.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerJump : MonoBehaviour
    {
        [SerializeField] private float flyHeight = 5f; // ������, �� ������� ������ "������"
        [SerializeField] private float flyDuration = 1f; // ������������ ����� � �������
        [SerializeField] private float jumpDuration = 1f; // ������������ ������� �� ������

        private PlayerCollisionHandler _playerCollisionHandler;
        private Rigidbody _rigidbody;
        private bool _isJumping; // ����, �����������, ��� ������ �����������
        private bool _isFlying; // ����, �����������, ��� ������ ������������ �� ������
        private float _jumpTimer; // ������ ��� �������
        private float _flyTimer; // ������ ��� ��������� �� ������
        private float _startHeight; // ��������� ������ �������
        private Vector3 _velocity; // ��� ���������� �������������� ��������

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

        private void FixedUpdate()
        {
            if (_isJumping)
            {
                _jumpTimer += Time.fixedDeltaTime;
                float t = Mathf.Clamp01(_jumpTimer / jumpDuration);

                // ������ ��������� ������ �� �������� ������
                Vector3 currentPosition = transform.position;
                float targetY = Mathf.Lerp(_startHeight, flyHeight, t);
                currentPosition.y = targetY;
                transform.position = currentPosition;

                // ��������� �������������� ��������
                _rigidbody.linearVelocity = new Vector3(_velocity.x, 0f, _velocity.z);

                if (t >= 1f)
                {
                    _isJumping = false;
                    _isFlying = true;
                    _flyTimer = flyDuration;
                    _rigidbody.useGravity = false; // ��������� ���������� �� ����� ���������
                }
            }

            if (_isFlying)
            {
                // ���������� ������ �� �������� ������
                Vector3 currentPosition = transform.position;
                currentPosition.y = flyHeight;
                transform.position = currentPosition;

                // ��������� �������������� ��������
                _rigidbody.linearVelocity = new Vector3(_velocity.x, 0f, _velocity.z);

                // ��������� ������ �����
                _flyTimer -= Time.fixedDeltaTime;

                if (_flyTimer <= 0f)
                {
                    // ��������� ����: �������� ���������� ��� �������
                    _rigidbody.useGravity = true;
                    _isFlying = false;
                }
            }
        }

        private void OnJump()
        {
            if (_isJumping || _isFlying) return; // �� ��� �������, ���� ��� � ��������

            // ��������� ������� �������������� ��������
            _velocity = _rigidbody.linearVelocity;

            // ���������� ������������ ��������
            _rigidbody.linearVelocity = new Vector3(_velocity.x, 0f, _velocity.z);

            // �������� ������
            _startHeight = transform.position.y;
            _jumpTimer = 0f;
            _isJumping = true;
            _rigidbody.useGravity = false; // ��������� ���������� �� ����� �������
        }
    }
}