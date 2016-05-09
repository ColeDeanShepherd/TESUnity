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

	public static void Flip2DArrayVertically<T>(ref T[] arr, int rowCount, int columnCount)
	{
		Debug.Assert((rowCount >= 0) && (columnCount >= 0));

		var tmpRow = new T[columnCount];
		var lastRowIndex = rowCount - 1;

		for(int rowIndex = 0; rowIndex < (rowCount / 2); rowIndex++)
		{
			var otherRowIndex = lastRowIndex - rowIndex;

			var rowStartIndex = rowIndex * columnCount;
			var otherRowStartIndex = otherRowIndex * columnCount;

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
}