using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    // Variables
    [SerializeField] Transform player;

    // Update is called once per frame
    void Update()
    {
        transform.position = player.position;
    }
}
