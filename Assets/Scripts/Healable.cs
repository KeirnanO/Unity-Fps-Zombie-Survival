using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Healable : NetworkBehaviour
{
    [SerializeField] private Health health = null;

    public void Heal(float value)
    {
        health.Add(value);
    }
}