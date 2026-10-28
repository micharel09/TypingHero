using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[DefaultExecutionOrder(1100)]
public sealed class TutorialOverlay : MonoBehaviour
{
    internal event Action Unavailable;
    [Serializable]
    internal sealed class Arrow
    {
        public RectTransform source, target, shaft, tipA, tipB;
        public Vector2 sourceAnchor, targetAnchor;
    }
    [SerializeField] Button continueButton;
    // Retained builder bindings for the live typing and guard HUD.
    [SerializeField] RectTransform typingTray, staminaTarget;
    [SerializeField] Arrow[] arrows = Array.Empty<Arrow>();
    [SerializeField] GameObject parryModal;
    [SerializeField] GameObject[] hudContent;
    [SerializeField] Button backButton, startButton;
    [SerializeField] Button watchButton, skipButton, practiceBack, practiceAction;
    [SerializeField] GameObject practicePanel;
    [SerializeField] TMP_Text demoBadge, practiceMessage, practiceBadge, readiness;
    [SerializeField] RectTransform practiceFocus;
    readonly Vector3[] focusCorners = new Vector3[4];
    RectTransform practiceTarget;
    bool demoPlaying, inPractice;
    double totalDuration;
    [SerializeField] Image clipImage;
    [SerializeField] TMP_Text clipCaption;
    [SerializeField] TMP_Text clipRule;
    [SerializeField] RectTransform captionPanel, clipFocus;
    [SerializeField] RectTransform guideShaft, guideTipA, guideTipB;
    [SerializeField] RectTransform guideBend, guideEnd;
    [Serializable]
    internal sealed class Guidance
    {
        // Top-left coordinates in the 640x360 clip, independent of viewport scale.
        public Rect focus;
        public Vector2 label;
    }
    [Serializable]
    internal sealed class Lesson
    {
        public string title;
        public string rule;
        public string[] atlasPaths;
        public int[] frameAtlas, frameSprite;
        public float[] durations;
        public string[] captions;
        public Guidance[] guidance;
    }
    [SerializeField] Lesson[] lessons = Array.Empty<Lesson>();
    [SerializeField] TMP_Text clipTitle, modalStep, hudStep, actionLabel;
    readonly TutorialAtlasCache atlasCache = new TutorialAtlasCache();
    bool frameReady, waitingForFrame, waitingForLoop;
    string displayedAtlas;
    double clipTime, pendingTime, finalFrameElapsed;
    enum LoopPhase { Playing, FadeOut, FadeIn }
    LoopPhase loopPhase;
    float loopElapsed;
    const float LoopHalfDuration = .1f;
    int step;
    int displayedFrame = -1;
    internal int StepCount => lessons.Length;
    void OnEnable()
    {
        ShowStep(0);
#if TYPINGHERO_AUTOMATION
        Canvas.willRenderCanvases += TraceRender;
#endif
    }

