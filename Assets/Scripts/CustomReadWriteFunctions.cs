using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public static class CustomReadWriteFunctions
{
    public static void WriteLoadout(this NetworkWriter writer, Loadout loadout)
    {
        writer.WriteString(loadout.GetCurrentGun().GunName);

        NetworkIdentity networkIdentity = loadout.GetComponent<NetworkIdentity>();
        writer.WriteNetworkIdentity(networkIdentity);
    }

    public static Loadout ReadLoadout(this NetworkReader reader)
    {
        string gunName = reader.ReadString();

        NetworkIdentity networkIdentity = reader.ReadNetworkIdentity();
        Loadout loadout = networkIdentity != null
            ? networkIdentity.GetComponent<Loadout>()
            : null;

        return loadout;
    }
}