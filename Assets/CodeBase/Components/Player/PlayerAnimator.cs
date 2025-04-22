using Codebase.Services.Inputs;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace Codebase.Components.Player
{
    public class PlayerAnimator : MonoBehaviour
    {
        private Transform _carBody;
        [SerializeField] private float _tiltYAngle = 10f;
        [SerializeField] private float _tiltDuration = 0.25f;

        private IInput _playerInput;
        private PlayerMovement _playerMovement;
        private Tween _tiltTween;

        [Inject]
        private void Construct(IInput input)
        {
            _playerInput = input;
        }

        private void Start()
        {
            _playerMovement = GetComponent<PlayerMovement>();
            _playerMovement.OnMoving += HandleLean;
            _carBody = transform;
        }

        private void HandleLean()
        {
            _tiltTween?.Kill();

            float targetY = 0f;

            if (_playerInput.Right)
                targetY = _tiltYAngle;
            else if (_playerInput.Left)
                targetY = -_tiltYAngle;

            _tiltTween = _carBody.DOLocalRotate(
                new Vector3(0f, targetY, 0f), _tiltDuration)
                .SetEase(Ease.OutSine)
                .OnComplete(() =>
                {
                    _tiltTween = _carBody.DOLocalRotate(Vector3.zero, _tiltDuration)
                        .SetEase(Ease.OutSine);
                });
        }

        private void OnDestroy()
        {
            _playerMovement.OnMoving -= HandleLean;
        }
    }
}
