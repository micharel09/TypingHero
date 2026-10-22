using UnityEngine;

// Light intensity varies in world space; pause and cinematic hiding preserve phase.
[RequireComponent(typeof(Light))]
public sealed class DungeonTorchLight : MonoBehaviour
{
    [SerializeField, Min(0)] float baseIntensity = 22;
    [SerializeField] float phase;
    [SerializeField, Range(0, .06f)] float flickerAmplitude = .02f;
    [SerializeField] Renderer[] flameRenderers = new Renderer[0];
    [SerializeField] MeshFilter pixelFlame;
    [SerializeField] float flameTimeOffset;
    [SerializeField, Min(1)] int flameFrames = 5;
    [SerializeField, Min(1)] int flameColumns = 5;
    [SerializeField, Min(.001f)] float flameFrameSeconds = .1f;
    Light source;
    MaterialPropertyBlock flameBlock;
    float age;
    Mesh originalMesh, privateMesh;
    Vector2[] originalUV, frameUV;
    int displayedFrame = -1;

    void OnEnable()
    {
        source = GetComponent<Light>();
        if (pixelFlame && !privateMesh)
        {
            originalMesh = pixelFlame.sharedMesh;
            privateMesh = Instantiate(originalMesh);
            privateMesh.name = "Lantern pixel flame (runtime)";
            originalUV = originalMesh.uv;
            frameUV = new Vector2[originalUV.Length];
            pixelFlame.sharedMesh = privateMesh;
        }
        Apply();
    }

    void Update()
    {
        if (GamePauseController.InputBlocked || Time.timeScale == 0) return;
        age += Time.deltaTime;
        Apply();
    }

    void Apply()
    {
        float wave = pixelFlame
            ? 2 * Mathf.Clamp01(Mathf.PerlinNoise(phase + age * 4.7f, 11.73f)) - 1
            : .65f * Mathf.Sin(age * 12.57f + phase)
            + .35f * Mathf.Sin(age * 19.92f + phase * 1.7f);
        float multiplier=1+Mathf.Clamp(flickerAmplitude,0,pixelFlame ? .06f : .02f)*wave;
        source.intensity=baseIntensity*multiplier;
        int frame = Mathf.FloorToInt((age + flameTimeOffset) / flameFrameSeconds) % flameFrames;
        if (privateMesh && displayedFrame != frame)
        {
            for (int i = 0; i < frameUV.Length; i++)
                frameUV[i] = originalUV[i] + new Vector2((frame % flameColumns) / (float)flameColumns,
                    -(frame / flameColumns) / (float)Mathf.CeilToInt(flameFrames / (float)flameColumns));
            privateMesh.uv = frameUV;
            displayedFrame = frame;
        }
        if(flameBlock==null)flameBlock=new MaterialPropertyBlock();
        foreach(var renderer in flameRenderers)
        {
            if(!renderer||!renderer.sharedMaterial)continue;
            renderer.GetPropertyBlock(flameBlock);
            flameBlock.SetColor("_EmissionColor",renderer.sharedMaterial.GetColor("_EmissionColor")*multiplier);
            renderer.SetPropertyBlock(flameBlock);
        }
    }

    void OnDisable()
    {
        if (source) source.intensity = baseIntensity;
    }

    void OnDestroy()
    {
        if (pixelFlame && pixelFlame.sharedMesh == privateMesh) pixelFlame.sharedMesh = originalMesh;
        if (privateMesh) Destroy(privateMesh);
    }
}
