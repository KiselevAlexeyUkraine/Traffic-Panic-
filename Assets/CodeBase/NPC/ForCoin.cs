using UnityEngine;

public class ForCoin : MonoBehaviour
{
    [SerializeField] private GameObject Part;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            Part.SetActive(true);
            Destroy(gameObject, 2f);
        }
    }

}
