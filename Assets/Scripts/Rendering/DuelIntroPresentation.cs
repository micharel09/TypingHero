using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Session-owned presentation only. Actors, input, statistics and combat keep their owners.
[DefaultExecutionOrder(32000)]
internal sealed class DuelIntroPresentation : MonoBehaviour
{
    internal bool Running { get; private set; }
    internal float Elapsed { get; private set; }
    readonly Dictionary<Renderer, bool> enemyRenderers = new Dictionary<Renderer, bool>();
    readonly Dictionary<CanvasGroup, float> hud = new Dictionary<CanvasGroup, float>();
    SkeletonController enemy;
    GamePauseController pauseOwner;
    Canvas canvas;
    RectTransform curtain;
    TMP_Text label;
    ParticleSystem dust;
    readonly ParticleSystem.Particle[] puffs = new ParticleSystem.Particle[18];
    static readonly int IntroReveal = Shader.PropertyToID("_IntroReveal");
    readonly Dictionary<Renderer, Vector4> revealState = new Dictionary<Renderer, Vector4>();
    readonly HashSet<CanvasGroup> wordHud = new HashSet<CanvasGroup>();
    MaterialPropertyBlock revealBlock;
    float revealBottom, revealTop, revealProgress, wordAlpha;
    bool revealBoundsReady;
    static float Ease(float t) { t = Mathf.Clamp01(t); return t * t * (3 - 2 * t); }
    static float Pixel(float position) => Mathf.Round(position / 3) * 3;
    bool enemyVisible, tutorial;
    float hudAlpha;

