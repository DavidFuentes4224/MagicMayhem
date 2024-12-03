using System.Linq;
using UnityEngine;
using UnityExtensions;
public class CharacterCustomizer : MonoBehaviour
{
	[SerializeField] Material[] _playerMaterials;
	[SerializeField] Player _player;

	private void Start()
	{
		if (_playerMaterials is null || _playerMaterials.Length == 0 || _player is null) return;

		var renderers = gameObject.GetAllComponents<Renderer>().Distinct();
		foreach (var renderer in renderers)
		{
			renderer.material = _playerMaterials[_player.GetPlayerIndex()];
		}
	}
}
