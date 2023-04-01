using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
using Cinemachine;
using Mirror;
using UnityEngine.Events;

public class NetworkPlayerController : NetworkBehaviour
{
    StarterAssetsInputs input;

    [SyncVar(hook = nameof(OnNameChange))]
    public string playerName;

    public LayerMask InteractableLayerMask;

    CinemachineVirtualCamera StandardCamera;
    CinemachineVirtualCamera AimCamera;

    public Loadout loadout;

    [SyncVar(hook = nameof(SyncLoadout))]
    private int currentGunIndex = 0;

    public Gun startingGun;
    public Transform arms;

    public Transform debugTransform;

    public float maxHealth;

    [SyncVar(hook = nameof(OnHealthChange))]
    private float health;

    public float regenTime;
    float timeToRegen;

    public float regenTickRate;
    float regenTickRateMod;
    float regenTick;

    public float regenStrength;

    bool regenerating;
    bool spawned = false;

    [SerializeField] int startingPoints;
    [SerializeField] int points;

    float inputDelay;

    public override void OnStartServer()
    {
        EnsureInit();
        SyncLoadout(currentGunIndex, currentGunIndex);
        OnHealthChange(health, health);
        OnNameChange("", playerName);

        base.OnStartServer();
    }

    public override void OnStartAuthority()
    {
        FindObjectOfType<Canvas>().enabled = true;
        FindObjectOfType<ClientUIHandler>().SetPlayer(this);
        ChatBox.instance.SetPlayer(this);

        SetGameLayerRecursive(arms.gameObject, LayerMask.NameToLayer("Gun"));

        base.OnStartAuthority();
    }

    public override void OnStartLocalPlayer()
    {
        StandardCamera = GameObject.FindGameObjectWithTag("PlayerFollowCamera").GetComponent<CinemachineVirtualCamera>();
        AimCamera = GameObject.FindGameObjectWithTag("PlayerAimCamera").GetComponent<CinemachineVirtualCamera>();

        input = GetComponent<StarterAssetsInputs>();

        CmdSetPlayerName(PlayerPrefs.GetString("PlayerName", "Guest" + Random.Range(100, 999)));

        health = maxHealth;
        points = startingPoints;

        inputDelay = Time.time + 1f;
    }

    public override void OnStartClient()
    {
        EnsureInit();
        SyncLoadout(currentGunIndex, currentGunIndex);
        OnHealthChange(health, health);
        OnNameChange("", playerName);

        base.OnStartClient();
    }



    private void Start()
    {
        loadout = GetComponent<Loadout>();

        if (isOwned)
        {
            SendChatMessage("Asking Server For Gun");
            CmdGivePlayerGun(1);
        }
    }

    private void SyncLoadout(int _old, int _new)
    {
        EnsureInit();

        if (isOwned)
            SendChatMessage("Recieved Gun[" + _new + "]!");

        currentGunIndex = _new;
        loadout.EquipGun(loadout.WeaponArray[_new]);

        SendChatMessage("Syncing Guns Complete!");
    }

    private void OnHealthChange(float _old, float _new)
    {
        EnsureInit();

        health = _new;

        //SetHealth(health);
    }

    private void OnNameChange(string _old, string _new)
    {
        EnsureInit();

        playerName = _new;
    }

    private void Update()
    {
        if (isServer && Time.time > timeToRegen)
            Regen();


        if (!isLocalPlayer || Time.time < inputDelay)
            return;

        if (loadout.GetCurrentGun())
            HandleInputs();

        Vector2 centerScreenPoint = new(Screen.width / 2, Screen.height / 2);
        if (Physics.Raycast(Camera.main.ScreenPointToRay(centerScreenPoint), out RaycastHit hit, 999f))
        {
            debugTransform.position = hit.point;
        }

        if (Physics.Raycast(Camera.main.ScreenPointToRay(centerScreenPoint), out hit, 1f, InteractableLayerMask))
        {
            Interactable interactable = hit.transform.GetComponent<Interactable>();
            GameManager.instance.DisplayToolTip(interactable.GetTooltip());

            if (input.interact)
            {
                interactable.Interact();
                input.interact = false;
            }
        }
        else
        {
            //GameManager.instance.DisplayToolTip("");
        }
    }

    void HandleInputs()
    {
        if (input.reload)
        {
            input.sprint = false;
            input.aim = false;
            input.fire = false;

            loadout.GetCurrentGun().Reload();
            //loadout.SwapGuns();
        }
        else
        {
            loadout.GetCurrentGun().animator.SetBool("IsReloading", false);
        }

        if (input.sprint)
        {
            input.aim = false;
            input.fire = false;

            loadout.GetCurrentGun().animator.SetBool("IsSprinting", true);
        }
        else
        {
            loadout.GetCurrentGun().animator.SetBool("IsSprinting", false);
        }

        if (input.aim)
        {
            StandardCamera.enabled = false;
            AimCamera.enabled = true;
            loadout.GetCurrentGun().animator.SetBool("IsAiming", true);
        }
        else
        {
            StandardCamera.enabled = true;
            AimCamera.enabled = false;
            loadout.GetCurrentGun().animator.SetBool("IsAiming", false);
            loadout.GetCurrentGun().Aim(false);
        }

        if (input.fire)
        {
            Vector2 centerScreenPointer = new(Screen.width / 2, Screen.height / 2);
            Ray ray = Camera.main.ScreenPointToRay(centerScreenPointer);

            if(isServer)
            {
                loadout.GetCurrentGun().Fire(ray.origin, ray.direction);
                CmdFire(ray.origin, ray.direction);
            }
            else
            {
                loadout.GetCurrentGun().Fire();
                CmdFire(ray.origin, ray.direction);
            }
           
            //loadout.SwapGuns();
        }
        else
        {
            CmdUnFire();
            loadout.GetCurrentGun().Disengage();
        }
    }

