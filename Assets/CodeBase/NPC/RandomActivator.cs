using UnityEngine;
using System.Collections;

namespace Codebase.Components.Player
{
    public class RandomActivator : MonoBehaviour
    {
        [SerializeField] private GameObject[] objects;
        [SerializeField] private float deactivateDelay = 5f;
        [SerializeField] private PlayerCollisionHandler playerCollisionHandler;

        private GameObject activeObject;

        private void Start()
        {
           
            if (playerCollisionHandler != null)
                playerCollisionHandler.OnActivateRandomObject += ActivateRandomObject;
        }

        private void OnDestroy()
        {
            if (playerCollisionHandler != null)
                playerCollisionHandler.OnActivateRandomObject -= ActivateRandomObject;
        }

        private void ActivateRandomObject()
        {
            if (objects == null || objects.Length == 0)
                return;

            if (activeObject != null)
                activeObject.SetActive(false);

            int randomIndex = Random.Range(0, objects.Length);
            activeObject = objects[randomIndex];

            if (activeObject != null)
            {
                activeObject.SetActive(true);
                StartCoroutine(DeactivateAfterDelay(activeObject));
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
