using Cinemachine;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

// Runs after the existing viewport/zoom correction, before cinematic projection.
[DefaultExecutionOrder(995)]
public sealed class DungeonArenaCamera : MonoBehaviour
{
    public Transform player, enemy;
    [SerializeField] bool postProcessing = true;
    [SerializeField, Range(0, 15)] float pitch = 5f;
    [SerializeField] CombatHUDLayout framingLayout;
    [SerializeField] CombatCameraImpact impact;
    internal float ImpactZoom => impact ? impact.Zoom : 0;
    internal float PixelSizeAtDepth(float depth) => 2 * depth * EffectiveHalfHeight / framingDistance / WorldHeight;
    internal float EffectiveHalfHeight => view.orthographicSize / (1 + ImpactZoom);
    Vector3 unshakenPosition;
    [SerializeField, Min(.1f)] float framingDistance = 17.8403f;
    public const int WorldWidth = 320, WorldHeight = 180;
    public RenderTexture WorldTexture { get; private set; }
    public Camera PresentationCamera { get; private set; }
    public Rect PresentationRect { get; private set; } = new Rect(0, 0, 1, 1);
    public bool Cinematic { get; private set; }
    internal RTHandle EnvironmentScratch;
    public float FocusDepth => view.WorldToViewportPoint(player ? player.position : Vector3.zero).z;
    Camera view;
    CinemachineBrain brain;
    RenderTexture previousTexture;
    Rect previousRect;
    bool previousPostProcessing;
    float offset, velocity, originX;
    Vector3 framingOrigin;
    void OnEnable()
    {
        if (!Application.isPlaying) return;
        view = GetComponent<Camera>(); brain = GetComponent<CinemachineBrain>();
        previousTexture = view.targetTexture; previousRect = view.rect;
        unshakenPosition = transform.position;
        originX = transform.position.x;
        framingOrigin = framingLayout && player && enemy ? framingLayout.BodyCenter : Vector3.zero;
        offset = velocity = 0;
        WorldTexture = new RenderTexture(WorldWidth, WorldHeight, 24, RenderTextureFormat.ARGBHalf)
        {
            name = "Dungeon world 320x180 HDR", filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp, useMipMap = false, autoGenerateMips = false,
            antiAliasing = 1, useDynamicScale = false
        };
        WorldTexture.Create();
        // Unity2022's NullGfx still executes offscreen cameras and crashes in
        // DrawLineOrTrailFromNodeQueue. Offline logic tests keep projection and
        // ownership metadata, while only a graphics device receives the target.
        if (SystemInfo.graphicsDeviceType != GraphicsDeviceType.Null) view.targetTexture = WorldTexture;
        view.rect = new Rect(0, 0, 1, 1);
        view.aspect = (float)WorldWidth / WorldHeight;
        var screen = new GameObject("Dungeon Presentation"); screen.transform.SetParent(transform, false);
        PresentationCamera = screen.AddComponent<Camera>();
        PresentationCamera.cullingMask = 0; PresentationCamera.depth = view.depth + 1;
        PresentationCamera.clearFlags = CameraClearFlags.SolidColor; PresentationCamera.backgroundColor = Color.black;
        PresentationCamera.allowHDR = true; PresentationCamera.allowMSAA = false;
        PresentationCamera.orthographic = true;
        var data = PresentationCamera.GetUniversalAdditionalCameraData(); data.SetRenderer(2);
        data.renderShadows = false; data.renderPostProcessing = postProcessing;
        data.antialiasing = AntialiasingMode.None;
        data.requiresColorOption = data.requiresDepthOption = CameraOverrideOption.Off;
        screen.AddComponent<DungeonPresentationCamera>().Initialize(this);
        SetPresentationRect(previousRect);
    }
    public void SetPresentationRect(Rect rect)
    {
        PresentationRect = rect;
        if (PresentationCamera) PresentationCamera.rect = rect;
    }
    public Vector3 WorldToScreenPoint(Vector3 point)
    {
        var projected = view.WorldToViewportPoint(point);
        return new Vector3((PresentationRect.x + projected.x * PresentationRect.width) * Screen.width,
            (PresentationRect.y + projected.y * PresentationRect.height) * Screen.height, projected.z);
    }
    public void SetCinematic(bool active)
    {
        if (Cinematic == active) return;
        Cinematic = active;
        if (!PresentationCamera) return;
        var data = PresentationCamera.GetUniversalAdditionalCameraData();
        if (active) { previousPostProcessing = data.renderPostProcessing; data.renderPostProcessing = false; }
        else data.renderPostProcessing = previousPostProcessing;
    }
    internal void ResetImpacts(bool restoreView = false)
    {
        if (impact) impact.Clear();
        if (restoreView && view)
        {
            transform.position = unshakenPosition;
            view.fieldOfView = 2 * Mathf.Atan(view.orthographicSize / framingDistance) * Mathf.Rad2Deg;
        }
    }
    public void ResetFraming()
    {
        ResetImpacts();
        unshakenPosition = transform.position;
        offset = velocity = 0;
        originX = transform.position.x;
        framingOrigin = framingLayout && player && enemy ? framingLayout.BodyCenter : Vector3.zero;
    }
    void LateUpdate()
    {
        if (!view) return;
        // Existing Slayer/pause tweens still own this half-height zoom scalar.
        // Apply the perspective lens even while pause owns camera position.
        view.orthographic = false;
        view.fieldOfView = 2 * Mathf.Atan(EffectiveHalfHeight / framingDistance) * Mathf.Rad2Deg;
        if (!brain || !brain.enabled || GamePauseController.InputBlocked) return;
        if (framingLayout && (!player || !enemy)) return;
        var center = framingLayout ? framingLayout.BodyCenter : player && enemy ? (player.position + enemy.position) * .5f : framingOrigin;
        float goal = Mathf.Clamp(center.x - originX, -.5f, .5f);
        offset = Mathf.SmoothDamp(offset, goal, ref velocity, .35f, Mathf.Infinity, Time.deltaTime);
        // Start from the brain state, retaining impulses without accumulating offsets.
        var position = brain.CurrentCameraState.FinalPosition;
        position.x += offset;
        if (framingLayout)
            position.y += center.y - framingOrigin.y + Mathf.Tan(pitch * Mathf.Deg2Rad) * (center.z - framingOrigin.z);
        unshakenPosition = position - brain.CurrentCameraState.PositionCorrection;
        // Zoom around the existing foot landmark, rather than lifting the floor.
        if (framingLayout && ImpactZoom > 0)
        {
            var rotation = Quaternion.Euler(pitch, 0, 0);
            var local = Quaternion.Inverse(rotation) * (framingLayout.LowestFoot - unshakenPosition);
            float ratio = EffectiveHalfHeight / view.orthographicSize;
            float shift = local.y * (1 - ratio) /
                (Mathf.Cos(pitch * Mathf.Deg2Rad) + ratio * local.y / Mathf.Max(.01f, local.z) * Mathf.Sin(pitch * Mathf.Deg2Rad));
            position.y += shift;
        }
        transform.position = position;
        transform.rotation = Quaternion.Euler(pitch, 0, 0);
    }
    void OnDisable()
    {
        if (view) { view.targetTexture = previousTexture; view.rect = previousRect; view.ResetAspect(); }
        if (PresentationCamera)
        {
            PresentationCamera.enabled = false; Destroy(PresentationCamera.gameObject); PresentationCamera = null;
        }
        if (WorldTexture) { WorldTexture.Release(); Destroy(WorldTexture); WorldTexture = null; }
        EnvironmentScratch?.Release(); EnvironmentScratch = null;
        Cinematic = false;
    }
}
