using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace OccaSoftware.Altos.Runtime
{
    [CreateAssetMenu(fileName = "Cloud Definition", menuName = "Altos/Cloud Definition")]
    public class CloudDefinition : ScriptableObject
    {
        private void OnValidate()
        {
            planetRadius = GetRadiusFromCelestialBodySelection(celestialBodySelection, planetRadius);
            extinctionCoefficient = Mathf.Max(0, extinctionCoefficient);
            maxLightingDistance = Mathf.Max(0, maxLightingDistance);

            curlTextureInfluence = Mathf.Max(0, curlTextureInfluence);
            curlTextureScale = Mathf.Max(0, curlTextureScale);

            detail1TextureInfluence = Mathf.Clamp(detail1TextureInfluence, 0f, 1f);
            detail1TextureScale = Vector3.Max(Vector3.zero, detail1TextureScale);

            detail1TextureHeightRemap = ClampVec2_01(detail1TextureHeightRemap);
            detail1TextureHeightRemap = ClampVec2_01(detail1TextureHeightRemap);

            baseTexture = LoadVolumeTexture(baseTextureID, baseTextureQuality);
            detail1Texture = LoadVolumeTexture(detail1TextureID, detail1TextureQuality);

            if (detail1Texture == null)
                detail1TextureInfluence = 0.0f;


            highAltExtinctionCoefficient = Mathf.Max(0, highAltExtinctionCoefficient);

            highAltScale1 = Vector2.Max(Vector2.zero, highAltScale1);
            highAltScale2 = Vector2.Max(Vector2.zero, highAltScale2);
            highAltScale3 = Vector2.Max(Vector2.zero, highAltScale3);

            if(depthCullOptions == DepthCullOptions.RenderLocal && renderScaleSelection == RenderScaleSelection.Quarter)
			{
                renderScaleSelection = RenderScaleSelection.Half;
                Debug.Log("The quarter size render scale option is unavailable when depth culling is set to render local.");
			}

            renderScale = GetRenderScalingFromRenderScaleSelection(renderScaleSelection);

            blueNoise = RoundTo2Decimals(blueNoise);
            renderScale = RoundTo2Decimals(renderScale);
            HGEccentricityBackward = RoundTo2Decimals(HGEccentricityBackward);
            HGEccentricityForward = RoundTo2Decimals(HGEccentricityForward);
            cloudiness = RoundTo2Decimals(cloudiness);
            cloudinessDensityInfluence = RoundTo2Decimals(cloudinessDensityInfluence);
            heightDensityInfluence = RoundTo2Decimals(heightDensityInfluence);

            detail1TextureInfluence = RoundTo2Decimals(detail1TextureInfluence);

            highAltCloudiness = RoundTo2Decimals(highAltCloudiness);

            ambientExposure = Mathf.Max(ambientExposure, 0);
            cloudLayerHeight = Mathf.Max(cloudLayerHeight, 0);
        }

        
        private Texture3D EmptyTexture3D
		{
			get
			{
                if(emptyTexture3D == null)
				{
                    emptyTexture3D = new Texture3D(1, 1, 1, TextureFormat.RGBA32, false);
                    emptyTexture3D.SetPixel(0, 0, 0, Color.white);
                    emptyTexture3D.Apply();
				}

                return emptyTexture3D;
			}
		}
        private Texture3D emptyTexture3D;


        /// <summary>
        /// Fetches the correct volume texture given the input texture type and quality settings.
        /// </summary>
        /// <param name="id">The type of texture</param>
        /// <param name="quality">The quality level for the texture</param>
        /// <returns>A Texture3D matching the input type and quality.</returns>
        private Texture3D LoadVolumeTexture(TextureIdentifier id, TextureQuality quality)
		{
#if UNITY_EDITOR
            string loadTarget = "Assets/OccaSoftware/Altos/AssetResources/Textures/Noise Textures/VolumeTextures/";
			switch (id)
			{
                case TextureIdentifier.None:
                    return EmptyTexture3D;
                case TextureIdentifier.Perlin:
                    loadTarget += "Perlin/Perlin";
                    break;
                case TextureIdentifier.PerlinWorley:
                    loadTarget += "PerlinWorley/PerlinWorley";
                    break;
                case TextureIdentifier.Worley:
                    loadTarget += "Worley/Worley";
                    break;
                case TextureIdentifier.Billow:
                    loadTarget += "Billow/Billow";
                    break;
                default:
                    return EmptyTexture3D;
			}

			switch (quality)
			{
                case TextureQuality.Low:
                    loadTarget += "16";
                    break;
                case TextureQuality.Medium:
                    loadTarget += "32";
                    break;
                case TextureQuality.High:
                    loadTarget += "64";
                    break;
                case TextureQuality.Ultra:
                    loadTarget += "128";
                    break;
                default:
                    return EmptyTexture3D;
			}
            loadTarget += ".asset";
            return UnityEditor.AssetDatabase.LoadAssetAtPath<Texture3D>(loadTarget);
#else
            return EmptyTexture3D;
#endif
        }
        
        /// <summary>
        /// Rounds the input value to a maximum of two decimal points.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private float RoundTo2Decimals(float input)
        {
            return (float)System.Math.Round((double)input, 2);
        }

        /// <summary>
        /// Clamps the input vector to the [0,1] range.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private Vector2 ClampVec2_01(Vector2 input)
        {
            return Vector2.Min(Vector2.one, Vector2.Max(Vector2.zero, input));
        }


        // To do: Move to editor script.
#region Editor States
        public PageSelection pageSelection = PageSelection.Basic;

        public bool lowAltitudeModelingState = false;
        public bool lowAltitudeLightingState = false;
        public bool lowAltitudeWeatherState = false;
        public bool lowAltitudeBaseState = false;
        public bool lowAltitudeDetail1State = false;
        public bool lowAltitudeDetail2State = false;
        public bool lowAltitudeCurlState = false;
#endregion

#region Volumetric Basic Setup
        /// <summary>
        /// The number of steps taken through the cloud volume to sample the cloud data. Higher values are more costly but yield better results.
        /// </summary>
        public int stepCount = 32;

        /// <summary>
        /// The intensity of the noise term applied to the clouds to deband the cloud sampling results.
        /// </summary>
        public float blueNoise = 1.0f;

        /// <summary>
        /// The color of the sun used by the clouds for rendering.
        /// </summary>
        public Color sunColor = Color.white;

        /// <summary>
        /// Overall intensity of the ambient exposure term. Higher values mean the ambient lighting is stronger.
        /// </summary>
        public float ambientExposure = 1.0f;

        /// <summary>
        /// Uses a more performant ambient lighting algorithm.
        /// </summary>
        public bool cheapAmbientLighting = true;

        /// <summary>
        /// The Henyey-Greenstein term used for forward scattering light. A higher value results in a sharper falloff.
        /// </summary>
        public float HGEccentricityForward = 0.6f;
        /// <summary>
        /// The Henyey-Greenstein term used for backscattering light. A higher value results in a sharper falloff.
        /// </summary>
        public float HGEccentricityBackward = -0.2f;
        /// <summary>
        /// The intensity of the Henyey-Greenstein lighting effect. This effect dictates the intensity of the forward and backscattering of light as it passes through the cloud volume.
        /// </summary>
        public float HGStrength = 1.0f;

        /// <summary>
        /// A reference planet (or moon) used to automatically set the radius. The planet radius is used to control the curvature of the cloud volume.
        /// </summary>
        public CelestialBodySelection celestialBodySelection;
        /// <summary>
        /// The planet radius is used to control the curvature of the cloud volume.
        /// </summary>
        public int planetRadius = 6378;
        /// <summary>
        /// The cloud layer height is the altitude at which the low altitude cloud volume begins.
        /// </summary>
        public float cloudLayerHeight = 0.6f;

        /// <summary>
        /// The cloud layer thickness dictates the altitude at which the low altitude cloud volume ends by the formula cloudLayerHeight + cloudLayerThickness = cloudLayerEndHeight. Increase this to create taller clouds.
        /// </summary>
        public float cloudLayerThickness = 0.6f;

        /// <summary>
        /// The cloud fade distance dictates the distance at which clouds will no longer be rendered.
        /// </summary>
        [Min(5)]
        public float cloudFadeDistance = 40f;

        /// <summary>
        /// The visibility term controls the fog falloff. Typically, you want to set this to a value greater than or equal to the cloud fade distance.
        /// </summary>
        [Min(1)]
        public float visibilityKM = 40f;

        public float GetAtmosphereAttenuationDensity()
		{
            return Helpers.GetDensityFromVisibilityDistance(visibilityKM * 1000f);

        }

        /// <summary>
        /// Controls the scale at which the clouds will be rendered.
        /// </summary>
        public RenderScaleSelection renderScaleSelection = RenderScaleSelection.Half;

        /// <summary>
        /// Controls the scale at which the clouds will be rendered.
        /// </summary>
        public float renderScale = 0.5f;

        /// <summary>
        /// When enabled, the clouds will be rendered in scene view.
        /// </summary>
        public bool renderInSceneView = true;

        /// <summary>
        /// When enabled, Temporal Anti-Aliasing will attempt to reduce noise and banding in the cloud volume.
        /// </summary>
        public bool taaEnabled = true;

        /// <summary>
        /// Configures the influence of recent frame data over the temporal blending.
        /// </summary>
        public float taaBlendFactor = 0.1f;

        /// <summary>
        /// Controls whether the clouds will be rendered using depth-testing or not. When rendered as skybox, the clouds will be behind all opaque objects. When rendered in front of opaque objects, the clouds will be depth tested.
        /// </summary>
        public DepthCullOptions depthCullOptions = DepthCullOptions.RenderAsSkybox;

        /// <summary>
        /// Enables sub-pixel jittering to increase the quality of cloud data when conducting temporal anti-aliasing.
        /// </summary>
        public bool subpixelJitterEnabled = true;

        /// <summary>
        /// When enabled, the clouds will write shadow data to the cloud shadow textures.
        /// </summary>
        public bool castShadowsEnabled = true;

        /// <summary>
        /// When enabled, the clouds will automatically write cloud shadow data to the screen during rendering. If disabled, you must sample the cloud shadow data yourself.
        /// </summary>
        public bool screenShadows = true;

        /// <summary>
        /// Controls the strength of cloud shadows.
        /// </summary>
        [Range(0,1)]
        public float shadowStrength = 1.0f;

        /// <summary>
        /// Controls the resolution (and resulting quality) of cloud shadows.
        /// </summary>
        public ShadowResolution shadowResolution = ShadowResolution.Medium;

        /// <summary>
        /// Controls the maximum distance at which cloud shadows will be rendered.
        /// </summary>
        public int shadowDistance = 10000;
#endregion

#region Low Altitude

#region Rendering
        /// <summary>
        /// Controls the density of the low altitude clouds.
        /// </summary>
        public float extinctionCoefficient = 70f;

        /// <summary>
        /// Sets the maximum distance at which cloud lighting will be sampled. Cloud lighting normally attempts to sample to the end of the cloud volume in the direction of the main light, but in some cases (like when the sun is close to the horizon), this effect can break down.
        /// </summary>
        public int maxLightingDistance = 2000;

        /// <summary>
        /// Controls the intensity of the multiple scattering effect's octaves.
        /// </summary>
        public float multipleScatteringAmpGain = 0.3f;

        /// <summary>
        /// Controls the density of the multiple scattering effect's octaves.
        /// </summary>
        public float multipleScatteringDensityGain = 0.1f;

        /// <summary>
        /// Controls the number of octaves to sample for the multiple scattering approximation. This improves the quality of the cloud lighting by simulating bounced lighting within the cloud volume.
        /// </summary>
        public int multipleScatteringOctaves = 3;
#endregion

#region Modeling
        /// <summary>
        /// Used to set the overall cloud coverage level.
        /// </summary>
        public float cloudiness = 0.5f;

        /// <summary>
        /// Used to set the start distance at which the distant coverage cloudiness term will take effect.
        /// </summary>
        [Min(5)]
        public float distantCoverageDepth = 20f;

        /// <summary>
        /// Controls the coverage starting from the distant coverage depth.
        /// </summary>
        public float distantCoverageAmount = 0.8f;

        /// <summary>
        /// Used to increase the cloud density by the cloud altitude.
        /// </summary>
        public float heightDensityInfluence = 1.0f;

        /// <summary>
        /// Used to increase the cloud density as the cloud coverage value increases.
        /// </summary>
        public float cloudinessDensityInfluence = 1.0f;

        /// <summary>
        /// This curve is used a density height mapping to describe the cloud density throughout the altitude of the cloud volume.
        /// </summary>
        [SerializeField] public TextureCurve curve = new TextureCurve(new AnimationCurve(new Keyframe[] {new Keyframe(0, 1), new Keyframe(1, 0)}), 1, false, new Vector2(0,1));
#endregion

#region Weather
        /// <summary>
        /// Describes whether the cloud volume will reference a procedural weather volume or an input texture volume.
        /// </summary>
        public WeathermapType weathermapType = WeathermapType.Procedural;

        /// <summary>
        /// When using the Weathermap Type of Texture, describes the weathermap that will be used.
        /// </summary>
        public Texture2D weathermapTexture = null;

        /// <summary>
        /// Used to set the speed at which the weathermap moves across the scene.
        /// </summary>
        public Vector2 weathermapVelocity = Vector2.zero;

        /// <summary>
        /// Used to set the overall scale of the weathermap. Higher values mean a more dense weathermap.
        /// </summary>
        public float weathermapScale = 8.0f;
#endregion

#region Base Volume Model
        /// <summary>
        /// The type of the volume texture used for the base cloud shape.
        /// </summary>
        public TextureIdentifier baseTextureID = TextureIdentifier.Perlin;

        /// <summary>
        /// The quality of the volume texture used for the base cloud shape.
        /// </summary>
        public TextureQuality baseTextureQuality = TextureQuality.Ultra;

        /// <summary>
        /// The actual texture3D used for the base cloud shape.
        /// </summary>
        public Texture3D baseTexture = null;

        /// <summary>
        /// The scale of the base cloud shape.
        /// </summary>
        public Vector3 baseTextureScale = new Vector3(10f, 10f, 10f);

        /// <summary>
        /// The rate at which the base cloud shape will displace over time.
        /// </summary>
        public Vector3 baseTextureTimescale = new Vector3(10f, -10f, 0f);
#endregion

#region Detail 1 Volume Model
        /// <summary>
        /// The type of texture used for the detail texture.
        /// </summary>
        public TextureIdentifier detail1TextureID = TextureIdentifier.Worley;

        /// <summary>
        /// The quality of the texture used for the detail texture.
        /// </summary>
        public TextureQuality detail1TextureQuality = TextureQuality.Low;

        /// <summary>
        /// The actual Texture3D used for the detail texture.
        /// </summary>
        public Texture3D detail1Texture = null;

        /// <summary>
        /// The strength of the detail texture as applied as an erosion to the base cloud shape.
        /// </summary>
        public float detail1TextureInfluence = 0.2f;

        /// <summary>
        /// The scale of the detail texture volume.
        /// </summary>
        public Vector3 detail1TextureScale = new Vector3(125f, 125f, 125f);

        /// <summary>
        /// The rate at which the detail texture will displace over time.
        /// </summary>
        public Vector3 detail1TextureTimescale = new Vector3(25f, -50f, 30f);

        /// <summary>
        /// How the strength of the detail texture maps over the height of the cloud volume. For example, [0, 0.3] will cause the detail texture to have 0 strength at the base of the cloud volume, increasing gradually to full strength 30% of the way through the cloud volume.
        /// </summary>
        public Vector2 detail1TextureHeightRemap = new Vector2(0.0f, 0.3f);
        
#endregion

#region Detail Curl 2D Model
        /// <summary>
        /// The displacement texture used to offset noise, simulating detail wind.
        /// </summary>
        public Texture2D curlTexture;
        /// <summary>
        /// The strength of this displacement texture.
        /// </summary>
        public float curlTextureInfluence;

        /// <summary>
        /// The scale of the displacement texture.
        /// </summary>
        public float curlTextureScale;

        /// <summary>
        /// The rate at which the displacement texture moves across the cloud volume.
        /// </summary>
        public float curlTextureTimescale;
#endregion

#endregion

#region High Altitude
        /// <summary>
        /// The density of high altitude clouds.
        /// </summary>
        public float highAltExtinctionCoefficient = 0.2f;
        /// <summary>
        /// The cloud coverage for high altitude clouds.
        /// </summary>
        public float highAltCloudiness = 0.5f;

        /// <summary>
        /// The weathermap used in conjunction with the cloudiness value to set the cloud coverage areas.
        /// </summary>
        public Texture2D highAltTex1 = null;
        /// <summary>
        /// The scale of the high altitude weathermap.
        /// </summary>
        public Vector2 highAltScale1 = new Vector2(5f, 5f);
        /// <summary>
        /// The rate at which the high altitude weathermap pans over the scene.
        /// </summary>
        public Vector2 highAltTimescale1 = new Vector2(5f, 5f);

        /// <summary>
        /// One of the two erosion textures used for the high altitude clouds.
        /// </summary>
        public Texture2D highAltTex2 = null;
        /// <summary>
        /// The scale of this high altitude texture.
        /// </summary>
        public Vector2 highAltScale2 = new Vector2(5f, 5f);
        /// <summary>
        /// The rate at which this high altitude texture pans over the scene.
        /// </summary>
        public Vector2 highAltTimescale2 = new Vector2(5f, 5f);

        /// <summary>
        /// One of the two erosion textures used for the high altitude clouds.
        /// </summary>
        public Texture2D highAltTex3 = null;
        /// <summary>
        /// The scale of this high altitude texture.
        /// </summary>
        public Vector2 highAltScale3 = new Vector2(5f, 5f);
        /// <summary>
        /// The rate at which this high altitude texture pans over the scene.
        /// </summary>
        public Vector2 highAltTimescale3 = new Vector2(5f, 5f);

#endregion

        /// <summary>
        /// A helper method in which we've encoded the radius of each celestial body in KM.
        /// </summary>
        /// <param name="celestialBodySelection"></param>
        /// <param name="currentVal"></param>
        /// <returns></returns>
        private int GetRadiusFromCelestialBodySelection(CelestialBodySelection celestialBodySelection, int currentVal)
        {
            switch (celestialBodySelection)
            {
                case CelestialBodySelection.Earth:
                    return 6378;
                case CelestialBodySelection.Mars:
                    return 3389;
                case CelestialBodySelection.Venus:
                    return 6052;
                case CelestialBodySelection.Luna:
                    return 1737;
                case CelestialBodySelection.Titan:
                    return 2575;
                case CelestialBodySelection.Enceladus:
                    return 252;
                default:
                    return Mathf.Max(0, currentVal);
            }
        }

        /// <summary>
        /// Returns the render scale as a float from the render scale selection.
        /// </summary>
        /// <param name="renderScaleSelection"></param>
        /// <returns></returns>
        private float GetRenderScalingFromRenderScaleSelection(RenderScaleSelection renderScaleSelection)
        {
            switch (renderScaleSelection)
            {
                case RenderScaleSelection.Full:
                    return 1.0f;
                case RenderScaleSelection.Half:
                    return 0.5f;
                case RenderScaleSelection.Quarter:
                    return 0.25f;
                default:
                    return 0.5f;
            }
        }
    }

    /// <summary>
    /// The type of texture for this volume texture.
    /// </summary>
    public enum TextureIdentifier
    {
        None,
        Perlin,
        PerlinWorley,
        Worley,
        Billow
    }

    /// <summary>
    /// The resolution of the shadow map.
    /// </summary>
    public enum ShadowResolution
	{
        Low = 256,
        Medium = 512,
        High = 1024,
	}

    /// <summary>
    /// The quality of the Volume Texture to fetch.
    /// </summary>
    public enum TextureQuality
    {
        Low,
        Medium,
        High,
        Ultra
    }

    /// <summary>
    /// The active page in the inspector window.
    /// </summary>
    public enum PageSelection
    {
        Basic,
        LowAltitude,
        HighAltitude
    }

    /// <summary>
    /// The planet to reference for cloud curvature.
    /// </summary>
    public enum CelestialBodySelection
    {
        Earth,
        Mars,
        Venus,
        Luna,
        Titan,
        Enceladus,
        Custom
    }


    /// <summary>
    /// The resolution at which the clouds should be rendered.
    /// </summary>
    public enum RenderScaleSelection
    {
        Full,
        Half,
        Quarter
    }
    
    /// <summary>
    /// Whether to depth-test the clouds during rendering.
    /// </summary>
    public enum DepthCullOptions
    {
        [InspectorName("Background Only Clouds")]
        RenderAsSkybox,
        [InspectorName("Clouds Render Over Opaque Objects")]
        RenderLocal
    }

    /// <summary>
    /// The type of weathermap to use for coverage sampling.
    /// </summary>
    public enum WeathermapType
    {
        Procedural,
        Texture
    }

}