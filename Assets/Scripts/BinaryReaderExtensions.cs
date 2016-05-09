using System.IO;
using System.Text;
using UnityEngine;

public static class BinaryReaderExtensions
{
	public static bool ReadBool32(BinaryReader reader)
	{
		return reader.ReadUInt32() != 0;
	}

	public static byte[] ReadLength32PrefixedBytes(BinaryReader reader)
	{
		var length = reader.ReadUInt32();
		return reader.ReadBytes((int)length);
	}

	public static string ReadASCIIString(BinaryReader reader, int length)
	{
		Debug.Assert(length >= 0);

		return Encoding.ASCII.GetString(reader.ReadBytes(length));
	}
	public static string ReadPossiblyNullTerminatedASCIIString(BinaryReader reader, int lengthIncludingPossibleNullTerminator)
	{
		Debug.Assert(lengthIncludingPossibleNullTerminator > 0);

		var bytes = reader.ReadBytes(lengthIncludingPossibleNullTerminator);
		var charCount = (Utils.Last(bytes) != 0) ? bytes.Length : (bytes.Length - 1);

		return Encoding.ASCII.GetString(bytes, 0, charCount);
	}
	public static string ReadLength32PrefixedASCIIString(BinaryReader reader)
	{
		return Encoding.ASCII.GetString(ReadLength32PrefixedBytes(reader));
	}

	public static Vector2 ReadVector2(BinaryReader reader)
	{
		var x = reader.ReadSingle();
		var y = reader.ReadSingle();

		return new Vector2(x, y);
	}
	public static Vector3 ReadVector3(BinaryReader reader)
	{
		var x = reader.ReadSingle();
		var y = reader.ReadSingle();
		var z = reader.ReadSingle();

		return new Vector3(x, y, z);
	}
	public static Vector4 ReadVector4(BinaryReader reader)
	{
		var x = reader.ReadSingle();
		var y = reader.ReadSingle();
		var z = reader.ReadSingle();
		var w = reader.ReadSingle();

		return new Vector4(x, y, z, w);
	}

	/// <summary>
	/// Reads a column-major 3x3 matrix but returns a functionally equivalent 4x4 matrix.
	/// </summary>
	public static Matrix4x4 ReadColumnMajorMatrix3x3(BinaryReader reader)
	{
		var matrix = new Matrix4x4();

		for(int columnIndex = 0; columnIndex < 4; columnIndex++)
		{
			for(int rowIndex = 0; rowIndex < 4; rowIndex++)
			{
				// If we're in the 3x3 part of the matrix, read values from the BinaryReader. Otherwise, use the identity matrix.
				if(rowIndex <= 2 && columnIndex <= 2)
				{
					matrix[rowIndex, columnIndex] = reader.ReadSingle();
				}
				else
				{
					matrix[rowIndex, columnIndex] = (rowIndex == columnIndex) ? 1 : 0;
				}
			}
		}

		return matrix;
	}

	/// <summary>
	/// Reads a row-major 3x3 matrix but returns a functionally equivalent 4x4 matrix.
	/// </summary>
	public static Matrix4x4 ReadRowMajorMatrix3x3(BinaryReader reader)
	{
		var matrix = new Matrix4x4();

		for(int rowIndex = 0; rowIndex < 4; rowIndex++) 
		{
			for(int columnIndex = 0; columnIndex < 4; columnIndex++)
			{
				// If we're in the 3x3 part of the matrix, read values from the BinaryReader. Otherwise, use the identity matrix.
				if(rowIndex <= 2 && columnIndex <= 2)
				{
					matrix[rowIndex, columnIndex] = reader.ReadSingle();
				}
				else
				{
					matrix[rowIndex, columnIndex] = (rowIndex == columnIndex) ? 1 : 0;
				}
			}
		}

		return matrix;
	}

	public static Matrix4x4 ReadColumnMajorMatrix4x4(BinaryReader reader)
	{
		var matrix = new Matrix4x4();

		for(int columnIndex = 0; columnIndex < 4; columnIndex++)
		{
			for(int rowIndex = 0; rowIndex < 4; rowIndex++)
			{
				matrix[rowIndex, columnIndex] = reader.ReadSingle();
			}
		}

		return matrix;
	}
	public static Matrix4x4 ReadRowMajorMatrix4x4(BinaryReader reader)
	{
		var matrix = new Matrix4x4();

		for(int rowIndex = 0; rowIndex < 4; rowIndex++) 
		{
			for(int columnIndex = 0; columnIndex < 4; columnIndex++)
			{
				matrix[rowIndex, columnIndex] = reader.ReadSingle();
			}
		}

		return matrix;
	}

	public static Quaternion ReadQuaternionWFirst(BinaryReader reader)
	{
		float w = reader.ReadSingle();
		float x = reader.ReadSingle();
		float y = reader.ReadSingle();
		float z = reader.ReadSingle();

		return new Quaternion(x, y, z, w);
	}
	public static Quaternion ReadQuaternionWLast(BinaryReader reader)
	{
		float x = reader.ReadSingle();
		float y = reader.ReadSingle();
		float z = reader.ReadSingle();
		float w = reader.ReadSingle();

		return new Quaternion(x, y, z, w);
	}
}