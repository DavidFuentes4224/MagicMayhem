using UnityEngine;

public class ArrowProjectile : MonoBehaviour
{
    [SerializeField] float _damage;
	[SerializeField] LayerMask _layerMask;

	private void OnTriggerEnter(Collider other)
	{
		if (LayerMaskUtility.GameObjectIsInLayer(other.gameObject, _layerMask))
		{
			if (other.gameObject.tag == "Player")
			{
				var hc = other.gameObject.GetComponentInParent<HealthComponent>();
				if (hc)
					hc.TakeDamage(_damage);
			}

			Destroy(gameObject);
		}
	}
}
