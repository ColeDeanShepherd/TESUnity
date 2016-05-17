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

		public static Vector3 NifVectorToUnityVector(Vector3 NIFVector)
		{
			Utils.Swap(ref NIFVector.y, ref NIFVector.z);

			return NIFVector;
		}
		public static Vector3 NifPointToUnityPoint(Vector3 NIFPoint)
		{
			return NifVectorToUnityVector(NIFPoint) / meterInMWUnits;
		}
		public static Matrix4x4 NifRotationMatrixToUnityRotationMatrix(Matrix4x4 NIFRotationMatrix)
		{
			NIFRotationMatrix = NIFRotationMatrix.transpose;

			var yColumn = NIFRotationMatrix.GetColumn(1);
			var zColumn = NIFRotationMatrix.GetColumn(2);

			NIFRotationMatrix.SetColumn(1, zColumn);
			NIFRotationMatrix.SetColumn(2, yColumn);

			return NIFRotationMatrix;
		}
		public static Quaternion NifRotationMatrixToUnityQuaternion(Matrix4x4 NIFRotationMatrix)
		{
			return RotationMatrixToQuaternion(NifRotationMatrixToUnityRotationMatrix(NIFRotationMatrix));
		}
		public static Quaternion RotationMatrixToQuaternion(Matrix4x4 matrix)
		{
			var fourXSquaredMinus1 = matrix[0, 0] - matrix[1, 1] - matrix[2, 2];
			var fourYSquaredMinus1 = matrix[1, 1] - matrix[0, 0] - matrix[2, 2];
			var fourZSquaredMinus1 = matrix[2, 2] - matrix[0, 0] - matrix[1, 1];
			var fourWSquaredMinus1 = matrix[0, 0] + matrix[1, 1] + matrix[2, 2];

			int biggestIndex = 0;
			var fourBiggestSquaredMinus1 = fourWSquaredMinus1;

			if(fourXSquaredMinus1 > fourBiggestSquaredMinus1)
			{
				fourBiggestSquaredMinus1 = fourXSquaredMinus1;
				biggestIndex = 1;
			}

			if(fourYSquaredMinus1 > fourBiggestSquaredMinus1)
			{
				fourBiggestSquaredMinus1 = fourYSquaredMinus1;
				biggestIndex = 2;
			}

			if(fourZSquaredMinus1 > fourBiggestSquaredMinus1)
			{
				fourBiggestSquaredMinus1 = fourZSquaredMinus1;
				biggestIndex = 3;
			}

			var biggestVal = Mathf.Sqrt(fourBiggestSquaredMinus1 + 1) * 0.5f;
			var mult = 0.25f / biggestVal;

			Quaternion quat = new Quaternion();

			switch(biggestIndex)
			{
				case 0:
					quat.w = biggestVal;
					quat.x = (matrix[1, 2] - matrix[2, 1]) * mult;
					quat.y = (matrix[2, 0] - matrix[0, 2]) * mult;
					quat.z = (matrix[0, 1] - matrix[1, 0]) * mult;
					break;
				case 1:
					quat.w = (matrix[1, 2] - matrix[2, 1]) * mult;
					quat.x = biggestVal;
					quat.y = (matrix[0, 1] + matrix[1, 0]) * mult;
					quat.z = (matrix[2, 0] + matrix[0, 2]) * mult;
					break;
				case 2:
					quat.w = (matrix[2, 0] - matrix[0, 2]) * mult;
					quat.x = (matrix[0, 1] + matrix[1, 0]) * mult;
					quat.y = biggestVal;
					quat.z = (matrix[1, 2] + matrix[2, 1]) * mult;
					break;
				case 3:
					quat.w = (matrix[0, 1] - matrix[1, 0]) * mult;
					quat.x = (matrix[2, 0] + matrix[0, 2]) * mult;
					quat.y = (matrix[1, 2] + matrix[2, 1]) * mult;
					quat.z = biggestVal;
					break;
				default:
					Debug.Assert(false);
					break;
			}

			return quat;
		}
		public static Quaternion NifEulerAnglesToUnityQuaternion(Vector3 NifEulerAngles)
		{
			var eulerAngles = NifVectorToUnityVector(NifEulerAngles);

			var xRot = Quaternion.AngleAxis(Mathf.Rad2Deg * eulerAngles.x, Vector3.right);
			var yRot = Quaternion.AngleAxis(Mathf.Rad2Deg * eulerAngles.y, Vector3.up);
			var zRot = Quaternion.AngleAxis(Mathf.Rad2Deg * eulerAngles.z, Vector3.forward);

			return zRot * yRot * xRot;
		}
	}
}