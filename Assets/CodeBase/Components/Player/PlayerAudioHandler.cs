using UnityEngine;

namespace Codebase.Components.Player
{
    [RequireComponent(typeof(AudioSource))]
    public class PlayerAudioHandler : MonoBehaviour
    {
        [Header("Звуки Игрока")]
        [SerializeField] private AudioClip moveClip;
        [SerializeField] private AudioClip jumpClip;
        [SerializeField] private AudioClip coinCollectedClip;
        [SerializeField] private AudioClip deathClip;
        [Space(40)]
        [SerializeField] private PlayerMovement playerMovement;
        [SerializeField] private PlayerJump playerJump;
        [SerializeField] private PlayerCollisionHandler playerCollisionHandler;

        private AudioSource _audioSource;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void OnEnable()
        {
            if (playerMovement != null)
            {
                playerMovement.OnMovingLeft += PlayMoveSound;
                playerMovement.OnMovingRight += PlayMoveSound;
            }
            if (playerJump != null)
            {
                playerJump.OnJumping += PlayJumpSound;
            }
            if (playerCollisionHandler != null)
            {
                playerCollisionHandler.OnCoinCollected += PlayCoinCollectedSound;
                playerCollisionHandler.OnPlayerDeath += PlayDeathSound;
            }
        }

        private void OnDisable()
        {
            if (playerMovement != null)
            {
                playerMovement.OnMovingLeft -= PlayMoveSound;
                playerMovement.OnMovingRight -= PlayMoveSound;
            }
            if (playerJump != null)
            {
                playerJump.OnJumping -= PlayJumpSound;
            }
            if (playerCollisionHandler != null)
            {
                playerCollisionHandler.OnCoinCollected -= PlayCoinCollectedSound;
                playerCollisionHandler.OnPlayerDeath -= PlayDeathSound;
            }
        }

        private void PlayMoveSound()
        {
            PlaySound(moveClip);
        }

        private void PlayJumpSound()
        {
            PlaySound(jumpClip);
        }

        private void PlayCoinCollectedSound()
        {
            PlaySound(coinCollectedClip);
        }

        private void PlayDeathSound()
        {
            PlaySound(deathClip);
        }

        private void PlaySound(AudioClip clip)
        {
            if (clip == null)
                return;

            _audioSource.PlayOneShot(clip);
        }
    }
}
