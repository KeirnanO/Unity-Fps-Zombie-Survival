using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FactorioGamePlayer : NetworkGamePlayer
{
    Transform cameraTarget;
    [SyncVar(hook = nameof(OnHealthChangedHook))]
    [SerializeField] private int health = 10;
    [SyncVar(hook = nameof(OnMaxHealthChangedHook))]
    [SerializeField] private int maxHealth = 10;

    [SerializeField] private Slider healthBar;
    [SerializeField] private Inventory inventory;
    [SerializeField] private Network2DMovementController inputController;

    private float healthRegenDelta = 0f;
    private float healthRegenSpeed = 1f;
    private float healthRegenTimeout = 5f;

    private bool dead = false;

    private void Start()
    {
        OnHealthChangedHook(0, health);
        OnMaxHealthChangedHook(0, maxHealth);
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        StartCoroutine(GetCameraRoot());
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        maxHealth = 10;
        health = maxHealth;
    }

    private void OnHealthChangedHook(int _oldInt, int _newInt)
    {
        health = _newInt;

        healthBar.value = health;

        if (health == maxHealth)
        {
            healthBar.gameObject.SetActive(false);            
            return;
        }

        healthBar.gameObject.SetActive(true);
    }

    private void OnMaxHealthChangedHook(int _oldInt, int _newInt)
    {
        maxHealth = _newInt;

        healthBar.maxValue = maxHealth;

        if (health == maxHealth)
        {
            healthBar.gameObject.SetActive(false);
            return;
        }

        healthBar.gameObject.SetActive(true);
    }

    [ServerCallback]
    private void Update()
    {
        if (health >= maxHealth || dead)
            return;

        if(Time.time > healthRegenDelta)
        {
            health += 1;

            healthRegenDelta = Time.time + (1f / healthRegenSpeed);
        }
    }

    IEnumerator GetCameraRoot()
    {
        while (cameraTarget == null)
        {
            if (CustomNetworkCamera.instance != null)
            {
                cameraTarget = CustomNetworkCamera.instance.CameraTarget;
                cameraTarget.SetParent(transform);
                cameraTarget.localPosition = Vector3.zero;
            }

            yield return null;
        }
    }

    [TargetRpc]
    override public void Init()
    {
        InventoryMenuWindow.instance.SetInventory(inventory);
    }

    [Server]
    public void TakeDamage(int damage)
    {
        if (dead)
            return;

        health -= damage;

        if(health <= 0)
        {
            dead = true;
            RpcDie();
            StartCoroutine(RespawnTime(5f));
            return;
        }

        healthRegenDelta = Time.time + healthRegenTimeout;
    }

    [ClientRpc]
    public void RpcDie()
    {
        if (!isOwned)
            return;

        inputController.clientAnimator.SetBool("Dead", true);
        inputController.enabled = false;
    }

    [ClientRpc]
    public void RpcRespawn()
    {
        if (!isOwned)
            return;

        inputController.enabled = true;
        inputController.clientAnimator.SetBool("Dead", false);
    }

    public Inventory GetInventory()
    {
        return inventory;
    }

    public bool IsDead()
    {
        if (health <= 0)
            return true;

        return false;
    }

    IEnumerator RespawnTime(float time)
    {
        yield return new WaitForSeconds(time);

        health = maxHealth;
        inputController.RpcSetPositionAndRotation(Vector3.zero, Quaternion.identity);
        dead = false;
        RpcRespawn();
    }

    [ClientCallback]
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Item"))
        {
            var droppedItem = collision.GetComponent<ItemDrop>();

            if (inventory.ContainsStackableItem(droppedItem.item))
            {
                inventory.AddItem(droppedItem.item, droppedItem.amount);
                droppedItem.CommandPickUpItemCallBack();
                return;
            }

            if (inventory.IsFull())
                return;

            inventory.AddItem(droppedItem.item, droppedItem.amount);

            droppedItem.CommandPickUpItemCallBack();
        }
    }
}
