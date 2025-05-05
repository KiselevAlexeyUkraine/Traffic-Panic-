using UnityEngine;

namespace Codebase.Components.Player
{
    public class Pursuer_Car : MonoBehaviour
    {
        [SerializeField] private float speed = 10f; // �������� �������� �� Z (������ � �������)
        [SerializeField] private float startZ = -50f; // ��������� ������� Z
        [SerializeField] private float endZ = 50f;  // �������� ������� Z (��� �����������)

        private bool isMoving = false;
        private Vector3 initialPosition;

        private void Awake()
        {
         
            initialPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, startZ);
        }

        private void OnEnable()
        {
            
            isMoving = true;
            transform.localPosition = initialPosition; 
        }

        private void OnDisable()
        {
          
            isMoving = false;
        }

        private void Update()
        {
            if (isMoving)
            {
             
                Vector3 position = transform.localPosition;
                position.z += speed * Time.deltaTime;
                transform.localPosition = position;

           
                if (position.z >= endZ)
                {
                    gameObject.SetActive(false);
                }
            }
        }
    }
}