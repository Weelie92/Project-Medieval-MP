using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using static Rowlan.Yapp.BrushComponent;


namespace Rowlan.Yapp
{
    public class BrushModuleEditor: ModuleEditorI
    {
        #region Properties

        SerializedProperty brushSize;
        SerializedProperty brushRotation;
        SerializedProperty sizeGuide;
        SerializedProperty normalGuide;
        SerializedProperty rotationGuide;
        SerializedProperty allowOverlap;
        SerializedProperty checkCollider;
        SerializedProperty alignToTerrain;
        SerializedProperty alignToTerrainSlerpRandom;
        SerializedProperty alignToTerrainSlerpValue;
        SerializedProperty layerMask;
        SerializedProperty distribution;
        SerializedProperty poissonDiscSize;
        SerializedProperty poissonDiscRaycastOffset;
        SerializedProperty poissonDiscsRandomized;
        SerializedProperty poissonDiscsVisible;
        SerializedProperty fallOffCurve;
        SerializedProperty fallOff2dCurveX;
        SerializedProperty fallOff2dCurveZ;
        SerializedProperty curveSamplePoints;
        SerializedProperty slopeEnabled;

        SerializedProperty spawnTarget;

        #endregion Properties

        #region Integration to external applications

        VegetationStudioProIntegration vegetationStudioProIntegration;
        UnityTerrainTreesIntegration unityTerrainTreesIntegration;

        #endregion Integration to external applications

#pragma warning disable 0414
        PrefabPainterEditor editor;
        #pragma warning restore 0414
         
        PrefabPainter editorTarget;

        BrushComponent brushComponent = new BrushComponent();

        /// <summary>
        /// Auto physics only on special condition:
        /// + prefabs were added
        /// + mouse got released
        /// </summary>
        private bool needsPhysicsApplied = false;

        private List<GameObject> autoPhysicsCollection = new List<GameObject>();

        private BrushDistribution brushDistribution;

        public BrushModuleEditor(PrefabPainterEditor editor)
        {
            this.editor = editor;
            this.editorTarget = editor.GetPainter();

            brushDistribution = new BrushDistribution( this);

            brushSize = editor.FindProperty( x => x.brushSettings.brushSize);
            brushRotation = editor.FindProperty(x => x.brushSettings.brushRotation);

            sizeGuide = editor.FindProperty(x => x.brushSettings.sizeGuide);
            normalGuide = editor.FindProperty(x => x.brushSettings.normalGuide);
            rotationGuide = editor.FindProperty(x => x.brushSettings.rotationGuide);

            alignToTerrain = editor.FindProperty(x => x.brushSettings.alignToTerrain);
            alignToTerrainSlerpRandom = editor.FindProperty(x => x.brushSettings.alignToTerrainSlerpRandom);
            alignToTerrainSlerpValue = editor.FindProperty(x => x.brushSettings.alignToTerrainSlerpValue);

            distribution = editor.FindProperty(x => x.brushSettings.distribution);

            poissonDiscSize = editor.FindProperty(x => x.brushSettings.poissonDiscSize);
            poissonDiscRaycastOffset = editor.FindProperty(x => x.brushSettings.poissonDiscRaycastOffset);
            poissonDiscsRandomized = editor.FindProperty(x => x.brushSettings.poissonDiscsRandomized);
            poissonDiscsVisible = editor.FindProperty(x => x.brushSettings.poissonDiscsVisible);
            fallOffCurve = editor.FindProperty(x => x.brushSettings.fallOffCurve);
            fallOff2dCurveX = editor.FindProperty(x => x.brushSettings.fallOff2dCurveX);
            fallOff2dCurveZ = editor.FindProperty(x => x.brushSettings.fallOff2dCurveZ);
            curveSamplePoints = editor.FindProperty(x => x.brushSettings.curveSamplePoints);
            allowOverlap = editor.FindProperty(x => x.brushSettings.allowOverlap);
            checkCollider = editor.FindProperty(x => x.brushSettings.checkCollider);
            layerMask = editor.FindProperty(x => x.brushSettings.layerMask);
            slopeEnabled = editor.FindProperty(x => x.brushSettings.slopeEnabled);

            spawnTarget = editor.FindProperty(x => x.brushSettings.spawnTarget);

            // initialize integrated applications
            vegetationStudioProIntegration = new VegetationStudioProIntegration( editor);
            unityTerrainTreesIntegration = new UnityTerrainTreesIntegration(editor);

        }

