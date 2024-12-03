using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;

public abstract class AbstractTrap : MonoBehaviour
{
    public TrapInfo trapInfo;
    public GameObject TrapTriggerUI;
    public TrapCooldownIcon TrapCooldownUI;
    private List<Collider> trackedEnemies = new List<Collider>();
    [SerializeField] private Animator myTrap = null;
    [SerializeField] private AudioClip trapTriggerSound;
    [SerializeField] private AudioClip trapActivateSound;

    private void Start()
    {
        TrapTriggerUI.SetActive(false);
        _audioSource = GetComponent<AudioSource>();
        // Set the info text based on the trapInfo registry
        _trapInfoText = TrapTriggerUI.GetComponentsInChildren<TextMeshProUGUI>();
        _trapInfoText[0].text = $"Press F / Bottom Button to activate\nCost: {trapInfo.trapCost} Gold\nTrap Active Time: {trapInfo.activeDuration} Seconds";
    }

    public void ActivateTrap()
    {
        trapInfo.isActivated = true;
        _audioSource.PlayOneShot(trapActivateSound);
        TrapCooldownUI.ActivateCooldownUI();
        StartCoroutine(TrapActivationDuration());
    }

    public virtual IEnumerator TrapActivationDuration()
    {
        // Keep trap active for a certain duration and
        // then disable trap and reset animation
        yield return new WaitForSeconds(trapInfo.activeDuration);
        trapInfo.isActivated = false;
        myTrap.SetTrigger("ResetTrap");
        _audioSource.PlayOneShot(trapTriggerSound);
    }


    public virtual void OnTriggerEnter(Collider collision)
    {
        // If enemy is in trap and the trap is activated do the animation
        // and add the enemy to list of enemies in trap
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy") && trapInfo.isActivated)
        {
            trapInfo.isSprung = true;
            trackedEnemies.Add(collision);
            if (trackedEnemies.Count == 1)
            {
                myTrap.SetTrigger("TriggerTrap");
                _audioSource.PlayOneShot(trapTriggerSound);
            }
        }
    }

    public virtual void OnTriggerStay(Collider collision)
    {
        // If enemy is in trap and the trap is activated do the animation
        // and the trap animation has been sprung then handle the damage and start
        // trap damage cooldown
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy") && trapInfo.isActivated)
        {
            if (trapInfo.isSprung)
            {
                collision.GetComponent<HealthComponent>().TakeDamage(trapInfo.damage);
                trapInfo.isSprung = false;
                StartCoroutine(UseTrapCoroutine());
            }
        }
    }

    //Cooldown for enemy taking damage
    //If its a fire trap, this functionality would go into the enemy's class
    //If a spike trap, this function would stay in Trap's class
    public virtual IEnumerator UseTrapCoroutine()
    {
        yield return new WaitForSeconds(trapInfo.damageCooldown);
        trapInfo.isSprung = true;
    }

    public virtual void OnTriggerExit(Collider collision)
    {
        // enemy leaves trap and there are no longer any more enemies
        // in trap then reset the trap animation
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            trapInfo.isSprung = true;
            trackedEnemies.Remove(collision);
        }
    }
    void Update()
    {
        for (int i = trackedEnemies.Count - 1; i >= 0; i--)
        {
            if (!trackedEnemies[i].gameObject.activeInHierarchy) 
            {
                trackedEnemies.RemoveAt(i);
            }
        }
    }

    TextMeshProUGUI[] _trapInfoText;
    AudioSource _audioSource;
}
