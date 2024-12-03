using System.Collections;
using System.Linq;
using UnityEngine;
using UnityRandom = UnityEngine.Random;

[RequireComponent(typeof(Animator))]
public class Goblin : EnemyBase
{
	[Header("Appearance")]
	[SerializeField] GameObject[] _leftAccessories;
	[SerializeField] GameObject[] _rightAccessories;
	[SerializeField] GameObject[] _skins;
	[SerializeField] float _accessoryChance = 0.25f;

	protected override void DoAwake()
	{
		_animator = GetComponent<Animator>();
		_meleeAttackComponent = GetComponent<MeleeAttackComponent>();
		_dissolver = GetComponent<Dissolver>();
	}

	protected override void DoStart()
	{
		RandomizeLook();
		_animator.SetInteger("SwingNum", UnityRandom.Range(1, 4));
	}

	void FixedUpdate()
	{
		if (!_healthComponent.IsAlive)
			return;

		if (_meleeAttackComponent.canAttack)
			_meleeAttackComponent.MeleeAttack();
	}

	protected override IEnumerator SelfOnInit()
	{
		RandomizeLook();
		yield return null;
	}

	void RandomizeLook()
	{
		var randomSkin = UnityRandom.Range(0, _skins.Length);
		var selectedSkin = _skins[randomSkin];
		foreach (var skin in _skins.Where(s => s != selectedSkin))
			skin.SetActive(false);

		_dissolver.SetTargetRenderer(selectedSkin.GetComponent<Renderer>());

		GetComponent<RagdollComponent>().SetBoundMesh(selectedSkin.transform);

		selectedSkin.SetActive(true);

		if (UnityRandom.Range(0.0f, 1.0f) > _accessoryChance)
			_leftAccessories[UnityRandom.Range(0, _leftAccessories.Length)].SetActive(true);
		if (UnityRandom.Range(0.0f, 1.0f) > _accessoryChance)
			_rightAccessories[UnityRandom.Range(0, _rightAccessories.Length)].SetActive(true);
	}

	Animator _animator;
	Dissolver _dissolver;
	MeleeAttackComponent _meleeAttackComponent;
}