    internal void BindBack(UnityEngine.Events.UnityAction action)
    {
        backButton.onClick.AddListener(action);
        practiceBack.onClick.AddListener(action);
        practiceAction.onClick.AddListener(SubmitPractice);
    }
    void SubmitPractice() => startButton.onClick.Invoke();
    internal void BindLearning(UnityEngine.Events.UnityAction watch, UnityEngine.Events.UnityAction skip)
    {
        if (watch != null) watchButton.onClick.AddListener(watch);
        skipButton.onClick.AddListener(skip);
    }
    internal void SetPrimary(string text, bool enabled)
    {
        actionLabel.text = text;
        startButton.interactable = enabled;
        practiceAction.GetComponentInChildren<TMP_Text>().text = text;
        practiceAction.interactable = enabled;
    }
    internal void SetReadiness(bool typing, bool parry) => readiness.text = "HIT: " +
        (typing ? "OK" : "Not yet") + "\nPARRY: " + (parry ? "OK" : "Not yet");
    internal void PlayDemo()
    {
        demoPlaying = true;
        demoBadge.text = "WATCH ONLY";
        watchButton.GetComponentInChildren<TMP_Text>().text = "Replay";
    }
    internal void ShowPractice(int index, string message, bool finished)
    {
        ReleaseClip();
        step = index; inPractice = true;
        parryModal.SetActive(false);
        foreach (var node in hudContent) node.SetActive(false);
        foreach (var arrow in arrows)
        { arrow.shaft.gameObject.SetActive(false); arrow.tipA.gameObject.SetActive(false); arrow.tipB.gameObject.SetActive(false); }
        practicePanel.SetActive(true);
        practiceBadge.text = "YOUR TURN \u00b7 " + (step + 1) + "/" + StepCount + (step == 3 ? " \u00b7 No HP loss" : " \u00b7 No timer");
        practiceMessage.text = message;
        practiceTarget = index == 1 ? typingTray : finished && message.StartsWith("Parry successful", StringComparison.Ordinal) ? staminaTarget : null;
        practiceFocus.gameObject.SetActive(practiceTarget);
        UpdatePracticeFocus();
        practiceAction.interactable = finished;
        practiceAction.GetComponentInChildren<TMP_Text>().text = finished ? "Next \u00b7 Enter" : "Practicing...";
        watchButton.gameObject.SetActive(finished);
        watchButton.GetComponentInChildren<TMP_Text>().text = "Watch demo";
        skipButton.gameObject.SetActive(true);
        ClearSelection();
    }

    internal void ShowStep(int index)
    {
        ReleaseClip();
        step = index; inPractice = false;
        practicePanel.SetActive(false);
        watchButton.gameObject.SetActive(false);
        demoBadge.gameObject.SetActive(false);
        readiness.gameObject.SetActive(false);
        foreach (var node in hudContent) node.SetActive(false);
        foreach (var arrow in arrows)
            foreach (var part in new[] { arrow.shaft, arrow.tipA, arrow.tipB }) part.gameObject.SetActive(false);
        parryModal.SetActive(true);
        backButton.gameObject.SetActive(false);
        modalStep.gameObject.SetActive(false);
        clipTitle.text = lessons[step].title;
        clipRule.text = lessons[step].rule;
        actionLabel.text = step == StepCount - 1 ? "Start Battle" : "Next";
        startButton.interactable = true;
        demoPlaying = true;
        ApplyCompactLayout();
        totalDuration = 0;
        foreach (float hold in lessons[step].durations) totalDuration += hold;
        clipTime = 0;
        RenderClip();
        ClearSelection();
    }

    internal void ShowIntroFocus(int index)
    {
        foreach (var node in hudContent)
        {
            string name = node.name;
            node.SetActive(name == "PracticeButton" || name == "Step" ||
                (index == 0 && (name == "HealthCard" || name == "HealthGuide")) ||
                (index == 1 && (name == "TypingCard" || name == "TypingGuide")) ||
                (index == 2 && (name == "EnemyCard" || name == "EnemyGuide")));
        }
        int selected = index == 0 ? 0 : index == 1 ? 4 : 1;
        for (int i = 0; i < arrows.Length; i++)
        {
            arrows[i].shaft.gameObject.SetActive(i == selected);
            arrows[i].tipA.gameObject.SetActive(i == selected);
            arrows[i].tipB.gameObject.SetActive(i == selected);
        }
    }

