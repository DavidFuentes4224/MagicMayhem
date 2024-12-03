using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerChest : MonoBehaviour
{
    [SerializeField] private Animator myChest = null;

    private void OnTriggerEnter(Collider c)
    {
        Rigidbody r = c.attachedRigidbody;
        if (r != null)
        {
            if (c.attachedRigidbody.gameObject.tag == "Player")
            {
                myChest.SetTrigger("OpenChest");
            }
        }
    }
    private void OnTriggerExit(Collider c)
    {
        Rigidbody r = c.attachedRigidbody;
        if (r != null)
        {
            if (c.attachedRigidbody.gameObject.tag == "Player")
            {
                myChest.SetTrigger("CloseChest");
            }
        }
    }
}
