using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rowlan.Yapp
{
    [System.Serializable]
    public class FilterSettings
    {
        /// <summary>
        /// Enable the layer filter
        /// </summary>
        public bool layerFilterEnabled = false;

        /// <summary>
        /// Prefabs will be included on these textures
        /// </summary>
        // Note: Unity can only serialize list, but not eg hashset, hence we use a list
        public List<int> includes = new List<int>();

        /// <summary>
        /// Prefabs will be excluded from these textures
        /// </summary>
        // Note: Unity can only serialize list, but not eg hashset, hence we use a list
        public List<int> excludes = new List<int>();

    }
}
