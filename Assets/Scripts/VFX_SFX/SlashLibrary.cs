using UnityEngine;

public sealed class SlashLibrary : ScriptableObject
{
    [System.Serializable]
    internal struct Frame
    {
        [SerializeField] internal Sprite sprite;
        [SerializeField] internal Mesh front, back;
    }
    [SerializeField] internal Frame[] frames;
    [SerializeField] internal Material material;
}
