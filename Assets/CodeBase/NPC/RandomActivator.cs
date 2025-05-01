using UnityEngine;
using System.Collections;

namespace Codebase.Components.Player
{
    public class RandomActivator : MonoBehaviour
    {
        [SerializeField] private GameObject[] objects; // Один объект на каждую полосу
        [SerializeField] private float deactivateDelay = 5f;

        private GameObject activeObject;

        public void ActivateOnLane(Lane lane)
        {
            if (objects == null || objects.Length == 0 || objects.Length < 5)
            {
                Debug.LogWarning("Objects array is invalid or does not contain enough objects for all lanes.");
                return;
            }

            if (activeObject != null)
                activeObject.SetActive(false);

            int laneIndex = (int)lane;
            if (laneIndex >= 0 && laneIndex < objects.Length)
            {
                activeObject = objects[laneIndex];

                if (activeObject != null)
                {
                    activeObject.SetActive(true);
                    StartCoroutine(DeactivateAfterDelay(activeObject));
                }
                else
                {
                    Debug.LogWarning($"No object assigned for lane {lane}");
                }
            }
            else
            {
                Debug.LogWarning($"Invalid lane index: {laneIndex}");
            }
        }

        private IEnumerator DeactivateAfterDelay(GameObject obj)
        {
            yield return new WaitForSeconds(deactivateDelay);
            if (obj != null)
                obj.SetActive(false);
        }
    }
}