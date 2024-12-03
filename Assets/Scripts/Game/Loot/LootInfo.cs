using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Loot Info", menuName = "Loot Info")]
public class LootInfo : ScriptableObject
{
    public GameObject lootGameObject;
    public int amount = 1;
}