    internal void ApplyCompactLayout()
    {
        void Box(RectTransform rect, float x, float y, float width, float height)
        {
            rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0, 1);
            rect.anchoredPosition = new Vector2(x, -y);
            rect.sizeDelta = new Vector2(width, height);
        }
        var panel = (RectTransform)parryModal.transform.Find("Panel");
        Box(panel, 200, 100, 560, 340);
        parryModal.GetComponent<Image>().color = new Color(0, 0, 0, .25f);
        Box(clipTitle.rectTransform, 20, 10, 240, 32);
        clipTitle.fontSize = clipTitle.fontSizeMin = clipTitle.fontSizeMax = 26;
        clipTitle.alignment = TextAlignmentOptions.MidlineLeft;
        clipTitle.margin = Vector4.zero;
        clipTitle.enableAutoSizing = false;
        Box(clipRule.rectTransform, 20, 290, 520, 44);
        clipRule.fontSize = clipRule.fontSizeMin = clipRule.fontSizeMax = 18;
        clipRule.alignment = TextAlignmentOptions.Center;
        clipRule.margin = Vector4.zero;
        clipRule.enableAutoSizing = false;
        clipRule.enableWordWrapping = true;
        Box(clipImage.rectTransform, 80, 50, 400, 225);
        captionPanel.gameObject.SetActive(false);
        Box((RectTransform)startButton.transform, 360, 10, 176, 32);
        Box(actionLabel.rectTransform, 4, 0, 168, 32);
        actionLabel.fontSize = actionLabel.fontSizeMin = actionLabel.fontSizeMax = 22;
        Box((RectTransform)skipButton.transform, 480, 113, 60, 26);
        var skipImage = skipButton.GetComponent<Image>();
        skipImage.enabled = true; skipImage.sprite = null; skipImage.color = Color.clear;
        var skipText = skipButton.GetComponentInChildren<TMP_Text>(true);
        Box(skipText.rectTransform, 0, 0, 60, 26);
        skipText.text = "Skip";
        skipText.fontSize = skipText.fontSizeMin = skipText.fontSizeMax = 18;
        skipButton.gameObject.SetActive(true);
    }

    internal void ReleaseClip()
    {
        demoPlaying = false;
        loopPhase = LoopPhase.Playing;
        loopElapsed = 0;
        finalFrameElapsed = 0;
        SetClipAlpha(1);
        if (clipImage) clipImage.sprite = null;
        SetClipVisible(false);
        frameReady = waitingForFrame = waitingForLoop = false;
        displayedAtlas = null;
        displayedFrame = -1;
        atlasCache.Clear();
    }

    void OnDisable()
    {
        Unavailable?.Invoke();
#if TYPINGHERO_AUTOMATION
        Canvas.willRenderCanvases -= TraceRender;
#endif
        ReleaseClip();
    }
    void OnDestroy() => ReleaseClip();

    void SetClipVisible(bool visible)
    {
        if (clipImage) clipImage.enabled = visible;
        if (clipFocus) clipFocus.gameObject.SetActive(false);
        if (guideShaft) guideShaft.gameObject.SetActive(false);
        if (guideTipA) guideTipA.gameObject.SetActive(false);
        if (guideTipB) guideTipB.gameObject.SetActive(false);
        if (guideBend) guideBend.gameObject.SetActive(false);
        if (guideEnd) guideEnd.gameObject.SetActive(false);
    }

    bool RenderClip()
    {
        var lesson = lessons[step];
        double time = Math.Min(clipTime, totalDuration);
        int frame = 0;
        double boundary = lesson.durations[0];
        while (frame < lesson.frameAtlas.Length - 1 && time >= boundary) boundary += lesson.durations[++frame];
        if (frameReady && frame == displayedFrame) return true;
        int atlas = lesson.frameAtlas[frame];
        string path = lesson.atlasPaths[atlas];
        string following = lesson.atlasPaths.Length > 1 ?
            lesson.atlasPaths[(atlas + 1) % lesson.atlasPaths.Length] : null;
        var page = atlasCache.Get(path, following, displayedAtlas);
        if (!page)
        {
            if (!frameReady) SetCaption(atlasCache.Error ?? "Loading...");
            else if (atlasCache.Error != null) SetCaption(atlasCache.Error);
            return false;
        }
        clipImage.sprite = page.sprites[lesson.frameSprite[frame]];
        finalFrameElapsed = 0;
        displayedAtlas = path;
        displayedFrame = frame;
        atlasCache.CommitDisplay(path);
        frameReady = true;
        if (!clipImage.enabled) SetClipVisible(true);
        SetCaption(lesson.captions[frame]);
        var guide = lesson.guidance[frame];
        clipFocus.anchoredPosition = new Vector2(guide.focus.x, -guide.focus.y);
        clipFocus.sizeDelta = guide.focus.size;
        // The beat label remains below the complete gameplay frame.
        Vector2 center = guide.focus.center;
        Vector2 to, from;
        if (guide.focus.yMax > 300 || guide.focus.yMax < 94)
        {
            // Approach HUD strips sideways to avoid vertically stacked values.
            bool right = center.x < 160;
            to = new Vector2(right ? guide.focus.xMax : guide.focus.xMin, center.y);
            from = to + new Vector2(right ? 12 : -12, 0);
        }
        else
        {
            bool below = center.y < clipImage.rectTransform.sizeDelta.y * .5f;
            to = new Vector2(center.x, below ? guide.focus.yMax : guide.focus.yMin);
            from = to + new Vector2(0, below ? 12 : -12);
        }
        Vector2 Local(Vector2 point) => new Vector2(point.x - clipImage.rectTransform.sizeDelta.x * .5f,
            clipImage.rectTransform.sizeDelta.y * .5f - point.y);
        from = Local(from); to = Local(to);
        Segment(guideShaft, from, to, 2);
        var direction = (to - from).normalized;
        var normal = new Vector2(-direction.y, direction.x);
        float head = Mathf.Min(10, Vector2.Distance(from, to) * .5f);
        Segment(guideTipA, to, to - direction * head + normal * head * .6f, 3);
        Segment(guideTipB, to, to - direction * head - normal * head * .6f, 3);
        return true;
    }

    void SetCaption(string value)
    {
        if (clipCaption.text != value) clipCaption.text = value;
    }

    void SetClipAlpha(float alpha)
    {
        if (!clipImage || clipImage.color.a == alpha) return;
        var color = clipImage.color;
        color.a = alpha;
        clipImage.color = color;
    }

    void AdvanceClip(float delta)
    {
        if (loopPhase != LoopPhase.Playing)
        {
            float duration = LoopHalfDuration * 2;
            loopElapsed = Mathf.Min(duration, loopElapsed + delta);
            if (loopPhase == LoopPhase.FadeOut)
            {
                if (loopElapsed >= LoopHalfDuration || Mathf.Approximately(loopElapsed, LoopHalfDuration))
                {
                    SetClipAlpha(0);
                    clipTime = 0;
                    finalFrameElapsed = 0;
                    RenderClip();
                    loopPhase = LoopPhase.FadeIn;
                }
                else SetClipAlpha(1 - Mathf.SmoothStep(0, 1, loopElapsed / LoopHalfDuration));
            }
            if (loopPhase == LoopPhase.FadeIn)
            {
                SetClipAlpha(Mathf.SmoothStep(0, 1, (loopElapsed - LoopHalfDuration) / LoopHalfDuration));
                if (loopElapsed >= duration || Mathf.Approximately(loopElapsed, duration))
                {
                    SetClipAlpha(1);
                    loopElapsed = 0;
                    loopPhase = LoopPhase.Playing;
                }
            }
            TracePlayback("tick", clipTime);
            return;
        }
        var currentLesson = lessons[step];
        if (frameReady && displayedFrame == currentLesson.frameAtlas.Length - 1) finalFrameElapsed += delta;
        double previousTime = clipTime;
        if (frameReady) clipTime = waitingForFrame ? pendingTime : Math.Min(clipTime + delta, totalDuration);
        double requested = clipTime;
        if (RenderClip())
        {
            waitingForFrame = false;
            if (clipTime >= totalDuration &&
                finalFrameElapsed >= currentLesson.durations[currentLesson.durations.Length - 1])
            {
                // Protect the final Image until the first page is ready and the fade reaches zero.
                var lesson = lessons[step];
                if (atlasCache.Get(lesson.atlasPaths[0], null, displayedAtlas))
                {
                    loopPhase = LoopPhase.FadeOut;
                    // Only the fractional render tick after the final hold belongs to the fade.
                    // A delayed first-page load starts a fresh 100 ms phase instead.
                    loopElapsed = waitingForLoop ? 0 : Mathf.Min(LoopHalfDuration,
                        (float)Math.Min(delta, Math.Max(0, finalFrameElapsed - currentLesson.durations[currentLesson.durations.Length - 1])));
                    waitingForLoop = false;
                    SetClipAlpha(1 - Mathf.SmoothStep(0, 1, loopElapsed / LoopHalfDuration));
                }
                else waitingForLoop = true;
            }
        }
        else
        {
            // Retry the original candidate, excluding even the final loading interval.
            if (frameReady && !waitingForFrame) pendingTime = clipTime;
            waitingForFrame = frameReady;
            clipTime = previousTime;
        }
        TracePlayback("tick", requested);
    }

    [System.Diagnostics.Conditional("TYPINGHERO_AUTOMATION")]
    void TracePlayback(string kind, double requested)
    {
#if TYPINGHERO_AUTOMATION
        TutorialPlaybackTrace.Playback(kind, step, requested, clipTime, displayedFrame,
            displayedAtlas, waitingForFrame || waitingForLoop, atlasCache.Session, atlasCache.SlotCount,
            (int)loopPhase, clipImage ? clipImage.color.a : 1);
#endif
    }

