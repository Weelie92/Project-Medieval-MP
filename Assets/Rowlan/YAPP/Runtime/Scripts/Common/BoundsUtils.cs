using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rowlan.Yapp
{
    public class BoundsUtils
    {

        private static Bounds zeroBounds = new Bounds(Vector3.zero, Vector3.zero);

        /// <summary>
        /// Get the local and world bounds from a gameobject.
        /// Note: This doesn't work with LODs.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="localBounds"></param>
        /// <param name="worldBounds"></param>
        /// <returns></returns>
        public static bool GetBounds( Transform transform, out Bounds localBounds, out Bounds worldBounds)
        {
            MeshFilter meshFilter = transform.GetComponent<MeshFilter>();
            MeshRenderer meshRenderer = transform.GetComponent<MeshRenderer>();

            if (meshFilter && meshRenderer)
            {
                localBounds = meshFilter.sharedMesh.bounds;
                worldBounds = meshRenderer.bounds;

                return true;
            }
            else
            {

                SkinnedMeshRenderer skinnedMeshRenderer = transform.GetComponent<SkinnedMeshRenderer>();

                if (skinnedMeshRenderer)
                {
                    localBounds = skinnedMeshRenderer.sharedMesh.bounds;
                    worldBounds = skinnedMeshRenderer.bounds;

                    return true;
                }

            }

            localBounds = zeroBounds;
            worldBounds = zeroBounds;

            return false;
        }

        /// <summary>
        /// Get the bounds of a gameobject considering LOD.
        /// Thanks Lennart!
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        public static Bounds CalculateBounds(GameObject gameObject)
        {
            Bounds combinedbounds = new Bounds(gameObject.transform.position, Vector3.zero);
            Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();

            foreach (Renderer renderer in renderers)
            {
                if (renderer is SkinnedMeshRenderer)
                {
                    SkinnedMeshRenderer skinnedMeshRenderer = renderer as SkinnedMeshRenderer;

                    Mesh mesh = new Mesh();

                    skinnedMeshRenderer.BakeMesh(mesh);
                    Vector3[] vertices = mesh.vertices;

                    for (int i = 0; i <= vertices.Length - 1; i++)
                    {
                        vertices[i] = skinnedMeshRenderer.transform.TransformPoint(vertices[i]);
                    }
                    mesh.vertices = vertices;
                    mesh.RecalculateBounds();

                    Bounds meshBounds = mesh.bounds;
                    combinedbounds.Encapsulate(meshBounds);
                }
                else
                {
                    combinedbounds.Encapsulate(renderer.bounds);
                }
            }

            return combinedbounds;
        }
    }
}
