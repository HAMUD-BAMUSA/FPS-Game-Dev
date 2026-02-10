using UnityEngine;
using UnityEngine.AI;

public class EnemyMove : MonoBehaviour
{
    public Transform target;     // Player following by zombie
    NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (target != null)
            agent.SetDestination(target.position);
    }
}
