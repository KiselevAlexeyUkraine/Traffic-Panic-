using UnityEngine;
using System;
using Codebase.NPC;
using Codebase.Components.Ui;
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
        [SerializeField] private LayerMask nitroTriggerLayer;

        [Header("Settings")]
        [SerializeField] private float jumpCooldown = 1f;

        [Header("References")]
        [SerializeField] private GameObject particleSystemSkillsArmor;
        [SerializeField] private GameObject particleSystemSkillsMagnute;
        [SerializeField] private GameObject particleSystemSkillsNitro;
        [SerializeField] private GameObject magnet;
        [SerializeField] private RandomActivator randomActivator;

        public event Action OnPlayerDeath;
        public event Action OnPlayerJump;
        public event Action OnCoinCollected;

        private bool isAlive = false;
        public bool IsAlive => isAlive;

        private bool isSkillActive = false;
        private bool isMagnetActive = false;
        private bool isNitroActive = false;
        private bool canJump = true;
        private bool hasArmorProtection = false;
        private bool hasNitroImmunity = false;

        private float skillTimeLeft;
        private float magnetTimeLeft;
        private float nitroTimeLeft;
        private float jumpCooldownLeft;

        private float skillDuration;
        private float remainingSkillTime;
        private float remainingMagnetTime;
        private float skillDuration;
        private float magnetDuration;
        private float nitroDuration;

        private void Awake()
        {
            if (SkillProgressService.Instance == null)
            {
                Debug.LogError("SkillProgressService not initialized");
            }
        }
        public enum Lane
        {
            Lane1, // x = -6
            Lane2, // x = -4
            Lane3, // x = 0
            Lane4, // x = 4
            Lane5  // x = 6
        }
        private void Start()
        {
            Invoke(nameof(RefreshDurations), 0.1f);
        }

        private void Update()
        {
            if (isSkillActive)
            {
                skillTimeLeft -= Time.deltaTime;
                if (skillTimeLeft <= 0f)
                {
                    skillTimeLeft = 0f;
                    isSkillActive = false;
                    hasArmorProtection = false;
                    particleSystemSkillsArmor.SetActive(false);
                }
            }

            if (isMagnetActive)
            {
                magnetTimeLeft -= Time.deltaTime;
                if (magnetTimeLeft <= 0f)
                {
                    magnetTimeLeft = 0f;
                    isMagnetActive = false;
                    particleSystemSkillsMagnute.SetActive(false);
                    magnet.SetActive(false);
                }
            }

            if (isNitroActive)
            {
                nitroTimeLeft -= Time.deltaTime;
                if (nitroTimeLeft <= 0f)
                {
                    nitroTimeLeft = 0f;
                    isNitroActive = false;
                    hasNitroImmunity = false;
                    Time.timeScale = 1f;
                    particleSystemSkillsNitro.SetActive(false);
                }
            }

            if (!canJump)
            {
                jumpCooldownLeft -= Time.deltaTime;
                if (jumpCooldownLeft <= 0f)
                {
                    jumpCooldownLeft = 0f;
                    canJump = true;
                }
            }
        }

        private void RefreshDurations()
        {
            skillDuration = SkillProgressService.Instance.GetSkillDuration("Armor", 2f);
            magnetDuration = SkillProgressService.Instance.GetSkillDuration("Magnet", 4f);
            nitroDuration = SkillProgressService.Instance.GetSkillDuration("Nitro", 3f);

            Debug.Log("[PlayerCollisionHandler] SkillDuration = " + skillDuration);
            Debug.Log("[PlayerCollisionHandler] MagnetDuration = " + magnetDuration);
            Debug.Log("[PlayerCollisionHandler] NitroDuration = " + nitroDuration);
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
                case "Nitro":
                    ActivateNitro();
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
            else if ((policeCarTriggerLayer.value & otherLayerMask) != 0)
            {
                LaneTrigger laneTrigger = other.GetComponent<LaneTrigger>();
                if (randomActivator != null)
                {
                    randomActivator.ActivateOnLane(laneTrigger.SelectedLane);
                }
                else
                {
                    Debug.LogWarning("LaneTrigger or RandomActivator is missing on police car trigger.");
                }
            }

            if (!isAlive)
            {
                if ((enemyLayer.value & otherLayerMask) != 0)
                {
                    if (isSkillActive && hasArmorProtection)
                    {
                        hasArmorProtection = false;
                        isSkillActive = false;
                        skillTimeLeft = 0f;
                        particleSystemSkillsArmor.SetActive(false);
                    }
                    else if (isNitroActive && hasNitroImmunity)
                    {
                        return;
                    }
                    else
                    {
                        HandleEnemyCollision();
                        isAlive = true;
                    }
                }
                else if ((coinLayer.value & otherLayerMask) != 0)
                {
                    HandleCoinPickup(other.gameObject);
                }
                else if ((jumpLayer.value & otherLayerMask) != 0)
                {
                    TrySpringboard();
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
                else if ((nitroTriggerLayer.value & otherLayerMask) != 0)
                {
                    Destroy(other.gameObject);
                    ActivateNitro();
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
            jumpCooldownLeft = jumpCooldown;
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
            skillTimeLeft = skillDuration;
            hasArmorProtection = true;

            if (!isSkillActive)
            {
                isSkillActive = true;
                particleSystemSkillsArmor.SetActive(true);
            }
        }

        private void HandleMagnetPickup(GameObject magnetPickup)
        {
            Destroy(magnetPickup);
            ActivateMagnet();
        }

        public void ActivateMagnet()
        {
            RefreshDurations();
            magnetTimeLeft += magnetDuration;

            if (!isMagnetActive)
            {
                isMagnetActive = true;
                particleSystemSkillsMagnute.SetActive(true);
                magnet.SetActive(true);
            }
        }

        public void ActivateNitro()
        {
            RefreshDurations();
            nitroTimeLeft = nitroDuration;
            hasNitroImmunity = true;

            if (!isNitroActive)
            {
                isNitroActive = true;
                particleSystemSkillsNitro.SetActive(true);
                Time.timeScale = 3f;
            }
        }
    }
}