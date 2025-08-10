using UnityEngine;


public class DialogManager : MonoBehaviour
{
    [SerializeField] private GameObject dialogPanel;
    [SerializeField] private float maxDistance = 4f;
    [SerializeField] private Transform player;

    private bool playerInRange = false; // Indica si el jugador está dentro de la distancia
    private Target playerMovement; // Referencia al script de movimiento del jugador

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dialogPanel?.SetActive(false); // Oculta el panel al inicio. El operador de acceso condicional a nulos (?.) simplifica la comprobación
        if (player != null) playerMovement = player.GetComponent<Target>(); // Obtenemos el componente de movimiento del jugador
    }

    void Update()
    {
        if (player == null || dialogPanel == null) return; // Si las referencias no exisen, retornamos para evitar errores

        // Calcula distancia jugador-NPC y verifica si esta dentro del máximo
        float distance = Vector3.Distance(transform.position, player.position);
        playerInRange = distance <= maxDistance;

        // Si el jugador está en rango y presiona 'Z', alterna el panel y desactiva el movimiento
        if (playerInRange && Input.GetKeyDown(KeyCode.Z)) dialogPanel.SetActive(!dialogPanel.activeSelf);
    }
}
