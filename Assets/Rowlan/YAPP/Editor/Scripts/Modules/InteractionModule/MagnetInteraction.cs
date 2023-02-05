using Rowlan.Yapp;
using UnityEditor;
using UnityEngine;
using static Rowlan.Yapp.BrushComponent;

namespace Rowlan.Yapp
{
    public class MagnetInteraction : InteractionModuleI
    {
        SerializedProperty strength;

#pragma warning disable 0414
        PrefabPainterEditor editor;
        PrefabPainter editorTarget;
#pragma warning restore 0414

        public MagnetInteraction(PrefabPainterEditor editor)
        {
            this.editor = editor;
            this.editorTarget = editor.GetPainter();

            strength = editor.FindProperty(x => x.interactionSettings.magnet.strength);
        }

        public void OnInspectorGUI()
        {
            GUILayout.BeginVertical("box");

            EditorGUILayout.LabelField("Magnet", GUIStyles.BoxTitleStyle);

            EditorGUILayout.PropertyField(strength, new GUIContent("Strength", "Strength of the Magnet"));

            GUILayout.EndVertical();
        }

        public bool OnSceneGUI(BrushMode brushMode, RaycastHit raycastHit, out bool applyPhysics)
        {
            applyPhysics = false;

            switch (brushMode)
            {
                case BrushMode.ShiftPressed:

                    Attract(raycastHit);

                    applyPhysics = true;

                    // consume event, otherwise brush won't be drawn
                    if (Event.current.type != EventType.Layout && Event.current.type != EventType.Repaint)
                        Event.current.Use();

                    return true;

                case BrushMode.ShiftCtrlPressed:

                    Repell(raycastHit);

                    applyPhysics = true;

                    // consume event, otherwise brush won't be drawn
                    if (Event.current.type != EventType.Layout && Event.current.type != EventType.Repaint)
                        Event.current.Use();

                    return true;

            }

            return false;
        }
        private void Attract(RaycastHit hit)
        {
            Magnet(hit, true);
        }

        private void Repell(RaycastHit hit)
        {
            Magnet(hit, false);
        }

        /// <summary>
        /// Attract/Repell the gameobjects of the container which are within the brush
        /// </summary>
        /// <param name="hit"></param>
        /// <param name="attract"></param>
        private void Magnet(RaycastHit hit, bool attract)
        {
            // just some arbitrary value depending on the magnet strength which ranges from 0..100
            float magnetFactor = editorTarget.interactionSettings.magnet.strength / 1000f;

            Transform[] containerChildren = PrefabUtils.GetContainerChildren(editorTarget.container, hit, editorTarget.brushSettings.brushSize);

            foreach (Transform transform in containerChildren)
            {
                Vector3 distance = hit.point - transform.position;
                Vector3 direction = distance.normalized;

                transform.position += direction * magnetFactor * (attract ? 1 : -1);
            }
        }
    }
}
