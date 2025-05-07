using UnityEngine;

namespace Codebase.NPC
{
    public class NpcMover : MonoBehaviour
    {
        public enum MoveDirection { Right, Left, Stay, Random }

        [SerializeField] private float moveDuration = 1.2f;
        [SerializeField] private MoveDirection direction = MoveDirection.Stay;
        //[SerializeField] private float carSpeedX = 5f;
        [SerializeField] private Animator animator;

        private readonly float[] _positionsX = { -6f, -3f, 0f, 3f, 6f };
        private Vector3 _startPosition;
        private Vector3 _targetLocalPosition;
        private float _moveTimer;
        private bool _isMoving;
        private MoveDirection actualDirection; // “екуща€ фактическа€ направление движени€

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
                Vector3 lerpedPosition = Vector3.Lerp(_startPosition, _targetLocalPosition, t);
                transform.localPosition = new Vector3(lerpedPosition.x, lerpedPosition.y, transform.localPosition.z);

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
            actualDirection = direction; // ”станавливаем текущее направление по умолчанию

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

            // ¬оспроизведение анимации в зависимости от направлени€
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
        }

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