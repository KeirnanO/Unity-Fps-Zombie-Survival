using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Network2DBase : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnHealthChangedHook))]
    [SerializeField] private int health;
    [SyncVar]
    [SerializeField] private int maxHealth;

    [SerializeField] private Slider healthBar;

    private void Start()
    {
        OnHealthChangedHook(0, health);
    }

    public override void OnStartServer()
    {
        maxHealth = 20;
        health = maxHealth;
    }

    private void OnHealthChangedHook(int _oldInt, int _newInt)
    {
        health = _newInt;
        healthBar.value = health;
        healthBar.maxValue = maxHealth;
    }

    [Server]
    public void TakeDamage(int damage)
    {
        health -= damage;
    }
}
