using UnityEngine;

public class Cube : MonoBehaviour
{
    // Variables
    [SerializeField] Transform playerCamera;
    [SerializeField] float speed = 5f;
    [SerializeField] bool mode = true; // true = flechas o WASD, false = mouse

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (mode)
        {
            transform.position += GetInputDirection() * speed * Time.deltaTime;
        }
        else
        {
            if (Input.GetMouseButtonDown(0)) // Clic izquierdo
            {
                // Crear un rayo desde la cámara hacia la posición del mouse
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                // Realizar un raycast para detectar colisiones
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    // Debug (comentado)
                    // Debug.Log("Golpeó: " + hit.collider.name);
                    // Debug.DrawLine(ray.origin, hit.point, Color.red, 1f);

                    // Mover el cubo a la posición del punto de impacto
                    transform.position = hit.point;
                }
            }
        }

    }

    // Método para obtener la dirección de entrada del jugador con las teclas
    // Cuando mode = true
    Vector3 GetInputDirection()
    {
        // dir inicia como un vector nulo
        Vector3 dir = Vector3.zero;

        // Comprobar las teclas de flecha y letras y ajustar la dirección
        // Se usan ifs para permitir múltiples teclas presionadas
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) dir += playerCamera.forward;
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) dir -= playerCamera.right;
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) dir -= playerCamera.forward;
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) dir += playerCamera.right;

        // Normalizar la dirección y fijarla en horizontal para evitar que la velocidad sea mayor
        // cuando se presionan múltiples teclas y evitar movimiento en vertical
        dir.y = 0;
        return dir.normalized;
    }
}