    [Server]
    public void TakeDamage(float damage)
    {
        //OnHealthChange(health, health - damage);
        health -= damage;
        SetHealth(health);
        //OnHealthChange(health, health);

        timeToRegen = Time.time + regenTime;
        regenerating = false;
    }

    [Server]
    void Heal(float amount)
    {
        if (health + amount > maxHealth)
        {
            health = maxHealth;
            SetHealth(maxHealth);
            //OnHealthChange(health, health);
        }
        else
        {
            health += amount;
            SetHealth(health);
            //OnHealthChange(health, health);
        }
    }

    [Server]
    void Regen()
    {
        if (health >= maxHealth || Time.time < timeToRegen) return;

        if (!regenerating)
        {
            regenTickRateMod = regenTickRate;
            regenerating = true;
        }

        if (Time.time > regenTick)
        {
            Heal(regenStrength);

            regenTick = Time.time + regenTickRateMod;
            regenTickRateMod *= 0.95f;
        }
    }

    public void GivePoints(int amount)
    {
        points += amount;
    }
    public int GetPoints()
    {
        return points;
    }

    public bool TakePoints(int amount)
    {
        if (points >= amount)
        {
            points -= amount;
            return true;
        }

        return false;
    }

    [Command]
    void CmdReload()
    {
        RpcReload();
    }

    [Command]
    void CmdUnFire()
    {
        loadout.GetCurrentGun().Disengage();
        RpcUnFire();
    }

    [Command]
    void CmdSprint()
    {
        RpcSprint();
    }

    [Command]
    void CmdFire(Vector3 position, Vector3 direction)
    {
        //This is the only Fire method that will actually do anything to the environment
        loadout.GetCurrentGun().Fire(position, direction);

        RpcFire(position, direction);
    }

    [Command]
    void CmdAim()
    {
        RpcAim();
    }

    [Command]
    void CmdGivePlayerGun(int gunIndex)
    {
        SendServerMessage("Server Giving " + playerName + " Gun[" + gunIndex + "]");
        //currentGunIndex = gunIndex;
        RpcGivePlayerGun(gunIndex);
    }

    [ClientRpc]
    void RpcUnFire()
    {
        if (isLocalPlayer || isServer)
            return; 

        loadout.GetCurrentGun().Disengage();
    }

    [ClientRpc]
    void SetHealth(float _health)
    {
        if (isServer)
            return;

        health = _health;
    }

    [ClientRpc]
    void RpcGivePlayerGun(int gunIndex)
    {
        SendChatMessage(playerName + " Recieved Gun[" + gunIndex + "]!");

        currentGunIndex = gunIndex;
        loadout.EquipGun(loadout.WeaponArray[gunIndex]);

        SendChatMessage("Syncing Guns Complete!");
    }

    [Command]
    void CmdSetPlayerName(string name)
    {
        playerName = name;
    }

    [ClientRpc]
    void RpcGivePlayerGun()
    {
        if (isServer)
            return;

        loadout.PickUpGun(startingGun);
    }

    [ClientRpc]
    void RpcReload()
    {
        if (isLocalPlayer)
            return;

        if (loadout.GetCurrentGun())
            loadout.GetCurrentGun().Reload();
    }
    [ClientRpc]
    void RpcSprint()
    {
    }

    [ClientRpc]
    void RpcFire(Vector3 position, Vector3 direction)
    {
        if (isLocalPlayer || isServer)
            return;

        //Fire Blanks
        loadout.GetCurrentGun().Fire();
    }

    [ClientRpc]
    void RpcAim()
    {
    }

    [Command(requiresAuthority = false)]
    public void SendChatMessage(string chatMessage)
    {
        RPCRecieveChatMessage(chatMessage);
    }

    [ClientRpc]
    public void RPCRecieveChatMessage(string chatMessage)
    {
        ChatBox.instance.AddMessage(playerName, chatMessage);
    }

    [Command(requiresAuthority = false)]
    public void SendServerMessage(string chatMessage)
    {
        RPCRecieveServerMessage(chatMessage);
    }

    [ClientRpc]
    public void RPCRecieveServerMessage(string chatMessage)
    {
        ChatBox.instance.AddMessage("", chatMessage);
    }

    private void SetGameLayerRecursive(GameObject _go, int _layer)
    {
        _go.layer = _layer;
        foreach (Transform child in _go.transform)
        {
            child.gameObject.layer = _layer;

            Transform _HasChildren = child.GetComponentInChildren<Transform>();
            if (_HasChildren != null)
                SetGameLayerRecursive(child.gameObject, _layer);

        }
    }

    public Loadout GetLoadout()
    {
        return loadout;
    }

    private void EnsureInit()
    {
        if (!loadout)
        {
            loadout = GetComponent<Loadout>();
        }
    }

    public float GetHealth()
    {
        return health;
    }
}