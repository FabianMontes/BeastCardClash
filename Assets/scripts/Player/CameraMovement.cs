using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    // Variables
    [Header("Player")]
    [SerializeField] Transform player;

    void Update()
    {
        transform.position = player.position;
    }
}
