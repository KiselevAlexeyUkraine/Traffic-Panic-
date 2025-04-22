using UnityEngine;
using System;

namespace Codebase.Components.Player
{
    public class PlayerCollisionHandler : MonoBehaviour
    {
        [SerializeField] private LayerMask enemyLayer;
        [SerializeField] private LayerMask coinLayer;
        [SerializeField] private LayerMask jumpLayer;

        public event Action OnPlayerDeath;
        public event Action OnPlayerJump;

        private void OnCollisionEnter(Collision collision)
        {
            GameObject other = collision.gameObject;
            int otherLayerMask = 1 << other.layer;

            if ((enemyLayer.value & otherLayerMask) != 0)
            {
                HandleEnemyCollision();
            }
            else if ((coinLayer.value & otherLayerMask) != 0)
            {
                HandleCoinPickup(other);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            int otherLayerMask = 1 << other.gameObject.layer;

            if ((jumpLayer.value & otherLayerMask) != 0)
            {
                Springboard();
            }
        }

        private void HandleEnemyCollision()
        {
            Debug.Log("Столкновение с врагом!");
            OnPlayerDeath?.Invoke();
        }

        private void HandleCoinPickup(GameObject coin)
        {
            Debug.Log("Монета подобрана!");
            Destroy(coin);
        }

        private void Springboard()
        {
            Debug.Log("Игрок подпрыгнул");
            OnPlayerJump?.Invoke();
        }
    }
}
