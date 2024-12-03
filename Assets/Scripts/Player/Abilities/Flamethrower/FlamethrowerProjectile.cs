using UnityEngine;

public class FlamethrowerProjectile : Ability
{
    public Vector3 castingDirection;
    public Vector3 randomDeviationDirection;

    public override void Awake()
    {
        randomDeviationDirection = new Vector3(0, 0, Random.Range(-0.3f, 0.3f));
        base.Awake();
    }

    public override void UpdateProjectileVelocity()
    {
        var direction = (castingDirection + randomDeviationDirection).normalized;
        transform.position += direction * abilityInfo.projectileTargetSpeed * Time.deltaTime;
    }
}
