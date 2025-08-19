using UnityEngine;

[DisallowMultipleComponent]
public sealed class SlayerTrail : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] PlayerSlayerMode slayer;         // kéo từ Player
    [SerializeField] PlayerAttackEvents attackEvents; // kéo từ Player
    [SerializeField] TrailRenderer trail;             // trail đặt trên kiếm/tay

    [Header("Behavior")]
    [SerializeField] bool onlyDuringAttackWindow = true; // chỉ nhả vệt khi cửa sổ đánh mở
    [SerializeField] bool clearOnClose = true;           // đóng cửa sổ thì xóa vệt
    [SerializeField] bool clearOnExitSlayer = true;      // thoát Slayer thì xóa vệt
    [SerializeField] bool startCleared = true;           // vào game xóa vệt ngay

    bool _open;
    bool _wasSlayer;

    void Reset()
    {
        slayer = GetComponentInParent<PlayerSlayerMode>();
        attackEvents = GetComponentInParent<PlayerAttackEvents>();
        trail = GetComponentInChildren<TrailRenderer>();
    }

    void OnEnable()
    {
        if (attackEvents != null)
        {
            attackEvents.OnWindowOpen += OnOpen;
            attackEvents.OnWindowClose += OnClose;
        }
        if (trail)
        {
            trail.emitting = false;
            if (startCleared) trail.Clear();
        }
    }

    void OnDisable()
    {
        if (attackEvents != null)
        {
            attackEvents.OnWindowOpen -= OnOpen;
            attackEvents.OnWindowClose -= OnClose;
        }
    }

    void Update()
    {
        bool slayerActive = slayer && slayer.IsActive;

        // vừa rời Slayer → dọn vệt
        if (_wasSlayer && !slayerActive && clearOnExitSlayer && trail)
        {
            trail.emitting = false;
            trail.Clear();
        }
        _wasSlayer = slayerActive;

        bool shouldEmit = slayerActive && (!onlyDuringAttackWindow || _open);
        if (trail && trail.emitting != shouldEmit) trail.emitting = shouldEmit;
    }

    void OnOpen()
    {
        _open = true;
        // mở đợt mới → làm sạch đầu vệt cũ để không bị đứt khúc lạ
        if (trail && trail.emitting) trail.Clear();
    }

    void OnClose()
    {
        _open = false;
        if (trail && clearOnClose)
        {
            trail.emitting = false;
            trail.Clear();
        }
    }
}
