using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.UI;

public class ShopUIController : MonoBehaviour
{
    [SerializeField] GameObject abilityOption1;
    [SerializeField] GameObject abilityOption2;
    [SerializeField] GameObject abilityOption3;

    [SerializeField] TextMeshProUGUI goldDisplay;
    [SerializeField] TextMeshProUGUI shuffleGoldDisplay;

    [SerializeField] MultiplayerEventSystem multiplayerEventSystem;
    [SerializeField] GameObject firstSelectedGameObject;
    public InputSystemUIInputModule inputSystemUIInputModule => _inputModule;
    [SerializeField] InputSystemUIInputModule _inputModule;
    [SerializeField] Player _player;

    private void Awake()
    {
        var playerController = PlayerManager.instance.PlayerControllers.FirstOrDefault(x => x.playerIndex == _player.GetPlayerIndex());
        if (playerController != null)
        {
            playerController.playerInput.uiInputModule = _inputModule;
        }
    }

    public void UpdateShopDisplay(List<AbilityInfo> currentAbilitiesInShop, int shuffleGoldAmount, bool setFirstSelectedGameObject = true)
    {
        if (currentAbilitiesInShop != null && currentAbilitiesInShop.Count() >= 3)
        {
            abilityOption1.GetComponentInChildren<ShopOptionCardController>().UpdateShopOptionCardWithAbility(currentAbilitiesInShop[0]);
            abilityOption2.GetComponentInChildren<ShopOptionCardController>().UpdateShopOptionCardWithAbility(currentAbilitiesInShop[1]);
            abilityOption3.GetComponentInChildren<ShopOptionCardController>().UpdateShopOptionCardWithAbility(currentAbilitiesInShop[2]);
        }

        if (setFirstSelectedGameObject)
        {
            multiplayerEventSystem.SetSelectedGameObject(firstSelectedGameObject);
        }

        goldDisplay.text = $"Gold: {_player.PlayerStats.gold}";
        shuffleGoldDisplay.text = $"Shuffle - {shuffleGoldAmount} Gold";
    }

    public Player GetUIControllingPlayer() => _player;
}
