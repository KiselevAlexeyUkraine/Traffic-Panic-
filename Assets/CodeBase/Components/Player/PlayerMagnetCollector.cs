using UnityEngine;
using System;

namespace Codebase.Components.Player
{
    public class PlayerMagnetCollector : MonoBehaviour
    {
        [SerializeField] private LayerMask coinLayer;

        public event Action OnCoinCollected;

        private void Awake()
        {
            gameObject.SetActive(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (IsCoin(other))
            {
                HandleCoinPickup(other.gameObject);
            }
        }

        private bool IsCoin(Collider collider)
        {
            int otherLayerMask = 1 << collider.gameObject.layer;
            return (coinLayer.value & otherLayerMask) != 0;
        }

        private void HandleCoinPickup(GameObject coin)
        {
            if (coin != null)
            {
                Destroy(coin);
                OnCoinCollected?.Invoke();
            }
        }
    }
}
