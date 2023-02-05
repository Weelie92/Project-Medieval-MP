using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rowlan.Yapp
{
    public class ApplicationSettings
    {
        /// <summary>
        /// Use either the actual preview gameobject as preview or use it in combination with DrawMesh
        /// TODO: decide which way to go. If the latter, then we could drop the check for layer (because of collider etc)
        /// </summary>
        public static bool useInstanceAsPreview = true;
    }
}
