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
        [SerializeField] private float minSpeedZ = 3f; // ����������� �������� �� Z
        [SerializeField] private float maxSpeedZ = 7f; // ������������ �������� �� Z
        [SerializeField] private float speedChangeInterval = 2f; // �������� ����� �������� (2 �������)

        private readonly float[] _positionsX = { -6f, -3f, 0f, 3f, 6f };

        private Vector3 _startPosition;
        private Vector3 _targetLocalPosition;
        private float _moveTimer;
        private bool _isMoving;
        private float _currentSpeedZ; // ������� �������� �� Z
        private float _speedChangeTimer; // ������ ��� ����� ��������
        private bool _speedChangeEnabled; // ����, ����������� ����� ��������

        private void Start()
        {
            _targetLocalPosition = transform.localPosition;
            // ������������� ��������� ��������
            _currentSpeedZ = 0f; // �������� ���������� 0, ���� �� �������� �������
            _speedChangeTimer = speedChangeInterval;
            _speedChangeEnabled = false; // ���������� ����� �������� ���������
        }

        private void Update()
        {
            // �������� �� ��� X (������������ ������)
            if (_isMoving)
            {
                _moveTimer += Time.deltaTime;
                float t = Mathf.Clamp01(_moveTimer / moveDuration);
                Vector3 lerpedPosition = Vector3.Lerp(_startPosition, _targetLocalPosition, t);
                // ��������� ������ X � Y, �������� Z ��� ������������ ��������
                transform.localPosition = new Vector3(lerpedPosition.x, lerpedPosition.y, transform.localPosition.z);

                if (t >= 1f)
                    _isMoving = false;
            }

            // ����������� �������� �� ��� Z
            if (_currentSpeedZ != 0f)
            {
                Vector3 currentPosition = transform.localPosition;
                // ������� �� Z � ������� ���������
                currentPosition.z += _currentSpeedZ * Time.deltaTime;
                transform.localPosition = currentPosition;
            }

            // ��������� ������ ����� ��������, ������ ���� ����� �������� ���������
            if (_speedChangeEnabled)
            {
                _speedChangeTimer -= Time.deltaTime;
                if (_speedChangeTimer <= 0f)
                {
                    // ������ �������� � �������� ��������� ���������
                    _currentSpeedZ = Random.Range(minSpeedZ, maxSpeedZ);
                    _speedChangeTimer = speedChangeInterval; // ���������� ������
                }
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

                case MoveDirection.Stay:
                    return;
            }

            if (nextX == null) return; // ��� ��������� ������� � ��������� �����������

            _startPosition = transform.localPosition;
            _targetLocalPosition = new Vector3(nextX.Value, _startPosition.y, _startPosition.z);
            _moveTimer = 0f;
            _isMoving = true;

            // ���������� ����� ��������, ���� ��� ������ ����� TriggerMove
            if (!_speedChangeEnabled)
            {
                _speedChangeEnabled = true;
                _currentSpeedZ = Random.Range(minSpeedZ, maxSpeedZ); // ������������� ��������� ��������
                _speedChangeTimer = speedChangeInterval; // ���������� ������
            }
        }
    }
}