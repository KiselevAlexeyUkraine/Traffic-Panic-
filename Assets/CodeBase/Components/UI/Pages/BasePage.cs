using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Codebase.Components.Ui.Pages

{
    public class BasePage : MonoBehaviour
    {
        public Action OnOpen;
        public Action OnClose;
        public Action Opened;
        public Action Closed;

        public PageName pageName;

        public Vector3 openFade = new Vector3(0f, 1f, 0.2f);
        public Vector3 closeFade = new Vector3(1f, 0f, 0.2f);

        [SerializeField]
        private CanvasGroup _group;

        public PageSwitcher PageSwitcher { protected get; set; }

        private void Awake()
        {
            _group.alpha = 0f;
        }

        public async UniTask Open()
        {
            OnOpen?.Invoke();
            gameObject.SetActive(true);
            await Fade(openFade.x, openFade.y, openFade.z);
            Opened?.Invoke();
        }

        public void OpenInstantly()
        {
            OnOpen?.Invoke();
            gameObject.SetActive(true);
            Opened?.Invoke();
        }

        public async UniTask Close()
        {
            OnClose?.Invoke();
            await Fade(closeFade.x, closeFade.y, closeFade.z);
            gameObject.SetActive(false);
            Closed?.Invoke();
        }

        public void CloseInstantly()
        {
            OnClose?.Invoke();
            gameObject.SetActive(false);
            Closed?.Invoke();
        }

        private async UniTask Fade(float start, float end, float duration)
        {
            var elapsed = 0f;

            while (elapsed <= duration)
            {
                elapsed += Time.unscaledDeltaTime;
                _group.alpha = Mathf.Lerp(start, end, elapsed / duration);
                await UniTask.NextFrame();
            }
        }
    }

}
