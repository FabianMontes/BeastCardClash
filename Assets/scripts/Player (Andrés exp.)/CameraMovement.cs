using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    // Variables
    [SerializeField] Transform player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // El objeto alternativo es una esfera, seleccionada desde el inspector de Unity. Sigue al jugador
        // El objeto alternativo no rota, lo cual es necesario para neutralizar la rotación del jugador en la cámara
        transform.position = player.position;
    }
}

// Recomendación para el futuro: renombrar este script y su objeto asociado de una forma más descriptiva
