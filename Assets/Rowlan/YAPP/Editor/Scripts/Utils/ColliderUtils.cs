using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rowlan.Yapp
{
    public class ColliderUtils
    {
        /// <summary>
        /// Check if the prefab is colliding with any other collider
        /// </summary>
        /// <param name="prefab"></param>
        /// <returns></returns>
        public static bool IsColliding(GameObject prefab)
        {
            return IsColliding(prefab, null);
        }

        /// <summary>
        /// Check if the prefab is colliding with any of the children colliders
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="children"></param>
        /// <returns></returns>
        public static bool IsColliding( GameObject prefab, Transform[] children)
        {
            Collider prefabCollider = prefab.GetComponent<Collider>();

            if (!prefabCollider)
            {
                // in case the collider is in the children
                prefabCollider = prefab.GetComponentInChildren<Collider>();
                if (!prefabCollider)
                {
                    Debug.Log("No collider for " + prefab.name);
                    return false;
                }
            }

            // using the bounds magnitude for the overlap sphere
            Bounds bounds = BoundsUtils.CalculateBounds(prefab);

            float overlapSphereRadius = bounds.size.magnitude;

            int maxNeighbours = 32;

            Collider[] neighbours = new Collider[maxNeighbours];

            int count = Physics.OverlapSphereNonAlloc(prefab.transform.position, overlapSphereRadius, neighbours);

            for (int i = 0; i < count; ++i)
            {
                Collider otherCollider = neighbours[i];

                // skip self
                if (otherCollider == prefabCollider)
                    continue; 

                Vector3 otherPosition = otherCollider.gameObject.transform.position;
                Quaternion otherRotation = otherCollider.gameObject.transform.rotation;

                Vector3 direction;
                float distance;

                bool overlapped = Physics.ComputePenetration( prefabCollider, prefab.transform.position, prefab.transform.rotation, otherCollider, otherPosition, otherRotation, out direction, out distance);

                if (overlapped)
                {
                    // check against all colliders, not just container children
                    if (children == null)
                        return true;

                    // check only against container children
                    foreach( Transform transform in children)
                    {
                        if (transform == otherCollider.transform)
                            return true;
                    }
                }
            }

            return false;
        }

    }
}
