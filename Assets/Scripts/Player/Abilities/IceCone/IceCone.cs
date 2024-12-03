using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class IceCone : Ability
{
    [SerializeField] AudioSource audioSource;
    private bool isAudioPaused;
    private Coroutine audioCoroutine;
    private float timeToDisableSpellCollider = 0.1f;
    private float playerInputDisableTime = 0.2f;

    public override void Awake()
    {
        audioCoroutine = StartCoroutine(AudioFadeOut.FadeOut(audioSource, abilityInfo.despawnTime));
        StartCoroutine(DisableCollider());
        base.Awake();
    }

    public override void Start()
    {
        if (_player != null)
        {
            
            _playerController = PlayerManager.instance.PlayerControllers.FirstOrDefault(x => x.playerIndex == _player.GetPlayerIndex());
            _playerInput = _playerController.playerInput;
            StartCoroutine(DisablePlayerInput());
        }
    }

    IEnumerator DisableCollider()
    {
        yield return new WaitForSeconds(timeToDisableSpellCollider);
        transform.GetComponentInChildren<MeshCollider>().enabled = false;
    }

    IEnumerator DisablePlayerInput()
    {
        _playerInput.actions.FindActionMap("GamePlay").Disable();
        yield return new WaitForSeconds(playerInputDisableTime);
        _playerInput.actions.FindActionMap("GamePlay").Enable();
        if (WaveManager.instance != null && !WaveManager.instance.inWaveCooldown)
        {
            _playerInput.actions.FindAction("OpenShop").Disable();
        }
    }

    public override void Update()
    {
        if (Time.deltaTime == 0)
        {
            audioSource.Pause();
            StopCoroutine(audioCoroutine);
            isAudioPaused = true;
        }
        else
        {
            if (isAudioPaused == true)
            {
                audioSource.UnPause();
                audioCoroutine = StartCoroutine(AudioFadeOut.FadeOut(audioSource, remainingDespawnTime));
                isAudioPaused = false;
            }
        }
        base.Update();
    }

    public override void OnTriggerEnter(Collider collision)
    {
        if (collision != null && collision.tag != "Player" && collision.GetComponent<HealthComponent>() != null)
        {
            collision.GetComponent<HealthComponent>().TakeDamage(GetAbilityDamage());
        }
    }
    public override void UpdateProjectileVelocity() { }

    PlayerController _playerController;
    PlayerInput _playerInput;
}
