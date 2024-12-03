using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(RagdollComponent))]
[RequireComponent(typeof(HealthComponent))]
[RequireComponent(typeof(Dissolver))]
[RequireComponent(typeof(LootDropComponent))]
[RequireComponent(typeof(HitVisualizer))]
public class TutorialGoblin : EnemyBase
{
	public Transform[] wayPoints;
	[SerializeField] Renderer _targetRenderer;

	protected override void DoAwake()
	{
		_anim = GetComponent<Animator>();
		_dissolver = GetComponent<Dissolver>();
		_dissolver.SetTargetRenderer(_targetRenderer);
	}

	// Start is called before the first frame update
	protected override void DoStart()
	{
		_spawnPoint = gameObject.transform.position;
		InitFromPool(_spawnPoint, null);
	}

	protected override IEnumerator SelfOnInit()
	{
		gameObject.transform.position = _spawnPoint;
		gameObject.transform.rotation = Quaternion.identity;
		_currentTargetWaypoint = 0;
		gameObject.SetActive(true);
		StartCoroutine(nameof(GoToWayPoint));
		yield return null;
	}

	IEnumerator GoToWayPoint()
	{
		if (wayPoints.Length == 0)
			yield break;

		var target = wayPoints[_currentTargetWaypoint];
		_agent.destination = target.position;
		yield return new WaitUntil(() => (target.position - transform.position).magnitude < 1.5f);
		_currentTargetWaypoint = ++_currentTargetWaypoint % wayPoints.Length;
		yield return GoToWayPoint();
	}

	protected override IEnumerator SelfOnKilled()
	{
		StopCoroutine(nameof(GoToWayPoint));

		yield return new WaitForSeconds(5.0f);
		InitFromPool(_spawnPoint, null);
	}

	Animator _anim;
	RagdollComponent _ragdoll;
	Dissolver _dissolver;

	public int _currentTargetWaypoint = 0;
	Vector3 _spawnPoint;
}
