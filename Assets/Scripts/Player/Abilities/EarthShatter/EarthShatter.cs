using System.Collections;
using UnityEngine;

public class EarthShatter : Ability
{
    [SerializeField] GameObject movementColliders;
    [SerializeField] CapsuleCollider damageCollider;

    [Tooltip("Length of time in which Damage can occur after Awake.")]
    [SerializeField] float disableDamageColliderTime;

    [Tooltip("Length of time in which movement collision will be enabled after Awake.")]
    [SerializeField] float enableMovementColliderTime;

    public override void Awake()
    {
        StartCoroutine(EnableMovementCoroutine());
        StartCoroutine(DisableDamageCoroutine());
        transform.position += transform.forward * 2;
        base.Awake();
    }

    private IEnumerator EnableMovementCoroutine()
    {
        yield return new WaitForSeconds(enableMovementColliderTime);
        movementColliders.SetActive(true);
        // TODO fix issue with landing on top of collider
    }

    private IEnumerator DisableDamageCoroutine()
    {
        yield return new WaitForSeconds(disableDamageColliderTime);
        damageCollider.enabled = false;
    }
}