using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


namespace OccaSoftware.Altos.Runtime
{
    /// <summary>
    /// Renders the Altos Sky, Sky Objects, Stars, Clouds, and Cloud Shadows.
    /// </summary>
	internal sealed class AltosRenderFeature : ScriptableRendererFeature
    {
        private class StarRenderPass
        {
            public StarRenderPass()
            {
                Init();
            }

            public void Dispose()
			{
                CoreUtils.Destroy(starMaterial);
                starMaterial = null;
			}

            private Material starMaterial;

            private Material GetStarMaterial()
			{
                if(starMaterial == null)
				{
                    starMaterial = CoreUtils.CreateEngineMaterial(skyDirector.data.shaders.starShader);
				}

                return starMaterial;
			}

            private Mesh mesh = null;
            private Mesh Mesh
            {
                get
                {
                    if (mesh == null)
                        mesh = Helpers.CreateQuad();

                    return mesh;
                }
            }

            private int initialSeed = 1;
            private ComputeBuffer meshPropertiesBuffer = null;
            private ComputeBuffer argsBuffer = null;


            private Texture2D starTexture;

            private Texture2D white = null;
            private Texture2D White
            {
                get
                {
                    if (white == null)
                    {
                        white = new Texture2D(1, 1, TextureFormat.RGBA32, false);
                        white.SetPixel(0, 0, Color.white);
                        white.Apply();
                    }

                    return white;
                }
            }

            private bool initialized = false;

            private struct MeshProperties
            {
                public Matrix4x4 mat;
                public Vector3 color;
                public float brightness;
                public float id;

                public static int Size()
                {
                    return
                        sizeof(float) * 4 * 4 + // matrix
                        sizeof(float) * 3 +     // color
                        sizeof(float) +         // brightness
                        sizeof(float);          // id
                }
            }

            private void Init()
            {
                Cleanup();
                InitializeBuffers();
            }

            private void Cleanup()
            {
                ReleaseBuffer(ref argsBuffer);
                ReleaseBuffer(ref meshPropertiesBuffer);
            }


            private void ReleaseBuffer(ref ComputeBuffer b)
            {
                if (b != null)
                {
                    b.Release();
                    b = null;
                }
            }

            public void Draw(ref CommandBuffer cmd, SkyDefinition skyboxDefinition)
            {
                if (!initialized || skyDirector.starDefinition.IsDirty())
				{
                    Init();
                    initialized = true;
				}

                if (Mesh == null || GetStarMaterial() == null || argsBuffer == null || meshPropertiesBuffer == null)
                {
                    return;
                }

				if (skyDirector.starDefinition.positionStatic)
				{
                    GetStarMaterial().SetFloat(ShaderParams._EarthTime, 0);
				}
				else
				{
                    float currentTime = 0f;
                    if (skyboxDefinition != null)
                        currentTime = skyboxDefinition.CurrentTime;

                    GetStarMaterial().SetFloat(ShaderParams._EarthTime, currentTime);
                }

                GetStarMaterial().SetFloat(ShaderParams._Brightness, skyDirector.starDefinition.brightness);
                GetStarMaterial().SetFloat(ShaderParams._FlickerFrequency, skyDirector.starDefinition.flickerFrequency);
                GetStarMaterial().SetFloat(ShaderParams._FlickerStrength, skyDirector.starDefinition.flickerStrength);
                GetStarMaterial().SetFloat(ShaderParams._Inclination, -skyDirector.starDefinition.inclination);
                GetStarMaterial().SetColor(ShaderParams._Color, skyDirector.starDefinition.color);
                starTexture = skyDirector.starDefinition.texture == null ? White : skyDirector.starDefinition.texture;
                GetStarMaterial().SetTexture(ShaderParams._MainTex, starTexture);
                GetStarMaterial().SetBuffer(ShaderParams._Properties, meshPropertiesBuffer);
                cmd.DrawMeshInstancedIndirect(Mesh, 0, GetStarMaterial(), -1, argsBuffer);
            }

            private static class ShaderParams
			{
                public static int _EarthTime = Shader.PropertyToID("_EarthTime");
                public static int _Brightness = Shader.PropertyToID("_Brightness");
                public static int _FlickerFrequency = Shader.PropertyToID("_FlickerFrequency");
                public static int _FlickerStrength = Shader.PropertyToID("_FlickerStrength");
                public static int _MainTex = Shader.PropertyToID("_MainTex");
                public static int _Properties = Shader.PropertyToID("_Properties");
                public static int _Inclination = Shader.PropertyToID("_Inclination");
                public static int _Color = Shader.PropertyToID("_Color");
            }

            private void InitializeBuffers()
            {
                if (skyDirector.starDefinition == null)
                    return;

                uint[] args = new uint[5] { 0, 0, 0, 0, 0 };
                args[0] = (uint)Mesh.GetIndexCount(0);
                args[1] = (uint)skyDirector.starDefinition.count;
                argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
                argsBuffer.SetData(args);

                // Initialize buffer with the given population.
                MeshProperties[] meshPropertiesArray = new MeshProperties[skyDirector.starDefinition.count];
                Random.InitState(initialSeed);
                for (int i = 0; i < skyDirector.starDefinition.count; i++)
                {
                    MeshProperties meshProperties = new MeshProperties();
                    Vector3 position = Random.onUnitSphere * 100f;
                    Quaternion rotation = Quaternion.LookRotation(Vector3.zero - position, Random.onUnitSphere);
                    Vector3 scale = Vector3.one * Random.Range(1f, 2f) * 0.1f * skyDirector.starDefinition.size;

                    meshProperties.mat = Matrix4x4.TRS(position, rotation, scale);

					if (skyDirector.starDefinition.automaticColor)
					{
                        float temperature = Helpers.GetStarTemperature(Random.Range(0f, 1f));
                        meshProperties.color = Helpers.GetBlackbodyColor(temperature);
                    }
					else
					{
                        meshProperties.color = new Vector3(1,1,1);
					}

					if (skyDirector.starDefinition.automaticBrightness)
					{
                        meshProperties.brightness = Helpers.GetStarBrightness(Random.Range(0f, 1f));
                    }
					else
					{
                        meshProperties.brightness = 1f;
					}
                    
                    meshProperties.id = Random.Range(0f, 1f);
                    meshPropertiesArray[i] = meshProperties;
                }

                meshPropertiesBuffer = new ComputeBuffer(skyDirector.starDefinition.count, MeshProperties.Size());
                meshPropertiesBuffer.SetData(meshPropertiesArray);
                
                #if UNITY_EDITOR
                    UnityEditor.AssemblyReloadEvents.beforeAssemblyReload -= OnAssemblyReload;
                    UnityEditor.AssemblyReloadEvents.beforeAssemblyReload += OnAssemblyReload;
                    UnityEditor.AssemblyReloadEvents.afterAssemblyReload -= OnAssemblyReload;
                    UnityEditor.AssemblyReloadEvents.afterAssemblyReload += OnAssemblyReload;
                #endif
            }

            private void OnAssemblyReload()
			{
                Cleanup();
			}
        }


        private class SkyRenderPass : ScriptableRenderPass
		{
            private const string profilerTag = "Altos: Render Sky";
            private StarRenderPass stars = null;

            private RenderTargetHandle skyTarget;
            private const string skyTargetId = "_SkyTexture";

            private Material atmosphereMaterial;
            private Material backgroundMaterial;

            public void Dispose()
			{
                CoreUtils.Destroy(atmosphereMaterial);
                CoreUtils.Destroy(backgroundMaterial);
                atmosphereMaterial = null;
                backgroundMaterial = null;
			}

            public SkyRenderPass()
			{
                skyTarget.Init(skyTargetId);
            }

            public void Setup()
			{
                if (atmosphereMaterial == null) atmosphereMaterial = CoreUtils.CreateEngineMaterial(skyDirector.data?.shaders.atmosphereShader);
                if (backgroundMaterial == null) backgroundMaterial = CoreUtils.CreateEngineMaterial(skyDirector.data?.shaders.backgroundShader);
            }

            public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
			{
                RenderTextureDescriptor skyTargetDescriptor = cameraTextureDescriptor;
                skyTargetDescriptor.width = (int)(skyTargetDescriptor.width * 0.25f);
                skyTargetDescriptor.height = (int)(skyTargetDescriptor.height * 0.25f);

                cmd.GetTemporaryRT(skyTarget.id, skyTargetDescriptor);


                ConfigureClear(ClearFlag.All, Color.black);

                if(skyDirector.starDefinition != null)
				{
                    if (stars == null)
                        stars = new StarRenderPass();
                }
            }

