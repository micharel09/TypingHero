using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public sealed class ParrySparkVFX_Pooled : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] ParrySystem parrySystem;       // kéo ParrySystem (Player)
    [SerializeField] GameObject sparkPrefab;        // kéo prefab VFX_HitSpark_Parry (prefab, không để trong Scene)
    [SerializeField] Transform playerSparkAnchor;   // kéo Player/Attack1/ParrySparkAnchor

    [SerializeField] ImpactVfxProfile impactProfile;

    [Header("Tuning")]
    [SerializeField] Vector2 offset;
    [SerializeField] bool flipTowardEnemy = true;
    [SerializeField] bool parentToAnchor = false;
    [SerializeField] float groundY;

    [Header("Pool")]
    [SerializeField] int defaultCapacity = 8;
    [SerializeField] int maxSize = 32;

    [SerializeField] ParryClashBurst.Preset sparkPreset = ParryClashBurst.Preset.BCompact;
    readonly int[] variationSequences = new int[5];
#if TYPINGHERO_AUTOMATION
    void OnGUI() => GUI.Label(new Rect(12, 12, 300, 28), "F6 Spark: " + sparkPreset);
#endif
    ImpactVfxEmitter emitter;
    IObjectPool<GameObject> _pool;
    readonly List<GameObject> active = new(8);
    Transform poolRoot;

    void OnEnable()
    {
        if (!emitter) emitter = GetComponent<ImpactVfxEmitter>();
        if (!emitter) emitter = gameObject.AddComponent<ImpactVfxEmitter>();
        if (parrySystem) parrySystem.OnParrySuccess += Spawn;
    }

    void OnDisable()
    {
        if (parrySystem) parrySystem.OnParrySuccess -= Spawn;
        ClearActive();
        if (emitter) emitter.Clear();
    }

    void BuildPool()
    {
        poolRoot = new GameObject("ParryImpactPool").transform;
        UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(poolRoot.gameObject, gameObject.scene);
        _pool = new ObjectPool<GameObject>(
            createFunc: CreateItem,
            actionOnGet: OnGet,
            actionOnRelease: OnRelease,
            actionOnDestroy: OnDestroyItem,
            collectionCheck: false,
            defaultCapacity: defaultCapacity,
            maxSize: maxSize
        );
    }

    GameObject CreateItem()
    {
        var go = Instantiate(sparkPrefab, poolRoot);
        go.SetActive(false);

        // gắn PooledVFX để animation-end có thể trả về pool
        var pooled = go.GetComponent<PooledVFX>();
        if (!pooled) pooled = go.AddComponent<PooledVFX>();
        pooled.Setup(_pool);

        return go;
    }

    void OnGet(GameObject go)
    {
        active.Add(go);
        go.SetActive(true);
    }

    void OnRelease(GameObject go)
    {
        // thu gọn trước khi cất vào pool
        active.Remove(go);
        if (!go) return;
        if (poolRoot) go.transform.SetParent(poolRoot, true);
        go.SetActive(false);
    }

    void OnDestroyItem(GameObject go)
    {
        if (go) Destroy(go);
    }

    void OnDestroy()
    {
        if (_pool is ObjectPool<GameObject> pool) pool.Clear();
        if (poolRoot) Destroy(poolRoot.gameObject);
    }

    void Update()
    {
#if TYPINGHERO_AUTOMATION
        if (Input.GetKeyDown(KeyCode.F6)) sparkPreset = (ParryClashBurst.Preset)(((int)sparkPreset + 1) % 5);
#endif
        if (CombatSession.Finished) ClearActive();
    }
    void ClearActive()
    {
        while (active.Count > 0)
        {
            int last = active.Count - 1;
            var go = active[last]; active.RemoveAt(last);
            // Independent scene roots can be destroyed before the owner's OnDisable.
            if (go) go.GetComponent<PooledVFX>().Release();
        }
    }

    void Spawn(ParrySystem.ParryContext ctx)
    {
        if (!playerSparkAnchor || GamePauseController.InputBlocked || CombatSession.Finished || Time.timeScale == 0f) return;

        if (sparkPreset == ParryClashBurst.Preset.BCompact &&
            (impactProfile || sparkPrefab && sparkPrefab.GetComponent<ImpactVfxBurst>()))
        {
            // Old scenes retain their prefab reference; builders fill the new profile reference only if missing.
            var selected = impactProfile;
            if (!selected && sparkPrefab && sparkPrefab.TryGetComponent<ImpactVfxBurst>(out var template)) selected = template.impactProfile;
            var point = ctx.contact.valid ? ctx.contact.point : playerSparkAnchor.position + (Vector3)offset;
            float direction = ctx.targetRoot ? ctx.targetRoot.position.x - point.x : transform.lossyScale.x;
            if (direction == 0) direction = transform.lossyScale.x;
            var body = parrySystem ? parrySystem.GetComponent<Collider2D>() : null;
            var recipient = body ? PhysicalParticleProfile.RecipientSnapshot.FromBounds(body.bounds) : default;
            ClearActive();
            var burst = emitter.Play(selected, new ImpactVfxContext {
                point = point, direction = direction, floorY = groundY,
                seed = variationSequences[(int)sparkPreset]++, recipient = recipient,
                unitsPerMeter = ctx.contact.valid ? ctx.contact.unitsPerMeter : 0 });
            if (burst is ParryClashBurst adapter) adapter.SetContact(ctx.contact);
            return;
        }
        emitter.Clear();
        if (!sparkPrefab) return;
        if (_pool == null) BuildPool();
        if ((sparkPrefab.GetComponent<PixelSparkBurst>() || sparkPrefab.GetComponent<ParryClashBurst>()) && active.Count >= 8) active[0].GetComponent<PooledVFX>().Release();
        var go = _pool.Get();
        var t = go.transform;

        // đặt vị trí
        t.position = ctx.contact.valid ? ctx.contact.point : playerSparkAnchor.position + (Vector3)offset;
        if (parentToAnchor) t.SetParent(playerSparkAnchor, worldPositionStays: true);

        // lật hướng về enemy cho đẹp (tuỳ chọn)
        if (flipTowardEnemy && ctx.targetRoot)
        {
            float dir = Mathf.Sign(ctx.targetRoot.position.x - playerSparkAnchor.position.x);
            var s = t.localScale;
            t.localScale = new Vector3(Mathf.Abs(s.x) * (dir == 0 ? 1 : dir), s.y, s.z);
        }
        if (go.TryGetComponent<ParryClashBurst>(out var clash))
        {
            t.SetParent(poolRoot, true);
            float towardAttacker = ctx.targetRoot ? ctx.targetRoot.position.x - t.position.x : transform.lossyScale.x;
            if (towardAttacker == 0) towardAttacker = transform.lossyScale.x;
            var body = parrySystem ? parrySystem.GetComponent<Collider2D>() : null;
            var recipient = body ? PhysicalParticleProfile.RecipientSnapshot.FromBounds(body.bounds) : default;
            int sequence = variationSequences[(int)sparkPreset]++;
            clash.Emit(t.position, towardAttacker, groundY, sparkPreset,
                sequence, ctx.contact, recipient.center.x - t.position.x, recipient);
        }
        else if (go.TryGetComponent<PixelSparkBurst>(out var burst))
        {
            t.SetParent(poolRoot, true);
            burst.Emit(t.position, ctx.targetRoot ? ctx.targetRoot.position.x - playerSparkAnchor.position.x : transform.lossyScale.x, groundY);
        }
    }
}
