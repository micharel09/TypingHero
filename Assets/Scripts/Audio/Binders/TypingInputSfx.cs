using UnityEngine;

[DisallowMultipleComponent]
public sealed class TypingInputSfx : MonoBehaviour
{
    [SerializeField] SfxEvent correct;
    [SerializeField] SfxEvent error;

    void OnEnable() => TypingManager.OnInputResult += Handle;
    void OnDisable() => TypingManager.OnInputResult -= Handle;

    void Handle(bool accepted)
    {
        // WebGL can briefly start a reused channel before its new gain is applied.
        if (SfxPlayer.I && AudioPreferences.Sfx > 0f)
            SfxPlayer.I.Play(accepted ? correct : error, Vector3.zero);
    }
}
