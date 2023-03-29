using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GunDatabase
{

    public static Dictionary<Gun, string> guns = new Dictionary<Gun, string>();


    public static void RegisterGun(Gun gun, string name)
    {
        guns.Add(gun, name);
    }

    public static Gun GetRegisteredGun(string name)
    {
        foreach(var gun in guns)
        {
            if(gun.Value.Equals(name))
            {
                return gun.Key;
            }
        }

        return null;
    }
}
