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
        public event Action OnCoinCollected; 

        private void OnTriggerEnter(Collider other)
        {
            int otherLayerMask = 1 << other.gameObject.layer;

            if ((enemyLayer.value & otherLayerMask) != 0)
            {
                HandleEnemyCollision();
            }
            else if ((coinLayer.value & otherLayerMask) != 0)
            {
                HandleCoinPickup(other.gameObject);
            }
            else if ((jumpLayer.value & otherLayerMask) != 0)
            {
                Springboard();
            }
        }

        private void HandleEnemyCollision()
        {
            OnPlayerDeath?.Invoke();
        }

        private void HandleCoinPickup(GameObject coin)
        {
            Destroy(coin);
            OnCoinCollected?.Invoke();
        }

        private void Springboard()
        {
            OnPlayerJump?.Invoke();
        }
    }
}