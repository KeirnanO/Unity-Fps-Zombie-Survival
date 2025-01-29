using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Network2DEnemy : NetworkBehaviour
{
    [SerializeField] private float MoveSpeed = 5f;
    [SerializeField] private float attackSpeed = 1f;
    [SerializeField] private float attackTimeoutDelta = 0f;

    [SerializeField] private int health = 5;

    [SerializeField] private float VisionRadius;
    [SerializeField] private LayerMask PlayerLayer;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator clientAnimator;
    [SerializeField] private bool interacting;

    [SerializeField] private FactoryItem[] itemsOnDeath;

    private float SearchTime = 1.5f;
    private float SearchTimeDelta = 0f;

    public override void OnStartServer()
    {
        StartCoroutine(CoreLoop());
    }

    IEnumerator CoreLoop()
    {
        Transform target = null;

        Network2DBase homeBase = FindObjectOfType<Network2DBase>();

        //Loop active while alive
        while (health > 0)
        {
            if (Time.time > SearchTimeDelta)
            {
                RaycastHit2D[] results = Physics2D.CircleCastAll(transform.position, VisionRadius, Vector2.zero, 100f, PlayerLayer);

                if (results.Length > 0)
                {
                    foreach(var result in results)
                    {
                        if (!result.transform.GetComponent<FactorioGamePlayer>().IsDead())
                        {
                            target = result.transform;
                            Debug.DrawLine(transform.position, target.position);
                            break;
                        }

                        target = null;
                    }                  
                }
                else
                {
                    target = null;
                }

                clientAnimator.SetFloat("Speed", 0f);
                SearchTimeDelta = Time.time + SearchTime;
            }

            if (interacting)
            {
                yield return null;
                continue;
            }

            if (target != null)
            {
                //If in AttackRange
                if ((transform.position - target.position).magnitude < .5f)
                {
                    rb.velocity = Vector2.zero;

                    if (Time.time > attackTimeoutDelta)
                    {
                        Attack(target);
                    }
                }
                else
                {
                    MoveToTarget(target);
                }
            }
            else
            {
                //If in AttackRange
                if ((transform.position - homeBase.transform.position).magnitude < 2f)
                {
                    rb.velocity = Vector2.zero;

                    if (Time.time > attackTimeoutDelta)
                    {
                        Attack(homeBase);
                    }
                }
                else
                {
                    MoveToTarget(homeBase.transform);
                }
            }

            clientAnimator.SetFloat("Speed", (rb.velocity.normalized).magnitude);
            yield return null;
        }
    }

    //Temporary while a base entity class does not exist
    void Attack(Transform target)
    {        
        target.GetComponent<Network2DMovementController>().ApplyKnockback(transform, 4f);
        target.GetComponent<FactorioGamePlayer>().TakeDamage(1);

        attackTimeoutDelta = Time.time + (1f / attackSpeed);
    }
    void Attack(Network2DBase target)
    {
        target.TakeDamage(1);
        clientAnimator.SetTrigger("Attack");

        attackTimeoutDelta = Time.time + (1f / attackSpeed);
    }

    void MoveToTarget(Transform target)
    {
        Vector2 direction = target.position - transform.position;

        direction.Normalize();

        rb.velocity = MoveSpeed * direction;
    }

    [Server]
    public void ApplyKnockback(Transform position, float force)
    {
        if (isServer)
        {
            ServerApplyKnockback(position, force);
            return;
        }
    }

    [Server]
    public void ServerApplyKnockback(Transform position, float force)
    {
        if (interacting)
            return;

        Vector2 dir = (transform.position - position.position).normalized;

        StartCoroutine(Knockback(dir, force));
    }

    [Server]
    public void DealDamage(int damage)
    {
        health -= damage;

        if(health <= 0)
        {
            clientAnimator.SetBool("Dead", true);
        }
    }

    IEnumerator Knockback(Vector2 direction, float force)
    {
        interacting = true;

        rb.velocity = direction * force * force;

        while (rb.velocity.magnitude > 0.5f)
        {
            rb.velocity *= .90f;

            yield return null;
        }

        rb.velocity = Vector2.zero;
        interacting = false;
    }

    //This gets called at the end of the death animation
    [Server]
    void Die()
    {
        foreach(var item in itemsOnDeath)
        {
            var itemDrop = Instantiate(item.droppedItemObject, transform.position, Quaternion.identity);

            NetworkServer.Spawn(itemDrop.gameObject);

            itemDrop.ServerSetItem(item, 1);
            itemDrop.transform.Rotate(new Vector3(0, 0, Random.Range(0, 360)));
        }

        DestroyObject();
    }

    [Server]
    void DestroyObject()
    {
        NetworkServer.Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, VisionRadius);
    }
}
