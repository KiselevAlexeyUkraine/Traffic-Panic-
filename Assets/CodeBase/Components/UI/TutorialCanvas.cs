using UnityEngine;
using TMPro;
using Codebase.Components.Player;
//using Codebase.Services.Time;

namespace Codebase.UI
{
    public class TutorialCanvas : MonoBehaviour
    {
        [SerializeField] private PlayerMovement _playerMovement; // Ссылка на PlayerMovement
        [SerializeField] private SpeedModifier _speedModifier; // Ссылка на SpeedModifier
        [SerializeField] private Canvas _canvas; // Ссылка на канвас
        [SerializeField] private TMP_Text _hintText; // Ссылка на текстовый компонент UI
        [SerializeField] private bool IsFirstLevel = true; // Флаг первого уровня

        private const int REQUIRED_LANE_CHANGES = 3; // Требуемое количество перестроений
        private bool _isCompleted = false; // Флаг завершения всех условий
        private float _warningTimer = 0f; // Таймер для временного сообщения
        private const float WARNING_DURATION = 2f; // Длительность временного сообщения
        private bool _showWarning = false; // Флаг отображения предупреждения
        private float _finalMessageTimer = 0f; // Таймер для финального сообщения
        private const float FINAL_MESSAGE_DURATION = 3f; // Длительность финального сообщения
        private bool _showFinalMessage = false; // Флаг отображения финального сообщения

        private void Awake()
        {
            // Проверяем наличие компонентов
            if (_canvas == null)
            {
                Debug.LogError("Canvas is not assigned in TutorialCanvas!");
                _canvas = GetComponent<Canvas>();
                if (_canvas == null)
                {
                    enabled = false;
                    return;
                }
            }

            if (_hintText == null)
            {
                Debug.LogError("HintText is not assigned in TutorialCanvas!");
                enabled = false;
                return;
            }

            if (_playerMovement == null)
            {
                Debug.LogError("PlayerMovement is not assigned in TutorialCanvas!");
                enabled = false;
                return;
            }

            if (_speedModifier == null)
            {
                Debug.LogError("SpeedModifier is not assigned in TutorialCanvas!");
                enabled = false;
                return;
            }

            // Если не первый уровень, отключаем канвас и скрипт
            if (!IsFirstLevel)
            {
                _canvas.enabled = false;
                enabled = false;
                Debug.Log("TutorialCanvas disabled: IsFirstLevel is false.");
            }
        }

        private void Start()
        {
            if (!IsFirstLevel) return;

            // Подписываемся на события
            _playerMovement.OnLaneChanged += UpdateHint;
            _speedModifier.OnBoostUsed += UpdateHint;

            // Подписываемся на попытку ускорения
            _speedModifier.GetComponent<SpeedModifier>().OnBoostUsed += HandleBoostAttempt;

            // Устанавливаем начальный текст
            UpdateHint();
        }

        private void OnDestroy()
        {
            if (!IsFirstLevel) return;

            // Отписываемся от событий
            //_playerMovement.OnLaneChanged -= UpdateHint;
           // _speedModifier.OnBoostUsed -= UpdateHint;
            //_speedModifier.GetComponent<SpeedModifier>().OnBoostUsed -= HandleBoostAttempt;
        }

        private void Update()
        {
            if (!IsFirstLevel) return;

            if (_showWarning)
            {
                _warningTimer -= Time.deltaTime;
                if (_warningTimer <= 0f)
                {
                    _showWarning = false;
                    UpdateHint(); // Возвращаем стандартную подсказку
                }
            }

            if (_showFinalMessage)
            {
                _finalMessageTimer -= Time.deltaTime;
                if (_finalMessageTimer <= 0f)
                {
                    _showFinalMessage = false;
                    _canvas.enabled = false; // Отключаем канвас после финального сообщения
                    Debug.Log("Final message completed, canvas disabled.");
                }
            }
        }

        private void HandleBoostAttempt()
        {
            if (!IsFirstLevel) return;

            // Показываем предупреждение, если игрок пытается ускориться до 3 перестроений
            if (_playerMovement.LaneChangeCount < REQUIRED_LANE_CHANGES && !_isCompleted)
            {
                _hintText.text = "Нажимай на A/D, чтобы перестроиться 3 раза";
                _showWarning = true;
                _warningTimer = WARNING_DURATION;
                Debug.Log("Warning: Complete 3 lane changes before boosting!");
            }
        }

        private void UpdateHint()
        {
            if (!IsFirstLevel || _isCompleted || _showWarning || _showFinalMessage) return;

            // Проверяем прогресс перестроений
            int laneChanges = _playerMovement.LaneChangeCount;
            if (laneChanges < REQUIRED_LANE_CHANGES)
            {
                _hintText.text = $"Нажимай на A/D, чтобы перестроиться 3 раза ({laneChanges}/{REQUIRED_LANE_CHANGES})";
                Debug.Log($"Tutorial hint: Перестройтесь 3 раза ({laneChanges}/{REQUIRED_LANE_CHANGES})");
            }
            else if (!_speedModifier.HasBoosted)
            {
                _hintText.text = "Нажми на W, чтобы ускориться";
                Debug.Log("Tutorial hint: Ускорьтесь 1 раз");
            }
            else
            {
                // Все условия выполнены, показываем финальное сообщение
                _hintText.text = "Отвези клиента к финишу!";
                _showFinalMessage = true;
                _finalMessageTimer = FINAL_MESSAGE_DURATION;
                _isCompleted = true;
                Debug.Log("Showing final message: Отвези клиента к финишу!");
            }
        }
    }
}