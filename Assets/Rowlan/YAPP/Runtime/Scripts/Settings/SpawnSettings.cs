using UnityEngine;

namespace Rowlan.Yapp
{
    [System.Serializable]
    public class SpawnSettings
    {
        public enum AutoSimulationType
        {
            None,
            Continuous
        }
        public enum AutoCollider
        {
            [Tooltip("Add collider only to objects which are spawned during the current physics simulation")]
            SpawnedOnly,

            [Tooltip("Add a collider to all container children during the current physics simulation")]
            Container,

        }

        /// <summary>
        /// Automatically apply physics after painting
        /// </summary>
        public AutoSimulationType autoSimulationType = AutoSimulationType.None;

        /// <summary>
        /// When auto physics is enabled, then this value will be added to the y-position of the gameobject.
        /// This way e. g. rocks are placed higher by default and gravity can be applied
        /// </summary>
        public float autoSimulationHeightOffset = 1f;

        /// <summary>
        /// Add missing colliders either only to those object which are spawned during the physics simulation or to all container children
        /// </summary>
        public AutoCollider autoSimulationCollider = AutoCollider.SpawnedOnly;
    }
}