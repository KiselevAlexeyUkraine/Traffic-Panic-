using UnityEngine;
using System;
using System.Collections;
using Codebase.NPC;

namespace Codebase.Components.Player
{
    public class PlayerCollisionHandler : MonoBehaviour
    {
        [SerializeField] private LayerMask enemyLayer;
        [SerializeField] private LayerMask coinLayer;
        [SerializeField] private LayerMask jumpLayer;
        [SerializeField] private LayerMask stepTriggerLayer;
        [SerializeField] private LayerMask policeCarTriggerLayer;

        public event Action OnPlayerDeath;
        public event Action OnPlayerJump;
        public event Action OnCoinCollected;
        public event Action OnActivateRandomObject;

        private bool isAlive = false;
        private bool canJump = true;
        private float jumpCooldown = 1f;

        private void OnTriggerEnter(Collider other)
        {
            int otherLayerMask = 1 << other.gameObject.layer;

            if ((stepTriggerLayer.value & otherLayerMask) != 0)
            {
                NpcMover npcMover = other.GetComponent<NpcMover>();
                if (npcMover != null)
                    npcMover.TriggerMove();
            }

            if (!isAlive)
            {
                if ((enemyLayer.value & otherLayerMask) != 0)
                {
                    HandleEnemyCollision();
                    isAlive = true;
                }
                else if ((coinLayer.value & otherLayerMask) != 0)
                {
                    HandleCoinPickup(other.gameObject);
                }
                else if ((jumpLayer.value & otherLayerMask) != 0)
                {
                    TrySpringboard();
                }
                else if ((policeCarTriggerLayer.value & otherLayerMask) != 0)
                {
                    ActivateRandomObject();
                }
            }
        }

        private void HandleEnemyCollision()
        {
            LevelManager.l.HandlePlayerDeath();
            OnPlayerDeath?.Invoke();
        }

        private void HandleCoinPickup(GameObject coin)
        {
            Destroy(coin);
            OnCoinCollected?.Invoke();
        }

        private void TrySpringboard()
        {
            if (!canJump)
                return;

            Springboard();
            canJump = false;
            StartCoroutine(JumpCooldownCoroutine());
        }

        private void Springboard()
        {
            OnPlayerJump?.Invoke();
        }

        private void ActivateRandomObject()
        {
            OnActivateRandomObject?.Invoke();
        }

        private IEnumerator JumpCooldownCoroutine()
        {
            yield return new WaitForSeconds(jumpCooldown);
            canJump = true;
        }
    }
}
