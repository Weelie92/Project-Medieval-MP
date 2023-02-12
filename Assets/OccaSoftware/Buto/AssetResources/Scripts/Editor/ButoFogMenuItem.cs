using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace OccaSoftware.Buto.Editor
{
    public class ButoFogMenuItem : EditorWindow
    {
        private static string objectName = "Buto Fog Volume";
        private static List<System.Type> types = new List<System.Type>()
        {
            typeof(Volume)
        };

        [MenuItem("GameObject/Volume/Buto Fog Volume")]
        public static void CreateButoFogVolume()
        {
            ButoEditorCommon.CreateChildAndSelect(objectName, types);
        }
    }
}
