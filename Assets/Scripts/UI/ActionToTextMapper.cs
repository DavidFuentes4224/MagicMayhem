using UnityEngine;

public static class ActionToTextMapper
{
	public enum PlayerInputAction
	{
		OPENSTORE,
		CLOSESTORE,
		ACTIVATE,
		UPGRADES,
	}

	public static string GetInputTextForAction(PlayerInputAction action, bool usingMK) => action switch
	{
		PlayerInputAction.OPENSTORE => usingMK ? "[E]" : "(X)",
		PlayerInputAction.CLOSESTORE => usingMK ? "[E]" : "(X)",
		PlayerInputAction.ACTIVATE => usingMK ? "[F]" : "(A)",
		PlayerInputAction.UPGRADES => usingMK ? "[Tab]" : "(Y)",
		_ => ""
	};
}
