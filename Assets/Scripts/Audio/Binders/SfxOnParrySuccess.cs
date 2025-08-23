using UnityEngine;

[DisallowMultipleComponent]
public sealed class SfxOnParrySuccess : MonoBehaviour
{
    [SerializeField] ParrySystem parry;      // auto-find ở Reset
    [SerializeField] SfxEvent sfxParry;
    [SerializeField] Transform playAt;       // neo phát (vd: Player/ParrySparkAnchor)
    [SerializeField] bool useEnemyRoot;      // nếu bật: phát ở enemy từ ParryContext

    void Reset()
    {
        if (!parry) parry = GetComponentInParent<ParrySystem>();
        if (!playAt) playAt = transform;
    }

    void OnEnable() { if (parry) parry.OnParrySuccess += Handle; }
    void OnDisable() { if (parry) parry.OnParrySuccess -= Handle; }

    void Handle(ParrySystem.ParryContext ctx)
    {
        if (!SfxPlayer.I || !sfxParry) return;
        Transform t = (useEnemyRoot && ctx.targetRoot) ? ctx.targetRoot : (playAt ? playAt : transform);
        SfxPlayer.I.Play(sfxParry, t.position, t);
    }
}
