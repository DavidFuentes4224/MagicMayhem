using UnityEngine;
public class WaveInfo : MonoBehaviour
{
    [System.Serializable]
    public class WaveContent
    {
        [SerializeField] public GroupContent[] groups;
        [SerializeField] public int timeToNextWave;
        [SerializeField] public int timeBetweenEnemies;

        public GroupContent[] GetGroups()
        {
            return groups;
        }
        public float GetTimeToNextWave()
        {
            return timeToNextWave;
        }
        public float GetTimeBetweenEnemies()
        {
            return timeBetweenEnemies;
        }
        
        public int GetEnemyCount()
        {
            var count = 0;
            foreach (var group in groups)
                count += group.GetEnemyCount();

            return count;
        }
    }

    [SerializeField] WaveContent[] waves;

    public WaveContent[] GetWaveContents()
    {
        return waves;
    }
    
}

[System.Serializable]
public class GroupContent
{
	[SerializeField] public EnemyContent[] enemySpawn;
	[SerializeField] public float timeToNextGroup;

	public EnemyContent[] GetEnemySpawn()
	{
		return enemySpawn;
	}
	public float GetTimeToNextGroup()
	{
		return timeToNextGroup;
	}

    public int GetEnemyCount()
    {
        var count = 0;
        foreach(var enemy in enemySpawn)
            count += enemy.amountToSpawn;
        return count;
    }
}

[System.Serializable]
public class EnemyContent
{
	public enum EnemyToSpawn
	{
		Goblin,
		Golem,
		Test,
		WarChief,
        Archer
	};

	[SerializeField] public EnemyToSpawn enemyToSpawn;
	[SerializeField] public int amountToSpawn;

	public int GetAmountToSpawn()
	{
		return amountToSpawn;
	}
}