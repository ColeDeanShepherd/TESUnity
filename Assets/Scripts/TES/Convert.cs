using UnityEngine;

namespace TESUnity
{
	public static class Convert
	{
		public const int yardInMWUnits = 64;
		public const float meterInYards = 1.09361f;
		public const float meterInMWUnits = meterInYards * yardInMWUnits;

		public const int exteriorCellSideLengthInMWUnits = 8192;
		public const float exteriorCellSideLengthInMeters = (float)exteriorCellSideLengthInMWUnits / meterInMWUnits;
        
        public static Quaternion RotationMatrixToQuaternion(Matrix4x4 matrix)
		{
			return Quaternion.LookRotation(matrix.GetColumn(2), matrix.GetColumn(1));
		}
	}
}