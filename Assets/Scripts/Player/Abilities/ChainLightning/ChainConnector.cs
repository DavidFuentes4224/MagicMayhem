using UnityEngine;
public class ChainConnector : MonoBehaviour
{
    public float remainingDespawnTime;
    private Transform target1;
    private Transform target2;

    void Update()
    {
        remainingDespawnTime -= Time.deltaTime;
        if (remainingDespawnTime <= 0)
        {
            Destroy(gameObject);
        }
        if(target1 != null && target2 != null)
        {
            UpdateChainConnectorPosition();
        }
    }

    public void SetFirstTarget(Transform target)
    {
       target1 = target;
    }

    public void SetSecondTarget(Transform target)
    {
        target2 = target;
    }

    public void UpdateChainConnectorPosition()
    {
        var midpoint = (target1.position + target2.position) / 2;
        transform.position = new Vector3(midpoint.x, 1.5f, midpoint.z);

        var direction = target1.position - target2.position;
        transform.rotation = Quaternion.LookRotation(direction);

        var distance = direction.magnitude;
        transform.localScale = new Vector3(5, 1, distance);
    }
}
