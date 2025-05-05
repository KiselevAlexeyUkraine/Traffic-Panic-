using UnityEngine;
using UnityEngine.UI;
using Codebase.Progress;

namespace Codebase.Components.Ui
{
    public class UiProgressBar : MonoBehaviour
    {
        [SerializeField] private Image _fillImage;

        private ProgressLevel _timer;


        private void Start()
        {
            if (_timer == null)
                _timer = FindFirstObjectByType<ProgressLevel>();

            if (_timer == null)
            {
                Debug.LogError("ProgressLevel не найден на сцене.");
                enabled = false;
                return;
            }
            InvokeRepeating(nameof(UpdateFillAmount), 0f, 1f);
        }

        private void OnDestroy()
        {
            CancelInvoke(nameof(UpdateFillAmount));
        }

        private void UpdateFillAmount()
        {
            if (_fillImage != null)
            {
                _fillImage.fillAmount = _timer.Progress;
                Debug.Log("Прогресс бар обновлён");
            }
        }
    }
}
