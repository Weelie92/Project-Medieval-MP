using System;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using OccaSoftware.Altos.Runtime;

namespace OccaSoftware.Altos.Editor
{

    [CustomEditor(typeof(CloudDefinition))]
    [CanEditMultipleObjects]
    public class VolumetricCloudDefinitionEditor : UnityEditor.Editor
    {
        private class Params
        {
            public class Editor
            {
                public static SerializedProperty pageSelection;
                public static SerializedProperty lowAltitudeModelingState;
                public static SerializedProperty lowAltitudeLightingState;
                public static SerializedProperty lowAltitudeWeatherState;
                public static SerializedProperty lowAltitudeBaseState;
                public static SerializedProperty lowAltitudeDetail1State;
                public static SerializedProperty lowAltitudeCurlState;
			}

			public class Basic
			{
				public static SerializedProperty stepCount;
				public static SerializedProperty blueNoise;
				public static SerializedProperty sunColor;
				public static SerializedProperty ambientExposure;
                public static SerializedProperty cheapAmbientLighting;
                public static SerializedProperty HGEccentricityForward;
				public static SerializedProperty HGEccentricityBackward;
				public static SerializedProperty HGBlend;
				public static SerializedProperty HGStrength;

				public static SerializedProperty celestialBodySelection;
				public static SerializedProperty planetRadius;
				public static SerializedProperty cloudLayerHeight;
				public static SerializedProperty cloudLayerThickness;
				public static SerializedProperty cloudFadeDistance;
				public static SerializedProperty visibilityKM;

				public static SerializedProperty renderScaleSelection;
				public static SerializedProperty renderInSceneView;
				public static SerializedProperty taaEnabled;
				public static SerializedProperty taaBlendFactor;
				public static SerializedProperty depthCullOptions;
				public static SerializedProperty subpixelJitterEnabled;

                public static SerializedProperty castShadowsEnabled;
                public static SerializedProperty screenShadows;
                public static SerializedProperty shadowStrength;
                public static SerializedProperty shadowResolution;
                public static SerializedProperty shadowDistance;
            }

			public class LowAlt
            {
                public static SerializedProperty extinctionCoefficient;
                public static SerializedProperty cloudiness;
                public static SerializedProperty heightDensityInfluence;
                public static SerializedProperty cloudinessDensityInfluence;
                public static SerializedProperty cloudDensityCurve;
                public static SerializedProperty curve;
                public static SerializedProperty distantCoverageDepth;
                public static SerializedProperty distantCoverageAmount;


                public static SerializedProperty maxLightingDistance;
                public static SerializedProperty multipleScatteringAmpGain;
                public static SerializedProperty multipleScatteringDensityGain;
                public static SerializedProperty multipleScatteringOctaves;

                public class Weather
                {
                    public static SerializedProperty weathermapTexture;
                    public static SerializedProperty weathermapVelocity;
                    public static SerializedProperty weathermapScale;
                    public static SerializedProperty weathermapType;
                }

                public class Base
                {
                    public static SerializedProperty baseTextureID;
                    public static SerializedProperty baseTextureQuality;
                    public static SerializedProperty baseTextureScale;
                    public static SerializedProperty baseTextureTimescale;

                    public static SerializedProperty baseFalloffSelection;
                    public static SerializedProperty baseTextureRInfluence;
                    public static SerializedProperty baseTextureGInfluence;
                    public static SerializedProperty baseTextureBInfluence;
                    public static SerializedProperty baseTextureAInfluence;
                }

                public class Detail1
                {
                    public static SerializedProperty detail1TextureID;
                    public static SerializedProperty detail1TextureQuality;
                    public static SerializedProperty detail1TextureInfluence;
                    public static SerializedProperty detail1TextureScale;
                    public static SerializedProperty detail1TextureTimescale;

                    public static SerializedProperty detail1FalloffSelection;
                    public static SerializedProperty detail1TextureRInfluence;
                    public static SerializedProperty detail1TextureGInfluence;
                    public static SerializedProperty detail1TextureBInfluence;
                    public static SerializedProperty detail1TextureAInfluence;

                    public static SerializedProperty detail1TextureHeightRemap;
                }

                public class Curl
                {
                    public static SerializedProperty curlTexture;
                    public static SerializedProperty curlTextureInfluence;
                    public static SerializedProperty curlTextureScale;
                    public static SerializedProperty curlTextureTimescale;
                }
            }

            public class HighAlt
            {
                public static SerializedProperty highAltExtinctionCoefficient;
                public static SerializedProperty highAltCloudiness;


