using UnityEngine;

// One independently loadable atlas; the scene stores only its Resources path.
public sealed class TutorialAtlasPage : ScriptableObject
{
    public Texture2D texture;
    public Sprite[] sprites;
}
