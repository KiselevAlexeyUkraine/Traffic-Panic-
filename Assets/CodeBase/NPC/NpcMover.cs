using UnityEngine;
using System.Linq;

namespace Codebase.NPC
{
    public class NpcMover : MonoBehaviour
    {
        [SerializeField] private float moveInterval = 1f;
        [SerializeField] private float moveDuration = 0.5f;

        private readonly float[] _positionsX = { -4f, -2f, 0f, 2f, 4f };

        private Vector3 _startPosition;
        private Vector3 _targetLocalPosition;
        private float _moveTimer;
        private bool _isMoving;

        private void Start()
        {
            _targetLocalPosition = transform.localPosition;
            SetRandomTarget();
            InvokeRepeating(nameof(SetRandomTarget), moveInterval, moveInterval);
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

        private void SetRandomTarget()
        {
            if (_isMoving) return;

            float currentX = Mathf.Round(transform.localPosition.x);
            var options = _positionsX.Where(x => Mathf.Abs(x - currentX) > 0.1f).ToArray();
            if (options.Length == 0) return;

            float nextX = options[Random.Range(0, options.Length)];
            _startPosition = transform.localPosition;
            _targetLocalPosition = new Vector3(nextX, _startPosition.y, _startPosition.z);
            _moveTimer = 0f;
            _isMoving = true;
        }
    }
}
