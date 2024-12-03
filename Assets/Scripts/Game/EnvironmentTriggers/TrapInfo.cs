using UnityEngine;

[CreateAssetMenu(fileName = "New Trap Info", menuName = "Trap Info")]
public class TrapInfo : ScriptableObject
{
    public bool isActivated;
    public bool isSprung;
    public float activeDuration;
    public float damageCooldown;
    public float damage;
    public int trapCost;

    void Awake()
    {
        isActivated = false;
        isSprung = false;
    }
}
