using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;   // recommended: shorter NavMeshAgent usage

public class ZombiePatrollingState : StateMachineBehaviour
{
    float timer;
    public float patrollingTime = 10f;

    Transform player;  
    NavMeshAgent agent;

    public float detectionArea = 18f;
    public float patrolSpeed = 2f;

    List<Transform> waypointsList = new List<Transform>();

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // initialize player
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = animator.GetComponent<NavMeshAgent>();

        // set patrol speed
        agent.speed = patrolSpeed;
        timer = 0;

        // getting waypoints & moving to 1st waypoint
        GameObject waypointCluster = GameObject.FindGameObjectWithTag("Waypoints");

        foreach (Transform t in waypointCluster.transform)
        {
            waypointsList.Add(t);
        }

        Vector3 nextPosition = waypointsList[Random.Range(0, waypointsList.Count)].position;
        agent.SetDestination(nextPosition);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // check if agent arrived at waypoint then move to next one
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            agent.SetDestination(waypointsList[Random.Range(0, waypointsList.Count)].position);
        }

        // transition to idle state
        timer += Time.deltaTime;
        if (timer > patrollingTime)
        {
            animator.SetBool("isPatrolling", false);
        }

        // transitioning to chasing state
        float distanceFromPlayer = Vector3.Distance(player.position, animator.transform.position);

        if (distanceFromPlayer < detectionArea)
        {
            animator.SetBool("isChasing", true);
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // stop the agent
        agent.SetDestination(animator.transform.position);
    }
}
