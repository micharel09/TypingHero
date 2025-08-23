using UnityEngine;

[DisallowMultipleComponent]
public class MusicOnSceneStart : MonoBehaviour
{
    [SerializeField] MusicCue cue;
    [SerializeField] float fadeOut = 0.3f;
    [SerializeField] float fadeIn = 0.5f;

    void Start()
    {
        if (MusicPlayer.I && cue)
            MusicPlayer.I.Play(cue, fadeOut, fadeIn);
    }
}
