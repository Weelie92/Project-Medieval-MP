using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rowlan.Yapp
{
    [System.Serializable]
    public class BrushSettings
    {
        public enum Distribution
        {
            [InspectorName("Fluent (with Preview)")]
            Fluent,

            [InspectorName("Center")]
            Center,

            [InspectorName("Poisson (Any Collider)")]
            Poisson_Any,

            [InspectorName("Poisson (Terrain Only)")]
            Poisson_Terrain

            /*
            FallOff,
            FallOff2d
            */
        }

        public enum SpawnTarget
        {
            PrefabContainer = 0,
            TerrainTrees = 1,
            /* TerrainDetails = 2,*/
            VegetationStudioPro = 3,
        }

        public enum CheckCollider
        {
            /// <summary>
            /// Don't check against colliders
            /// </summary>
            None,

            /// <summary>
            /// Check only against colliders of siblings in the same container
            /// </summary>
            Container,

            /// <summary>
            /// Check against all colliders
            /// </summary>
            All
        }

        public float brushSize = 2.0f;

        /// <summary>
        /// Brush rotation for placement
        /// In single placement mode this acts as global prefab angle (but overwritten eg by random rotation). Set to 0 if you don't want it
        /// </summary>
        [Range(0, 360)]
        public int brushRotation = 0;

        /// <summary>
        /// Brush size
        /// </summary>
        public bool sizeGuide = true;

        /// <summary>
        /// Brush normal
        /// </summary>
        public bool normalGuide = true;

        /// <summary>
        /// Brush rotation
        /// </summary>
        public bool rotationGuide = false;

        /// <summary>
        /// Align the gameobject to the terrain's normal
        /// </summary>
        public bool alignToTerrain = false;

        /// <summary>
        /// Slerp align randomly between [0,1}
        /// </summary>
        public bool alignToTerrainSlerpRandom = false;

        /// <summary>
        /// Slerp align explicitly with the specified value
        /// </summary>
        [Range(0f,1f)]
        public float alignToTerrainSlerpValue = 1f;

        public Distribution distribution = Distribution.Fluent;

        /// <summary>
        /// The size of a disc in the poisson distribution.
        /// The smaller, the more discs will be inside the brush
        /// </summary>
        public float poissonDiscSize = 1.0f;

        /// <summary>
        /// If any collider (not only terrain) is used for the raycast, then this will used as offset from which the ray will be cast against the collider
        /// </summary>
        public float poissonDiscRaycastOffset = 100f;

        /// <summary>
        /// New distribution per click or keep the distribution always the same
        /// </summary>
        public bool poissonDiscsRandomized = true;

        /// <summary>
        /// Visualize the poisson discs
        /// </summary>
        public bool poissonDiscsVisible = false;

        /// <summary>
        /// Falloff curve
        /// </summary>
        public AnimationCurve fallOffCurve = AnimationCurve.Linear(1, 1, 1, 1);

        public AnimationCurve fallOff2dCurveX = AnimationCurve.Linear(1, 1, 1, 1);
        public AnimationCurve fallOff2dCurveZ = AnimationCurve.Linear(1, 1, 1, 1);

        [Range(1,50)]
        public int curveSamplePoints = 10;

        // slope
        public bool slopeEnabled = false;
        public float slopeMin = 0;
        public float slopeMinLimit = 0;
        public float slopeMax = 90;
        public float slopeMaxLimit = 90;

        /// <summary>
        /// Allow prefab overlaps or not.
        /// </summary>
        public bool allowOverlap = false;

        /// <summary>
        /// Check the prefab scale magnitude * 2 radius if there are colliding objects. In that case skip the instantiation of the prefab
        /// </summary>
        public CheckCollider checkCollider = CheckCollider.None;

        /// <summary>
        /// Layer mask for the brush raycast
        /// </summary>
        public LayerMask layerMask = (int) LayerUtils.LayerIndex.Everything;

        /// <summary>
        /// Where to add the instantiated objects
        /// </summary>
        public SpawnTarget spawnTarget = SpawnTarget.PrefabContainer;

        // The terrain to work with
        public Terrain targetTerrain = null;
    }
}
