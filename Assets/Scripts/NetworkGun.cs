using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkGun : NetworkBehaviour
{
    [SerializeField] LayerMask ShootableMask;

    public GameObject bulletSound;
    public GameObject hitMarkerSound;
    public GameObject BloodEffect;

    public Transform IKTargets;

    public Animator animator;

    public bool IsAiming;

    public GunRecoil recoil;
    public CameraRecoil cameraRecoil;

    float timetoFire = 0;
    public float FireRate; //Bullets Per Second

    public IKRig m_IKRig;

    public int ammo;
    public int maxammo;
    public int ammoInClip;
    public int magSize;

    public float damage;
    float damageMultiplyer = 1;

    public string GunName;

    private void Awake()
    {
        AssignIKTargets();

        animator = GetComponent<Animator>();

        //GunDatabase.RegisterGun(this, GunName);

        gameObject.SetActive(false);
    }


    //-----Local-----//
    public void Fire()
    {
        CmdFire();
        ammoInClip--;
    }

    public void Reload()
    {
        CmdReload();

        if (ammo > 0 && ammoInClip <= magSize)
        {
            animator.SetBool("IsReloading", true);
        }
    }

    public void Aim(bool aimState)
    {
        IsAiming = aimState;
    }

    public void Dequip()
    {
        animator.SetTrigger("Dequip");
    }

    public void SetInActive()
    {
        gameObject.SetActive(false);
    }

    public void ReloadAmmo()
    {
        CmdReload();

        int fillAmount = magSize - ammoInClip;

        if (ammo > fillAmount)
        {
            ammo -= fillAmount;

            ammoInClip = magSize;
        }
        else
        {
            ammoInClip += ammo;
            ammo = 0;
        }
    }

    public void RefillAmmo()
    {
        ammo = maxammo + (magSize - ammoInClip);
    }
    //-----Server----//
    [Command]
    void CmdFire()
    {
        if(ammoInClip > 0)
        {
            RpcFire();

            ammoInClip--;
        }
        else
        {
            //DryFire();
        }
    }

    [Command]
    void CmdReload()
    {
        int fillAmount = magSize - ammoInClip;

        if (ammo > fillAmount)
        {
            ammo -= fillAmount;

            ammoInClip = magSize;
        }
        else
        {
            ammoInClip += ammo;
            ammo = 0;
        }
    }

    //-----Client----//
    [ClientRpc]
    void RpcFire()
    {
        if (Time.time > timetoFire && ammoInClip > 0)
        {
            ammoInClip--;
            Instantiate(bulletSound, transform.position, Quaternion.identity);

            Vector2 centerScreenPoint = new(Screen.width / 2, Screen.height / 2);
            if (Physics.Raycast(Camera.main.ScreenPointToRay(centerScreenPoint), out RaycastHit hit, 999f, ShootableMask))
            {

                if (hit.transform.CompareTag("Enemy"))
                {
                    Instantiate(hitMarkerSound, transform.position, Quaternion.identity);
                    Transform effect = Instantiate(BloodEffect, hit.point, Quaternion.identity).transform;
                    effect.LookAt(hit.point + hit.normal);

                    hit.transform.GetComponent<ShootableObject>().GetShot(damage * damageMultiplyer, 0);
                }

            }

            recoil.ApplyRecoil();
            cameraRecoil.ApplyRecoil();
            //animator.SetTrigger("Fire");



            timetoFire = Time.time + 1 / FireRate;
        }
        else
        {
            //Play mag empty sound
        }
    }


    void AssignIKTargets()
    {
        Transform LeftHand           = IKTargets.Find("LeftHandIKTarget");
        Transform LeftThumb          = LeftHand.Find("ThumbIKTarget");
        Transform LeftIndex          = LeftHand.Find("IndexIKTarget");
        Transform LeftMiddle         = LeftHand.Find("MiddleIKTarget");
        Transform LeftRing           = LeftHand.Find("RingIKTarget");
        Transform LeftPinky          = LeftHand.Find("PinkyIKTarget");

        Transform LeftHandHint       = IKTargets.Find("LeftHandHint");
        Transform LeftThumbHint      = LeftHand.Find("ThumbIKHint");
        Transform LeftIndexHint      = LeftHand.Find("IndexIKHint");
        Transform LeftMiddleHint     = LeftHand.Find("MiddleIKHint");
        Transform LeftRingHint       = LeftHand.Find("RingIKHint");
        Transform LeftPinkyHint      = LeftHand.Find("PinkyIKHint");

        Transform RightHand          = IKTargets.Find("RightHandIKTarget");
        Transform RightThumb         = RightHand.Find("ThumbIKTarget");
        Transform RightIndex         = RightHand.Find("IndexIKTarget");
        Transform RightMiddle        = RightHand.Find("MiddleIKTarget");
        Transform RightRing          = RightHand.Find("RingIKTarget");
        Transform RightPinky         = RightHand.Find("PinkyIKTarget");

        Transform RightHandHint      = IKTargets.Find("RightHandHint");
        Transform RightThumbHint     = RightHand.Find("ThumbIKHint");
        Transform RightIndexHint     = RightHand.Find("IndexIKHint");
        Transform RightMiddleHint    = RightHand.Find("MiddleIKHint");
        Transform RightRingHint      = RightHand.Find("RingIKHint");
        Transform RightPinkyHint     = RightHand.Find("PinkyIKHint");

        m_IKRig.SetIKRig(
            LeftHand, LeftHandHint,  LeftThumb, LeftThumbHint, LeftIndex, LeftIndexHint, LeftMiddle, LeftMiddleHint, LeftRing, LeftRingHint, LeftPinky, LeftPinkyHint,
            RightHand, RightHandHint, RightThumb, RightThumbHint, RightIndex, RightIndexHint, RightMiddle, RightMiddleHint, RightRing, RightRingHint, RightPinky, RightPinkyHint
            );
    }

    public IKRig GetIKRig()
    {
        return m_IKRig;
    }
}
