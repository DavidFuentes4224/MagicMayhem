using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] AudioClip collectAudio;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void SetValue(int value)
    {
        _value = value;
    }


    private void OnTriggerEnter(Collider collision)
    {
        if (collision != null && collision.gameObject.tag == "Player")
        {
            transform.localScale = Vector3.zero;
            PlayerManager.instance.AddGold(_value);
            _audioSource.PlayOneShot(collectAudio);
            StartCoroutine(DestroyAfterAudio());
        }
    }

    private IEnumerator DestroyAfterAudio()
    {
        yield return new WaitForSeconds(collectAudio.length);
        Destroy(this.gameObject);
    }

    AudioSource _audioSource;
    int _value;
}
