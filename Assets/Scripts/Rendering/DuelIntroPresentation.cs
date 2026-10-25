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
    readonly ParticleSystem.Particle[] puffs = new ParticleSystem.Particle[12];
    bool enemyVisible, tutorial;
    float hudAlpha;

    void EnsureCanvas()
    {
        if (canvas) return;
        var root = new GameObject("Duel presentation", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler));
        root.transform.SetParent(transform, false);
        canvas = root.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        // Pause remains above the opponent label. The cover is only used during scene changes.
        canvas.sortingOrder = 50;
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
            if (rect) rect.anchoredPosition = start + Vector2.up * (540 * k * k);
            SetCover(k);
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
            SetCover(1 - t / duration);
            yield return null;
        }
        SetCover(0);
    }
    internal void Prepare(DungeonSessionBindings gate, bool isTutorial)
    {
        EnsureCanvas(); SetCover(1);
        tutorial = isTutorial; Running = true; Elapsed = 0;
        enemyVisible = isTutorial; hudAlpha = isTutorial ? 1 : 0;
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
        float t = Mathf.Clamp01(age / duration);
        for (int i = 0; i < puffs.Length; i++)
        {
            float side = i % 2 == 0 ? -1 : 1;
            float width = (4 + i % 3) / 16f;
            puffs[i] = new ParticleSystem.Particle {
                position = dust.transform.position + new Vector3(side * (.03f * (i / 2) + .4f * t),
                    .06f + (i % 3) * .04f + Mathf.Sin(t * Mathf.PI) * .10f, -.05f),
                startSize3D = new Vector3(width, width * .5f, 1),
                startColor = new Color(.48f, .46f, .42f, (1 - t) * .9f),
                startLifetime = duration, remainingLifetime = duration - age + .1f,
                randomSeed = (uint)(i + 1)
            };
        }
        dust.SetParticles(puffs, age >= duration ? 0 : puffs.Length);
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
                    SetCover(1 - Elapsed / reveal);
                    if (Elapsed >= spawn && !dust) CreateDust();
                    PaintDust(Elapsed - spawn, spawnDuration);
                    if (!enemyVisible && Elapsed >= spawn + spawnDuration * .5f) RevealEnemy();
                    float inAlpha = Mathf.Clamp01((Elapsed - title) / .12f);
                    hudAlpha = Mathf.Clamp01((Elapsed - hudStart) / (duration - hudStart));
                    label.alpha = inAlpha * (1 - hudAlpha);
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
        if (!tutorial && enemy && !enemyVisible) HideEnemy();
        foreach (var pair in hud) if (pair.Key) pair.Key.alpha = pair.Value * hudAlpha;
        if (label) label.enabled = !GamePauseController.IsPaused;
    }
    void Finish()
    {
        RevealEnemy();
        foreach (var pair in hud) if (pair.Key) pair.Key.alpha = pair.Value;
        hud.Clear(); enemyRenderers.Clear(); enemy = null;
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
