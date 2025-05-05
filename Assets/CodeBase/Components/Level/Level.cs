using UnityEngine;

namespace Codebase.Components.Level
{
    public class Level : MonoBehaviour
    {
        [field: SerializeField]
        public float Extents { get; private set; }
        [field: SerializeField]
        public float Center { get; private set; }
        [SerializeField] Canvas canvas;
        [SerializeField] Canvas canvas2;
        private void Awake()
        {
            if(canvas != null) canvas.worldCamera = Camera.main;
            if (canvas2 != null) canvas2.worldCamera = Camera.main;
            Tile[] tiles = GetComponentsInChildren<Tile>();

            foreach (Tile tile in tiles)
            {
                Extents += tile.GetBounds().extents.z;
                Center += tile.GetBounds().center.z;
            }

            Center /= tiles.Length;
        }

        public void Move(Vector3 direction, float speed, float acceleration)
        {
            transform.position += direction * ((speed + acceleration * Time.time) * Time.deltaTime);
        }

        public void MoveToEdge(Level lastLevel, Level newLevel)
        {
            Vector3 newLevelCenter = newLevel.transform.TransformPoint(0f, 0f, newLevel.Center);

            Vector3 lastLevelCenter = lastLevel.transform.TransformPoint(0f, 0f, lastLevel.Center);
            Vector3 lastLevelEdge = lastLevelCenter + new Vector3(0f, 0f, lastLevel.Extents);

            Vector3 targetPosition = lastLevelEdge
                        + new Vector3(0f, 0f, newLevel.Extents - (newLevelCenter.z - newLevel.transform.position.z));

            transform.position = targetPosition;
        }
    }
}