using System;
using System.Collections;
using System.Collections.Generic;
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
}