using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public enum ManaType
{
    Normal = 0,
    Power = 1,
    Ice = 2,
    Fire = 3,
}

public class Mana : NetworkBehaviour
{
    //ManaTypes which determine the type of mana in the pool
    //0: Normal - A single mana used as base mana for cards
    //1: Power - Counts as 2 mana used as base mana for cards
    //2: Ice - Counts as 3 mana towards base mana OR Counts as 1 ice-mana 
    //3: Fire - Counts as 3 mana towards base mana Or Counts as 1 fire-mana
      
    public List<ManaType> manaPool;

    [Server]
    public void AddMana(int manaType)
    {
        manaPool.Add((ManaType)manaType);
        RpcAddMana(manaType);
    }

    [Server]
    public void ClearMana()
    {
        manaPool.Clear();

        RpcClearMana();
    }

    [ClientRpc]
    public void RpcClearMana()
    {
        manaPool.Clear();
    }

    /// <summary>
    /// Removes Regular mana until a card is paid.
    /// Will still remove mana even if no mana is available
    /// Ensure the manaPool can cover the incoming manaList before calling this metehod
    /// </summary>
    /// <param name="manaTypes"></param>
    [Server]
    public void RemoveMana(List<ManaType> manaTypes)
    {
        //Determines Regular cost of the card
        int cost = 0;

        //Remove specific manaTypes from pool
        foreach (ManaType mana in manaTypes)
        {
            if (mana != ManaType.Normal) { manaPool.Remove(mana); RpcRemoveMana((int)mana); continue; }

            cost++;
        }


        while (cost > 0)
        {
            if (cost == 1 && manaPool.Contains(ManaType.Normal)) { manaPool.Remove(ManaType.Normal); break; }

            if (manaPool.Contains(ManaType.Power)) { manaPool.Remove(ManaType.Power); RpcRemoveMana(2); cost -= 2; continue; }
            else { manaPool.Remove(ManaType.Normal); RpcRemoveMana(1); cost -= 1; continue; }
        }
    }

    //Add Mana To All Clients
    [ClientRpc]
    public void RpcAddMana(int manaType)
    {
        //Only necessary becuase client can be a server
        if (isServer)
            return;

        manaPool.Add((ManaType)manaType);
    }

    //Remove Mana from AllClients
    [ClientRpc]
    public void RpcRemoveMana(int manaType)
    {
        //Only necessary becuase client can be a server
        if (isServer)
            return;

        manaPool.Remove((ManaType)manaType);
    }

    //Returns True if there is enough mana in the pool to cover manaTypes
    public bool CheckCost(List<ManaType> manaTypes)
    {
        int cost = 0;

        List<ManaType> tempManaPool = new List<ManaType>(manaPool);

        foreach (ManaType mana in manaTypes)
        {
            if (mana == ManaType.Normal) { cost++; continue; }

            if (!tempManaPool.Contains(mana))
                return false;

            tempManaPool.Remove(mana);
        }

        int manaAmount = 0;
        foreach (ManaType manaInPool in tempManaPool)
        {
            manaAmount += Mathf.Clamp((int)manaInPool + 1, 1, 3);
        }

        return manaAmount >= cost;
    }

    static int SortByManaType(ManaType type1, ManaType type2)
    {
        return ((int)type1).CompareTo((int)type2);
    }

    public void Sort()
    {
        manaPool.Sort(SortByManaType);
    }
}
