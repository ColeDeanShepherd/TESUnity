using UnityEngine;

namespace TESUnity
{
	public static class Convert
	{
		public static float MW2UnityScale = 1.0f / 128;

		public static Vector3 NifVector3ToUnityVector3(Vector3 NIFVector3)
		{
			Utils.Swap(ref NIFVector3.y, ref NIFVector3.z);

			return NIFVector3;
		}
		public static Vector3 NifPointToUnityPoint(Vector3 NIFPoint)
		{
			return NifVector3ToUnityVector3(NIFPoint) * MW2UnityScale;
		}
		public static Quaternion NifMatrix4x4ToUnityQuaternion(Matrix4x4 NIFMatrix4x4)
		{
			var quat = RotationMatrixToQuaternion(NIFMatrix4x4);
			Utils.Swap(ref quat.y, ref quat.z);

			return Quaternion.Inverse(quat);
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
		public static Vector3 NifEulerAnglesToUnityEulerAngles(Vector3 NifEulerAngles)
		{
			return Mathf.Rad2Deg * NifVector3ToUnityVector3(NifEulerAngles);
		}
	}
}