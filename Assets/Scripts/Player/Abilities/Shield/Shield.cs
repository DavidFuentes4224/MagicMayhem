using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Shield : Ability
{
    [SerializeField] StatusEffect buffStatusEffect;
    [SerializeField] AudioSource audioSource;

    public override void Awake()
    {
        StartCoroutine(AudioFadeOut.FadeOut(audioSource, abilityInfo.despawnTime));
        base.Awake();
    }

    public override void Start()
    {
        if (_player != null)
        {
            _player.PlayerStats.StatusEffects.AddStatusEffect(buffStatusEffect);
        }
    }

    public override void OnTriggerEnter(Collider collision) { }

    public override void UpdateProjectileVelocity()
    {
        transform.position = _player.GetAvatarPosition();
    }
}
