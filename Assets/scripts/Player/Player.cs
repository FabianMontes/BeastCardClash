using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour
{
    // Variables
    [Header("Player target")]
    [SerializeField] Transform target; // Objetivo, el cual controla el jugador
    NavMeshAgent agent; // Agente NavMesh

    void Start()
    {
        TryGetComponent<NavMeshAgent>(out agent);
    }

    void Update()
    {
        agent.SetDestination(target.position);
    }
}
