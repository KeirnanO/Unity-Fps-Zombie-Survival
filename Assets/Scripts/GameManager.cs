using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public AimController player;

    static public GameManager instance;
    private void Start()
    {
        instance = this;

        player.loadout.EquipGun(player.loadout.WeaponArray[0]);
    }

    public void GivePlayerPoints(int points)
    {
        player.GivePoints(points);

        UIHandler.instance.CreatePointPrefab(points);
    }

    public bool TakePlayerPoints(int points)
    {
        if (player.TakePoints(points))
        {
            UIHandler.instance.CreatePointPrefab(-points);
            return true;
        }

        return false;
    }

    public void GivePlayerAmmo()
    {
        Gun gun = player.loadout.GetCurrentGun();
        gun.ammo = gun.maxammo + (gun.magSize - gun.ammoInClip);
    }

    public void DisplayToolTip(string tooltip)
    {
        UIHandler.instance.DisplayToolTip(tooltip);
    }

    public AimController GetPlayer()
    {
        return player;
    }
}
