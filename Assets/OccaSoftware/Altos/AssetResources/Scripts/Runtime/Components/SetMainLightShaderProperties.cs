using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OccaSoftware.Altos.Runtime
{
    /// <summary>
    /// Use this component on your main directional light when rendering clouds without the other components of Altos.
    /// <br/>
    /// </summary>
    [ExecuteAlways]
    [AddComponentMenu("Altos/SetMainLightShaderProperties")]
    [RequireComponent(typeof(Light))]
    public class SetMainLightShaderProperties : MonoBehaviour
    {
        private Light _light;
        private Light GetLight()
        {
            if (_light == null)
            {
                _light = GetComponent<Light>();
            }

            return _light;
        }

		private void Update()
		{
            SetProperties();
		}

		private void SetProperties()
		{
            Shader.SetGlobalVector(SkyObject.ShaderParams._SunDirection, -transform.forward);
            Shader.SetGlobalColor(SkyObject.ShaderParams._SunColor, GetLight().color);
            Shader.SetGlobalFloat(SkyObject.ShaderParams._SunIntensity, GetLight().intensity);
        }
    }
}
