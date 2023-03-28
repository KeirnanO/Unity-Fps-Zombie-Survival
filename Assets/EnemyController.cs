using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    NavMeshAgent agent;
    Animator animator;

    [SerializeField] float health = 5;
    float maxHealth = 5;

    public Transform    RangeDetectionTransform;
    public float        RangeRadius;
    public LayerMask    PlayerLayerMask;

    public float attackTime;
    float timeToAttack;
    bool attacking;

    public AimController target;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        target = FindObjectOfType<AimController>();

        health = maxHealth;
    }

    private void Update()
    {
        if (animator)
        {
            //animator.SetFloat("Speed", agent.velocity.magnitude);
        }


        if ((target.transform.position - transform.position).magnitude > 1.5f)
        {
            agent.SetDestination(target.transform.position);
            attacking = false;
        }
        else
        {
            if (!attacking)
            {
                timeToAttack = Time.time + attackTime;
            }
            attacking = true;

            if (Time.time > timeToAttack)
            {
                target.TakeDamage(1);

                attacking = false;
            }
        }

        /*RaycastHit[] hit;
        hit =Physics.SphereCastAll(RangeDetectionTransform.position, RangeRadius, transform.forward, RangeRadius, PlayerLayerMask);

        if(hit.Length > 0){

            if ((hit[0].transform.position - transform.position).magnitude > 0.5f)
            {
                agent.SetDestination(hit[0].transform.position);
                attacking = false;
            }
            else
            {
                if(!attacking)
                {
                    timeToAttack = Time.time + attackTime;
                }
                attacking = true;

                if(Time.time > timeToAttack)
                {
                    hit[0].transform.GetComponent<AimController>().TakeDamage(1);

                    attacking = false;
                }
            }
           
        }*/
    }

    public void TakeDamage(float damage, bool headShot)
    {
        health -= damage;

        GameManager.instance.GivePlayerPoints(10);

        if(health <= 0)
        {
            if (headShot)
                GameManager.instance.GivePlayerPoints(100);
            else
                GameManager.instance.GivePlayerPoints(60);

            Destroy(gameObject);
        }
    }
}
