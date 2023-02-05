using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Rowlan.Yapp
{
    public class SpawnExtension
    {
        #region Properties

        SerializedProperty autoSimulationType;
        SerializedProperty autoSimulationHeightOffset;
        SerializedProperty autoSimulationCollider;

        #endregion Properties

#pragma warning disable 0414
        PrefabPainterEditor editor;
#pragma warning restore 0414

        PrefabPainter editorTarget;

        public SpawnExtension(PrefabPainterEditor editor)
        {
            this.editor = editor;
            this.editorTarget = editor.GetPainter();

            autoSimulationType = editor.FindProperty(x => x.spawnSettings.autoSimulationType);
            autoSimulationHeightOffset = editor.FindProperty(x => x.spawnSettings.autoSimulationHeightOffset);
            autoSimulationCollider = editor.FindProperty(x => x.spawnSettings.autoSimulationCollider);

        }

        public void OnInspectorGUI()
        {
            GUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Physics", GUIStyles.BoxTitleStyle);

            // auto physics
            EditorGUILayout.PropertyField(autoSimulationType, new GUIContent("Simulation"));
            if (autoSimulationType.enumValueIndex != (int)SpawnSettings.AutoSimulationType.None)
            {
                EditorGUILayout.HelpBox("Please backup your scene before using Editor Physics!", MessageType.Warning);

                EditorGUI.indentLevel++;
                {
                    EditorGUILayout.PropertyField(autoSimulationHeightOffset, new GUIContent("Height Offset"));

                    EditorGUILayout.PropertyField(autoSimulationCollider, new GUIContent("Auto Collider"));

                    // visualize collision detection mode
                    bool prevGUIEnabled = GUI.enabled;
                    GUI.enabled = false;
                    EditorGUILayout.TextField(new GUIContent("Collision Detection", "Detection can be changed in the global Physics Settings in Operations"), editorTarget.physicsSettings.collisionDetectionMode.ToString());
                    GUI.enabled = prevGUIEnabled;


                }
                EditorGUI.indentLevel--;
            }

            GUILayout.EndVertical();
        }

       
    }
}