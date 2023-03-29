using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerIKRig : MonoBehaviour
{
    IKRig currentRig;

    public TwoBoneIKConstraint LeftHandConstraint, LeftThumbConstraint, LeftIndexConstraint, LeftMiddleConstraint, LeftRingConstraint, LeftPinky;
    public TwoBoneIKConstraint RightHandConstraint, RightThumbConstraint, RightIndexConstraint, RightMiddleConstraint, RightRingConstraint, RightPinky;

    public Rig[] rigs;

    public GameObject[] limbs;

    public void Update()
    {
        if (currentRig == null)
        {
            print("No Gun Rig for player");
            return;
        }

        LeftHandConstraint.data.target.position = currentRig.LeftHand.position;
        LeftThumbConstraint.data.target.position = currentRig.LeftThumb.position;
        LeftIndexConstraint.data.target.position = currentRig.LeftIndex.position;
        LeftMiddleConstraint.data.target.position = currentRig.LeftMiddle.position;
        LeftRingConstraint.data.target.position = currentRig.LeftRing.position;
        LeftPinky.data.target.position = currentRig.LeftPinky.position;

        LeftHandConstraint.data.hint.position = currentRig.LeftHandHint.position;
        LeftThumbConstraint.data.hint.position = currentRig.LeftThumbHint.position;
        LeftIndexConstraint.data.hint.position = currentRig.LeftIndexHint.position;
        LeftMiddleConstraint.data.hint.position = currentRig.LeftMiddleHint.position;
        LeftRingConstraint.data.hint.position = currentRig.LeftRingHint.position;
        LeftPinky.data.hint.position = currentRig.LeftPinkyHint.position;

        RightHandConstraint.data.target.position = currentRig.RightHand.position;
        RightThumbConstraint.data.target.position = currentRig.RightThumb.position;
        RightIndexConstraint.data.target.position = currentRig.RightIndex.position;
        RightMiddleConstraint.data.target.position = currentRig.RightMiddle.position;
        RightRingConstraint.data.target.position = currentRig.RightRing.position;
        RightPinky.data.target.position = currentRig.RightPinky.position;

        RightHandConstraint.data.hint.position = currentRig.RightHandHint.position;
        RightThumbConstraint.data.hint.position = currentRig.RightThumbHint.position;
        RightIndexConstraint.data.hint.position = currentRig.RightIndexHint.position;
        RightMiddleConstraint.data.hint.position = currentRig.RightMiddleHint.position;
        RightRingConstraint.data.hint.position = currentRig.RightRingHint.position;
        RightPinky.data.hint.position = currentRig.RightPinkyHint.position;

        LeftHandConstraint.data.target.rotation = currentRig.LeftHand.rotation;
        LeftThumbConstraint.data.target.rotation = currentRig.LeftThumb.rotation;
        LeftIndexConstraint.data.target.rotation = currentRig.LeftIndex.rotation;
        LeftMiddleConstraint.data.target.rotation = currentRig.LeftMiddle.rotation;
        LeftRingConstraint.data.target.rotation = currentRig.LeftRing.rotation;
        LeftPinky.data.target.rotation = currentRig.LeftPinky.rotation;

        LeftHandConstraint.data.hint.rotation = currentRig.LeftHandHint.rotation;
        LeftThumbConstraint.data.hint.rotation = currentRig.LeftThumbHint.rotation;
        LeftIndexConstraint.data.hint.rotation = currentRig.LeftIndexHint.rotation;
        LeftMiddleConstraint.data.hint.rotation = currentRig.LeftMiddleHint.rotation;
        LeftRingConstraint.data.hint.rotation = currentRig.LeftRingHint.rotation;
        LeftPinky.data.hint.rotation = currentRig.LeftPinkyHint.rotation;

        RightHandConstraint.data.target.rotation = currentRig.RightHand.rotation;
        RightThumbConstraint.data.target.rotation = currentRig.RightThumb.rotation;
        RightIndexConstraint.data.target.rotation = currentRig.RightIndex.rotation;
        RightMiddleConstraint.data.target.rotation = currentRig.RightMiddle.rotation;
        RightRingConstraint.data.target.rotation = currentRig.RightRing.rotation;
        RightPinky.data.target.rotation = currentRig.RightPinky.rotation;

        RightHandConstraint.data.hint.rotation = currentRig.RightHandHint.rotation;
        RightThumbConstraint.data.hint.rotation = currentRig.RightThumbHint.rotation;
        RightIndexConstraint.data.hint.rotation = currentRig.RightIndexHint.rotation;
        RightMiddleConstraint.data.hint.rotation = currentRig.RightMiddleHint.rotation;
        RightRingConstraint.data.hint.rotation = currentRig.RightRingHint.rotation;
        RightPinky.data.hint.rotation = currentRig.RightPinkyHint.rotation;
    }

    public void SetIKRig(IKRig rig)
    {
        if(rig == null) print("Null rig");
        currentRig = rig;
    }

    public void SetRigWeights(int weight)
    {
        foreach(Rig rig in rigs)
        {
            rig.weight = weight;
        }
    }

    public void SetActive(bool activeState)
    {
        gameObject.SetActive(activeState);
    }

    public void SetHidden(bool hiddenState)
    {
        foreach(GameObject obj in limbs)
        {
            obj.SetActive(!hiddenState);
        }
    }
}
