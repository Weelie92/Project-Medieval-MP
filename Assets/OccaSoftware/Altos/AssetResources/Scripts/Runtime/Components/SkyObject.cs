using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace OccaSoftware.Altos.Runtime
{
    /// <summary>
    /// A Sky Object describes a sun, moon, or other celestial body that should be rendered independently.
    /// <br/>
    /// </summary>
    [AddComponentMenu("Altos/Sky Object")]
    [ExecuteAlways]
    public class SkyObject : MonoBehaviour
    {
        private const float horizonAngle = -5f;

        private static List<SkyObject> _SkyObjects = new List<SkyObject>();
        public static List<SkyObject> SkyObjects
		{
            get => _SkyObjects;
		}

        private static List<SkyObject> _Sun = new List<SkyObject>();
        public static SkyObject Sun
		{
            get => _Sun.Count > 0 ? _Sun[0] : null;
		}

        internal void Index(SkyObject skyObject)
        {
			if (!_SkyObjects.Contains(skyObject))
			{
                _SkyObjects.Add(skyObject);
            }
            
            _SkyObjects = _SkyObjects.OrderByDescending(o => o.sortOrder).ToList();

            if (skyObject.type == ObjectType.Sun)
            {
				if (!_Sun.Contains(skyObject))
				{
                    _Sun.Add(skyObject);
				}
            }
        }

        internal void Delete(SkyObject skyObject)
        {
            _SkyObjects.Remove(skyObject);
            _Sun.Remove(skyObject);
            _SkyObjects = _SkyObjects.OrderByDescending(o => o.sortOrder).ToList();
        }

        private Transform earth
        {
            get => transform.parent;
        }

        private Transform child
		{
			get
			{
                if(transform.childCount <= 0)
				{
                    GameObject child = new GameObject(name + " Light");
                    child.transform.hideFlags = HideFlags.NotEditable;
                    child.AddComponent<Light>();
                    child.transform.parent = transform;
                }

                return transform.GetChild(0);
			}
		}

        public Transform GetChild()
		{
            return child;
		}

        public Quaternion GetRotation()
		{
            return child.transform.rotation;
        }


        private Mesh quad = null;

        public Mesh Quad
		{
			get
			{
                if(quad == null)
				{
                    quad = Helpers.CreateQuad();
				}

                return Helpers.CreateQuad();
			}
		}

        private Light _light;

        [Header("Identity")]
        [Tooltip("When set to Sun, this object will be treated as the main directional light in the scene and will be used for shadow rendering, cloud rendering, and cloud shadow rendering. Only one sun is supported. A sun must be present in the scene.")]
        public ObjectType type = ObjectType.Sun;
        public enum ObjectType
        {
            Sun,
            Other
        }

        [Header("Rendering")]
        [Range(0, 90)]
        [Tooltip("The angular diameter of the object. Bigger values means a bigger object in the sky.")]
        public float angularDiameterDegrees = 0.52f;

        [Tooltip("The texture to use for the sky object. When no texture is set, the object will still be rendered as a circle.")]
        public Texture2D texture = null;

        [Tooltip("Tint color applied to the object.")]
        [ColorUsage(false, true)]
        public Color objectColor = Color.white;

        [Header("Lighting")]
        [Tooltip("Automatically set the color temperature and brightness based on the object's position in the sky.")]
        public bool automaticColorAndBrightness = true;

        [Min(0)]
        [Tooltip("Light intensity when the object is at its zenith.")]
        public float peakIntensity = 3;

        [Tooltip("Tint color applied to the directional light owned by the object. Lower the value to fade the object into the skybox. It will still occlude stars.")]
        [ColorUsage(false, true)]
        public Color lightingColorMask = Color.white;

        private Material material = null;
        public Material GetMaterial()
		{
            if (material == null)
            {
                Shader s = FindObjectOfType<AltosSkyDirector>()?.data.shaders.skyObjectShader;
                if (s != null)
                {
                    material = new Material(s);
                }
            }

            return material;
        }
        
        public float CalculateSize()
		{
            return Mathf.Tan(angularDiameterDegrees * Mathf.Deg2Rad) * sortOrder;
        }
        
        [HideInInspector]
        public Vector3 positionRelative;
        [Header("Positioning")]
        [Range(1, 10)]
        [Tooltip("Objects with a higher sort order are considered farther away and will be rendered behind objects with a lower sort order.")]
        public int sortOrder = 5;
        
        [Tooltip("Distance along the path around this planet.")]
        public float orbitOffset = 0f;
        [Tooltip("The angle between the path of this sky object and this planet's elliptical.")]
        public float inclinationOffset = 0f;

        [Tooltip("When enabled, this sky object's position will be static throughout the day-night cycle.")]
        public bool positionIsStatic = false;

        [HideInInspector]
        public Vector3 direction;
        public Vector3 GetForward() { return -direction; }

        [Header("Sky Influence")]
        [Min(0)]
        [Tooltip("Higher values cause the color of this object to bleed into the sky color. Only applies for objects of type Sun.")]
        public float falloff = 1;


        [Header("Editor Properties")]
        [Tooltip("Sets the color of the handles used for this object in the editor.")]
        public Color handleColor = Color.white;


        private void OnEnable()
		{
            Index(this);
            SetIcon();
		}


        public void SetIcon()
        {
#if UNITY_EDITOR
            string directory = AltosData.packagePath + "/Textures/Editor/";
            string id = "sun-icon.png";
            if(type == ObjectType.Other)
			{
                id = "moon-icon.png";
			}
            Texture2D icon = (Texture2D)UnityEditor.AssetDatabase.LoadAssetAtPath(directory + id, typeof(Texture2D));
            UnityEditor.EditorGUIUtility.SetIconForObject(child.gameObject, icon);
#endif
        }

        private void OnDisable()
		{
            Delete(this);
		}

		private void OnDestroy()
		{
            Delete(this);
		}


		private void OnValidate()
        {
            Delete(this);
            Index(this);
            SetIcon();


            // TO DO: Move out of OnValidate
            if (orbitOffset > 360f)
                orbitOffset = 0f;
            if (orbitOffset < 0f)
                orbitOffset = 360f;

            if (inclinationOffset > 90f)
                inclinationOffset = -90f;
            if (inclinationOffset < -90f)
                inclinationOffset = 90f;
        }


		void Update()
        {
            
            UpdateRotations();
            UpdateDistance();
            UpdatePositionAndDirection();
            

            GetLight();
            if (automaticColorAndBrightness)
                UpdateLightProperties();

            SetShaderProperties();
        }

        public Color GetColor()
		{
            return GetLightColor();
		}
        public float GetFalloff()
		{
            return falloff;
		}

        public Vector4 GetDirection()
		{
            return direction;
		}

        /// <summary>
        /// Sets the shader properties for self-rendering and for global variables (like sun color and intensity).
        /// Local variables (like atmosphere falloff) are set in during the relevant render pass.
        /// </summary>
        private void SetShaderProperties()
		{
            if(type == ObjectType.Sun)
			{
                Shader.SetGlobalVector(ShaderParams._SunDirection, direction);
                Shader.SetGlobalColor(ShaderParams._SunColor, GetLightColor());
                Shader.SetGlobalFloat(ShaderParams._SunIntensity, GetLight().intensity);
                Shader.SetGlobalFloat(ShaderParams._SunFalloff, falloff);
            }

            if(texture != null)
                GetMaterial().SetTexture(ShaderParams._MainTex, texture);

            GetMaterial().SetColor(ShaderParams._Color, objectColor);
        }

        internal static class ShaderParams
		{
            public static int _SunDirection = Shader.PropertyToID("_SunDirection");
            public static int _SunColor = Shader.PropertyToID("_SunColor");
            public static int _SunIntensity = Shader.PropertyToID("_SunIntensity");
            public static int _SunFalloff = Shader.PropertyToID("_SunFalloff");

            public static int _MainTex = Shader.PropertyToID("_MainTex");
            public static int _Color = Shader.PropertyToID("_Color");
        }

        private void UpdateRotations()
		{
            transform.localRotation = Quaternion.identity;
            transform.Rotate(Vector3.right, -inclinationOffset);
            
            float timeOfDayOffset = 0f;
			if (!positionIsStatic)
			{
                if(earth.TryGetComponent(out AltosSkyDirector data))
				{
                    if(data.skyDefinition != null)
					{
                        timeOfDayOffset = data.skyDefinition.CurrentTime * AltosSkyDirector._HOURS_TO_DEGREES;
                    }
				}
            }
            
            transform.Rotate(Vector3.forward, orbitOffset + timeOfDayOffset, Space.World);
            child.transform.LookAt(earth.position, Vector3.Cross(transform.right, transform.up));
        }

        private void UpdateDistance()
		{
            child.localPosition = new Vector3(0, -sortOrder, 0);
        }

        private void UpdatePositionAndDirection()
		{
            positionRelative = child.position - earth.position;
            direction = positionRelative.normalized;
        }


        public Light GetLight()
		{
            if(_light == null)
			{
                _light = child.GetComponent<Light>();
                if(_light == null)
				{
                    _light = child.gameObject.AddComponent<Light>();
                }
			}

            return _light;
        }

        public Color GetLightColor()
		{
            return Mathf.CorrelatedColorTemperatureToRGB(GetLight().colorTemperature) * GetLight().color;
		}

        /* Source: Wikipedia (https://en.wikipedia.org/wiki/Golden_hour_(photography))
         * The color temperature of daylight varies with the time of day. 
         * It tends to be around 2,000 K shortly after sunrise or before sunset, 
         * around 3,500 K during "golden hour", 
         * and around 5,500 K at midday.
        */
        float cachedAngle = 0f;
        float cachedPeak = 0f;
        private bool UpdateLightProperties()
        {

            _light.type = LightType.Directional;
            if(type != ObjectType.Sun)
			{
                _light.shadows = LightShadows.None;
			}
            float lightAngle = direction.y * 180f;
            

            _light.useColorTemperature = true;
            _light.color = lightingColorMask;
            float cachedTemp = _light.colorTemperature;
            float cachedIntensity = _light.intensity;

            if (Mathf.Abs(cachedAngle - lightAngle) < 0.1f && Mathf.Abs(cachedPeak - peakIntensity) < 0.1f)
			{
                return false;
			}
            
            cachedAngle = lightAngle;
            cachedPeak = peakIntensity;

            if (lightAngle < -horizonAngle)
            {
                _light.intensity = 0;
                _light.colorTemperature = 2000;
            }

            if (lightAngle > -horizonAngle)
            {
                float t = Helpers.Remap01(lightAngle, -horizonAngle, 70f);
                _light.intensity = Mathf.Lerp(0f, peakIntensity, t);
                _light.colorTemperature = 2000f + 3500f * t;
            }

            if (Mathf.Abs(cachedTemp - _light.colorTemperature) > 1f || Mathf.Abs(cachedIntensity - _light.intensity) > 0.01f)
            {
                return true;
            }

            return false;
        }

        public float CalculateDiscIntensity()
		{
            float solidAngle = 2f * Mathf.PI * (1f - Mathf.Cos(0.5f * angularDiameterDegrees * Mathf.Deg2Rad));
            float v = peakIntensity / solidAngle;
            Debug.Log(v);
            return v;
		}


        #region Editor
        #if UNITY_EDITOR

        private void OnDrawGizmos()
		{
            Transform[] transforms = UnityEditor.Selection.transforms;
            foreach (Transform transform in transforms)
            {
                if (transform.root == earth)
                {
                    DrawGizmos();
                }
            }
            
        }

		private void DrawGizmos()
		{

            UnityEditor.Handles.color = handleColor;
            Gizmos.color = handleColor;


            float euclidianDistance = Mathf.Sqrt(positionRelative.y * positionRelative.y + positionRelative.x * positionRelative.x);
            DrawUpperArc(euclidianDistance);


            DimColorIfBelowHorizon();
            DrawDirectLine();
            DrawSphere();


            UseDimColor();
            DrawIndirectLines();
            DrawLowerArc(euclidianDistance);
            DrawDiscHighlight();

            DrawText();
        }



		private void DimColorIfBelowHorizon()
		{
            if (positionRelative.y < 0)
            {
                UnityEditor.Handles.color = new Color(handleColor.r, handleColor.g, handleColor.b, 0.2f);
                Gizmos.color = new Color(handleColor.r, handleColor.g, handleColor.b, 0.2f);
            }
        }

        private void UseDimColor()
		{
            float dimColorAlpha = 0.15f;
            UnityEditor.Handles.color = new Color(handleColor.r, handleColor.g, handleColor.b, dimColorAlpha);
            Gizmos.color = new Color(handleColor.r, handleColor.g, handleColor.b, dimColorAlpha);
        }

        private void DrawDirectLine()
		{
            UnityEditor.Handles.DrawLine(earth.position, child.position, 3f);
        }
        private void DrawSphere()
		{
            Gizmos.DrawWireSphere(child.transform.position, Helpers.Remap(angularDiameterDegrees, 0, 90, 0.2f, 1f));
        }

        private void DrawIndirectLines()
		{
            UnityEditor.Handles.DrawLine(earth.position, new Vector3(child.position.x, earth.position.y, child.position.z), 0f);
            UnityEditor.Handles.DrawLine(new Vector3(child.position.x, earth.position.y, child.position.z), child.position, 0f);
        }

        private void DrawUpperArc(float euclidianDistance)
		{
            UnityEditor.Handles.DrawWireArc(earth.transform.position + new Vector3(0, 0, positionRelative.z), Vector3.forward, Vector3.right, 180f, euclidianDistance, 2f);
        }

        private void DrawLowerArc(float euclidianDistance)
		{
            UnityEditor.Handles.DrawWireArc(earth.transform.position + new Vector3(0, 0, positionRelative.z), Vector3.forward, Vector3.right, -180f, euclidianDistance, 2f);
        }

        private void DrawDiscHighlight()
		{
            UnityEditor.Handles.color = new Color(1, 1, 1, 0.3f);
            UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, sortOrder, 1f);
        }

        private void DrawText()
        {
            GUIStyle s = new GUIStyle();
            s.fontSize = 12;
            s.normal.textColor = Color.white;
            
            s.alignment = TextAnchor.UpperRight;
            s.fontStyle = FontStyle.Bold;

            UnityEditor.Handles.color = new Color(1, 1, 1, 0.5f);
            Vector3 offset = Vector3.right;
            UnityEditor.Handles.Label(child.transform.position + offset * 3f, new GUIContent($"{name.ToLower()}"), s);

            s.fontSize = 9;
            s.normal.textColor = new Color(1, 1, 1, 0.8f);
            UnityEditor.Handles.Label(child.transform.position + offset * 3f, new GUIContent($"\n\ndiameter\n {angularDiameterDegrees:0.00}°\n\norbit\n {orbitOffset:0.00}°\n\ninclination\n {inclinationOffset:0.00}°\n\nintensity\n {GetLight().intensity:0.00}"), s);

            UnityEditor.Handles.DrawLine(child.transform.position + offset * 2f, child.position, 0f);
        }
        #endif
		#endregion
	}

}