        public void OnInspectorGUI()
        {
            GUILayout.BeginVertical("box");

            EditorGUILayout.LabelField("Brush settings", GUIStyles.BoxTitleStyle);

            EditorGUILayout.PropertyField(brushSize, new GUIContent("Brush Size"));
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.PropertyField(brushRotation, new GUIContent("Brush Rotation"));
                if (GUILayout.Button("Reset", EditorStyles.miniButton, GUILayout.ExpandWidth(false)))
                {
                    brushRotation.intValue = 0;
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.PrefixLabel("Brush Visual");

                GUILayout.Label("Size", GUILayout.ExpandWidth(false));
                EditorGUILayout.PropertyField(sizeGuide, GUIContent.none, GUILayout.Width(20));

                GUILayout.Label("Normal", GUILayout.ExpandWidth(false));
                EditorGUILayout.PropertyField(normalGuide, GUIContent.none, GUILayout.Width(20));

                GUILayout.Label("Rotation", GUILayout.ExpandWidth(false));
                EditorGUILayout.PropertyField(rotationGuide, GUIContent.none, GUILayout.Width(20));
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.PropertyField(alignToTerrain, new GUIContent("Align To Surface"));
            if(alignToTerrain.boolValue)
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(alignToTerrainSlerpRandom, new GUIContent("Slerp Random", "Slerp randomly [0..1] between Vector3.Up and the surface normal. 0 = Up, 1 = Normal"));

                if (!alignToTerrainSlerpRandom.boolValue)
                {
                    EditorGUILayout.PropertyField(alignToTerrainSlerpValue, new GUIContent("Slerp Value", "Slerp explicitly [0..1] between Vector3.Up and the surface normal. 0 = Up, 1 = Normal"));
                }

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.PropertyField(allowOverlap, new GUIContent("Allow Overlap", "Center Mode: Check against brush size.\nPoisson Mode: Check against Poisson Disc size"));
            EditorGUILayout.PropertyField(layerMask, new GUIContent("Layer Mask", "Layer mask for the brush raycast"));

            EditorGUILayout.PropertyField(distribution, new GUIContent("Distribution"));

            switch (editorTarget.brushSettings.distribution)
            {
                case BrushSettings.Distribution.Center:
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(checkCollider, new GUIContent("Check Collider", "Center Mode: Check the prefab scale magnitude * 2 radius if there are colliding objects. In that case skip the instantiation of the prefab"));
                    EditorGUI.indentLevel--;
                    break;
                case BrushSettings.Distribution.Fluent:
                    break;
                case BrushSettings.Distribution.Poisson_Any:
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(poissonDiscSize, new GUIContent("Poisson Disc Size"));
                    EditorGUILayout.PropertyField(poissonDiscRaycastOffset, new GUIContent("Raycast Offset", "If any collider (not only terrain) is used for the raycast, then this will used as offset from which the ray will be cast against the collider"));
                    EditorGUILayout.PropertyField(poissonDiscsRandomized, new GUIContent("Discs Randomized", "New distribution per click or always use the same distribution"));
                    EditorGUILayout.PropertyField(poissonDiscsVisible, new GUIContent("Discs Visible", "Show poisson discs"));
                    EditorGUI.indentLevel--;
                    break;
                case BrushSettings.Distribution.Poisson_Terrain:
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(poissonDiscSize, new GUIContent("Poisson Disc Size"));
                    EditorGUILayout.PropertyField(poissonDiscsRandomized, new GUIContent("Discs Randomized", "New distribution per click or always use the same distribution"));
                    EditorGUILayout.PropertyField(poissonDiscsVisible, new GUIContent("Discs Visible", "Show poisson discs"));
                    EditorGUI.indentLevel--;
                    break;
                /*
                case BrushSettings.Distribution.FallOff:
                    EditorGUILayout.PropertyField(curveSamplePoints, new GUIContent("Curve Sample Points"));
                    EditorGUILayout.PropertyField(fallOffCurve, new GUIContent("FallOff"));
                    break;
                case BrushSettings.Distribution.FallOff2d:
                    EditorGUILayout.PropertyField(curveSamplePoints, new GUIContent("Curve Sample Points"));
                    EditorGUILayout.PropertyField(fallOff2dCurveX, new GUIContent("FallOff X"));
                    EditorGUILayout.PropertyField(fallOff2dCurveZ, new GUIContent("FallOff Z"));
                    break;
                */
            }

            // TODO: how to create a minmaxslider with propertyfield?
            EditorGUILayout.PropertyField(slopeEnabled, new GUIContent("Slope"));

            if (slopeEnabled.boolValue)
            {

                EditorGUILayout.BeginHorizontal();

                EditorGUI.indentLevel++;
                EditorGUILayout.PrefixLabel("Range [Degrees]");
                EditorGUI.indentLevel--;

                EditorGUILayout.IntField((int)editorTarget.brushSettings.slopeMin, GUILayout.Width(32));
                EditorGUILayout.MinMaxSlider(ref editorTarget.brushSettings.slopeMin, ref editorTarget.brushSettings.slopeMax, editorTarget.brushSettings.slopeMinLimit, editorTarget.brushSettings.slopeMaxLimit, GUILayout.ExpandWidth(true));
                EditorGUILayout.IntField((int)editorTarget.brushSettings.slopeMax, GUILayout.Width(32));

                EditorGUILayout.EndHorizontal();

            }

            // consistency check
            float minDiscSize = PoissonDiscSampleProvider.MIN_DISC_SIZE;
            if( poissonDiscSize.floatValue < minDiscSize)
            {
                Debug.LogError("Poisson Disc Size is too small. Setting it to " + minDiscSize);
                poissonDiscSize.floatValue = minDiscSize;
            }

            GUILayout.EndVertical();

            GUILayout.BeginVertical("box");
            {
                EditorGUILayout.LabelField("Spawn", GUIStyles.BoxTitleStyle);

                EditorGUILayout.PropertyField(spawnTarget, new GUIContent("Target"));
            }
            GUILayout.EndVertical();

            // Integrations
            // show integration settings in case they are selected
            switch (editorTarget.brushSettings.spawnTarget)
            {
                case BrushSettings.SpawnTarget.PrefabContainer:
                    // nothing to do
                    break;

                case BrushSettings.SpawnTarget.TerrainTrees:
                    unityTerrainTreesIntegration.OnInspectorGUI();
                    break;

                /*
                case BrushSettings.SpawnTarget.TerrainDetails:
                    EditorGUILayout.HelpBox("Not implemented", MessageType.Error);
                    break;
                */
                case BrushSettings.SpawnTarget.VegetationStudioPro:
                    vegetationStudioProIntegration.OnInspectorGUI();
                    break;
            }
        }

        public void OnSceneGUI()
        {

            // paint prefabs on mouse drag. don't do anything if no mode is selected, otherwise e.g. movement in scene view wouldn't work with alt key pressed
            if ( brushComponent.DrawBrush(editorTarget.mode, editorTarget.brushSettings, out BrushMode brushMode, out RaycastHit raycastHit))
            {
                switch( brushMode)
                {
                    case BrushMode.ShiftDrag:

                        AddPrefabs(raycastHit);

                        needsPhysicsApplied = true;

                        // consume event
                        Event.current.Use();
                        break;

                    case BrushMode.ShiftCtrlDrag:

                        RemovePrefabs(raycastHit);

                        // consume event
                        Event.current.Use();
                        break;

                }
            }

            if (IsFluent())
            {
                if( !brushDistribution.HasPreviewPrefab())
                {
                    brushDistribution.CreatePreviewPrefab();
                }

                // alt + mousewheel = modify height of preview prefab in transform up direction
                if (Event.current.modifiers == EventModifiers.Alt)
                {
                    if (Event.current.isScrollWheel)
                    {
                        PrefabSettings previewPrefabSettings = brushDistribution.GetPreviewPrefabSettings();

                        // the delta value will be modified (reduced) in the offset calculation; it might be too much as it is
                        previewPrefabSettings.brushOffsetUp += Event.current.delta.y;

                        Event.current.Use();
                    }
                }

                brushDistribution.UpdatePreviewPrefab(raycastHit.point, raycastHit.normal);
            }

            // info for the scene gui; used to be dynamic and showing number of prefabs (currently is static until refactoring is done)
            string[] guiInfo;
            if ( IsFluent())
            {
                guiInfo = new string[] {
                "Add prefabs: shift + drag mouse"
                +"\nRemove prefabs: shift + ctrl + drag mouse" +
                "\nBrush size: ctrl + mousewheel, Brush rotation: ctrl + shift + mousewheel" +
                "\nPrefab Y position: alt + mousewheel"
                };

            }
            else
            {
                guiInfo = new string[] {
                "Add prefabs: shift + drag mouse"
                +"\nRemove prefabs: shift + ctrl + drag mouse" +
                "\nBrush size: ctrl + mousewheel, Brush rotation: ctrl + shift + mousewheel"
                };

            }

            brushComponent.Layout(guiInfo);

            // auto physics
            bool applyAutoPhysics = needsPhysicsApplied 
                && autoPhysicsCollection.Count > 0
                && editorTarget.spawnSettings.autoSimulationType != SpawnSettings.AutoSimulationType.None 
                && Event.current.type == EventType.MouseUp;
            if (applyAutoPhysics)
            {
                switch(editorTarget.spawnSettings.autoSimulationCollider)
                {
                    case SpawnSettings.AutoCollider.SpawnedOnly:
                        AutoPhysicsSimulation.ApplyPhysics(editorTarget.physicsSettings, autoPhysicsCollection, editorTarget.spawnSettings.autoSimulationType);
                        break;

                    case SpawnSettings.AutoCollider.Container:
                        AutoPhysicsSimulation.ApplyPhysics(editorTarget.physicsSettings, editorTarget.container, editorTarget.spawnSettings.autoSimulationType);
                        break;

                    default:
                        Debug.LogWarning("Unsupported auto simulation collider option: " + editorTarget.spawnSettings.autoSimulationCollider);
                        break;
                }

                autoPhysicsCollection.Clear();
            }

        }
        
        public bool IsFluent()
        {
            return editorTarget.brushSettings.distribution == BrushSettings.Distribution.Fluent;
        }

        #region Paint Prefabs

        private void AddPrefabs(RaycastHit hit)
        {
            if (!editor.IsEditorSettingsValid())
                return;

            switch (editorTarget.brushSettings.distribution)
            {
                case BrushSettings.Distribution.Center:
                    // create a prefab
                    brushDistribution.CreatePreviewPrefab();
                    // add prefab
                    brushDistribution.AddPrefabs_Center(hit.point, hit.normal);
                    break;

                case BrushSettings.Distribution.Fluent:
                    // use preview prefab
                    brushDistribution.AddPrefabs_Center(hit.point, hit.normal);
                    // create next preview prefab
                    brushDistribution.CreatePreviewPrefab();
                    break;
                case BrushSettings.Distribution.Poisson_Any:
                    brushDistribution.AddPrefabs_Poisson_Any(hit.point, hit.normal, editorTarget.brushSettings.poissonDiscsRandomized);
                    break;
                case BrushSettings.Distribution.Poisson_Terrain:
                    brushDistribution.AddPrefabs_Poisson_Terrain(hit.point, hit.normal, editorTarget.brushSettings.poissonDiscsRandomized);
                    break;
                /*
                case BrushSettings.Distribution.FallOff:
                    Debug.Log("Not implemented yet: " + editorTarget.brushSettings.distribution);
                    break;
                case BrushSettings.Distribution.FallOff2d:
                    Debug.Log("Not implemented yet: " + editorTarget.brushSettings.distribution);
                    break;
                */
            }

        }


        /// <summary>
        /// Remove prefabs
        /// </summary>
        private void RemovePrefabs( RaycastHit raycastHit)
        {

            if (!editor.IsEditorSettingsValid())
                return;

            switch (editorTarget.brushSettings.spawnTarget)
            {
                case BrushSettings.SpawnTarget.PrefabContainer:

                    // get children within brush
                    Transform[] containerChildren = PrefabUtils.GetContainerChildren(editorTarget.container, raycastHit, editorTarget.brushSettings.brushSize);

                    // remove gameobjects
                    foreach (Transform transform in containerChildren)
                    {
                        Undo.DestroyObjectImmediate(transform.gameObject);
                    }

                    break;

                case BrushSettings.SpawnTarget.TerrainTrees:

                    unityTerrainTreesIntegration.RemovePrefabs( raycastHit);

                    break;

                /*
                case BrushSettings.SpawnTarget.TerrainDetails:

                    Debug.LogError("Not implemented");

                    break;
                */
                case BrushSettings.SpawnTarget.VegetationStudioPro:

                    vegetationStudioProIntegration.RemovePrefabs(raycastHit, editorTarget.brushSettings.brushSize);

                    break;
            }



        }

        public void PersistPrefab(PrefabSettings prefabSettings, PrefabTransform prefabTransform)
        {
            switch (editorTarget.brushSettings.spawnTarget)
            {
                case BrushSettings.SpawnTarget.PrefabContainer:

                    // new prefab
                    GameObject instance = PrefabUtility.InstantiatePrefab(prefabSettings.prefab) as GameObject;

                    instance.transform.position = prefabTransform.position;
                    instance.transform.rotation = prefabTransform.rotation;
                    instance.transform.localScale = prefabTransform.scale;

                    // attach as child of container
                    instance.transform.parent = editorTarget.container.transform;

                    // check collision; we need the actual transformed instance for that
                    bool isColliding;
                    switch (editorTarget.brushSettings.checkCollider)
                    {
                        case BrushSettings.CheckCollider.None:
                            isColliding = false;
                            break;

                        case BrushSettings.CheckCollider.Container:
                            Transform[] containerChildren = PrefabUtils.GetContainerChildren(editorTarget.container);
                            isColliding = ColliderUtils.IsColliding(instance, containerChildren);
                            break;

                        case BrushSettings.CheckCollider.All:
                            isColliding = ColliderUtils.IsColliding(instance);
                            break;

                        default: throw new System.Exception("Unsupported enum " + editorTarget.brushSettings.checkCollider);
                    }

                    // abort positioning the prefab in case there is a collision
                    if (isColliding)
                    {
                        Object.DestroyImmediate(instance);
                        return;
                    }

                    Undo.RegisterCreatedObjectUndo(instance, "Instantiate Prefab");

                    if (editorTarget.spawnSettings.autoSimulationType != SpawnSettings.AutoSimulationType.None)
                    {
                        autoPhysicsCollection.Add(instance);
                    }
                    
                    break;

                case BrushSettings.SpawnTarget.TerrainTrees:

                    unityTerrainTreesIntegration.AddNewPrefab(prefabSettings, prefabTransform.position, prefabTransform.rotation, prefabTransform.scale);

                    break;

                /*
                case BrushSettings.SpawnTarget.TerrainDetails:

                    Debug.LogError("Not implemented");

                    break;
                */
                case BrushSettings.SpawnTarget.VegetationStudioPro:

                    vegetationStudioProIntegration.AddNewPrefab(prefabSettings, prefabTransform.position, prefabTransform.rotation, prefabTransform.scale);

                    break;
            }
        }

        #endregion Paint Prefabs

        public PrefabPainter GetPainter()
        {
            return editorTarget;
        }

        public void OnEnable()
        {
        }

        public void OnDisable()
        {
            brushDistribution.DestroyPreviewPrefab();
        }

        public void ModeChanged(PrefabPainter.Mode mode)
        {
            if(mode == PrefabPainter.Mode.Brush)
            {
                brushDistribution.CreatePreviewPrefab();
            }
            else
            {
                brushDistribution.DestroyPreviewPrefab();
            }
        }

        public void OnEnteredPlayMode()
        {
            // ensure no preview gameobject remains when we enter play mode
            brushDistribution.DestroyPreviewPrefab();
        }
    }

}
