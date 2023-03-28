using UnityEngine;
using StarterAssets;

public class PlayerAnimationController : MonoBehaviour
{
    public Animator animator;

    public StarterAssetsInputs input;

    // Update is called once per frame
    void Update()
    {
        if(input.crouch)
        {
            animator.SetBool("IsCrouched", true);
        }
        else
        {
            animator.SetBool("IsCrouched", false);
        }
    }
}
