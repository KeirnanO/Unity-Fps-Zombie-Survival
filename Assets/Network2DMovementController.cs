using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Network2DMovementController : NetworkBehaviour
{ 
	enum MoveState
	{
		None,
		Walk,
		Sprint
	};

	enum Direction
	{
		Down,
		Left,
		Right,
		Up
	};

	[SerializeField] private MoveState state;
	[SerializeField] private Direction direction;

	[Header("Player")]
	[Tooltip("Move speed of the character in m/s")]
	[SerializeField] private float MoveSpeed = 4.0f;
	[Tooltip("Sprint speed of the character in m/s")]
	[SerializeField] private float SprintSpeed = 6.0f;
	[Tooltip("Acceleration and deceleration")]
	[SerializeField] private float SpeedChangeRate = 10.0f;

	[SerializeField] private Rigidbody2D rb = null;

	private Vector2 previousInput;	

	public Animator clientAnimator;

	//Movement
	private float _speed = 4.0f;
	private Vector2 momentum = Vector2.zero;
	private bool isSprint = false;

	private Controls controls;

	private Controls Controls
	{
		get
		{
			if (controls != null) { return controls; }
			return controls = new Controls();
		}
	}

	private bool interacting = false;
	private float invicibleTimeoutDelta = 0;
	private float invicibleTime= 3f;

	public override void OnStartAuthority()
	{
		enabled = true;

		Controls.Player.Move.performed += ctx => SetMovement(ctx.ReadValue<Vector2>());
		Controls.Player.Move.canceled += ctx => ResetMovement();

		Controls.Player.Sprint.performed += ctx => Sprint();

		Controls.Player.Fire.performed += ctx => Fire();

		Controls.Player.Inventory.performed += ctx => OpenInventory();

		Controls.Player.Inventory.performed += ctx => InventoryMenuWindow.instance.OpenInventory();

		state = MoveState.Walk;
	}

	[ClientCallback]
	private void OnEnable() => Controls.Enable();
	[ClientCallback]
	private void OnDisable() => Controls.Disable();
	[ClientCallback]
	private void Update()
	{

		if (interacting)
			return;

		switch (state)
		{
			case (MoveState.None):
				break;
			case (MoveState.Walk):
				Move(MoveSpeed);
				break;
			case (MoveState.Sprint):
				Move(SprintSpeed);
				break;
			default:
				break;
		}

	}

	[Client]
	private void SetMovement(Vector2 movement) => previousInput = movement;
	[Client]
	private void ResetMovement() => previousInput /= 100;

	[Client]
	private void Sprint()
	{

		isSprint = !isSprint;

		//No longer sprinting - change to walking
		if (!isSprint)
		{
			//clientAnimator.SetBool("Sprint", false);
			state = MoveState.Walk;
			return;
		}
		else
		{
			//clientAnimator.SetBool("Sprint", true);
			state = MoveState.Sprint;
		}
	}

    [Client]
	private void OpenInventory()
    {

    }

    [Client]
    private void Fire()
    {
		clientAnimator.SetTrigger("Attack");

		if (isServer)
			return;

		CommandFire();
    }

    [Command]
	public void CommandFire()
    {
		ServerFire();
    }

    [Server]
	public void ServerFire()
    {
		clientAnimator.SetTrigger("Attack");
	}

    [ClientRpc]
	public void RpcFire()
    {
		if (isOwned)
			return;

		clientAnimator.SetTrigger("Attack");
	}

	[Client]
	private void Move(float targetSpeed)
	{
		if (previousInput.magnitude < 0.1f)
			targetSpeed = 0.0f;

		float currentHorizontalSpeed = previousInput.magnitude;

		float speedOffset = 0.1f;

		// accelerate or decelerate to target speed
		if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
		{
			// creates curved result rather than a linear one giving a more organic speed change
			// note T in Lerp is clamped, so we don't need to clamp our speed
			_speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed, SpeedChangeRate);

			// round speed to 3 decimal places
			_speed = Mathf.Round(_speed * 1000f) / 1000f;
		}
		else
		{
			_speed = targetSpeed;
		}
		// normalise input direction
		Vector3 inputDirection = new Vector3(previousInput.x, previousInput.y, 0f).normalized;

		// move the player
		rb.velocity = _speed * inputDirection;
		clientAnimator.SetFloat("Speed", (inputDirection *_speed).magnitude);

		if (Mathf.Abs(previousInput.y) > Mathf.Abs(previousInput.x))
		{
			if (previousInput.y > 0)
				direction = Direction.Up;
			else
				direction = Direction.Down;
		}
        else
        {
			if (previousInput.x > 0)
				direction = Direction.Right;
			else
				direction = Direction.Left;
		}

		Vector2 animDir = Vector2.zero;
        switch (direction)
        {
			case Direction.Down:
				animDir = Vector2.down;
				break;
			case Direction.Left:
				animDir = Vector2.left;
				break;
			case Direction.Right:
				animDir = Vector2.right;
                break;
			case Direction.Up:
				animDir = Vector2.up;
                break;
			default:
				animDir = Vector2.down;
				break;
		}


        clientAnimator.SetFloat("XInput", animDir.x);
		clientAnimator.SetFloat("YInput", animDir.y);
	}

	[ClientRpc]
	public void RpcSetPositionAndRotation(Vector3 position, Quaternion rotation)
	{
		if (!isOwned)
			return;

		transform.SetPositionAndRotation(position, rotation);
	}

	[ClientRpc]
	public void RpcSetEnabled(bool enabled)
	{
		if (!isOwned)
			return;

		this.enabled = enabled;
	}

    [Server]
    public void ApplyKnockback(Transform position, float force)
    {
		if (Time.time < invicibleTimeoutDelta)
			return;

		Vector2 dir = (transform.position - position.position).normalized;

		invicibleTimeoutDelta = Time.time + invicibleTime;

		RpcApplyKnockback(dir, force);
    }

    [ClientRpc]
	public void RpcApplyKnockback(Vector2 direction, float force)
    {
		if (!isOwned)
			return;

		StartCoroutine(Knockback(direction, force));
    }

	IEnumerator Knockback(Vector2 direction, float force)
    {
		interacting = true;

		rb.velocity = direction * force * force;

		while (rb.velocity.magnitude > 0.5f)
        {
			rb.velocity *= .90f;

			yield return null;
		}

		rb.velocity = Vector2.zero;
		interacting = false;
    }
}
