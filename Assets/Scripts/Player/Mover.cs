using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Animations.Rigging;

[RequireComponent(typeof(Animator), typeof(CharacterController))]
public class Mover : MonoBehaviour
{
	[Header("Speed Settings")]
	[SerializeField] float _maxPlayerSpeed = 4.8f;
	[SerializeField] float _rotationSpeedDegrees = 180.0f;
	[SerializeField] float _animSpeed = 1.0f;
	[SerializeField] float _rollCooldown = 1.5f;
	[SerializeField] float _rootOverride = 1.0f;
	[SerializeField] float _aimRootOverride = 1.0f;
	[SerializeField] float _rollRootOverride = 1.0f;

	public TextMeshProUGUI playerVelocityCounter;

	public bool isRolling => !_canRoll;

	void Awake()
	{
		_anim = GetComponent<Animator>();
		_characterController = GetComponent<CharacterController>();
		_anim.applyRootMotion = true;
		_rigBuilder = GetComponent<RigBuilder>();
		_upperBodyIndex = _anim.GetLayerIndex("UpperBody");
		_canRoll = true;
	}

	private void Start()
	{
	}

	void Update()
	{
		_currentMovement = _targetMovement;
		UpdateGravity();
		UpdateAnimParams();
		UpdateRotation();
	}

	void FixedUpdate()
	{
		HandleRoll();
	}

	void OnAnimatorMove()
	{
		Animator anim = GetComponentInChildren<Animator>();
		if (!anim)
			return;

		var targetMovement = _anim.deltaPosition;
		if (_isRolling)
			targetMovement *= _rollRootOverride;
		else if (_isAiming)
			targetMovement *= _aimRootOverride;
		else
			targetMovement *= _rollRootOverride;

		if (Time.deltaTime > 0)
		{
			float currentSpeed = targetMovement.magnitude / Time.deltaTime;
			if (!_isRolling && currentSpeed > _maxPlayerSpeed)
				targetMovement *= _maxPlayerSpeed / currentSpeed;
		}
		
		targetMovement.y = _gravitySpeed * Time.deltaTime;

		_characterController.Move(targetMovement);
	}

	public void SetMovement(Vector3 movement) => _targetMovement = movement.sqrMagnitude > 1 ? movement.normalized : movement;
	
	public void SetAiming(bool aiming, Vector2 aimDir, bool useMouse)
	{
		if (_isAiming != aiming && !aiming && _currentMovement.sqrMagnitude > 0.1f) // If we transtion from aiming to not aiming, rotate player to match input
		{
			var targetRot = Quaternion.LookRotation(_currentMovement, Vector3.up);
			var angleBetween = Vector3.Angle(transform.forward, _currentMovement);
			transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, angleBetween * 0.5f);
		}
		_anim.SetBool("IsAiming", aiming);
		_isAiming = aiming;
		_useMouse = useMouse;

		_aimDirection = aimDir;
		if (aimDir != Vector2.zero)
			_prevAimDirection = aimDir;
	}

	public void SetFollowCursor()
	{
		_anim.SetBool("IsAiming", true);
		_isAiming = true;
		_useMouse = true;
	}
		
	public void SetPlayer(Player player) => _player = player;

	public void OnRoll()
	{
		_shouldRoll = true;
	}

	public void OnRollingFinished()
	{
		foreach (var layer in _rigBuilder.layers)
			layer.active = true;

		_isRolling = false;
		_anim.SetLayerWeight(_upperBodyIndex, 1);
	}

	void UpdateGravity()
	{
		if (_characterController.isGrounded)
			_gravitySpeed = 0;
		else
			_gravitySpeed += Physics.gravity.y * Time.deltaTime;
	}

	void UpdateAnimParams()
	{
		var inputAmount = _currentMovement.magnitude;
		var targetForward = 0.0f;
		var targetRight = 0.0f;

		_isMoving = Mathf.Abs(inputAmount) > 0.1;
		if (_useMouse)
			_aimDirection = GetMouseDirection();

		if (_isAiming) // Player maintains rotation while moving
		{
			if (_isMoving)
			{
				var localInput = transform.InverseTransformDirection(_currentMovement);
				targetForward = localInput.z;
				targetRight =  localInput.x;
			}
			else // Player rotates to aim position (mouse)
			{
				var dirToMouse3D = new Vector3(_aimDirection.x, 0, _aimDirection.y);
				var turnDirection = Vector3.Cross(transform.forward, dirToMouse3D).y;
				
				// Cross product can produce a vector of mag 0 if vectors are parallel.
				// If this happens, fall back to dot product to see if they are aligned or opposed.
				if (turnDirection == float.NaN)
					turnDirection = Vector3.Dot(transform.forward, dirToMouse3D) > 0 ? 0.0f : 1.0f;

				targetRight = turnDirection;
			}
		}
		else // Player rotates to face movement direction
		{
			targetForward = inputAmount;
		}

		_anim.SetFloat("Forward", targetForward);

		_anim.SetFloat("Right", targetRight);

		_anim.SetBool("IsMoving", _isMoving);

		_anim.speed = _animSpeed;
	}

	void UpdateRotation()
	{
		if (_isAiming && !_isRolling)
		{
			var aimDir = _useMouse ? GetMouseDirection() : _aimDirection;
			var targetDir = new Vector3(aimDir.x, 0, aimDir.y);
			var lookRotation = Quaternion.LookRotation(targetDir, Vector3.up);
			transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, _rotationSpeedDegrees * Time.deltaTime);
		}
		else // Rotate to face direction of movement
		{
			var lookRotation = _currentMovement == Vector3.zero 
				? transform.rotation 
				: Quaternion.LookRotation(_currentMovement, Vector3.up);
			transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, _rotationSpeedDegrees * Time.deltaTime);
		}
	}

	void HandleRoll()
	{
		if (_shouldRoll && _canRoll)
		{
			_canRoll = false;
			_shouldRoll = false;

			transform.rotation = Quaternion.LookRotation(_currentMovement);
			
			_anim.SetTrigger("Roll");
			_isRolling = true;

			// Disable all rig builder layers while rolling
			foreach (var layer in _rigBuilder.layers)
				layer.active = false;

			_anim.SetLayerWeight(_upperBodyIndex, 0);
			StartCoroutine(nameof(StartRollCooldown));
		}
		else
		{
			_shouldRoll = false;
		}
	}

	/// <summary>
	/// Returns normalized Vector2 pointing from Player to mouse cursor.
	/// </summary>
	Vector2 GetMouseDirection()
	{
		var cursorPos = Input.mousePosition;
		var playerPos = Camera.main.WorldToScreenPoint(gameObject.transform.position);
		return (cursorPos - playerPos).normalized;
	}

	public Vector2 GetAimDirection() => _useMouse ? GetMouseDirection() : _prevAimDirection.normalized;

	IEnumerator StartRollCooldown()
	{
		yield return new WaitForSeconds(_rollCooldown);
		_canRoll = true;
	}

	Animator _anim;
	CharacterController _characterController;
	Player _player;
	RigBuilder _rigBuilder;

	int _upperBodyIndex;

	Vector3 _currentMovement;
	Vector3 _targetMovement;
	Vector2 _aimDirection;
	Vector2 _prevAimDirection;
	bool _isAiming;
	bool _isMoving;
	bool _useMouse;
	bool _shouldRoll;
	bool _canRoll;
	bool _isRolling;
	float _gravitySpeed;
}
