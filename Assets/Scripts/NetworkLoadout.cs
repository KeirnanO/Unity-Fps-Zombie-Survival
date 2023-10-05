using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Cinemachine;

public class NetworkLoadout : NetworkBehaviour
{
    [Header("Loadout")]
    [SyncVar(hook = nameof(HandleWeaponChange))]
    [SerializeField] private int currentGun;
    [SerializeField] private Gun[] gunArray;
    [SerializeField] private GameObject[] clientGunArray;

    [Header("Player")]
    [SerializeField] private PlayerIKRig localPlayerRig = null;
    [SerializeField] private PlayerIKRig clientPlayerRig = null;

    [Header("Camera")]
    [SerializeField] private CinemachineVirtualCamera normalCamera;
    [SerializeField] private CinemachineVirtualCamera aimCamera;

    private Controls controls;

    private Controls Controls
    {
        get
        {
            if (controls != null) { return controls; }
            return controls = new Controls();
        }
    }

    bool firing;

    public override void OnStartAuthority()
    {
        enabled = true;

        Controls.Player.Fire.started += ctx => StartFire();
        Controls.Player.Fire.canceled += ctx => StopFire();

        Controls.Player.Aim.started += ctx => StartAim();
        Controls.Player.Aim.canceled += ctx => EndAim();

        controls.Player.Reload.started += ctx => Reload(true);
        controls.Player.Reload.canceled += ctx => Reload(false);

        controls.Player.Sprint.started += ctx => Sprint(true);
        controls.Player.Sprint.canceled += ctx => Sprint(false);
    }

    public override void OnStartServer()
    {
        HandleWeaponChange(0 ,currentGun);
    }


    void Reload(bool state)
    {
        if (gunArray[currentGun] == null)
            return;

        if (state)
            gunArray[currentGun].Reload();
        else
            gunArray[currentGun].animator.SetBool("IsReloading", state);
            gunArray[currentGun].clientAnimator.SetBool("IsReloading", state);
    }

    void Sprint(bool state)
    {
        if (gunArray[currentGun] == null)
            return;

        gunArray[currentGun].animator.SetBool("IsSprinting", state);
        gunArray[currentGun].clientAnimator.SetBool("IsSprinting", state);
    }

    public void HandleWeaponChange(int _oldvalue, int _newvalue) => EquipGun(_oldvalue, _newvalue);

    [ClientCallback]
    private void OnEnable() => Controls.Enable();
    [ClientCallback]
    private void OnDisable() => Controls.Disable();
    [ClientCallback]
    private void Update()
    {
        if (firing && currentGun != 0)
            CmdFire(Camera.main.transform.position, Camera.main.transform.forward);
        else if (currentGun != 0 && !firing)
        {
            gunArray[currentGun].Disengage();
            CmdDisengage();
        }
    }

    [Client]
    private void StartAim()
    {
        normalCamera.enabled = false;
        aimCamera.enabled = true;

        gunArray[currentGun].animator.SetBool("IsAiming", true);
        gunArray[currentGun].clientAnimator.SetBool("IsAiming", true);
    }
    [Client]
    private void EndAim()
    {
        aimCamera.enabled = false;
        normalCamera.enabled = true;

        gunArray[currentGun].animator.SetBool("IsAiming", false);
        gunArray[currentGun].clientAnimator.SetBool("IsAiming", false);
    }

    [Command]
    private void CmdFire(Vector3 position, Vector3 direction)
    {
        gunArray[currentGun].Fire(position, direction);
        RpcFire();
    }

    [Server]
    public void GiveGun(int gunIndex)
    {
        currentGun = gunIndex;
    }

    [Command]
    public void CmdGiveGun(int gunIndex)
    {
        currentGun = gunIndex;
    }

    [Command]
    private void CmdDisengage()
    {
        gunArray[currentGun].Disengage();
    }

    [ClientRpc]
    private void RpcFire()
    {
        gunArray[currentGun].Fire();
    }

    [Client]
    private void StartFire()
    {
        firing = true;
    }

    [Client]
    private void StopFire()
    {
        firing = false;
    }

    [Client]
    public void EquipGun(int oldIndex, int gunIndex)
    {
        StopAllCoroutines();
        StartCoroutine(EquipCore(oldIndex, gunIndex));
    }

    IEnumerator EquipCore(int oldIndex, int gunIndex)
    {
        //If we are told to e
        if (oldIndex == gunIndex && gunIndex != 0)
        {
            gunArray[gunIndex].RefillAmmo();
            StopAllCoroutines();
        }

        if (oldIndex != 0)
        {
            print("Dequipping");
            yield return StartCoroutine(DequipGun(oldIndex));
            print("Dequipped");
        }

        print("Equipping");
        yield return StartCoroutine(StartEquip(gunIndex));
        print("Equipped");
    }

    IEnumerator StartEquip(int gunIndex)
    {
        if (gunArray[gunIndex] == null)
        {
            localPlayerRig.SetHidden(true);
        }
        else
        {
            gunArray[gunIndex].gameObject.SetActive(true);
            localPlayerRig.SetIKRig(gunArray[gunIndex].GetIKRig());
            gunArray[gunIndex].enabled = true;


            clientGunArray[gunIndex].SetActive(true);
            clientPlayerRig.SetIKRig(clientGunArray[gunIndex].GetComponent<IKRig>());
            yield return null;

            localPlayerRig.SetHidden(false);
        }
    }

    IEnumerator DequipGun(int gunIndex)
    {
        if (gunIndex != 0)
        {
            gunArray[gunIndex].Dequip();

            clientGunArray[gunIndex].GetComponent<Animator>().SetTrigger("Dequip");

            yield return new WaitUntil(() => gunArray[gunIndex].gameObject.activeSelf == false);
        }
    }

}
    
