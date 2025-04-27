using UnityEngine;
using System;
using System.Collections;
using Codebase.NPC;
using Codebase.Components.Ui;

namespace Codebase.Components.Player
{
    public class PlayerCollisionHandler : MonoBehaviour
    {
        [SerializeField] private LayerMask enemyLayer;
        [SerializeField] private LayerMask coinLayer;
        [SerializeField] private LayerMask jumpLayer;
        [SerializeField] private LayerMask stepTriggerLayer;
        [SerializeField] private LayerMask policeCarTriggerLayer;
        [SerializeField] private LayerMask ArmorTriggerLayer;

        [SerializeField] private SkillProgressCoin skillProgressCoin;

        public event Action OnPlayerDeath;
        public event Action OnPlayerJump;
        public event Action OnCoinCollected;
        public event Action OnActivateRandomObject;
        //public event Action OnSkillActive;

        private bool isAlive = false;
        public bool IsAlive { get { return isAlive; } }
        private bool IsSkill = false;
        private bool canJump = true;
        private float jumpCooldown = 1f;
        [SerializeField] private float skillTime = 4f;
        [SerializeField] private GameObject particleSystemSkils;

        private void OnEnable()
        {
            skillProgressCoin.OnSkillActivated += CLickSkill;
        }

        private void OnDisable()
        {
            skillProgressCoin.OnSkillActivated -= CLickSkill;
        }

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
                if ((enemyLayer.value & otherLayerMask) != 0 && IsSkill == false)
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
                else if ((ArmorTriggerLayer.value & otherLayerMask) != 0)
                {
                    Destroy(other.gameObject);
                    CLickSkill();
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

        private void CLickSkill()
        {
            IsSkill = true;
            particleSystemSkils.SetActive(true);
            StartCoroutine(SkillTime());
        }

        private IEnumerator SkillTime()
        {
            yield return new WaitForSeconds(skillTime);
            particleSystemSkils.SetActive(false);
            IsSkill = false;
        }
    }
}
