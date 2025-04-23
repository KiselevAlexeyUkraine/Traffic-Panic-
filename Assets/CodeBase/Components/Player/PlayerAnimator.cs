using UnityEngine;

namespace Codebase.Components.Player
{
    public class PlayerAnimator : MonoBehaviour
    {
        private Animator _animator;
        private PlayerMovement _playerMovement;
        private PlayerCollisionHandler _playerCollisionHandler;
        private PlayerJump playerJump;


        private void Start()
        {
            _playerMovement = GetComponent<PlayerMovement>();
            _playerCollisionHandler = GetComponent<PlayerCollisionHandler>();
            playerJump = GetComponent<PlayerJump>();
            _animator = GetComponent<Animator>();
            playerJump.OnJumping += HandJump;
            _playerMovement.OnMovingLeft += HandleLeanLeft;
            _playerMovement.OnMovingRight += HandleLeanRight;
            _playerCollisionHandler.OnPlayerDeath += HandleDeathAnimation;
        }

        private void HandJump()
        {
            _animator.SetTrigger("Jump");
        }

        private void HandleLeanLeft()
        {
            _animator.SetTrigger("LeftMove");
        }

        private void HandleLeanRight()
        {
            _animator.SetTrigger("RightMove");
        }

        private void HandleDeathAnimation()
        {
            int random = Random.Range(0, 2);

            switch (random)
            {
                case 0:
                    _animator.Play("Death_1");
                    break;
                case 1:
                    _animator.Play("Death_2");
                    break;
                //case 2:
                //    _animator.Play("Death_3");
                //    break;
                //case 3:
                //    _animator.Play("Death_4");
                //    break;
                default:
                    Debug.Log("Нет анимаций");
                    break;
            }
            //_animator.Play("Death_1");
            //Debug.Log("Столкновение с врагом!");
        }

        private void OnDestroy()
        {
            _playerMovement.OnMovingLeft -= HandleLeanLeft;
            _playerMovement.OnMovingRight -= HandleLeanRight;
            _playerCollisionHandler.OnPlayerDeath -= HandleDeathAnimation;
            playerJump.OnJumping -= HandJump;
        }
    }
}