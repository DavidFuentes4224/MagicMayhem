using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BellTower : MonoBehaviour
{
    public bool isActivatable;
    // Start is called before the first frame update
    void Start()
    {
        if (WaveManager.instance is null) return;
        isActivatable = true;
        gameObject.GetComponent<BoxCollider>().enabled = true;
        gameObject.transform.GetChild(1).gameObject.SetActive(true);
        WaveManager.instance.waveStarted += WaveManager_WaveStarted;
        WaveManager.instance.waveFinished += WaveManager_WaveFinished;
    }

    private void OnDestroy()
    {
        WaveManager.instance.waveStarted -= WaveManager_WaveStarted;
        WaveManager.instance.waveFinished -= WaveManager_WaveFinished;
    }

    private void WaveManager_WaveStarted(object sender, WaveStartedEventArgs e)
    {
        isActivatable = false;
        gameObject.GetComponent<BoxCollider>().enabled = false;
        gameObject.transform.GetChild(1).gameObject.SetActive(false);
    }

    private void WaveManager_WaveFinished(object sender, WaveEndedEventArgs e)
    {
        isActivatable = true;
        gameObject.GetComponent<BoxCollider>().enabled = true;
        gameObject.transform.GetChild(1).gameObject.SetActive(true);
    }
}