                public static SerializedProperty highAltTex1;
                public static SerializedProperty highAltScale1;
                public static SerializedProperty highAltTimescale1;


                public static SerializedProperty highAltTex2;
                public static SerializedProperty highAltScale2;
                public static SerializedProperty highAltTimescale2;


                public static SerializedProperty highAltTex3;
                public static SerializedProperty highAltScale3;
                public static SerializedProperty highAltTimescale3;
            }
        }

        private void SetupSerializedPropertyParams<T>()
        {
            foreach (FieldInfo field in typeof(T).GetFields())
            {
                field.SetValue(null, serializedObject.FindProperty(field.Name));
            }
        }

        public static SerializedProperty curve_T;

        private void OnEnable()
        {
            SetupSerializedPropertyParams<Params.Editor>();

            SetupSerializedPropertyParams<Params.Basic>();

            SetupSerializedPropertyParams<Params.LowAlt>();
            SetupSerializedPropertyParams<Params.LowAlt.Weather>();
            SetupSerializedPropertyParams<Params.LowAlt.Base>();
            SetupSerializedPropertyParams<Params.LowAlt.Detail1>();
            SetupSerializedPropertyParams<Params.LowAlt.Curl>();

            SetupSerializedPropertyParams<Params.HighAlt>();

            curve_T = serializedObject.FindProperty("curve.m_Curve");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            Draw();
            serializedObject.ApplyModifiedProperties();
        }

        private void Draw()
        {
            DrawCloudDefinitionHeader();

            switch (Params.Editor.pageSelection.enumValueIndex)
            {
                case 0: DrawVolumetricBasicSetup(); break;
                case 1: HandleLowAltitudeDrawing(); break;
                case 2: HandleHighAltitudeDrawing(); break;
                default: break;
            }
        }


        void DrawCloudDefinitionHeader()
        {
            if (GUILayout.Button("Basic Configuration"))
            {
                Params.Editor.pageSelection.enumValueIndex = 0;
            }

            if (GUILayout.Button("Low Altitude Configuration"))
            {
                Params.Editor.pageSelection.enumValueIndex = 1;
            }

            if (GUILayout.Button("High Altitude Configuration"))
            {
                Params.Editor.pageSelection.enumValueIndex = 2;
            }
        }

        #region Draw Basic Setup
        void DrawVolumetricBasicSetup()
        {
            #region Basic Setup
            EditorHelpers.HandleIndentedGroup("Rendering", DrawBasicRendering);
            EditorHelpers.HandleIndentedGroup("Lighting", DrawBasicLighting);
            EditorHelpers.HandleIndentedGroup("Atmosphere", DrawBasicAtmosphere);
            EditorHelpers.HandleIndentedGroup("Shadows", DrawBasicShadows);
            #endregion
        }

        void DrawBasicRendering()
        {
            EditorGUILayout.IntSlider(Params.Basic.stepCount, 1, 128);
            EditorGUILayout.Slider(Params.Basic.blueNoise, 0f, 1f, new GUIContent("Noise"));

            EditorGUILayout.PropertyField(Params.Basic.renderScaleSelection, new GUIContent("Render Scale"));
            EditorGUILayout.PropertyField(Params.Basic.renderInSceneView);
            EditorGUILayout.PropertyField(Params.Basic.taaEnabled);
            EditorGUILayout.Slider(Params.Basic.taaBlendFactor, 0f, 1f);
            EditorGUILayout.PropertyField(Params.Basic.subpixelJitterEnabled);
            EditorGUILayout.PropertyField(Params.Basic.depthCullOptions, new GUIContent("Rendering Mode"));
        }

        void DrawBasicLighting()
        {
            Params.Basic.sunColor.colorValue = EditorGUILayout.ColorField(new GUIContent("Sun Color Mask", "This value is multiplied by the color of your main directional light."), Params.Basic.sunColor.colorValue, false, false, true);
            EditorGUILayout.PropertyField(Params.Basic.ambientExposure);
            EditorGUILayout.PropertyField(Params.Basic.cheapAmbientLighting);
            EditorGUILayout.Slider(Params.Basic.HGStrength, 0f, 1f);
            EditorGUILayout.Slider(Params.Basic.HGEccentricityForward, 0f, 0.99f);
            EditorGUILayout.Slider(Params.Basic.HGEccentricityBackward, -0.99f, 0f);
        }

