using Cinemachine;
using UnityEngine;
using UnityEngine.Rendering;

public sealed class DungeonWorld : MonoBehaviour
{
    public Object[] objects;
    public Transform environment;
    public Camera view;
    Vector3 position;
    Quaternion rotation;
    float ortho, fieldOfView;
    [SerializeField] Color ambient, fogColor;
    [SerializeField] float fogStart, fogEnd, fogDensity, ambientIntensity;
    [SerializeField] bool fog;
    [SerializeField] FogMode fogMode;
    [SerializeField] AmbientMode ambientMode;
    public DungeonArenaCamera Arena => view.GetComponent<DungeonArenaCamera>();
    void Awake()
    {
        position = view.transform.position; rotation = view.transform.rotation;
        ortho = view.orthographicSize; fieldOfView = view.fieldOfView;
        ResetView();
    }
    public void CaptureEnvironment()
    {
        ambient = RenderSettings.ambientLight; ambientMode = RenderSettings.ambientMode;
        ambientIntensity = RenderSettings.ambientIntensity;
        fog = RenderSettings.fog; fogMode = RenderSettings.fogMode; fogColor = RenderSettings.fogColor;
        fogStart = RenderSettings.fogStartDistance; fogEnd = RenderSettings.fogEndDistance; fogDensity = RenderSettings.fogDensity;
    }
    public void ApplyEnvironment()
    {
        RenderSettings.ambientMode = ambientMode; RenderSettings.ambientLight = ambient;
        RenderSettings.ambientIntensity = ambientIntensity;
        RenderSettings.fog = fog; RenderSettings.fogMode = fogMode; RenderSettings.fogColor = fogColor;
        RenderSettings.fogStartDistance = fogStart; RenderSettings.fogEndDistance = fogEnd; RenderSettings.fogDensity = fogDensity;
    }
    public void ResetView()
    {
        environment.gameObject.SetActive(true);
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
