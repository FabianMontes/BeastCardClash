using UnityEngine;
using UnityEngine.AI;

public class WorldPlayer : MonoBehaviour
{
    // Variables
    [SerializeField] Transform target;
    NavMeshAgent agent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TryGetComponent<NavMeshAgent>(out agent); // Busca el NavMeshAgent del jugador
    }

    // Update is called once per frame
    void Update()
    {
        // Ubica al agente en la posición del target
        // El target es el cubo, seleccionado desde el inspector de Unity
        agent.SetDestination(target.position); 
    }
}

// Recomendación para el futuro: renombrar este script como Player y la carpeta Player (Andrés exp.) también