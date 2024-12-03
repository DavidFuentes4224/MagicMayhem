using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(LootDropComponent), typeof(Rigidbody), typeof(CapsuleCollider))]
public class BreakableObject : MonoBehaviour
{
    private HealthComponent healthComponent;
    [SerializeField] private GameObject unbrokenGameObject;
    [SerializeField] private GameObject brokenGameObject;
    [SerializeField] private float despawnTime;
    [SerializeField] AudioClip breakAudio;
    [SerializeField] float explosionForce = 10.0f;
    AudioSource audioSource;
    private void Awake()
    {
        healthComponent = GetComponent<HealthComponent>();
        audioSource = GetComponent<AudioSource>();
        healthComponent.onDeath += HealthComponent_OnDeath;
    }

    private void OnDestroy()
    {
        healthComponent.onDeath -= HealthComponent_OnDeath;
    }

    private void HealthComponent_OnDeath(object sender, EventArgs e)
    {
        healthComponent.onDeath -= HealthComponent_OnDeath;
        GetComponent<Collider>().enabled = false;
        audioSource.PlayOneShot(breakAudio);
        unbrokenGameObject.SetActive(false);
        brokenGameObject.SetActive(true);
        foreach (var rb in brokenGameObject.GetComponentsInChildren<Rigidbody>())
        {
            rb.AddExplosionForce(explosionForce, gameObject.transform.position + Vector3.up, 5);
        }
        GetComponent<LootDropComponent>().DropLoot();
        StartCoroutine(DespawnCoroutine());
    }


    private IEnumerator DespawnCoroutine()
    {
        yield return new WaitForSeconds(despawnTime);
        Destroy(gameObject);
    }
}