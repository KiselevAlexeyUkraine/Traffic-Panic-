using UnityEngine;
using System;
using Codebase.NPC;
using Codebase.Components.Ui;
using Codebase.Services;
using Codebase.Services.Time;

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
        [SerializeField] private LayerMask FinishTriggerLayer;
        [Header("Settings")]
        [SerializeField] private float jumpCooldown = 1f;

        [Header("References")]
        [SerializeField] private GameObject particleSystemSkillsArmor;
        [SerializeField] private GameObject particleSystemSkillsMagnute;
        [SerializeField] private GameObject particleSystemSkillsNitro;
        [SerializeField] private GameObject magnet;
        [SerializeField] private SpeedModifier speedModifier;
        [SerializeField] private RandomActivator randomActivator;

        public event Action OnPlayerDeath;
        public event Action OnPlayerJump;
        public event Action OnCoinCollected;

        
        private bool isAlive = true; // Исправлено: игрок жив по умолчанию
        public bool IsAlive => isAlive;

        private bool isSkillActive;
        private bool isMagnetActive;
        private bool isNitroActive;
        private bool canJump = true;
        private bool hasArmorProtection;
        private bool hasNitroImmunity;

        private float skillTimeLeft;
        private float magnetTimeLeft;
        private float nitroTimeLeft;
        private float jumpCooldownLeft;

        private float skillDuration;
        private float magnetDuration;
        private float nitroDuration;

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
            ValidateDependencies();
            RefreshDurations();
        }

       

        private void ValidateDependencies()
        {
            if (SkillProgressService.Instance == null)
            {
                Debug.LogError("SkillProgressService not initialized");
                enabled = false;
                return;
            }
            if (speedModifier == null || particleSystemSkillsArmor == null || particleSystemSkillsMagnute == null || particleSystemSkillsNitro == null || magnet == null)
            {
                Debug.LogError("Missing required references in PlayerCollisionHandler");
                enabled = false;
            }
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
                    magnetTimeLeft = 0f; // Исправлено: не сбрасываем на magnetDuration
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
                    speedModifier.enabled = true;
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
            skillDuration = 30f;//SkillProgressService.Instance.GetSkillDuration("Armor", 30f);
            magnetDuration = 15f;//SkillProgressService.Instance.GetSkillDuration("Magnet", 15f);
            nitroDuration = 8f; //SkillProgressService.Instance.GetSkillDuration("Nitro", 8f);

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
            if (other == null) return;

            int otherLayerMask = 1 << other.gameObject.layer;

            if ((stepTriggerLayer.value & otherLayerMask) != 0)
            {
                NpcMover npcMover = other.GetComponent<NpcMover>();
                npcMover?.TriggerMove();
            }
            else if ((policeCarTriggerLayer.value & otherLayerMask) != 0)
            {
                LaneTrigger laneTrigger = other.GetComponent<LaneTrigger>();
                if (laneTrigger == null)
                {
                    Debug.LogError("LaneTrigger component is missing on the police car trigger object.");
                    return;
                }
                if (randomActivator == null)
                {
                    Debug.LogError("RandomActivator is not assigned in PlayerCollisionHandler.");
                    return;
                }
                randomActivator.SpawnOnLane(laneTrigger.SelectedLane);
                Debug.Log("Police car trigger activated, spawning on lane: " + laneTrigger.SelectedLane);
            }

            if (isAlive) // Исправлено: проверяем триггеры, если игрок жив
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
                        isAlive = false; // Исправлено: игрок умирает
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
                else if ((FinishTriggerLayer.value & otherLayerMask) != 0)
                {
                    //Destroy(other.gameObject);
                   // ActivateNitro();

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
            if (!canJump) return;

            Springboard();
            canJump = false;
            jumpCooldownLeft = jumpCooldown;
        }

        private void Springboard()
        {
            OnPlayerJump?.Invoke();
        }

        private void ActivateSkill()
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
            magnetTimeLeft = magnetDuration; // Исправлено: устанавливаем изначально
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
            speedModifier.enabled = false;

            if (!isNitroActive)
            {
                isNitroActive = true;
                particleSystemSkillsNitro.SetActive(true);
                Time.timeScale = 2.5f;
            }
        }
    }
}