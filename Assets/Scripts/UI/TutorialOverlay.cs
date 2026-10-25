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
        // Top-left coordinates in the 720x270 clip, independent of viewport scale.
        public Rect focus;
        public Vector2 label;
    }
    [Serializable]
    internal sealed class Lesson
    {
        public string title;
        public string rule;
        public Sprite[] frames;
        public float[] durations;
        public string[] captions;
        public Guidance[] guidance;
    }
    [SerializeField] Lesson[] lessons = Array.Empty<Lesson>();
    [SerializeField] TMP_Text clipTitle, modalStep, hudStep, actionLabel;
    double clipTime;
    int step;
    internal int StepCount => lessons.Length + 1;
    void OnEnable() => ShowStep(0);

    internal void BindBack(UnityEngine.Events.UnityAction action) => backButton.onClick.AddListener(action);

    internal void ShowStep(int index)
    {
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

    void RenderClip()
    {
        if (step == 0) return;
        var lesson = lessons[step - 1];
        double duration = 0;
        foreach (float hold in lesson.durations) duration += hold;
        double time = clipTime % duration;
        int frame = 0;
        double boundary = lesson.durations[0];
        while (frame < lesson.frames.Length - 1 && time >= boundary) boundary += lesson.durations[++frame];
        clipImage.sprite = lesson.frames[frame];
        clipCaption.text = lesson.captions[frame];
        var guide = lesson.guidance[frame];
        captionPanel.anchoredPosition = new Vector2(guide.label.x, -guide.label.y);
        clipFocus.anchoredPosition = new Vector2(guide.focus.x, -guide.focus.y);
        clipFocus.sizeDelta = guide.focus.size;
        // Join the nearest edges of the label and focus, never crossing their text.
        var label = new Rect(guide.label, captionPanel.sizeDelta);
        Vector2 center = guide.focus.center;
        Vector2 from = new Vector2(Mathf.Clamp(center.x, label.xMin, label.xMax),
            Mathf.Clamp(center.y, label.yMin, label.yMax));
        Vector2 to = new Vector2(Mathf.Clamp(from.x, guide.focus.xMin, guide.focus.xMax),
            Mathf.Clamp(from.y, guide.focus.yMin, guide.focus.yMax));
        // Route left-HUD callouts around the other strips instead of through their values.
        bool aroundHud = guide.focus.x < 80 * 3;
        guideBend.gameObject.SetActive(aroundHud); guideEnd.gameObject.SetActive(aroundHud);
        Vector2 Local(Vector2 point) => new Vector2(point.x - 360, 135 - point.y);
        if (aroundHud)
        {
            from = new Vector2(label.xMin, label.center.y);
            to = new Vector2(guide.focus.xMin, guide.focus.center.y);
            var bend = new Vector2(3, from.y);
            var end = new Vector2(3, to.y);
            Segment(guideShaft, Local(from), Local(bend), 3);
            Segment(guideBend, Local(bend), Local(end), 3);
            Segment(guideEnd, Local(end), Local(to), 3);
            from = end;
        }
        from = Local(from); to = Local(to);
        if (!aroundHud) Segment(guideShaft, from, to, 3);
        var direction = (to - from).normalized;
        var normal = new Vector2(-direction.y, direction.x);
        float head = Mathf.Min(10, Vector2.Distance(from, to) * .5f);
        Segment(guideTipA, to, to - direction * head + normal * head * .6f, 3);
        Segment(guideTipB, to, to - direction * head - normal * head * .6f, 3);
    }

    Canvas canvas;
    CanvasGroup interaction;

    void Awake() { canvas = GetComponent<Canvas>(); interaction = GetComponent<CanvasGroup>(); }
    void LateUpdate()
    {
        canvas.enabled = !GamePauseController.IsPaused;
        interaction.interactable = interaction.blocksRaycasts = canvas.enabled;
        if (!canvas.enabled) return;
        if (TutorialRun.Active && step > 0)
        {
            clipTime += Time.unscaledDeltaTime;
            RenderClip();
        }
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
