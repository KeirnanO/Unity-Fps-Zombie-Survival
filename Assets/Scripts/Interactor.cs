using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Interactor : NetworkBehaviour
{
    [SerializeField] private LayerMask interactableMask = 1 << 0;

    private Controls controls;

    private Controls Controls
    {
        get
        {
            if (controls != null) { return controls; }
            return controls = new Controls();
        }
    }

    public override void OnStartAuthority()
    {
        enabled = true;
    }

    [ClientCallback]
    private void OnEnable() => Controls.Enable();
    [ClientCallback]
    private void OnDisable() => Controls.Disable();
    [ClientCallback]
    private void Update()
    {
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, 999f,interactableMask))
        {
            if (Controls.Player.Interact.triggered)
            {
                hit.transform.GetComponent<NetworkInteractable>().Interact(netId);
                Controls.Player.Interact.Reset();
            }
        }
        else
        {
            Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * 2f);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * 2f);
    }


}
