using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using OccaSoftware.Altos.Runtime;

namespace OccaSoftware.Altos.Editor
{
    [CustomEditor(typeof(Runtime.SkyDefinition))]
    [CanEditMultipleObjects]
    public class SkyboxDefinitionEditor : UnityEditor.Editor
    {
        private static GUILayoutOption miniButtonWidth = GUILayout.Width(20f);

        SerializedProperty periodsOfDay;
        SerializedProperty initialTime;
        SerializedProperty dayNightCycleDuration;

        SkyDefinition skyDefinition;

        private void OnEnable()
        {
            skyDefinition = (SkyDefinition)serializedObject.targetObject;

            periodsOfDay = serializedObject.FindProperty("periodsOfDay");
            initialTime = serializedObject.FindProperty("initialTime");
            dayNightCycleDuration = serializedObject.FindProperty("dayNightCycleDuration");
        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            if (Event.current.type == EventType.MouseUp)
            {
                skyDefinition.SortPeriodsOfDay();
            }
            
            Draw();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawTimeSettings()
		{
            // Time Settings
            EditorGUILayout.LabelField("Time Settings", EditorStyles.boldLabel);

            
            EditorGUI.indentLevel++;
            GUIContent dayContent = new GUIContent("Day: " + skyDefinition.CurrentDay, "Represents the current in-game day. Always initializes as 0. Not settable.");
            EditorGUILayout.LabelField(dayContent, EditorStyles.boldLabel);

            System.TimeSpan timeActive = System.TimeSpan.FromHours(skyDefinition.CurrentTime);
            GUIContent timeContent = new GUIContent("Time: " + timeActive.ToString("hh':'mm':'ss"), "Represents the current in-game time for a particular day.");
            EditorGUILayout.LabelField(timeContent, EditorStyles.boldLabel);
            EditorGUI.indentLevel--;

            EditorGUILayout.PropertyField(initialTime);
            EditorGUILayout.PropertyField(dayNightCycleDuration, new GUIContent("Day-Night Cycle Duration (h)", "The duration of each full day-night cycle (in hours). Set to 0 to disable the automatic progression of time."));
            EditorGUILayout.Space();
        }

        private void DrawKeyframeSettings()
		{

            // Periods of Day Settings
            EditorGUILayout.LabelField(new GUIContent("Periods of Day Key Frames", "Periods of day are treated as keyframes. The sky will linearly interpolate between the current period's colorset and the next period's colorset."), EditorStyles.boldLabel);
            for (int i = 0; i < periodsOfDay.arraySize; i++)
            {
                EditorGUILayout.Space(5f);
                SerializedProperty periodOfDay = periodsOfDay.GetArrayElementAtIndex(i);

                SerializedProperty description_Prop = periodOfDay.FindPropertyRelative("description");
                SerializedProperty startTime_Prop = periodOfDay.FindPropertyRelative("startTime");
                SerializedProperty horizonColor_Prop = periodOfDay.FindPropertyRelative("horizonColor");
                SerializedProperty zenithColor_Prop = periodOfDay.FindPropertyRelative("zenithColor");
                SerializedProperty groundColor_Prop = periodOfDay.FindPropertyRelative("groundColor");

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(description_Prop);

                if (GUILayout.Button("-", EditorStyles.miniButtonRight, miniButtonWidth))
                {
                    periodsOfDay.DeleteArrayElementAtIndex(i);
                }
                else
                {
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.PropertyField(startTime_Prop, new GUIContent("Start Time", "Defines the start time for the values associated with this key frame. The previous key frame linearly interpolates to these values from the previous key frame."));
                    EditorGUILayout.PropertyField(zenithColor_Prop, new GUIContent("Sky Color"));
                    EditorGUILayout.PropertyField(horizonColor_Prop, new GUIContent("Equator Color"));
                    EditorGUILayout.PropertyField(groundColor_Prop, new GUIContent("Ground Color"));
                }
            }
            EditorGUILayout.Space();
            if (GUILayout.Button("+"))
            {
                periodsOfDay.arraySize += 1;
            }
            EditorGUILayout.Space();
        }


        private void Draw()
        {
            DrawTimeSettings();
            DrawKeyframeSettings();
        }
    }

}
