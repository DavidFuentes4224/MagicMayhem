using System.Linq;

public class ScatterShot : Ability
{
	public override void SetPlayer(Player player)
	{
		base.SetPlayer(player);
		foreach (var subSpell in GetComponentsInChildren<Ability>().Where(x => x != this))
			subSpell.SetPlayer(player);
	}
}
