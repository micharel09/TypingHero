using UnityEngine;
using TMPro;

public static class ScoreUI_Flash
{
    static float _lastFlashAt;
    public static void Do(TMP_Text text, float mul)
    {
        if (!text) return;
        float now = Time.unscaledTime;
        if (now - _lastFlashAt < 0.06f) return; // ch?n flash dày

        _lastFlashAt = now;
        text.transform.localScale = Vector3.one * 1.0f;
        text.StopAllCoroutines();
        text.StartCoroutine(FlashCo(text, mul));
    }

    static System.Collections.IEnumerator FlashCo(TMP_Text t, float mul)
    {
        float dur = 0.12f;
        float big = (mul >= 3f) ? 1.20f : (mul >= 2f ? 1.12f : 1.06f);

        float start = Time.unscaledTime;
        while (Time.unscaledTime - start < dur)
        {
            float k = (Time.unscaledTime - start) / dur;
            float s = Mathf.Lerp(big, 1f, k);
            t.transform.localScale = new Vector3(s, s, 1f);
            yield return null;
        }
        t.transform.localScale = Vector3.one;
    }
}
