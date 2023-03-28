using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

public class AimController : MonoBehaviour
{
    StarterAssetsInputs input;

    public LayerMask InteractableLayerMask;

    public GameObject StandardCamera;
    public GameObject AimCamera;

    public Loadout loadout;

    public Transform debugTransform;

    public float maxHealth;
    public float health;

    public float regenTime;
    float timeToRegen;

    public float regenTickRate;
    float regenTickRateMod;
    float regenTick;

    public float regenStrength;

    bool regenerating;

    [SerializeField] int startingPoints;
    [SerializeField] int points;

    private void Awake()
    {
        input = GetComponent<StarterAssetsInputs>();
    }

    private void Start()
    {
        health = maxHealth;
        points = startingPoints;

        transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);    
    }

    private void Update()
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

        if(input.aim)
        {
            StandardCamera.SetActive(false);
            AimCamera.SetActive(true);
            loadout.GetCurrentGun().animator.SetBool("IsAiming", true);
            loadout.GetCurrentGun().Aim(true);
        }
        else
        {
            StandardCamera.SetActive(true);
            AimCamera.SetActive(false);
            loadout.GetCurrentGun().animator.SetBool("IsAiming", false);
            loadout.GetCurrentGun().Aim(false);
        }

        if(input.fire)
        {
            loadout.GetCurrentGun().Fire();
            //loadout.SwapGuns();
        }
        else
        {

        }

        

        Vector2 centerScreenPoint = new(Screen.width / 2, Screen.height / 2);
        if(Physics.Raycast(Camera.main.ScreenPointToRay(centerScreenPoint), out RaycastHit hit, 999f))
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
            GameManager.instance.DisplayToolTip("");
        }

        if (Time.time > timeToRegen)
        {
            Regen();
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        timeToRegen = Time.time + regenTime;
        regenerating = false;
    }

    void Heal(float amount)
    {
        if(health + amount > maxHealth)
        {
            health = maxHealth;
        }
        else
        {
            health += amount;
        }
    }

    void Regen()
    {
        if (health >= maxHealth) return;

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
        if(points >= amount)
        {
            points -= amount;
            return true;
        }

        return false;
    }

}
