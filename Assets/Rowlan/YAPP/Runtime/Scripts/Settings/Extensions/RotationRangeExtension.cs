using System;

namespace Rowlan.Yapp
{
    public static class RotationRangeExtension
    {
        public static float GetMinimum(this PrefabSettings.RotationRange rotationRange)
        {
            switch(rotationRange)
            {
                case PrefabSettings.RotationRange.Base_360: return 0f;
                case PrefabSettings.RotationRange.Base_180: return -180f;
                default: throw new Exception("Unsupported enum " + rotationRange);
            };
        }
        public static float GetMaximum(this PrefabSettings.RotationRange rotationRange)
        {
            switch(rotationRange)
            {
                case PrefabSettings.RotationRange.Base_360: return 360f;
                case PrefabSettings.RotationRange.Base_180: return 180f;
                default: throw new Exception("Unsupported enum " + rotationRange);
            };
        }

        public static PrefabSettings.RotationRange GetNext(this PrefabSettings.RotationRange rotationRange)
        {
            switch(rotationRange)
            {
                case PrefabSettings.RotationRange.Base_360: return PrefabSettings.RotationRange.Base_180;
                case PrefabSettings.RotationRange.Base_180:  return PrefabSettings.RotationRange.Base_360;
                default: throw new Exception("Unsupported enum " + rotationRange);
            };
        }

        public static string GetDisplayName(this PrefabSettings.RotationRange rotationRange)
        {
            switch(rotationRange)
            {
                case PrefabSettings.RotationRange.Base_360: return "0..360";
                case PrefabSettings.RotationRange.Base_180: return "-180..180";
                default: throw new Exception("Unsupported enum " + rotationRange);
            };
        }
    }
}
