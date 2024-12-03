using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChainLightning : Ability
{
	public GameObject chainConnector;
	public int chainConnectorDetectionRadius;
	public int chainConnectorTargetCap;
	public float chainConnectorDamageReduction;

	private bool hit;
	private List<Transform> targets;
	private int targetIndex;
	private Transform currentTarget;
	[SerializeField] LayerMask chainConnectorLayerMask;

	public override void Awake()
	{
		hit = false;
		targets = new List<Transform>();
		base.Awake();
	}

	public override void OnTriggerEnter(Collider collision)
	{
		if (!hit && collision != null && !collision.CompareTag("Player") && collision.GetComponent<HealthComponent>() != null)
		{
			hit = true;
			Collider[] chainConnectorRange = Physics.OverlapSphere(collision.transform.position, chainConnectorDetectionRadius, chainConnectorLayerMask);

			foreach (Collider enemy in chainConnectorRange)
			{
				if(enemy.transform != collision.transform)
				{
					targets.Add(enemy.transform);
				}
			}
			if (targets.Count > 0)
			{
				collision.GetComponent<HealthComponent>().TakeDamage(GetAbilityDamage());
				PickTarget(collision);
			}
			else
			{
				OnlyOriginalTarget(collision);
			}
		}
		else if (LayerMaskUtility.GameObjectIsInLayer(collision.gameObject, layerMask))
		{
			Destroy(gameObject);
		}

	}

	public void OnlyOriginalTarget(Collider collision)
	{
		collision.GetComponent<HealthComponent>().TakeDamage(GetAbilityDamage());
		Despawn();
	}

	public void PickTarget(Collider collision)
	{
		var  midpoint = (collision.transform.position + targets[targetIndex].position) / 2;
		var direction = collision.transform.position - targets[targetIndex].position;
		var distance = direction.magnitude;
		
		var chainConnectorInstance = Instantiate(chainConnector, new Vector3(midpoint.x, 1.5f, midpoint.z), Quaternion.LookRotation(direction));
		chainConnectorInstance.transform.localScale = new Vector3(5, 1, distance);

		chainConnectorInstance.GetComponent<ChainConnector>().SetFirstTarget(collision.transform);
		chainConnectorInstance.GetComponent<ChainConnector>().SetSecondTarget(targets[targetIndex]);

		targets[targetIndex].GetComponent<HealthComponent>().TakeDamage(GetAbilityDamage()*chainConnectorDamageReduction);
		currentTarget = targets[targetIndex];
		targetIndex++;

		if (currentTarget != null && hit && targetIndex < chainConnectorTargetCap && targetIndex < targets.Count)
		{
			PickTarget(currentTarget.GetComponent<Collider>());
		}
		else if (hit && targets.Count > 0)
		{
			Despawn();
		}
	}
}
