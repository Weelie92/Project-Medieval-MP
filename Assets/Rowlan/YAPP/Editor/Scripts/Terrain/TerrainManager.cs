using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rowlan.Yapp
{
	/// <summary>
	/// Manager of terrains for multi tile terrain support.
	/// Find terrain at world position.
	/// Get the layer index at a world position.
	/// </summary>
	public class TerrainManager
	{
        #region Singleton

        private static TerrainManager _instance;
		public static TerrainManager Instance
        {
            get
            {
				if( _instance == null)
                {
					_instance = new TerrainManager();
                }

				return _instance;
            }
        }

		#endregion Singleton

		/// <summary>
		/// All terrains connected to the active terrain
		/// </summary>
		private Terrain[] terrainGroup;

		/// <summary>
		/// Map of terrain data, used for performance reasons
		/// </summary>
		private Dictionary<Terrain, TerrainRecord> map = new Dictionary<Terrain, TerrainRecord>();

		/// <summary>
		/// Container of terrain data, used for performance reasons
		/// </summary>
		class TerrainRecord
		{
			public TerrainData terrainData;
			public int alphamapWidth;
			public int alphamapHeight;
			public float[,,] splatmapData;
			public int numTextures;

			public TerrainRecord(Terrain terrain)
			{
				terrainData = terrain.terrainData;
				alphamapWidth = terrainData.alphamapWidth;
				alphamapHeight = terrainData.alphamapHeight;

				splatmapData = terrainData.GetAlphamaps(0, 0, alphamapWidth, alphamapHeight);
				numTextures = splatmapData.Length / (alphamapWidth * alphamapHeight);
			}
		}

		public TerrainManager()
		{
			UpdateTerrainGroup();
		}

		private void UpdateTerrainGroup()
		{
			// get all terrains in group
			terrainGroup = TerrainUtilities.GetTerrainsInGroup();

			map.Clear();

			// create record of all terrains
			foreach (Terrain terrain in terrainGroup)
			{
				TerrainRecord record = new TerrainRecord(terrain);

				map.Add(terrain, record);
			}
		}

		private bool IsTerrainGroupValid()
        {
			foreach (Terrain terrain in terrainGroup)
            {
				if (terrain == null)
					return false;
            }

			return true;

		}

		/// <summary>
		/// Get the terrain at the world position
		/// </summary>
		/// <param name="worldPosition"></param>
		/// <returns>The terrain at the world position or null if there is no terrain at that position</returns>
		public Terrain GetTerrain(Vector3 worldPosition)
        {
			if( !IsTerrainGroupValid())
            {
				UpdateTerrainGroup();
			}

			// find the terrian in the group of terrains
			foreach (Terrain terrain in terrainGroup)
			{
				Vector3 terrainWorldPosition = terrain.transform.position;
				
				// world position
				float x = ((worldPosition.x - terrainWorldPosition.x) / terrain.terrainData.size.x) * terrain.terrainData.alphamapWidth;
				float z = ((worldPosition.z - terrainWorldPosition.z) / terrain.terrainData.size.z) * terrain.terrainData.alphamapHeight;

				// if the world position is within the terrain's world position return the terrain
				if (x >= 0 && z >= 0 && x < terrain.terrainData.heightmapResolution && z < terrain.terrainData.heightmapResolution)
				{
					return terrain;
				}

			}

			// terrain not found at world position
			return null;
		}

		/// <summary>
		/// Find the target terrain at the world position in all terrains of the group and return it including the position of the world position on the splat map.
		/// </summary>
		/// <param name="worldPosition"></param>
		/// <param name="splatPosition"></param>
		/// <param name="targetTerrain"></param>
		/// <returns></returns>
		private bool ConvertToSplatMapCoordinate(Vector3 worldPosition, ref Vector3 splatPosition, ref Terrain targetTerrain)
		{
			foreach (Terrain terrain in terrainGroup)
			{
				bool found = ConvertToSplatMapCoordinate(terrain, worldPosition, ref splatPosition);

				if (found)
				{
					targetTerrain = terrain;
					return true;
				}
			}

			Debug.LogError("Position out of terrain bounds: " + worldPosition);

			return false;
		}

		/// <summary>
		/// Convert the world position to the splat map position on the terrain.
		/// </summary>
		/// <param name="terrain"></param>
		/// <param name="worldPosition"></param>
		/// <param name="splatPosition"></param>
		/// <param name="targetTerrain"></param>
		/// <returns></returns>
		private bool ConvertToSplatMapCoordinate(Terrain terrain, Vector3 worldPosition, ref Vector3 splatPosition)
		{
			Vector3 terrainWorldPosition = terrain.transform.position;

			splatPosition.x = ((worldPosition.x - terrainWorldPosition.x) / terrain.terrainData.size.x) * terrain.terrainData.alphamapWidth;
			splatPosition.z = ((worldPosition.z - terrainWorldPosition.z) / terrain.terrainData.size.z) * terrain.terrainData.alphamapHeight;

			if (splatPosition.x >= 0 && splatPosition.z >= 0 && splatPosition.x < terrain.terrainData.heightmapResolution && splatPosition.z < terrain.terrainData.heightmapResolution)
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// Find the terrain texture index at the given position.
		/// This considers all terrains in the group of the active terrain
		/// </summary>
		/// <param name="worldPosition"></param>
		/// <returns>The layer index or -1 if none found at the world position</returns>
		public int GetTerrainLayerIndex(Vector3 worldPosition)
		{
			Vector3 terrainCord = new Vector3();
			Terrain terrain = Terrain.activeTerrain;

			bool ok = ConvertToSplatMapCoordinate(worldPosition, ref terrainCord, ref terrain);
			if (!ok)
				return -1;

			int activeTerrainIndex = 0;
			float largestOpacity = 0f;

			TerrainRecord record = map[terrain];

			for (int i = 0; i < record.numTextures; i++)
			{
				if (largestOpacity < record.splatmapData[(int)terrainCord.z, (int)terrainCord.x, i])
				{
					activeTerrainIndex = i;
					largestOpacity = record.splatmapData[(int)terrainCord.z, (int)terrainCord.x, i];
				}
			}

			return activeTerrainIndex;
		}

	}
}
