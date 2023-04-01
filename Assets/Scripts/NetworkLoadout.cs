using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkLoadout : NetworkBehaviour
{
    public NetworkGun[] guns = new NetworkGun[2];

    int gunNum;

    [SerializeField]
    PlayerIKRig playerRig;

    private void Start()
    {
        StartCoroutine(EquipGun(0));
    }

    //-----Local-----//
    public void PickUpGun(NetworkGun gun)
    {
        CmdPickUpGun();
    }

    public void SwapGuns()
    {
        CmdSwapGuns();
    }

    IEnumerator EquipGun(int gunNum)
    {
        playerRig.SetIKRig(guns[gunNum].GetIKRig());

        guns[gunNum].gameObject.SetActive(true);
        guns[gunNum].GetComponent<NetworkGun>().enabled = true;

        yield return null;

        playerRig.SetHidden(false);
    }

    IEnumerator DequipGun()
    {
        guns[gunNum].Dequip();
        //guns[gunNum].GetComponent<Gun>().enabled = false;

        yield return new WaitUntil(() => guns[gunNum].gameObject.activeSelf == false);
    }

    IEnumerator GunPickUp(NetworkGun gun)
    {
        if (guns[0] == null)
        {
            guns[0] = gun;
            yield return StartCoroutine(EquipGun(0));
        }
        else if (guns[1] == null)
        {
            guns[1] = gun;
            SwapGuns();
        }
        else
        {
            guns[gunNum] = gun;

            yield return StartCoroutine(DequipGun());
            yield return StartCoroutine(EquipGun(gunNum));
        }
    }

    IEnumerator Swap(NetworkGun gun)
    {
        yield return StartCoroutine(DequipGun());

        playerRig.SetIKRig(null);
        playerRig.SetHidden(true);

        gunNum = (gunNum + 1) % guns.Length;

        yield return StartCoroutine(EquipGun(gunNum));
    }

    public NetworkGun GetCurrentGun()
    {
        return guns[gunNum];
    }

    public bool HasGun(NetworkGun _gun)
    {
        foreach (NetworkGun gun in guns)
        {
            if (gun == _gun)
            {
                return true;
            }
        }

        return false;
    }

    //-----Server-----//
    [Command]
    void CmdPickUpGun()
    {
        RpcPickUpGun();
    }

    [Command]
    void CmdSwapGuns()
    {
        RpcSwapGuns();
    }

    //------Client-----//
    [ClientRpc]
    void RpcPickUpGun()
    {
        StopAllCoroutines();
        //StartCoroutine(GunPickUp());
    }

    [ClientRpc]
    void RpcSwapGuns()
    {
        NetworkGun gun = guns[(gunNum + 1) % guns.Length];

        if (gun == null || gun == guns[gunNum])
            return;

        StopAllCoroutines();
        StartCoroutine(Swap(gun));
    }

}
    
