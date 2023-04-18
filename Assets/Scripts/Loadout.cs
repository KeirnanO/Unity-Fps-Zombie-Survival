using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Loadout : MonoBehaviour
{
    public Gun[] guns = new Gun[2];

    [SerializeField] int gunNum = 0;
    [SerializeField] Gun currentGun;

    public PlayerIKRig playerRig;

    public Gun[] WeaponArray;

    public void EquipGun(Gun gun)
    {
        if (gun == currentGun && currentGun != null)
        {
            gun.RefillAmmo();
            return;
        }

        if (currentGun != null)
            StartCoroutine(DequipGun());

        if (guns[0] == gun || guns[0] == null)
        {
            guns[0] = gun;
            StartCoroutine(EquipGun(0));
        }
        else if (guns[1] == gun || guns[1] == null)
        {
            guns[1] = gun;
            StartCoroutine(EquipGun(1));
        }
        else
        {
            guns[gunNum] = gun;
            StartCoroutine(EquipGun(gunNum));
        }
    }

    IEnumerator EquipGun(int gunNum)
    {
        if (guns[gunNum] == null)
        {
            currentGun = guns[gunNum];
            playerRig.SetHidden(true);
        }
        else
        {
            guns[gunNum].gameObject.SetActive(true);

            this.gunNum = gunNum;
            playerRig.SetIKRig(guns[gunNum].GetIKRig());
            guns[gunNum].GetComponent<Gun>().enabled = true;
            currentGun = guns[gunNum];
            yield return null;

            playerRig.SetHidden(false);
        }
    }

    IEnumerator DequipGun()
    {
        if (guns[gunNum])
        {
            guns[gunNum].Dequip();
            //guns[gunNum].GetComponent<Gun>().enabled = false;

            yield return new WaitUntil(() => guns[gunNum].gameObject.activeSelf == false);
        }
    }

    public void PickUpGun(Gun gun)
    {
        StopAllCoroutines();
        StartCoroutine(GunPickUp(gun));
    }

    IEnumerator GunPickUp(Gun gun)
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

    public void SwapGuns()
    {
        Gun gun = guns[(gunNum + 1) % guns.Length];

        if (gun == null || gun == guns[gunNum])
            return;

        StopAllCoroutines();
        StartCoroutine(Swap(gun));
    }

    IEnumerator Swap(Gun gun)
    {
        yield return StartCoroutine(DequipGun());

        playerRig.SetIKRig(null);
        playerRig.SetHidden(true);

        gunNum = (gunNum + 1) % guns.Length;

        yield return StartCoroutine(EquipGun(gunNum));
    }

    public Gun GetCurrentGun()
    {
        return currentGun;
    }

    public bool HasGun(Gun _gun)
    {
        foreach(Gun gun in guns)
        {
            if(gun == _gun)
            {
                return true;
            }
        }

        return false;
    }
}
