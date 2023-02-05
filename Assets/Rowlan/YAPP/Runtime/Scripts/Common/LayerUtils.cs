using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rowlan.Yapp
{
    /// <summary>
    /// Common utils for layer handling
    /// </summary>
    public class LayerUtils
    {
        /// <summary> 
        /// Specific layer values
        /// </summary>
        public enum LayerIndex
        {
            Nothing = 0,
            Everything = int.MaxValue,
            IgnoreRaycast = 2,
        }

        /// <summary>
        /// The layer index for the preview prefab
        /// </summary>
        public static LayerIndex PreviewLayerIndex = LayerIndex.IgnoreRaycast;

        /// <summary>
        /// Get layer mask without the preview layer index
        /// </summary>
        /// <param name="layerMask"></param>
        /// <returns></returns>
        public static LayerMask GetPreviewLayerMask( LayerMask layerMask)
        {
            if (ApplicationSettings.useInstanceAsPreview)
            {
                return layerMask & (int)~(1 << (int)PreviewLayerIndex);
            }
            else
            {
                return layerMask;
            }
        }

        /// <summary>
        /// Set the layer on parent and recursively on all children
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="layer"></param>
        public static void SetLayer( Transform parent, int layer)
        {
            parent.gameObject.layer = layer;

            for (int i = 0; i < parent.childCount; i++)
            {
                SetLayer(parent.GetChild(i), layer);
            }
        }

    }
}
