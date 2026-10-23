using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Cinemachine;

[DisallowMultipleComponent]
public class HitSparkSpawner : MonoBehaviour
{
    [SerializeField] GameObject sparkPrefab;
    [SerializeField] GameObject[] slayerPrefabs;
    [SerializeField] GameObject lethalPrefab;
    [SerializeField] SfxEvent normalImpact, slayerImpact, lethalImpact;
    [SerializeField] CinemachineImpulseSource lethalImpulse;
    [SerializeField] bool parentUnderAttacker;
    [SerializeField] bool keepWorldScale = true;
    [SerializeField] Vector3 worldOffset;
    [SerializeField] bool faceFromAttacker = true;
    [SerializeField] float zRotationOffset;
    [SerializeField] Vector2 angleJitter = new(-2f, 2f);
    [SerializeField, Range(0, 1)] float flipXChance = .5f;
    [SerializeField, Range(0, 1)] float flipYChance;
    [SerializeField] Vector2 scaleRange = Vector2.one;
    [SerializeField] Vector2 verticalJitter = new(-.03f, .03f);
    [SerializeField] float minInterval = .02f;
    [SerializeField] float groundY;

    readonly Dictionary<GameObject, ObjectPool<GameObject>> pools = new();
    readonly List<GameObject> active = new(8);
    Transform poolRoot;
    float nextAt;
    int slayerIndex;
    int voxelSequence;

    void Awake()
    {
        // WebGL decodes referenced clips lazily; request every variant before the first hit.
        if (normalImpact) normalImpact.PreloadClipData();
        if (slayerImpact) slayerImpact.PreloadClipData();
        if (lethalImpact) lethalImpact.PreloadClipData();
        if (sparkPrefab && sparkPrefab.GetComponent<NormalHitSpark>())
        {
            var shader = sparkPrefab.GetComponent<SpriteRenderer>().sharedMaterial.shader;
            var variants = new ShaderVariantCollection();
            variants.Add(new ShaderVariantCollection.ShaderVariant(shader, UnityEngine.Rendering.PassType.Normal));
            variants.WarmUp();
            Destroy(variants);
        }
        // Independent world transform: sparks neither inherit motion nor survive their owner.
        poolRoot = new GameObject("CombatImpactPool").transform;
        UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(poolRoot.gameObject, gameObject.scene);
    }

    ObjectPool<GameObject> PoolFor(GameObject prefab)
    {
        if (pools.TryGetValue(prefab, out var existing)) return existing;
        ObjectPool<GameObject> pool = null;
        pool = new ObjectPool<GameObject>(() =>
        {
            var go = Instantiate(prefab, poolRoot);
            go.SetActive(false);
            go.GetComponent<PooledVFX>().Setup(pool);
            return go;
        }, go => { active.Add(go); go.SetActive(true); }, go =>
        {
            active.Remove(go);
            if (go) { go.SetActive(false); go.transform.SetParent(poolRoot, true); }
        }, go => { if (go) Destroy(go); }, true, 8, 32);
        pools.Add(prefab, pool);
        return pool;
    }

    // Retain existing callers and their rate limit; confirmed impacts are already deduplicated by hitbox.
    public void Spawn(Vector2 worldPoint, Transform attacker, Transform targetRoot)
    {
        if (Time.unscaledTime < nextAt) return;
        nextAt = Time.unscaledTime + minInterval;
        SpawnPrefab(sparkPrefab, worldPoint, attacker, targetRoot);
    }

    internal void SpawnImpact(Vector2 point, Transform attacker, bool wasSlayer, bool lethal, Transform targetRoot = null, float? directionOverride = null)
    {
        if (!isActiveAndEnabled || GamePauseController.InputBlocked || Time.timeScale == 0f) return;
        GameObject prefab = sparkPrefab;
        // Existing serialized consumers retain their spark until role prefabs are assigned.
        if (lethal && lethalPrefab) prefab = lethalPrefab;
        else if (!lethal && wasSlayer && slayerPrefabs != null && slayerPrefabs.Length > 0)
            prefab = slayerPrefabs[slayerIndex++ % slayerPrefabs.Length];
        var images = wasSlayer && attacker ? attacker.GetComponentInChildren<SlayerAfterimagePool>() : null;
        SpawnPrefab(prefab, point, attacker, targetRoot, directionOverride, images ? images.CurrentTint : Color.clear);
        var sound = lethal ? lethalImpact : wasSlayer ? slayerImpact : normalImpact;
        if (sound && SfxPlayer.I) SfxPlayer.I.Play(sound, point);
        // Confirmed contacts own camera impact independently of this visual emitter.
    }

