using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[DefaultExecutionOrder(1100)]
public sealed class TutorialOverlay : MonoBehaviour
{
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
    bool frameReady, waitingForFrame;
    string displayedAtlas;
    double clipTime, pendingTime;
    int step;
    internal int StepCount => lessons.Length + 1;
    void OnEnable() => ShowStep(0);

    internal void BindBack(UnityEngine.Events.UnityAction action) => backButton.onClick.AddListener(action);

    internal void ShowStep(int index)
    {
        ReleaseClip();
        step = index;
        bool modal = step > 0;
        foreach (var node in hudContent) node.SetActive(!modal);
        foreach (var arrow in arrows)
            foreach (var part in new[] { arrow.shaft, arrow.tipA, arrow.tipB }) part.gameObject.SetActive(!modal);
        parryModal.SetActive(modal);
        if (hudStep) hudStep.text = "1/" + StepCount;
        if (modal)
        {
            clipTitle.text = lessons[step - 1].title;
            clipRule.text = lessons[step - 1].rule;
            modalStep.text = (step + 1) + "/" + StepCount;
            actionLabel.text = step == StepCount - 1 ? "Start Battle \u00b7 Enter" : "Next \u00b7 Enter";
        }
        clipTime = 0;
        RenderClip();
        ClearSelection();
    }

    internal void ReleaseClip()
    {
        if (clipImage) clipImage.sprite = null;
        SetClipVisible(false);
        frameReady = waitingForFrame = false;
        displayedAtlas = null;
        atlasCache.Clear();
    }

    void OnDisable() => ReleaseClip();
    void OnDestroy() => ReleaseClip();

    void SetClipVisible(bool visible)
    {
        if (clipImage) clipImage.enabled = visible;
        if (clipFocus) clipFocus.gameObject.SetActive(visible);
        if (guideShaft) guideShaft.gameObject.SetActive(visible);
        if (guideTipA) guideTipA.gameObject.SetActive(visible);
        if (guideTipB) guideTipB.gameObject.SetActive(visible);
        if (guideBend) guideBend.gameObject.SetActive(false);
        if (guideEnd) guideEnd.gameObject.SetActive(false);
    }

    bool RenderClip()
    {
        if (step == 0) return false;
        var lesson = lessons[step - 1];
        double duration = 0;
        foreach (float hold in lesson.durations) duration += hold;
        double time = clipTime % duration;
        int frame = 0;
        double boundary = lesson.durations[0];
        while (frame < lesson.frameAtlas.Length - 1 && time >= boundary) boundary += lesson.durations[++frame];
        int atlas = lesson.frameAtlas[frame];
        string path = lesson.atlasPaths[atlas];
        string following = lesson.atlasPaths[(atlas + 1) % lesson.atlasPaths.Length];
        // Remove the UI reference before Get can evict its atlas.
        var previous = clipImage.sprite;
        clipImage.sprite = null;
        var page = atlasCache.Get(path, following, displayedAtlas);
        if (!page)
        {
            clipImage.sprite = previous;
            if (!frameReady) { SetClipVisible(false); clipCaption.text = atlasCache.Error ?? "Loading..."; }
            else if (atlasCache.Error != null) clipCaption.text = atlasCache.Error;
            return false;
        }
        clipImage.sprite = page.sprites[lesson.frameSprite[frame]];
        displayedAtlas = path;
        frameReady = true;
        SetClipVisible(true);
        clipCaption.text = lesson.captions[frame];
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

    void AdvanceClip(float delta)
    {
        double previousTime = clipTime;
        if (frameReady) clipTime = waitingForFrame ? pendingTime : clipTime + delta;
        if (RenderClip()) waitingForFrame = false;
        else
        {
            // Retry the original candidate, excluding even the final loading interval.
            if (frameReady && !waitingForFrame) pendingTime = clipTime;
            waitingForFrame = frameReady;
            clipTime = previousTime;
        }
    }

    Canvas canvas;
    CanvasGroup interaction;

    void Awake() { canvas = GetComponent<Canvas>(); interaction = GetComponent<CanvasGroup>(); }
    void LateUpdate()
    {
        canvas.enabled = !GamePauseController.IsPaused;
        interaction.interactable = interaction.blocksRaycasts = canvas.enabled;
        if (!canvas.enabled) return;
        if (TutorialRun.Active && step > 0) AdvanceClip(Time.unscaledDeltaTime);
        Canvas.ForceUpdateCanvases();
        if (step > 0) return;
        foreach (var arrow in arrows)
        {
            var root = (RectTransform)arrow.shaft.parent;
            var from = Point(root, arrow.source, arrow.sourceAnchor);
            var to = Point(root, arrow.target, arrow.targetAnchor);
            Segment(arrow.shaft, from, to);
            var direction = (to - from).normalized;
            var normal = new Vector2(-direction.y, direction.x);
            Segment(arrow.tipA, to, to - direction * 10 + normal * 6);
            Segment(arrow.tipB, to, to - direction * 10 - normal * 6);
        }
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
        var button = step > 0 ? startButton : continueButton;
        if (button.gameObject.activeInHierarchy && button.IsInteractable()) button.onClick.Invoke();
    }

    internal void ClearSelection()
    {
        if (EventSystem.current) EventSystem.current.SetSelectedGameObject(null);
    }
}