            internal static class SkyShaderParams
            {
                public static int _Direction = Shader.PropertyToID("_Direction");
                public static int _Color = Shader.PropertyToID("_Color");
                public static int _Falloff = Shader.PropertyToID("_Falloff");
                public static int _Count = Shader.PropertyToID("_SkyObjectCount");
            }
            private static int _MAX_SKY_OBJECT_COUNT = 8;
            Vector4[] directions = new Vector4[_MAX_SKY_OBJECT_COUNT];
            Vector4[] colors = new Vector4[_MAX_SKY_OBJECT_COUNT];
            float[] falloffs = new float[_MAX_SKY_OBJECT_COUNT];

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
			{
                Profiler.BeginSample(profilerTag);
                CommandBuffer cmd = CommandBufferPool.Get(profilerTag);
				// Draw Background
				Matrix4x4 m = new Matrix4x4();
                m.SetTRS(renderingData.cameraData.worldSpaceCameraPos, Quaternion.identity, Vector3.one * renderingData.cameraData.camera.farClipPlane);
                cmd.DrawMesh(skyDirector.data?.meshes.skyboxMesh, m, backgroundMaterial, 0);


                // Draw Stars
                if(skyDirector.starDefinition != null)
                    stars.Draw(ref cmd, skyDirector.skyDefinition);

                // Draw Sun / Moon
                for (int i = 0; i < SkyObject.SkyObjects.Count; i++)
                {
                    if (SkyObject.SkyObjects[i] == null)
                        continue;

                    m = new Matrix4x4();
                    m.SetTRS(SkyObject.SkyObjects[i].positionRelative + renderingData.cameraData.worldSpaceCameraPos, SkyObject.SkyObjects[i].GetRotation(), Vector3.one * SkyObject.SkyObjects[i].CalculateSize());
                    
                    cmd.DrawMesh(SkyObject.SkyObjects[i].Quad, m, SkyObject.SkyObjects[i].GetMaterial());
                    directions[i] = SkyObject.SkyObjects[i].GetDirection();
                    colors[i] = SkyObject.SkyObjects[i].GetColor();
                    falloffs[i] = SkyObject.SkyObjects[i].GetFalloff();
                }

                atmosphereMaterial.SetVectorArray(SkyShaderParams._Direction, directions);
                atmosphereMaterial.SetVectorArray(SkyShaderParams._Color, colors);
                atmosphereMaterial.SetFloatArray(SkyShaderParams._Falloff, falloffs);
                atmosphereMaterial.SetInt(SkyShaderParams._Count, Mathf.Min(SkyObject.SkyObjects.Count, _MAX_SKY_OBJECT_COUNT));

                // Draw Sky
                if (skyDirector.skyDefinition != null)
				{
                    m.SetTRS(renderingData.cameraData.camera.transform.position, Quaternion.identity, Vector3.one * renderingData.cameraData.camera.farClipPlane);

                    cmd.SetGlobalColor(ShaderParams._HorizonColor, skyDirector.skyDefinition.SkyColors.equatorColor);
                    cmd.SetGlobalColor(ShaderParams._ZenithColor, skyDirector.skyDefinition.SkyColors.skyColor);
                    cmd.DrawMesh(skyDirector.data?.meshes.skyboxMesh, m, atmosphereMaterial, 0);

                    RenderTargetIdentifier source = renderingData.cameraData.renderer.cameraColorTarget;

                    // Draw Sky Target
                    cmd.SetRenderTarget(skyTarget.id);
                    cmd.ClearRenderTarget(true, true, Color.black);
                    cmd.DrawMesh(skyDirector.data?.meshes.skyboxMesh, m, atmosphereMaterial, 0);
                    cmd.SetGlobalInt("_HasSkyTexture", 1);

                    cmd.SetRenderTarget(source);
                }
                

                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();

                CommandBufferPool.Release(cmd);
                Profiler.EndSample();
            }

			public override void FrameCleanup(CommandBuffer cmd)
			{
                cmd.ReleaseTemporaryRT(skyTarget.id);
			}

            private static class ShaderParams
			{
                public static int _MainTex = Shader.PropertyToID("_MainTex");
                public static int _Color = Shader.PropertyToID("_Color");
                public static int _SunFalloff = Shader.PropertyToID("_SunFalloff");
                public static int _SunColor = Shader.PropertyToID("_SunColor");
                public static int _SunForward = Shader.PropertyToID("_SunForward");
                public static int _HorizonColor = Shader.PropertyToID("_HorizonColor");
                public static int _ZenithColor = Shader.PropertyToID("_ZenithColor");
			}
        }


        private static void AssignDefaultDescriptorSettings(ref RenderTextureDescriptor desc, RenderTextureFormat format = RenderTextureFormat.DefaultHDR)
        {
            desc.msaaSamples = 1;
            desc.depthBufferBits = 0;
            desc.useDynamicScale = false;
            desc.colorFormat = format;
        }

        private class VolumetricCloudsRenderPass : ScriptableRenderPass
        {
            #region RT Handles
            private RenderTargetHandle cloudTarget;
            private RenderTargetHandle temporalTarget;
            private RenderTargetHandle upscaleHalfRes;
            private RenderTargetHandle upscaleQuarterRes;
            private RenderTargetHandle mergeTarget;
            private RenderTargetHandle depthTex;
            #endregion

            #region Input vars
            private const string profilerTag = "Altos: Render Clouds";
            #endregion

            #region Shader Variable References
            private const string mergePassInputTextureShaderReference = "_MERGE_PASS_INPUT_TEX";
            private const string colorHistoryId = "_PREVIOUS_TAA_CLOUD_RESULTS";
            private const string depthId = "_DitheredDepthTex";
            #endregion

            #region Texture Ids
            private const string cloudId = "_CloudRenderPass";
            private const string upscaleHalfId = "_CloudUpscaleHalfResTarget";
            private const string upscaleQuarterId = "_CloudUpscaleQuarterResTarget";
            private const string taaId = "_CloudTemporalIntegration";
            private const string mergeId = "_CloudSceneMergeTarget";
            #endregion

            #region Materials
            private Material cloudTaa;
            private Material merge;
            private Material upscale;
            private Material ditherDepth;
            #endregion

            // RT Desc.
            private RenderTextureDescriptor cloudRenderDescriptor;

            // TAA Class
            private TemporalAA taa;

            public VolumetricCloudsRenderPass()
            {
                // Create TAA handler
                taa = new TemporalAA();

                // Setup RT Handles
                cloudTarget.Init(cloudId);
                upscaleHalfRes.Init(upscaleHalfId);
                upscaleQuarterRes.Init(upscaleQuarterId);
                temporalTarget.Init(taaId);
                mergeTarget.Init(mergeId);
                depthTex.Init(depthId);
            }

            public void Dispose()
			{
                CoreUtils.Destroy(merge);
                CoreUtils.Destroy(cloudTaa);
                CoreUtils.Destroy(upscale);
                CoreUtils.Destroy(ditherDepth);
                merge = null;
                cloudTaa = null;
                upscale = null;
                ditherDepth = null;

                if(taa != null)
				{
                    taa.Dispose();
				}
			}

            public void Setup()
			{
                // Setup Materials
                if (merge == null) merge = CoreUtils.CreateEngineMaterial(skyDirector.data.shaders.mergeClouds);
                if (cloudTaa == null) cloudTaa = CoreUtils.CreateEngineMaterial(skyDirector.data.shaders.temporalIntegration);
                if (upscale == null) upscale = CoreUtils.CreateEngineMaterial(skyDirector.data.shaders.upscaleClouds);
                if (ditherDepth == null) ditherDepth = CoreUtils.CreateEngineMaterial(skyDirector.data.shaders.ditherDepth);
            }


            private static class TimeManager
            {
                private static float managedTime = 0;
                private static int frameCount = 0;

                public static float ManagedTime
                {
                    get => managedTime;
                }

                public static int FrameCount
                {
                    get => frameCount;
                }

                public static void Update()
                {
                    float unityRealtimeSinceStartup = Time.realtimeSinceStartup;
                    int unityFrameCount = Time.frameCount;

                    bool newFrame;
                    if (Application.isPlaying)
                    {
                        newFrame = frameCount != unityFrameCount;
                        frameCount = unityFrameCount;
                    }
                    else
                    {
                        newFrame = (unityRealtimeSinceStartup - managedTime) > 0.0166f;
                        if (newFrame)
                            frameCount++;
                    }

                    if (newFrame)
                    {
                        managedTime = unityRealtimeSinceStartup;
                    }
                }
            }

            private class TemporalAA
			{
                public TemporalAA()
				{
                    // Constructor
				}

                private Dictionary<Camera, TAACameraData> temporalData = new Dictionary<Camera, TAACameraData>();
                public Dictionary<Camera, TAACameraData> TemporalData
				{
                    get => temporalData;
				}


                public void Cleanup()
				{
                    CleanupDictionary();
                }

                public void Dispose()
				{
                    foreach(KeyValuePair<Camera, TAACameraData> data in temporalData)
					{
                        data.Value.Dispose();
					}
				}


                internal class TAACameraData
                {
                    private int lastFrameUsed;
                    private RenderTexture colorTexture;
                    private string cameraName;
                    private Matrix4x4 prevViewProj;

                    public void Dispose()
					{
                        colorTexture.Release();
					}

                    public TAACameraData(int lastFrameUsed, RenderTexture colorTexture, string cameraName)
                    {
                        LastFrameUsed = lastFrameUsed;
                        ColorTexture = colorTexture;
                        CameraName = cameraName;
                        prevViewProj = Matrix4x4.identity;
                    }

                    public int LastFrameUsed
                    {
                        get => lastFrameUsed;
                        set => lastFrameUsed = value;
                    }

