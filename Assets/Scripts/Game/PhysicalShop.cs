using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalShop : MonoBehaviour
{
    [SerializeField] Transform[] _shopSpawnLocations;
    public Transform[] ShopSpawnLocations => _shopSpawnLocations;
}
