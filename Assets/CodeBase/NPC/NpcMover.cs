using UnityEngine;

namespace Codebase.NPC
{
    public class NpcMover : MonoBehaviour
    {
        public enum MoveDirection
        {
            Right,
            Left,
            Stay
        }

        [SerializeField] private float moveDuration = 1.2f;
        [SerializeField] private MoveDirection direction = MoveDirection.Stay;
        [SerializeField] private float minSpeedZ = 3f; // Минимальная скорость по Z
        [SerializeField] private float maxSpeedZ = 7f; // Максимальная скорость по Z
        [SerializeField] private float speedChangeInterval = 2f; // Интервал смены скорости (2 секунды)
        [SerializeField] private float minSpeedZ = 3f;
        [SerializeField] private float maxSpeedZ = 7f;
        [SerializeField] private float speedChangeInterval = 2f;
        [SerializeField] private float carSpeedX = 5f;
        [SerializeField] private Animator animator;

        private readonly float[] _positionsX = { -6f, -3f, 0f, 3f, 6f };

        private Vector3 _startPosition;
        private Vector3 _targetLocalPosition;
        private float _moveTimer;
        private bool _isMoving;
        private float _currentSpeedZ; // Текущая скорость по Z
        private float _speedChangeTimer; // Таймер для смены скорости
        private bool _speedChangeEnabled; // Флаг, разрешающий смену скорости

        private void Start()
        {
            _targetLocalPosition = transform.localPosition;
            // Устанавливаем начальную скорость
            _currentSpeedZ = 0f; // Скорость изначально 0, пока не сработал триггер
            _speedChangeTimer = speedChangeInterval;
            _speedChangeEnabled = false; // Изначально смена скорости отключена
        }
            _currentSpeedZ = 0f;
            _speedChangeTimer = speedChangeInterval;
            _speedChangeEnabled = direction != MoveDirection.Random;

            if (_speedChangeEnabled)
            {
                _currentSpeedZ = Random.Range(minSpeedZ, maxSpeedZ);
            }
        }

        private void Update()
        {
            if (_isMoving)
            {
                _moveTimer += Time.deltaTime;
                float t = Mathf.Clamp01(_moveTimer / moveDuration);
                Vector3 lerpedPosition = Vector3.Lerp(_startPosition, _targetLocalPosition, t);
                // Обновляем только X и Y, сохраняя Z для непрерывного движения
                transform.localPosition = new Vector3(lerpedPosition.x, lerpedPosition.y, transform.localPosition.z);

                if (t >= 1f)
                    _isMoving = false;
            }

            // Непрерывное движение по оси Z
            if (_currentSpeedZ != 0f)
            {
                Vector3 currentPosition = transform.localPosition;
                // Двигаем по Z с текущей скоростью
                currentPosition.z += _currentSpeedZ * Time.deltaTime;
                transform.localPosition = currentPosition;
            }

            // Обновляем таймер смены скорости, только если смена скорости разрешена
            if (_speedChangeEnabled)
            {
                _speedChangeTimer -= Time.deltaTime;
                if (_speedChangeTimer <= 0f)
                {
                    // Меняем скорость в пределах заданного диапазона
                    _currentSpeedZ = Random.Range(minSpeedZ, maxSpeedZ);
                    _speedChangeTimer = speedChangeInterval; // Сбрасываем таймер
                }
            }
        }
                    _speedChangeTimer = speedChangeInterval;
                }

                Vector3 currentPosition = transform.localPosition;
                currentPosition.z += _currentSpeedZ * Time.deltaTime;
                transform.localPosition = currentPosition;
            }
        }

        public void TriggerMove()
        {
            if (_isMoving) return;

            float currentX = Mathf.Round(transform.localPosition.x);
            int currentIndex = System.Array.IndexOf(_positionsX, currentX);
            if (currentIndex == -1) return;

            float? nextX = null;

            switch (direction)
            {
                case MoveDirection.Right:
                    if (currentIndex < _positionsX.Length - 1)
                        nextX = _positionsX[currentIndex + 1];
                    break;

                case MoveDirection.Left:
                    if (currentIndex > 0)
                        nextX = _positionsX[currentIndex - 1];
                    break;

                case MoveDirection.Random:
                    int dir = Random.Range(0, 2);
                    if (dir == 0 && currentIndex > 0)
                    {
                        nextX = _positionsX[currentIndex - 1];
                        actualDirection = MoveDirection.Left;
                    }
                    else if (dir == 1 && currentIndex < _positionsX.Length - 1)
                    {
                        nextX = _positionsX[currentIndex + 1];
                        actualDirection = MoveDirection.Right;
                    }
                    break;

                case MoveDirection.Stay:
                    return;
            }

            if (nextX == null) return;

            if (actualDirection == MoveDirection.Right)
            {
                if (animator != null)
                    animator.Play("Right");
            }
            else if (actualDirection == MoveDirection.Left)
            {
                if (animator != null)
                    animator.Play("Left"); 
            }

            _startPosition = transform.localPosition;
            _targetLocalPosition = new Vector3(nextX.Value, _startPosition.y, _startPosition.z);
            _moveTimer = 0f;
            _isMoving = true;

        private int GetClosestIndex(float value)
        {
            float minDiff = float.MaxValue;
            int closestIndex = -1;
            for (int i = 0; i < _positionsX.Length; i++)
            {
                float diff = Mathf.Abs(_positionsX[i] - value);
                if (diff < minDiff)
                {
                    minDiff = diff;
                    closestIndex = i;
                }
            }
            return closestIndex;
        }
    }
}
