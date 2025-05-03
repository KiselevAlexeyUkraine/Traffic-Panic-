using UnityEngine;

public class BillboardCanvas : MonoBehaviour
{
    [SerializeField] private Camera targetCamera; //  амера, на которую должен смотреть Canvas
    [SerializeField] private bool lockYRotation = true; // ‘иксировать ли вращение по оси Y (чтобы Canvas оставалс€ вертикальным)

    private void Awake()
    {
        // ≈сли камера не указана в инспекторе, используем главную камеру по умолчанию
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        if (targetCamera == null)
        {
            Debug.LogError("Target camera not found! Please assign a camera to the BillboardCanvas script.");
            enabled = false;
        }
    }

    private void LateUpdate()
    {
        if (targetCamera == null) return;

        // ѕолучаем направление от Canvas к камере
        Vector3 directionToCamera = targetCamera.transform.position - transform.position;

        if (lockYRotation)
        {
            // ќбнул€ем Y-компоненту направлени€, чтобы Canvas оставалс€ вертикальным
            directionToCamera.y = 0;
        }

        // ≈сли направление не нулевое, поворачиваем Canvas
        if (directionToCamera != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(directionToCamera);
        }
    }
}