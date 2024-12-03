using UnityEngine;
using UnityEngine.InputSystem.UI;

public class AbilitySlotConfirmationController : MonoBehaviour
{
    [SerializeField] InputSystemUIInputModule _inputModule;
    [SerializeField] Player _player;
    [SerializeField] GameObject firstSelectedGameObject;

    public void UpdateAbilityOptionDisplay(MultiplayerEventSystem multiplayerEventSystem)
    {
        multiplayerEventSystem.SetSelectedGameObject(firstSelectedGameObject);
    }
}
