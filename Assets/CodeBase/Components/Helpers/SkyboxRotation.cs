using UnityEngine;

namespace Codebase.Components.Helpers
{
    public class SkyboxRotation : MonoBehaviour
    {
        [SerializeField] private Material skybox;
        [SerializeField] private float angleSpeed = 1f;
        [SerializeField] private Gradient gradient;

        private float _angle;

        private static readonly int Tint = Shader.PropertyToID("_Tint");
        private static readonly int Rotation = Shader.PropertyToID("_Rotation");

        private void Update()
        {
            _angle += angleSpeed * Time.deltaTime;

            if (_angle > 360f)
            {
                _angle = 0f;
            }

            skybox.SetColor(Tint, gradient.Evaluate(_angle / 360f));
            skybox.SetFloat(Rotation, _angle);
        }
    }
}
