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
    [SerializeField] RectTransform exampleKey;
    [SerializeField] Image exampleFace, exampleLocked;
    [SerializeField] TMP_Text exampleCaption;
    [SerializeField] Sprite exampleWarning, exampleLight, exampleDown, exampleBlocked;
    double exampleTime;
    void OnEnable() { exampleTime = 0; RenderExample(); }

    void RenderExample()
    {
        if (!exampleKey || !exampleCaption) return;
        double t = exampleTime % 2.75;
        bool tap = t >= .8 && t < 1;
        bool open = t >= 1 && t < 1.25;
        bool blocked = t >= 1.25 && t < 2;
        exampleKey.gameObject.SetActive(TutorialRun.Active && t < 2);
        exampleFace.sprite = blocked ? exampleBlocked : open ? exampleDown : tap ?
            ((t - .8) % .1 >= .05 ? exampleDown : exampleLight) : exampleWarning;
        ColorUtility.TryParseHtmlString(blocked ? "#817b73" : open ? "#73b6aa" : "#f4eeda", out var color);
        exampleFace.color = color;
        exampleLocked.gameObject.SetActive(blocked); exampleLocked.color = color;
        exampleCaption.text = t < .8 ? "Get ready." : tap ? "Tap Space now." : open ?
            "Parry window open." : blocked ? "X: parry unavailable." : "Release between hits.";
    }

    Canvas canvas;
    CanvasGroup interaction;

    void Awake() { canvas = GetComponent<Canvas>(); interaction = GetComponent<CanvasGroup>(); }
    void LateUpdate()
    {
        canvas.enabled = !GamePauseController.IsPaused;
        interaction.interactable = interaction.blocksRaycasts = canvas.enabled;
        if (!canvas.enabled) return;
        if (TutorialRun.Active)
        {
            exampleTime += Time.unscaledDeltaTime;
            RenderExample();
        }
        else if (exampleKey) exampleKey.gameObject.SetActive(false);
        Canvas.ForceUpdateCanvases();
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

    static void Segment(RectTransform line, Vector2 from, Vector2 to)
    {
        line.localPosition = (from + to) * .5f;
        line.sizeDelta = new Vector2(Vector2.Distance(from, to), 2);
        line.localRotation = Quaternion.Euler(0, 0, Mathf.Atan2(to.y - from.y, to.x - from.x) * Mathf.Rad2Deg);
    }

    internal void SubmitContinue()
    {
        if (continueButton.gameObject.activeInHierarchy && continueButton.IsInteractable())
            continueButton.onClick.Invoke();
    }

    internal void ClearSelection()
    {
        if (EventSystem.current) EventSystem.current.SetSelectedGameObject(null);
    }
}
