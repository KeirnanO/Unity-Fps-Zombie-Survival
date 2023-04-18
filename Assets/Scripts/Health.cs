using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class Health : NetworkBehaviour
{
    [SerializeField] private float maxHealth = 100f;

    [SyncVar(hook = nameof(HandleHealthChange))]
    [SerializeField] private float health;

    public static event Action<NetworkConnectionToClient> OnDeath;
    public event Action<float> OnHealthChanged;

    public bool IsDead => health == 0f;


    public override void OnStartServer()
    {
        health = maxHealth;
    }

    [ServerCallback]

    private void OnDestroy()
    {
        OnDeath?.Invoke(connectionToClient);
    }

    [Server]
    public void Add(float value)
    {
        value = Mathf.Max(value, 0);

        health = Mathf.Min(health + value, maxHealth);        
    }

    [Server]
    public void Remove(float value)
    {
        value = Mathf.Max(value, 0);

        health = Mathf.Max(health - value, 0);

        if (health == 0)
        {
            OnDeath?.Invoke(connectionToClient);

            RpcHandleDeath();
        }
    }

    private void HandleHealthChange(float _oldvalue, float _newvalue)
    {
        OnHealthChanged?.Invoke(health);
    }

    [ClientRpc]
    private void RpcHandleDeath()
    {
        gameObject.SetActive(false);
    }

}
