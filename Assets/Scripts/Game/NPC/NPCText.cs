using TMPro;
using UnityEngine;

public class NPCText : MonoBehaviour
{
    public TextMeshProUGUI infoText;

    private int plainsLevelWaveStart;
    private int goblinsLevelWaveStart;
    private int waveCounter = 1;

    // Start is called before the first frame update
    void Start()
    {
        infoText.text = firstStage;
        if (WaveManager.instance is null) return;
        WaveManager.instance.waveFinished += WaveManager_WaveFinished;
        plainsLevelWaveStart = WaveManager.instance.startPlainsLevel;
        goblinsLevelWaveStart = WaveManager.instance.startGoblinLevel;
    }

    private void OnDestroy()
    {
        if (WaveManager.instance is null) return;
        WaveManager.instance.waveFinished -= WaveManager_WaveFinished;
    }

    private void WaveManager_WaveFinished(object sender, WaveEndedEventArgs e)
    {
        waveCounter++;
        if (waveCounter == plainsLevelWaveStart || waveCounter == goblinsLevelWaveStart)
        {
            if (waveCounter == plainsLevelWaveStart)
            {
                UpdateText(secondStage);
            }
            else
            {
                UpdateText(thirdStage);
            }
        }
    }

    public void UpdateText(string newText)
    {
        infoText.text = newText;
    }

    private string firstStage = @"Traveler, the goblins are coming!

Survive their onslaught for ten days, and use the time in between to buy more powerful spells from the shop! There are vases that contain gold, barrels with health potions and spike traps scattered around the map to aid you.

Are you ready wield magic and ensure your survival!";

    private string secondStage = @"It looks like we are close to their encampment! If we can hold off for a couple more days we can push the fight to them!

Also did you know you can upgrade your spells? It will definitely come in handy for the fights ahead";

    private string thirdStage = @"We made it to their base traveller!

Their numbers grow thin. If we can defeat them here the town will be saved! I found this trap over here that I think you can activate! I heard it does a ton of damage!!!";
}
