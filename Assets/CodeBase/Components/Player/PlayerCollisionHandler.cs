using UnityEngine;
using System;
using Codebase.NPC;

namespace Codebase.Components.Player
{
    public class PlayerCollisionHandler : MonoBehaviour
    {
        [SerializeField] private LayerMask enemyLayer;
        [SerializeField] private LayerMask coinLayer;
        [SerializeField] private LayerMask jumpLayer;
        [SerializeField] private LayerMask stepTriggerLayer;

        public event Action OnPlayerDeath;
        public event Action OnPlayerJump;
        public event Action OnCoinCollected;

        private bool IsAlive = false;

        private void OnTriggerEnter(Collider other)
        {
            int otherLayerMask = 1 << other.gameObject.layer;

            if ((stepTriggerLayer.value & otherLayerMask) != 0)
            {
                NpcMover npcMover = other.GetComponent<NpcMover>();
                if (npcMover != null)
                    npcMover.TriggerMove();
            }

            if (IsAlive == false)
            {
                if ((enemyLayer.value & otherLayerMask) != 0)
                {
                    HandleEnemyCollision();
                    IsAlive = true;
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
