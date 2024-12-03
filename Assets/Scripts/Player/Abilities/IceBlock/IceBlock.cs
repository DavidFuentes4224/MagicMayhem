using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class IceBlock : Ability
{
    [SerializeField] StatusEffect firstBuffStatusEffect;
    [SerializeField] StatusEffect secondBuffStatusEffect;
    [SerializeField] AudioSource audioSource;
    public float playerAnimPauseTime;
    private bool isAudioPaused;
    private Coroutine audioCoroutine;

    public override void Awake()
    {
        audioCoroutine = StartCoroutine(AudioFadeOut.FadeOut(audioSource, abilityInfo.despawnTime));
        base.Awake();
    }

    public override void Update()
    {
        if (Time.deltaTime == 0)
        {
            audioSource.Pause();
            StopCoroutine(audioCoroutine);
            isAudioPaused = true;
            _playerInput.actions.FindActionMap("PauseOnly").Disable();
        }
        else
        {
            if (isAudioPaused == true)
            {
                audioSource.UnPause();
                audioCoroutine = StartCoroutine(AudioFadeOut.FadeOut(audioSource, remainingDespawnTime));
                isAudioPaused = false;
                DisableGamePlayandUIInput();
            }
        }
        base.Update();
    }

    public override void Start()
    {
        if (_player != null)
        {
            _player.PlayerStats.StatusEffects.AddStatusEffect(firstBuffStatusEffect);
            _player.PlayerStats.StatusEffects.AddStatusEffect(secondBuffStatusEffect);
            _playerController = PlayerManager.instance.PlayerControllers.FirstOrDefault(x => x.playerIndex == _player.GetPlayerIndex());
            _playerInput = _playerController.playerInput;
            _playerAnim = _player.transform.GetChild(0).GetComponent<Animator>();
            StartCoroutine(DisablePlayerAnimator(_playerAnim, playerAnimPauseTime));
            DisableGamePlayandUIInput();
        }
    }
    
    public override void UpdateProjectileVelocity()
    {
        transform.position = _player.GetAvatarPosition();
    }

    public override void Despawn()
    {
        EnablePlayerInput();
        base.Despawn();
    }

    public void DisableGamePlayandUIInput()
    {
        _playerInput.actions.FindActionMap("UI").Disable();
        _playerInput.actions.FindActionMap("GamePlay").Disable();
        _playerInput.actions.FindActionMap("PauseOnly").Enable();
    }

    public void EnablePlayerInput()
    {
        _playerInput.actions.FindActionMap("PauseOnly").Disable();
        _playerInput.actions.FindActionMap("GamePlay").Enable();
        _playerAnim.enabled = true;
        if (WaveManager.instance != null && !WaveManager.instance.inWaveCooldown)
        {
            _playerInput.actions.FindAction("OpenShop").Disable();
        }
    }

    public IEnumerator DisablePlayerAnimator(Animator playerAnim, float playerAnimPauseTime)
    {
        yield return new WaitForSeconds(playerAnimPauseTime);
        playerAnim.enabled = false;
    }

    PlayerController _playerController;
    PlayerInput _playerInput;
    Animator _playerAnim;
}
