using UnityEngine;

namespace Codebase.NPC
{
    public class NpcMover : MonoBehaviour
    {
        [SerializeField] private float moveDuration = 1.2f;

        private readonly float[] _positionsX = { -4f, -2f, 0f, 2f, 4f };

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

            var adjacentOptions = new System.Collections.Generic.List<float>();
            if (currentIndex > 0) adjacentOptions.Add(_positionsX[currentIndex - 1]);
            if (currentIndex < _positionsX.Length - 1) adjacentOptions.Add(_positionsX[currentIndex + 1]);

            if (adjacentOptions.Count == 0) return;

            float nextX = adjacentOptions[Random.Range(0, adjacentOptions.Count)];
            _startPosition = transform.localPosition;
            _targetLocalPosition = new Vector3(nextX, _startPosition.y, _startPosition.z);
            _moveTimer = 0f;
            _isMoving = true;

        }
    }
}
