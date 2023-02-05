using Rowlan.Yapp;
using UnityEditor;
using UnityEngine;
using static Rowlan.Yapp.BrushComponent;

namespace Rowlan.Yapp
{
    public class AntiGravityInteraction : InteractionModuleI
    {
        SerializedProperty antiGravityStrength;

#pragma warning disable 0414
        PrefabPainterEditor editor;
        PrefabPainter editorTarget;
#pragma warning restore 0414

        public AntiGravityInteraction(PrefabPainterEditor editor)
        {
            this.editor = editor;
            this.editorTarget = editor.GetPainter();

            antiGravityStrength = editor.FindProperty(x => x.interactionSettings.antiGravity.strength);
        }

        public void OnInspectorGUI()
        {
            GUILayout.BeginVertical("box");

            EditorGUILayout.LabelField("Anti-Gravity", GUIStyles.BoxTitleStyle);

            EditorGUILayout.PropertyField(antiGravityStrength, new GUIContent("Strength", "Increments in Y-Position per editor step"));

            GUILayout.EndVertical();
        }

        public bool OnSceneGUI(BrushMode brushMode, RaycastHit raycastHit, out bool applyPhysics)
        {
            applyPhysics = false;

            switch (brushMode)
            {
                case BrushMode.ShiftPressed:

                    AntiGravity(raycastHit);

                    applyPhysics = true;

                    // consume event, otherwise brush won't be drawn
                    if (Event.current.type != EventType.Layout && Event.current.type != EventType.Repaint)
                        Event.current.Use();

                    return true;
            }

            return false;
        }



        /// <summary>
        /// Increment y-position in world space
        /// </summary>
        /// <param name="hit"></param>
        private void AntiGravity(RaycastHit hit)
        {
            // just some arbitrary value depending on the magnet strength which ranges from 0..100
            float antiGravityFactor = editorTarget.interactionSettings.antiGravity.strength / 1000f;

            Transform[] containerChildren = PrefabUtils.GetContainerChildren(editorTarget.container, hit, editorTarget.brushSettings.brushSize);

            foreach (Transform transform in containerChildren)
            {
                // https://docs.unity3d.com/ScriptReference/Transform-up.html
                // https://docs.unity3d.com/ScriptReference/Vector3-up.html
                transform.position += Vector3.up * antiGravityFactor;
            }
        }
    }
}
