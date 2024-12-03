using UnityEngine;

public class PhysicsPusher : MonoBehaviour
{
	[SerializeField] float _pushForce = 50.0f;
	private void OnControllerColliderHit(ControllerColliderHit hit)
	{
		var rb = hit.collider.attachedRigidbody;
		if (rb is null || rb.isKinematic)
			return;

		var pushDir = hit.moveDirection;
		rb.AddForce(pushDir * _pushForce, ForceMode.Impulse);
	}
}
