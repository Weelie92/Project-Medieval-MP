using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
#if VEGETATION_STUDIO_PRO
using AwesomeTechnologies.Vegetation.PersistentStorage;
using AwesomeTechnologies.VegetationSystem;
using AwesomeTechnologies.VegetationStudio;
#endif

namespace Rowlan.Yapp
{
    public class VegetationStudioProIntegration
    {
        PrefabPainterEditor editor;

        public VegetationStudioProIntegration(PrefabPainterEditor editor)
        {
            this.editor = editor;
        }

        public void OnInspectorGUI()
        {
#if VEGETATION_STUDIO_PRO
            // no detail settings yet
#else
            EditorGUILayout.HelpBox("Vegetation Studio Pro not found", MessageType.Error);
#endif
        }

        public void OnInspectorSettingsGUI()
        {
            // no detail settings yet
        }

        public void AddNewPrefab(PrefabSettings prefabSettings, Vector3 newPosition, Quaternion newRotation, Vector3 newLocalScale)
        {
#if VEGETATION_STUDIO_PRO

                // ensure the prefab has a VegetationItemID
                updateVSProSettings( prefabSettings, true);

                if( !string.IsNullOrEmpty( prefabSettings.vspro_VegetationItemID))
                {
                    string vegetationItemID = prefabSettings.vspro_VegetationItemID;
                    Vector3 worldPosition = newPosition;
                    Vector3 scale = newLocalScale; // TODO local or world?
                    Quaternion rotation = newRotation;
                    bool applyMeshRotation = true; // TODO ???
                    float distanceFalloff = 1f; // TODO ???
                    bool clearCellCache = true; // TODO ???

                    byte vegetationSourceID = Constants.VegetationStudioPro_SourceId;

                    VegetationStudioManager.AddVegetationItemInstance(vegetationItemID, worldPosition, scale, rotation, applyMeshRotation, vegetationSourceID, distanceFalloff, clearCellCache);

                }
#endif

        }

        /// <summary>
        /// Remove vegetation items.
        /// VSP operates on instance ID, so we need to iterate through all prefabs of the container and invoke the remove operation on them.
        /// </summary>
        /// <param name="raycastHit"></param>
        /// <param name="brushSize"></param>
        public void RemovePrefabs(RaycastHit raycastHit, float brushSize)
        {
#if VEGETATION_STUDIO_PRO

            float radius = brushSize / 2f;

            for (int i = 0; i < editor.GetPainter().prefabSettingsList.Count; i++)
            {
                string vspro_VegetationItemID = editor.GetPainter().prefabSettingsList[i].vspro_VegetationItemID;

                if (string.IsNullOrEmpty(vspro_VegetationItemID))
                    continue;

                VegetationStudioManager.RemoveVegetationItemInstance(vspro_VegetationItemID, raycastHit.point, radius, true);

            }

#endif
        }

        /// <summary>
        /// Ensure the prefab has a VegetationItemID
        /// </summary>
        /// <param name="prefabSettings"></param>
        private void updateVSProSettings(PrefabSettings prefabSettings, bool forceVegetationItemIDUpdate)
        {
#if VEGETATION_STUDIO_PRO

            GameObject prefab = prefabSettings.prefab;

            // check if we have a VegetationItemID, otherwise create it using the current prefab
            if (string.IsNullOrEmpty(prefabSettings.vspro_VegetationItemID) || forceVegetationItemIDUpdate)
            {
                // get the asset guid
                if (string.IsNullOrEmpty(prefabSettings.assetGUID))
                {
                    string assetPath = AssetDatabase.GetAssetPath(prefab);
                    if (!string.IsNullOrEmpty(assetPath))
                    {
                        string assetGUID = AssetDatabase.AssetPathToGUID(assetPath);
                        prefabSettings.assetGUID = assetGUID;
                    }
                }

                // if we have a guid, get the vs pro id
                if (!string.IsNullOrEmpty(prefabSettings.assetGUID))
                {
                    // get the VegetationItemID
                    prefabSettings.vspro_VegetationItemID = VegetationStudioManager.GetVegetationItemID(prefabSettings.assetGUID);

                    // if the vegetation item id doesn't exist, create a new vegetation item
                    if (string.IsNullOrEmpty(prefabSettings.vspro_VegetationItemID))
                    {
                        VegetationType vegetationType = VegetationType.Objects;
                        bool enableRuntimeSpawn = false; // no runtime spawn, we want it spawned from persistent storage
                        BiomeType biomeType = BiomeType.Default;

                        prefabSettings.vspro_VegetationItemID = VegetationStudioManager.AddVegetationItem(prefab, vegetationType, enableRuntimeSpawn, biomeType);
                    }

                }
                else
                {
                    Debug.LogError("Can't get assetGUID for prefab " + prefab);
                }
            }

            if (string.IsNullOrEmpty(prefabSettings.vspro_VegetationItemID))
            {
                Debug.LogError("Can't get VegetationItemId for prefab " + prefab);
            }
#endif
        }
    }
}