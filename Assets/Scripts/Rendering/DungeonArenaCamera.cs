using Cinemachine;
using UnityEngine;
using UnityEngine.Rendering.Universal;

// Runs after the existing viewport/zoom correction, before cinematic projection.
[DefaultExecutionOrder(995)]
public sealed class DungeonArenaCamera : MonoBehaviour
{
    public Transform player, enemy;
    Camera view;
    CinemachineBrain brain;
    UniversalRenderPipelineAsset pipeline;
    float previousScale, offset, velocity, originX;
    void OnEnable()
    {
        view = GetComponent<Camera>(); brain = GetComponent<CinemachineBrain>();
        pipeline = UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
        if (pipeline) previousScale = pipeline.renderScale;
        originX = transform.position.x;
    }
    void LateUpdate()
    {
        if (!view) return;
        // Screen-space HUD remains full resolution; URP scales only the camera world.
        if (pipeline) { pipeline.renderScale = Mathf.Clamp(320f / Screen.width, .1f, 2f); pipeline.upscalingFilter = UpscalingFilterSelection.Point; }
        if (!brain || !brain.enabled || GamePauseController.InputBlocked) return;
        float goal = player && enemy ? Mathf.Clamp((player.position.x + enemy.position.x) * .5f - originX, -.5f, .5f) : 0;
        offset = Mathf.SmoothDamp(offset, goal, ref velocity, .35f, Mathf.Infinity, Time.deltaTime);
        // CombatViewportCamera already restores the brain position every frame.
        var position = transform.position; position.x += offset; transform.position = position;
        transform.rotation = Quaternion.Euler(10, 0, 0);
    }
    void OnDisable() { if (pipeline) pipeline.renderScale = previousScale; }
}
