using UnityEngine;

namespace Codebase.Components.Level
{
    public class Tile : MonoBehaviour
    {
        [SerializeField]
        private Renderer _renderer;

        public Bounds GetBounds()
        {
            return _renderer.bounds;
        }
    }
}