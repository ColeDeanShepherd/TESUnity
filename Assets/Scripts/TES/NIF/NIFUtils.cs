using System;
using System.Collections.Generic;
using UnityEngine;

namespace TESUnity
{
    public static class NIFUtils
    {
        public static Vector3 NifVectorToUnityVector(Vector3 NIFVector)
        {
            Utils.Swap(ref NIFVector.y, ref NIFVector.z);

            return NIFVector;
        }
        public static Vector3 NifPointToUnityPoint(Vector3 NIFPoint)
        {
            return NifVectorToUnityVector(NIFPoint) / Convert.meterInMWUnits;
        }
        public static Matrix4x4 NifRotationMatrixToUnityRotationMatrix(Matrix4x4 NIFRotationMatrix)
        {
            Matrix4x4 matrix = new Matrix4x4();
            matrix.m00 = NIFRotationMatrix.m00;
            matrix.m01 = NIFRotationMatrix.m02;
            matrix.m02 = NIFRotationMatrix.m01;
            matrix.m03 = 0;
            matrix.m10 = NIFRotationMatrix.m20;
            matrix.m11 = NIFRotationMatrix.m22;
            matrix.m12 = NIFRotationMatrix.m21;
            matrix.m13 = 0;
            matrix.m20 = NIFRotationMatrix.m10;
            matrix.m21 = NIFRotationMatrix.m12;
            matrix.m22 = NIFRotationMatrix.m11;
            matrix.m23 = 0;
            matrix.m30 = 0;
            matrix.m31 = 0;
            matrix.m32 = 0;
            matrix.m33 = 1;

            return matrix;
        }
        public static Quaternion NifRotationMatrixToUnityQuaternion(Matrix4x4 NIFRotationMatrix)
        {
            return Convert.RotationMatrixToQuaternion(NifRotationMatrixToUnityRotationMatrix(NIFRotationMatrix));
        }
        public static Quaternion NifEulerAnglesToUnityQuaternion(Vector3 NifEulerAngles)
        {
            var eulerAngles = NifVectorToUnityVector(NifEulerAngles);

            var xRot = Quaternion.AngleAxis(Mathf.Rad2Deg * eulerAngles.x, Vector3.right);
            var yRot = Quaternion.AngleAxis(Mathf.Rad2Deg * eulerAngles.y, Vector3.up);
            var zRot = Quaternion.AngleAxis(Mathf.Rad2Deg * eulerAngles.z, Vector3.forward);

            return xRot * zRot * yRot;
        }
    }
}