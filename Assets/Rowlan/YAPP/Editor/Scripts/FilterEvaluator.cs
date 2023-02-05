using UnityEngine;

namespace Rowlan.Yapp
{
	/// <summary>
	/// Evaluate filter criteria like if the brush position is on a required terrain layer.
	/// </summary>
	public class FilterEvaluator
    {

		/// <summary>
		/// Check if the filter is matched given the provided parameters
		/// </summary>
		/// <param name="worldPosition"></param>
		/// <param name="filterSettings"></param>
		/// <returns></returns>
		public bool IsInFilter( Vector3 worldPosition, FilterSettings filterSettings)
		{
			// terrain layer filter
			if (filterSettings.layerFilterEnabled)
			{
				// get the layer index at the world position
				int layerIndex = TerrainManager.Instance.GetTerrainLayerIndex(worldPosition);

				// check include filter
				if (filterSettings.includes.Count > 0 && filterSettings.includes.Contains(layerIndex))
					return true;

				// check exclude filter
				if (filterSettings.excludes.Count > 0 && !filterSettings.excludes.Contains(layerIndex))
					return true;

				// not in filter
				return false;
			}

			// default filter
			return true;
        }

	}
}
