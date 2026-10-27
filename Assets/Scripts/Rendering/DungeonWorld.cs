using Cinemachine;
using UnityEngine;
using UnityEngine.Rendering;

[DefaultExecutionOrder(-1000)]
public sealed class DungeonWorld : MonoBehaviour
{
    public enum Slot { Camera, Arena, Brain, Environment }
    public WorldDefinition definition;
    public WorldLightingProfile Profile => definition.lighting;
    public Object Resolve(Slot slot)
    {
        switch (slot)
        {
            case Slot.Camera: return view;
            case Slot.Arena: return Arena;
            case Slot.Brain: return view.GetComponent<CinemachineBrain>();
            case Slot.Environment: return environment;
            default: throw new System.ArgumentOutOfRangeException(nameof(slot));
        }
    }
    public Slot SlotFor(Object value)
    {
        foreach (Slot slot in System.Enum.GetValues(typeof(Slot))) if (Resolve(slot) == value) return slot;
        throw new System.InvalidOperationException(name + ": unsupported world binding " + value);
    }
    public Transform environment;
    public Camera view;
    Vector3 position;
    Quaternion rotation;
    float ortho, fieldOfView;
    public DungeonArenaCamera Arena => view.GetComponent<DungeonArenaCamera>();
    void Awake()
    {
        definition.Validate();
        Arena.profile = Profile;
        var volume = environment.GetComponent<Volume>();
        if (volume) volume.sharedProfile = Profile.post;
        foreach (var motion in environment.GetComponentsInChildren<DungeonEnvironmentMotion>(true)) motion.profile = Profile;
        position = view.transform.position; rotation = view.transform.rotation;
        ortho = view.orthographicSize; fieldOfView = view.fieldOfView;
        ResetView();
    }
    public void ApplyEnvironment() => Profile.ApplyEnvironment();
    public void ResetView()
    {
        environment.gameObject.SetActive(true);
        Arena.SetSlayerBackground(Color.clear);
        Arena.EnvironmentPresentationGain = 1f;
        Shader.SetGlobalFloat("_DungeonSlayerWeight", 0);
        var brain = view.GetComponent<CinemachineBrain>();
        if (brain) brain.enabled = false;
        view.transform.SetPositionAndRotation(position, rotation);
        view.orthographicSize = ortho; view.fieldOfView = fieldOfView;
        Arena.ResetFraming(); Arena.SetCinematic(false); Arena.SetPresentationRect(new Rect(0, 0, 1, 1));
        ApplyEnvironment();
    }
    public void EnableGameplayCamera()
    {
        Arena.ResetFraming();
        var brain = view.GetComponent<CinemachineBrain>();
        if (brain) brain.enabled = true;
    }
}
