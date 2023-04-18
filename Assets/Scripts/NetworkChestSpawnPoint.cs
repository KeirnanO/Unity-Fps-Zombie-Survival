using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkChestSpawnPoint : MonoBehaviour
{
    private void Awake() => NetworkChestSpawnSystem.AddSpawnPoint(transform);

    private void OnDestroy() => NetworkChestSpawnSystem.RemoveSpawnPoint(transform);

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawCube(transform.position, new Vector3(2, 1, 1));
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 2f);
    }
}
