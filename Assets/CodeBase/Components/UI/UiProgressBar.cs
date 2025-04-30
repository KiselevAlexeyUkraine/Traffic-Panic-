using UnityEngine;
using UnityEngine.UI;
using Codebase.Progress;

namespace Codebase.Components.Ui
{
    public class UiProgressBar : MonoBehaviour
    {
        [SerializeField] private Image _fillImage;
        [SerializeField] private ProgressTimer _timer;

        private void Start()
        {
            InvokeRepeating(nameof(UpdateFillAmount), 0f, 1f);
        }

        private void OnDestroy()
        {
            CancelInvoke(nameof(UpdateFillAmount));
        }

        private void UpdateFillAmount()
        {
            _fillImage.fillAmount = _timer.Progress;
            Debug.Log("Прогресс бар обновлён");
        }
    }
}
