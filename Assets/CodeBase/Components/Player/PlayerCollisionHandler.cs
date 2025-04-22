using UnityEngine;
using System;

namespace Codebase.Components.Player
{
    public class PlayerCollisionHandler : MonoBehaviour
    {
        [SerializeField] private LayerMask enemyLayer;
        [SerializeField] private LayerMask coinLayer;

        public event Action OnPlayerDeath;

        private void OnCollisionEnter(Collision collision)
        {
            GameObject other = collision.gameObject;
            int otherLayerMask = 1 << other.layer;

            if ((enemyLayer.value & otherLayerMask) != 0)
            {
                HandleEnemyCollision(other);
            }
            else if ((coinLayer.value & otherLayerMask) != 0)
            {
                HandleCoinPickup(other);
            }
        }

        private void HandleEnemyCollision(GameObject enemy)
        {
            Debug.Log("Столкновение с врагом!");

            OnPlayerDeath?.Invoke();
        }

        private void HandleCoinPickup(GameObject coin)
        {
            Debug.Log("Монета подобрана!");
            Destroy(coin);
        }
    }
}
