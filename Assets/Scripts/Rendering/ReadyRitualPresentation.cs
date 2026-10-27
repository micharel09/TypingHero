using System;
using System.Collections.Generic;
using UnityEngine;

// Retained by DungeonSession: no reference to a dead opponent is needed by the seal.
[DefaultExecutionOrder(32001)]
internal sealed class ReadyRitualPresentation : MonoBehaviour
{
    enum Phase { Dormant, Arriving, Waiting, Charged, Departing, Summoning }
    Phase phase;
    internal bool Committed => phase == Phase.Departing;
    DungeonArenaCamera arena;
    GamePauseController pause;
    TypingManager typing;
    Action continuation;
    Vector3 foot;
    Vector3 groundFoot, projectedCenter;
    readonly Vector3[] markOffsets = new Vector3[5];
    Camera view;
    Plane floor;
    float projectedHeight;
    float height, savedGain = 1, elapsed, resetDuration = .08f;
    int progress;
    bool chargePainted;
    double? previous;
    Material material;
    SfxEvent completionCue;
    ParticleSystem ring, marks;
    Mesh ringMesh, markMesh;
    readonly float[] levels = new float[5];
    readonly ParticleSystem.Particle[] ringParticle = new ParticleSystem.Particle[1];
    readonly ParticleSystem.Particle[] markParticles = new ParticleSystem.Particle[5];
    static readonly Color Amber = new Color32(229, 190, 114, 255);
    const string Channel = "duel-seal";
    static float Ease(float t) { t = Mathf.Clamp01(t); return t * t * (3 - 2 * t); }

