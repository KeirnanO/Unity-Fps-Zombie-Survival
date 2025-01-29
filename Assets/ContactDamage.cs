using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContactDamage : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag.Equals("Enemy"))
        {
            collision.gameObject.GetComponent<Network2DEnemy>().ApplyKnockback(transform.parent, 4f);
            collision.gameObject.GetComponent<Network2DEnemy>().DealDamage(1);
        }
    }
}
