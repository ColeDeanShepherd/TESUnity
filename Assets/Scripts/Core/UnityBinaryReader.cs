using System;
using System.IO;
using UnityEngine;

/// <summary>
/// An improved BinaryReader for Unity.
/// </summary>
/// <remarks>Not thread safe.</remarks>
public class UnityBinaryReader : IDisposable
{
	public Stream BaseStream { get { return reader.BaseStream; } }

	public UnityBinaryReader(Stream input)
	{
		reader = new BinaryReader(input, System.Text.Encoding.UTF8);
	}
	public UnityBinaryReader(Stream input, System.Text.Encoding encoding)
	{
		reader = new BinaryReader(input, encoding);
	}

	void IDisposable.Dispose()
	{
		Close();
	}
	~UnityBinaryReader()
	{
		Close();
	}
	public void Close()
	{
		if(reader != null)
		{
			reader.Close();
			reader = null;
		}
	}

	public byte ReadByte()
	{
		return reader.ReadByte();
	}
	public sbyte ReadSByte()
	{
		return reader.ReadSByte();
	}

	public void Read(byte[] buffer, int index, int count)
	{
		reader.Read(buffer, index, count);
	}
	public byte[] ReadBytes(int count)
	{
		return reader.ReadBytes(count);
	}

	public byte[] ReadRestOfBytes()
	{
		var remainingByteCount = reader.BaseStream.Length - reader.BaseStream.Position;

		Debug.Assert(remainingByteCount <= int.MaxValue);

		return reader.ReadBytes((int)remainingByteCount);
	}
	public void ReadRestOfBytes(byte[] buffer, int startIndex)
	{
		var remainingByteCount = reader.BaseStream.Length - reader.BaseStream.Position;

		Debug.Assert((startIndex >= 0) && (remainingByteCount <= int.MaxValue) && ((startIndex + remainingByteCount) <= buffer.Length));

		reader.Read(buffer, startIndex, (int)remainingByteCount);
	}

	public string ReadASCIIString(int length)
	{
		Debug.Assert(length >= 0);

		return System.Text.Encoding.ASCII.GetString(reader.ReadBytes(length));
	}
	public string ReadPossiblyNullTerminatedASCIIString(int lengthIncludingPossibleNullTerminator)
	{
		Debug.Assert(lengthIncludingPossibleNullTerminator > 0);

		var bytes = reader.ReadBytes(lengthIncludingPossibleNullTerminator);

        // Ignore the null terminator.
        var charCount = (Utils.Last(bytes) != 0) ? bytes.Length : (bytes.Length - 1);
        return System.Text.Encoding.Default.GetString(bytes, 0, charCount);
	}

	#region Little Endian
	public bool ReadLEBool32()
	{
		return ReadLEUInt32() != 0;
	}

	public ushort ReadLEUInt16()
	{
		reader.Read(readBuffer, 0, 2);

		return (ushort)((readBuffer[1] << 8) | readBuffer[0]);
	}
	public uint ReadLEUInt32()
	{
		reader.Read(readBuffer, 0, 4);

		return ((uint)readBuffer[3] << 24) | ((uint)readBuffer[2] << 16) | ((uint)readBuffer[1] << 8) | readBuffer[0];
	}
	public ulong ReadLEUInt64()
	{
		reader.Read(readBuffer, 0, 8);

		return ((ulong)readBuffer[7] << 56) | ((ulong)readBuffer[6] << 48) | ((ulong)readBuffer[5] << 40) | ((ulong)readBuffer[4] << 32) | ((ulong)readBuffer[3] << 24) | ((ulong)readBuffer[2] << 16) | ((ulong)readBuffer[1] << 8) | readBuffer[0];
	}

	public short ReadLEInt16()
	{
		var shortAsUShort = ReadLEUInt16();

		unsafe
		{
			return *((short*)(&shortAsUShort));
		}
	}
	public int ReadLEInt32()
	{
		var intAsUInt = ReadLEUInt32();

		unsafe
		{
			return *((int*)(&intAsUInt));
		}
	}
	public long ReadLEInt64()
	{
		var longAsULong = ReadLEUInt64();

		unsafe
		{
			return *((long*)(&longAsULong));
		}
	}

