using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loadout : MonoBehaviour
{
    public Gun[] guns = new Gun[2];

    int gunNum;
    PlayerIKRig playerRig;

    private void Awake()
    {
        playerRig = FindObjectOfType<PlayerIKRig>();        
    }

    IEnumerator EquipGun(int gunNum)
    {
        playerRig.SetIKRig(guns[gunNum].GetIKRig());

        guns[gunNum].gameObject.SetActive(true);
        guns[gunNum].GetComponent<Gun>().enabled = true;

        yield return null;
        
        playerRig.SetHidden(false);
    }

    IEnumerator DequipGun()
    {
        guns[gunNum].Dequip();
        //guns[gunNum].GetComponent<Gun>().enabled = false;

        yield return new WaitUntil(() => guns[gunNum].gameObject.activeSelf == false);
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
        return guns[gunNum];
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
