using UnityEngine;

public class Target : MonoBehaviour
{
    // Variables
    [Header("Player components and controls")]
    [SerializeField] Transform playerCamera; // No puede llamarse Camera por solapamiento
    [SerializeField] float speed = 10f; // Velocidad de movimiento
    [SerializeField] bool useArrows = true; // true = flechas o WASD, false = mouse

    void Update()
    {
        // Cambiar el modo de movimiento segun el modo elegido
        // true = flechas o WASD, false = mouse
        if (useArrows)
        {
            // Mover el cubo en la dirección de entrada de las flechas o las teclas WASD
            transform.position += GetInputDirection() * speed * Time.deltaTime;
        }
        else
        {
            // Detecta clic izquierdo
            if (Input.GetMouseButtonDown(0))
            {
                // Crear un rayo desde la cámara hacia la posición del mouse
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                // Realizar un raycast para detectar colisiones
                // Mueve el cubo a la posición del punto de impacto si hay una colisión
                if (Physics.Raycast(ray, out RaycastHit hit)) transform.position = hit.point;
            }
        }

    }

    // Obtiene la dirección de entrada del jugador con las teclas, cuando mode = true
    Vector3 GetInputDirection()
    {
        // Por defecto, la dirección es cero
        Vector3 dir = Vector3.zero;

        // Comprobar las teclas de flecha y letras y ajustar la dirección
        // Se usa if en lugar de switch para permitir múltiples teclas presionadas
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) dir += playerCamera.forward; // W: Arriba
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) dir -= playerCamera.right; // A: Izquierda
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) dir -= playerCamera.forward; // S: Abajo
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) dir += playerCamera.right; // D: Derecha

        // Normalizar la dirección y fijarla en horizontal para evitar que la velocidad sea mayor
        // cuando se presionan múltiples teclas y evitar movimiento en vertical
        dir.y = 0;
        return dir.normalized;
    }
}
