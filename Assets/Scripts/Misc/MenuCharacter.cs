using TMPro;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class MenuCharacter : MonoBehaviour
{
	[Header("UI")]
	[SerializeField] TextMeshProUGUI _joinText;
	[SerializeField] TextMeshProUGUI _playerText;

	[Header("Config")]
	[SerializeField] int _playerIndex;
	[SerializeField] int _idlePose = 1;

	public int playerIndex => _playerIndex;

	private void Awake()
	{
		_anim = GetComponent<Animator>();
	}

	private void Start()
	{
		_anim.SetInteger("Pose", _idlePose);
	}

	private void OnDestroy()
	{
		var cancelAction = _playerController?.playerInput?.currentActionMap?.FindAction("Cancel");
		if (cancelAction is object)
			cancelAction.performed -= PlayerController_OnCancelPerformed; _playerController = null;
	}

	private void PlayerController_OnCancelPerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
	{
		var cancelAction = _playerController?.playerInput?.currentActionMap?.FindAction("Cancel");
		if (cancelAction is object)
			cancelAction.performed -= PlayerController_OnCancelPerformed;
		PlayerManager.instance.RemovePlayer(_playerController.playerIndex);
		BackOut();
	}

	public void Join(PlayerController controller)
	{
		_playerText.gameObject.SetActive(true);
		_playerText.text = $"Player {playerIndex + 1}";
		_joinText.gameObject.SetActive(false);
		
		if (_playerController != controller)
		{
			_playerController = controller;
			_playerController.playerInput.currentActionMap.FindAction("Cancel").performed += PlayerController_OnCancelPerformed;
		}
	}

	public void BackOut()
	{
		_playerText.gameObject.SetActive(false);
		_joinText.gameObject.SetActive(true);

		var cancelAction = _playerController?.playerInput?.currentActionMap?.FindAction("Cancel");
		if (cancelAction is object)
			cancelAction.performed -= PlayerController_OnCancelPerformed; _playerController = null;
		_playerController = null;
	}

	Animator _anim;
	PlayerController _playerController;
}