#if TYPINGHERO_AUTOMATION
    int lastRenderFrame = -1;
    void TraceRender()
    {
        if (inPractice || !canvas || !canvas.enabled || lastRenderFrame == Time.frameCount) return;
        lastRenderFrame = Time.frameCount;
        TracePlayback("render", clipTime);
    }
#endif

    Canvas canvas;
    CanvasGroup interaction;

    void Awake() { canvas = GetComponent<Canvas>(); interaction = GetComponent<CanvasGroup>(); }
    void LateUpdate()
    {
        canvas.enabled = !GamePauseController.IsPaused;
        interaction.interactable = interaction.blocksRaycasts = canvas.enabled;
        if (!canvas.enabled) return;
        if (TutorialRun.Active && !inPractice) AdvanceClip(demoPlaying ? Time.unscaledDeltaTime : 0);
        if (inPractice) UpdatePracticeFocus();
    }

    void UpdatePracticeFocus()
    {
        if (!practiceTarget) return;
        practiceTarget.GetWorldCorners(focusCorners);
        var parent = practiceFocus.parent;
        Vector2 lower = parent.InverseTransformPoint(focusCorners[0]);
        Vector2 upper = parent.InverseTransformPoint(focusCorners[2]);
        practiceFocus.localPosition = (lower + upper) * .5f;
        practiceFocus.sizeDelta = upper - lower;
    }

    static Vector2 Point(RectTransform root, RectTransform rect, Vector2 anchor)
    {
        var local = rect.rect.min + Vector2.Scale(rect.rect.size, anchor);
        return root.InverseTransformPoint(rect.TransformPoint(local));
    }

    static void Segment(RectTransform line, Vector2 from, Vector2 to, float thickness = 2)
    {
        line.localPosition = (from + to) * .5f;
        line.sizeDelta = new Vector2(Vector2.Distance(from, to), thickness);
        line.localRotation = Quaternion.Euler(0, 0, Mathf.Atan2(to.y - from.y, to.x - from.x) * Mathf.Rad2Deg);
    }

    internal void SubmitContinue()
    {
        var button = startButton;
        if (button.gameObject.activeInHierarchy && button.IsInteractable()) button.onClick.Invoke();
    }

    internal void ClearSelection()
    {
        if (EventSystem.current) EventSystem.current.SetSelectedGameObject(null);
    }
}
