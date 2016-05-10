using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A reference to a range of elements in an array.
/// </summary>
public struct ArrayRange<T> : IEnumerable<T>
{
	public struct Enumerator : IEnumerator<T>
	{
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

		public Enumerator(ArrayRange<T> arrayRange)
		{
			this.arrayRange = arrayRange;
			currentIndex = arrayRange.offset - 1; // Enumerators start positioned before the first element.
		}
		public void Dispose()
		{
			arrayRange = new ArrayRange<T>();
			currentIndex = -1;
		}

		public bool MoveNext()
		{
			currentIndex++;

			return currentIndex < (arrayRange.offset + arrayRange.length);
		}
		public void Reset()
		{
			currentIndex = arrayRange.offset - 1; // Enumerators start positioned before the first element.
		}

		private ArrayRange<T> arrayRange;
		private int currentIndex;
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
	public int length
	{
		get
		{
			return _length;
		}
	}

	public ArrayRange(T[] array)
	{
		_array = array;
		_offset = 0;
		_length = array.Length;
	}
	public ArrayRange(T[] array, int offset, int length)
	{
		Debug.Assert((offset >= 0) && (length >= 0) && ((offset + length) <= array.Length));

		_array = array;
		_offset = offset;
		_length = length;
	}
	public IEnumerator<T> GetEnumerator()
	{
		return new Enumerator(this);
	}
	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	private T[] _array;
	private int _offset;
	private int _length;
}