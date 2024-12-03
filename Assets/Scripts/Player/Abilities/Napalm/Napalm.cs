using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Napalm : Ability
{
    [SerializeField] float timeBetweenInstanceSpawns;
    [SerializeField] GameObject napalmInstanceGameObject;
    [SerializeField] float spawnDuration;
    [SerializeField] float damageIntervalTime;

    private float remainingSpawnDuration;
    private HashSet<GameObject> napalmInstances = new HashSet<GameObject>();

    public override void Awake()
    {
        remainingDespawnTime = spawnDuration + abilityInfo.despawnTime;
        remainingSpawnDuration = spawnDuration;
    }

    public override void Start()
    {
        StartCoroutine(SpawnInstanceOfNapalmCoroutine());
        StartCoroutine(DealDamageCoroutine());
    }

    public override void Update()
    {
        remainingSpawnDuration -= Time.deltaTime;
        base.Update();
    }

    public override void UpdateProjectileVelocity()
    {
        if (_player != null)
        {
            transform.position = _player.transform.position;
        }
    }

    private IEnumerator SpawnInstanceOfNapalmCoroutine()
    {
        if (remainingSpawnDuration > 0)
        {
            var napalmInstance = Instantiate(napalmInstanceGameObject, _player.GetAvatarPosition(), Quaternion.identity);
            napalmInstance.GetComponent<NapalmInstance>().SetPlayer(_player);
            napalmInstance.GetComponent<NapalmInstance>().SetNapalm(this);
            napalmInstances.Add(napalmInstance);
            yield return new WaitForSeconds(timeBetweenInstanceSpawns);
            StartCoroutine(SpawnInstanceOfNapalmCoroutine());
        }
    }

    private IEnumerator DealDamageCoroutine()
    {
        yield return new WaitForSeconds(damageIntervalTime);
        DealDamageAcrossAllNapalmInstances();
        StartCoroutine(DealDamageCoroutine());
    }

    private void DealDamageAcrossAllNapalmInstances()
    {
        var alreadyDamagedEntities = new HashSet<GameObject>();
        foreach (var napalmInstanceGameObject in napalmInstances)
        {
            if (napalmInstanceGameObject.IsDestroyed()) return;

            var napalmInstance = napalmInstanceGameObject.GetComponent<NapalmInstance>();
            foreach(var damagableEntity in napalmInstance.GetDamagableCollisions())
            {
                if (!alreadyDamagedEntities.Contains(damagableEntity))
                {
                    damagableEntity.GetComponent<HealthComponent>().TakeDamage(GetAbilityDamage());
                    alreadyDamagedEntities.Add(damagableEntity);
                }
            }
        }
    }

    public void RemoveNapalmInstance(GameObject napalmInstance)
    {
        napalmInstances.Remove(napalmInstance);
    }
}