using TMPro;
using UnityEngine;

public class DialogManager : MonoBehaviour
{
    [SerializeField] private GameObject dialogPanel; // Panel de diálogo en la UI
    [SerializeField] private TextMeshProUGUI namePanel; // Panel de nombre del personaje en la UI
    [SerializeField] private string characterName; // Nombre del personaje
    [SerializeField] private float maxDistance = 3f; // Distancia máxima para mostrar el diálogo
    [SerializeField] private Transform target; // Target (jugador)

    private Target targetMovement; // Script de movimiento del target (necesario para desactivarlo)

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        targetMovement = target.GetComponent<Target>();
        dialogPanel?.SetActive(false); // Oculta el panel al inicio. El operador de acceso condicional a nulos (?.) simplifica la comprobación
    }

    // Update is called once per frame
    void Update()
    {
        // Calcula la distancia entre el personaje y el target y verifica si esta dentro del máximo
        float distance = Vector3.Distance(transform.position, target.position);
        bool targetInRange = distance <= maxDistance;

        // Si el jugador está en rango y presiona 'Z', alterna el panel y desactiva el movimiento
        if (targetInRange && Input.GetKeyDown(KeyCode.Z))
        {
            dialogPanel.SetActive(!dialogPanel.activeSelf); // Alterna la visibilidad del panel
            targetMovement.enabled = !dialogPanel.activeSelf; // Alterna el movimiento de forma opuesta al panel
            if (dialogPanel.activeSelf) namePanel.text = characterName; // Muestra el nombre en el panel, si está activo
        }
    }
}
