using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Rowlan.Yapp
{
    public class UnityTerrainTreesIntegration
    {

        // internal properties, maybe we'll make them public
        private bool randomTreeColor = false;
        private float treeColorAdjustment = 0.8f;

        SerializedProperty targetTerrain;

        PrefabPainterEditor editor;

        UnityTerrainTreeManager terrainTreeManager;

        public UnityTerrainTreesIntegration(PrefabPainterEditor editor)
        {
            this.editor = editor;

            terrainTreeManager = new UnityTerrainTreeManager(editor);

            targetTerrain = editor.FindProperty(x => x.brushSettings.targetTerrain);

        }

        public void OnInspectorGUI()
        {
            GUILayout.BeginVertical("box");
            {
                EditorGUILayout.LabelField("Terrain Trees", GUIStyles.BoxTitleStyle);

                if (this.editor.GetPainter().brushSettings.targetTerrain == null)
                {
                    editor.SetErrorBackgroundColor();
                }

                EditorGUILayout.PropertyField(targetTerrain, new GUIContent("Target Terrain", "The terrain to work with"));

                editor.SetDefaultBackgroundColor();


                EditorGUILayout.Space();

                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button(new GUIContent( "Extract Prefabs", "Replace the prefabs with the ones from the Unity terrain"), GUILayout.Width(100)))
                    {
                        CreatePrefabSettingsFromUnityTerrain();
                    }

                    if (GUILayout.Button(new GUIContent("Clear Prefabs", "Remove all prefab settings"), GUILayout.Width(100)))
                    {
                        ClearPrefabs();
                    }

                    if (GUILayout.Button(new GUIContent("Log Info", "Log terrain info to the console"), GUILayout.Width(100)))
                    {
                        LogInfo();
                    }

                    if (GUILayout.Button(new GUIContent( "Clear Terrain", "Remove all trees from the terrain"), GUILayout.Width(120)))
                    {
                        RemoveAll();
                    }

                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.HelpBox("Terrain Trees is highly experimental and not fully implemented yet! Backup your project!", MessageType.Warning);

            }
            GUILayout.EndVertical();


        }

        private void LogInfo()
        {
            terrainTreeManager.LogPrototypes();
        }

        private void RemoveAll()
        {
            terrainTreeManager.RemoveAllInstances();
        }

        public void AddNewPrefab(PrefabSettings prefabSettings, Vector3 newPosition, Quaternion newRotation, Vector3 newLocalScale)
        {
            // brush mode
            float brushSize = editor.GetPainter().brushSettings.brushSize;

            // poisson mode: use the discs as brush size
            if (editor.GetPainter().brushSettings.distribution == BrushSettings.Distribution.Poisson_Any || editor.GetPainter().brushSettings.distribution == BrushSettings.Distribution.Poisson_Terrain)
            {
                brushSize = editor.GetPainter().brushSettings.poissonDiscSize;
            }

            GameObject prefab = prefabSettings.prefab;
            bool allowOverlap = editor.GetPainter().brushSettings.allowOverlap;

            terrainTreeManager.PlacePrefab( prefab, newPosition, newLocalScale, newRotation, brushSize, randomTreeColor, treeColorAdjustment, allowOverlap);
        }

        public void RemovePrefabs( RaycastHit raycastHit)
        {
            Vector3 position = raycastHit.point;
            float brushSize = editor.GetPainter().brushSettings.brushSize;

            terrainTreeManager.RemoveOverlapping( position, brushSize);

        }

        /// <summary>
        /// Extract the prefabs of the unity terrain and create yapp settings from them.
        /// </summary>
        private void CreatePrefabSettingsFromUnityTerrain()
        {
            // get the prefabs
            List<GameObject> prefabs = terrainTreeManager.ExtractPrefabs();

            // create new settings list
            editor.AddPrefabs( Constants.TEMPLATE_NAME_TREE, prefabs, true);
        }

        private void ClearPrefabs()
        {
            // create new settings list
            editor.ClearPrefabs();
        }

    }
}