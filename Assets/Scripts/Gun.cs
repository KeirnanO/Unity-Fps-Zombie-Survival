using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] LayerMask ShootableMask;

    public AudioClip bulletSound;
    public GameObject hitMarkerSound;
    public GameObject BloodEffect;

    public Transform IKTargets;

    public Animator animator;
    public AudioSource audioSource;

    public bool IsAiming;

    public GunRecoil recoil;
    public CameraRecoil cameraRecoil;

    float timetoFire = 0;
    public float FireRate; //Bullets Per Second

    public IKRig m_IKRig;

    public bool automatic;
    bool engaged = false;

    public int ammo;
    public int maxammo;
    public int ammoInClip;
    public int magSize;

    public float damage;

    public string GunName;

    private void Awake()
    {
        AssignIKTargets();

        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();  

        //GunDatabase.RegisterGun(this, GunName);
    }

    //Fires a Blank - Useful for shooting on clients
    public void Fire()
    {
        if (ammoInClip <= 0)
            return;

        if (!automatic)
        {
            if (engaged)
                return;

            engaged = true;
        }

        if (Time.time < timetoFire)
            return;

        ammoInClip--;
        audioSource.PlayOneShot(bulletSound);

        recoil.ApplyRecoil();
        cameraRecoil.ApplyRecoil();

        timetoFire = Time.time + 1 / FireRate;
    }


    //Fires a bullet in direction from a players position
    //This "Bullet" can affect Objects it hits
    //Only the Server should call this method in an online game
    public void Fire(Vector3 position, Vector3 direction)
    {
        if (ammoInClip <= 0)
            return;

        if (!automatic)
        {
            if (engaged)
                return;

            engaged = true;
        }

        if (Time.time < timetoFire)
            return;

        ammoInClip--;
        audioSource.PlayOneShot(bulletSound);

        if (Physics.Raycast(position, direction, out RaycastHit hit, 999f, ShootableMask))
        {
            if (hit.transform.CompareTag("Enemy") || hit.transform.CompareTag("Player"))
            {
                //Instantiate(hitMarkerSound, transform.position, Quaternion.identity);
                //Transform effect = Instantiate(BloodEffect, hit.point, Quaternion.identity).transform;
                //effect.LookAt(hit.point + hit.normal);

                hit.transform.GetComponent<Limb>().GetShot(damage, 0);
            }
        }

        Debug.DrawRay(position, direction * 100, Color.red, 1f);

        recoil.ApplyRecoil();
        cameraRecoil.ApplyRecoil();

        timetoFire = Time.time + 1 / FireRate;
    }


    public bool Reload()
    {
        if (ammo > 0 && ammoInClip < magSize)
        {
            animator.SetBool("IsReloading", true);

            return true;
        }

        return false;
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
        int fillAmount = magSize - ammoInClip;

        if(ammo > fillAmount)
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

    public void Disengage()
    {
        engaged = false;
    }
}
