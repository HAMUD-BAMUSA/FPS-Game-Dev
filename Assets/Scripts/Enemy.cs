using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int HP = 100;
    public bool isDead = false;//was prv

    private Animator animator;
    private UnityEngine.AI.NavMeshAgent navAgent;

    void Start()
    {
        animator = GetComponent<Animator>();
        navAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        HP -= amount;

        if (HP <= 0)
        {
            isDead = true;

            int randomValue = Random.Range(0, 2);

            if (randomValue == 0)
                animator.SetTrigger("DIE1");
            else
                animator.SetTrigger("DIE2");

            navAgent.enabled = false;

            // Play animation before destroying
            Destroy(gameObject, 2f);
        }
        else
        {
            animator.SetTrigger("DAMAGE");
        }
    }

    
    
}