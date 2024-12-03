using UnityEngine;

public class ProjectileMovement : MonoBehaviour
{
    [SerializeField] float _timeTilDeath;
    public Vector3 velocity;

    // Update is called once per frame
    void Update()
    {
        _timeTilDeath -= Time.deltaTime;
        // We could probably use a projectile pool if the game becomes laggy.
        if (_timeTilDeath <= 0.0f)
            Destroy(gameObject);
        transform.position += transform.forward * velocity.z * Time.deltaTime;
    }
}
