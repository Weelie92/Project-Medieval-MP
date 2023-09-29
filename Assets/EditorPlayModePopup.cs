using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[InitializeOnLoad]
#endif
public class EditorPlayModePopup : MonoBehaviour
{
    // Set your popup message here
    private const string PopupMessage = "Det kan hende du må stoppe og starte playmode igjen hvis du tester spillet i Unity Editor";

#if UNITY_EDITOR
    static EditorPlayModePopup()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            EditorUtility.DisplayDialog("Editor Play Mode", PopupMessage, "OK");
        }
    }
#endif
}