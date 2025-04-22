using UnityEngine;
using Unity.Cinemachine;

namespace Codebase.Components.Player
{
    public class CameraShaker : MonoBehaviour
    {
        [SerializeField] private CinemachineImpulseSource _impulseSource;
        [SerializeField] private PlayerCollisionHandler _playerCollisionHandler;

        private void OnEnable()
        {
            if (_playerCollisionHandler != null)
                _playerCollisionHandler.OnPlayerDeath += Shake;
        }

        private void OnDisable()
        {
            if (_playerCollisionHandler != null)
                _playerCollisionHandler.OnPlayerDeath -= Shake;
        }

        private void Shake()
        {
            _impulseSource.GenerateImpulse(Vector3.forward);
        }
    }
}
