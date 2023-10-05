using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;
using Mirror;

public class AttackAnimation : NetworkBehaviour
{
    public CinemachineVirtualCamera animationCamera;
    public CinemachineDollyCart cart;
    public Animator animator;

    public bool animating = false;
    public int damage;

    [ServerCallback]
    private void OnEnable()
    {
        StartAnimation();
    }

    public void StartAnimation()
    {
        //animationCamera.enabled = true;
        animator.enabled = true;

        if(cart)
            cart.m_Position = 0;
        if (animationCamera)
            animationCamera.enabled = true;

        animating = true;
    }

    public void DealDamage()
    {
        print("Animation Deal " + damage + " UI Damage");
    }

    public void EndAnimation()
    {
        if (animationCamera)
            animationCamera.enabled = false;

        animating = false;
    }
}