	public float ReadLESingle()
	{
		var singleAsUInt = ReadLEUInt32();

		unsafe
		{
			return *((float*)(&singleAsUInt));
		}
	}
	public double ReadLEDouble()
	{
		var doubleAsUInt = ReadLEUInt64();

		unsafe
		{
			return *((double*)(&doubleAsUInt));
		}
	}

	public byte[] ReadLELength32PrefixedBytes()
	{
		var length = ReadLEUInt32();
		return reader.ReadBytes((int)length);
	}
	public string ReadLELength32PrefixedASCIIString()
	{
		return System.Text.Encoding.ASCII.GetString(ReadLELength32PrefixedBytes());
	}

	public Vector2 ReadLEVector2()
	{
		var x = ReadLESingle();
		var y = ReadLESingle();

		return new Vector2(x, y);
	}
	public Vector3 ReadLEVector3()
	{
		var x = ReadLESingle();
		var y = ReadLESingle();
		var z = ReadLESingle();

		return new Vector3(x, y, z);
	}
	public Vector4 ReadLEVector4()
	{
		var x = ReadLESingle();
		var y = ReadLESingle();
		var z = ReadLESingle();
		var w = ReadLESingle();

		return new Vector4(x, y, z, w);
	}

	/// <summary>
	/// Reads a column-major 3x3 matrix but returns a functionally equivalent 4x4 matrix.
	/// </summary>
	public Matrix4x4 ReadLEColumnMajorMatrix3x3()
	{
		var matrix = new Matrix4x4();

		for(int columnIndex = 0; columnIndex < 4; columnIndex++)
		{
			for(int rowIndex = 0; rowIndex < 4; rowIndex++)
			{
				// If we're in the 3x3 part of the matrix, read values. Otherwise, use the identity matrix.
				if(rowIndex <= 2 && columnIndex <= 2)
				{
					matrix[rowIndex, columnIndex] = ReadLESingle();
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
	public Matrix4x4 ReadLERowMajorMatrix3x3()
	{
		var matrix = new Matrix4x4();

		for(int rowIndex = 0; rowIndex < 4; rowIndex++)
		{
			for(int columnIndex = 0; columnIndex < 4; columnIndex++)
			{
				// If we're in the 3x3 part of the matrix, read values. Otherwise, use the identity matrix.
				if(rowIndex <= 2 && columnIndex <= 2)
				{
					matrix[rowIndex, columnIndex] = ReadLESingle();
				}
				else
				{
					matrix[rowIndex, columnIndex] = (rowIndex == columnIndex) ? 1 : 0;
				}
			}
		}

		return matrix;
	}

	public Matrix4x4 ReadLEColumnMajorMatrix4x4()
	{
		var matrix = new Matrix4x4();

		for(int columnIndex = 0; columnIndex < 4; columnIndex++)
		{
			for(int rowIndex = 0; rowIndex < 4; rowIndex++)
			{
				matrix[rowIndex, columnIndex] = ReadLESingle();
			}
		}

		return matrix;
	}
	public Matrix4x4 ReadLERowMajorMatrix4x4()
	{
		var matrix = new Matrix4x4();

		for(int rowIndex = 0; rowIndex < 4; rowIndex++)
		{
			for(int columnIndex = 0; columnIndex < 4; columnIndex++)
			{
				matrix[rowIndex, columnIndex] = ReadLESingle();
			}
		}

		return matrix;
	}

	public Quaternion ReadLEQuaternionWFirst()
	{
		float w = ReadLESingle();
		float x = ReadLESingle();
		float y = ReadLESingle();
		float z = ReadLESingle();

		return new Quaternion(x, y, z, w);
	}
	public Quaternion ReadLEQuaternionWLast()
	{
		float x = ReadLESingle();
		float y = ReadLESingle();
		float z = ReadLESingle();
		float w = ReadLESingle();

		return new Quaternion(x, y, z, w);
	}
	#endregion

	private BinaryReader reader;

	// A buffer for read bytes the size of a decimal variable. Created to minimize allocations. 
	private byte[] readBuffer = new byte[16];
}