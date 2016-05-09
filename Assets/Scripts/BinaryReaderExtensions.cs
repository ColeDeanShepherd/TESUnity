using System;
using System.IO;
using UnityEngine;

public static class BinaryReaderExtensions
{
	public static bool ReadBool32(BinaryReader reader)
	{
		return reader.ReadUInt32() != 0;
	}
	public static string ReadASCIIString(BinaryReader reader, int length)
	{
		return System.Text.Encoding.ASCII.GetString(reader.ReadBytes(length));
	}
	public static string ReadPossiblyNullTerminatedASCIIString(BinaryReader reader, int lengthIncludingPossibleNullTerminator)
	{
		var bytes = reader.ReadBytes(lengthIncludingPossibleNullTerminator);
		var charCount = (Utils.Last(bytes) != 0) ? bytes.Length : (bytes.Length - 1);

		return System.Text.Encoding.ASCII.GetString(bytes, 0, charCount);
	}
	public static byte[] ReadLengthPrefixedBytes32(BinaryReader reader)
	{
		var length = reader.ReadUInt32();
		return reader.ReadBytes((int)length);
	}
	public static string ReadLengthPrefixedASCIIString32(BinaryReader reader)
	{
		return System.Text.Encoding.ASCII.GetString(ReadLengthPrefixedBytes32(reader));
	}
	public static Vector3 ReadVector3(BinaryReader reader)
	{
		float x = reader.ReadSingle();
		float y = reader.ReadSingle();
		float z = reader.ReadSingle();

		return new Vector3(x, y, z);
	}
	public static Vector4 ReadVector4(BinaryReader reader)
	{
		float x = reader.ReadSingle();
		float y = reader.ReadSingle();
		float z = reader.ReadSingle();
		float w = reader.ReadSingle();

		return new Vector4(x, y, z, w);
	}
	public static Matrix4x4 ReadMatrix3x3(BinaryReader reader)
	{
		var mat = new Matrix4x4();

		for(int j = 0; j < 4; j++)
		{
			for(int i = 0; i < 4; i++)
			{
				if(i < 3 && j < 3)
				{
					mat[i, j] = reader.ReadSingle();
				}
				else
				{
					mat[i, j] = (i == j) ? 1 : 0;
				}
			}
		}

		return mat;
	}
	public static Quaternion ReadQuaternionWFirst(BinaryReader reader)
	{
		float w = reader.ReadSingle();
		float x = reader.ReadSingle();
		float y = reader.ReadSingle();
		float z = reader.ReadSingle();

		return new Quaternion(x, y, z, w);
	}
	public static T Read<T>(BinaryReader reader)
	{
		if(typeof(T) == typeof(float))
		{
			return (T)((object)reader.ReadSingle());
		}
		else
		{
			throw new NotImplementedException("Tried to read an unsupported type.");
		}
	}
}