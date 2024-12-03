using UnityEngine;

[CreateAssetMenu(fileName = "DistancePoll", menuName = "AI Distance Poll Info")]
public class DistancePollObject : ScriptableObject
{
    public float PollInterval;
    public float DistanceSqThreshold;
}
