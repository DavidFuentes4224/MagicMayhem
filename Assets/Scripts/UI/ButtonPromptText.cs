using TMPro;
using UnityEngine;

public class ButtonPromptText : MonoBehaviour
{
    [SerializeField] Transform _avatarTransform;
    [SerializeField] TextMeshPro _promptText;
	[SerializeField] Vector3 _offset;

    void Update()
    {
        transform.position = _avatarTransform.position + _offset;
    }

    public void SetPrompt(string prompt) => _promptText.text = prompt;
    public void ClearPrompt() => _promptText.text = string.Empty;
}
