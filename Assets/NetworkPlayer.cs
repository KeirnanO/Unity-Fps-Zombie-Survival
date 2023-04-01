using Mirror;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using StarterAssets;

public class NetworkPlayer : NetworkBehaviour
{

    [SerializeField]
    NetworkFirstPersonController m_Controller;

    [SerializeField]
    GameObject m_Camera;

    private void Awake()
    {
        if (!isLocalPlayer && !isClient)
        {
            print("no local player");
            return;
        }

        if (m_Camera == null)
        {
            m_Camera = GameObject.FindGameObjectWithTag("MainCamera");
        }

        StarterAssetsInputs input = gameObject.AddComponent<StarterAssetsInputs>();

        print(input.transform);

        //m_Controller.SetCamera(m_Camera);
    }

    public override void OnStartLocalPlayer()
    {
        m_Controller = GetComponent<NetworkFirstPersonController>();
    }

    private void Start()
    {
        if (!isLocalPlayer)
            return;       

        m_Camera.SetActive(true);
    }

    private void Update()
    {
        if (!isLocalPlayer)
            return;
    }    
}
