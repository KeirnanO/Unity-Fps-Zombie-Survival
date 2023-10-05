using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKRig : MonoBehaviour
{
    public Transform LeftHand, LeftThumb, LeftIndex, LeftMiddle, LeftRing, LeftPinky;
    public Transform RightHand, RightThumb, RightIndex, RightMiddle, RightRing, RightPinky;

    public Transform LeftHandHint, LeftThumbHint, LeftIndexHint, LeftMiddleHint, LeftRingHint, LeftPinkyHint;
    public Transform RightHandHint, RightThumbHint, RightIndexHint, RightMiddleHint, RightRingHint, RightPinkyHint;

    public Transform IKTargets;

    private void Awake()
    {
        if(IKTargets != null)
            AssignIKTargets();
    }

    void AssignIKTargets()
    {
        Transform LeftHand = IKTargets.Find("LeftHandIKTarget");
        Transform LeftThumb = LeftHand.Find("ThumbIKTarget");
        Transform LeftIndex = LeftHand.Find("IndexIKTarget");
        Transform LeftMiddle = LeftHand.Find("MiddleIKTarget");
        Transform LeftRing = LeftHand.Find("RingIKTarget");
        Transform LeftPinky = LeftHand.Find("PinkyIKTarget");

        Transform LeftHandHint = IKTargets.Find("LeftHandHint");
        Transform LeftThumbHint = LeftHand.Find("ThumbIKHint");
        Transform LeftIndexHint = LeftHand.Find("IndexIKHint");
        Transform LeftMiddleHint = LeftHand.Find("MiddleIKHint");
        Transform LeftRingHint = LeftHand.Find("RingIKHint");
        Transform LeftPinkyHint = LeftHand.Find("PinkyIKHint");

        Transform RightHand = IKTargets.Find("RightHandIKTarget");
        Transform RightThumb = RightHand.Find("ThumbIKTarget");
        Transform RightIndex = RightHand.Find("IndexIKTarget");
        Transform RightMiddle = RightHand.Find("MiddleIKTarget");
        Transform RightRing = RightHand.Find("RingIKTarget");
        Transform RightPinky = RightHand.Find("PinkyIKTarget");

        Transform RightHandHint = IKTargets.Find("RightHandHint");
        Transform RightThumbHint = RightHand.Find("ThumbIKHint");
        Transform RightIndexHint = RightHand.Find("IndexIKHint");
        Transform RightMiddleHint = RightHand.Find("MiddleIKHint");
        Transform RightRingHint = RightHand.Find("RingIKHint");
        Transform RightPinkyHint = RightHand.Find("PinkyIKHint");

        SetIKRig(
            LeftHand, LeftHandHint, LeftThumb, LeftThumbHint, LeftIndex, LeftIndexHint, LeftMiddle, LeftMiddleHint, LeftRing, LeftRingHint, LeftPinky, LeftPinkyHint,
            RightHand, RightHandHint, RightThumb, RightThumbHint, RightIndex, RightIndexHint, RightMiddle, RightMiddleHint, RightRing, RightRingHint, RightPinky, RightPinkyHint
            );
    }

    public void SetIKRig(
        Transform leftHand, Transform leftHandHint, Transform leftThumb, Transform leftThumbHint, Transform leftIndex, Transform leftIndexHint, Transform leftMiddle, Transform leftMiddleHint, Transform leftRing, Transform leftRingHint, Transform leftPinky, Transform leftPinkyHint,
        Transform rightHand, Transform rightHandHint, Transform rightThumb, Transform rightThumbHint, Transform rightIndex, Transform rightIndexHint, Transform rightMiddle, Transform rightMiddleHint, Transform rightRing, Transform rightRingHint, Transform rightPinky, Transform rightPinkyHint)
    {
        LeftHand = leftHand;
        LeftThumb = leftThumb;
        LeftIndex = leftIndex;
        LeftMiddle = leftMiddle;
        LeftRing = leftRing;
        LeftPinky = leftPinky;

        LeftHandHint = leftHandHint;
        LeftThumbHint = leftThumbHint;        
        LeftIndexHint = leftIndexHint;
        LeftMiddleHint = leftMiddleHint;
        LeftRingHint = leftRingHint;
        LeftPinkyHint = leftPinkyHint;

        RightHand = rightHand;
        RightThumb = rightThumb;
        RightIndex = rightIndex;
        RightMiddle = rightMiddle;
        RightRing = rightRing;
        RightPinky = rightPinky;

        RightHandHint = rightHandHint;
        RightThumbHint = rightThumbHint;
        RightIndexHint = rightIndexHint;
        RightMiddleHint = rightMiddleHint;
        RightRingHint = rightRingHint;
        RightPinkyHint = rightPinkyHint;
    }
}
