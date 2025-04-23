using UnityEngine;
using UnityEngine.UI;
using Codebase.Progress;

namespace Codebase.Components.Ui
{
    public class UiProgressBar : MonoBehaviour
    {
        [SerializeField] private Slider _slider;
        [SerializeField] private ProgressTimer _timer;

        private void Start()
        {
            InvokeRepeating(nameof(UpdateSlider), 0f, 1f);
        }

        private void OnDestroy()
        {
            CancelInvoke(nameof(UpdateSlider));
        }

        private void UpdateSlider()
        {
            _slider.value = _timer.Progress;
            Debug.Log("Прогресс бар обновлён");
        }
    }
}