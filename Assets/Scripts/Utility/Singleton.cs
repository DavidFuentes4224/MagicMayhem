using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T instance { get; private set; }
	void Awake()
	{
		if (instance != null)
		{
			Destroy(this);
			return;
		}

		instance = this as T;

        DoAwake();
	}

	// Start is called before the first frame update
	void Start()
    {
        DoStart();
    }

    // Update is called once per frame
    void Update()
    {
        DoUpdate();
    }

    protected virtual void DoAwake() { }
    protected virtual void DoStart() { }
    protected virtual void DoUpdate() { }
}
