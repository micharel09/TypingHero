using System;
using System.Collections.Generic;
using UnityEngine;

// Owned by one overlay. Completions own cleanup even after its MonoBehaviour dies.
internal sealed class TutorialAtlasCache
{
    readonly Dictionary<string, TutorialAtlasPage> pages = new Dictionary<string, TutorialAtlasPage>();
    string current, next, loading;
    int session;
    internal string Error { get; private set; }
    internal int SlotCount => pages.Count + (loading == null ? 0 : 1);

    internal TutorialAtlasPage Get(string path, string following, string held = null)
    {
        current = path; next = following;
        string keep = pages.ContainsKey(path) ? next : held;
        foreach (string key in new List<string>(pages.Keys))
            if (key != current && key != keep)
            {
                var old = pages[key];
                pages.Remove(key);
                Release(old);
            }
        Pump();
        return pages.TryGetValue(path, out var page) ? page : null;
    }

    internal void Clear()
    {
        session++;
        current = next = null;
        Error = null;
        var retired = new List<TutorialAtlasPage>(pages.Values);
        pages.Clear();
        foreach (var page in retired) Release(page);
        // Resources requests cannot be cancelled. Their slot stays reserved until completion.
    }

    void Pump()
    {
        if (loading != null || current == null || Error != null) return;
        string path = !pages.ContainsKey(current) ? current :
            next != null && !pages.ContainsKey(next) ? next : null;
        if (path == null || SlotCount >= 2) return;
        loading = path;
        int version = session;
        var request = Resources.LoadAsync<TutorialAtlasPage>(path);
        request.completed += _ => {
            loading = null;
            var page = request.asset as TutorialAtlasPage;
            if (version != session || (path != current && path != next)) Release(page);
            else if (!page || !page.texture || page.sprites == null || page.sprites.Length == 0)
            {
                Release(page);
                Error = "Unable to load tutorial. Use Back or Menu.";
            }
            else pages.Add(path, page);
            Pump();
        };
    }

    static void Release(TutorialAtlasPage page)
    {
        if (!page) return;
        var texture = page.texture;
        foreach (var sprite in page.sprites ?? Array.Empty<Sprite>())
            if (sprite) Resources.UnloadAsset(sprite);
        if (texture) Resources.UnloadAsset(texture);
        Resources.UnloadAsset(page);
    }
}
