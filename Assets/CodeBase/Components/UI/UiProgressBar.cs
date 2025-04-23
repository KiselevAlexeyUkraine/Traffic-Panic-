using UnityEngine;
using UnityEngine.UI;
using Codebase.Progress;

namespace Codebase.Components.Ui
{
    public class UiProgressBar : MonoBehaviour
    {
        [SerializeField] private Slider _slider;
        [SerializeField] private ProgressTimer _timer;

        private void Update()
        {
            _slider.value = _timer.Progress;
        }
    }
}