using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    private void Start()
    {
		PlayerPrefs.SetInt("RunsPlayed", PlayerPrefs.GetInt("RunsPlayed", 0) + 1);
	}
}