        void DrawBasicAtmosphere()
        {
            EditorGUILayout.PropertyField(Params.Basic.celestialBodySelection, new GUIContent("Planet Radius"));
            if (GetEnum<CelestialBodySelection>(Params.Basic.celestialBodySelection) == CelestialBodySelection.Custom)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(Params.Basic.planetRadius, new GUIContent("Planet Radius (km)"));
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.PropertyField(Params.Basic.cloudLayerHeight, new GUIContent("Cloud Layer Altitude (km)"));
            EditorGUILayout.Slider(Params.Basic.cloudLayerThickness, 0.1f, 4.0f, new GUIContent("Cloud Layer Thickness (km)"));
            EditorGUILayout.PropertyField(Params.Basic.cloudFadeDistance, new GUIContent("Cloud Fade Distance (km)"));
            EditorGUILayout.PropertyField(Params.Basic.visibilityKM, new GUIContent("Visibility (km)", "Sets the distance over which the clouds fade into the atmosphere."));
        }

        void DrawBasicShadows()
		{
            EditorGUILayout.PropertyField(Params.Basic.castShadowsEnabled, new GUIContent("Cast Cloud Shadows"));
			if (Params.Basic.castShadowsEnabled.boolValue)
			{
                EditorGUILayout.PropertyField(Params.Basic.screenShadows, new GUIContent("Screen Shadows Enabled", "When enabled, Altos applies cloud shadows for you as a post-process. This is easy, but it is not physically realistic: It will equally attenuate ambient, additional, and direct lighting. This only applies to depth-tested opaque geometry. When disabled, Altos allows you to write your own shadow sampler into your frag shader stage for objects in your scene for more realistic results. More complicated, but better results. See docs for details."));
                EditorGUILayout.PropertyField(Params.Basic.shadowResolution, new GUIContent("Shadow Resolution"));
                EditorGUILayout.PropertyField(Params.Basic.shadowStrength, new GUIContent("Shadow Strength"));
                EditorGUILayout.PropertyField(Params.Basic.shadowDistance, new GUIContent("Shadow Distance (m)", "Radius around the camera in which cloud shadows will be rendered. Clamped to the camera's far clip plane and the cloud fade distance."));
			}
		}
        #endregion

        #region Low Altitude
        void HandleLowAltitudeDrawing()
        {
            Params.Editor.lowAltitudeModelingState.boolValue = EditorHelpers.HandleFoldOutGroup(Params.Editor.lowAltitudeModelingState.boolValue, "Modeling", DrawLowAltitudeModeling);
            Params.Editor.lowAltitudeLightingState.boolValue = EditorHelpers.HandleFoldOutGroup(Params.Editor.lowAltitudeLightingState.boolValue, "Lighting", DrawLowAltitudeLighting);
            Params.Editor.lowAltitudeWeatherState.boolValue = EditorHelpers.HandleFoldOutGroup(Params.Editor.lowAltitudeWeatherState.boolValue, "Weather", DrawLowAltitudeWeather);
            Params.Editor.lowAltitudeBaseState.boolValue = EditorHelpers.HandleFoldOutGroup(Params.Editor.lowAltitudeBaseState.boolValue, "Base Clouds", DrawLowAltitudeBase);

            if (GetEnum<TextureIdentifier>(Params.LowAlt.Base.baseTextureID) != TextureIdentifier.None)
            {
                Params.Editor.lowAltitudeDetail1State.boolValue = EditorHelpers.HandleFoldOutGroup(Params.Editor.lowAltitudeDetail1State.boolValue, "Cloud Detail", DrawLowAltitudeDetail1);
                Params.Editor.lowAltitudeCurlState.boolValue = EditorHelpers.HandleFoldOutGroup(Params.Editor.lowAltitudeCurlState.boolValue, "Cloud Distortion", DrawLowAltitudeDistortion);
            }
        }

