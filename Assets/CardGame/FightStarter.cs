using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class FightStarter : NetworkBehaviour
{
    public NetworkIdentity otherMonster;

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            FindObjectOfType<BattleManager>().ServerAddMonsterToFight(netIdentity);
            FindObjectOfType<BattleManager>().ServerAddMonsterToFight(otherMonster);
            FindObjectOfType<BattleManager>().ServerAddPlayerToFight(true, other.GetComponent<NetworkIdentity>());

            enabled = false;
        }
    }
}
