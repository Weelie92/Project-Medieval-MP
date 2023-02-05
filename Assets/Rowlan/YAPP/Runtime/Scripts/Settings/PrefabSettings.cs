using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Rowlan.Yapp
{
    [System.Serializable]
    public class PrefabSettings
    {

        public enum RotationRange
        {
            [InspectorName("0..360")]
            Base_360,

            [InspectorName("-180..180")]
            Base_180
        }

        /// <summary>
        /// The name which will be displayed in the prefab template grid of the inspector
        /// </summary>
        public string templateName;

        /// <summary>
        /// The prefab which should be instanted and placed at the brush position
        /// </summary>
        [HideInInspector]
        public GameObject prefab = null;

        /// <summary>
        /// Whether the prefab is used or not
        /// </summary>
        public bool active = true;

        /// <summary>
        /// The probability at which the prefab is chosen to be instantiated.
        /// This value is relative to all other prefabs.
        /// So 0 doesn't mean it won't be instantiated at all, it means it's less probable
        /// to be instantiated than others which don't have 0.
        /// Ranges from 0 (not probable at all) to 1 (highest probability).
        /// The value is relative. If all a
        /// </summary>
        public float probability = 1;

        /// <summary>
        /// The offset that should be added to the instantiated gameobjects position.
        /// This is useful to correct the position of prefabs. 
        /// It's also useful in combination with the physics module in order to let e. g. pumpkins fall naturally on the terrain.
        /// </summary>
        public Vector3 positionOffset;

        /// <summary>
        /// The offset that should be added to the instantiated gameobjects rotation.
        /// This is useful to correct the rotation of prefabs.
        /// The offset is Vector3, uses Eulers.
        /// </summary>
        public Vector3 rotationOffset;

        /// <summary>
        /// Randomize rotation
        /// </summary>
        public bool randomRotation;

        /// <summary>
        /// The rotation range
        /// </summary>
        public RotationRange rotationRange = RotationRange.Base_360;

        /// <summary>
        /// Minimum X rotation in degrees when random rotation is used.
        /// </summary>
        public float rotationMinX = 0f;

        /// <summary>
        /// Maximum X rotation in degrees when random rotation is used.
        /// </summary>
        public float rotationMaxX = 360f;

        /// <summary>
        /// Minimum Y rotation in degrees when random rotation is used.
        /// </summary>
        public float rotationMinY = 0f;

        /// <summary>
        /// Maximum Y rotation in degrees when random rotation is used.
        /// </summary>
        public float rotationMaxY = 360f;

        /// <summary>
        /// Minimum Z rotation in degrees when random rotation is used.
        /// </summary>
        public float rotationMinZ = 0f;

        /// <summary>
        /// Maximum Z rotation in degrees when random rotation is used.
        /// </summary>
        public float rotationMaxZ = 360f;

        /// <summary>
        /// Randomize Scale Minimum
        /// </summary>
        public bool changeScale = false;

        /// <summary>
        /// Randomize Scale Minimum
        /// </summary>
        public float scaleMin = 0.5f;

        /// <summary>
        /// Randomize Scale Maximum
        /// </summary>
        public float scaleMax = 1.5f;

        /// <summary>
        /// Storing asset GUID here for future reference (performance reasons)
        /// </summary>
        [HideInInspector]
        public string assetGUID = null;

        /// <summary>
        /// Vegetation Studio Pro vspro_VegetationItemID
        /// </summary>
        [HideInInspector]
        public string vspro_VegetationItemID = null;

        public Quaternion instanceRotation = Quaternion.identity;

        /// <summary>
        /// Additional offset which will be applied along the up axis of the prefab.
        /// This is used to make corrections in the up axis while the preview prefab is being moved in fluent mode
        /// </summary>
        public float brushOffsetUp = 0f;

        /// <summary>
        /// Apply the settings of the template to the current prefab settings
        /// </summary>
        /// <param name="template"></param>
        public void ApplyTemplate(PrefabSettingsTemplate template)
        {

            active = template.active;
            probability = template.probability;
            positionOffset = template.positionOffset;
            rotationOffset = template.rotationOffset;
            randomRotation = template.randomRotation;
            // rotationRange = template.rotationRange; not available in the template
            rotationMinX = template.rotationMinX;
            rotationMaxX = template.rotationMaxX;
            rotationMinY = template.rotationMinY;
            rotationMaxY = template.rotationMaxY;
            rotationMinZ = template.rotationMinZ;
            rotationMaxZ = template.rotationMaxZ;
            changeScale = template.changeScale;
            scaleMin = template.scaleMin;
            scaleMax = template.scaleMax;

            UpdateInstanceData();

        }

        /// <summary>
        /// Apply the settings of one prefab to this one
        /// </summary>
        /// <param name="template"></param>
        public void Apply(PrefabSettings template)
        {
            active = template.active;
            probability = template.probability;
            positionOffset = template.positionOffset;
            rotationOffset = template.rotationOffset;
            randomRotation = template.randomRotation;
            rotationRange = template.rotationRange;
            rotationMinX = template.rotationMinX;
            rotationMaxX = template.rotationMaxX;
            rotationMinY = template.rotationMinY;
            rotationMaxY = template.rotationMaxY;
            rotationMinZ = template.rotationMinZ;
            rotationMaxZ = template.rotationMaxZ;
            changeScale = template.changeScale;
            scaleMin = template.scaleMin;
            scaleMax = template.scaleMax;


            UpdateInstanceData();

        }

        public PrefabSettings Clone()
        {
            return (PrefabSettings)this.MemberwiseClone();
        }

        /// <summary>
        /// Set the data depending on the settings when an instance is created.
        /// We don't want to eg have consistent random rotation while the prefab is being moved around.
        /// </summary>
        public void UpdateInstanceData()
        {
            float rotationX = Random.Range(rotationMinX, rotationMaxX);
            float rotationY = Random.Range(rotationMinY, rotationMaxY);
            float rotationZ = Random.Range(rotationMinZ, rotationMaxZ);

            instanceRotation = Quaternion.Euler(rotationX, rotationY, rotationZ);

            // don't keep the brush offset
            brushOffsetUp = 0f;
        }

    }

}
