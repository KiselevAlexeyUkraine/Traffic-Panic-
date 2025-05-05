using UnityEngine;
using TMPro;
using Codebase.Components.Player;
//using Codebase.Services.Time;

namespace Codebase.UI
{
    public class TutorialCanvas : MonoBehaviour
    {
        [SerializeField] private PlayerMovement _playerMovement; // ������ �� PlayerMovement
        [SerializeField] private SpeedModifier _speedModifier; // ������ �� SpeedModifier
        [SerializeField] private Canvas _canvas; // ������ �� ������
        [SerializeField] private TMP_Text _hintText; // ������ �� ��������� ��������� UI
        [SerializeField] private bool IsFirstLevel = true; // ���� ������� ������

        private const int REQUIRED_LANE_CHANGES = 3; // ��������� ���������� ������������
        private bool _isCompleted = false; // ���� ���������� ���� �������
        private float _warningTimer = 0f; // ������ ��� ���������� ���������
        private const float WARNING_DURATION = 2f; // ������������ ���������� ���������
        private bool _showWarning = false; // ���� ����������� ��������������
        private float _finalMessageTimer = 0f; // ������ ��� ���������� ���������
        private const float FINAL_MESSAGE_DURATION = 3f; // ������������ ���������� ���������
        private bool _showFinalMessage = false; // ���� ����������� ���������� ���������

        private void Awake()
        {
            // ��������� ������� �����������
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

            // ���� �� ������ �������, ��������� ������ � ������
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

            // ������������� �� �������
            _playerMovement.OnLaneChanged += UpdateHint;
            _speedModifier.OnBoostUsed += UpdateHint;

            // ������������� �� ������� ���������
            _speedModifier.GetComponent<SpeedModifier>().OnBoostUsed += HandleBoostAttempt;

            // ������������� ��������� �����
            UpdateHint();
        }

        private void OnDestroy()
        {
            if (!IsFirstLevel) return;

            // ������������ �� �������
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
                    UpdateHint(); // ���������� ����������� ���������
                }
            }

            if (_showFinalMessage)
            {
                _finalMessageTimer -= Time.deltaTime;
                if (_finalMessageTimer <= 0f)
                {
                    _showFinalMessage = false;
                    _canvas.enabled = false; // ��������� ������ ����� ���������� ���������
                    Debug.Log("Final message completed, canvas disabled.");
                }
            }
        }

        private void HandleBoostAttempt()
        {
            if (!IsFirstLevel) return;

            // ���������� ��������������, ���� ����� �������� ���������� �� 3 ������������
            if (_playerMovement.LaneChangeCount < REQUIRED_LANE_CHANGES && !_isCompleted)
            {
                _hintText.text = "������� �� A/D, ����� ������������� 3 ����";
                _showWarning = true;
                _warningTimer = WARNING_DURATION;
                Debug.Log("Warning: Complete 3 lane changes before boosting!");
            }
        }

        private void UpdateHint()
        {
            if (!IsFirstLevel || _isCompleted || _showWarning || _showFinalMessage) return;

            // ��������� �������� ������������
            int laneChanges = _playerMovement.LaneChangeCount;
            if (laneChanges < REQUIRED_LANE_CHANGES)
            {
                _hintText.text = $"������� �� A/D, ����� ������������� 3 ���� ({laneChanges}/{REQUIRED_LANE_CHANGES})";
                Debug.Log($"Tutorial hint: ������������� 3 ���� ({laneChanges}/{REQUIRED_LANE_CHANGES})");
            }
            else if (!_speedModifier.HasBoosted)
            {
                _hintText.text = "����� �� W, ����� ����������";
                Debug.Log("Tutorial hint: ���������� 1 ���");
            }
            else
            {
                // ��� ������� ���������, ���������� ��������� ���������
                _hintText.text = "������ ������� � ������!";
                _showFinalMessage = true;
                _finalMessageTimer = FINAL_MESSAGE_DURATION;
                _isCompleted = true;
                Debug.Log("Showing final message: ������ ������� � ������!");
            }
        }
    }
}