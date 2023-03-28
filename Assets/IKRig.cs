using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKRig : MonoBehaviour
{
    public Transform LeftHand, LeftThumb, LeftIndex, LeftMiddle, LeftRing, LeftPinky;
    public Transform RightHand, RightThumb, RightIndex, RightMiddle, RightRing, RightPinky;

    public Transform LeftHandHint, LeftThumbHint, LeftIndexHint, LeftMiddleHint, LeftRingHint, LeftPinkyHint;
    public Transform RightHandHint, RightThumbHint, RightIndexHint, RightMiddleHint, RightRingHint, RightPinkyHint;

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