    void SpawnPrefab(GameObject prefab, Vector2 point, Transform attacker, Transform targetRoot, float? directionOverride = null, Color? impactTint = null)
    {
        if (!isActiveAndEnabled || !prefab || GamePauseController.InputBlocked || Time.timeScale == 0f) return;
        if (prefab.GetComponent<NormalHitSpark>() || prefab.GetComponent<PixelSparkBurst>() || prefab.GetComponent<VoxelImpactBurst>())
        {
            int count = 0; GameObject oldest = null;
            foreach (var item in active)
                if (!item.GetComponent<StrikeAccentBurst>()) { count++; if (!oldest) oldest = item; }
            if (count >= 8) oldest.GetComponent<PooledVFX>().Release();
        }
        var go = PoolFor(prefab).Get();
        var t = go.transform;
        if (go.TryGetComponent<NormalHitSpark>(out var normal))
        {
            t.SetParent(poolRoot, true);
            float direction = directionOverride ?? (attacker ? (targetRoot ? targetRoot.position.x : point.x) - attacker.position.x : 1);
            if (direction == 0) direction = attacker ? attacker.lossyScale.x : 1;
            normal.Emit((Vector3)point + worldOffset, direction);
            return;
        }
        if (go.TryGetComponent<VoxelImpactBurst>(out var voxel))
        {
            t.SetParent(poolRoot, true);
            float direction = directionOverride ?? (attacker ? (targetRoot ? targetRoot.position.x : point.x) - attacker.position.x : -1);
            voxel.Emit((Vector3)point + worldOffset, direction, groundY, voxelSequence++, impactTint ?? Color.clear);
            return;
        }
        if (go.TryGetComponent<PixelSparkBurst>(out var burst))
        {
            t.SetParent(poolRoot, true);
            float direction = attacker ? (targetRoot ? targetRoot.position.x : point.x) - attacker.position.x : 1f;
            if (direction == 0) direction = attacker ? attacker.lossyScale.x : 1f;
            burst.Emit((Vector3)point + worldOffset, direction, groundY);
            return;
        }
        // Retain serialized tuning for legacy animated consumers.
        t.SetParent(parentUnderAttacker && attacker ? attacker : poolRoot, true);
        t.position = (Vector3)point + worldOffset + Vector3.up * Random.Range(verticalJitter.x, verticalJitter.y);
        float face = faceFromAttacker && attacker && attacker.lossyScale.x < 0 ? -1 : 1;
        t.rotation = Quaternion.Euler(0, 0, zRotationOffset + Random.Range(angleJitter.x, angleJitter.y) * face);
        Vector3 scale = prefab.transform.localScale * Random.Range(scaleRange.x, scaleRange.y);
        if (parentUnderAttacker && keepWorldScale && attacker)
        {
            var p = attacker.lossyScale;
            scale = new Vector3(SafeDiv(scale.x, p.x), SafeDiv(scale.y, p.y), SafeDiv(scale.z, p.z));
        }
        if (Random.value < flipXChance) scale.x *= -1;
        if (Random.value < flipYChance) scale.y *= -1;
        t.localScale = scale;
    }

    internal void SpawnAccent(GameObject prefab, Vector3 point, float direction)
    {
        if (!isActiveAndEnabled || !prefab || GamePauseController.InputBlocked || Time.timeScale == 0f) return;
        int count = 0;
        GameObject oldest = null;
        foreach (var item in active)
            if (item.GetComponent<StrikeAccentBurst>()) { count++; if (!oldest) oldest = item; }
        if (count >= 8) oldest.GetComponent<PooledVFX>().Release();
        PoolFor(prefab).Get().GetComponent<StrikeAccentBurst>().Emit(point, direction);
    }
    internal void ClearAccents()
    {
        for (int i = active.Count - 1; i >= 0; i--)
            if (active[i] && (active[i].GetComponent<StrikeAccentBurst>() || active[i].GetComponent<NormalHitSpark>())) active[i].GetComponent<PooledVFX>().Release();
    }
    void OnDisable()
    {
        while (active.Count > 0)
        {
            int last = active.Count - 1;
            var go = active[last]; active.RemoveAt(last);
            if (go) go.GetComponent<PooledVFX>().Release();
        }
        nextAt = 0;
    }
    void OnDestroy()
    {
        foreach (var pool in pools.Values) pool.Clear();
        pools.Clear();
        if (poolRoot) Destroy(poolRoot.gameObject);
    }
    static float SafeDiv(float a, float b) => a / (Mathf.Abs(b) < 1e-5f ? 1 : b);
}
