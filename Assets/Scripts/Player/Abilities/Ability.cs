using UnityEngine;

public class Ability : MonoBehaviour
{
	[SerializeField] protected LayerMask layerMask;
	public AbilityInfo abilityInfo;
    protected float remainingDespawnTime;
    protected Vector3 aimDirection;

    public virtual void Awake()
    {
        remainingDespawnTime = abilityInfo.despawnTime;
        DoAwake();
    }

    public virtual void Start()
    {
        SetDirectionToPlayerAimDirection();
        DoStart();
    }

    public virtual void Update()
    {
        UpdateProjectileVelocity();
        DoUpdate();
        remainingDespawnTime -= Time.deltaTime;
        if (remainingDespawnTime <= 0)
        {
            Despawn();
        }
    }

    protected virtual void DoAwake() { }
    protected virtual void DoStart() { }
    protected virtual void DoUpdate() { }

    public virtual void OnTriggerEnter(Collider collision)
    {
        if (collision != null && collision.tag != "Player" && collision.GetComponent<HealthComponent>() != null)
        {
            collision.GetComponent<HealthComponent>().TakeDamage(GetAbilityDamage());
            if (abilityInfo.projectileDestoryAfterCollision)
            {
                Despawn();
            }
            OnHit(collision);
        }
		else if (LayerMaskUtility.GameObjectIsInLayer(collision.gameObject, layerMask))
		{
			Destroy(gameObject);
            OnHit(collision);
		}
	}

    protected virtual void OnHit(Collider collision) { }

    public virtual float GetAbilityDamage()
    {
        if (_player != null)
        {
            return _player.GetComponent<PlayerStats>().GetAbilityDamage(abilityInfo.damage);
        }
        return abilityInfo.damage;
    }

    public virtual void Despawn()
    {
        StopAllCoroutines();
        Destroy(gameObject);
    }

    public virtual Vector3 SetDirectionToPlayerAimDirection()
    {
        if (_player != null)
        {
            var playerAimDirection = _player.GetAimDirection();
            aimDirection = new Vector3(playerAimDirection.x, 0, playerAimDirection.y);
            return aimDirection;
        }
        return Vector3.zero;
    }

    public virtual void UpdateProjectileVelocity()
    {
		transform.position += transform.forward * abilityInfo.projectileTargetSpeed * Time.deltaTime;
	}

	public virtual void SetPlayer(Player player) => _player = player;

    protected Player _player;
}
