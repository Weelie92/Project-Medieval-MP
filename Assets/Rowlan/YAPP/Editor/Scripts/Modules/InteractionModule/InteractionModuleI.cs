using UnityEngine;
using static Rowlan.Yapp.BrushComponent;

namespace Rowlan.Yapp
{
    /// <summary>
    /// Interface for the Integration modules
    /// </summary>
    public interface InteractionModuleI
    {
        void OnInspectorGUI();

        /// <summary>
        /// Apply OnSceneGUI.
        /// </summary>
        /// <param name="brushMode"></param>
        /// <param name="raycastHit"></param>
        /// <param name="needsPhysicsApplied"></param>
        /// <returns>true if something changed, false otherwise</returns>
        bool OnSceneGUI(BrushMode brushMode, RaycastHit raycastHit, out bool needsPhysicsApplied);

    }
}
