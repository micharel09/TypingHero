using UnityEngine;

// Light intensity varies in world space; pause and cinematic hiding preserve phase.
[RequireComponent(typeof(Light))]
public sealed class DungeonTorchLight : MonoBehaviour
{
    [SerializeField, Min(0)] float baseIntensity = 22;
    [SerializeField] float phase;
    [SerializeField] Renderer[] flameRenderers = new Renderer[0];
    Light source;
    MaterialPropertyBlock flameBlock;
    float age;

    void OnEnable()
    {
        source = GetComponent<Light>();
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
        float wave = .65f * Mathf.Sin(age * 12.57f + phase)
            + .35f * Mathf.Sin(age * 19.92f + phase * 1.7f);
        float multiplier=1+.05f*wave;
        source.intensity=baseIntensity*multiplier;
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
}
