using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Rowlan.Yapp
{
    public class FilterExtension
    {
        #region Properties

        SerializedProperty layerFilterEnabled;
        SerializedProperty includeTextures;
        SerializedProperty excludesTextures;

        #endregion Properties

#pragma warning disable 0414
        PrefabPainterEditor editor;
#pragma warning restore 0414

        PrefabPainter editorTarget;

        public FilterExtension(PrefabPainterEditor editor)
        {
            this.editor = editor;
            this.editorTarget = editor.GetPainter();

            layerFilterEnabled = editor.FindProperty(x => x.filterSettings.layerFilterEnabled);
            includeTextures = editor.FindProperty(x => x.filterSettings.includes);
            excludesTextures = editor.FindProperty(x => x.filterSettings.excludes);
        }

        public void OnInspectorGUI()
        {
            GUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Filter", GUIStyles.BoxTitleStyle);

            EditorGUILayout.PropertyField( layerFilterEnabled, new GUIContent( "Terrain Layers"));

            if (layerFilterEnabled.boolValue)
            {
                TerrainLayerUtils.ShowTerrainLayersSelection("Layers", Terrain.activeTerrain, editorTarget.filterSettings.includes, editorTarget.filterSettings.excludes);
            }

            GUILayout.EndVertical();
        }

       
    }
}