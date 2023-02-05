using UnityEngine;
using UnityEditor;
using static Rowlan.Yapp.BrushComponent;

namespace Rowlan.Yapp
{
    public class InteractionModuleEditor : ModuleEditorI
    {
        #region Properties

        SerializedProperty interactionType;

        #endregion Properties

#pragma warning disable 0414
        PrefabPainterEditor editor;
        PrefabPainter editorTarget;
#pragma warning restore 0414

        BrushComponent brushComponent = new BrushComponent();

        /// <summary>
        /// Auto physics only on special condition:
        /// + prefabs were added
        /// + mouse got released
        /// </summary>
        private bool needsPhysicsApplied = false; // TODO property


        private InteractionModuleI antiGravityModule;
        private InteractionModuleI magnetModule;
        private InteractionModuleI changeScaleModule;
        private InteractionModuleI setScaleModule;

        public InteractionModuleEditor(PrefabPainterEditor editor)
        {
            this.editor = editor;
            this.editorTarget = editor.GetPainter();

            interactionType = editor.FindProperty(x => x.interactionSettings.interactionType);

            antiGravityModule = new AntiGravityInteraction(editor);
            magnetModule = new MagnetInteraction(editor);
            changeScaleModule = new ChangeScaleInteraction(editor);
            setScaleModule = new SetScaleInteraction(editor);
        }

        public void OnInspectorGUI()
        {

            GUILayout.BeginVertical("box");

            EditorGUILayout.LabelField("Interaction (Experimental)", GUIStyles.BoxTitleStyle);

            EditorGUILayout.HelpBox("Perform interactive operations on the container children\nThis is highly experimental and bound to change", MessageType.Info);

            GUILayout.EndVertical();


            GUILayout.BeginVertical("box");

            EditorGUILayout.LabelField("Interaction Type", GUIStyles.BoxTitleStyle);

            EditorGUILayout.PropertyField(interactionType, new GUIContent("Type", "Type of interaction"));

            GUILayout.EndVertical();

            switch (interactionType.enumValueIndex)
            {
                case (int)InteractionSettings.InteractionType.AntiGravity:
                    antiGravityModule.OnInspectorGUI();
                    break;

                case (int)InteractionSettings.InteractionType.Magnet:
                    magnetModule.OnInspectorGUI();
                    break;

                case (int)InteractionSettings.InteractionType.ChangeScale:
                    changeScaleModule.OnInspectorGUI();
                    break;

                case (int)InteractionSettings.InteractionType.SetScale:
                    setScaleModule.OnInspectorGUI();
                    break;

                default:
                    throw new System.Exception("Not implemented interaction index: " + interactionType.enumValueIndex);
            }
        }

        public void OnSceneGUI()
        {

            // paint prefabs on mouse drag. don't do anything if no mode is selected, otherwise e.g. movement in scene view wouldn't work with alt key pressed
            if (brushComponent.DrawBrush(editorTarget.mode, editorTarget.brushSettings, out BrushMode brushMode, out RaycastHit raycastHit))
            {

                bool applyPhysics = false;

                switch (interactionType.enumValueIndex)
                {
                    case (int)InteractionSettings.InteractionType.AntiGravity:
                        antiGravityModule.OnSceneGUI(brushMode, raycastHit, out applyPhysics);
                        break;

                    case (int)InteractionSettings.InteractionType.Magnet:
                        magnetModule.OnSceneGUI(brushMode, raycastHit, out applyPhysics);
                        break;

                    case (int)InteractionSettings.InteractionType.ChangeScale:
                        changeScaleModule.OnSceneGUI(brushMode, raycastHit, out applyPhysics);
                        break;

                    case (int)InteractionSettings.InteractionType.SetScale:
                        setScaleModule.OnSceneGUI(brushMode, raycastHit, out applyPhysics);
                        break;

                    default:
                        throw new System.Exception("Not implemented interaction index: " + interactionType.enumValueIndex);

                }

                if (applyPhysics)
                    needsPhysicsApplied = true;
            }

            // TODO: change text
            // info for the scene gui; used to be dynamic and showing number of prefabs (currently is static until refactoring is done)
            string[] guiInfo = new string[] { "Add prefabs: shift + drag mouse\nRemove prefabs: shift + ctrl + drag mouse\nBrush size: ctrl + mousewheel, Brush rotation: ctrl + shift + mousewheel" };
            brushComponent.Layout(guiInfo);

            // auto physics
            bool applyAutoPhysics = needsPhysicsApplied && editorTarget.spawnSettings.autoSimulationType != SpawnSettings.AutoSimulationType.None && Event.current.type == EventType.MouseUp;
            if (applyAutoPhysics)
            {
                AutoPhysicsSimulation.ApplyPhysics(editorTarget.physicsSettings, editorTarget.container, editorTarget.spawnSettings.autoSimulationType);
            }
        }

        public void OnEnable()
        {
        }

        public void OnDisable()
        {
        }

        public void ModeChanged(PrefabPainter.Mode mode)
        {
        }

        public void OnEnteredPlayMode()
        {
        }

    }
}
