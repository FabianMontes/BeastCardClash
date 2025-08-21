using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    // Variables
    [Header("Player")]
    [SerializeField] Transform player; // Jugador

    void Update()
    {
        transform.position = player.position;
    }
}
