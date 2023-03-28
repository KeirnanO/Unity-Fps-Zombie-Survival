using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRecoil : MonoBehaviour
{
    private Vector3 currentRotation;
    private Vector3 targetRotation;
    private Vector3 returnRotation;

    [SerializeField] private float recoilX;
    [SerializeField] private float recoilY;
    [SerializeField] private float recoilZ;

    [SerializeField] private float aimRecoilX;
    [SerializeField] private float aimRecoilY;
    [SerializeField] private float aimRecoilZ;

    [SerializeField] private float snappiness;
    [SerializeField] private float returnSpeed;

    public Transform target;

    private void Update()
    {
        targetRotation = Vector3.Lerp(targetRotation, returnRotation, returnSpeed * Time.deltaTime);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, snappiness * Time.fixedDeltaTime);
        transform.localRotation = Quaternion.Euler(currentRotation);
    }

    public void ApplyRecoil()
    {
        Vector3 recoil = new(recoilX, Random.Range(-recoilY, recoilY), Random.Range(-recoilZ, recoilZ));
        targetRotation += recoil;
    }

    public void ApplyAimRecoil()
    {
        Vector3 recoil = new(aimRecoilX, Random.Range(-aimRecoilY, aimRecoilY), Random.Range(-aimRecoilZ, aimRecoilZ));
        targetRotation += recoil;
    }
}
