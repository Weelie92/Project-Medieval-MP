using UnityEngine;

namespace Rowlan.Yapp
{
	/// <summary>
	/// Evaluate settings like if the placement position is in the required slope range.
	/// </summary>
	public class PlacementConditionsEvaluator
    {
		/// <summary>
		/// Check if the placement conditions are met given the provided parameters
		/// </summary>
		/// <param name="worldPosition"></param>
		/// <param name="brushSettings"></param>
		/// <returns></returns>
		public bool IsConditionsMet( Vector3 worldPosition, Vector3 normal, BrushSettings brushSettings)
		{
			#region Slope

			if (brushSettings.slopeEnabled)
			{
				// angle
				float slopeAngle = Vector3.Angle(normal.normalized, new Vector3(0, 1, 0));

				// check if placement position is inside the slope range
				if (slopeAngle < brushSettings.slopeMin || slopeAngle > brushSettings.slopeMax)
				{
					return false;
				}
			}

			#endregion Slope

			// default condition evaluates to true
			return true;
        }

	}
}
