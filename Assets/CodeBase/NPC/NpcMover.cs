using UnityEngine;

namespace Codebase.NPC
{
    public class NpcMover : MonoBehaviour
    {
        // добавил выбор куда двигается машина, для более осмысленного левелдизайна
        public enum MoveDirection
        {
            Right,
            Left,
            Stay
        }

        [SerializeField] private float moveDuration = 1.2f;
        [SerializeField] private MoveDirection direction = MoveDirection.Stay;

        private readonly float[] _positionsX = { -6f, -3f, 0f, 3f, 6f };

        private Vector3 _startPosition;
        private Vector3 _targetLocalPosition;
        private float _moveTimer;
        private bool _isMoving;

        private void Start()
        {
            _targetLocalPosition = transform.localPosition;
        }

        private void Update()
        {
            if (_isMoving)
            {
                _moveTimer += Time.deltaTime;
                float t = Mathf.Clamp01(_moveTimer / moveDuration);
                transform.localPosition = Vector3.Lerp(_startPosition, _targetLocalPosition, t);

                if (t >= 1f)
                    _isMoving = false;
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

            if (nextX == null) return; // Нет доступной позиции в выбранном направлении

            _startPosition = transform.localPosition;
            _targetLocalPosition = new Vector3(nextX.Value, _startPosition.y, _startPosition.z);
            _moveTimer = 0f;
            _isMoving = true;
        }
    }
}