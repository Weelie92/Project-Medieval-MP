using Rowlan.Yapp;
using UnityEditor;
using UnityEngine;
using static Rowlan.Yapp.BrushComponent;

namespace Rowlan.Yapp
{
    public class ChangeScaleInteraction : InteractionModuleI
    {
        SerializedProperty changeScaleStrength;

#pragma warning disable 0414
        PrefabPainterEditor editor;
        PrefabPainter editorTarget;
#pragma warning restore 0414

        UnityTerrainTreeManager terrainTreeManager;

        public ChangeScaleInteraction(PrefabPainterEditor editor)
        {
            this.editor = editor;
            this.editorTarget = editor.GetPainter();

            terrainTreeManager = new UnityTerrainTreeManager(editor);

            changeScaleStrength = editor.FindProperty(x => x.interactionSettings.changeScale.changeScaleStrength);
        }

        public void OnInspectorGUI()
        {
            // show the spawn target in bold
            EditorStyles.helpBox.richText = true;
            EditorGUILayout.HelpBox(new GUIContent("Using target <b>" + editor.GetPainter().brushSettings.spawnTarget + "</b>"));
            EditorStyles.helpBox.richText = false;

            EditorGUILayout.HelpBox(new GUIContent("Shift = Grow, Ctrl+Shift = Shrink"));

            GUILayout.BeginVertical("box");

            EditorGUILayout.LabelField("Change Scale", GUIStyles.BoxTitleStyle);

            EditorGUILayout.PropertyField(changeScaleStrength, new GUIContent("Strength", "Strength of the scale adjustment"));

            GUILayout.EndVertical();

        }

        public bool OnSceneGUI(BrushMode brushMode, RaycastHit raycastHit, out bool applyPhysics)
        {
            applyPhysics = false;

            switch (brushMode)
            {
                case BrushMode.ShiftPressed:

                    Grow(raycastHit);

                    applyPhysics = true;

                    // consume event, otherwise brush won't be drawn
                    if (Event.current.type != EventType.Layout && Event.current.type != EventType.Repaint)
                        Event.current.Use();

                    return true;

                case BrushMode.ShiftCtrlPressed:

                    Shrink(raycastHit);

                    applyPhysics = true;

                    // consume event, otherwise brush won't be drawn
                    if (Event.current.type != EventType.Layout && Event.current.type != EventType.Repaint)
                        Event.current.Use();

                    return true;

            }

            return false;
        }
        private void Grow(RaycastHit hit)
        {
            ChangeScale(hit, true);
        }

        private void Shrink(RaycastHit hit)
        {
            ChangeScale(hit, false);
        }

        private void ChangeScale(RaycastHit hit, bool grow)
        {
            switch (editor.GetPainter().brushSettings.spawnTarget)
            {
                case BrushSettings.SpawnTarget.PrefabContainer:

                    ChangeScalePrefabs(hit, grow);

                    break;

                case BrushSettings.SpawnTarget.TerrainTrees:

                    float brushSize = editorTarget.brushSettings.brushSize;
                    float adjustFactor = editorTarget.interactionSettings.changeScale.changeScaleStrength / 1000f;

                    terrainTreeManager.ChangeScale(hit.point, brushSize, grow, adjustFactor);
                    
                    break;

                /*
                case BrushSettings.SpawnTarget.TerrainDetails:
                    Debug.LogError("Not implemented");
                    break;
                */
                case BrushSettings.SpawnTarget.VegetationStudioPro:
                    Debug.LogError("Not implemented");
                    break;
            }
        }


        // TODO: check performance; currently invoked multiple times in the editor loop
        private void ChangeScalePrefabs(RaycastHit hit, bool grow)
        {
            // just some arbitrary value depending on the magnet strength which ranges from 0..100
            float adjustFactor = editorTarget.interactionSettings.changeScale.changeScaleStrength / 1000f;

            Transform[] containerChildren = PrefabUtils.GetContainerChildren(editorTarget.container, hit, editorTarget.brushSettings.brushSize);

            foreach (Transform transform in containerChildren)
            {
                Undo.RegisterCompleteObjectUndo(transform, "Change scale");

                transform.localScale += transform.localScale * adjustFactor * (grow ? 1 : -1);
            }
        }

    }
}
