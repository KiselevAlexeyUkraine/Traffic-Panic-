using UnityEngine;

namespace Codebase.Components.Player
{
    [RequireComponent(typeof(AudioSource))]
    public class PlayerAudioHandler : MonoBehaviour
    {
        [Header("����� ������")]
        [SerializeField] private AudioClip moveClip;
        [SerializeField] private AudioClip jumpClip;
        [SerializeField] private AudioClip coinCollectedClip;
        [SerializeField] private AudioClip deathClip;
        [Space(40)]
        [SerializeField] private PlayerMovement playerMovement;
        [SerializeField] private PlayerCollisionHandler playerCollisionHandler;
        [SerializeField] private PlayerMagnetCollector playerMagnetCollector;

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

            if (playerCollisionHandler != null)
            {
                playerCollisionHandler.OnCoinCollected += PlayCoinCollectedSound;
                playerCollisionHandler.OnPlayerDeath += PlayDeathSound;
                playerCollisionHandler.OnPlayerJump += PlayJumpSound;
            }
            playerMagnetCollector.OnCoinCollected += PlayCoinCollectedSound;
        }

        private void OnDisable()
        {
            if (playerMovement != null)
            {
                playerMovement.OnMovingLeft -= PlayMoveSound;
                playerMovement.OnMovingRight -= PlayMoveSound;
            }
            if (playerCollisionHandler != null)
            {
                playerCollisionHandler.OnCoinCollected -= PlayCoinCollectedSound;
                playerCollisionHandler.OnPlayerDeath -= PlayDeathSound;
                playerCollisionHandler.OnPlayerJump -= PlayJumpSound;
            }
            playerMagnetCollector.OnCoinCollected -= PlayCoinCollectedSound;
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