    void OnEnable() => GamePauseController.FreezeChanged += FreezeChanged;
    void FreezeChanged(bool _) => previous = null;
    internal void CaptureSpawn(SkeletonController enemy, DungeonArenaCamera camera, float floorY)
    {
        arena = camera;
        var body = enemy.GetComponent<BoxCollider2D>();
        var bounds = body.bounds;
        foot = new Vector3(bounds.center.x, bounds.min.y, enemy.transform.position.z + .04f);
        groundFoot = new Vector3(foot.x, floorY + .015f, foot.z);
        view = camera.GetComponent<Camera>();
        height = bounds.size.y;
        var template = FindObjectOfType<NormalStrikeEffects>().IntroDustTemplate;
        material = template.GetComponent<ParticleSystemRenderer>().sharedMaterial;
        completionCue = Resources.Load<SfxEvent>("SummonSeal");
    }
    internal void Begin(TypingManager source, Action onReady)
    {
        Cancel();
        savedGain = arena.EnvironmentPresentationGain;
        typing = source; continuation = onReady;
        pause = FindObjectOfType<GamePauseController>();
        phase = Phase.Arriving; elapsed = 0; previous = null; progress = 0;
        Array.Clear(levels, 0, levels.Length);
        typing.ReadyInputChanged += InputChanged;
        ringMesh = CreateRingMesh();
        markMesh = DuelIntroPresentation.CreateSmokeMesh();
        // Inverse-project the pixel drawing onto the actual floor. A view-facing
        // ellipse puts its lower half underground and loses two of the five marks.
        floor = new Plane(Vector3.up, groundFoot);
        projectedCenter = view.WorldToViewportPoint(groundFoot);
        projectedHeight = view.WorldToViewportPoint(groundFoot + Vector3.up * height).y - projectedCenter.y;
        PlaceOnFloor(ringMesh, new Vector2(.9f, .22f));
        PlaceOnFloor(markMesh, new Vector2(.09f, .07f));
        for (int i = 0; i < markOffsets.Length; i++)
        {
            float angle = (90 - i * 72) * Mathf.Deg2Rad;
            markOffsets[i] = FloorOffset(Mathf.Round(Mathf.Cos(angle) * 18) * .02f,
                Mathf.Round(Mathf.Sin(angle) * 4) * .02f);
        }
        ring = CreateParticles("Summoning ring", ringMesh, 1);
        marks = CreateParticles("Five seal marks", markMesh, 5);
        Paint(0);
    }
    ParticleSystem CreateParticles(string title, Mesh mesh, int count)
    {
        var root = new GameObject(title); root.transform.SetParent(transform, false);
        var effect = root.AddComponent<ParticleSystem>();
        effect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        var main = effect.main;
        main.playOnAwake = main.loop = false; main.simulationSpeed = 0;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.startSize3D = true; main.maxParticles = count;
        var emission = effect.emission; emission.enabled = false;
        var shape = effect.shape; shape.enabled = false;
        var renderer = effect.GetComponent<ParticleSystemRenderer>();
        renderer.sharedMaterial = material;
        renderer.renderMode = ParticleSystemRenderMode.Mesh;
        renderer.alignment = ParticleSystemRenderSpace.World;
        renderer.mesh = mesh; renderer.sortingOrder = -1;
        effect.Play(false);
        return effect;
    }
    Vector3 FloorOffset(float x, float y)
    {
        var point = projectedCenter + new Vector3(x * projectedHeight / view.aspect, y * projectedHeight, 0);
        var ray = view.ViewportPointToRay(point);
        floor.Raycast(ray, out float distance);
        return ray.GetPoint(distance) - groundFoot;
    }
    void PlaceOnFloor(Mesh mesh, Vector2 size)
    {
        var vertices = mesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
            vertices[i] = FloorOffset(vertices[i].x * size.x, vertices[i].y * size.y);
        mesh.vertices = vertices; mesh.RecalculateNormals(); mesh.RecalculateBounds();
    }
    static Mesh CreateRingMesh()
    {
        // A raster ellipse made of solid cells. No antialiasing, rotating UV or new texture.
        var vertices = new List<Vector3>(); var indices = new List<int>();
        for (int y = 0; y < 11; y++)
        for (int x = 0; x < 45; x++)
        {
            float px = (x - 22) / 22.5f, py = (y - 5) / 5.5f;
            float outer = px * px + py * py;
            float inner = px * px / (.91f * .91f) + py * py / (.64f * .64f);
            if (outer > 1 || inner < 1) continue;
            int v = vertices.Count;
            float left = x / 45f - .5f, bottom = y / 11f - .5f;
            vertices.Add(new Vector3(left, bottom, 0)); vertices.Add(new Vector3(left, bottom + 1f / 11, 0));
            vertices.Add(new Vector3(left + 1f / 45, bottom + 1f / 11, 0)); vertices.Add(new Vector3(left + 1f / 45, bottom, 0));
            indices.AddRange(new[] { v, v + 1, v + 2, v, v + 2, v + 3 });
        }
        var mesh = new Mesh { name = "Pixel summoning ellipse", vertices = vertices.ToArray(), triangles = indices.ToArray() };
        mesh.RecalculateNormals(); mesh.RecalculateBounds(); return mesh;
    }
    void InputChanged(int count, bool error)
    {
        progress = count;
        resetDuration = error ? .12f : .08f;
    }
    void Charge()
    {
        if (phase != Phase.Waiting) return;
        phase = Phase.Charged; elapsed = 0; chargePainted = false; previous = null;
    }
    void LateUpdate()
    {
        if (phase == Phase.Dormant || phase == Phase.Summoning) return;
        if (pause && pause.IsRestarting) { Cancel(); return; }
        if (GamePauseController.PresentationFrozen) { previous = null; return; }
        double now = Time.realtimeSinceStartupAsDouble;
        float delta = previous.HasValue ? (float)(now - previous.Value) : 0;
        previous = now;
        elapsed += delta;
        if (phase == Phase.Arriving)
        {
            arena.EnvironmentPresentationGain = savedGain * Mathf.Lerp(1, .85f, Ease(elapsed / .24f));
            Paint(Ease(elapsed / .24f));
            if (elapsed >= .24f)
            {
                phase = Phase.Waiting; elapsed = 0;
                typing.BeginReady(Charge);
            }
            return;
        }
        for (int i = 0; i < levels.Length; i++)
            levels[i] = Mathf.MoveTowards(levels[i], i < progress ? 1 : 0, delta / resetDuration);
        if (phase == Phase.Charged)
        {
            // Anchor only after applying the fully charged seal, never to the input batch.
            if (!chargePainted)
            {
                Array.Fill(levels, 1f); elapsed = 0; chargePainted = true;
                if (SfxPlayer.I && AudioPreferences.Sfx > 0)
                    SfxPlayer.I.PlayOnChannel(completionCue, foot, null, Channel, transform, 0);
            }
            Paint(1);
            if (elapsed >= .18f)
            {
                phase = Phase.Departing;
                typing.ReadyInputChanged -= InputChanged; typing = null;
                var next = continuation; continuation = null;
                next?.Invoke();
            }
            return;
        }
        Paint(1);
    }
    void Paint(float opacity)
    {
        if (!ring || !marks) return;
        float breath = BrowserMotion.Reduced || phase != Phase.Waiting ? 0 : .06f * Mathf.Sin(elapsed * Mathf.PI * 2 / 1.6f);
        Color color = Amber; color.a = opacity * (.44f + breath);
        ringParticle[0] = Particle(groundFoot, Vector3.one, color);
        ring.SetParticles(ringParticle, 1);
        for (int i = 0; i < markParticles.Length; i++)
        {
            color.a = opacity * Mathf.Lerp(.24f + breath, 1, levels[i]);
            markParticles[i] = Particle(groundFoot + markOffsets[i], Vector3.one, color);
        }
        marks.SetParticles(markParticles, markParticles.Length);
    }
    static ParticleSystem.Particle Particle(Vector3 position, Vector3 size, Color color)
        => new ParticleSystem.Particle { position = position, startSize3D = size, startColor = color,
            startLifetime = 1000, remainingLifetime = 1000, randomSeed = 1 };
    internal void BeginSummon()
    {
        phase = Phase.Summoning;
        if (arena) arena.EnvironmentPresentationGain = savedGain * .85f;
    }
    internal void PaintSummon(float time)
    {
        if (phase != Phase.Summoning) return;
        arena.EnvironmentPresentationGain = savedGain * Mathf.Lerp(.85f, 1, Ease(time / .6f));
        Paint(1 - Ease((time - .2f) / .4f));
        if (time >= .6f) Cancel();
    }
    internal void Cancel()
    {
        bool active = phase != Phase.Dormant;
        phase = Phase.Dormant; continuation = null; previous = null;
        if (typing)
        {
            typing.ReadyInputChanged -= InputChanged;
            typing.CancelReady();
        }
        typing = null;
        if (active && arena) arena.EnvironmentPresentationGain = savedGain;
        if (SfxPlayer.I) SfxPlayer.I.FadeOutChannel(transform, Channel, 0);
        if (ring) { ring.gameObject.SetActive(false); Destroy(ring.gameObject); }
        if (marks) { marks.gameObject.SetActive(false); Destroy(marks.gameObject); }
        if (ringMesh) Destroy(ringMesh);
        if (markMesh) Destroy(markMesh);
        ring = marks = null; ringMesh = markMesh = null;
    }
    void OnDisable()
    {
        GamePauseController.FreezeChanged -= FreezeChanged;
        Cancel();
    }
}
