using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using System;
using UnityEngine.Rendering;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
#endif

namespace OccaSoftware.Altos.Runtime
{

    [Serializable]
    public class AltosData : ScriptableObject
    {
        public static string packagePath = "Assets/OccaSoftware/Altos/AssetResources";

        #if UNITY_EDITOR
        internal class CreateAltosDataAsset : EndNameEditAction
		{
            public override void Action (int instanceId, string pathName, string resourceFile)
			{
                var instance = CreateInstance<AltosData>();
                AssetDatabase.CreateAsset(instance, pathName);
                ResourceReloader.ReloadAllNullIn(instance, packagePath);
                Selection.activeObject = instance;
            }
		}

        [MenuItem("Assets/Create/Altos/AltosData", priority = CoreUtils.Sections.section5 + CoreUtils.Priorities.assetsCreateRenderingMenuPriority)]
        static void CreateAltosData()
        {
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, CreateInstance<CreateAltosDataAsset>(), "AltosDataAsset.asset", null, null);
        }
        #endif

        public ShaderResources shaders;
        public TextureResources textures;
        public MeshResources meshes;

        [Serializable, ReloadGroup]
        public sealed class ShaderResources
        {
            [Reload("ShaderLibrary/Shaders/Skybox/AtmosphereShader.shader")]
            public Shader atmosphereShader;

            [Reload("ShaderLibrary/Resources/Skybox/BackgroundShader.shader")]
            public Shader backgroundShader;

            [Reload("ShaderLibrary/Resources/Skybox/SkyObjectShader.shader")]
            public Shader skyObjectShader;

            [Reload("ShaderLibrary/Resources/Skybox/StarShader.shader")]
            public Shader starShader;


            [Reload("ShaderLibrary/Resources/Clouds/DitherDepth_OS.shadergraph")]
            public Shader ditherDepth;

            [Reload("ShaderLibrary/Resources/Clouds/MergeClouds_OS.shadergraph")]
            public Shader mergeClouds;

            [Reload("ShaderLibrary/Resources/Clouds/RenderClouds_OS.shadergraph")]
            public Shader renderClouds;

            [Reload("ShaderLibrary/Resources/Clouds/TemporalIntegration_OS.shadergraph")]
            public Shader temporalIntegration;

            [Reload("ShaderLibrary/Resources/Clouds/UpscaleClouds_OS.shadergraph")]
            public Shader upscaleClouds;


            [Reload("ShaderLibrary/Resources/Clouds/Shadows/IntegrateCloudShadows.shader")]
            public Shader integrateCloudShadows;

            [Reload("ShaderLibrary/Resources/Clouds/Shadows/RenderShadowsToScreen_OS.shadergraph")]
            public Shader renderShadowsToScreen;

            [Reload("ShaderLibrary/Resources/Clouds/Shadows/ScreenShadows.shader")]
            public Shader screenShadows;

            [Reload("ShaderLibrary/Resources/Atmosphere/AtmosphereBlending_OS.shadergraph")]
            public Shader atmosphereBlending;
        }

        [Serializable, ReloadGroup]
        public sealed class TextureResources
        {
            [Reload("Textures/Noise Textures/HaltonSequence/Halton_23_Sequence.png")]
            public Texture2D halton;

            [Reload("Textures/Noise Textures/BlueNoise/LDR_LLL1_{0}.png", 0, 64)]
            public Texture2D[] blue;
        }

        [Serializable, ReloadGroup]
        public sealed class MeshResources
		{
            [Reload("Meshes/Altos_Sphere.fbx")]
            public Mesh skyboxMesh;
        }
    }

    
}
