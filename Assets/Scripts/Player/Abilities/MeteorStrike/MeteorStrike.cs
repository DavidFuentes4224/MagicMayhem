using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorStrike : Ability
{
    [SerializeField] AudioSource audioSource;
    private bool isAudioPaused;
    private Coroutine audioCoroutine;
    private float timeToEnableSpellCollider = 0.4f;
    private float timeToDisableSpellCollider = 0.1f;

    public override void Awake()
    {
        audioCoroutine = StartCoroutine(AudioFadeOut.FadeOut(audioSource, abilityInfo.despawnTime));
        StartCoroutine(EnableHitBox());
        base.Awake();
    }

    public override void UpdateProjectileVelocity() { }

    IEnumerator EnableHitBox()
    {
        yield return new WaitForSeconds(timeToEnableSpellCollider);
        transform.GetComponent<SphereCollider>().enabled = true;
        yield return new WaitForSeconds(timeToDisableSpellCollider);
        transform.GetComponentInChildren<SphereCollider>().enabled = false;
    }

    public override void Update()
    {
        if (Time.deltaTime == 0)
        {
            audioSource.Pause();
            StopCoroutine(audioCoroutine);
            isAudioPaused = true;
        }
        else
        {
            if (isAudioPaused == true)
            {
                audioSource.UnPause();
                audioCoroutine = StartCoroutine(AudioFadeOut.FadeOut(audioSource, remainingDespawnTime));
                isAudioPaused = false;
            }
        }
        base.Update();
    }

    public override void OnTriggerEnter(Collider collision)
    {
        if (collision != null && collision.tag != "Player" && collision.GetComponent<HealthComponent>() != null)
        {
            collision.GetComponent<HealthComponent>().TakeDamage(GetAbilityDamage());
        }
    }
}
