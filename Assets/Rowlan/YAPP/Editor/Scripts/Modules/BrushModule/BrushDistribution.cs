using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Rowlan.Yapp
{
    public class BrushDistribution
    {
        private BrushModuleEditor brushModuleEditor;
        private PrefabPainter editorTarget;

        private PreviewPrefab previewPrefab;

        private FilterEvaluator filterEvaluator;
        private PlacementConditionsEvaluator placementConditionsEvaluator;

        public BrushDistribution(BrushModuleEditor brushModuleEditor)
        {
            this.brushModuleEditor = brushModuleEditor;
            this.editorTarget = brushModuleEditor.GetPainter();

            this.filterEvaluator = new FilterEvaluator();
            this.placementConditionsEvaluator = new PlacementConditionsEvaluator();
        }

        public bool HasPreviewPrefab()
        {
            return previewPrefab != null && previewPrefab.prefabInstance != null;
        }

        public void CreatePreviewPrefab()
        {
            if (!this.editorTarget.HasPrefab())
                return;

            if (previewPrefab != null)
                DestroyPreviewPrefab();

            previewPrefab = new PreviewPrefab();

            previewPrefab.prefabSettings = this.editorTarget.CreatePrefabSettings();
            previewPrefab.prefabInstance = PrefabUtility.InstantiatePrefab(previewPrefab.prefabSettings.prefab) as GameObject;

            // disable all colliders so that the preview doesn't affect physics simulation
            Collider[] colliders = previewPrefab.prefabInstance.transform.GetComponentsInChildren<Collider>();
            foreach (Collider collider in colliders)
            {
                collider.enabled = false;
            }

            // don't save the prefab
            previewPrefab.prefabInstance.hideFlags = HideFlags.HideAndDontSave;

            // attach as root
            // as child of container it would be removed on "clear"
            // as child of editortarget it would show navigation handles
            // => root it is
            previewPrefab.prefabInstance.transform.parent = null;

            // move to bottom
            previewPrefab.prefabInstance.transform.SetAsLastSibling();

            // hide in hierarchy
            // TODO: activate. leaving it visible for the time being
            // previewPrefab.prefabInstance.hideFlags = HideFlags.HideInHierarchy;

            previewPrefab.prefabInstance.name = "Preview Prefab [Yapp Temp]";

            // set the layer index for the preview prefab. we need to ignore it in the brush raycast or else there'd be an endless loop
            // with the preview prefab coming close to the camera consistently
            LayerUtils.SetLayer(previewPrefab.prefabInstance.transform, (int)LayerUtils.LayerIndex.IgnoreRaycast);

        }

        public PrefabSettings GetPreviewPrefabSettings()
        {
            return previewPrefab.prefabSettings;
        }

        public Vector3 CalculateBrushOffsetUp()
        {
            if (!HasPreviewPrefab())
                return Vector3.zero;

            Bounds bounds = BoundsUtils.CalculateBounds(previewPrefab.prefabInstance.transform.gameObject);

            // the offset is in mouse wheel dimensions. we don't want the full offset
            // plates should have finer granularity than huge rocks
            float offsetReduction = bounds.size.y / 100f; // 1/100 is just arbitrary. let's see how that turns out during usage, might change later

            float offsetUp = previewPrefab.prefabSettings.brushOffsetUp * offsetReduction;

            return previewPrefab.prefabInstance.transform.up * offsetUp;
        }

        public void UpdatePreviewPrefab(Vector3 position, Vector3 normal)
        {
            if (previewPrefab == null)
                return;

            PrefabTransform appliedTransform = CreateAppliedTransform(previewPrefab.prefabSettings, position, normal);

            previewPrefab.prefabInstance.transform.position = appliedTransform.position;
            previewPrefab.prefabInstance.transform.rotation = appliedTransform.rotation;
            previewPrefab.prefabInstance.transform.localScale = appliedTransform.scale;

            /*
            // toggle visibility depending on shift key
            bool visible = !(Event.current.shift && Event.current.control);
            previewPrefab.prefabInstance.SetActive(visible);
            */

            if(ApplicationSettings.useInstanceAsPreview)
            {
                // nothing to do
            }
            else
            {
                // hide the preview instance
                previewPrefab.prefabInstance.SetActive(false);

                // use DrawMesh to render the instance
                MeshUtils.RenderGameObject(previewPrefab.prefabInstance, 0);
            }
        }

        public void DestroyPreviewPrefab()
        {
            if (previewPrefab == null)
                return;

            PrefabPainter.DestroyImmediate(previewPrefab.prefabInstance);

            previewPrefab.prefabInstance = null;
            previewPrefab.prefabSettings = null;
            previewPrefab = null;
        }

        /// <summary>
        /// Add prefabs, mode Center
        /// </summary>
        public void AddPrefabs_Center(Vector3 position, Vector3 normal)
        {

            // check if a gameobject is already within the brush size
            // allow only 1 instance per bush size
            GameObject container = editorTarget.container as GameObject;

            // check if a prefab already exists within the brush
            bool prefabExists = false;

            // check overlap
            if (!editorTarget.brushSettings.allowOverlap)
            {
                foreach (Transform child in container.transform)
                {
                    // ignore the preview
                    if (previewPrefab != null && child.gameObject == previewPrefab.prefabInstance)
                        continue;

                    float dist = Vector3.Distance(position, child.transform.position);

                    // check against the brush
                    if (dist <= editorTarget.brushSettings.brushSize)
                    {
                        prefabExists = true;
                        break;
                    }

                }
            }

            // create position vector
            Vector3 prefabPosition = new Vector3(position.x, position.y, position.z);

            //// auto physics height offset is moved to AddNewPrefab
            // prefabPosition = ApplyAutoPhysicsHeightOffset(prefabPosition);

            if (!prefabExists)
            {
                bool isInFilter = filterEvaluator.IsInFilter(position, editorTarget.filterSettings);
                bool isConditionsMet = placementConditionsEvaluator.IsConditionsMet(prefabPosition, normal, editorTarget.brushSettings);

                if (isInFilter && isConditionsMet)
                {

                    PrefabSettings prefabSettings = previewPrefab.prefabSettings;

                    AddNewPrefab(prefabSettings, prefabPosition, normal);
                }
            }
        }

        private PrefabTransform CreateAppliedTransform(PrefabSettings prefabSettings, Vector3 position, Vector3 normal)
        {
            ///
            /// Calculate position / rotation / scale
            /// 

            // get new position
            Vector3 newPosition = position;

            // add offset of brush in up direction
            newPosition += CalculateBrushOffsetUp();

            // add offset of prefab settings
            newPosition += prefabSettings.positionOffset;

            // auto physics height offset
            newPosition = ApplyAutoPhysicsHeightOffset(newPosition);

            Vector3 newLocalScale = prefabSettings.prefab.transform.localScale;

            // size
            // please note that the scale might be change later again (scale to brush size)
            // which should happen after the rotation
            if (prefabSettings.changeScale)
            {
                newLocalScale = Vector3.one * Random.Range(prefabSettings.scaleMin, prefabSettings.scaleMax);
            }

            // rotation
            Quaternion alignedRotation = Quaternion.identity;
            Quaternion objectRotation;

            if (this.editorTarget.brushSettings.alignToTerrain)
            {
                float slerpValue;

                if (this.editorTarget.brushSettings.alignToTerrainSlerpRandom)
                {
                    slerpValue = Random.Range(0f, 1f);
                }
                else
                {
                    slerpValue = this.editorTarget.brushSettings.alignToTerrainSlerpValue;
                }

                Vector3 alignedVector = Vector3.Slerp(Vector3.up, normal, slerpValue);
                alignedRotation = Quaternion.FromToRotation(Vector3.up, alignedVector);
            }

            if (prefabSettings.randomRotation)
            {
                objectRotation = prefabSettings.instanceRotation;
            }
            else
            {
                objectRotation = Quaternion.Euler(prefabSettings.rotationOffset);
            }

            // additionally consider brush rotation
            Quaternion brushRotation = Quaternion.Euler(0f, editorTarget.brushSettings.brushRotation, 0f);

            // combine terrain aligned rotation and object rotation
            Quaternion newRotation = alignedRotation * objectRotation * brushRotation;

            // scale to brush size
            // this uses world bounds and happens after the rotation
            if (editorTarget.brushSettings.distribution == BrushSettings.Distribution.Fluent)
            {
                GameObject prefab = prefabSettings.prefab;

                Quaternion prevRotation = prefab.transform.rotation;
                {
                    // we need to rotate the gameobject now in order to calculate the world bounds
                    prefab.transform.rotation = newRotation;

                    float brushSize = editorTarget.brushSettings.brushSize;
                    Bounds worldBounds = BoundsUtils.CalculateBounds(prefab);

                    Vector3 prefabScale = prefab.transform.localScale;

                    float scaleFactorX = brushSize / worldBounds.size.x;
                    float scaleFactorY = brushSize / worldBounds.size.y;
                    float scaleFactorZ = brushSize / worldBounds.size.z;

                    float scaleFactorXYZ = Mathf.Min(scaleFactorX, scaleFactorY, scaleFactorZ);

                    newLocalScale = prefabScale * scaleFactorXYZ;

                }
                prefab.transform.rotation = prevRotation;
            }

            PrefabTransform prefabTransform = new PrefabTransform( newPosition, newRotation, newLocalScale);

            return prefabTransform;

        }

        private void AddNewPrefab(PrefabSettings prefabSettings, Vector3 position, Vector3 normal)
        {
            PrefabTransform appliedTransform = CreateAppliedTransform(prefabSettings, position, normal);

            // create instance and apply position / rotation / scale
            brushModuleEditor.PersistPrefab( prefabSettings, appliedTransform);

        }

        /// <summary>
        /// Add prefabs, mode Poisson
        /// </summary>
        public void AddPrefabs_Poisson_Any(Vector3 position, Vector3 normal, bool forceNewDistribution)
        {
            GameObject container = editorTarget.container as GameObject;

            float brushSize = editorTarget.brushSettings.brushSize;
            float brushRadius = brushSize / 2.0f;
            float discRadius = editorTarget.brushSettings.poissonDiscSize / 2;
            float discSize = editorTarget.brushSettings.poissonDiscSize;

            IEnumerable<Vector2> samples = PoissonDiscSampleProvider.Instance.Samples(brushSize, brushSize, discRadius, forceNewDistribution);

            foreach (Vector2 sample in samples)
            {
                // x/z come from the poisson sample 
                float x = position.x + sample.x - brushRadius;
                float z = position.z + sample.y - brushRadius;

                float y = position.y + editorTarget.brushSettings.poissonDiscRaycastOffset;
                Vector3 raycastPosition = new Vector3(x, y, z);

                Vector3 discNormal;

                // create layer mask without ignore raycast on which the preview gameobject is
                LayerMask layerMask = LayerUtils.GetPreviewLayerMask(editorTarget.brushSettings.layerMask);

                if (Physics.Raycast(raycastPosition, Vector3.down, out RaycastHit raycastHitDown, Mathf.Infinity, layerMask))
                {
                    y = raycastHitDown.point.y;
                    discNormal = raycastHitDown.normal;
                }
                else if (Physics.Raycast(raycastPosition, Vector3.up, out RaycastHit raycastHitUp, Mathf.Infinity, layerMask))
                {
                    y = raycastHitUp.point.y;
                    discNormal = raycastHitUp.normal;
                }
                else
                {
                    continue;
                }

                // create position vector
                Vector3 prefabPosition = new Vector3(x, y, z);

                // auto physics height offset
                prefabPosition = ApplyAutoPhysicsHeightOffset(prefabPosition);

                // check if a prefab already exists within the brush
                bool prefabExists = false;

                // check overlap
                if (!editorTarget.brushSettings.allowOverlap)
                {
                    foreach (Transform child in container.transform)
                    {
                        float dist = Vector3.Distance(prefabPosition, child.transform.position);

                        // check against a single poisson disc
                        if (dist <= discSize)
                        {
                            prefabExists = true;
                            break;
                        }

                    }
                }

                // add prefab
                if (!prefabExists)
                {
                    bool isInFilter = filterEvaluator.IsInFilter(prefabPosition, editorTarget.filterSettings);
                    bool isConditionsMet = placementConditionsEvaluator.IsConditionsMet(prefabPosition, discNormal, editorTarget.brushSettings);

                    if (isInFilter && isConditionsMet)
                    {
                        PrefabSettings prefabSettings = this.editorTarget.CreatePrefabSettings();

                        AddNewPrefab(prefabSettings, prefabPosition, normal);
                    }
                }
            }
        }

        /// <summary>
        /// Add prefabs, mode Poisson
        /// </summary>
        public void AddPrefabs_Poisson_Terrain(Vector3 position, Vector3 normal, bool forceNewDistribution)
        {
            Terrain terrain = TerrainManager.Instance.GetTerrain(position);
            if (!terrain)
                return;

            GameObject container = editorTarget.container as GameObject;

            float brushSize = editorTarget.brushSettings.brushSize;
            float brushRadius = brushSize / 2.0f;
            float discRadius = editorTarget.brushSettings.poissonDiscSize / 2;
            float discSize = editorTarget.brushSettings.poissonDiscSize;

            IEnumerable<Vector2> samples = PoissonDiscSampleProvider.Instance.Samples(brushSize, brushSize, discRadius, forceNewDistribution);

            foreach (Vector2 sample in samples)
            {

                // x/z come from the poisson sample 
                float x = position.x + sample.x - brushRadius;
                float z = position.z + sample.y - brushRadius;

                // y depends on the terrain height
                Vector3 terrainPosition = new Vector3(x, position.y, z);

                // get terrain y position and add Terrain Transform Y-Position
                float y = terrain.SampleHeight(terrainPosition) + terrain.GetPosition().y;

                // raycast against terrain
                float raycastY = y + editorTarget.brushSettings.poissonDiscRaycastOffset;
                Vector3 raycastPosition = new Vector3(x, raycastY, z);

                Vector3 discNormal;

                // create layer mask without ignore raycast on which the preview gameobject is
                LayerMask layerMask = LayerUtils.GetPreviewLayerMask(editorTarget.brushSettings.layerMask);

                if (Physics.Raycast(raycastPosition, Vector3.down, out RaycastHit raycastHitDown, Mathf.Infinity, layerMask))
                {
                    // y = raycastHitDown.point.y; // y is used from the terrain
                    discNormal = raycastHitDown.normal;
                }
                else if (Physics.Raycast(raycastPosition, Vector3.up, out RaycastHit raycastHitUp, Mathf.Infinity, layerMask))
                {
                    // y = raycastHitUp.point.y; // y is used from the terrain
                    discNormal = raycastHitUp.normal;
                }
                else
                {
                    continue;
                }

                // create position vector
                Vector3 prefabPosition = new Vector3(x, y, z);

                // auto physics height offset
                prefabPosition = ApplyAutoPhysicsHeightOffset(prefabPosition);

                // check if a prefab already exists within the brush
                bool prefabExists = false;

                // check overlap
                if (!editorTarget.brushSettings.allowOverlap)
                {
                    foreach (Transform child in container.transform)
                    {
                        float dist = Vector3.Distance(prefabPosition, child.transform.position);

                        // check against a single poisson disc
                        if (dist <= discSize)
                        {
                            prefabExists = true;
                            break;
                        }

                    }
                }

                // add prefab
                if (!prefabExists)
                {
                    bool isInFilter = filterEvaluator.IsInFilter(prefabPosition, editorTarget.filterSettings);
                    bool isConditionsMet = placementConditionsEvaluator.IsConditionsMet(prefabPosition, discNormal, editorTarget.brushSettings);

                    if (isInFilter && isConditionsMet)
                    {
                        PrefabSettings prefabSettings = this.editorTarget.CreatePrefabSettings();

                        AddNewPrefab(prefabSettings, prefabPosition, normal);
                    }
                }
            }
        }

        /// <summary>
        /// Add additional height offset if auto physics is enabled
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        private Vector3 ApplyAutoPhysicsHeightOffset(Vector3 position)
        {
            if (editorTarget.spawnSettings.autoSimulationType == SpawnSettings.AutoSimulationType.None)
                return position;

            // auto physics: add additional height offset
            position.y += editorTarget.spawnSettings.autoSimulationHeightOffset;

            return position;
        }

    }
}
