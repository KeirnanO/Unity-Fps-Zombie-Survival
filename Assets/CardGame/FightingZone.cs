using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Cinemachine;
using System;


//This class should only handle the display of the battlefield and any graphics display around each fighting entity (ex. pips, wards, charms)

public class FightingZone : NetworkBehaviour
{
    [SerializeField] public Transform[] TeamAPositions;
    [SerializeField] public Transform[] TeamBPositions;

    public readonly SyncList<FightEntity> TeamAEntities = new SyncList<FightEntity>();
    public readonly SyncList<FightEntity> TeamBEntities = new SyncList<FightEntity>();

    public CinemachineVirtualCamera BattleCamera;
    public CinemachineVirtualCamera PlanningCamera;

    [Client]
    public void ChangeCamera(bool battle)
    {
        PlanningCamera.enabled = !battle;
        BattleCamera.enabled = battle;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(transform.position, new Vector3(4f, 0.1f, 4f));
    }
}