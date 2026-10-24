#if UNITY_EDITOR || TYPINGHERO_DEPTH_DEMO
using System;
using UnityEngine;
using UnityEngine.Rendering;

// Visual laboratory only; excluded from normal players, no gameplay bridge.
[DefaultExecutionOrder(31000)]
public sealed class SpriteDepthDemo : MonoBehaviour
{
    [Serializable] public struct Pose
    {
        public Sprite sprite;
        public Texture2D depth;
        public Vector3 position, scale;
        public Quaternion rotation;
        public float groundY;
    }
    public Pose[] poses;
    public Shader diagnosticShader;
    public Mesh neutralFloorMesh;
    DungeonWorld world;
    SpriteRenderer actor;
    SpriteDepthBinding binding;
    SpriteDepthLibrary library;
    Material material, diagnostic, floorMaterial;
    GameObject neutralFloor;
    Light key, fill;
    int selected, mode=2, diagnosticMode;
    bool mirrored, dungeon, orbit, fillEnabled, zoom;
    float angle=90, lightDepth=2;
    float previousTimeScale;
    readonly string[] actorNames={"Hero - combo_3 F1","Sword skeleton - Attack1 F2"};
    readonly string[] modes={"1 - Flat normal / OFF","2 - Current normal / OFF","3 - Self-shadow / ON"};
    readonly string[] diagnostics={"Final image","Depth","Occlusion mask","Local ray"};
    GUIStyle label, title, button;
    void Start()
    {
        previousTimeScale=Time.timeScale; Time.timeScale=0;
        world=FindObjectOfType<DungeonWorld>();
        material=new Material(world.Profile.actorMaterial);
        diagnostic=new Material(diagnosticShader);
        var host=new GameObject("Self-shadow specimen",typeof(SpriteRenderer),typeof(SpriteDepthBinding));
        actor=host.GetComponent<SpriteRenderer>(); actor.sharedMaterial=material;
        actor.shadowCastingMode=ShadowCastingMode.TwoSided; actor.receiveShadows=true;
        binding=host.GetComponent<SpriteDepthBinding>();
        library=ScriptableObject.CreateInstance<SpriteDepthLibrary>();
        library.frames=new SpriteDepthLibrary.Frame[poses.Length];
        for(int i=0;i<poses.Length;i++)
        {
            var sprite=poses[i].sprite; var size=new Vector2(sprite.texture.width,sprite.texture.height);
            var rect=sprite.rect;
            library.frames[i]=new SpriteDepthLibrary.Frame { sprite=sprite,depth=poses[i].depth,textureSize=size,
                pixelsPerUnit=sprite.pixelsPerUnit,uvRect=new Vector4(rect.xMin/size.x,rect.yMin/size.y,rect.xMax/size.x,rect.yMax/size.y) };
        }
        binding.Bind(library.CreateLookup());
        key=NewLight("White diagnostic key",Color.white,new Vector3(3,0,-2));
        fill=NewLight("Independent blue fill",new Color(.25f,.5f,1),new Vector3(-3,0,-2));
        neutralFloor=new GameObject("Neutral receiver",typeof(MeshFilter),typeof(MeshRenderer));
        neutralFloor.GetComponent<MeshFilter>().sharedMesh=neutralFloorMesh;
        neutralFloor.name="Neutral receiver"; neutralFloor.transform.localScale=Vector3.one*3;
        floorMaterial=new Material(Shader.Find("Universal Render Pipeline/Lit"));
        floorMaterial.SetColor("_BaseColor",new Color(.2f,.2f,.2f)); floorMaterial.SetFloat("_Smoothness",0);
        neutralFloor.GetComponent<Renderer>().sharedMaterial=floorMaterial;
        world.Arena.player=host.transform; world.Arena.enemy=host.transform;
        ApplyPose(); ApplyRoom(); ApplyMode();
        Debug.Log("DEPTH_DEMO_READY: two frozen poses; production camera/material; no gameplay");
    }
    Light NewLight(string name,Color color,Vector3 position)
    {
        var light=new GameObject(name,typeof(Light)).GetComponent<Light>();
        light.type=LightType.Point; light.color=color; light.intensity=24; light.range=20;
        light.shadows=LightShadows.Hard; light.shadowBias=.015f; light.shadowNormalBias=.02f;
        light.renderMode=LightRenderMode.ForcePixel; light.transform.position=position;
        return light;
    }
    void ApplyPose()
    {
        var pose=poses[selected]; actor.sprite=pose.sprite; actor.flipX=mirrored;
        actor.transform.SetPositionAndRotation(pose.position,pose.rotation); actor.transform.localScale=pose.scale;
        neutralFloor.transform.position=new Vector3(0,pose.groundY-.01f,0);
        binding.Refresh(true);
    }
    void ApplyMode()
    {
        material.SetFloat("_NormalStrength",mode==0 ? 0 : world.Profile.actorMaterial.GetFloat("_NormalStrength"));
        material.SetFloat("_SelfShadowStrength",mode==2 ? .65f : 0);
        material.SetFloat("_DepthScale",.15f); material.SetFloat("_SelfShadowBias",.01f);
        diagnostic.SetFloat("_SelfShadowStrength",.65f); diagnostic.SetFloat("_DepthScale",.15f);
        diagnostic.SetFloat("_SelfShadowBias",.01f); diagnostic.SetFloat("_DiagnosticMode",diagnosticMode);
        actor.sharedMaterial=diagnosticMode==0 ? material : diagnostic;
        binding.Refresh(true);
    }
    void ApplyRoom()
    {
        world.environment.gameObject.SetActive(dungeon); neutralFloor.SetActive(!dungeon);
        key.enabled=!dungeon; fill.enabled=!dungeon && fillEnabled;
        if(dungeon) world.ApplyEnvironment();
        else { RenderSettings.ambientMode=AmbientMode.Flat; RenderSettings.ambientLight=Color.black; RenderSettings.fog=false; }
        // Room mode preserves the saved volume and presentation; neutral mode
        // disables its volume with the environment, without changing the camera.
    }
    void Update()
    {
        if(!actor) return;
        if(orbit && !dungeon) angle=(angle+Time.unscaledDeltaTime*35)%360;
        var center=actor.bounds.center;
        var radians=angle*Mathf.Deg2Rad;
        key.transform.position=center+new Vector3(Mathf.Cos(radians)*3,Mathf.Sin(radians)*3,-lightDepth);
        fill.transform.position=center+new Vector3(-Mathf.Cos(radians)*3,-Mathf.Sin(radians)*3,-2);
        diagnostic.SetVector("_ProbePosition",key.transform.position);
    }
    void OnGUI()
    {
        if(!actor) return;
        // The production camera clears only its combat viewport. Clear the lab's
        // unused right margin too, otherwise a hidden zoom leaves a stale image.
        float viewportRight=world.Arena.PresentationRect.xMax*Screen.width;
        if(viewportRight<Screen.width)
        {
            var previousColor=GUI.color;
            GUI.color=Color.black;
            GUI.DrawTexture(new Rect(viewportRight,0,Screen.width-viewportRight,Screen.height),Texture2D.whiteTexture);
            GUI.color=previousColor;
        }
        if(label==null)
        {
            label=new GUIStyle(GUI.skin.label) { fontSize=16,wordWrap=true };
            title=new GUIStyle(label) { fontSize=21,fontStyle=FontStyle.Bold };
            button=new GUIStyle(GUI.skin.button) { fontSize=15,padding=new RectOffset(12,12,9,9) };
        }
        GUI.Box(new Rect(10,10,Screen.width-20,146),GUIContent.none);
        GUILayout.BeginArea(new Rect(24,16,Screen.width-48,136));
        GUILayout.Label("CLACKBLADE - Self-shadow lab - 2 frozen poses",title);
        GUILayout.BeginHorizontal();
        int next=GUILayout.SelectionGrid(selected,actorNames,2,button);
        if(next!=selected) { selected=next; ApplyPose(); }
        if(GUILayout.Button(mirrored ? "Facing: mirror" : "Facing: original",button)) { mirrored=!mirrored; ApplyPose(); }
        if(GUILayout.Button(dungeon ? "Scene: dungeon" : "Scene: white light",button)) { dungeon=!dungeon; diagnosticMode=0; ApplyRoom(); ApplyMode(); }
        GUILayout.EndHorizontal();
        next=GUILayout.SelectionGrid(mode,modes,3,button);
        if(next!=mode) { mode=next; diagnosticMode=0; ApplyMode(); }
        GUILayout.Label("A/B: fixed pose, camera, lighting - Strength 0.65 / Depth 0.15 / Bias 0.01 - 8 samples",label);
        GUILayout.EndArea();
        GUI.Box(new Rect(10,Screen.height-162,Screen.width-20,152),GUIContent.none);
        GUILayout.BeginArea(new Rect(24,Screen.height-156,Screen.width-48,142));
        GUILayout.BeginHorizontal(); GUI.enabled=!dungeon;
        foreach(var entry in new[] { "Left","Right","Top","Bottom" })
            if(GUILayout.Button(entry,button)) { angle=entry=="Left" ? 180 : entry=="Right" ? 0 : entry=="Top" ? 90 : 270; orbit=false; }
        if(GUILayout.Button(orbit ? "Pause light" : "Move light",button)) orbit=!orbit;
        if(GUILayout.Button(fillEnabled ? "Fill: ON" : "Fill: OFF",button)) { fillEnabled=!fillEnabled; ApplyRoom(); }
        if(GUILayout.Button(lightDepth==2 ? "Angle: standard" : "Grazing (diagnostic)",button)) lightDepth=lightDepth==2 ? .5f : 2;
        GUI.enabled=true; if(GUILayout.Button(zoom ? "Hide zoom" : "Pixel zoom",button)) zoom=!zoom;
        GUILayout.EndHorizontal();
        GUI.enabled=!dungeon; next=GUILayout.SelectionGrid(diagnosticMode,diagnostics,4,button);
        if(next!=diagnosticMode) { diagnosticMode=next; ApplyMode(); }
        GUI.enabled=true;
        GUILayout.Label(dungeon ? "Original dungeon light at frozen phase. Animation and full-game performance are not validated." :
            "Live white light. Depth/mask are diagnostic views; judge the Final image.",label);
        GUILayout.EndArea();
        if(zoom && world.Arena.WorldTexture)
        {
            var texture=world.Arena.WorldTexture; var uv=world.view.WorldToViewportPoint(actor.bounds.center);
            int x=Mathf.Clamp(Mathf.RoundToInt(uv.x*texture.width)-32,0,texture.width-64);
            int y=Mathf.Clamp(Mathf.RoundToInt(uv.y*texture.height)-32,0,texture.height-64);
            GUI.Box(new Rect(Screen.width-286,172,276,302),GUIContent.none);
            GUI.Label(new Rect(Screen.width-278,178,260,35),"World pixels x4 - before post",label);
            GUI.DrawTextureWithTexCoords(new Rect(Screen.width-276,214,256,256),texture,
                new Rect((float)x/texture.width,(float)y/texture.height,64f/texture.width,64f/texture.height));
        }
    }
    void OnDestroy()
    {
        Time.timeScale=previousTimeScale;
        if(material) Destroy(material); if(diagnostic) Destroy(diagnostic);
        if(floorMaterial) Destroy(floorMaterial); if(library) Destroy(library);
        if(actor) Destroy(actor.gameObject); if(key) Destroy(key.gameObject); if(fill) Destroy(fill.gameObject);
        if(neutralFloor) Destroy(neutralFloor);
    }
}
#endif
