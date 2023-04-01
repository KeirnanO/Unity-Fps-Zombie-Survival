using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallPurchase : Interactable
{
    int cost = 0;
    public int gunCost = 100;
    public int ammoCost = 100;

    public Gun purchasedGun;

    public override void Interact()
    {
        base.Interact();

        if(GameManager.instance.TakePlayerPoints(cost))
        {
            if (GameManager.instance.GetPlayer().loadout.HasGun(purchasedGun))
            {
                purchasedGun.RefillAmmo();
            }
            else
            {
                GameManager.instance.GetPlayer().loadout.EquipGun(purchasedGun);
            }          
        }
    }

    public override string GetTooltip()
    {
        string tooltip;

        if (GameManager.instance.GetPlayer().loadout.HasGun(purchasedGun))
        {
            cost = ammoCost;
            tooltip = "Hold 'E' to refill ammo : " + cost;
        }
        else
        {
            cost = gunCost;
            tooltip = "Hold 'E' to buy " + purchasedGun.GunName + " : " + cost;
        }

        return tooltip;
    }

}
