using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace Rowlan.Yapp
{
    /// <summary>
    /// Prefab Painter allows you to paint prefabs in the scene
    /// </summary>
    [ExecuteInEditMode()]
    [CanEditMultipleObjects]
    [CustomEditor(typeof(PrefabPainter))]
    public class PrefabPainterEditor : BaseEditor<PrefabPainter>
    {

        #region Properties

        SerializedProperty container;
        SerializedProperty mode;

        #endregion Properties

        private PrefabPainter editorTarget;

        private PhysicsExtension physicsModule;
        private CopyPasteExtension copyPasteModule;
        private SelectionExtension selectionModule;
        private ToolsExtension toolsModule;
        private SpawnExtension spawnModule;
        private FilterExtension filterModule;

        private BrushModuleEditor brushModule;
        private SplineModuleEditor splineModule;
        private InteractionModuleEditor interactionModule;
        private ContainerModuleEditor containerModule;

        private PrefabModuleEditor prefabModule;

        private Color defaultColor;

        PrefabPainterEditor editor;

        // TODO handle prefab dragging only in prefab painter editor
        public List<PrefabSettings> newDraggedPrefabs = null;

        GUIContent[] modeButtons;

        public void OnEnable()
        {
            this.editor = this;

            container = FindProperty( x => x.container); 
            mode = FindProperty(x => x.mode);

            this.editorTarget = target as PrefabPainter;

            this.brushModule = new BrushModuleEditor(this);
            this.splineModule = new SplineModuleEditor(this);
            this.interactionModule = new InteractionModuleEditor(this);
            this.containerModule = new ContainerModuleEditor(this);
            this.prefabModule = new PrefabModuleEditor(this);
            this.physicsModule = new PhysicsExtension(this);
            this.copyPasteModule = new CopyPasteExtension(this);
            this.selectionModule = new SelectionExtension(this);
            this.toolsModule = new ToolsExtension(this);
            this.spawnModule = new SpawnExtension(this);
            this.filterModule = new FilterExtension(this);

            modeButtons = new GUIContent[]
            {
                // TODO: icons
                new GUIContent( "Brush", "Paint prefabs using a brush"),
                new GUIContent( "Spline", "Align prefabs along a spline"),
                new GUIContent( "Interaction", "Brush interaction on the container children"),
                new GUIContent( "Operations", "Operations on the container"),
            };

            // subscribe to play mode state changes
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

            // subscribe to scene gui changes
            SceneView.duringSceneGui -= OnSceneGUI;
            SceneView.duringSceneGui += OnSceneGUI;

            brushModule.OnEnable();
            splineModule.OnEnable();
            interactionModule.OnEnable();
            containerModule.OnEnable();
        }

        public void OnDisable()
		{
            brushModule.OnDisable();
            splineModule.OnDisable();
            interactionModule.OnDisable();
            containerModule.OnDisable();

            // unsubscribe from scene gui changes
            SceneView.duringSceneGui -= OnSceneGUI;

            // unsubscribe to play mode state changes
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;

        }

        public PrefabPainter GetPainter()
        {
            return this.editorTarget;
        }


        public override void OnInspectorGUI()
        {

            // Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
            editor.serializedObject.Update();

            newDraggedPrefabs = null;

            // draw default inspector elements
            DrawDefaultInspector();

            /// 
            /// Info & Help
            /// 
            GUILayout.BeginVertical(GUIStyles.HelpBoxStyle);
            {
                EditorGUILayout.BeginHorizontal();


                if (GUILayout.Button("Asset Store", EditorStyles.miniButton, GUILayout.Width(120)))
                {
                    Application.OpenURL("https://assetstore.unity.com/packages/tools/terrain/yapp-yet-another-prefab-painter-223381");
                }

                if (GUILayout.Button("Documentation", EditorStyles.miniButton))
                {
                    Application.OpenURL("https://bit.ly/yapp-doc");
                }

                if (GUILayout.Button("Forum", EditorStyles.miniButton, GUILayout.Width(120)))
                {
                    Application.OpenURL("https://forum.unity.com/threads/released-yapp-yet-another-prefab-painter.1290977");
                }

                EditorGUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical(GUIStyles.AppTitleBoxStyle);
            {
                EditorGUILayout.LabelField("YAPP - Yet Another Prefab Painter", GUIStyles.AppTitleBoxStyle, GUILayout.Height(30));

            }
            GUILayout.EndVertical();

            /// 
            /// General settings
            /// 


            GUILayout.BeginVertical("box");
            {

                EditorGUILayout.LabelField("General Settings", GUIStyles.BoxTitleStyle);

                EditorGUILayout.BeginHorizontal();

                // container
                EditorGUILayout.PrefixLabel("");

                if (this.editorTarget.container == null)
                {
                    editor.SetErrorBackgroundColor();
                }

                EditorGUILayout.PropertyField(container);

                editor.SetDefaultBackgroundColor();

                if (GUILayout.Button("New", EditorStyles.miniButton, GUILayout.Width(50)))
                {
                    GameObject newContainer = new GameObject();

                    string name = editorTarget.name + " Container" + " (" + (this.editorTarget.transform.childCount + 1) + ")";
                    newContainer.name = name;

                    // set parent; reset position & rotation
                    newContainer.transform.SetParent( this.editorTarget.transform, false);

                    // set as new value
                    container.objectReferenceValue = newContainer;

                }

                if (GUILayout.Button("Clear", EditorStyles.miniButton, GUILayout.Width(50)))
                {
                    if(container != null)
                    {
                        this.toolsModule.RemoveContainerChildren();
                    }
  
                }

                EditorGUILayout.EndHorizontal();

            }
            GUILayout.EndVertical();

            ///
            /// mode
            /// 

            GUILayout.BeginVertical("box");
            {

                EditorGUILayout.LabelField("Mode", GUIStyles.BoxTitleStyle);

                EditorGUILayout.BeginHorizontal();

                EditorGUI.BeginChangeCheck();
                {
                    mode.intValue = GUILayout.Toolbar(mode.intValue, modeButtons);
                } 
                if(EditorGUI.EndChangeCheck())
                {
                    brushModule.ModeChanged( (PrefabPainter.Mode) mode.intValue);
                }

                EditorGUILayout.EndHorizontal();

            }
            GUILayout.EndVertical();

            /// 
            /// Mode dependent
            /// 

            switch (this.editorTarget.mode)
            {
                case PrefabPainter.Mode.Brush:

                    brushModule.OnInspectorGUI();

                    // spawn
                    spawnModule.OnInspectorGUI();

                    // filter
                    filterModule.OnInspectorGUI();

                    /// Prefabs
                    this.prefabModule.OnInspectorGUI();

                    break;

                case PrefabPainter.Mode.Spline:

                    splineModule.OnInspectorGUI();

                    // spawn
                    if (editorTarget.splineSettings.spawnMechanism == SplineSettings.SpawnMechanism.Manual)
                    {
                        spawnModule.OnInspectorGUI();
                    }

                    /// Prefabs
                    this.prefabModule.OnInspectorGUI();

                    break;

                case PrefabPainter.Mode.Interaction:

                    interactionModule.OnInspectorGUI();

                    // spawn
                    spawnModule.OnInspectorGUI();

                    break;

                case PrefabPainter.Mode.Container:
                    containerModule.OnInspectorGUI();

                    /// Physics
                    this.physicsModule.OnInspectorGUI();

                    /// Copy/Paste
                    this.copyPasteModule.OnInspectorGUI();

                    // Selection
                    this.selectionModule.OnInspectorGUI();

                    // Tools
                    this.toolsModule.OnInspectorGUI();
                    break;
                    
            }



            // add new prefabs
            if(newDraggedPrefabs != null)
            {
                this.editorTarget.prefabSettingsList.AddRange(newDraggedPrefabs);
            }

            // Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
            editor.serializedObject.ApplyModifiedProperties();
        }
        public void SetErrorBackgroundColor()
        {
            GUI.backgroundColor = GUIStyles.ErrorBackgroundColor;
        }

        public void SetDefaultBackgroundColor()
        {
            GUI.backgroundColor = GUIStyles.DefaultBackgroundColor;
        }

        public void AddGUISeparator( float topSpace, float bottomSpace)
        {
            GUILayout.Space(topSpace);
            GUILayout.Box("", GUIStyles.SeparatorStyle);
            GUILayout.Space(bottomSpace);
        }

        private void OnSceneGUI( SceneView sceneView)
        {
            // perform method only when the mouse is really in the sceneview; the scene view would register other events as well
            var isMouseInSceneView = new Rect(0, 0, sceneView.position.width, sceneView.position.height).Contains(Event.current.mousePosition);
            if (!isMouseInSceneView)
                return;            

            this.editorTarget = target as PrefabPainter;

            if (this.editorTarget == null)
                return;

            switch (this.editorTarget.mode)
            {
                case PrefabPainter.Mode.Brush:
                    brushModule.OnSceneGUI();
                    break;

                case PrefabPainter.Mode.Spline:
                    splineModule.OnSceneGUI();
                    break;

                case PrefabPainter.Mode.Interaction:
                    interactionModule.OnSceneGUI();
                    break;

                case PrefabPainter.Mode.Container:
                    containerModule.OnSceneGUI();
                    break;
            }

        }

        private void OnPlayModeStateChanged(PlayModeStateChange stateChange)
        {
            if (stateChange == PlayModeStateChange.EnteredEditMode || stateChange == PlayModeStateChange.ExitingPlayMode)
            {
                // currently nothing to do
            }
            else if (stateChange == PlayModeStateChange.ExitingEditMode || stateChange == PlayModeStateChange.EnteredPlayMode)
            {
                brushModule.OnEnteredPlayMode();
            }
        }

        public static void ShowGuiInfo(string[] texts)
        {

            // sceneview dimensions; need to consider EditorGUIUtility.pixelsPerPoint for 4k display
            float windowWidth = Screen.width / EditorGUIUtility.pixelsPerPoint;
            float windowHeight = Screen.height / EditorGUIUtility.pixelsPerPoint;

            // the info panel dimensions
            float panelWidth = 500;
            float panelHeight = 100;

            float panelX = windowWidth * 0.5f - panelWidth * 0.5f;
            float panelY = windowHeight - panelHeight;

            Rect infoRect = new Rect(panelX, panelY, panelWidth, panelHeight);

            Color textColor = Color.white;
            Color backgroundColor = Color.red;

            var defaultColor = GUI.backgroundColor;
            GUI.backgroundColor = backgroundColor;

            GUIStyle labelStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                alignment = TextAnchor.MiddleCenter
            };
            labelStyle.normal.textColor = textColor;

            // add a little bit of space. some user report problems with cut-off text
            infoRect.y -= 12f;

            GUILayout.BeginArea(infoRect);
            {
                EditorGUILayout.BeginVertical();
                {
                    foreach (string text in texts)
                    {
                        GUILayout.Label(text, labelStyle);
                    }
                }
                EditorGUILayout.EndVertical();
            }
            GUILayout.EndArea();

            GUI.backgroundColor = defaultColor;
        }

        public bool IsEditorSettingsValid()
        {
            // container must be set
            if (this.editorTarget.container == null)
            {
                return false;
            }

            // check prefabs
            foreach (PrefabSettings prefabSettings in this.editorTarget.prefabSettingsList)
            {
                // prefab must be set
                if ( prefabSettings.prefab == null)
                {
                    return false;
                }


            }

            return true;
        }

        #region Common methods

        public Transform[] getContainerChildren()
        {
            if (editorTarget.container == null)
                return new Transform[0];

            Transform[] children = editorTarget.container.transform.Cast<Transform>().ToArray();

            return children;
        }

        public PrefabSettingsTemplate FindTemplate( string templateName)
        {
            PrefabSettingsTemplate template = editor.prefabModule.templateCollection.templates.Find(x => x.name == templateName);

            return template;
        }

        /// <summary>
        /// Remove all prefab settings
        /// </summary>
        public void ClearPrefabs()
        {
            editor.GetPainter().prefabSettingsList.Clear();
        }


        /// <summary>
        /// Add the provided prefabs as new prefab settings using the template specified by name.
        /// Clear is applied if requested.
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="prefabs"></param>
        /// <param name="clear"></param>
        public void AddPrefabs( string templateName, List<GameObject> prefabs, bool clear)
        {
            // find tree template
            PrefabSettingsTemplate template = editor.FindTemplate(templateName);

            if (!template)
            {
                Debug.LogError("Template not found: " + templateName);
                return;
            }

            // clear the prefab settings list if required
            if (clear)
            {
                editor.GetPainter().prefabSettingsList.Clear();
            }

            // add the prefabs using the given template
            foreach (GameObject prefab in prefabs)
            {
                PrefabSettings prefabSettings = new PrefabSettings();

                prefabSettings.prefab = prefab;

                prefabSettings.ApplyTemplate(template);

                editor.GetPainter().prefabSettingsList.Add(prefabSettings);
            }

            // Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
            editor.serializedObject.ApplyModifiedProperties();
        }

        #endregion Common methods



    }

}