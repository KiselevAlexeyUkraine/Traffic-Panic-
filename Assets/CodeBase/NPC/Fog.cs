using UnityEngine;

public class Fog : MonoBehaviour
{
    [SerializeField] private float visibilityReduction = 0.5f; // ����������� ���������� ��������� (0-1)
    [SerializeField] private Color fogColor = new Color(0.5f, 0.5f, 0.5f, 0.5f); // ���� ������
    [SerializeField] private float fogDensity = 0.1f; // ��������� ������

    private bool playerInside = false;
    private Color originalFogColor;
    private float originalFogDensity;
    private float originalFarClipPlane;

    private void Start()
    {
        originalFogColor = RenderSettings.fogColor;
        originalFogDensity = RenderSettings.fogDensity;
        originalFarClipPlane = Camera.main.farClipPlane;
        RenderSettings.fog = false; // ��������� ���������� ����� �� ���������
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            ApplyFog();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            ResetFog();
        }
    }

    private void Update()
    {
        if (playerInside)
        {
            ApplyFog();
        }
    }

    private void ApplyFog()
    {
        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.Exponential;
        RenderSettings.fogColor = fogColor;
        RenderSettings.fogDensity = fogDensity;
        Camera.main.farClipPlane = originalFarClipPlane * visibilityReduction;
    }

    private void ResetFog()
    {
        RenderSettings.fog = false;
        RenderSettings.fogColor = originalFogColor;
        RenderSettings.fogDensity = originalFogDensity;
        Camera.main.farClipPlane = originalFarClipPlane;
    }
}