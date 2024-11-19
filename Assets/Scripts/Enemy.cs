using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int HP = 100;
    private Animator animator;

    private NavMeshAgent navAgent;

    public bool isDead;

    private void Start()
    {
        animator = GetComponent<Animator>();
        navAgent = GetComponent<NavMeshAgent>();
    }

    public void TakeDamage(int damageAmount)
    {
        HP -= damageAmount;

        if (HP <= 0)
        {
            int randomValue = Random.Range(0, 2);

            
            if (randomValue == 0)
            {
                animator.SetTrigger("Die1");
            }
            else
            {
                animator.SetTrigger("Die2");
            }
            GetComponent<CapsuleCollider>().enabled = false;
            isDead = true;


            // Tắt Rigidbody nếu có
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;  // Đảm bảo không còn tác động vật lý
            }

            // Tắt NavMeshAgent nếu có
            if (navAgent != null)
            {
                navAgent.enabled = false;
            }

            //Dead sound
            SoundManager.Instance.zombieChannel2.PlayOneShot(SoundManager.Instance.zombieDeath);
            
        }
        else
        {
            animator.SetTrigger("Damage");

            //hurt sound
            SoundManager.Instance.zombieChannel2.PlayOneShot(SoundManager.Instance.zombieHurt);
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 2.5f); //Attacking // Stop attacking

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, 150f); //Detection start chasing

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 151f); //stop Chasing
    }
}