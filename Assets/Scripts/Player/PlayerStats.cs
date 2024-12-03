using System.Collections;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(StatusEffects))]
public class PlayerStats : HealthComponent
{
    private float damage;
    private float healthRegenPerSecond;
    private float armor;
    private float dodgeChance;
    private bool canPlayHitSound;
    public int gold;

    public float GetAbilityDamage(float abilityDamage = 0) => GetStatWithStatusEffects(damage + abilityDamage, StatusEffectStat.Damage);
    public float HealthRegenPerSecond => GetStatWithStatusEffects(healthRegenPerSecond, StatusEffectStat.HealthRegenPerSecond);
    public float Armor => GetStatWithStatusEffects(armor, StatusEffectStat.Armor);
    public float DodgeChance => GetStatWithStatusEffects(dodgeChance, StatusEffectStat.DodgeChance);
    public bool IsShielded => statusEffects.currentStatusEffects.Any(x => x.stat == StatusEffectStat.Shield);
    public bool IsInvulnerable => statusEffects.currentStatusEffects.Any(x => x.stat == StatusEffectStat.Invulnerable);
    public override void Awake()
    {
        statusEffects = GetComponent<StatusEffects>();
        _audioSource = GetComponentInChildren<AudioSource>();
        canPlayHitSound = true;
        base.Awake();
    }

    private void Update()
    {
        health += HealthRegenPerSecond * Time.deltaTime;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
    }

    public override void TakeDamage(float damage) 
    {
        if (IsShielded)
        {
            OnHitSound();
            return;
        }
        var randomDodgeChance = Random.Range(0f, 100f);
        if (randomDodgeChance < DodgeChance)
        {
            return;
        }
        var armorReducedDamage = damage * GetPercentDamageTakenWithArmor(Armor);
        if (IsInvulnerable)
        {
            armorReducedDamage = 0;
        }
        base.TakeDamage(armorReducedDamage);
        OnHitSound();
    }

    public float GetPercentDamageTakenWithArmor(float armor)
    {
        return 1 / (1 + (armor / 15));
    }

    public float GetStatWithStatusEffects(float baseValue, StatusEffectStat statusEffectStat)
    {
        var currentStatusEffectsForStat = statusEffects.currentStatusEffects.Where(x => x.stat == statusEffectStat);
        var flatIncreasedStatValue = currentStatusEffectsForStat.Where(x => x.valueType == StatusEffectValueType.Flat).Sum(x => x.value);
        var perctangeIncreasedStatValue = 1 + currentStatusEffectsForStat.Where(x => x.valueType == StatusEffectValueType.Percentage).Sum(x => x.value);
        var statValue = (baseValue + flatIncreasedStatValue) * perctangeIncreasedStatValue;
        return Mathf.Max(statValue, 0);
    }

    private void OnHitSound()
    {
        if (canPlayHitSound)
        {
            if (IsShielded || IsInvulnerable)
            {
                _audioSource.PlayOneShot(onShieldHit);
            }
            else
            {
                _audioSource.PlayOneShot(onHit);
            }
            canPlayHitSound = false;
            StartCoroutine(OnHitSoundCooldown());
        }
    }

    IEnumerator OnHitSoundCooldown()
    {
        yield return new WaitForSeconds(0.25f);
        canPlayHitSound = true;
    }

    public StatusEffects StatusEffects => statusEffects;
    [SerializeField] StatusEffects statusEffects;
    [SerializeField] AudioClip onHit;
    [SerializeField] AudioClip onShieldHit;
    AudioSource _audioSource;
}
