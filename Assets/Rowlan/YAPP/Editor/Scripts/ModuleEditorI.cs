using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Rowlan.Yapp.PrefabPainter;

namespace Rowlan.Yapp
{
    /// <summary>
    /// Interface for the editor modules
    /// </summary>
    public interface ModuleEditorI
    {

        void OnInspectorGUI();

        void OnSceneGUI();

        void OnEnable();

        void OnDisable();

        void ModeChanged(Mode mode);

        void OnEnteredPlayMode();

    }
}
