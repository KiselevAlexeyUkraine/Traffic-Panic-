using UnityEngine;
using FMODUnity;

public class FMODMovementSounds
{
    [SerializeField]  private string stepEvent; // Укажи путь к ивенту FMOD

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D)) // Если нажато ← или →
        {
            RuntimeManager.PlayOneShot(stepEvent); // Проигрываем случайный звук
        }
    }
}