                    public RenderTexture ColorTexture
                    {
                        get => colorTexture;
                        set => colorTexture = value;
                    }


                    public string CameraName
                    {
                        get => cameraName;
                        set => cameraName = value;
                    }

                    public Matrix4x4 PrevViewProj
					{
                        get => prevViewProj;
                        set => prevViewProj = value;
					}
                }

                public bool IsTemporalDataValid(Camera camera, RenderTextureDescriptor descriptor)
                {
                    if (temporalData.TryGetValue(camera, out TAACameraData cameraData))
                    {
                        bool isColorTexValid = IsRenderTextureValid(descriptor, cameraData.ColorTexture);

                        if (isColorTexValid)
                            return true;
                    }

                    return false;


                    bool IsRenderTextureValid(RenderTextureDescriptor desc, RenderTexture rt)
                    {
                        if (rt == null)
                        {
                            return false;
                        }

                        bool rtWrongSize = (rt.width != desc.width || rt.height != desc.height) ? true : false;
                        if (rtWrongSize)
                        {
                            return false;
                        }

                        return true;
                    }
                }


                public void SetupTemporalData(Camera camera, RenderTextureDescriptor descriptor)
                {
                    SetupColorTexture(camera, descriptor, out RenderTexture color);

                    if (temporalData.ContainsKey(camera))
                    {
                        if (temporalData[camera].ColorTexture != null)
						{
                            temporalData[camera].ColorTexture.Release();
                            temporalData[camera].ColorTexture = null;
                        }

                        temporalData[camera].ColorTexture = color;
                    }
                    else
                    {
                        temporalData.Add(camera, new TAACameraData(TimeManager.FrameCount, color, camera.name));
                    }
                }

                private void SetupColorTexture(Camera camera, RenderTextureDescriptor descriptor, out RenderTexture renderTexture)
                {
                    descriptor.colorFormat = RenderTextureFormat.DefaultHDR;
                    descriptor.depthBufferBits = 0;
                    descriptor.msaaSamples = 1;
                    descriptor.useDynamicScale = false;

                    renderTexture = new RenderTexture(descriptor);
                    renderTexture.name = camera.name + " Color History";
                    renderTexture.filterMode = FilterMode.Point;
                    renderTexture.wrapMode = TextureWrapMode.Clamp;
                    renderTexture.Create();

                    Helpers.ClearRenderTexture(ref renderTexture);
                }


                private void CleanupDictionary()
                {
                    List<Camera> removeTargets = new List<Camera>();
                    foreach (KeyValuePair<Camera, TAACameraData> entry in temporalData)
                    {
                        if (entry.Value.LastFrameUsed < TimeManager.FrameCount - 10)
                        {
                            if (entry.Value.ColorTexture != null)
							{
                                entry.Value.ColorTexture.Release();
                                entry.Value.ColorTexture = null;
                            }
                                

                            removeTargets.Add(entry.Key);
                        }
                    }

                    for (int i = 0; i < removeTargets.Count; i++)
                    {
                        temporalData.Remove(removeTargets[i]);
                    }
                }


                public struct ProjectionMatrices
				{
                    public Matrix4x4 viewProjection;
                    public Matrix4x4 projection;
                    public Matrix4x4 inverseProjection;
				}

                public ProjectionMatrices GetCurrentViewProjection(Camera camera)
                {
                    ProjectionMatrices m;
                    var proj = camera.nonJitteredProjectionMatrix;
                    m.projection = proj;
                    m.inverseProjection = proj.inverse;

                    var view = camera.worldToCameraMatrix;
                    var viewProj = proj * view;
                    m.viewProjection = viewProj;

                    return m;
                }

                public Matrix4x4 GetPreviousViewProjection(Camera camera)
                {
                    if (temporalData.TryGetValue(camera, out TAACameraData data))
                    {
                        return data.PrevViewProj;
                    }
                    else
                    {
                        return Matrix4x4.identity;
                    }
                }

                public void SetPreviousViewProjection(Camera camera, Matrix4x4 currentViewProjection)
                {
                    if (temporalData.ContainsKey(camera))
                    {
                        temporalData[camera].PrevViewProj = currentViewProjection;
                    }
                }
            }


            public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
            {
                CloudShaderParamHandler.SetCloudMaterialSettings(skyDirector.cloudDefinition, cloudRenderMaterial);

                RenderTextureDescriptor rtDescriptor = cameraTextureDescriptor;
                AssignDefaultDescriptorSettings(ref rtDescriptor);


                ConfigureCloudRendering(cmd, rtDescriptor);
                ConfigureDepth(cmd, cameraTextureDescriptor);
                ConfigureUpscaling(cmd, rtDescriptor);


                cmd.GetTemporaryRT(temporalTarget.id, rtDescriptor, FilterMode.Point);
                cmd.GetTemporaryRT(mergeTarget.id, rtDescriptor, FilterMode.Point);

                void ConfigureCloudRendering(CommandBuffer cmd, RenderTextureDescriptor descriptor)
                {
                    cloudRenderDescriptor = descriptor;
                    cloudRenderDescriptor.height = (int)(cloudRenderDescriptor.height * skyDirector.cloudDefinition.renderScale);
                    cloudRenderDescriptor.width = (int)(cloudRenderDescriptor.width * skyDirector.cloudDefinition.renderScale);
                    cmd.GetTemporaryRT(cloudTarget.id, cloudRenderDescriptor, FilterMode.Point);
                }
                void ConfigureDepth(CommandBuffer cmd, RenderTextureDescriptor descriptor)
                {
                    RenderTextureDescriptor depthDescriptor = descriptor;
                    AssignDefaultDescriptorSettings(ref depthDescriptor, RenderTextureFormat.RFloat);
                    cmd.GetTemporaryRT(depthTex.id, depthDescriptor, FilterMode.Point);
                }
                void ConfigureUpscaling(CommandBuffer cmd, RenderTextureDescriptor descriptor)
                {
                    if (skyDirector.cloudDefinition.renderScaleSelection == RenderScaleSelection.Half || skyDirector.cloudDefinition.renderScaleSelection == RenderScaleSelection.Quarter)
                    {
                        cmd.GetTemporaryRT(upscaleHalfRes.id, descriptor, FilterMode.Point);

                    }

                    if (skyDirector.cloudDefinition.renderScaleSelection == RenderScaleSelection.Quarter)
                    {
                        RenderTextureDescriptor halfResDescriptor = descriptor;
                        halfResDescriptor.width = (int)(descriptor.width * 0.5f);
                        halfResDescriptor.height = (int)(descriptor.height * 0.5f);
                        cmd.GetTemporaryRT(upscaleQuarterRes.id, halfResDescriptor, FilterMode.Point);
                    }
                }
            }

           

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                Profiler.BeginSample(profilerTag);
                CommandBuffer cmd = CommandBufferPool.Get(profilerTag);
				CoreUtils.SetKeyword(cmd, decalBuffers[0], false);
				CoreUtils.SetKeyword(cmd, decalBuffers[1], false);
				CoreUtils.SetKeyword(cmd, decalBuffers[2], false);

				TimeManager.Update();
                RenderTargetIdentifier source = renderingData.cameraData.renderer.cameraColorTarget;


                Vector3 cameraPosition = renderingData.cameraData.worldSpaceCameraPos;                
                SetupGlobalShaderParams();


                RenderClouds(cmd, source);
                UpscaleClouds(cmd, out RenderTargetIdentifier taaInput);
                TemporalAntiAliasing(cmd, renderingData, taaInput);
                Merge(cmd, source);


                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();
                CommandBufferPool.Release(cmd);
                Profiler.EndSample();

