using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPooler : MonoBehaviour
{
 
    [SerializeField] private List<GameObject> enemyPreFabs;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private Transform[] enemyParentObjects;
    [SerializeField] private int maxPoolSize;
    private static ObjectPool<GameObject> enemyPool;
    private Dictionary<string, Transform> enemyParentObjectDictionary;
    public static Dictionary<string, ObjectPool<GameObject>> enemyPools;

    // https://docs.unity3d.com/ScriptReference/Pool.ObjectPool_1.html

    //In Inspector, must match indexes of enemyPreFabs and enemyParentObjects for each enemy
    private void Start()
    {
        enemyPools = new Dictionary<string, ObjectPool<GameObject>>();
        enemyParentObjectDictionary = new Dictionary<string, Transform>();
        foreach (var enemyPreFab in enemyPreFabs)
        {
            enemyPools.Add(enemyPreFab.tag, CreateObjectPoolList(enemyPreFab));
            enemyParentObjectDictionary.Add(enemyPreFab.tag, enemyParentObjects[enemyPreFabs.IndexOf(enemyPreFab)]);
        } 
    }

    private ObjectPool<GameObject> CreateObjectPoolList(GameObject enemyPreFab)
    {
        enemyPool = new ObjectPool<GameObject>(
            createFunc: () => CreateObjectPool(enemyPreFab),
            actionOnGet: OnGetObjectFromPool,
            actionOnRelease: OnReturnObjectToPool, 
            actionOnDestroy: OnDestroyObject, 
            collectionCheck: true, 
            defaultCapacity: 10, 
            maxSize: maxPoolSize);
        return enemyPool;
    }

    private GameObject CreateObjectPool(GameObject enemyPreFab)
    {
       GameObject enemyInstance = Instantiate(enemyPreFab, Vector3.zero, Quaternion.identity, parent:null);
       return enemyInstance;
    }

    private void OnGetObjectFromPool(GameObject enemyPreFab)
    {
        enemyPreFab.SetActive(true);
    }

    private void OnReturnObjectToPool(GameObject enemyPreFab)
    {
        enemyPreFab.SetActive(false);
    }

    private void OnDestroyObject(GameObject enemyPreFab)
    {
        Destroy(enemyPreFab);
    }

    public void SpawnEnemy(string enemyTag)
    {
        var enemy = enemyPools[enemyTag].Get();
		Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
		enemy.transform.position = randomSpawnPoint.position;
    }

    public void EnemyPoolRelease(string enemyTag, GameObject enemyPreFab)
    {
        enemyPools[enemyTag].Release(enemyPreFab);
    }
}
