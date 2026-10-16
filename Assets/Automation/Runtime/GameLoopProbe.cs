#if TYPINGHERO_AUTOMATION
using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Observation only: no command dispatcher, setters, or input injection.
public sealed class GameLoopProbe : MonoBehaviour
{
    [Serializable] sealed class State
    {
        public string scene, current, next, playerName;
        public bool inputBlocked;
        public float multiplier, timer;
        public int frame, score, hp, enemyHp, words, chars, parries, errors, objects;
        public bool slayer, parryWindow, canParry, enemyAttacking, overlay, paused;
        public float enemyAnimationTime, frameMs, scroll;
        public long memory;
        public int rows, pool;
        public Point[] controls;
    }
    [Serializable] sealed class Point { public string name, kind; public float x, y; }
    int words, chars, parries, errors;
    ParrySystem observedParry;
    static readonly BindingFlags Fields = BindingFlags.Instance | BindingFlags.NonPublic;
    static T Read<T>(object target, string field, T fallback = default)
        => target == null ? fallback : (T)target.GetType().GetField(field, Fields).GetValue(target);
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")] static extern void TypingHeroQA_Publish(string json);
#endif
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Install()
    {
        var go = new GameObject("TypingHero QA observation");
        DontDestroyOnLoad(go);
        go.AddComponent<GameLoopProbe>();
    }
    void OnEnable()
    {
        TypingManager.OnWordAdvanced += Word;
        TypingManager.OnCorrectChar += Char;
        Application.logMessageReceived += Log;
        StartCoroutine(Publish());
    }
    void OnDisable()
    {
        TypingManager.OnWordAdvanced -= Word;
        TypingManager.OnCorrectChar -= Char;
        Application.logMessageReceived -= Log;
        if (observedParry) observedParry.OnParrySuccess -= Parry;
    }
    void Word() => words++;
    void Char(char _) => chars++;
    void Parry(ParrySystem.ParryContext _) => parries++;
    void Log(string _, string trace, LogType type)
    {
        if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert) errors++;
    }
    IEnumerator Publish()
    {
        var wait = new WaitForSecondsRealtime(.1f);
        while (true)
        {
            var typing = FindObjectOfType<TypingManager>();
            var health = FindObjectOfType<PlayerHealth>();
            var enemy = FindObjectOfType<SkeletonController>();
            var parry = FindObjectOfType<ParrySystem>();
            if (observedParry != parry)
            {
                if (observedParry) observedParry.OnParrySuccess -= Parry;
                observedParry = parry;
                if (observedParry) observedParry.OnParrySuccess += Parry;
            }
            var slayer = FindObjectOfType<PlayerSlayerMode>();
            var gate = FindObjectOfType<PlayerInputGate>();
            var pause = FindObjectOfType<GamePauseController>();
            var board = FindObjectOfType<LeaderboardUIBinder>();
            var pool = Read<System.Collections.Generic.List<LeaderboardRowView>>(board, "pool");
            var scroll = Read<ScrollRect>(board, "scrollRect");
            var animator = Read<Animator>(enemy, "animator");
            var info = animator ? animator.GetCurrentAnimatorStateInfo(0) : default;
            var controls = FindObjectsOfType<Selectable>().Where(s => s.IsInteractable()).Select(s =>
            {
                var canvas = s.GetComponentInParent<Canvas>();
                var rt = (RectTransform)s.transform;
                var p = RectTransformUtility.WorldToScreenPoint(
                    canvas && canvas.renderMode != RenderMode.ScreenSpaceOverlay ? canvas.worldCamera : null,
                    rt.TransformPoint(rt.rect.center));
                return new Point { name = s.name, kind = s is TMP_InputField ? "input" : "button",
                    x = p.x / Screen.width, y = 1f - p.y / Screen.height };
            }).ToList();
            if (scroll && scroll.viewport)
            {
                var canvas = scroll.GetComponentInParent<Canvas>();
                var p = RectTransformUtility.WorldToScreenPoint(
                    canvas && canvas.renderMode != RenderMode.ScreenSpaceOverlay ? canvas.worldCamera : null,
                    scroll.viewport.TransformPoint(scroll.viewport.rect.center));
                controls.Add(new Point { name = scroll.name, kind = "scroll", x = p.x / Screen.width,
                    y = 1f - p.y / Screen.height });
            }
            var state = new State
            {
                scene = SceneManager.GetActiveScene().name, frame = Time.frameCount,
                current = Read(typing, "current", ""), next = Read(typing, "next", ""),
                playerName = PlayerIdentity.Name, score = ScoreSystem.I ? ScoreSystem.I.Score : 0,
                hp = health ? health.Current : -1, enemyHp = enemy ? enemy.Current : -1,
                words = words, chars = chars, parries = parries, errors = errors,
                slayer = slayer && slayer.IsActive, parryWindow = parry && parry.IsWindowActive,
                canParry = gate && gate.CanParry, enemyAttacking = Read(enemy, "_attacking", false),
                enemyAnimationTime = info.normalizedTime, overlay = Read(pause, "_overlayLocked", false),
                paused = Read(pause, "_paused", false), inputBlocked = GamePauseController.InputBlocked,
                multiplier = ScoreSystem.I ? ScoreSystem.I.Multiplier : 0, timer = Read(typing, "timer", 0f), frameMs = Time.unscaledDeltaTime * 1000,
                memory = Profiler.GetTotalAllocatedMemoryLong(), objects = Resources.FindObjectsOfTypeAll<GameObject>().Length,
                rows = pool == null ? 0 : pool.Count(r => r && r.gameObject.activeInHierarchy),
                pool = pool == null ? 0 : pool.Count, scroll = scroll ? scroll.verticalNormalizedPosition : -1,
                controls = controls.ToArray()
            };
#if UNITY_WEBGL && !UNITY_EDITOR
            TypingHeroQA_Publish(JsonUtility.ToJson(state));
#endif
            yield return wait;
        }
    }
}
#endif
