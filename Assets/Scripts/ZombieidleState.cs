using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieIdleState : StateMachineBehaviour
{
    float timer;
    public float idleTime = 3f;

    Transform player;
    public float detectionAreaRadius = 18f;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer = 0;
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // will transition to patrol state
        timer += Time.deltaTime;

        if (timer > idleTime)
        {
            animator.SetBool("isPatrolling", true);
        }

        // transitioning to chasing state
        float distanceFromPlayer = Vector3.Distance(player.position, animator.transform.position);

        if (distanceFromPlayer < detectionAreaRadius)
        {
            animator.SetBool("isChasing", true);
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
