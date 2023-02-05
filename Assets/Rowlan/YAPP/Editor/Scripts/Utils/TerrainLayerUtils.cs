using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEditor;
using UnityEngine;

namespace Rowlan.Yapp
{
    public class TerrainLayerUtils
    {
        private static int ThumbnailSize = 64;

        public static void ShowTerrainLayersSelection(string title, Terrain terrain, List<int> includeTextures, List<int> excludeTextures)
        {
            GUIContent terrainLayers = EditorGUIUtility.TrTextContent(title);

            if (!string.IsNullOrEmpty(title))
                GUILayout.Label(terrainLayers, EditorStyles.boldLabel);

            EditorGUILayout.HelpBox("Click = Include, Ctrl+Click = Exclude", MessageType.None);

            GUI.changed = false;

            if (terrain.terrainData.terrainLayers.Length > 0)
            {
                TerrainLayer[] layers = terrain.terrainData.terrainLayers;
                Texture2D[] layerIcons = new Texture2D[layers.Length];
                for (int i = 0; i < layerIcons.Length; ++i)
                {
                    layerIcons[i] = (layers[i] == null || layers[i].diffuseTexture == null) ? EditorGUIUtility.whiteTexture : AssetPreview.GetAssetPreview(layers[i].diffuseTexture) ?? layers[i].diffuseTexture;
                }

                ShowSelectionGrid(includeTextures, excludeTextures, layerIcons, ThumbnailSize);
            }
        }

        public static void ShowSelectionGrid(List<int> includeTextures, List<int> excludeTextures, Texture[] textures, int approxSize)
        {
            if (textures.Length == 0)
            {
                EditorGUILayout.HelpBox("No terrain layers found", MessageType.Error);
                return;
            }

            int columns = (int)(EditorGUIUtility.currentViewWidth - approxSize) / approxSize;
            int rows = (int)Mathf.Ceil((textures.Length + columns - 2) / columns);

            int textureIndex = -1;
            for (int row = 0; row < rows; row++)
            {
                EditorGUILayout.BeginHorizontal();
                {
                    for (int col = 0; col < columns; col++)
                    {
                        textureIndex++;

                        if (textureIndex < textures.Length)
                        {

                            bool isRemove = Event.current.control;
                            bool isAdd = !isRemove;

                            bool isInclude = includeTextures.Contains(textureIndex);
                            bool isExclude = excludeTextures.Contains(textureIndex);

                            Texture previewTexture = textures[textureIndex];

                            GUIStyle style = GUIStyles.TextureSelectionStyleUnselected;

                            if (isInclude)
                                style = GUIStyles.TextureSelectionStyleInclude;
                            else if (isExclude)
                                style = GUIStyles.TextureSelectionStyleExclude;
                            else
                                style = GUIStyles.TextureSelectionStyleUnselected;

                            if (GUILayout.Button(previewTexture, style, GUILayout.Width(approxSize), GUILayout.Height(approxSize)))
                            {

                                if (isInclude || isExclude)
                                {
                                    includeTextures.Remove(textureIndex);
                                    excludeTextures.Remove(textureIndex);
                                }
                                else
                                if (isAdd)
                                {
                                    if (!isInclude && !includeTextures.Contains(textureIndex))
                                        includeTextures.Add(textureIndex);
                                }
                                else
                                if (isRemove)
                                {
                                    if (!isExclude && !excludeTextures.Contains(textureIndex))
                                        excludeTextures.Add(textureIndex);
                                }

                            }

                            // unselected: make them appear grey-ish
                            if (!isInclude && !isExclude)
                            {
                                Color overlay = Color.white;
                                overlay.a = 0.5f;

                                Rect lastRect = GUILayoutUtility.GetLastRect();
                                EditorGUI.DrawRect(lastRect, overlay);
                            }
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        public static int FindTerrainLayerIndex(Terrain terrain, TerrainLayer inputLayer)
        {
            for (int i = 0; i < terrain.terrainData.terrainLayers.Length; i++)
            {
                if (terrain.terrainData.terrainLayers[i] == inputLayer)
                    return i;
            }
            return -1;
        }

        public static TerrainLayer FindTerrainLayer(Terrain terrain, int layerIndex)
        {
            if (layerIndex < 0 || layerIndex >= terrain.terrainData.terrainLayers.Length)
                return null;

            return terrain.terrainData.terrainLayers[layerIndex];
        }

    }

}
