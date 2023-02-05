using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rowlan.Yapp
{
    public class MeshUtils
    {

        /// <summary>
        /// Render a game object using DrawMesh.
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="sourceLODLevel"></param>
        public static void RenderGameObject(GameObject gameObject, int sourceLODLevel)
        {
            GameObject[] roots = new GameObject[] { gameObject };

            LODGroup lodGroup = gameObject.GetComponent<LODGroup>();
            if (lodGroup && lodGroup.lodCount > 0)
            {
                // considering submeshes as well
                roots = new GameObject[lodGroup.GetLODs()[sourceLODLevel].renderers.Length];

                for( int i=0; i < roots.Length; i++)
                {
                    roots[i] = lodGroup.GetLODs()[sourceLODLevel].renderers[i].gameObject;
                }
            }

            foreach (GameObject root in roots)
            {
                MeshRenderer[] renderers = root.GetComponentsInChildren<MeshRenderer>();
                for (int i = 0; i < renderers.Length; i++)
                {
                    MeshFilter meshFilter = renderers[i].gameObject.GetComponent<MeshFilter>();
                    if (meshFilter)
                    {
                        Matrix4x4 matrix = Matrix4x4.TRS(
                            renderers[i].transform.position,
                            renderers[i].transform.rotation,
                            renderers[i].transform.lossyScale);

                        Mesh mesh = meshFilter.sharedMesh;

                        for (int j = 0; j < renderers[i].sharedMaterials.Length; j++)
                        {
                            Material material = renderers[i].sharedMaterials[j];
                            material.SetPass(0);

                            // TODO: use this in later Unity versions: https://docs.unity3d.com/2021.2/Documentation/ScriptReference/Graphics.RenderMesh.html
                            Graphics.DrawMesh(mesh, matrix, material, 0);
                        }
                    }
                }
            }
        }
    }
}
