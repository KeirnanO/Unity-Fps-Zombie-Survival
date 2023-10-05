using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Limb : ShootableObject
{
    [SerializeField] float damageMultiplyer;
    public bool isHead;


    public override void GetShot(float damage, float force)
    {
        base.GetShot(damage, force);

       GetComponentInParent<Damageable>().DealDamage(damage * damageMultiplyer);

        print(transform + "Got shot for " + damage * damageMultiplyer + " damage!");
    }
}
