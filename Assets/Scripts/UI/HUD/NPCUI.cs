using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCUI : MonoBehaviour
{
    [SerializeField] Button closeDialogButton;
    [SerializeField] Button skipShopDialogButton;
    [SerializeField] Button playGameDialogButton;
    [SerializeField] Button addGoldDialogButton;

    public Button GetCloseDialogButton() => closeDialogButton;
    public Button GetSkipShopDialogButton() => skipShopDialogButton;
    public Button GetPlayGameDialogButton() => playGameDialogButton;
    public Button GetAddGoldDialogButton() => addGoldDialogButton;
}
