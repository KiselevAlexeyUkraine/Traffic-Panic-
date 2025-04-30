using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
using Codebase.NPC;
using Codebase.Components.Ui;
using System.Threading;
using Codebase.Services;

namespace Codebase.Components.Player
{
    public class PlayerCollisionHandler : MonoBehaviour
    {
        [Header("Layers")]
        [SerializeField] private LayerMask enemyLayer;
        [SerializeField] private LayerMask coinLayer;
        [SerializeField] private LayerMask jumpLayer;
        [SerializeField] private LayerMask stepTriggerLayer;
        [SerializeField] private LayerMask policeCarTriggerLayer;
        [SerializeField] private LayerMask armorTriggerLayer;
        [SerializeField] private LayerMask magnetTriggerLayer;

        [Header("Settings")]
        [SerializeField] private float jumpCooldown = 1f;

        [Header("References")]
        [SerializeField] private GameObject particleSystemSkillsArmor;
        [SerializeField] private GameObject particleSystemSkillsMagnute;
        [SerializeField] private GameObject magnet;

        public event Action OnPlayerDeath;
        public event Action OnPlayerJump;
        public event Action OnCoinCollected;
        public event Action OnActivateRandomObject;

        private bool isAlive = false;
        public bool IsAlive => isAlive;

        private bool isSkillActive = false;
        private bool isMagnetActive = false;
        private bool canJump = true;

        private float remainingSkillTime;
        private float remainingMagnetTime;

        private float skillDuration;
        private float magnetDuration;

        private CancellationTokenSource skillCts;
        private CancellationTokenSource magnetCts;
        private CancellationTokenSource jumpCooldownCts;

        private void Awake()
        {
            if (SkillProgressService.Instance == null)
            {
                Debug.LogError("SkillProgressService not initialized");
            }
        }

        private void Start()
        {
            Invoke(nameof(RefreshDurations), 0.1f);
        }

        private void RefreshDurations()
        {
            skillDuration = SkillProgressService.Instance.GetSkillDuration("Armor", 2f);
            magnetDuration = SkillProgressService.Instance.GetSkillDuration("Magnet", 4f);

            Debug.Log("[PlayerCollisionHandler] SkillDuration = " + skillDuration);
            Debug.Log("[PlayerCollisionHandler] MagnetDuration = " + magnetDuration);
        }

        public void TriggerSkillByKey(string skillKey)
        {
            switch (skillKey)
            {
                case "Armor":
                    ActivateSkill();
                    break;
                case "Magnet":
                    ActivateMagnet();
                    break;
                default:
                    Debug.LogWarning("Unknown skill key: " + skillKey);
                    break;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            int otherLayerMask = 1 << other.gameObject.layer;

            if ((stepTriggerLayer.value & otherLayerMask) != 0)
            {
                NpcMover npcMover = other.GetComponent<NpcMover>();
                npcMover?.TriggerMove();
            }

            if (!isAlive)
            {
                if ((enemyLayer.value & otherLayerMask) != 0 && !isSkillActive)
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
                else if ((armorTriggerLayer.value & otherLayerMask) != 0)
                {
                    Destroy(other.gameObject);
                    ActivateSkill();
                }
                else if ((magnetTriggerLayer.value & otherLayerMask) != 0)
                {
                    HandleMagnetPickup(other.gameObject);
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
            jumpCooldownCts?.Cancel();
            jumpCooldownCts = new CancellationTokenSource();
            JumpCooldownAsync(jumpCooldownCts.Token).Forget();
        }

        private void Springboard()
        {
            OnPlayerJump?.Invoke();
        }

        private void ActivateRandomObject()
        {
            OnActivateRandomObject?.Invoke();
        }

        public void ActivateSkill()
        {
            RefreshDurations();
            remainingSkillTime += skillDuration;

            if (!isSkillActive)
            {
                skillCts?.Cancel();
                skillCts = new CancellationTokenSource();
                SkillTimerAsync(skillCts.Token).Forget();
            }
        }

        private async UniTaskVoid SkillTimerAsync(CancellationToken token)
        {
            isSkillActive = true;
            particleSystemSkillsArmor.SetActive(true);

            while (remainingSkillTime > 0f)
            {
                await UniTask.Yield(PlayerLoopTiming.Update, token);
                remainingSkillTime -= Time.deltaTime;
            }

            particleSystemSkillsArmor.SetActive(false);
            isSkillActive = false;
        }

        private void HandleMagnetPickup(GameObject magnetPickup)
        {
            Destroy(magnetPickup);
            ActivateMagnet();
        }

        public void ActivateMagnet()
        {
            RefreshDurations();
            remainingMagnetTime += magnetDuration;

            if (!isMagnetActive)
            {
                magnetCts?.Cancel();
                magnetCts = new CancellationTokenSource();
                MagnetTimerAsync(magnetCts.Token).Forget();
            }
        }

        private async UniTaskVoid MagnetTimerAsync(CancellationToken token)
        {
            isMagnetActive = true;
            particleSystemSkillsMagnute.SetActive(true);
            magnet.SetActive(true);

            while (remainingMagnetTime > 0f)
            {
                await UniTask.Yield(PlayerLoopTiming.Update, token);
                remainingMagnetTime -= Time.deltaTime;
            }

            particleSystemSkillsMagnute.SetActive(false);
            magnet.SetActive(false);
            isMagnetActive = false;
        }

        private async UniTaskVoid JumpCooldownAsync(CancellationToken token)
        {
            float cooldown = jumpCooldown;

            while (cooldown > 0f)
            {
                await UniTask.Yield(PlayerLoopTiming.Update, token);
                cooldown -= Time.deltaTime;
            }

            canJump = true;
        }
    }
}
