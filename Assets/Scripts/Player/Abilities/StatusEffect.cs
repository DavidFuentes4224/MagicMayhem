using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatusEffectValueType
{
    Flat,
    Percentage
}

public enum StatusEffectStat
{
    Health,
    Damage,
    HealthRegenPerSecond,
    Armor,
    DodgeChance,
    Shield,
    Invulnerable,
}

[CreateAssetMenu(fileName = "New Status Effect", menuName = "Status Effect")]
public class StatusEffect : ScriptableObject
{
    public string statusEffectName;
    public string statusEffectDescription;
    public StatusEffectStat stat;
    public StatusEffectValueType valueType = StatusEffectValueType.Flat;
    public float value;
    public float duration;
}
