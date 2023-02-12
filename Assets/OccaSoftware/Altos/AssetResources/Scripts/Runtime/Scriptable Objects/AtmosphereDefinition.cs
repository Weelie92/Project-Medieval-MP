using UnityEngine;

namespace OccaSoftware.Altos.Runtime
{
    [CreateAssetMenu(fileName = "Atmosphere Definition", menuName = "Altos/Atmosphere Definition")]
    public class AtmosphereDefinition : ScriptableObject
    {
        [Tooltip("Sets the atmosphere blending end distance. For best results, set this to less than your camera's far clip plane.")]
        public int visibilityKm = 40000;

        /// <summary>
        /// Returns the density value to be used in shaders.
        /// </summary>
        /// <returns>A density value.</returns>
        public float GetDensity()
		{
            return Helpers.GetDensityFromVisibilityDistance(visibilityKm);
        }
    }
}
