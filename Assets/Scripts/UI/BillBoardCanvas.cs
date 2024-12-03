using UnityEngine;

public class BillBoardCanvas : MonoBehaviour
{
	public bool flip = false;
	void Update()
	{
		transform.rotation = Quaternion.Euler(35,0,0);
		if (flip)
			transform.rotation = transform.rotation * Quaternion.Euler(0, 180, 0);
	}

	Vector3 camRotation = new Vector3 (35, 0, 0);
}
