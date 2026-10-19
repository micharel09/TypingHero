using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Presentation only: disabled tint and existing validation label, without event listeners.
public sealed class DungeonControlSkin : MonoBehaviour
{
    [SerializeField] TMP_Text error;
    Image image;
    Selectable control;
    void Awake() { image = GetComponent<Image>(); control = GetComponent<Selectable>(); }
    void LateUpdate()
    {
        image.color = !control.IsInteractable() ? new Color(.345f, .318f, .373f) : error && error.gameObject.activeSelf && !string.IsNullOrEmpty(error.text)
            ? new Color(1f, .42f, .4f) : Color.white;
    }
}
