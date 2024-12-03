using UnityEngine;

[CreateAssetMenu(fileName = "New Ability Info", menuName = "Ability Info")]
public class AbilityInfo : ScriptableObject
{
    public string abilityName;
    public string description;
    public AbiltyType abiltyType;
    public float cooldown;
    public float damage;
    public int cost;
    public GameObject abilityPrefab;
    public bool projectileDestoryAfterCollision = true;
    public float despawnTime;
    public Vector3 projectileTargetDirection;
    public Vector3 castingOffset;
    public float projectileTargetSpeed;
    public float projectileSpread = 0.0f;
    public AudioClip castingSound;
    public Sprite icon;
    public AbilityInfo nextLevel;

    // This should be an ID in the future
    public bool IsEqual(AbilityInfo other)
    {
        return abilityName == other.abilityName;
    }
}
