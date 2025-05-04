using Codebase.Interfaces;
using UnityEngine;

namespace Codebase.Components
{
    [RequireComponent(typeof(Collider))]
    public class WetZoneTrigger : MonoBehaviour
    {
        [SerializeField] private float speedMultiplier = 1.5f;

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out IWetMove wetMove))
            {
                wetMove.SetSpeedMultiplier(speedMultiplier);
                Debug.Log("������ ��������");
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out IWetMove wetMove))
            {
                wetMove.ResetSpeedMultiplier();
                Debug.Log("������ ���������");
            }
        }
    }
}