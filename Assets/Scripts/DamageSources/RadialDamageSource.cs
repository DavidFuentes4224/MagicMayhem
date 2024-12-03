using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class RadialDamageSource : MonoBehaviour
{
    
    [SerializeField] RadialDamageInfo _damageInfo;

    public HashSet<GameObject> objectsWithinRadius;

	private void Awake()
	{
		_objectsDeltDamage = new HashSet<GameObject>();
	}
	// Start is called before the first frame update
	void Start()
    {
        transform.localScale = _damageInfo.initialSize;
        _radius = Mathf.Max(_damageInfo.finalSize.magnitude / 2, 1);
	}

    // Update is called once per frame
    void Update()
    {
		transform.localScale = Vector3.MoveTowards(transform.localScale, _damageInfo.finalSize, _damageInfo.expansionSpeed * Time.deltaTime);
        if ((transform.localScale - _damageInfo.finalSize).sqrMagnitude < 0.1f && !_finishedExpanding)
        {
            _finishedExpanding = true;
            StartCoroutine(nameof(FinishExpanding));
        }
    }

	public void OnTriggerEnter(Collider other)
	{
        // If not kinematic, apply explosion force
        if (!other.attachedRigidbody?.isKinematic ?? false)
            other.attachedRigidbody.AddExplosionForce(_damageInfo.forceStrength, transform.position, transform.localScale.magnitude, 2.0f);

        VerifyCollision(other.gameObject);
	}

	public void OnTriggerStay(Collider other)
	{
		VerifyCollision(other.gameObject);
	}

    private void VerifyCollision(GameObject other)
    {
		if (LayerMaskUtility.GameObjectIsInLayer(other, _damageInfo.objectsToTrack) == false) return;
		if (_objectsDeltDamage.Contains(other)) return;

		_objectsDeltDamage.Add(other);
		var hc = other.GetComponentInParent<HealthComponent>();
		if (hc)
        {
            var baseDamage = _damageInfo.damage;
            var distance = Vector3.Distance(other.transform.position, transform.position);
            var multiplier = Mathf.Max(1 - (distance / _radius), 0);
			hc.TakeDamage((int) (baseDamage * multiplier));
        }
	}

	IEnumerator FinishExpanding()
    {
        if (_damageInfo.timeBeforeDestory > 0)
        {
            yield return new WaitForSeconds(_damageInfo.timeBeforeDestory);
            Destroy(gameObject);
        }
        else
        {
            yield return new WaitForEndOfFrame();
            Destroy(gameObject);
        }
    }

    bool _finishedExpanding;
    HashSet<GameObject> _objectsDeltDamage;
    Vector3 _scale;
    float _radius;
}
