using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Rowlan.Yapp
{
    /// <summary>
    /// Play mode state change:
    /// + stop physics simulation in case it's running and the user presses play
    /// </summary>
    [InitializeOnLoadAttribute]
    public static class PlayModeStateChangedHandler
    {
        static PlayModeStateChangedHandler()
        {
            EditorApplication.playModeStateChanged += ModeStateChanged;
        }

        private static void ModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingEditMode)
            {
                if (PhysicsSimulator.IsActive())
                {
                    Debug.Log("Physics simulation running while user entered play mode. Stopping simulation now!");

                    PhysicsSimulator.Stop();
                }
            }
        }

    }
}