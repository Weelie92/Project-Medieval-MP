using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class JUFPMenu : MonoBehaviour
{
    [MenuItem("JU Foot Placement/Help/Julhiecio Assetstore Page")]
    public static void OpenAssetStorePage()
    {
        Application.OpenURL("https://assetstore.unity.com/publishers/50337");
    }

    [MenuItem("JU Foot Placement/Help/Open Documentation", priority = 2)]
    public static void OpenDocumentation()
    {
        Application.OpenURL(Application.dataPath + "/JU Foot Placement/JU Foot Placement Documentation.pdf");
    }

    [MenuItem("JU Foot Placement/Help/Support Email", priority = 500)]
    public static void OpenSupportEmail()
    {
        Application.OpenURL("mailto:julhieciogames1@gmail.com");
    }
    
    [MenuItem("JU Foot Placement/Help/Tutorial Playlist", priority = 1)]
    public static void OpenTutorialPlaylist()
    {
        Application.OpenURL("https://www.youtube.com/playlist?list=PLznOHnSwmVcG1LrMqGfAsECDY2EVlvVcR");
    }

    static Animator anim;

    [MenuItem("GameObject/JU Foot Placement/Quick Rigidbody Character Setup", false, priority = 1)]
    [MenuItem("JU Foot Placement/Quick Rigidbody Character Setup")]
    public static void QuickCharacterSetup()
    {
        if (Selection.activeGameObject == null)
        {
            Debug.LogWarning("Quick Character Setup failed: No character selected, please select a humanoid character recognized by the Mecanim");
        }
        else
        {
            if (Selection.activeGameObject.GetComponent<Animator>() == null)
            {
                Debug.LogWarning("Quick Character Setup failed: The selected object does not contain an Animator component, please select a character with animator.");
            }
            else
            {
                anim = Selection.activeGameObject.GetComponent<Animator>();
                GameObject pl = anim.gameObject;
                if (anim.isHuman)
                {

                    //Change Character Layer
                    pl.layer = 2;
                    pl.tag = "Player";

                    //Animator Setup
                    anim.applyRootMotion = false;
                    string path = "Assets/JU Foot Placement/Animations/Animator Controller.controller";
                    //Debug.Log(path);

                    anim.runtimeAnimatorController = (RuntimeAnimatorController)AssetDatabase.LoadAssetAtPath(path, typeof(RuntimeAnimatorController));
                    //Rigid Body Setup
                    pl.AddComponent<Rigidbody>();
                    pl.GetComponent<Rigidbody>().mass = 70;
                    pl.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;

                    //Capsule Collider Setup
                    pl.AddComponent<CapsuleCollider>();
                    pl.GetComponent<CapsuleCollider>().center = new Vector3(0, 0.9f, 0);
                    pl.GetComponent<CapsuleCollider>().radius = 0.35f;
                    pl.GetComponent<CapsuleCollider>().height = 1.8f;

                    //Look At Script Setup
                    pl.AddComponent<HeadLookAtDirection>();

                    //Third Person Controller Setup
                    pl.AddComponent<JUFPThirdPersonController>();
                    pl.GetComponent<JUFPThirdPersonController>().GroundLayers = LayerMask.GetMask("Default");
                    pl.GetComponent<JUFPThirdPersonController>().JumpForce = 1;

                    //JU Foot Placement Setup
                    pl.AddComponent<JUFootPlacement>();
                    pl.GetComponent<JUFootPlacement>().GroundLayers = LayerMask.GetMask("Default");



                    Debug.Log("Quick Character Setup: Setup sucessful!");
                    QuickCharacterSetupSucessfulMessage.ShowWindow();
                }
                else
                {
                    Debug.LogError("Quick Character Setup failed: The selected game object does not contain a humanoid rig, please select a humanoid model recognized by Mecanim, otherwise it won't work.");
                }
            }
        }
    }
    [MenuItem("GameObject/JU Foot Placement/Quick Character Controller Setup", false, priority = 1)]
    [MenuItem("JU Foot Placement/Quick Character Controller Setup")]
    public static void QuickCharacterControllerSetup()
    {
        if (Selection.activeGameObject == null)
        {
            Debug.LogWarning("Quick Character Setup failed: No character selected, please select a humanoid character recognized by the Mecanim");
        }
        else
        {
            if (Selection.activeGameObject.GetComponent<Animator>() == null)
            {
                Debug.LogWarning("Quick Character Setup failed: The selected object does not contain an Animator component, please select a character with animator.");
            }
            else
            {
                anim = Selection.activeGameObject.GetComponent<Animator>();
                GameObject pl = anim.gameObject;
                if (anim.isHuman)
                {

                    //Change Character Layer
                    pl.layer = 2;
                    pl.tag = "Player";

                    //Animator Setup
                    anim.applyRootMotion = false;
                    string path = "Assets/JU Foot Placement/Animations/Animator Controller.controller";

                    anim.runtimeAnimatorController = (RuntimeAnimatorController)AssetDatabase.LoadAssetAtPath(path, typeof(RuntimeAnimatorController));
                    //JU Foot Placement Setup
                    pl.AddComponent<JUFootPlacement>();
                    //Capsule Collider Setup
                    pl.AddComponent<CharacterController>();
                    pl.GetComponent<CharacterController>().center = new Vector3(0, 0.9f, 0);
                    pl.GetComponent<CharacterController>().radius = 0.35f;
                    pl.GetComponent<CharacterController>().height = 1.8f;
                    pl.GetComponent<CharacterController>().stepOffset = pl.GetComponent<JUFootPlacement>().MaxStepHeight;

                    //Look At Script Setup
                    pl.AddComponent<HeadLookAtDirection>();

                    //Third Person Controller Setup
                    pl.AddComponent<JUFPThirdPersonController>();
                    pl.GetComponent<JUFPThirdPersonController>().GroundLayers = LayerMask.GetMask("Default");
                    pl.GetComponent<JUFPThirdPersonController>().JumpForce = 1;





                    Debug.Log("Quick Character Setup: Setup sucessful!");
                    QuickCharacterSetupSucessfulMessage.ShowWindow();
                }
                else
                {
                    Debug.LogError("Quick Character Setup failed: The selected game object does not contain a humanoid rig, please select a humanoid model recognized by Mecanim, otherwise it won't work.");
                }
            }
        }
    }

    [MenuItem("GameObject/JU Foot Placement/Add JU Foot Placement", false, priority = -200)]
    [MenuItem("JU Foot Placement/Add JU Foot Placement", priority = 0)]
    public static void AddJUFootPlacement()
    {
        if (Selection.activeGameObject == null)
        {
            Debug.LogWarning("failed: No character selected, please select a humanoid character recognized by the Mecanim");
        }
        else
        {
            if (Selection.activeGameObject.GetComponent<Animator>() == null)
            {
                Debug.LogWarning("failed: The selected object does not contain an Animator component, please select a character with animator or add animator to your character.");
            }
            else
            {
                anim = Selection.activeGameObject.GetComponent<Animator>();
                GameObject pl = anim.gameObject;
                if (anim.isHuman)
                {
                    //JU Foot Placement Setup
                    pl.AddComponent<JUFootPlacement>();
                }
                else
                {
                    Debug.LogError("failed: The selected game object does not contain a humanoid rig, please select a humanoid model recognized by Mecanim, otherwise it won't work.");
                }
            }
        }
    }


    public class QuickCharacterSetupSucessfulMessage : EditorWindow
    {
        public static void ShowWindow()
        {
            GetWindow(typeof(QuickCharacterSetupSucessfulMessage));
            GetWindow(typeof(QuickCharacterSetupSucessfulMessage)).titleContent.text = "Quick Character Setup";
            const int width = 272;
            const int height = 232;

            var x = (Screen.currentResolution.width - width) / 2;
            var y = (Screen.currentResolution.height - height) / 2;

            GetWindow<QuickCharacterSetupSucessfulMessage>().position = new Rect(x, y, width, height);
        }


        public Texture2D Banner;
        private void OnGUI()
        {
            if (Banner == null)
            {
                Banner = JUEditor.CustomEditorUtilities.GetImage("Assets/JU Foot Placement/Textures/Editor/JULOGO.png");
            }
            if (Banner != null)
            {
                JUEditor.CustomEditorUtilities.RenderImageWithResize(Banner, new Vector2(265, 70));
            }

            var style = new GUIStyle(EditorStyles.label);
            style.font = JUEditor.CustomEditorStyles.JUEditorFont();
            style.fontSize = 16;
            style.wordWrap = true;

            GUILayout.Label("Quick Character Setup Sucessful!", JUEditor.CustomEditorStyles.Header());
            GUILayout.Label("Warning: The character layer has been changed to 'Ignore Raycast', it will work well initially." +
                "\r\n But it is recommended to create your own layer for the Player and change it.", style);


            if (GUILayout.Button("OK", JUEditor.CustomEditorStyles.MiniToolbarButton()))
            {
                GetWindow<QuickCharacterSetupSucessfulMessage>().Close();
            }
        }
    }
}
