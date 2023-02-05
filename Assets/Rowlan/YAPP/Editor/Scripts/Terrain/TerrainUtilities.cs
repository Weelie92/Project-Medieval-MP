using System.Collections.Generic;
using UnityEngine;

namespace Rowlan.Yapp
{
	public static class TerrainUtilities
	{
		/// <summary>
		/// Get all terrains in the group of the active terrain
		/// </summary>
		/// <returns></returns>
		public static Terrain[] GetTerrainsInGroup()
        {
			return GetTerrainsInGroup(Terrain.activeTerrain.groupingID);				
        }

		/// <summary>
		/// Get all active terrains with the given group id
		/// </summary>
		/// <param name="groupingID"></param>
		/// <returns></returns>
		public static Terrain[] GetTerrainsInGroup(int groupingID)
		{
			List<Terrain> groupTerrains = new List<Terrain>();

			Terrain[] activeTerrains = Terrain.activeTerrains;

			for (int i = 0; i < activeTerrains.Length; ++i)
			{
				if (activeTerrains[i].groupingID == groupingID)
				{
					groupTerrains.Add(activeTerrains[i]);
				}
			}

			return groupTerrains.ToArray();
		}
	}
}
