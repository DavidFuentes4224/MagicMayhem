using UnityEngine;

public abstract class RefreshableComponent : MonoBehaviour
{
	public abstract void OnInit();
	public abstract void OnKilled();
	public virtual int updatePriority => 0;
}
