using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class NapalmInstance : Ability
{
    private HashSet<GameObject> damagableCollisions = new HashSet<GameObject>();
    public List<GameObject> GetDamagableCollisions() => damagableCollisions.Where(x => !x.IsDestroyed() && x.GetComponent<HealthComponent>().health > 0).ToList();

    public override void OnTriggerEnter(Collider collision)
    {
        if (collision != null && collision.tag != "Player" && collision.GetComponent<HealthComponent>() != null)
        {
            damagableCollisions.Add(collision.gameObject);
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (damagableCollisions.Contains(collision.gameObject))
        {
            damagableCollisions.Remove(collision.gameObject);
        }
    }

    private void OnDestroy()
    {
        _napalm.RemoveNapalmInstance(this.gameObject);
        GetComponent<AudioSource>().Stop();
    }

    public void SetNapalm(Napalm napalm) => _napalm = napalm;

    private Napalm _napalm;
}