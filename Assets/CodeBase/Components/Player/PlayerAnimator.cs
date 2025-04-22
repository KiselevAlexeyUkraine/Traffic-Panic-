using Codebase.Services.Inputs;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace Codebase.Components.Player
{
    public class PlayerAnimator : MonoBehaviour
    {
        [SerializeField] private Transform _carBody;
        [SerializeField] private float _tiltYAngle = 10f;
        [SerializeField] private float _tiltDuration = 0.25f;

        private IInput _playerInput;
        private PlayerMovement _playerMovement;
        private PlayerCollisionHandler _playerCollisionHandler;
        private Tween _tiltTween;

        [Inject]
        private void Construct(IInput input)
        {
            _playerInput = input;
        }

        private void Start()
        {
            _carBody ??= transform;

            _playerMovement = GetComponent<PlayerMovement>();
            _playerCollisionHandler = GetComponent<PlayerCollisionHandler>();

            _playerMovement.OnMoving += HandleLean;
            _playerCollisionHandler.OnPlayerDeath += HandleDeathAnimation;
        }

        private void HandleLean()
        {
            if (_carBody == null || !_carBody.gameObject.activeInHierarchy) return;

            _tiltTween?.Kill();

            float targetY = 0f;
            if (_playerInput.Right) targetY = _tiltYAngle;
            else if (_playerInput.Left) targetY = -_tiltYAngle;

            _tiltTween = _carBody.DOLocalRotate(
                new Vector3(0f, targetY, 0f), _tiltDuration)
                .SetEase(Ease.OutSine)
                .OnComplete(() =>
                {
                    _tiltTween = _carBody.DOLocalRotate(Vector3.zero, _tiltDuration)
                        .SetEase(Ease.OutSine);
                });
        }

        private void HandleDeathAnimation()
        {
            if (_carBody == null || !_carBody.gameObject.activeInHierarchy) return;

            _carBody.DOKill();
            float originalY = _carBody.localPosition.y;

            float rotationY = 0f;
            bool applyLift = false;
            string label = "";
            int effect = Random.Range(0, 5);

            switch (effect)
            {
                case 0:
                    label = "Лёгкая тряска + поворот по Y";
                    rotationY = 20f;
                    _carBody.DOShakePosition(0.4f, new Vector3(0.2f, 0.2f, 0.1f));
                    break;

                case 1:
                    label = "Подброс + поворот по Y";
                    rotationY = -45f;
                    applyLift = true;
                    break;

                case 2:
                    label = "Крутящий разворот на 360°";
                    rotationY = 360f;
                    break;

                case 3:
                    label = "Резкий поворот на 90°";
                    rotationY = 90f;
                    break;

                case 4:
                    label = "Рывок + поворот по Y";
                    rotationY = -90f;
                    _carBody.DOShakePosition(0.3f, new Vector3(0.15f, 0.2f, 0.1f));
                    break;
            }

            Debug.Log($"DeathEffect: {label}");

            _carBody.DOLocalRotate(new Vector3(0f, rotationY, 0f), 0.6f, RotateMode.Fast)
                .SetEase(Ease.OutExpo);

            if (applyLift)
            {
                _carBody.DOLocalMoveY(originalY + 1f, 0.3f)
                    .SetEase(Ease.OutQuad)
                    .OnComplete(() =>
                    {
                        _carBody.DOLocalMoveY(originalY, 0.4f)
                            .SetEase(Ease.InBounce);
                    });
            }
            else
            {
                _carBody.DOLocalMoveY(originalY, 0.3f).SetEase(Ease.OutSine);
            }
        }

        private void OnDestroy()
        {
            _playerMovement.OnMoving -= HandleLean;
            _playerCollisionHandler.OnPlayerDeath -= HandleDeathAnimation;
        }
    }
}