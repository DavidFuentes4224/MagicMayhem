using UnityEngine;

public class NPCTrigger : MonoBehaviour
{
	public bool playerTalkingToNPC = false;

    // Trigger the ui based on if the player is inside the collider
    public virtual void OnTriggerEnter(Collider collision)
	{
		if (collision.CompareTag("Player"))
		{
			var player = collision.gameObject.GetComponentInParent<Player>();
			if (player)
			{
				player.SetPromptText(_talkToNpcTextFormat, ActionToTextMapper.PlayerInputAction.ACTIVATE);
				player.SetNpcTrigger(this);
			}
				
			playerTalkingToNPC = true;
		}
	}

	public virtual void OnTriggerExit(Collider collision)
	{
		if (collision.CompareTag("Player"))
		{
			var player = collision.gameObject.GetComponentInParent<Player>();
			if (player)
			{
				player.ClearPromptText();
				player.SetNpcTrigger(null);
			}
			playerTalkingToNPC = false;
		}
	}

	string _talkToNpcTextFormat = "Press {0} to talk.";
}
