using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NPCMenu : MonoBehaviour
{
    [Header("UIElements")]
    [SerializeField] Button _closeDialogButton;
    GameObject currentUI;

    void Start()
    {
        if (WaveManager.instance is null) return;
        WaveManager.instance.gameStarting += WaveManager_GameStarted;
        WaveManager.instance.waveStarted += WaveManager_WaveStarted;
    }

    private void OnDestroy()
    {
        WaveManager.instance.gameStarting -= WaveManager_GameStarted;
        WaveManager.instance.waveStarted -= WaveManager_WaveStarted;
    }

    private void WaveManager_GameStarted(object sender, GameStartedEventArgs e)
    {
        gameObject.SetActive(true);
    }

    private void WaveManager_WaveStarted(object sender, WaveStartedEventArgs e)
    {
        gameObject.SetActive(false);
    }

    private void ExitNPC()
    {
		PlayerManager.instance.EnableAllMovement();
        PlayerManager.instance.ClearAllUIRoots();
		Time.timeScale = 1;
    }

    private void EnterNPC(PlayerController callingController)
    {
		var eventSystem = PlayerManager.instance.GetEventSystemForController(callingController);
        eventSystem.playerRoot = gameObject;
		eventSystem.SetSelectedGameObject(callingController.usingMK ? null : _closeDialogButton.gameObject);

        var inputModule = PlayerManager.instance.GetInputModuleForController(callingController);
        inputModule.enabled = true;

        PlayerManager.instance.DisableAllMovement();

		Time.timeScale = 0;
    }

    public void ToggleNPCUI(bool isEnabled, PlayerController callingController, NPCTrigger npcTrigger)
    {
        gameObject?.SetActive(isEnabled);
        if (isEnabled && callingController is object && npcTrigger != null)
        {
            var npc = npcTrigger.GetComponentInParent<NPC>();
            if (npc != null && npc.npcUI != null)
            {
                var uiGameObject = Instantiate(npc.npcUI);
                uiGameObject.transform.SetParent(this.transform);
                currentUI = uiGameObject;
                var npcUI = uiGameObject.GetComponent<NPCUI>();
                if (npcUI != null)
                {
                    _closeDialogButton = npcUI.GetCloseDialogButton();
                    var skipShopButton = npcUI.GetSkipShopDialogButton();
                    var playGameButton = npcUI.GetPlayGameDialogButton();
                    var addGoldButton = npcUI.GetAddGoldDialogButton();
                    if (_closeDialogButton != null)
                        _closeDialogButton.onClick.AddListener(() => CloseDialog());
                    if (skipShopButton != null)
                        skipShopButton.onClick.AddListener(() => skipShopPhase());
                    if (playGameButton != null)
                        playGameButton.onClick.AddListener(() => PlayGame());
                    if(addGoldButton != null)
                        addGoldButton.onClick.AddListener(() => AddGold());
                }
            }
            EnterNPC(callingController);
        }
        else
        {
            ExitNPC();
        }
    }

    public void skipShopPhase()
    {
        if (WaveManager.instance is null) return;
        ExitNPC();
        PlayerManager.instance.ClearAllTextPrompts();
        WaveManager.instance.SkipShopPhase();
    }

    public void CloseDialog()
    {
        if (currentUI != null)
        {
            Destroy(currentUI);
            currentUI = null;
        }
        ToggleNPCUI(false, null, null);
    }

    public void PlayGame()
    {
        CloseDialog();

		LevelLoadManager.instance.QueueScene(LevelLoadManager.gameSceneName);
        SceneManager.LoadScene(LevelLoadManager.loadingSceneName);
    }

    public void AddGold()
    {
        foreach(var player in PlayerManager.instance.players)
        {
            player.PlayerStats.gold += 300;
        }
        CloseDialog();
    }
}