using UnityEngine;

public class TimeManipulator : MonoBehaviour
{
    [Range(0, 1)] public float deltaTimeScale = 1.0f;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            deltaTimeScale += 0.05f;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            deltaTimeScale -= 0.05f;
        }
        deltaTimeScale = Mathf.Clamp(deltaTimeScale, 0.0f, 1.0f);
        Time.timeScale = deltaTimeScale;
    }
}
