using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace System.IO
{
	public class FileFormatException : FormatException
	{
		public FileFormatException() : base()
		{
		}
		public FileFormatException(string message) : base(message)
		{
		}
		public FileFormatException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}

// TODO: improve error handling
public struct ArrayRange<T> : IEnumerable<T>
{
	public struct Enumerator : IEnumerator<T>
	{
		public ArrayRange<T> arrayRange;
		public int currentIndex;

		public Enumerator(ArrayRange<T> arrayRange)
		{
			this.arrayRange = arrayRange;
			currentIndex = arrayRange.offset - 1;
		}

		public T Current
		{
			get
			{
				return arrayRange.array[currentIndex];
			}
		}
		object IEnumerator.Current
		{
			get
			{
				return Current;
			}
		}

		public void Dispose()
		{
			arrayRange = new ArrayRange<T>();
			currentIndex = -1;
		}
		public bool MoveNext()
		{
			currentIndex++;

			return currentIndex < (arrayRange.offset + arrayRange.count);
		}
		public void Reset()
		{
			currentIndex = arrayRange.offset - 1;
		}
	}

	public T[] array
	{
		get
		{
			return _array;
		}
	}
	public int offset
	{
		get
		{
			return _offset;
		}
	}
	public int count
	{
		get
		{
			return _count;
		}
	}

	public ArrayRange(T[] array, int offset, int count)
	{
		_array = array;
		_offset = offset;
		_count = count;
	}
	public IEnumerator<T> GetEnumerator()
	{
		return new Enumerator(this);
	}

	private T[] _array;
	private int _offset;
	private int _count;

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}

public static class StringUtils
{
	public static bool Equals(byte[] ASCIIBytes, string str)
	{
		if(ASCIIBytes.Length != str.Length)
		{
			return false;
		}

		for(int i = 0; i < ASCIIBytes.Length; i++)
		{
			if((int)ASCIIBytes[i] != (int)str[i])
			{
				return false;
			}
		}

		return true;
	}
}

public static class BinaryReaderUtils
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
}

public static class Utils
{
	public static bool ContainsBitFlags(uint x, params uint[] bitFlags)
	{
		uint encodedBitFlags = 0;

		foreach(var bitFlag in bitFlags)
		{
			encodedBitFlags |= bitFlag;
		}

		return (x & encodedBitFlags) == encodedBitFlags;
	}

	public static void Flip2DArrayVertically<T>(ref T[] arr, uint rowCount, uint columnCount)
	{
		var tmpRow = new T[columnCount];
		var lastRowIndex = rowCount - 1;

		for(uint rowIndex = 0; rowIndex < (rowCount / 2); rowIndex++)
		{
			uint otherRowIndex = lastRowIndex - rowIndex;

			uint rowStartIndex = rowIndex * columnCount;
			uint otherRowStartIndex = otherRowIndex * columnCount;

			Array.Copy(arr, otherRowStartIndex, tmpRow, 0, columnCount); // other -> tmp
			Array.Copy(arr, rowStartIndex, arr, otherRowStartIndex, columnCount); // row -> other
			Array.Copy(tmpRow, 0, arr, rowStartIndex, columnCount); // tmp -> row
		}
	}

	public static ulong GetBits(uint bitOffset, uint bitCount, byte[] bytes)
	{
		Debug.Assert(bitCount <= 64);

		ulong bits = 0;
		var remainingBitCount = bitCount;
		var byteIndex = bitOffset / 8;
		var bitIndex = bitOffset - (byteIndex * 8);

		while(remainingBitCount > 0)
		{
			var numBitsLeftInByte = 8 - bitIndex;
			var numBitsReadNow = Math.Min(remainingBitCount, numBitsLeftInByte);
			var unmaskedBits = (uint)bytes[byteIndex] >> (int)(8 - (bitIndex + numBitsReadNow));
			var bitMask = 0xFFu >> (int)(8 - numBitsReadNow);
			uint bitsReadNow = unmaskedBits & bitMask;

			bits <<= (int)numBitsReadNow;
			bits |= bitsReadNow;

			bitIndex += numBitsReadNow;

			if(bitIndex == 8)
			{
				byteIndex++;
				bitIndex = 0;
			}

			remainingBitCount -= numBitsReadNow;
		}

		return bits;
	}

	public static void Swap<T>(ref T a, ref T b)
	{
		var tmp = a;
		a = b;
		b = tmp;
	}

	public static T Last<T>(T[] array)
	{
		Debug.Assert(array.Length > 0);

		return array[array.Length - 1];
	}
	public static T Last<T>(List<T> list)
	{
		Debug.Assert(list.Count > 0);

		return list[list.Count - 1];
	}

	public static float ChangeRange(float x, float min0, float max0, float min1, float max1)
	{
		var range0 = max0 - min0;
		var range1 = max1 - min1;

		var xPct = (x - min0) / range0;

		return min1 + (xPct * range1);
	}

	public static void GetMinMax(float[,] array, out float min, out float max)
	{
		min = float.MaxValue;
		max = float.MinValue;

		foreach(var element in array)
		{
			min = Math.Min(min, element);
			max = Math.Max(max, element);
		}
	}
}