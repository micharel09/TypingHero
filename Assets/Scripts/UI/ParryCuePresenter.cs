using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Session-only comparison. Reads combat state; never owns input or combat timing.
[DefaultExecutionOrder(1080)]
public sealed class ParryCuePresenter : MonoBehaviour
{
    [SerializeField] float fillLeadSeconds = .2f;
    [SerializeField] RectTransform key;
    [SerializeField] Image left, right, locked;
    [SerializeField] TMP_Text label;
    [SerializeField] Sprite outlineLeft, outlineRight, filledLeft, filledRight;
    [SerializeField] ParrySystem parry;
    [SerializeField] PlayerSlayerMode slayer;
    static bool useKey = true;
    bool lastVisible;
    SkeletonController boundEnemy;
    readonly Dictionary<Graphic, bool> legacyGraphics = new Dictionary<Graphic, bool>();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetSession() { useKey = true; }

    void Update()
    {
        if (GamePauseController.IsPaused && Input.GetKeyDown(KeyCode.F8))
        {
            useKey = !useKey;
            ApplyLegacyVisibility();
            key.gameObject.SetActive(useKey && lastVisible);
        }
    }

    void ApplyLegacyVisibility()
    {
        foreach (var pair in legacyGraphics)
            if (pair.Key) pair.Key.enabled = useKey ? false : pair.Value;
    }

    void LateUpdate()
    {
        // The parry target is rebound by encounter transitions; don't retain the old actor.
        var enemy = parry && parry.enemy ? parry.enemy.GetComponentInParent<SkeletonController>() : null;
        if (enemy != boundEnemy)
        {
            RestoreLegacyGraphics();
            boundEnemy = enemy;
            if (enemy && enemy.AttackCueObject)
                foreach (var graphic in enemy.AttackCueObject.GetComponentsInChildren<Graphic>(true))
                    if (!legacyGraphics.ContainsKey(graphic))
                    { legacyGraphics.Add(graphic, graphic.enabled); graphic.enabled = useKey ? false : graphic.enabled; }
        }
        if (!enemy || !enemy.isActiveAndEnabled || enemy.IsDead || CombatSession.Finished ||
            (slayer && slayer.IsActive) || GamePauseController.EncounterReading)
        {
            lastVisible = false;
            key.gameObject.SetActive(false);
            return;
        }
        // Pause/reading and hitstop retain the last readable state, including gate colors.
        if (GamePauseController.InputBlocked || Time.timeScale == 0) return;
        float remaining = 0;
        bool visible = enemy && !(slayer && slayer.IsActive) && enemy.TryGetAttackTelegraph(out remaining);
        lastVisible = visible;
        key.gameObject.SetActive(useKey && visible);
        if (!visible) return;
        bool open = parry.IsWindowActive;
        bool blocked = !open && !parry.CanBeginParry;
        bool fill = !blocked && (open || remaining <= fillLeadSeconds);
        left.sprite = fill ? filledLeft : outlineLeft;
        right.sprite = fill ? filledRight : outlineRight;
        Color color = blocked ? Hex("817b73") : open ? Hex("73b6aa") : fill ? Hex("f2e7cf") : Hex("dcc47c");
        left.color = right.color = color;
        locked.gameObject.SetActive(blocked); locked.color = color;
        label.gameObject.SetActive(!blocked);
        label.color = fill ? Hex("211c19") : color;
    }

    static Color Hex(string value) { ColorUtility.TryParseHtmlString("#" + value, out var color); return color; }
    void RestoreLegacyGraphics()
    {
        foreach (var pair in legacyGraphics) if (pair.Key) pair.Key.enabled = pair.Value;
        legacyGraphics.Clear();
    }
    void OnDisable()
    {
        RestoreLegacyGraphics();
        boundEnemy = null; lastVisible = false;
        if (key) key.gameObject.SetActive(false);
    }
}
