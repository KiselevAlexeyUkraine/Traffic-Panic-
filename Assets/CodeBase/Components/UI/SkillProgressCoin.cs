using UnityEngine;
using UnityEngine.UI;
using Codebase.Components.Player;
using Codebase.Services.Inputs;
using Codebase.Services;
using Zenject;

namespace Codebase.Components.Ui
{
    public class SkillProgressCoin : MonoBehaviour
    {
        [SerializeField] private Image _progressImage;
        [SerializeField] private PlayerCollisionHandler _collisionHandler;
        [SerializeField] private PlayerMagnetCollector playerMagnetCollector;
        [SerializeField] private float _progressPerCoin = 0.1f;
        [SerializeField] private GameObject TextReady;
        [Inject]
        private void Construct(DesktopInput desktopInput)
        {
            _playerInput = desktopInput;
        }

        private IInput _playerInput;

        private float _currentProgress = 0f;
        private bool _skillReady = false;

        private void Awake()
        {
            ResetProgress();
            _collisionHandler.OnCoinCollected += HandleCoinCollected;
            playerMagnetCollector.OnCoinCollected += HandleCoinCollected;
        }

        private void OnDestroy()
        {
            _collisionHandler.OnCoinCollected -= HandleCoinCollected;
            playerMagnetCollector.OnCoinCollected -= HandleCoinCollected;
        }

        private void Update()
        {
            if (_skillReady && _playerInput.Action)
            {
                TextReady.SetActive(false);
                Debug.Log("Skill is good!");
                var skillKey = SkillSelectorPersistent.Instance.SelectedSkill.ToString();
                _collisionHandler.TriggerSkillByKey(skillKey);
                ResetProgress();
            }
        }

        private void HandleCoinCollected()
        {
            if (_skillReady)
                return;

            _currentProgress += _progressPerCoin;
            _currentProgress = Mathf.Clamp01(_currentProgress);
            UpdateProgressImage();

            if (Mathf.Approximately(_currentProgress, 1f))
            {
                TextReady.SetActive(true);
                _skillReady = true;
            }
        }

        private void UpdateProgressImage()
        {
            if (_progressImage != null)
            {
                _progressImage.fillAmount = _currentProgress;
            }
        }

        public void ResetProgress()
        {
            TextReady.SetActive(false);
            _currentProgress = 0f;
            _skillReady = false;
            UpdateProgressImage();
        }
    }
}
