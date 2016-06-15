using System;
using System.Collections.Generic;
using UnityEngine;

namespace System.IO
{
	/// <summary>
	/// A reimplementation of a standard exception type that was introduced in .NET 3.0.
	/// </summary>
	public class FileFormatException : FormatException
	{
		public FileFormatException() : base() {}
		public FileFormatException(string message) : base(message) {}
		public FileFormatException(string message, Exception innerException) : base(message, innerException) {}
	}
}

public static class StringUtils
{
	/// <summary>
	/// Quickly checks if an ASCII encoded string is equal to a C# string.
	/// </summary>
	public static bool Equals(byte[] ASCIIBytes, string str)
	{
		if(ASCIIBytes.Length != str.Length)
		{
			return false;
		}

		for(int i = 0; i < ASCIIBytes.Length; i++)
		{
			if(ASCIIBytes[i] != str[i])
			{
				return false;
			}
		}

		return true;
	}
}

public static class Utils
{
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

	public static void Swap<T>(ref T a, ref T b)
	{
		var tmp = a;
		a = b;
		b = tmp;
	}

	/// <summary>
	/// Checks if a bit string (an unsigned integer) contains a collection of bit flags.
	/// </summary>
	public static bool ContainsBitFlags(uint bitString, params uint[] bitFlags)
	{
		// Construct a bit string containing all the bit flags.
		uint allBitFlags = 0;

		foreach(var bitFlag in bitFlags)
		{
			allBitFlags |= bitFlag;
		}

		// Check if the bit string contains all the bit flags.
		return (bitString & allBitFlags) == allBitFlags;
	}

	/// <summary>
	/// Extracts a range of bits from a byte array.
	/// </summary>
	/// <param name="bitOffset">An offset in bits from the most significant bit (byte 0, bit 0) of the byte array.</param>
	/// <param name="bitCount">The number of bits to extract. Cannot exceed 64.</param>
	/// <param name="bytes">A big-endian byte array.</param>
	/// <returns>A ulong containing the right-shifted extracted bits.</returns>
	public static ulong GetBits(uint bitOffset, uint bitCount, byte[] bytes)
	{
		Debug.Assert((bitCount <= 64) && ((bitOffset + bitCount) <= (8 * bytes.Length)));

		ulong bits = 0;
		var remainingBitCount = bitCount;
		var byteIndex = bitOffset / 8;
		var bitIndex = bitOffset - (8 * byteIndex);

		while(remainingBitCount > 0)
		{
			// Read bits from the byte array.
			var numBitsLeftInByte = 8 - bitIndex;
			var numBitsReadNow = Math.Min(remainingBitCount, numBitsLeftInByte);
			var unmaskedBits = (uint)bytes[byteIndex] >> (int)(8 - (bitIndex + numBitsReadNow));
			var bitMask = 0xFFu >> (int)(8 - numBitsReadNow);
			uint bitsReadNow = unmaskedBits & bitMask;

			// Store the bits we read.
			bits <<= (int)numBitsReadNow;
			bits |= bitsReadNow;

			// Prepare for the next iteration.
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

	/// <summary>
	/// Transforms x from an element of [min0, max0] to an element of [min1, max1].
	/// </summary>
	public static float ChangeRange(float x, float min0, float max0, float min1, float max1)
	{
		Debug.Assert((min0 <= max0) && (min1 <= max1) && (x >= min0) && (x <= max0));

		var range0 = max0 - min0;
		var range1 = max1 - min1;

		var xPct = (x - min0) / range0;

		return min1 + (xPct * range1);
	}

	/// <summary>
	/// Calculates the minimum and maximum values of an array.
	/// </summary>
	public static void GetExtrema(float[] array, out float min, out float max)
	{
		min = float.MaxValue;
		max = float.MinValue;

		foreach(var element in array)
		{
			min = Math.Min(min, element);
			max = Math.Max(max, element);
		}
	}

	/// <summary>
	/// Calculates the minimum and maximum values of a 2D array.
	/// </summary>
	public static void GetExtrema(float[,] array, out float min, out float max)
	{
		min = float.MaxValue;
		max = float.MinValue;

		foreach(var element in array)
		{
			min = Math.Min(min, element);
			max = Math.Max(max, element);
		}
	}

	/// <summary>
	/// Calculates the minimum and maximum values of a 3D array.
	/// </summary>
	public static void GetExtrema(float[,,] array, out float min, out float max)
	{
		min = float.MaxValue;
		max = float.MinValue;

		foreach(var element in array)
		{
			min = Math.Min(min, element);
			max = Math.Max(max, element);
		}
	}

	public static void Flip2DArrayVertically<T>(T[] arr, int rowCount, int columnCount)
	{
		Flip2DSubArrayVertically(arr, 0, rowCount, columnCount);
	}

	/// <summary>
	/// Flips a portion of a 2D array vertically.
	/// </summary>
	/// <param name="arr">A 2D array represented as a 1D row-major array.</param>
	/// <param name="startIndex">The 1D index of the top left element in the portion of the 2D array we want to flip.</param>
	/// <param name="rowCount">The number of rows in the sub-array.</param>
	/// <param name="columnCount">The number of columns in the sub-array.</param>
	public static void Flip2DSubArrayVertically<T>(T[] arr, int startIndex, int rowCount, int columnCount)
	{
		Debug.Assert((startIndex >= 0) && (rowCount >= 0) && (columnCount >= 0) && ((startIndex + (rowCount * columnCount)) <= arr.Length));

		var tmpRow = new T[columnCount];
		var lastRowIndex = rowCount - 1;

		for(int rowIndex = 0; rowIndex < (rowCount / 2); rowIndex++)
		{
			var otherRowIndex = lastRowIndex - rowIndex;

			var rowStartIndex = startIndex + (rowIndex * columnCount);
			var otherRowStartIndex = startIndex + (otherRowIndex * columnCount);

			Array.Copy(arr, otherRowStartIndex, tmpRow, 0, columnCount); // other -> tmp
			Array.Copy(arr, rowStartIndex, arr, otherRowStartIndex, columnCount); // row -> other
			Array.Copy(tmpRow, 0, arr, rowStartIndex, columnCount); // tmp -> row
		}
	}
}