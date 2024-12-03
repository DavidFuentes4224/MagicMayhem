using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityExtensions;

[RequireComponent(typeof(HealthComponent))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(NavPollerComponent))]
public class EnemyBase : MonoBehaviour
{
	[SerializeField] RefreshableComponent[] _additionalRefreshableComponents;
	[SerializeField] AudioClip onHitSound;
	[SerializeField] AudioClip onDeathSound; 

	void Awake()
	{
		_healthComponent = GetComponent<HealthComponent>();
		_healthComponent.onDeath += HealthComp_OnDeath;
		_healthComponent.damageTaken += HealthComp_OnDamageTaken;

		_agent = GetComponent<NavMeshAgent>();
		_navPoller = GetComponent<NavPollerComponent>();

		_audioSource = GetComponent<AudioSource>();

		_refreshableComponents = gameObject.GetAllComponents<RefreshableComponent>();
		if (_additionalRefreshableComponents?.Length > 0)
			_refreshableComponents = _refreshableComponents.Concat(_additionalRefreshableComponents).ToArray();
		_refreshableComponents = _refreshableComponents.Distinct().ToArray();
		DoAwake();
	}

	// Start is called before the first frame update
	void Start()
	{
		DoStart();
		
		// Scale all enemy health by 1.5x per extra player
		if (PlayerManager.instance.PlayerControllers.Count > 1)
		{
			_healthComponent.maxHealth *= ((PlayerManager.instance.PlayerControllers.Count - 1) * 1.5f);
			_healthComponent.health = _healthComponent.maxHealth;
		}
	}

	// Update is called once per frame
	void Update()
	{
		if (_healthComponent.IsAlive == false) return;

		DoUpdate();
	}

	void OnDestroy()
	{
		_healthComponent.onDeath -= HealthComp_OnDeath;
		_healthComponent.damageTaken -= HealthComp_OnDamageTaken;
	}

	protected virtual void DoAwake() { }
	protected virtual void DoStart() { }
	protected virtual void DoUpdate() { }

	public void InitFromPool(Vector3 location, Action<EnemyBase> releaseAction)
	{
		transform.position = location;
		_agent.transform.position = location;

		enabled = true;
		_agent.enabled = false;

		_healthComponent.health = _healthComponent.maxHealth;

		if (_refreshableComponents != null)
		{
			foreach (var comp in _refreshableComponents)
			{
				comp.OnInit();
			}
		}

		StartCoroutine(nameof(SelfOnInit));

		if (_releaseToPoolAction is null)
			_releaseToPoolAction = releaseAction;
	}

	protected virtual IEnumerator SelfOnInit() { yield return null; }
	protected virtual IEnumerator SelfOnKilled() { yield return null; }

	private void HealthComp_OnDeath(object sender, EventArgs e)
	{
		StartCoroutine(nameof(HandleDeath));
	}

	private void HealthComp_OnDamageTaken(object sender, EventArgs e)
	{
		var rand = UnityEngine.Random.Range(1, 100);
		if (rand <= 25 && onHitSound != null && _healthComponent.health > 0)
			_audioSource.PlayOneShot(onHitSound);
	}

	IEnumerator HandleDeath()
	{
		enabled = false;

		WaveManager.instance?.ReportEnemyKilled();

		var rand = UnityEngine.Random.Range(1, 100); ;
		if (transform.gameObject.CompareTag("GolemEnemy"))
			 rand = 19;
		if (rand <= 20 && onDeathSound != null)
			_audioSource.PlayOneShot(onDeathSound);
		

		foreach (var comp in _refreshableComponents)
		{
			comp.OnKilled();
		}
		
		yield return SelfOnKilled();

		yield return new WaitForSeconds(3.0f);
		_releaseToPoolAction?.Invoke(this);
	}

	protected HealthComponent _healthComponent;
	protected NavMeshAgent _agent;
	protected NavPollerComponent _navPoller;
	protected RefreshableComponent[] _refreshableComponents;
	protected AudioSource _audioSource;
	Action<EnemyBase> _releaseToPoolAction;
}
