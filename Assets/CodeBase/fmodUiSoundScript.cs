using UnityEngine;
using UnityEngine.EventSystems;
using FMODUnity;

public class FMODButtonSounds : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    [SerializeField] private EventReference hoverSound;
    [SerializeField] private EventReference clickSound;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverSound.IsNull) return;
        RuntimeManager.PlayOneShot(hoverSound);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (clickSound.IsNull) return;
        RuntimeManager.PlayOneShot(clickSound);
    }
}
