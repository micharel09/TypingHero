using System;
using System.Collections.Generic;
using UnityEngine;

// Owned by one overlay. Completions own cleanup even after its MonoBehaviour dies.
internal sealed class TutorialAtlasCache
{
    readonly Dictionary<string, TutorialAtlasPage> pages = new Dictionary<string, TutorialAtlasPage>(2);
    readonly List<string> retiredKeys = new List<string>(2);
    string current, next, loading;
    int session;
    internal string Error { get; private set; }
    internal int SlotCount => pages.Count + (loading == null ? 0 : 1);
    internal int Session => session;

    [System.Diagnostics.Conditional("TYPINGHERO_AUTOMATION")]
    void Trace(string kind, string path, int requestSession = -1)
    {
#if TYPINGHERO_AUTOMATION
        TutorialPlaybackTrace.Resource(kind, path, requestSession < 0 ? session : requestSession, SlotCount);
#endif
    }

    internal TutorialAtlasPage Get(string path, string following, string held = null)
    {
        current = path; next = following;
        RetireExcept(current, held != null && held != current ? held : next);
        Pump();
        return pages.TryGetValue(path, out var page) ? page : null;
    }

    // The Image must already reference the destination before the old page is retired.
    internal void CommitDisplay(string path)
    {
        if (path != current || !pages.ContainsKey(path)) return;
        RetireExcept(current, next);
        Pump();
    }

    void RetireExcept(string first, string second)
    {
        retiredKeys.Clear();
        foreach (string key in pages.Keys)
            if (key != first && key != second) retiredKeys.Add(key);
        foreach (string key in retiredKeys)
        {
            var old = pages[key];
            pages.Remove(key);
            Release(old);
        }
        retiredKeys.Clear();
    }

    internal void Clear()
    {
        session++;
        current = next = null;
        Error = null;
        RetireExcept(null, null);
        // Resources requests cannot be cancelled. Their slot stays reserved until completion.
    }

    void Pump()
    {
        if (loading != null || current == null || Error != null) return;
        string path = !pages.ContainsKey(current) ? current :
            next != null && !pages.ContainsKey(next) ? next : null;
        if (path == null || SlotCount >= 2) return;
        BeginLoad(path);
    }

    // Keep the completion closure entirely off the stable Get/Pump path.
    void BeginLoad(string path)
    {
        loading = path;
        int version = session;
        Trace("load-start", path);
        var request = Resources.LoadAsync<TutorialAtlasPage>(path);
        Trace("load-return", path);
        request.completed += _ => {
            Trace("completion-start", path, version);
            loading = null;
            var page = request.asset as TutorialAtlasPage;
            if (version != session || (path != current && path != next)) Release(page);
            else if (!page || !page.texture || page.sprites == null || page.sprites.Length == 0)
            {
                Release(page);
                Error = "Unable to load tutorial. Use Back or Menu.";
            }
            else pages.Add(path, page);
            Trace("completion-end", path, version);
            Pump();
        };
    }

    void Release(TutorialAtlasPage page)
    {
        if (!page) return;
        Trace("unload-start", page.name);
        var texture = page.texture;
        foreach (var sprite in page.sprites ?? Array.Empty<Sprite>())
            if (sprite) Resources.UnloadAsset(sprite);
        if (texture) Resources.UnloadAsset(texture);
        Resources.UnloadAsset(page);
        Trace("unload-end", null);
    }
}
