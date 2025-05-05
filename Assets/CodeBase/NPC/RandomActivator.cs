using UnityEngine;
using System.Collections;

namespace Codebase.Components.Player
{
    public class RandomActivator : MonoBehaviour
    {
        [SerializeField] private GameObject prefab; // Один префаб для всех полос
        [SerializeField] private float deactivateDelay = 5f; // Задержка до уничтожения
        [SerializeField] private Vector3 spawnOffset = Vector3.zero; // Смещение позиции спавна
        //[SerializeField] private float randomOffsetRange = 1f; // Диапазон случайного смещения
        [SerializeField]
        private Vector3[] laneOffsets = new Vector3[]
        {
            new Vector3(-6f, 0f, 0f),  // Lane1
            new Vector3(-3f, 0f, 0f),  // Lane2
            new Vector3(0f, 0f, 0f),   // Lane3
            new Vector3(3f, 0f, 0f),   // Lane4
            new Vector3(6f, 0f, 0f)    // Lane5
        }; // Смещения для каждой полосы

        private void Awake()
        {
            if (prefab == null)
            {
                Debug.LogError("Prefab is not assigned!");
                enabled = false;
            }
          
        }

        public void SpawnOnLane(Lane lane)
        {
            if (prefab == null)
            {
                Debug.LogWarning("Prefab is not assigned.");
                return;
            }

            int laneIndex = (int)lane;
            if (laneIndex < 0 || laneIndex >= laneOffsets.Length)
            {
                Debug.LogWarning($"Invalid lane index: {laneIndex}. Using default index 0.");
                laneIndex = 0;
            }

            Vector3 spawnPosition = transform.position + spawnOffset + laneOffsets[laneIndex];
            // spawnPosition.x += Random.Range(-randomOffsetRange, randomOffsetRange); // Случайное смещение
          
            GameObject spawnedObject = Instantiate(prefab, spawnPosition, Quaternion.identity, transform);
           // Debug.LogError(spawnedObject);
            StartCoroutine(DeactivateAfterDelay(spawnedObject));
         
        }

        private IEnumerator DeactivateAfterDelay(GameObject obj)
        {
            yield return new WaitForSeconds(deactivateDelay);
            if (obj != null)
            {
                Destroy(obj); // Уничтожаем объект вместо деактивации
            }
        }
       

    }
}