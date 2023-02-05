using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Rowlan.Yapp
{
    public class PrefabUtils
    {
        /// <summary>
        /// Get the transforms of the given container
        /// </summary>
        /// <param name="container"></param>
        /// <returns></returns>
        public static Transform[] GetContainerChildren( GameObject container)
        {
            if (container == null)
                return new Transform[0];

            Transform[] children = container.transform.Cast<Transform>().ToArray();

            return children;
        }

        /// <summary>
        /// Get the transforms which are within the brush at the position
        /// </summary>
        /// <param name="container">The container with the children to check</param>
        /// <param name="hit">The position</param>
        /// <param name="brushSize">The brush size</param>
        /// <returns></returns>
        public static Transform[] GetContainerChildren(GameObject container, RaycastHit hit, float brushSize)
        {
            if (container == null)
                return new Transform[0];

            float brushRadius = brushSize / 2f;

            Transform[] children = container.transform.Cast<Transform>()
                .Select( x => x)
                //.Where(x => (hit.point - x.position).magnitude <= brushRadius)
                .Where( x => Vector3.Distance(hit.point, x.position) <= brushRadius)
                .ToArray();

            return children;

            // the long version; leaving it here, might be faster because of less overhead
            /*
            List<Transform> list = new List<Transform>();
            Transform[] containerChildren = GetContainerChildren(container);

            foreach (Transform transform in containerChildren)
            {
                Vector3 distance = hit.point - transform.position;

                // only those within the brush
                if (distance.magnitude > brushSize / 2f)
                    continue;

                list.Add(transform);
            }

            return list;
            */

        }
    }
}