    void EnsureCanvas()
    {
        if (revealBlock == null) revealBlock = new MaterialPropertyBlock();
        if (canvas) return;
        var root = new GameObject("Duel presentation", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler));
        root.transform.SetParent(transform, false);
        canvas = root.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        // Pause remains above the opponent label. The cover is only used during scene changes.
        canvas.sortingOrder = 39; // Authored Pause canvas is40 and must remain fully visible.
        var scaler = root.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(960, 540);
        scaler.matchWidthOrHeight = .5f;
        var cover = new GameObject("Scene cover", typeof(RectTransform), typeof(Image));
        cover.transform.SetParent(root.transform, false);
        curtain = (RectTransform)cover.transform;
        curtain.anchorMin = Vector2.zero; curtain.anchorMax = Vector2.one;
        curtain.offsetMin = curtain.offsetMax = Vector2.zero;
        var image = cover.GetComponent<Image>();
        image.color = new Color32(13, 12, 15, 255); image.raycastTarget = false;
        SetCover(0);
    }
    void SetCover(float amount)
    {
        curtain.anchorMin = new Vector2(0, 1 - Mathf.Clamp01(amount));
        curtain.anchorMax = Vector2.one;
        curtain.offsetMin = curtain.offsetMax = Vector2.zero;
        curtain.gameObject.SetActive(amount > 0);
    }
    internal IEnumerator Cover(float duration, bool liftMenu)
    {
        EnsureCanvas();
        var menu = liftMenu ? FindObjectOfType<MainMenuUI>() : null;
        var rect = menu ? menu.IntroMenuRoot : null;
        var start = rect ? rect.anchoredPosition : Vector2.zero;
        double previous = Time.realtimeSinceStartupAsDouble;
        for (float t = 0; t < duration;)
        {
            double now = Time.realtimeSinceStartupAsDouble;
            t += (float)(now - previous); previous = now;
            float k = Mathf.Clamp01(t / duration);
            if (rect) rect.anchoredPosition = start + Vector2.up * Pixel(540 * Ease(k));
            SetCover(Ease(k));
            // Close upward with the departing menu; uncover also lifts upward.
            curtain.anchorMin = Vector2.zero;
            curtain.anchorMax = new Vector2(1, Ease(k));
            yield return null;
        }
        SetCover(1);
    }
    internal IEnumerator Uncover(float duration)
    {
        double previous = Time.realtimeSinceStartupAsDouble;
        for (float t = 0; t < duration;)
        {
            double now = Time.realtimeSinceStartupAsDouble;
            float delta = (float)(now - previous); previous = now;
            if (!GamePauseController.IsPaused && (!pauseOwner || !pauseOwner.IsRestarting)) t += delta;
            SetCover(1 - Ease(t / duration));
            yield return null;
        }
        SetCover(0);
    }
    internal void Prepare(DungeonSessionBindings gate, bool isTutorial)
    {
        EnsureCanvas(); SetCover(1);
        tutorial = isTutorial; Running = true; Elapsed = 0;
        enemyVisible = isTutorial; hudAlpha = wordAlpha = isTutorial ? 1 : 0;
        revealProgress = 0; revealBoundsReady = false; wordHud.Clear(); revealState.Clear();
        enemyRenderers.Clear(); hud.Clear(); pauseOwner = null;
        foreach (var root in gate.roots)
        {
            if (!enemy) enemy = root.GetComponentInChildren<SkeletonController>(true);
            var pause = root.GetComponentInChildren<GamePauseController>(true);
            if (pause) pauseOwner = pause;
            if (!isTutorial && pause && pause.IntroHud != null)
                foreach (var group in pause.IntroHud)
                    if (group && !hud.ContainsKey(group)) { hud.Add(group, group.alpha); group.alpha = 0; }
        }
        if (!isTutorial)
        {
            var layout = gate.GetComponentInChildren<CombatHUDLayout>(true);
            if (!layout) foreach (var root in gate.roots)
            { layout = root.GetComponentInChildren<CombatHUDLayout>(true); if (layout) break; }
            if (layout && layout.IntroTypingTray)
                foreach (var group in hud.Keys)
                    if (group.transform == layout.IntroTypingTray || group.transform.IsChildOf(layout.IntroTypingTray)) wordHud.Add(group);
        }
        if (!isTutorial && enemy) HideEnemy();
    }
    void HideEnemy()
    {
        // Includes contact shadow and any renderers created during actor startup.
        foreach (var renderer in enemy.GetComponentsInChildren<Renderer>(true))
        {
            if (!enemyRenderers.ContainsKey(renderer)) enemyRenderers.Add(renderer, renderer.forceRenderingOff);
            renderer.forceRenderingOff = true;
            if (!revealState.ContainsKey(renderer) && renderer.sharedMaterial && renderer.sharedMaterial.HasProperty(IntroReveal))
            {
                renderer.GetPropertyBlock(revealBlock);
                revealState.Add(renderer, revealBlock.GetVector(IntroReveal));
            }
        }
    }
    void PaintEnemyReveal(float progress)
    {
        revealProgress = progress;
        if (progress <= 0) return;
        if (!revealBoundsReady)
        {
            // Stable body landmarks exclude transparent cel padding, so the visible
            // body receives the full120ms rather than a near-instant middle slice.
            var body = enemy.GetComponent<BoxCollider2D>();
            revealBottom = body.bounds.min.y - .04f;
            revealTop = body.bounds.max.y + .08f;
            revealBoundsReady = true;
        }
        foreach (var pair in revealState) if (pair.Key)
        {
            pair.Key.GetPropertyBlock(revealBlock);
            float height = Mathf.Lerp(revealBottom, revealTop, Ease(progress));
            revealBlock.SetVector(IntroReveal, new Vector4(1, Mathf.Round(height * 32) / 32, 0, 0));
            pair.Key.SetPropertyBlock(revealBlock);
            pair.Key.forceRenderingOff = enemyRenderers[pair.Key];
        }
        if (progress >= 1) RevealEnemy();
    }
    void RevealEnemy()
    {
        foreach (var pair in revealState) if (pair.Key)
        {
            pair.Key.GetPropertyBlock(revealBlock);
            revealBlock.SetVector(IntroReveal, pair.Value);
            pair.Key.SetPropertyBlock(revealBlock);
        }
        foreach (var pair in enemyRenderers) if (pair.Key) pair.Key.forceRenderingOff = pair.Value;
        enemyVisible = true;
    }
    void CreateLabel()
    {
        var source = FindObjectOfType<TutorialOverlay>(true);
        var template = source ? source.transform.Find("Reference960x540/TutorialPrompt/Heading") : null;
        if (!template) throw new System.InvalidOperationException("Duel intro requires the authored heading style");
        label = Instantiate(template.gameObject, canvas.transform).GetComponent<TMP_Text>();
        label.name = "Opponent label"; label.gameObject.SetActive(true);
        label.raycastTarget = false; label.alignment = TextAlignmentOptions.Center;
        label.enableWordWrapping = false; label.enableAutoSizing = false; label.fontSize = 24;
        var rect = label.rectTransform;
        rect.anchorMin = rect.anchorMax = new Vector2(.5f, .8f);
        rect.pivot = new Vector2(.5f, .5f); rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = new Vector2(700, 72); rect.localScale = Vector3.one;
        var run = EndlessRun.Current;
        label.text = run == null ? "DUEL" : run.Enemy.Name + "\n<size=65%>DUEL " + (run.Slot + 1) + "/6</size>";
        label.alpha = 0;
    }
    void CreateDust()
    {
        var accents = FindObjectOfType<NormalStrikeEffects>();
        var template = accents ? accents.IntroDustTemplate : null;
        if (!template || !enemy) throw new System.InvalidOperationException("Duel intro requires the existing dust template and opponent");
        // A private spawn variant reuses the strike dust material/rendering without
        // touching its prefab, pool, damage flash, lighting or camera impact.
        var root = new GameObject("Opponent spawn dust"); root.transform.SetParent(transform, false);
        dust = root.AddComponent<ParticleSystem>(); dust.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        var main = dust.main; main.loop = false; main.playOnAwake = false;
        main.simulationSpeed = 0; main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.startSize3D = true; main.maxParticles = puffs.Length;
        var emission = dust.emission; emission.enabled = false;
        var shape = dust.shape; shape.enabled = false;
        var renderer = dust.GetComponent<ParticleSystemRenderer>();
        var original = template.GetComponent<ParticleSystemRenderer>();
        renderer.sharedMaterial = original.sharedMaterial;
        renderer.renderMode = original.renderMode;
        renderer.sortingLayerID = original.sortingLayerID; renderer.sortingOrder = 3;
        var body = enemy.GetComponent<BoxCollider2D>();
        var point = body ? body.bounds.center : enemy.transform.position;
        point.y = body ? body.bounds.min.y + .12f : point.y;
        dust.transform.position = point; dust.Play(false);
    }
    void PaintDust(float age, float duration)
    {
        if (!dust) return;
        int count = 0;
        for (int i = 0; i < puffs.Length; i++)
        {
            int cluster = i / 6;
            float delay = (cluster == 0 ? 0 : cluster == 1 ? .055f : .12f) + (i % 3) * .009f;
            float life = duration - delay;
            float t = (age - delay) / life;
            if (t < 0 || t >= 1) continue;
            float side = i % 2 == 0 ? -1 : 1;
            float spread = cluster == 0 ? .26f : cluster == 1 ? .48f : .34f;
            float width = (2 + (i * 7) % 3) / 32f;
            float outward = 1 - Mathf.Pow(1 - t, 2);
            float alpha = Mathf.Clamp01(t / .10f) * Mathf.Pow(1 - t, cluster == 2 ? .8f : 1.4f);
            puffs[count++] = new ParticleSystem.Particle {
                position = dust.transform.position + new Vector3(side * (.035f * ((i * 5) % 6) + spread * outward),
                    .015f + ((i * 7) % 5) * .022f + Mathf.Sin(t * Mathf.PI) * (cluster == 0 ? .13f : .07f), -.05f),
                startSize3D = new Vector3(width * (1 + .3f * t), width * (.65f + .16f * t), 1),
                startColor = new Color(.48f, .46f, .42f, alpha * (cluster == 0 ? .65f : .45f)),
                startLifetime = life, remainingLifetime = life * (1 - t) + .1f,
                randomSeed = (uint)(i + 1)
            };
        }
        dust.SetParticles(puffs, count);
    }
    internal IEnumerator Play(bool shortened, bool isTutorial)
    {
        // Start/configuration run while the scene is covered and gameplay is locked.
        yield return null;
        if (isTutorial)
        {
            yield return Uncover(.45f);
        }
        else
        {
            CreateLabel();
            float reveal = shortened ? .2f : .45f;
            float spawn = shortened ? .2f : .85f;
            float spawnDuration = shortened ? .4f : .5f;
            float title = shortened ? .6f : 1.35f;
            float hudStart = shortened ? .8f : 2f;
            float duration = shortened ? 1f : 2.4f; // short flow includes its .2s cover
            double previous = Time.realtimeSinceStartupAsDouble;
            while (Elapsed < duration)
            {
                double now = Time.realtimeSinceStartupAsDouble;
                float delta = (float)(now - previous); previous = now;
                if (!GamePauseController.PresentationFrozen && (!pauseOwner || !pauseOwner.IsRestarting))
                {
                    Elapsed = Mathf.Min(duration, Elapsed + delta);
                    SetCover(1 - Ease(Elapsed / reveal));
                    if (Elapsed >= spawn && !dust) CreateDust();
                    PaintDust(Elapsed - spawn, spawnDuration);
                    if (!enemyVisible) PaintEnemyReveal(Mathf.Clamp01((Elapsed - (shortened ? .34f : 1.04f)) / .12f));
                    float inAlpha = Ease((Elapsed - title) / .12f);
                    hudAlpha = Ease((Elapsed - hudStart) / (duration - hudStart));
                    wordAlpha = Ease((Elapsed - hudStart - .08f) / (duration - hudStart - .08f));
                    label.alpha = inAlpha * (1 - hudAlpha);
                    label.rectTransform.anchoredPosition = Vector2.up * Pixel(-6 * (1 - inAlpha) + 6 * hudAlpha);
                }
                yield return null;
            }
        }
        // No queued input or OS key repeat from keys held across the handoff.
        DungeonSession.Current.ConsumeIntroInput();
        if (enemy && !isTutorial) enemy.BeginIntroCombat();
        Finish();
        GamePauseController.SetIntro(false);
    }
    void LateUpdate()
    {
        if (!Running) return;
        if (!tutorial && enemy && !enemyVisible) { HideEnemy(); PaintEnemyReveal(revealProgress); }
        foreach (var pair in hud) if (pair.Key) pair.Key.alpha = pair.Value * (wordHud.Contains(pair.Key) ? wordAlpha : hudAlpha);
        if (label) label.enabled = !GamePauseController.IsPaused;
    }
    void Finish()
    {
        RevealEnemy();
        foreach (var pair in hud) if (pair.Key) pair.Key.alpha = pair.Value;
        hud.Clear(); wordHud.Clear(); revealState.Clear(); enemyRenderers.Clear(); enemy = null;
        if (dust) Destroy(dust.gameObject);
        if (label) Destroy(label.gameObject);
        dust = null; label = null; Running = false; SetCover(0);
    }
    internal void ClearCovered()
    {
        Finish(); SetCover(1);
    }
    internal void Cancel()
    {
        if (canvas) Finish();
        GamePauseController.SetIntro(false);
    }
}
