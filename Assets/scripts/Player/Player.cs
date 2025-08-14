using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour
{
    // Variables
    NavMeshAgent agent;
    [SerializeField] Transform target;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TryGetComponent<NavMeshAgent>(out agent);
    }

    // Update is called once per frame
    void Update()
    {
        agent.SetDestination(target.position);
    }
}
