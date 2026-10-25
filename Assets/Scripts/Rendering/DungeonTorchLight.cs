using UnityEngine;

// Light intensity varies in world space; pause and cinematic hiding preserve phase.
[RequireComponent(typeof(Light))]
public sealed class DungeonTorchLight : MonoBehaviour
{
    public LightProfile profile;
    [SerializeField] float phase;
    [SerializeField] Renderer[] flameRenderers = new Renderer[0];
    [SerializeField] MeshFilter pixelFlame;
    [SerializeField] float flameTimeOffset;
    public MeshFilter Flame => pixelFlame;
    public void Preview()
    {
        if (!profile) throw new System.InvalidOperationException(name + ": missing LightProfile");
        profile.Validate(); profile.Apply(GetComponent<Light>());
        if (pixelFlame) pixelFlame.GetComponent<Renderer>().enabled = profile.flame;
        if (profile.flame && pixelFlame)
        {
            pixelFlame.sharedMesh = profile.flame.mesh;
            pixelFlame.GetComponent<Renderer>().sharedMaterial = profile.flame.material;
        }
    }
    Light source;
    MaterialPropertyBlock flameBlock;
    float age;
    Mesh originalMesh, privateMesh;
    Vector2[] originalUV, frameUV;
    int displayedFrame = -1;

    void OnEnable()
    {
        if (!profile) throw new System.InvalidOperationException(name + ": missing LightProfile");
        profile.Validate();
        source = GetComponent<Light>(); profile.Apply(source);
        if (pixelFlame) pixelFlame.GetComponent<Renderer>().enabled = profile.flame;
        if (pixelFlame && profile.flame && !privateMesh)
        {
            originalMesh = profile.flame.mesh;
            pixelFlame.GetComponent<Renderer>().sharedMaterial = profile.flame.material;
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
        if (GamePauseController.PresentationFrozen || Time.timeScale == 0) return;
        age += Time.deltaTime;
        Apply();
    }

    void Apply()
    {
        float multiplier = profile.Multiplier(age, phase);
        source.intensity = profile.intensity * multiplier;
        int frame = profile.flame ? profile.flame.FrameAt(age + flameTimeOffset) : -1;
        if (privateMesh && displayedFrame != frame)
        {
            var from = profile.flame.meshUV; var to = profile.flame.frames[frame].uv;
            for (int i = 0; i < frameUV.Length; i++)
                frameUV[i] = to.position + Vector2.Scale(new Vector2((originalUV[i].x - from.x) / from.width,
                    (originalUV[i].y - from.y) / from.height), to.size);
            privateMesh.uv = frameUV;
            displayedFrame = frame;
        }
        if(flameBlock==null)flameBlock=new MaterialPropertyBlock();
        if (!profile.synchronizeEmission) return;
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
        if (source) source.intensity = profile.intensity;
    }

    void OnDestroy()
    {
        if (pixelFlame && pixelFlame.sharedMesh == privateMesh) pixelFlame.sharedMesh = originalMesh;
        if (privateMesh) Destroy(privateMesh);
    }
}
