using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// When collision is detected and is a trap
// pass this trap reference to the player
public class AvatarTrapActivation : MonoBehaviour
{
	public void Awake()
    {
		_player = GetComponentInParent<Player>();
    }

    private void OnTriggerEnter(Collider collision)
	{
		if (collision.tag == "Trap")
		{
			// Assign collision to trap variable and
			// pass trap reference to the player instance
			_trap = collision.GetComponent<AbstractTrap>();
			_trap.TrapTriggerUI.SetActive(true);
			if (_player != null && _trap != null)
			{
				// Pass trap reference to the player instance
				_player.SetDetectedTrap(_trap);
            }
		}
	}
	private void OnTriggerExit(Collider collision)
	{
		if (collision.tag == "Trap")
		{
			// Reset trap variable to null and pass it to
			// player instance
			_trap.TrapTriggerUI.SetActive(false);
			_trap = null;
			if (_player != null)
			{
				_player.SetDetectedTrap(_trap);

			}
		}
	}

	Player _player;
	AbstractTrap _trap;
}
