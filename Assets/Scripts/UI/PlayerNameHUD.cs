using TMPro;
using UnityEngine;

/// Hiển thị tên người chơi lên HUD. Lắng nghe sự kiện đổi tên.
[DisallowMultipleComponent]
public sealed class PlayerNameHUD : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] TextMeshProUGUI target;

    [Header("Format")]
    [SerializeField] string prefix = "";          // ví dụ: "PLAYER: "
    [SerializeField] bool upperCase = false;
    [SerializeField] bool hideWhenEmpty = true;

    void OnEnable()
    {
        PlayerIdentity.EnsureLoaded();
        PlayerIdentity.OnNameChanged += HandleNameChanged;
        Apply(PlayerIdentity.Name);
    }

    void OnDisable()
    {
        PlayerIdentity.OnNameChanged -= HandleNameChanged;
    }

    void Reset()
    {
        target = GetComponentInChildren<TextMeshProUGUI>();
    }

    void HandleNameChanged(string newName) => Apply(newName);

    void Apply(string nameRaw)
    {
        string name = nameRaw ?? string.Empty;
        if (upperCase) name = name.ToUpperInvariant();

        if (!target) return;

        bool empty = string.IsNullOrWhiteSpace(name);
        if (hideWhenEmpty)
        {
            target.gameObject.SetActive(!empty);
            if (!empty) target.text = $"{prefix}{name}";
        }
        else
        {
            target.gameObject.SetActive(true);
            target.text = empty ? $"{prefix}Guest" : $"{prefix}{name}";
        }
    }
}
