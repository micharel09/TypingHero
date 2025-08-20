using System.Collections.Generic;
using UnityEngine;

/// Gắn lên root Enemy (và/hoặc Player).
/// Kéo đúng các script hit-flash vào Targets (vd: EnemyHitFlash).
[DisallowMultipleComponent]
public class DisableHitFlashOnSlayer : MonoBehaviour
{
    [SerializeField] List<MonoBehaviour> targets = new List<MonoBehaviour>();

    void OnEnable()
    {
        SlayerModeSignals.OnSetActive += Apply;
        Apply(SlayerModeSignals.Active); // đồng bộ ngay
    }

    void OnDisable()
    {
        SlayerModeSignals.OnSetActive -= Apply;
        SetTargetsEnabled(true); // đảm bảo bật lại nếu component bị tắt giữa chừng
    }

    void Apply(bool slayerOn) => SetTargetsEnabled(!slayerOn);

    void SetTargetsEnabled(bool enable)
    {
        for (int i = 0; i < targets.Count; i++)
        {
            var mb = targets[i];
            if (mb) mb.enabled = enable;
        }
    }
}
