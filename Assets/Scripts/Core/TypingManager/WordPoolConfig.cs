using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WP_Common 200", menuName = "Typing/Word Pool Config")]
public sealed class WordPoolConfig : ScriptableObject
{
    [Header("Paste block text vào đây (mỗi dòng 1 từ)")]
    [TextArea(5, 20)]
    [SerializeField] string rawWords = "";

    [Header("Options")]
    [SerializeField] bool normalizeLowercase = true;
    [SerializeField] bool removeDuplicates = true;

    // cache runtime
    string[] _cached = System.Array.Empty<string>();

    // ===== API dùng bởi TypingManager =====
    public void BuildCache()
    {
        if (string.IsNullOrWhiteSpace(rawWords)) { _cached = System.Array.Empty<string>(); return; }

        var lines = rawWords.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
        List<string> list = new(lines.Length);
        HashSet<string> seen = removeDuplicates ? new() : null;

        foreach (var line in lines)
        {
            var s = line.Trim();
            if (s.Length == 0) continue;
            if (normalizeLowercase) s = s.ToLowerInvariant();

            if (removeDuplicates)
            {
                if (seen.Add(s)) list.Add(s);
            }
            else list.Add(s);
        }
        _cached = list.ToArray();
    }

    public string GetRandomWord()
    {
        if (_cached == null || _cached.Length == 0) return null;
        return _cached[Random.Range(0, _cached.Length)];
    }

    // tiện ích trong Editor
    [ContextMenu("Build From Raw")]
    void BuildFromRaw_Inspector()
    {
        BuildCache();
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
        Debug.Log($"[WordPoolConfig] Built: {_cached.Length} words");
    }

    void OnValidate()
    {
        if (!Application.isPlaying) BuildCache();
    }
}
