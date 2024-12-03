using UnityEngine;

public class LootDropComponent : RefreshableComponent
{
	[SerializeField] LootInfo lootInfo;

	public void DropLoot()
	{
		var loot = Instantiate(lootInfo.lootGameObject, transform.position, Quaternion.identity);
		if(loot.GetComponent<Coin>() != null) 
			loot.GetComponent<Coin>().SetValue(lootInfo.amount);
		if (loot.GetComponent<HealthPotion>() != null)
			loot.GetComponent<HealthPotion>().SetValue(lootInfo.amount);
	}

	// N/A
	public override void OnInit() { }

	public override void OnKilled()
	{
		DropLoot();
	}
}