        void DrawLowAltitudeModeling()
        {
            #region Modeling
            EditorGUILayout.PropertyField(Params.LowAlt.extinctionCoefficient);
            EditorGUILayout.Slider(Params.LowAlt.cloudiness, 0f, 1f);

            EditorGUILayout.Slider(Params.LowAlt.cloudinessDensityInfluence, 0f, 1f);
            EditorGUILayout.Slider(Params.LowAlt.heightDensityInfluence, 0f, 1f);
            EditorGUI.BeginChangeCheck();
            EditorHelpers.CurveProperty("Cloud Density Curve", curve_T);
            if (EditorGUI.EndChangeCheck())
			{
                var t = target as CloudDefinition;

                if (t == null)
                    return;

                t.curve.Release();
                t.curve.SetDirty();
            }
            #endregion

            #region Distant Coverage Configuration
            // Distant Coverage Configuration
            EditorGUILayout.LabelField("Distant Coverage", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(Params.LowAlt.distantCoverageDepth, new GUIContent("Start Distance"));
            EditorGUILayout.Slider(Params.LowAlt.distantCoverageAmount, 0f, 1f, new GUIContent("Cloudiness"));
            EditorGUI.indentLevel--;
            #endregion
        }

        void DrawLowAltitudeLighting()
        {
            EditorGUILayout.PropertyField(Params.LowAlt.maxLightingDistance);
            EditorGUILayout.LabelField("Multiple Scattering", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.IntSlider(Params.LowAlt.multipleScatteringOctaves, 1, 4, new GUIContent("Octaves"));
            if (Params.LowAlt.multipleScatteringOctaves.intValue > 1)
            {
                EditorGUILayout.Slider(Params.LowAlt.multipleScatteringAmpGain, 0f, 1f, new GUIContent("Amp Gain"));
                EditorGUILayout.Slider(Params.LowAlt.multipleScatteringDensityGain, 0f, 1f, new GUIContent("Density Gain"));
            }
            EditorGUI.indentLevel--;
        }

        void DrawLowAltitudeWeather()
        {
            EditorGUILayout.PropertyField(Params.LowAlt.Weather.weathermapType);
            if(Params.LowAlt.Weather.weathermapType.enumValueIndex == (int)WeathermapType.Texture)
            {
                EditorGUILayout.PropertyField(Params.LowAlt.Weather.weathermapTexture);
                EditorGUILayout.PropertyField(Params.LowAlt.Weather.weathermapVelocity);
            }
            else
            {
                EditorGUILayout.LabelField("Type: Perlin, Octaves: 3");
                EditorGUILayout.PropertyField(Params.LowAlt.Weather.weathermapScale, new GUIContent("Scale"));
                EditorGUILayout.PropertyField(Params.LowAlt.Weather.weathermapVelocity, new GUIContent("Velocity"));
            }
        }

        #region Draw Base
        void DrawLowAltitudeBase()
        {
            EditorGUILayout.PropertyField(Params.LowAlt.Base.baseTextureID, new GUIContent("Type"));

            if (GetEnum<TextureIdentifier>(Params.LowAlt.Base.baseTextureID) != TextureIdentifier.None)
            {

                EditorGUILayout.PropertyField(Params.LowAlt.Base.baseTextureQuality, new GUIContent("Quality"));
                EditorGUILayout.Space(5);

                //EditorGUILayout.LabelField("Scale", EditorStyles.boldLabel);
                float s = EditorGUILayout.FloatField("Scale", Params.LowAlt.Base.baseTextureScale.vector3Value.x);
                Params.LowAlt.Base.baseTextureScale.vector3Value = new Vector3(s, s, s);
                EditorGUILayout.PropertyField(Params.LowAlt.Base.baseTextureTimescale, new GUIContent("Velocity"));
                EditorGUILayout.Space(5);
            }
        }
        #endregion


        #region Draw Detail
        private void DrawLowAltitudeDetail1()
        {
            DetailData detailData = new DetailData
            {
                texture = Params.LowAlt.Detail1.detail1TextureID,
                quality = Params.LowAlt.Detail1.detail1TextureQuality,
                influence = Params.LowAlt.Detail1.detail1TextureInfluence,
                heightRemap = Params.LowAlt.Detail1.detail1TextureHeightRemap,
                scale = Params.LowAlt.Detail1.detail1TextureScale,
                timescale = Params.LowAlt.Detail1.detail1TextureTimescale,
                falloffSelection = Params.LowAlt.Detail1.detail1FalloffSelection,
            };

            DrawDetail(detailData);
        }

        private void DrawDetail(DetailData detailData)
        {
            EditorGUILayout.PropertyField(detailData.texture, new GUIContent("Type"));

            if (GetEnum<TextureIdentifier>(detailData.texture) != TextureIdentifier.None)
            {
                EditorGUILayout.PropertyField(detailData.quality, new GUIContent("Quality"));
                EditorGUILayout.Space(5);

                //EditorGUILayout.LabelField("Strength", EditorStyles.boldLabel);
                EditorGUILayout.Slider(detailData.influence, 0f, 1f, new GUIContent("Intensity"));
                EditorGUILayout.PropertyField(detailData.heightRemap, new GUIContent("Height Mapping"));
                EditorGUILayout.Space(5);

                //EditorGUILayout.LabelField("Scale", EditorStyles.boldLabel);
                float s = EditorGUILayout.FloatField("Scale", detailData.scale.vector3Value.x);
                detailData.scale.vector3Value = new Vector3(s, s, s);
                EditorGUILayout.PropertyField(detailData.timescale, new GUIContent("Velocity"));
                EditorGUILayout.Space(5);
            }
        }

        private struct DetailData
        {
            public SerializedProperty texture;
            public SerializedProperty quality;
            public SerializedProperty influence;
            public SerializedProperty heightRemap;
            public SerializedProperty scale;
            public SerializedProperty timescale;
            public SerializedProperty falloffSelection;
        }
        #endregion


        #region Draw Distortion
        private void DrawLowAltitudeDistortion()
        {
            EditorGUILayout.PropertyField(Params.LowAlt.Curl.curlTexture, new GUIContent("Texture"));
            if (Params.LowAlt.Curl.curlTexture.objectReferenceValue != null)
            {
                EditorGUILayout.PropertyField(Params.LowAlt.Curl.curlTextureInfluence, new GUIContent("Intensity"));
                EditorGUILayout.PropertyField(Params.LowAlt.Curl.curlTextureScale, new GUIContent("Scale"));
                EditorGUILayout.PropertyField(Params.LowAlt.Curl.curlTextureTimescale, new GUIContent("Speed"));
            }
        }
        #endregion
        #endregion

        void HandleHighAltitudeDrawing()
        {
            bool tex1State = Params.HighAlt.highAltTex1.objectReferenceValue != null ? true : false;
            bool tex2State = Params.HighAlt.highAltTex2.objectReferenceValue != null ? true : false;
            bool tex3State = Params.HighAlt.highAltTex3.objectReferenceValue != null ? true : false;
            bool aggTexState = tex1State || tex2State || tex3State ? true : false;
            
            if (aggTexState)
            {
                EditorGUILayout.LabelField("Basic Configuration", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(Params.HighAlt.highAltExtinctionCoefficient, new GUIContent("Extinction Coefficient"));
                EditorGUILayout.Slider(Params.HighAlt.highAltCloudiness, 0f, 1f, new GUIContent("Cloudiness"));
                EditorGUI.indentLevel--;
            }

            if (aggTexState)
            {
                EditorGUILayout.Space(10f);
                EditorGUILayout.LabelField("Textures", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
            }

            EditorGUILayout.LabelField("Weathermap", EditorStyles.boldLabel);

            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(Params.HighAlt.highAltTex1, new GUIContent("Texture"));
            if (tex1State)
            {
                EditorGUILayout.PropertyField(Params.HighAlt.highAltScale1, new GUIContent("Scale"));
                EditorGUILayout.PropertyField(Params.HighAlt.highAltTimescale1, new GUIContent("Timescale"));
            }

            EditorGUI.indentLevel--;


            EditorGUILayout.Space(10f);
            EditorGUILayout.LabelField("Cloud Texture 1", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(Params.HighAlt.highAltTex2, new GUIContent("Texture"));
            if (tex2State)
            {
                EditorGUILayout.PropertyField(Params.HighAlt.highAltScale2, new GUIContent("Scale"));
                EditorGUILayout.PropertyField(Params.HighAlt.highAltTimescale2, new GUIContent("Timescale"));
            }
            EditorGUI.indentLevel--;


            EditorGUILayout.Space(10f);
            EditorGUILayout.LabelField("Cloud Texture 2", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(Params.HighAlt.highAltTex3, new GUIContent("Texture"));
            if (tex3State)
            {
                EditorGUILayout.PropertyField(Params.HighAlt.highAltScale3, new GUIContent("Scale"));
                EditorGUILayout.PropertyField(Params.HighAlt.highAltTimescale3, new GUIContent("Timescale"));
            }
            EditorGUI.indentLevel--;

            if (aggTexState) EditorGUI.indentLevel--;
        }

        T GetEnum<T>(SerializedProperty property)
        {
            return (T)Enum.ToObject(typeof(T), property.enumValueIndex);
        }
    }

    static class EditorHelpers
    {
        public static bool HandleFoldOutGroup(bool state, string header, Action controls)
        {
            state = EditorGUILayout.BeginFoldoutHeaderGroup(state, header);

            if (state)
            {
                EditorGUI.indentLevel++;
                controls();
                EditorGUI.indentLevel--;
                EditorGUILayout.Space(10);
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            return state;
        }

        public static void HandleIndentedGroup(string header, Action controls)
        {
            EditorGUILayout.LabelField(header, EditorStyles.boldLabel);

            EditorGUI.indentLevel++;
            controls();
            EditorGUI.indentLevel--;

            EditorGUILayout.Space(10);
        }

        public static void CurveProperty(string label, SerializedProperty curve)
		{
            curve.animationCurveValue = EditorGUILayout.CurveField(label, curve.animationCurveValue, Color.white, new Rect(0, 0, 1, 1));
        }
    }
}