                void SetupGlobalShaderParams()
				{
                    cmd.SetGlobalTexture(CloudShaderParamHandler.ShaderParams._Halton_23_Sequence, Helpers.GetHaltonSequence(skyDirector.data));
                    cmd.SetGlobalTexture(CloudShaderParamHandler.ShaderParams._BLUE_NOISE, Helpers.GetBlueNoise(skyDirector.data));
                    cmd.SetGlobalInt(CloudShaderParamHandler.ShaderParams._FrameId, TimeManager.FrameCount);
                    cmd.SetGlobalVector(CloudShaderParamHandler.ShaderParams._MainCameraOrigin, cameraPosition);
                    cmd.SetGlobalFloat(CloudShaderParamHandler.ShaderParams.Shadows._CloudShadowStrength, skyDirector.cloudDefinition.shadowStrength);
                }
                void RenderClouds(CommandBuffer cmd, RenderTargetIdentifier source)
                {
                    CloudShaderParamHandler.SetDepthCulling(skyDirector.cloudDefinition, cloudRenderMaterial);
                    CloudShaderParamHandler.SetDepthCulling(skyDirector.cloudDefinition, merge);
                    
                    cmd.SetGlobalFloat(CloudShaderParamHandler.ShaderParams._CLOUD_RENDER_SCALE, skyDirector.cloudDefinition.renderScale);
                    bool useDitheredDepth = skyDirector.cloudDefinition.depthCullOptions == DepthCullOptions.RenderLocal && skyDirector.cloudDefinition.renderScaleSelection != RenderScaleSelection.Full ? true : false;
                    cmd.SetGlobalInt(CloudShaderParamHandler.ShaderParams._USE_DITHERED_DEPTH, useDitheredDepth ? 1 : 0);
                    Blit(cmd, source, depthTex.Identifier(), ditherDepth);
                    cmd.SetGlobalTexture(CloudShaderParamHandler.ShaderParams._DitheredDepthTexture, depthTex.Identifier());
                    cmd.SetGlobalInt(CloudShaderParamHandler.ShaderParams._ShadowPass, 0);
                    cmd.SetGlobalVector(CloudShaderParamHandler.ShaderParams._RenderTextureDimensions, new Vector4(1f / cloudRenderDescriptor.width, 1f / cloudRenderDescriptor.height, cloudRenderDescriptor.width, cloudRenderDescriptor.height));
                    Blit(cmd, source, cloudTarget.Identifier(), cloudRenderMaterial);
                }
                void UpscaleClouds(CommandBuffer cmd, out RenderTargetIdentifier taaInput)
                {
                    RenderTargetIdentifier upscaleInput = cloudTarget.Identifier();
                    taaInput = upscaleInput;

                    if (skyDirector.cloudDefinition.renderScaleSelection != RenderScaleSelection.Full)
                    {
                        if (skyDirector.cloudDefinition.renderScaleSelection == RenderScaleSelection.Quarter)
                        {
                            cmd.SetGlobalFloat(CloudShaderParamHandler.ShaderParams._UPSCALE_SOURCE_RENDER_SCALE, 0.25f);
                            Blit(cmd, upscaleInput, upscaleQuarterRes.Identifier(), upscale);

                            cmd.SetGlobalFloat(CloudShaderParamHandler.ShaderParams._UPSCALE_SOURCE_RENDER_SCALE, 0.5f);
                            Blit(cmd, upscaleQuarterRes.Identifier(), upscaleHalfRes.Identifier(), upscale);
                            taaInput = upscaleHalfRes.Identifier();
                        }

                        if (skyDirector.cloudDefinition.renderScaleSelection == RenderScaleSelection.Half)
                        {
                            cmd.SetGlobalFloat(CloudShaderParamHandler.ShaderParams._UPSCALE_SOURCE_RENDER_SCALE, 0.5f);
                            Blit(cmd, upscaleInput, upscaleHalfRes.Identifier(), upscale);
                            taaInput = upscaleHalfRes.Identifier();
                        }
                    }
                }
                void TemporalAntiAliasing(CommandBuffer cmd, RenderingData renderingData, RenderTargetIdentifier taaInput)
                {
                    Camera camera = renderingData.cameraData.camera;
                    if (skyDirector.cloudDefinition.taaEnabled && skyDirector.cloudDefinition.taaBlendFactor < 1.0f)
                    {
                        TemporalAA.ProjectionMatrices matrices = taa.GetCurrentViewProjection(camera);
                        cmd.SetGlobalMatrix(CloudShaderParamHandler.ShaderParams._ViewProjM, matrices.viewProjection);
                        cmd.SetGlobalMatrix(CloudShaderParamHandler.ShaderParams._PrevViewProjM, taa.GetPreviousViewProjection(camera));

                        taa.SetPreviousViewProjection(camera, matrices.viewProjection);

                        bool isTemporalDataValid = taa.IsTemporalDataValid(camera, renderingData.cameraData.cameraTargetDescriptor);
                        if (!isTemporalDataValid)
                        {
                            taa.SetupTemporalData(camera, renderingData.cameraData.cameraTargetDescriptor);
                            CloudShaderParamHandler.IgnoreTAAThisFrame(cloudTaa);
                        }
                        else
                        {
                            CloudShaderParamHandler.ConfigureTAAParams(skyDirector.cloudDefinition, cloudTaa);
                            cmd.SetGlobalTexture(colorHistoryId, taa.TemporalData[camera].ColorTexture);
                            taa.TemporalData[camera].LastFrameUsed = TimeManager.FrameCount;
                        }

                        Blit(cmd, taaInput, temporalTarget.Identifier(), cloudTaa);
                        Blit(cmd, temporalTarget.Identifier(), taa.TemporalData[camera].ColorTexture);

                        cmd.SetGlobalTexture(mergePassInputTextureShaderReference, temporalTarget.Identifier());

                        taa.TemporalData[camera].LastFrameUsed = TimeManager.FrameCount;
                    }
                    else
                    {
                        cmd.SetGlobalTexture(mergePassInputTextureShaderReference, taaInput);
                    }
                }
                void Merge(CommandBuffer cmd, RenderTargetIdentifier source)
                {
                    Blit(cmd, source, mergeTarget.Identifier(), merge);
                    Blit(cmd, mergeTarget.Identifier(), source);
                }
            }

            public override void FrameCleanup(CommandBuffer cmd)
            {
                taa.Cleanup();
                cmd.ReleaseTemporaryRT(depthTex.id);
                cmd.ReleaseTemporaryRT(cloudTarget.id);
                cmd.ReleaseTemporaryRT(upscaleHalfRes.id);
                cmd.ReleaseTemporaryRT(upscaleQuarterRes.id);
                cmd.ReleaseTemporaryRT(temporalTarget.id);
                cmd.ReleaseTemporaryRT(mergeTarget.id);
            }
        }

        class CloudShadowsRenderPass : ScriptableRenderPass
        {

            private RenderTargetHandle cloudShadowsTempTarget;
            private RenderTargetHandle cloudShadowsTarget;
            private RenderTargetHandle screenShadowsTarget;
            private RenderTargetHandle shadowsToScreenTarget;

            private const string shadowTempId = "_CloudShadowTemporaryTexture";
            private const string shadowPermanentId = "_CloudShadowTexture";
            private const string screenShadowsId = "_CloudScreenShadows";
            private const string shadowsToScreenId = "_CloudShadowsOnScreen";

            //private Material renderMaterial;
            private Material shadowTaa;
            private Material screenShadows;
            private Material shadowsToScreen;


            private RenderTexture cloudShadowHistory = null;
            private const string profilerTag = "Altos: Cloud Shadows";


            public CloudShadowsRenderPass()
            {
                cloudShadowsTempTarget.Init(shadowTempId);
                cloudShadowsTarget.Init(shadowPermanentId);
                screenShadowsTarget.Init(screenShadowsId);
                shadowsToScreenTarget.Init(shadowsToScreenId);
            }

            public void Dispose()
			{
                CoreUtils.Destroy(shadowTaa);
                CoreUtils.Destroy(screenShadows);
                CoreUtils.Destroy(shadowsToScreen);

                shadowTaa = null;
                screenShadows = null;
                shadowsToScreen = null;

                if(cloudShadowHistory != null)
				{
                    cloudShadowHistory.Release();
                    cloudShadowHistory = null;
                }
			}

            public void Setup()
            {
                if (shadowTaa == null) shadowTaa = CoreUtils.CreateEngineMaterial(skyDirector.data.shaders.integrateCloudShadows);
                if (screenShadows == null) screenShadows = CoreUtils.CreateEngineMaterial(skyDirector.data.shaders.screenShadows);
                if (shadowsToScreen == null) shadowsToScreen = CoreUtils.CreateEngineMaterial(skyDirector.data.shaders.renderShadowsToScreen);
            }


