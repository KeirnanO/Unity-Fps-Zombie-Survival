using Mirror;
using UnityEngine;

public class NetworkMovementController : NetworkBehaviour
{
	[Header("Player")]
	[Tooltip("Move speed of the character in m/s")]
	[SerializeField] private float MoveSpeed = 4.0f;
	[Tooltip("Sprint speed of the character in m/s")]
	[SerializeField] private float SprintSpeed = 6.0f;
	[Tooltip("Acceleration and deceleration")]
	[SerializeField] private float SpeedChangeRate = 10.0f;

	[Space(10)]
	[Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
	[SerializeField] private float JumpTimeout = 0.1f;
	[Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
	[SerializeField] private float FallTimeout = 0.15f;

	[Header("Player Grounded")]
	[Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
	[SerializeField] private bool Grounded = true;
	[Tooltip("Useful for rough ground")]
	[SerializeField] private float GroundedOffset = -0.14f;
	[Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
	[SerializeField] private float GroundedRadius = 0.5f;
	[Tooltip("What layers the character uses as ground")]
	[SerializeField] private LayerMask GroundLayers;

	[Space(10)]
	[Tooltip("The height the player can jump")]
	[SerializeField] private float JumpHeight = 1.2f;
	[Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
	[SerializeField] private float Gravity = -15.0f;

	[Header("Character Controller")]
	[SerializeField] private CharacterController controller = null;

	private Vector2 previousInput;

	//Movement
	private float _speed = 4.0f;
	private float _verticalVelocity;
	private float _terminalVelocity = 53.0f;

	// timeout deltatime
	private float _jumpTimeoutDelta;
	private float _fallTimeoutDelta;

	[SerializeField] private bool isSprint;

	private Controls controls;

	private Controls Controls
    {
        get
        {
			if(controls != null) { return controls; }
			return controls = new Controls();
        }
    }

    public override void OnStartAuthority()
    {
		enabled = true;

		Controls.Player.Move.performed += ctx => SetMovement(ctx.ReadValue<Vector2>());
		Controls.Player.Move.canceled += ctx => ResetMovement();

		Controls.Player.Sprint.started += ctx => isSprint = true;
		Controls.Player.Sprint.canceled += ctx => isSprint = false;

		Controls.Player.Jump.performed += ctx => Jump();
	}

	[ClientCallback]
	private void OnEnable() => Controls.Enable();
	[ClientCallback]
	private void OnDisable() => Controls.Disable();
	[ClientCallback]
	private void Update() => Move();

	[Client]
	private void SetMovement(Vector2 movement) => previousInput = movement;
	[Client]
	private void ResetMovement() => previousInput = Vector2.zero;

    [Client]
	private void GroundedCheck()
	{
		// set sphere position, with offset
		Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
		Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
	}

    [Client]
	private void ApplyGravity()
    {
		// apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
		if (_verticalVelocity < _terminalVelocity)
		{
			_verticalVelocity += Gravity * Time.deltaTime;
		}
	}


	[Client]
	private void Move()
    {
		GroundedCheck();
		ApplyGravity();

		float targetSpeed = isSprint? SprintSpeed: MoveSpeed;

		if (previousInput == Vector2.zero)
			targetSpeed = 0.0f;

		float currentHorizontalSpeed = new Vector3(previousInput.x, 0.0f, previousInput.y).magnitude;

		float speedOffset = 0.1f;

		// accelerate or decelerate to target speed
		if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
		{
			// creates curved result rather than a linear one giving a more organic speed change
			// note T in Lerp is clamped, so we don't need to clamp our speed
			_speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed, Time.deltaTime * SpeedChangeRate);

			// round speed to 3 decimal places
			_speed = Mathf.Round(_speed * 1000f) / 1000f;
		}
		else
		{
			_speed = targetSpeed;
		}
		// normalise input direction
		Vector3 inputDirection = new Vector3(previousInput.x, 0.0f, previousInput.y).normalized;

		// note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
		// if there is a move input rotate player when the player is moving
		if (inputDirection != Vector3.zero)
		{
			// move
			inputDirection = transform.right * previousInput.x + transform.forward * previousInput.y;
		}

		print("Input = " + inputDirection);
		print("Horizontal Speed = " + inputDirection.normalized * (_speed * Time.deltaTime) + " ** Vertical Speed = " + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

		// move the player
		controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
	}

    [Client]
	private void Jump()
    {
		if (Grounded)
		{
			if (Time.time < _jumpTimeoutDelta)
				return;

			_verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);						
			_jumpTimeoutDelta = Time.time + JumpTimeout;
		}
	}

    [TargetRpc]
	public void SetPositionAndRotation(Vector3 position, Quaternion rotation)
    {
        controller.enabled = false;

		transform.SetPositionAndRotation(position, rotation);

		controller.enabled = true;
    }
}
