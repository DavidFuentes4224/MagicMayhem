using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
[RequireComponent (typeof(NavMeshAgent))]
public class TutorialSimpleRootMotionNavAgent : RefreshableComponent
{
	[SerializeField] float _lookSpeed = 180.0f;
	[SerializeField] float _movementScale = 1.0f;
	[SerializeField] bool _canSprint = false;
	[SerializeField] [Range(0,1)] float _sprintThreshold = 0.35f;
	[SerializeField] LayerMask _visionLayerMask;

	[Header("Pathing")]
	[SerializeField] float _stoppingDistance = 1.0f;
	[SerializeField] float _cornerThreshold = 0.33f;


	private void Awake()
	{
		_anim = GetComponent<Animator>();
		_agent = GetComponent<NavMeshAgent>();
		_goblin = GetComponent<TutorialGoblin>();

		_anim.applyRootMotion = true;
		_agent.updatePosition = false;
		_agent.updateRotation = false;
	}

	void Update()
	{
		TryFaceWaypoint();
		UpdateAnimParamsFromSteering();


		if (_agent.remainingDistance < _stoppingDistance)
		{
			_anim.SetBool("IsMoving", false);
			_anim.SetBool("IsRunning", false);
		}
	}

	void OnAnimatorMove()
	{
		var rootMotionMovement = _anim.deltaPosition * _movementScale;
		_agent.nextPosition = transform.position + rootMotionMovement;
		
		transform.position = _agent.nextPosition;
	}

	private void UpdateAnimParamsFromSteering()
	{
		var dir = (_agent.steeringTarget - transform.position);
		var animDir = transform.InverseTransformDirection(dir);

		if (animDir.sqrMagnitude > 1)
			animDir = animDir.normalized;

		_anim.SetFloat("VelY", animDir.z);
		_anim.SetFloat("VelX", animDir.x);
		var isRunning = _canSprint && (animDir.z > 0.5f && Mathf.Abs(animDir.x) < _sprintThreshold);
		_anim.SetBool("IsRunning", isRunning);
		_anim.SetBool("IsMoving", true);
	}

	private void TryFaceWaypoint()
	{
		if (_goblin.wayPoints.Length < 1)
		{
			return;
		}

		var lookDir = _goblin.wayPoints[_goblin._currentTargetWaypoint].position - transform.position;
		lookDir.y = transform.position.y;
		Debug.DrawRay(transform.position + Vector3.up, lookDir, Color.yellow);
		if (Physics.Raycast(transform.position + Vector3.up, lookDir, out var hit, _visionDistance, _visionLayerMask))
		{
			transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(lookDir), _lookSpeed * Time.deltaTime);
		}
		else
		{
			var dir = (_agent.steeringTarget - transform.position);
			if (dir.sqrMagnitude > 0.01f)
				transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(dir), _lookSpeed * Time.deltaTime);
		}
	}

	public override void OnInit()
	{
		enabled = true;
	}

	public override void OnKilled()
	{
		enabled = false;
	}

	const float _visionDistance = 20.0f;

	Animator _anim;
	NavMeshAgent _agent;
	TutorialGoblin _goblin;
}