            public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
			{
                RenderTextureDescriptor rtDescriptor = cameraTextureDescriptor;
                AssignDefaultDescriptorSettings(ref rtDescriptor);

                ConfigureShadows(cmd, rtDescriptor);
                ConfigureScreenShadows(cmd, rtDescriptor);
                cmd.GetTemporaryRT(shadowsToScreenTarget.id, rtDescriptor, FilterMode.Point);

                void ConfigureShadows(CommandBuffer cmd, RenderTextureDescriptor descriptor)
                {
                    if (skyDirector.cloudDefinition.castShadowsEnabled)
                    {
                        descriptor.width = (int)skyDirector.cloudDefinition.shadowResolution;
                        descriptor.height = (int)skyDirector.cloudDefinition.shadowResolution;
                        descriptor.colorFormat = RenderTextureFormat.DefaultHDR;
                        cmd.GetTemporaryRT(cloudShadowsTempTarget.id, descriptor);
                        cmd.GetTemporaryRT(cloudShadowsTarget.id, descriptor);

                        if (cloudShadowHistory == null)
                        {
                            cloudShadowHistory = new RenderTexture(descriptor);
                            cloudShadowHistory.name = "Cloud Shadow History";
                            cloudShadowHistory.filterMode = FilterMode.Point;
                            cloudShadowHistory.wrapMode = TextureWrapMode.Clamp;
                            cloudShadowHistory.Create();
                            Helpers.ClearRenderTexture(ref cloudShadowHistory);
                        }
                    }
                    else
                    {
                        if (cloudShadowHistory != null)
                        {
                            cloudShadowHistory.Release();
                            cloudShadowHistory = null;
                        }
                    }
                }

                void ConfigureScreenShadows(CommandBuffer cmd, RenderTextureDescriptor descriptor)
                {
                    cmd.GetTemporaryRT(screenShadowsTarget.id, descriptor, FilterMode.Point);
                }
            }


            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
			{
                Profiler.BeginSample(profilerTag);
                CommandBuffer cmd = CommandBufferPool.Get(profilerTag);

				CoreUtils.SetKeyword(cmd, decalBuffers[0], false);
				CoreUtils.SetKeyword(cmd, decalBuffers[1], false);
				CoreUtils.SetKeyword(cmd, decalBuffers[2], false);
				RenderTargetIdentifier source = renderingData.cameraData.renderer.cameraColorTarget;

                RenderShadows(cmd, renderingData, source);
                DrawScreenSpaceShadows(cmd, source);
				if (skyDirector.cloudDefinition.screenShadows)
				{
                    RenderToScreen(cmd, source);
                }

                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();
                CommandBufferPool.Release(cmd);
                Profiler.EndSample();

                void RenderShadows(CommandBuffer cmd, RenderingData renderingData, RenderTargetIdentifier source)
                {
                    const int zFar = 30000;
                    int halfWidth = (int)Mathf.Min(skyDirector.cloudDefinition.shadowDistance, Mathf.Min(renderingData.cameraData.camera.farClipPlane, skyDirector.cloudDefinition.cloudFadeDistance * 1000));
                    
                    Vector3 shadowCasterCameraPosition = renderingData.cameraData.worldSpaceCameraPos - SkyObject.Sun.GetForward() * zFar;

                    Matrix4x4 viewMatrix = MatrixHandler.SetupViewMatrix(shadowCasterCameraPosition, SkyObject.Sun.GetChild().forward, SkyObject.Sun.GetChild().up);
                    Matrix4x4 projectionMatrix = MatrixHandler.SetupProjectionMatrix(halfWidth, zFar);


                    cmd.SetGlobalVector(CloudShaderParamHandler.ShaderParams.Shadows._CloudShadowOrthoParams, new Vector4(halfWidth * 2, halfWidth * 2, zFar, 0));
                    cmd.SetGlobalVector(CloudShaderParamHandler.ShaderParams.Shadows._ShadowCasterCameraPosition, shadowCasterCameraPosition);
                    Matrix4x4 worldToShadow = MatrixHandler.ConvertToWorldToShadowMatrix(projectionMatrix * viewMatrix);
                    cmd.SetGlobalMatrix(CloudShaderParamHandler.ShaderParams.Shadows._CloudShadow_WorldToShadowMatrix, worldToShadow);

                    cmd.SetGlobalVector(CloudShaderParamHandler.ShaderParams.Shadows._ShadowCasterCameraForward, SkyObject.Sun.GetChild().transform.forward);
                    cmd.SetGlobalVector(CloudShaderParamHandler.ShaderParams.Shadows._ShadowCasterCameraUp, SkyObject.Sun.GetChild().transform.up);
                    cmd.SetGlobalVector(CloudShaderParamHandler.ShaderParams.Shadows._ShadowCasterCameraRight, SkyObject.Sun.GetChild().transform.right);
                    cmd.SetGlobalInt(CloudShaderParamHandler.ShaderParams._ShadowPass, 1);
                    cmd.SetGlobalVector(CloudShaderParamHandler.ShaderParams._RenderTextureDimensions, new Vector4(1f / (int)skyDirector.cloudDefinition.shadowResolution, 1f / (int)skyDirector.cloudDefinition.shadowResolution, (int)skyDirector.cloudDefinition.shadowResolution, (int)skyDirector.cloudDefinition.shadowResolution));

                    Blit(cmd, source, cloudShadowsTempTarget.Identifier(), cloudRenderMaterial);
                    cmd.SetGlobalTexture(CloudShaderParamHandler.ShaderParams.Shadows._CLOUD_SHADOW_PREVIOUS_HISTORY, cloudShadowHistory);
                    cmd.SetGlobalTexture(CloudShaderParamHandler.ShaderParams.Shadows._CLOUD_SHADOW_CURRENT_FRAME, cloudShadowsTempTarget.Identifier());
                    shadowTaa.SetFloat(CloudShaderParamHandler.ShaderParams.Shadows._IntegrationRate, 0.03f);
                    Blit(cmd, cloudShadowsTempTarget.Identifier(), cloudShadowsTarget.Identifier(), shadowTaa);
                    Blit(cmd, cloudShadowsTarget.Identifier(), cloudShadowHistory);
                    cmd.SetGlobalTexture(CloudShaderParamHandler.ShaderParams.Shadows._CloudShadowHistoryTexture, cloudShadowHistory);
                }

                void DrawScreenSpaceShadows(CommandBuffer cmd, RenderTargetIdentifier source)
                {
                    cmd.SetGlobalInt(CloudShaderParamHandler.ShaderParams._CastScreenCloudShadows, skyDirector.cloudDefinition.screenShadows ? 1 : 0);
                    Blit(cmd, source, screenShadowsTarget.Identifier(), screenShadows);
                }
                void RenderToScreen(CommandBuffer cmd, RenderTargetIdentifier source)
				{
                    Blit(cmd, source, shadowsToScreenTarget.Identifier(), shadowsToScreen);
                    Blit(cmd, shadowsToScreenTarget.Identifier(), source);
                }
            }

            public override void FrameCleanup(CommandBuffer cmd)
			{
                cmd.ReleaseTemporaryRT(cloudShadowsTarget.id);
                cmd.ReleaseTemporaryRT(cloudShadowsTempTarget.id);
                cmd.ReleaseTemporaryRT(shadowsToScreenTarget.id);
            }
        }

        private sealed class AtmosphereBlendingPass : ScriptableRenderPass
        {
            private Material blendingMaterial;
            private RenderTargetHandle blendingTarget;

            private const string blendingTargetId = "_AltosFogTarget";
            private const string profilerTag = "Altos: Atmosphere Blending";

            public AtmosphereBlendingPass()
            {
                blendingTarget.Init(blendingTargetId);
            }

            public void Dispose()
			{
                CoreUtils.Destroy(blendingMaterial);
                blendingMaterial = null;
			}

            public void Setup()
			{
                if (blendingMaterial == null) blendingMaterial = CoreUtils.CreateEngineMaterial(skyDirector.data.shaders.atmosphereBlending);
            }

            public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
            {
                RenderTextureDescriptor rtDescriptor = cameraTextureDescriptor;
                cmd.GetTemporaryRT(blendingTarget.id, rtDescriptor);
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                Profiler.BeginSample(profilerTag);

                RenderTargetIdentifier source = renderingData.cameraData.renderer.cameraColorTarget;
                CommandBuffer cmd = CommandBufferPool.Get(blendingTargetId);
				
                CoreUtils.SetKeyword(cmd, decalBuffers[0], false);
				CoreUtils.SetKeyword(cmd, decalBuffers[1], false);
				CoreUtils.SetKeyword(cmd, decalBuffers[2], false);

				blendingMaterial.SetFloat("_Density", skyDirector.atmosphereDefinition.GetDensity());
                Blit(cmd, source, blendingTarget.Identifier(), blendingMaterial);
                Blit(cmd, blendingTarget.Identifier(), source);
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();
                CommandBufferPool.Release(cmd);

                Profiler.EndSample();
            }

            public override void FrameCleanup(CommandBuffer cmd)
            {
                cmd.ReleaseTemporaryRT(blendingTarget.id);
            }
        }

		private static readonly string[] decalBuffers = { "_DBUFFER_MRT1", "_DBUFFER_MRT2", "_DBUFFER_MRT3" };
		private SkyRenderPass skyRenderPass;
        private AtmosphereBlendingPass atmospherePass;
        private VolumetricCloudsRenderPass cloudRenderPass;
        private CloudShadowsRenderPass shadowRenderPass;
        
        internal static Material cloudRenderMaterial = null;

        protected override void Dispose(bool disposing)
		{
            skyRenderPass?.Dispose();
            atmospherePass?.Dispose();
            cloudRenderPass?.Dispose();
            shadowRenderPass?.Dispose();

            skyRenderPass = null;
            atmospherePass = null;
            cloudRenderPass = null;
            shadowRenderPass = null;
            CoreUtils.Destroy(cloudRenderMaterial);
        }

        private void OnEnable()
        {
            Helpers.RenderFeatureOnEnable(Recreate);
        }
        
        private void OnDisable()
        {
            Helpers.RenderFeatureOnDisable(Recreate);
        }

        private void Recreate(UnityEngine.SceneManagement.Scene current, UnityEngine.SceneManagement.Scene next)
        {
            Create();
        }

        public override void Create()
        {
            skyRenderPass = new SkyRenderPass();
            skyRenderPass.renderPassEvent = RenderPassEvent.BeforeRenderingGbuffer;

            shadowRenderPass = new CloudShadowsRenderPass();
            shadowRenderPass.renderPassEvent = RenderPassEvent.AfterRenderingOpaques;

            atmospherePass = new AtmosphereBlendingPass();
            atmospherePass.renderPassEvent = RenderPassEvent.AfterRenderingOpaques + 1;

            cloudRenderPass = new VolumetricCloudsRenderPass();
            cloudRenderPass.renderPassEvent = RenderPassEvent.BeforeRenderingTransparents;
        }
        

