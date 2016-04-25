using System.Collections;
using System.Collections.Generic;

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