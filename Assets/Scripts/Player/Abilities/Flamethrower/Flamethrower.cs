using UnityEngine;

public class Flamethrower : Ability
{
    public GameObject flamethrowerProjectile;

    private int numberOfFlameProjectiles = 10;
    private float timeBetweenProjectiles = 0.2f;
    private float currentTimeBetweenProjectiles = 0f;

    public override void Awake()
    {
        remainingDespawnTime = abilityInfo.despawnTime + (timeBetweenProjectiles * numberOfFlameProjectiles);
        SetDirectionToPlayerAimDirection();
    }

    public override void Update()
    {
        if (numberOfFlameProjectiles > 0)
        {
            if (currentTimeBetweenProjectiles <= 0)
            {
                currentTimeBetweenProjectiles = timeBetweenProjectiles;
                CastFlamethrowerProjectile();
            }
            else
            {
                currentTimeBetweenProjectiles -= Time.deltaTime;
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void CastFlamethrowerProjectile()
    {
        numberOfFlameProjectiles -= 1;
        var direction = SetDirectionToPlayerAimDirection();
        var projectile = Instantiate(flamethrowerProjectile, _player.GetAvatarPosition(), Quaternion.identity);
        projectile.GetComponent<FlamethrowerProjectile>().SetPlayer(_player);
        projectile.GetComponent<FlamethrowerProjectile>().castingDirection = direction;
    }
}
