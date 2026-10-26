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
    TypingManager typing;
    GamePauseController pauseOwner;
    Canvas canvas;
    RectTransform curtain;
    TMP_Text label;
    ParticleSystem dust;
    readonly ParticleSystem.Particle[] puffs = new ParticleSystem.Particle[18];
    readonly HashSet<CanvasGroup> wordHud = new HashSet<CanvasGroup>();
    Mesh smokeMesh;
    TMP_Text enemyHudName;
    float enemyHudNameAlpha, nameAlpha, smokeHeight, wordAlpha;
    static float Ease(float t) { t = Mathf.Clamp01(t); return t * t * (3 - 2 * t); }
    static float Pixel(float position) => Mathf.Round(position / 3) * 3;
    bool enemyVisible, tutorial, covering;
    float hudAlpha;
    double previewSeconds;
    double? previewPrevious;
    const double PreviewDuration = .3;

    void OnEnable() { GamePauseController.FreezeChanged += PreviewFreezeChanged; }
    void OnDisable() { GamePauseController.FreezeChanged -= PreviewFreezeChanged; previewPrevious = null; }
    void PreviewFreezeChanged(bool frozen) { previewPrevious = null; }
    void ResetPreviewClock() { previewSeconds = 0; previewPrevious = null; }

    void EnsureCanvas()
    {
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
        EnsureCanvas(); covering = true;
        // Navigation can begin while the opponent heading is still visible.
        curtain.SetAsLastSibling();
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
        tutorial = isTutorial; Running = true; Elapsed = 0; covering = false;
        ResetPreviewClock();
        enemyVisible = isTutorial; hudAlpha = wordAlpha = isTutorial ? 1 : 0;
        wordHud.Clear(); nameAlpha = 0;
        enemyRenderers.Clear(); hud.Clear(); pauseOwner = null; typing = null;
        foreach (var root in gate.roots)
        {
            if (!enemy) enemy = root.GetComponentInChildren<SkeletonController>(true);
            if (!isTutorial && !typing) typing = root.GetComponentInChildren<TypingManager>(true);
            var pause = root.GetComponentInChildren<GamePauseController>(true);
            if (pause) pauseOwner = pause;
            var status = root.GetComponentInChildren<CombatR7Status>(true);
            if (!isTutorial && status && status.IntroEnemyLabel)
            {
                enemyHudName = status.IntroEnemyLabel;
                enemyHudNameAlpha = enemyHudName.alpha;
                enemyHudName.alpha = 0;
            }
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
        if (typing) typing.BeginIntroPreview();
        if (!isTutorial && enemy) HideEnemy();
    }
    void HideEnemy()
    {
        // Includes contact shadow and any renderers created during actor startup.
        foreach (var renderer in enemy.GetComponentsInChildren<Renderer>(true))
        {
            if (!enemyRenderers.ContainsKey(renderer)) enemyRenderers.Add(renderer, renderer.forceRenderingOff);
            renderer.forceRenderingOff = true;
        }
    }
    void RevealEnemy()
    {
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
        label.enableWordWrapping = false; label.enableAutoSizing = false; label.fontSize = 30;
        var rect = label.rectTransform;
        rect.anchorMin = rect.anchorMax = new Vector2(.5f, .8f);
        rect.pivot = new Vector2(.5f, .5f); rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = new Vector2(700, 72); rect.localScale = Vector3.one;
        var run = EndlessRun.Current;
        label.text = run == null ? "DUEL" : run.Enemy.Name + "\n<size=18>DUEL " + (run.Slot + 1) + "/6</size>";
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
        renderer.renderMode = ParticleSystemRenderMode.Mesh;
        renderer.alignment = ParticleSystemRenderSpace.View;
        smokeMesh = CreateSmokeMesh();
        renderer.mesh = smokeMesh;
        renderer.sortingLayerID = original.sortingLayerID; renderer.sortingOrder = 3;
        var body = enemy.GetComponent<BoxCollider2D>();
        var point = body ? body.bounds.center : enemy.transform.position;
        smokeHeight = body ? body.bounds.size.y : 1;
        point.y = body ? body.bounds.min.y : point.y;
        dust.transform.position = point; dust.Play(false);
    }
    static Mesh CreateSmokeMesh()
    {
        // Five stepped strips, centered in the XY plane; no texture/prefab mutation.
        var vertices = new Vector3[20];
        var triangles = new int[30];
        float[] widths = { .5f, .8f, 1f, .8f, .5f };
        for (int row = 0; row < 5; row++)
        {
            float x = widths[row] * .5f, y = row * .2f - .5f;
            int v = row * 4, t = row * 6;
            vertices[v] = new Vector3(-x, y, 0); vertices[v + 1] = new Vector3(-x, y + .2f, 0);
            vertices[v + 2] = new Vector3(x, y + .2f, 0); vertices[v + 3] = new Vector3(x, y, 0);
            triangles[t] = v; triangles[t + 1] = v + 1; triangles[t + 2] = v + 2;
            triangles[t + 3] = v; triangles[t + 4] = v + 2; triangles[t + 5] = v + 3;
        }
        var mesh = new Mesh { name = "Intro stepped smoke", vertices = vertices, triangles = triangles };
        mesh.RecalculateNormals(); mesh.RecalculateBounds();
        return mesh;
    }
    void PaintDust(float age, float duration)
    {
        if (!dust) return;
        float u = Mathf.Clamp01(age / duration);
        float rise = Ease((u - .12f) / .16f);
        float disperse = Ease((u - .48f) / .52f);
        float opacity = Ease(u / .12f) * (1 - disperse);
        for (int i = 0; i < puffs.Length; i++)
        {
            bool core = i < 6;
            int n = core ? i : i - 6;
            float side = n % 2 == 0 ? -1 : 1;
            // Asymmetric lobes avoid a continuous flat upper edge.
            float x = core ? (n - 2.5f) * .135f : side * (.21f + (n / 2) * .035f);
            float width = core ? .36f + (n % 3) * .025f : .12f + (n % 3) * .025f;
            float height = core ? .30f + (n % 3) * .045f : .13f;
            float y = core ? .17f + (n % 3) * .065f : .07f + (n % 4) * .035f;
            float sx = width * Mathf.Lerp(.55f, 1, rise) * Mathf.Lerp(1, .6f, disperse);
            float sy = height * Mathf.Lerp(.25f, 1, rise) * Mathf.Lerp(1, .25f, disperse);
            float py = Mathf.Lerp(.045f, y, rise) * Mathf.Lerp(1, .25f, disperse);
            Color color = core ? new Color32(167, 156, 136, 255) : new Color32(118, 109, 94, 255);
            color.a = opacity * (core ? .82f : .60f);
            puffs[i] = new ParticleSystem.Particle {
                position = dust.transform.position + smokeHeight * new Vector3(
                    x * Mathf.Lerp(.55f, 1, rise) + side * .20f * disperse, py, -.05f),
                startSize3D = smokeHeight * new Vector3(sx, sy, 1),
                startColor = color, startLifetime = duration, remainingLifetime = duration + 1,
                randomSeed = (uint)(i + 1)
            };
        }
        dust.SetParticles(puffs, u < 1 ? puffs.Length : 0);
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
            float title = shortened ? .42f : 1.10f;
            float titleIn = shortened ? .08f : .10f;
            float titleOut = shortened ? .68f : 1.72f;
            float hudStart = shortened ? .54f : 1.82f;
            float hudEnd = shortened ? .64f : 1.98f;
            float wordStart = shortened ? .62f : 1.90f;
            float wordEnd = shortened ? .7f : 2.1f;
            float duration = shortened ? 1f : 2.4f; // short flow includes its .2s cover
            double previous = Time.realtimeSinceStartupAsDouble;
            while (Elapsed < duration || previewSeconds < PreviewDuration || GamePauseController.PresentationFrozen || (pauseOwner && pauseOwner.IsRestarting))
            {
                double now = Time.realtimeSinceStartupAsDouble;
                float delta = (float)(now - previous); previous = now;
                if (!GamePauseController.PresentationFrozen && (!pauseOwner || !pauseOwner.IsRestarting))
                {
                    Elapsed = Mathf.Min(duration, Elapsed + delta);
                    SetCover(1 - Ease(Elapsed / reveal));
                    if (Elapsed >= spawn && !dust) CreateDust();
                    PaintDust(Elapsed - spawn, spawnDuration);
                    if (!enemyVisible && Elapsed >= spawn + .36f * spawnDuration) RevealEnemy();
                    float inAlpha = Ease((Elapsed - title) / titleIn);
                    float titleEnd = shortened ? .82f : 1.88f;
                    float outAlpha = Ease((Elapsed - titleOut) / (titleEnd - titleOut));
                    nameAlpha = Ease((Elapsed - titleEnd) / .08f);
                    hudAlpha = Ease((Elapsed - hudStart) / (hudEnd - hudStart));
                    wordAlpha = Ease((Elapsed - wordStart) / (wordEnd - wordStart));
                    label.alpha = inAlpha * (1 - outAlpha);
                }
                yield return null;
            }
        }
        // No queued input or OS key repeat from keys held across the handoff.
        DungeonSession.Current.ConsumeIntroInput();
        if (enemy && !isTutorial) enemy.BeginIntroCombat();
        Finish(true);
        GamePauseController.SetIntro(false);
    }
    void LateUpdate()
    {
        if (!Running) return;
        if (!tutorial && enemy && !enemyVisible) HideEnemy();
        foreach (var pair in hud) if (pair.Key) pair.Key.alpha = pair.Value * (wordHud.Contains(pair.Key) ? wordAlpha : hudAlpha);
        // Anchor only after this frame applies the fully visible tray. Never credit
        // the interval that made it appear, even when that frame passed timeline end.
        if (!tutorial && wordAlpha == 1 && !covering &&
            !GamePauseController.PresentationFrozen && (!pauseOwner || !pauseOwner.IsRestarting))
        {
            double now = Time.realtimeSinceStartupAsDouble;
            if (previewPrevious.HasValue) previewSeconds += now - previewPrevious.Value;
            previewPrevious = now;
        }
        else previewPrevious = null;
        if (enemyHudName) enemyHudName.alpha = enemyHudNameAlpha * nameAlpha;
        if (label) label.enabled = !GamePauseController.IsPaused && !covering && (!pauseOwner || !pauseOwner.IsRestarting);
    }
    void Finish(bool combatHandoff = false)
    {
        ResetPreviewClock();
        if (typing) typing.EndIntroPreview(combatHandoff);
        typing = null;
        RevealEnemy();
        foreach (var pair in hud) if (pair.Key) pair.Key.alpha = pair.Value;
        hud.Clear(); wordHud.Clear(); enemyRenderers.Clear(); enemy = null;
        if (enemyHudName) enemyHudName.alpha = enemyHudNameAlpha;
        enemyHudName = null;
        if (dust) Destroy(dust.gameObject);
        if (label) Destroy(label.gameObject);
        if (smokeMesh) Destroy(smokeMesh);
        smokeMesh = null;
        dust = null; label = null; Running = false; SetCover(0);
    }
    void OnDestroy() { if (smokeMesh) Destroy(smokeMesh); }
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
