using UnityEngine;

public class BillboardCanvas : MonoBehaviour
{
    [SerializeField] private Camera targetCamera; // ������, �� ������� ������ �������� Canvas
    [SerializeField] private bool lockYRotation = true; // ����������� �� �������� �� ��� Y (����� Canvas ��������� ������������)

    private void Awake()
    {
        // ���� ������ �� ������� � ����������, ���������� ������� ������ �� ���������
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

        // �������� ����������� �� Canvas � ������
        Vector3 directionToCamera = targetCamera.transform.position - transform.position;

        if (lockYRotation)
        {
            // �������� Y-���������� �����������, ����� Canvas ��������� ������������
            directionToCamera.y = 0;
        }

        // ���� ����������� �� �������, ������������ Canvas
        if (directionToCamera != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(directionToCamera);
        }
    }
}