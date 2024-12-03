using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MagicMissileProjectile : Ability
{
	[SerializeField] GameObject _hitPrefab;

	protected override void OnHit(Collider collision)
	{
		base.OnHit(collision);
		Instantiate(_hitPrefab, transform.position, Quaternion.identity);
	}
}