        internal static AltosSkyDirector skyDirector;

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.camera.TryGetComponent(out DisableAltosRendering _))
            {
                return;
            }


            skyDirector = FindObjectOfType<AltosSkyDirector>();
            

            if (skyDirector == null)
                return;


            if (PassValidator.IsValidSkyPass(renderingData.cameraData.camera))
            {
                skyRenderPass.Setup();
                renderer.EnqueuePass(skyRenderPass);
            }


            if (PassValidator.IsValidCloudPass(renderingData.cameraData.camera))
            {
                if (cloudRenderMaterial == null) cloudRenderMaterial = CoreUtils.CreateEngineMaterial(skyDirector.data.shaders.renderClouds);
                SetDefaultCloudSettings();
                shadowRenderPass.Setup();
                renderer.EnqueuePass(shadowRenderPass);

                if (skyDirector.cloudDefinition.castShadowsEnabled)
				{
                    cloudRenderPass.Setup();
                    renderer.EnqueuePass(cloudRenderPass);
                }
            }
            

			if (PassValidator.IsValidAtmospherePass(renderingData.cameraData.camera))
			{
                atmospherePass.Setup();
                renderer.EnqueuePass(atmospherePass);
			}
        }

        private void SetDefaultCloudSettings()
        {
            Shader.SetGlobalInt(ShaderParams._HasSkyTexture, 0);
            Shader.SetGlobalColor(ShaderParams._HorizonColor, RenderSettings.ambientEquatorColor);
            Shader.SetGlobalColor(ShaderParams._ZenithColor, RenderSettings.ambientSkyColor);
        }

        private static class ShaderParams
        {
            public static int _HasSkyTexture = Shader.PropertyToID("_HasSkyTexture");
            public static int _HorizonColor = Shader.PropertyToID("_HorizonColor");
            public static int _ZenithColor = Shader.PropertyToID("_ZenithColor");
        }

        private static class PassValidator
		{
            private const string previewCameraName = "Preview Camera";
            private const string previewSceneCameraName = "Preview Scene Camera";

            public static bool IsValidCloudPass(Camera camera)
            {
                if (skyDirector.cloudDefinition == null)
                    return false;

                if (IsPreviewCamera(camera))
                    return false;

                if (IsSceneCamera(camera) && !skyDirector.cloudDefinition.renderInSceneView)
                    return false;

                if (IsReflectionCamera(camera))
                    return false;

                return true;
            }

            public static bool IsValidSkyPass(Camera c)
            {
                #if UNITY_EDITOR
                if (IsSceneCamera(c))
                {
                    if (!IsSkyboxEnabled || !IsDrawingTextured)
                        return false;
                }
                #endif

                if (skyDirector.skyDefinition == null)
                    return false;
                return true;
            }

            public static bool IsValidAtmospherePass(Camera c)
			{
                #if UNITY_EDITOR
                if (IsSceneCamera(c))
                {
                    if (!IsFogEnabled || !IsDrawingTextured)
                        return false;
                }
                #endif

                if (IsReflectionCamera(c))
                    return false;

                if (skyDirector.skyDefinition == null)
                    return false;

                if (skyDirector.atmosphereDefinition == null)
                    return false;

                return true;
            }

			#region Camera Checks
			private static bool IsPreviewCamera(Camera c)
            {
                return (c.name == previewCameraName || c.name == previewSceneCameraName);
            }

            private static bool IsSceneCamera(Camera c)
            {
                return c.cameraType == CameraType.SceneView;
            }

            private static bool IsReflectionCamera(Camera c)
            {
                return c.cameraType == CameraType.Reflection;
			}
			#endregion

			#region Editor Checks
            #if UNITY_EDITOR
			private static bool IsSkyboxEnabled { get => UnityEditor.SceneView.currentDrawingSceneView.sceneViewState.skyboxEnabled; }
            private static bool IsFogEnabled { get => UnityEditor.SceneView.currentDrawingSceneView.sceneViewState.fogEnabled; }
            private static bool IsDrawingTextured { get => UnityEditor.SceneView.currentDrawingSceneView.cameraMode.drawMode == UnityEditor.DrawCameraMode.Textured; }
            #endif
            #endregion
        }

        private static class MatrixHandler
        {
            public static Matrix4x4 SetupViewMatrix(Vector3 position, Vector3 forward, Vector3 up)
            {
                Matrix4x4 lookMatrix = Matrix4x4.LookAt(position, position + forward, up);
                Matrix4x4 scaleMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(1, 1, -1));
                Matrix4x4 viewMatrix = scaleMatrix * lookMatrix.inverse;
                return viewMatrix;
            }

            public static Matrix4x4 SetupProjectionMatrix(int halfWidth, int distance)
            {
                float s = halfWidth;
                Matrix4x4 proj = Matrix4x4.Ortho(-s, s, -s, s, 0, distance);
                return GL.GetGPUProjectionMatrix(proj, true);
            }

            public static Matrix4x4 ConvertToWorldToShadowMatrix(Matrix4x4 input)
            {
                if (SystemInfo.usesReversedZBuffer)
                {
                    input.m20 = -input.m20;
                    input.m21 = -input.m21;
                    input.m22 = -input.m22;
                    input.m23 = -input.m23;
                }

                input.m00 = 0.5f * (input.m00 + input.m30);
                input.m01 = 0.5f * (input.m01 + input.m31);
                input.m02 = 0.5f * (input.m02 + input.m32);
                input.m03 = 0.5f * (input.m03 + input.m33);
                input.m10 = 0.5f * (input.m10 + input.m30);
                input.m11 = 0.5f * (input.m11 + input.m31);
                input.m12 = 0.5f * (input.m12 + input.m32);
                input.m13 = 0.5f * (input.m13 + input.m33);
                input.m20 = 0.5f * (input.m20 + input.m30);
                input.m21 = 0.5f * (input.m21 + input.m31);
                input.m22 = 0.5f * (input.m22 + input.m32);
                input.m23 = 0.5f * (input.m23 + input.m33);

                return input;
            }
        }

        private class CloudShaderParamHandler
        {
            public static class ShaderParams
            {
                public static int _CLOUD_RENDER_SCALE = Shader.PropertyToID("_CLOUD_RENDER_SCALE");
                public static int depthCullReference = Shader.PropertyToID("_CLOUD_DEPTH_CULL_ON");
                public static int taaBlendFactorReference = Shader.PropertyToID("_TAA_BLEND_FACTOR");
                public static int _ViewProjM = Shader.PropertyToID("_ViewProjM");
                public static int _PrevViewProjM = Shader.PropertyToID("_PrevViewProjM");
                public static int _UPSCALE_SOURCE_RENDER_SCALE = Shader.PropertyToID("_UPSCALE_SOURCE_RENDER_SCALE");

                public static int _CastScreenCloudShadows = Shader.PropertyToID("_CastScreenCloudShadows");
                
                public static int _Halton_23_Sequence = Shader.PropertyToID("_Halton_23_Sequence");
                public static int _BLUE_NOISE = Shader.PropertyToID("_BLUE_NOISE");
                public static int _FrameId = Shader.PropertyToID("_FrameId");
                public static int _MainCameraOrigin = Shader.PropertyToID("_MainCameraOrigin");
                public static int _USE_DITHERED_DEPTH = Shader.PropertyToID("_USE_DITHERED_DEPTH");
                public static int _DitheredDepthTexture = Shader.PropertyToID("_DitheredDepthTexture");
                public static int _ShadowPass = Shader.PropertyToID("_ShadowPass");
                public static int _RenderTextureDimensions = Shader.PropertyToID("_RenderTextureDimensions");

                public static class Shadows
				{
                    public static int _CloudShadowOrthoParams = Shader.PropertyToID("_CloudShadowOrthoParams");
                    public static int _ShadowCasterCameraPosition = Shader.PropertyToID("_ShadowCasterCameraPosition");
                    public static int _CloudShadow_WorldToShadowMatrix = Shader.PropertyToID("_CloudShadow_WorldToShadowMatrix");
                    public static int _ShadowCasterCameraForward = Shader.PropertyToID("_ShadowCasterCameraForward");
                    public static int _ShadowCasterCameraUp = Shader.PropertyToID("_ShadowCasterCameraUp");
                    public static int _ShadowCasterCameraRight = Shader.PropertyToID("_ShadowCasterCameraRight");
                    public static int _CLOUD_SHADOW_PREVIOUS_HISTORY = Shader.PropertyToID("_CLOUD_SHADOW_PREVIOUS_HISTORY");
                    public static int _CLOUD_SHADOW_CURRENT_FRAME = Shader.PropertyToID("_CLOUD_SHADOW_CURRENT_FRAME");
                    public static int _IntegrationRate = Shader.PropertyToID("_IntegrationRate");
                    public static int _CloudShadowHistoryTexture = Shader.PropertyToID("_CloudShadowHistoryTexture");

                    public static int _CloudShadowStrength = Shader.PropertyToID("_CloudShadowStrength");
                }

                public static class CloudData
                {
                    public static int _CLOUD_STEP_COUNT = Shader.PropertyToID("_CLOUD_STEP_COUNT");
                    public static int _CLOUD_BLUE_NOISE_STRENGTH = Shader.PropertyToID("_CLOUD_BLUE_NOISE_STRENGTH");
                    public static int _CLOUD_BASE_TEX = Shader.PropertyToID("_CLOUD_BASE_TEX");
                    public static int _CLOUD_DETAIL1_TEX = Shader.PropertyToID("_CLOUD_DETAIL1_TEX");
                    public static int _CLOUD_EXTINCTION_COEFFICIENT = Shader.PropertyToID("_CLOUD_EXTINCTION_COEFFICIENT");
                    public static int _CLOUD_COVERAGE = Shader.PropertyToID("_CLOUD_COVERAGE");
                    public static int _CLOUD_SUN_COLOR_MASK = Shader.PropertyToID("_CLOUD_SUN_COLOR_MASK");
                    public static int _CLOUD_LAYER_HEIGHT = Shader.PropertyToID("_CLOUD_LAYER_HEIGHT");
                    public static int _CLOUD_LAYER_THICKNESS = Shader.PropertyToID("_CLOUD_LAYER_THICKNESS");
                    public static int _CLOUD_FADE_DIST = Shader.PropertyToID("_CLOUD_FADE_DIST");
                    public static int _CLOUD_BASE_SCALE = Shader.PropertyToID("_CLOUD_BASE_SCALE");
                    public static int _CLOUD_DETAIL1_SCALE = Shader.PropertyToID("_CLOUD_DETAIL1_SCALE");
                    public static int _CLOUD_DETAIL1_STRENGTH = Shader.PropertyToID("_CLOUD_DETAIL1_STRENGTH");
                    public static int _CLOUD_BASE_TIMESCALE = Shader.PropertyToID("_CLOUD_BASE_TIMESCALE");
                    public static int _CLOUD_DETAIL1_TIMESCALE = Shader.PropertyToID("_CLOUD_DETAIL1_TIMESCALE");
                    public static int _CLOUD_FOG_POWER = Shader.PropertyToID("_CLOUD_FOG_POWER");
                    public static int _CLOUD_MAX_LIGHTING_DIST = Shader.PropertyToID("_CLOUD_MAX_LIGHTING_DIST");
                    public static int _CLOUD_PLANET_RADIUS = Shader.PropertyToID("_CLOUD_PLANET_RADIUS");

                    public static int _CLOUD_CURL_TEX = Shader.PropertyToID("_CLOUD_CURL_TEX");
                    public static int _CLOUD_CURL_SCALE = Shader.PropertyToID("_CLOUD_CURL_SCALE");
                    public static int _CLOUD_CURL_STRENGTH = Shader.PropertyToID("_CLOUD_CURL_STRENGTH");
                    public static int _CLOUD_CURL_TIMESCALE = Shader.PropertyToID("_CLOUD_CURL_TIMESCALE");
                    public static int _CLOUD_CURL_ADJUSTMENT_BASE = Shader.PropertyToID("_CLOUD_CURL_ADJUSTMENT_BASE");

                    public static int _CLOUD_DETAIL2_TEX = Shader.PropertyToID("_CLOUD_DETAIL2_TEX");
                    public static int _CLOUD_DETAIL2_SCALE = Shader.PropertyToID("_CLOUD_DETAIL2_SCALE");
                    public static int _CLOUD_DETAIL2_TIMESCALE = Shader.PropertyToID("_CLOUD_DETAIL2_TIMESCALE");
                    public static int _CLOUD_DETAIL2_STRENGTH = Shader.PropertyToID("_CLOUD_DETAIL2_STRENGTH");

                    public static int _CLOUD_HGFORWARD = Shader.PropertyToID("_CLOUD_HGFORWARD");
                    public static int _CLOUD_HGBACK = Shader.PropertyToID("_CLOUD_HGBACK");
                    public static int _CLOUD_HGBLEND = Shader.PropertyToID("_CLOUD_HGBLEND");
                    public static int _CLOUD_HGSTRENGTH = Shader.PropertyToID("_CLOUD_HGSTRENGTH");

                    public static int _CLOUD_AMBIENT_EXPOSURE = Shader.PropertyToID("_CLOUD_AMBIENT_EXPOSURE");

                    public static int _CheapAmbientLighting = Shader.PropertyToID("_CheapAmbientLighting");

                    public static int _CLOUD_DISTANT_COVERAGE_START_DEPTH = Shader.PropertyToID("_CLOUD_DISTANT_COVERAGE_START_DEPTH");
                    public static int _CLOUD_DISTANT_CLOUD_COVERAGE = Shader.PropertyToID("_CLOUD_DISTANT_CLOUD_COVERAGE");
                    public static int _CLOUD_DETAIL1_HEIGHT_REMAP = Shader.PropertyToID("_CLOUD_DETAIL1_HEIGHT_REMAP");

                    public static int _CLOUD_DETAIL1_INVERT = Shader.PropertyToID("_CLOUD_DETAIL1_INVERT");
                    public static int _CLOUD_DETAIL2_HEIGHT_REMAP = Shader.PropertyToID("_CLOUD_DETAIL2_HEIGHT_REMAP");
                    public static int _CLOUD_DETAIL2_INVERT = Shader.PropertyToID("_CLOUD_DETAIL2_INVERT");
                    public static int _CLOUD_HEIGHT_DENSITY_INFLUENCE = Shader.PropertyToID("_CLOUD_HEIGHT_DENSITY_INFLUENCE");
                    public static int _CLOUD_COVERAGE_DENSITY_INFLUENCE = Shader.PropertyToID("_CLOUD_COVERAGE_DENSITY_INFLUENCE");

                    public static int _CLOUD_HIGHALT_TEX_1 = Shader.PropertyToID("_CLOUD_HIGHALT_TEX_1");
                    public static int _CLOUD_HIGHALT_TEX_2 = Shader.PropertyToID("_CLOUD_HIGHALT_TEX_2");
                    public static int _CLOUD_HIGHALT_TEX_3 = Shader.PropertyToID("_CLOUD_HIGHALT_TEX_3");

                    public static int _CLOUD_HIGHALT_OFFSET1 = Shader.PropertyToID("_CLOUD_HIGHALT_OFFSET1");
                    public static int _CLOUD_HIGHALT_OFFSET2 = Shader.PropertyToID("_CLOUD_HIGHALT_OFFSET2");
                    public static int _CLOUD_HIGHALT_OFFSET3 = Shader.PropertyToID("_CLOUD_HIGHALT_OFFSET3");
                    public static int _CLOUD_HIGHALT_SCALE1 = Shader.PropertyToID("_CLOUD_HIGHALT_SCALE1");
                    public static int _CLOUD_HIGHALT_SCALE2 = Shader.PropertyToID("_CLOUD_HIGHALT_SCALE2");
                    public static int _CLOUD_HIGHALT_SCALE3 = Shader.PropertyToID("_CLOUD_HIGHALT_SCALE3");
                    public static int _CLOUD_HIGHALT_COVERAGE = Shader.PropertyToID("_CLOUD_HIGHALT_COVERAGE");
                    public static int _CLOUD_HIGHALT_INFLUENCE1 = Shader.PropertyToID("_CLOUD_HIGHALT_INFLUENCE1");
                    public static int _CLOUD_HIGHALT_INFLUENCE2 = Shader.PropertyToID("_CLOUD_HIGHALT_INFLUENCE2");
                    public static int _CLOUD_HIGHALT_INFLUENCE3 = Shader.PropertyToID("_CLOUD_HIGHALT_INFLUENCE3");
                    public static int _CLOUD_BASE_RGBAInfluence = Shader.PropertyToID("_CLOUD_BASE_RGBAInfluence");
                    public static int _CLOUD_DETAIL1_RGBAInfluence = Shader.PropertyToID("_CLOUD_DETAIL1_RGBAInfluence");
                    public static int _CLOUD_DETAIL2_RGBAInfluence = Shader.PropertyToID("_CLOUD_DETAIL2_RGBAInfluence");
                    public static int _CLOUD_HIGHALT_EXTINCTION = Shader.PropertyToID("_CLOUD_HIGHALT_EXTINCTION");

                    public static int _CLOUD_HIGHALT_SHAPE_POWER = Shader.PropertyToID("_CLOUD_HIGHALT_SHAPE_POWER");
                    public static int _CLOUD_SCATTERING_AMPGAIN = Shader.PropertyToID("_CLOUD_SCATTERING_AMPGAIN");
                    public static int _CLOUD_SCATTERING_DENSITYGAIN = Shader.PropertyToID("_CLOUD_SCATTERING_DENSITYGAIN");
                    public static int _CLOUD_SCATTERING_OCTAVES = Shader.PropertyToID("_CLOUD_SCATTERING_OCTAVES");

                    public static int _CLOUD_SUBPIXEL_JITTER_ON = Shader.PropertyToID("_CLOUD_SUBPIXEL_JITTER_ON");
                    public static int _CLOUD_WEATHERMAP_TEX = Shader.PropertyToID("_CLOUD_WEATHERMAP_TEX");
                    public static int _CLOUD_WEATHERMAP_VELOCITY = Shader.PropertyToID("_CLOUD_WEATHERMAP_VELOCITY");
                    public static int _CLOUD_WEATHERMAP_SCALE = Shader.PropertyToID("_CLOUD_WEATHERMAP_SCALE");
                    public static int _CLOUD_WEATHERMAP_VALUE_RANGE = Shader.PropertyToID("_CLOUD_WEATHERMAP_VALUE_RANGE");
                    public static int _USE_CLOUD_WEATHERMAP_TEX = Shader.PropertyToID("_USE_CLOUD_WEATHERMAP_TEX");

                    public static int _CLOUD_DENSITY_CURVE_TEX = Shader.PropertyToID("_CLOUD_DENSITY_CURVE_TEX");
                }
            }


            public static void SetDepthCulling(CloudDefinition cloudDefinition, Material material)
            {
                material.SetInt(ShaderParams.depthCullReference, (int)cloudDefinition.depthCullOptions);
            }

            public static void ConfigureTAAParams(CloudDefinition cloudDefinition, Material material)
            {
                material.SetFloat(ShaderParams.taaBlendFactorReference, cloudDefinition.taaBlendFactor);
            }

            public static void IgnoreTAAThisFrame(Material material)
            {
                material.SetFloat(ShaderParams.taaBlendFactorReference, 1);
            }

            public static void SetCloudMaterialSettings(CloudDefinition d, Material cloudRenderMaterial)
            {
                cloudRenderMaterial.SetFloat(ShaderParams._CLOUD_RENDER_SCALE, d.renderScale);

                cloudRenderMaterial.SetFloat(ShaderParams.CloudData._CLOUD_AMBIENT_EXPOSURE, d.ambientExposure);
                cloudRenderMaterial.SetInt(ShaderParams.CloudData._CheapAmbientLighting, d.cheapAmbientLighting ? 1 : 0);

                cloudRenderMaterial.SetVector(ShaderParams.CloudData._CLOUD_BASE_SCALE, d.baseTextureScale);
                cloudRenderMaterial.SetTexture(ShaderParams.CloudData._CLOUD_BASE_TEX, d.baseTexture);
                cloudRenderMaterial.SetVector(ShaderParams.CloudData._CLOUD_BASE_TIMESCALE, d.baseTextureTimescale);
                cloudRenderMaterial.SetFloat(ShaderParams.CloudData._CLOUD_BLUE_NOISE_STRENGTH, d.blueNoise);
                cloudRenderMaterial.SetFloat(ShaderParams.CloudData._CLOUD_COVERAGE, d.cloudiness);
                cloudRenderMaterial.SetFloat(ShaderParams.CloudData._CLOUD_COVERAGE_DENSITY_INFLUENCE, d.cloudinessDensityInfluence);

                cloudRenderMaterial.SetFloat(ShaderParams.CloudData._CLOUD_CURL_SCALE, d.curlTextureScale);
                cloudRenderMaterial.SetFloat(ShaderParams.CloudData._CLOUD_CURL_STRENGTH, d.curlTextureInfluence);
                cloudRenderMaterial.SetTexture(ShaderParams.CloudData._CLOUD_CURL_TEX, d.curlTexture);
                cloudRenderMaterial.SetFloat(ShaderParams.CloudData._CLOUD_CURL_TIMESCALE, d.curlTextureTimescale);

                cloudRenderMaterial.SetVector(ShaderParams.CloudData._CLOUD_DETAIL1_HEIGHT_REMAP, d.detail1TextureHeightRemap);
                cloudRenderMaterial.SetVector(ShaderParams.CloudData._CLOUD_DETAIL1_SCALE, d.detail1TextureScale);
                cloudRenderMaterial.SetFloat(ShaderParams.CloudData._CLOUD_DETAIL1_STRENGTH, d.detail1TextureInfluence);
                cloudRenderMaterial.SetTexture(ShaderParams.CloudData._CLOUD_DETAIL1_TEX, d.detail1Texture);
                cloudRenderMaterial.SetVector(ShaderParams.CloudData._CLOUD_DETAIL1_TIMESCALE, d.detail1TextureTimescale);

                cloudRenderMaterial.SetFloat(ShaderParams.CloudData._CLOUD_DISTANT_CLOUD_COVERAGE, d.distantCoverageAmount);
                cloudRenderMaterial.SetFloat(ShaderParams.CloudData._CLOUD_DISTANT_COVERAGE_START_DEPTH, d.distantCoverageDepth);
                cloudRenderMaterial.SetFloat(ShaderParams.CloudData._CLOUD_EXTINCTION_COEFFICIENT, d.extinctionCoefficient);
                cloudRenderMaterial.SetFloat(ShaderParams.CloudData._CLOUD_FADE_DIST, d.cloudFadeDistance);
                cloudRenderMaterial.SetFloat(ShaderParams.CloudData._CLOUD_FOG_POWER, d.GetAtmosphereAttenuationDensity());
                cloudRenderMaterial.SetFloat(ShaderParams.CloudData._CLOUD_HEIGHT_DENSITY_INFLUENCE, d.heightDensityInfluence);

                cloudRenderMaterial.SetFloat(ShaderParams.CloudData._CLOUD_HGFORWARD, d.HGEccentricityForward);
                cloudRenderMaterial.SetFloat(ShaderParams.CloudData._CLOUD_HGBACK, d.HGEccentricityBackward);
                cloudRenderMaterial.SetFloat(ShaderParams.CloudData._CLOUD_HGSTRENGTH, d.HGStrength);

                cloudRenderMaterial.SetFloat(ShaderParams.CloudData._CLOUD_HIGHALT_COVERAGE, d.highAltCloudiness);
                cloudRenderMaterial.SetFloat(ShaderParams.CloudData._CLOUD_HIGHALT_EXTINCTION, d.highAltExtinctionCoefficient);
                cloudRenderMaterial.SetVector(ShaderParams.CloudData._CLOUD_HIGHALT_OFFSET1, d.highAltTimescale1);
                cloudRenderMaterial.SetVector(ShaderParams.CloudData._CLOUD_HIGHALT_OFFSET2, d.highAltTimescale2);
                cloudRenderMaterial.SetVector(ShaderParams.CloudData._CLOUD_HIGHALT_OFFSET3, d.highAltTimescale3);
                cloudRenderMaterial.SetVector(ShaderParams.CloudData._CLOUD_HIGHALT_SCALE1, d.highAltScale1);
                cloudRenderMaterial.SetVector(ShaderParams.CloudData._CLOUD_HIGHALT_SCALE2, d.highAltScale2);
                cloudRenderMaterial.SetVector(ShaderParams.CloudData._CLOUD_HIGHALT_SCALE3, d.highAltScale3);
                cloudRenderMaterial.SetTexture(ShaderParams.CloudData._CLOUD_HIGHALT_TEX_1, d.highAltTex1);
                cloudRenderMaterial.SetTexture(ShaderParams.CloudData._CLOUD_HIGHALT_TEX_2, d.highAltTex2);
                cloudRenderMaterial.SetTexture(ShaderParams.CloudData._CLOUD_HIGHALT_TEX_3, d.highAltTex3);


                cloudRenderMaterial.SetFloat(ShaderParams.CloudData._CLOUD_LAYER_HEIGHT, d.cloudLayerHeight);
                cloudRenderMaterial.SetFloat(ShaderParams.CloudData._CLOUD_LAYER_THICKNESS, d.cloudLayerThickness);
                cloudRenderMaterial.SetInt(ShaderParams.CloudData._CLOUD_MAX_LIGHTING_DIST, d.maxLightingDistance);
                cloudRenderMaterial.SetInt(ShaderParams.CloudData._CLOUD_PLANET_RADIUS, d.planetRadius);
                cloudRenderMaterial.SetFloat(ShaderParams.CloudData._CLOUD_SCATTERING_AMPGAIN, d.multipleScatteringAmpGain);
                cloudRenderMaterial.SetFloat(ShaderParams.CloudData._CLOUD_SCATTERING_DENSITYGAIN, d.multipleScatteringDensityGain);
                cloudRenderMaterial.SetInt(ShaderParams.CloudData._CLOUD_SCATTERING_OCTAVES, d.multipleScatteringOctaves);
                cloudRenderMaterial.SetInt(ShaderParams.CloudData._CLOUD_STEP_COUNT, d.stepCount);
                cloudRenderMaterial.SetVector(ShaderParams.CloudData._CLOUD_SUN_COLOR_MASK, d.sunColor);


                cloudRenderMaterial.SetInt(ShaderParams.CloudData._CLOUD_SUBPIXEL_JITTER_ON, d.subpixelJitterEnabled == true ? 1 : 0);
                cloudRenderMaterial.SetTexture(ShaderParams.CloudData._CLOUD_WEATHERMAP_TEX, d.weathermapTexture);
                cloudRenderMaterial.SetVector(ShaderParams.CloudData._CLOUD_WEATHERMAP_VELOCITY, d.weathermapVelocity);
                cloudRenderMaterial.SetFloat(ShaderParams.CloudData._CLOUD_WEATHERMAP_SCALE, d.weathermapScale);
                cloudRenderMaterial.SetInt(ShaderParams.CloudData._USE_CLOUD_WEATHERMAP_TEX, d.weathermapType == WeathermapType.Texture ? 1 : 0);

                cloudRenderMaterial.SetTexture(ShaderParams.CloudData._CLOUD_DENSITY_CURVE_TEX, d.curve.GetTexture());
            }
        }
    }

}
