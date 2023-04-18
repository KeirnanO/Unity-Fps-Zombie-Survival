using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Cinemachine;

public class NetworkCameraController : NetworkBehaviour
{
	[Header("Cinemachine")]
	[Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
	[SerializeField] private GameObject CinemachineCameraTarget;
	[Tooltip("How far in degrees can you move the camera up")]
	[SerializeField] private float TopClamp = 90.0f;
	[Tooltip("How far in degrees can you move the camera down")]
	[SerializeField] private float BottomClamp = -90.0f;
	[Tooltip("Rotation speed of the character")]
	[SerializeField] private float RotationSpeed = 1.0f;
	[Tooltip("The Inital Camera On Start")]
	[SerializeField] private CinemachineVirtualCamera virtualCamera = null;

	private float _rotationVelocity;
	private float _cinemachineTargetPitch;

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
		virtualCamera.enabled = true;

		enabled = true;

		Controls.Player.Look.performed += ctx => CameraRotation(ctx.ReadValue<Vector2>());
    }

	[ClientCallback]
	private void OnEnable() => Controls.Enable();
	[ClientCallback]
	private void OnDisable() => Controls.Disable();


	private void CameraRotation(Vector2 lookAxis)
	{
		_cinemachineTargetPitch += lookAxis.y * RotationSpeed;
		_rotationVelocity = lookAxis.x * RotationSpeed;

		// clamp our pitch rotation
		_cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

		// Update Cinemachine camera target pitch
		CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);

		// rotate the player left and right
		transform.Rotate(Vector3.up * _rotationVelocity);
	}

	private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
	{
		if (lfAngle < -360f) lfAngle += 360f;
		if (lfAngle > 360f) lfAngle -= 360f;
		return Mathf.Clamp(lfAngle, lfMin, lfMax);
	}
}
