using System.Collections.Generic;
using UnityEngine;

public class FollowCam : Singleton<FollowCam>
{
	public List<Transform> targets;
	[SerializeField] float _minDistance = 20.0f;
	[SerializeField] float _maxDistance = 40.0f;
	[SerializeField] float _zoomScale = 2.5f;

	// set private variables for boundaries
	private float xMinBoundary;
	private float xMaxBoundary;
	private float zMinBoundary;
	private float zMaxBoundary;

	public float followSpeed = 5.0f;

	private void Start()
    {	
		// If a levelmanager instance is in the scene use the bounds
		// otherwise default to a large number for the bounds
		if(LevelManager.instance != null)
        {
			LevelManager.instance.UpdateBoundary += UpdateBounds;
			UpdateBounds();
		}
        else
        {
			xMinBoundary = -1000;
			xMaxBoundary = 1000;
			zMinBoundary = -1000;
			zMaxBoundary = 1000;
		}
	}

	private void UpdateBounds()
	{
		if (LevelManager.instance != null)
		{
			xMinBoundary = LevelManager.instance.xMinBoundary;
			xMaxBoundary = LevelManager.instance.xMaxBoundary;
			zMinBoundary = LevelManager.instance.zMinBoundary;
			zMaxBoundary = LevelManager.instance.zMaxBoundary;
		}
	}


	private void LateUpdate()
	{
		if (targets is null || targets.Count == 0) return;

		transform.position = Vector3.Lerp(transform.position, GetTargetPosition(), Time.deltaTime * followSpeed);
	}

	private Vector3 GetTargetPosition()
	{
		Vector3 camPosition = Vector3.zero;
		float distance = _minDistance;
		if (targets.Count == 1)
		{
			camPosition = targets[0].position;
		}
		else
		{
			// Calc center of all targets
			foreach (var target in targets)
			{
				camPosition += target.position;
			}
			camPosition = camPosition / targets.Count;

			// Calc max distance from center
			float maxDist = 0f;
			foreach (var target in targets)
			{
				float dist = Vector3.Distance(camPosition, target.position);
				if (dist > maxDist)
					maxDist = dist;
			}
			distance = Mathf.Clamp(maxDist * _zoomScale, _minDistance, _maxDistance);
		}

		//camPosition += -Quaternion.Euler(-transform.rotation.x, 0, 0).eulerAngles * _distance;
		camPosition -= transform.forward * distance;

		// clamp the camera position according to the x,z bounds
		float clampedX = Mathf.Clamp(camPosition.x, xMinBoundary, xMaxBoundary);
		float clampedZ = Mathf.Clamp(camPosition.z, zMinBoundary, zMaxBoundary);

		// Give a new camera position with the bounds
		var targetPos = new Vector3(clampedX, camPosition.y, clampedZ);
		return targetPos;
	}
}
