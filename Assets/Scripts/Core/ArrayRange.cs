using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A reference to a range of elements in a non-null array.
/// </summary>
public struct ArrayRange<T> : IEnumerable<T>
{
	/// <summary>
	/// An enumerator for the elements in an array range.
	/// </summary>
	public struct Enumerator : IEnumerator<T>
	{
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

		public T Current { get { return arrayRange.array[currentIndex]; } }
		object IEnumerator.Current { get { return Current; } }

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

	/// <summary>
	/// Constructs an ArrayRange referring to an entire array.
	/// </summary>
	/// <param name="array">A non-null array.</param>
	public ArrayRange(T[] array)
	{
		Debug.Assert(array != null);

		_array = array;
		_offset = 0;
		_length = array.Length;
	}

	/// <summary>
	/// Constructs an ArrayRange referring to a portion of an array.
	/// </summary>
	/// <param name="array">A non-null array.</param>
	/// <param name="offset">A nonnegative offset.</param>
	/// <param name="length">A nonnegative length.</param>
	public ArrayRange(T[] array, int offset, int length)
	{
		Debug.Assert(array != null);
		Debug.Assert((offset >= 0) && (length >= 0) && ((offset + length) <= array.Length));

		_array = array;
		_offset = offset;
		_length = length;
	}

	public T[] array { get { return _array; } }
	public int offset { get { return _offset; } }
	public int length { get { return _length; } }

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