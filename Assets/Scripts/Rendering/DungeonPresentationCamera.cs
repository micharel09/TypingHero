using UnityEngine;

// The presentation renderer reads only the arena-owned HDR texture; HUD is overlay UGUI.
public sealed class DungeonPresentationCamera : MonoBehaviour
{
    public DungeonArenaCamera Arena { get; private set; }
    internal void Initialize(DungeonArenaCamera arena) { Arena = arena; }
}
