using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public sealed partial class ParryClashTests
{
#if UNITY_EDITOR
    [TestCase(false), TestCase(true), Category("Quick")]
    public void RepeatedCombatBuilderRemovesReceivedRouteAndPreservesOutgoingAndParry(bool legacyRoute)
    {
        PrepareVoxel();
        var root = UnityEditor.PrefabUtility.LoadPrefabContents("Assets/Characters/Player/Player.prefab");
        try
        {
            var health = root.GetComponentInChildren<PlayerHealth>(true);
            if (legacyRoute && !health.GetComponent<PlayerDamageVoxel>())
            {
                var received = new GameObject("ReceivedImpact");
                received.transform.SetParent(health.transform, false);
                var emitter = received.AddComponent<HitSparkSpawner>();
                Set(emitter, "sparkPrefab", UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Art/VFX/CombatVoxel/Received.prefab"));
                Set(emitter, "lethalPrefab", UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Art/VFX/CombatVoxel/ReceivedLethal.prefab"));
                var damage = health.gameObject.AddComponent<PlayerDamageVoxel>();
                Set(damage, "health", health); Set(damage, "emitter", emitter);
            }
            System.Type builder = null;
            foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
                if ((builder = assembly.GetType("CombatFeedbackBuild")) != null) break;
            Assert.That(builder, Is.Not.Null);
            var configure = builder.GetMethod("Configure", BindingFlags.Static | BindingFlags.NonPublic);
            for (int repeat = 0; repeat < 2; repeat++)
            {
                configure.Invoke(null, new object[] { root, null, null, null });
                Assert.That(root.GetComponentsInChildren<PlayerDamageVoxel>(true), Is.Empty);
                Assert.That(health.transform.Find("ReceivedImpact"), Is.Null);
                var outgoing = root.GetComponentsInChildren<HitSparkSpawner>(true);
                Assert.That(outgoing.Length, Is.EqualTo(1));
                var so = new UnityEditor.SerializedObject(outgoing[0]);
                foreach (var role in new[] { ("sparkPrefab", "NormalHitSpark"), ("lethalPrefab", "Lethal") })
                    Assert.That(so.FindProperty(role.Item1).objectReferenceValue,
                        Is.EqualTo(UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Art/VFX/CombatVoxel/" + role.Item2 + ".prefab")));
                var variants = so.FindProperty("slayerPrefabs");
                Assert.That(variants.arraySize, Is.EqualTo(1));
                Assert.That(variants.GetArrayElementAtIndex(0).objectReferenceValue,
                    Is.EqualTo(UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Art/VFX/CombatVoxel/Slayer.prefab")));
                var hitbox = new UnityEditor.SerializedObject(root.GetComponentInChildren<PlayerWeaponHitbox>(true));
                Assert.That(hitbox.FindProperty("hitSpark").objectReferenceValue, Is.EqualTo(outgoing[0]));
                var parry = new UnityEditor.SerializedObject(root.GetComponentInChildren<ParrySparkVFX_Pooled>(true));
                Assert.That(parry.FindProperty("sparkPrefab").objectReferenceValue,
                    Is.EqualTo(UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Art/VFX/ParryClash/ParryClash.prefab")));
                Assert.That(parry.FindProperty("sparkPreset").intValue, Is.EqualTo((int)ParryClashBurst.Preset.BCompact));
                Assert.That(parry.FindProperty("parrySystem").objectReferenceValue, Is.Not.Null);
            }
        }
        finally { UnityEditor.PrefabUtility.UnloadPrefabContents(root); }
    }
#endif
    static void PrepareVoxel()
    {
        foreach (string name in new[] { "ParryClashAssets", "CombatVoxelAssets", "SlashVolumeAssets", "NormalHitSparkAssets" })
        foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
        {
            var type = assembly.GetType(name);
            if (type != null) { type.GetMethod("Prepare", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, null); break; }
        }
    }
    VoxelImpactBurst Voxel(string name)
    {
        PrepareVoxel();
#if UNITY_EDITOR
        var go = Object.Instantiate(UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Art/VFX/CombatVoxel/" + name + ".prefab"));
        owned.Add(go); go.SetActive(true); return go.GetComponent<VoxelImpactBurst>();
#else
        return null;
#endif
    }
    [Test, Category("Quick")]
    public void VoxelProfilesHaveVolumeSettleAndLeaveGameplayRandomUntouched()
    {
        var random = Random.state;
        string[] names = { "Normal", "Slayer", "Lethal", "Received", "ReceivedLethal" };
        for (int role = 0; role < names.Length; role++)
        {
            var burst = Voxel(names[role]); Random.InitState(734); var saved = Random.state;
            burst.Emit(Vector3.up, 1, 0, 7, Color.magenta);
            float next = Random.value; Random.state = saved; Assert.That(Random.value, Is.EqualTo(next));
            Assert.That(burst.ParticleCount, Is.EqualTo(new[] {6,8,14,4,6}[role]));
            var ps = burst.GetComponentInChildren<ParticleSystem>(); var particles = new ParticleSystem.Particle[14];
            var rotations = new Dictionary<uint, Vector3>(); bool rested = false;
            for (int frame = 0; frame < 250; frame++)
            {
                int count = ps.GetParticles(particles);
                for (int i = 0; i < count; i++)
                {
                    var p = particles[i]; var q = Quaternion.Euler(p.rotation3D); var half = p.startSize3D * .5f;
                    float support = Mathf.Abs((q * Vector3.right).y)*half.x + Mathf.Abs((q * Vector3.up).y)*half.y + Mathf.Abs((q * Vector3.forward).y)*half.z;
                    Assert.That(p.position.y - support, Is.GreaterThanOrEqualTo(-.00001f));
                    if (p.rotation3D.x == 0 && p.rotation3D.z == 0)
                    {
                        rested = true;
                        if (rotations.TryGetValue(p.randomSeed, out var r)) Assert.That(p.rotation3D, Is.EqualTo(r));
                        rotations[p.randomSeed] = p.rotation3D;
                    }
                }
                burst.Advance(.01f);
            }
            if (role != 0) Assert.That(rested, Is.True); Assert.That(burst.ParticleCount, Is.Zero);
        }
        Random.state = random;
    }
    [Test, Category("Quick")]
    public void VoxelImpactPrecedenceAndEightLeaseReclaimDoNotStackRoles()
    {
        PrepareVoxel(); var host = new GameObject("voxel spawner"); host.SetActive(false); owned.Add(host);
        var spawner = host.AddComponent<HitSparkSpawner>();
#if UNITY_EDITOR
        GameObject Load(string role) => UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Art/VFX/CombatVoxel/"+role+".prefab");
        Set(spawner,"sparkPrefab",Load("Normal")); Set(spawner,"slayerPrefabs",new[]{Load("Slayer")}); Set(spawner,"lethalPrefab",Load("Lethal"));
#endif
        host.SetActive(true);
        var active=(List<GameObject>)typeof(HitSparkSpawner).GetField("active",Flags).GetValue(spawner);
        for(int n=0;n<12;n++)
        {
            spawner.SpawnImpact(Vector2.up,host.transform,true,n%3==0);
            Assert.That(active.Count,Is.EqualTo(Mathf.Min(n+1,8)));
            Assert.That(active[active.Count-1].GetComponent<VoxelImpactBurst>().Role,Is.EqualTo(n%3==0?2:1));
            int particles=0;foreach(var go in active)particles+=go.GetComponent<VoxelImpactBurst>().ParticleCount;
            Assert.That(particles,Is.LessThanOrEqualTo(112));
        }
        spawner.SpawnImpact(Vector2.up,host.transform,false,true);
        var normalLethal=active[active.Count-1].GetComponent<VoxelImpactBurst>();var heat=new List<Vector4>();
        foreach(var ps in normalLethal.Systems){ps.GetCustomParticleData(heat,ParticleSystemCustomData.Custom1);foreach(var temperature in heat)Assert.That(temperature.x,Is.Zero,"normal lethal has no Slayer glow");}
        host.SetActive(false);Assert.That(active,Is.Empty);
    }
    [UnityTest, Category("Quick")]
    public IEnumerator DamageNotificationOccursOnlyForAcceptedDamageAndBeforeLethalEnd()
    {
        var host = new GameObject("received damage");owned.Add(host);var health=host.AddComponent<PlayerHealth>();
        int calls=0; bool lethal=false;health.DamageAccepted+=(p,last)=>{calls++;lethal=last;};
        health.TakeDamage(5,Vector2.right);Assert.That(calls,Is.EqualTo(1));
        health.TakeDamage(5,Vector2.right);Assert.That(calls,Is.EqualTo(1),"iframe blocks VFX too");
        GamePauseController.SetTutorialReading(true);health.TakeDamage(100,Vector2.right);Assert.That(calls,Is.EqualTo(1));
        GamePauseController.SetTutorialReading(false);yield return new WaitForSeconds(.06f);
        health.TakeDamage(100,Vector2.right);Assert.That(calls,Is.EqualTo(2));Assert.That(lethal,Is.True);
    }
    [UnityTest, Category("Quick")]
    public IEnumerator AfterimageCapReclaimsOldGenerationsWithoutReturningNewLeases()
    {
        var host=new GameObject("bounded ghosts");host.SetActive(false);owned.Add(host);
        var sr=host.AddComponent<SpriteRenderer>();
        var tex=new Texture2D(2,2);var sprite=Sprite.Create(tex,new Rect(0,0,2,2),Vector2.one*.5f);sr.sprite=sprite;
        var pool=host.AddComponent<SlayerAfterimagePool>();Set(pool,"source",sr);Set(pool,"burstCount",1);Set(pool,"burstInterval",0f);
        host.SetActive(true);for(int n=0;n<36;n++)pool.SpawnBurstFromCurrentFrame();
        var live=(HashSet<GameObject>)typeof(SlayerAfterimagePool).GetField("activeImages",Flags).GetValue(pool);
        Assert.That(live.Count,Is.EqualTo(12));yield return new WaitForSecondsRealtime(.25f);Assert.That(live,Is.Empty);
        Object.Destroy(sprite);Object.Destroy(tex);
    }
    [Test, Category("Quick")]
    public void LethalVoxelKeepsAttackTintWhenDamageExitsSlayer()
    {
        PrepareVoxel();var player=new GameObject("tint snapshot");player.SetActive(false);owned.Add(player);
        player.AddComponent<BoxCollider2D>();var mode=player.AddComponent<PlayerSlayerMode>();
        var images=player.AddComponent<SlayerAfterimagePool>();Set(images,"useSaturationBoost",false);
        var gradient=new Gradient();gradient.SetKeys(new[]{new GradientColorKey(Color.blue,0),new GradientColorKey(Color.magenta,.4f),new GradientColorKey(Color.magenta,1)},new[]{new GradientAlphaKey(1,0),new GradientAlphaKey(1,1)});
        Set(images,"comboGradient",gradient);var emitter=player.AddComponent<HitSparkSpawner>();
#if UNITY_EDITOR
        Set(emitter,"lethalPrefab",UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Art/VFX/CombatVoxel/Lethal.prefab"));
#endif
        var box=player.AddComponent<PlayerWeaponHitbox>();box.targetLayers=1;Set(box,"hitSpark",emitter);Set(box,"slayer",mode);
        player.SetActive(true);Set(mode,"_active",true);Set(mode,"_typedSteps",8);images.CaptureImpactTint(8);
        var enemy=new GameObject("lethal target");owned.Add(enemy);var damage=enemy.AddComponent<FeedbackDamageTarget>();var target=enemy.AddComponent<BoxCollider2D>();
        damage.onDamage=()=>{damage.dead=true;Set(mode,"_active",false);Set(mode,"_typedSteps",0);SlayerModeSignals.SetActive(false);};
        box.BeginAttack(1);typeof(PlayerWeaponHitbox).GetMethod("OnTriggerEnter2D",Flags).Invoke(box,new object[]{target});
        var active=(List<GameObject>)typeof(HitSparkSpawner).GetField("active",Flags).GetValue(emitter);
        Assert.That(active.Count,Is.EqualTo(1));var burst=active[0].GetComponent<VoxelImpactBurst>();Assert.That(burst.Role,Is.EqualTo(2));
        bool magenta=false;var buffer=new ParticleSystem.Particle[14];
        foreach(var ps in burst.Systems){int n=ps.GetParticles(buffer);for(int i=0;i<n;i++)if(buffer[i].startColor.r==255&&buffer[i].startColor.b==255&&buffer[i].startColor.g==0)magenta=true;}
        Assert.That(magenta,Is.True,"lethal uses the pre-exit attack tint, not reset combo zero");
    }
    [Test, Category("Quick")]
    public void WeakImpactCannotEvictEightStrongParryPulses()
    {
        var lightType=System.Type.GetType("UnityEngine.Rendering.Universal.Light2D, Unity.RenderPipelines.Universal.Runtime");
        var host=new GameObject("mixed lights");host.SetActive(false);owned.Add(host);var light=host.AddComponent(lightType);
        var rig=host.AddComponent<ClashLightingRig>(); rig.profile = LightingTestAssets.Definition.combat;Set(rig,"clashLight",light);host.SetActive(true);
        for(int n=0;n<8;n++)Burst().Emit(Vector3.one,1,0,ParryClashBurst.Preset.BCompact);
        var weak=Voxel("Normal");weak.Emit(Vector3.up,1,0,9,Color.white);
        var pulses=(System.Array)typeof(ClashLightingRig).GetField("pulses",Flags).GetValue(rig);
        for(int i=0;i<pulses.Length;i++)Assert.That(pulses.GetValue(i).GetType().GetField("owner").GetValue(pulses.GetValue(i)),Is.TypeOf<ParryClashBurst>());
        Assert.That((float)lightType.GetProperty("intensity").GetValue(light),Is.EqualTo(5));
    }
    [Test, Category("Quick")]
    public void ForwardArenaLightUsesCompactTimingAndClearsOnDisable()
    {
        var host=new GameObject("forward arena pulse");host.SetActive(false);owned.Add(host);
        var light=host.AddComponent<Light>();var rig=host.AddComponent<ClashLightingRig>(); rig.profile = LightingTestAssets.Definition.combat;
        Set(rig,"combatLight",light);host.SetActive(true);
        var burst=Burst();var point=new Vector3(2,3,0);burst.Emit(point,1,0,ParryClashBurst.Preset.BCompact);
        Assert.That(light.intensity,Is.EqualTo(5));Assert.That(light.transform.position,Is.EqualTo(point+new Vector3(0,2,-4)));
        var pulses=(System.Array)typeof(ClashLightingRig).GetField("pulses",Flags).GetValue(rig);
        foreach(var sample in new[]{(.07f,5f),(.175f,2.5f),(.28f,0f)})
        {
            var pulse=pulses.GetValue(0);pulse.GetType().GetField("age").SetValue(pulse,sample.Item1);pulses.SetValue(pulse,0);
            typeof(ClashLightingRig).GetMethod("Render",Flags).Invoke(rig,null);
            Assert.That(light.intensity,Is.EqualTo(sample.Item2).Within(.0001f));
        }
        typeof(ClashLightingRig).GetMethod("LateUpdate",Flags).Invoke(rig,null);
        Assert.That(Shader.GetGlobalFloat("_VoxelWorld3D"),Is.EqualTo(1));
        host.SetActive(false);Assert.That(light.intensity,Is.Zero);Assert.That(Shader.GetGlobalFloat("_VoxelWorld3D"),Is.Zero);
    }
    [Test, Category("Quick")]
    public void SlashLibraryFollowsActualCelAndClearsWhenCelHasNoTrail()
    {
        PrepareVoxel();
#if UNITY_EDITOR
        var library=UnityEditor.AssetDatabase.LoadAssetAtPath<SlashLibrary>("Assets/Art/VFX/SlashVolume/SlashLibrary.asset");
        Assert.That(library.frames.Length,Is.GreaterThanOrEqualTo(20));
        var host=new GameObject("mesh slash");host.SetActive(false);owned.Add(host);var sr=host.AddComponent<SpriteRenderer>();
        var slash=host.AddComponent<SlashVolume>();Set(slash,"source",sr);Set(slash,"library",library);
        host.SetActive(true);sr.sprite=library.frames[0].sprite;
        var update=typeof(SlashVolume).GetMethod("LateUpdate",Flags);update.Invoke(slash,null);
        Assert.That(host.GetComponentsInChildren<MeshRenderer>()[0].enabled||host.GetComponentsInChildren<MeshRenderer>()[1].enabled,Is.True);
        foreach(var frame in library.frames)foreach(var mesh in new[]{frame.front,frame.back})if(mesh)
            Assert.That(mesh.bounds.size.z,Is.GreaterThanOrEqualTo(1f/32-.00001f));
        sr.sprite=null;update.Invoke(slash,null);foreach(var renderer in host.GetComponentsInChildren<MeshRenderer>())Assert.That(renderer.enabled,Is.False);
#endif
    }
}
