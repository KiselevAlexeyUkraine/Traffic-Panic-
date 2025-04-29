using UnityEngine;

namespace Codebase.Components.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerJump : MonoBehaviour
    {
        [SerializeField] private float flyHeight = 5f; // Высота, на которой машина "летает"
        [SerializeField] private float flyDuration = 1f; // Длительность полёта в воздухе
        [SerializeField] private float jumpDuration = 1f; // Длительность подъёма до высоты

        private PlayerCollisionHandler _playerCollisionHandler;
        private Rigidbody _rigidbody;
        private bool _isJumping; // Флаг, указывающий, что машина поднимается
        private bool _isFlying; // Флаг, указывающий, что машина удерживается на высоте
        private float _jumpTimer; // Таймер для подъёма
        private float _flyTimer; // Таймер для удержания на высоте
        private float _startHeight; // Начальная высота подъёма
        private Vector3 _velocity; // Для сохранения горизонтальной скорости

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

                // Плавно поднимаем машину до заданной высоты
                Vector3 currentPosition = transform.position;
                float targetY = Mathf.Lerp(_startHeight, flyHeight, t);
                currentPosition.y = targetY;
                transform.position = currentPosition;

                // Сохраняем горизонтальную скорость
                _rigidbody.linearVelocity = new Vector3(_velocity.x, 0f, _velocity.z);

                if (t >= 1f)
                {
                    _isJumping = false;
                    _isFlying = true;
                    _flyTimer = flyDuration;
                    _rigidbody.useGravity = false; // Отключаем гравитацию во время удержания
                }
            }

            if (_isFlying)
            {
                // Удерживаем машину на заданной высоте
                Vector3 currentPosition = transform.position;
                currentPosition.y = flyHeight;
                transform.position = currentPosition;

                // Сохраняем горизонтальную скорость
                _rigidbody.linearVelocity = new Vector3(_velocity.x, 0f, _velocity.z);

                // Уменьшаем таймер полёта
                _flyTimer -= Time.fixedDeltaTime;

                if (_flyTimer <= 0f)
                {
                    // Завершаем полёт: включаем гравитацию для падения
                    _rigidbody.useGravity = true;
                    _isFlying = false;
                }
            }
        }

        private void OnJump()
        {
            if (_isJumping || _isFlying) return; // Не даём прыгать, если уже в процессе

            // Сохраняем текущую горизонтальную скорость
            _velocity = _rigidbody.linearVelocity;

            // Сбрасываем вертикальную скорость
            _rigidbody.linearVelocity = new Vector3(_velocity.x, 0f, _velocity.z);

            // Начинаем подъём
            _startHeight = transform.position.y;
            _jumpTimer = 0f;
            _isJumping = true;
            _rigidbody.useGravity = false; // Отключаем гравитацию на время подъёма
        }
    }
}