using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace FileExplorerCore.Helpers;

public struct ArrayPoolList<T> : IList<T>, IDisposable
{
	private T[] _items;
	private int _size, _version;

	public ArrayPoolList()
	{
		_items = Array.Empty<T>();
		_size = 0;
		_version = 0;
	}

	public ArrayPoolList(int capacity)
	{
		_items = ArrayPool<T>.Shared.Rent(capacity);
		_size = capacity;
		_version = 0;
	}

	public int Count => _size;

	public bool IsReadOnly => false;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Add(T item)
	{
		_version++;

		var array = DisposeCheck(_items);
		var size = _size;

		if ((uint)size < (uint)array.Length)
		{
			_size = size + 1;
			array[size] = item;
		}
		else
		{
			AddWithResize(item);
		}
	}

	public Span<T> AppendSpan(int length)
	{
		_version++;

		var index = _size;

		EnsureCapacity(index + length);

		return _items.AsSpan(index, length);
	}

	public void AppendSpan(ReadOnlySpan<T> span)
	{
		_version++;

		var index = _size;

		EnsureCapacity(index + span.Length);

		span.CopyTo(_items.AsSpan(index));

		_size = index + span.Length;
	}

	public void Dispose()
	{
		var arr = _items;
		_items = null!;

		if (arr != null)
		{
			ArrayPool<T>.Shared.Return(arr);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void AddWithResize(T item)
	{
		var size = _size;

		EnsureCapacity(size + 1);

		_size = size + 1;
		_items[size] = item;
	}

	private const int DefaultCapacity = 4, MaxArrayLength = 0x7FEFFFFF;

	private void EnsureCapacity(int min)
	{
		if (_items.Length < min)
		{
			var newCapacity = Math.Min(_items.Length * 2, DefaultCapacity);

			if ((uint)newCapacity > MaxArrayLength)
			{
				newCapacity = MaxArrayLength;
			}

			if (newCapacity < min)
			{
				newCapacity = min;
			}

			Capacity = newCapacity;
		}
	}

	public int Capacity
	{
		get => DisposeCheck(_items).Length;
		set
		{
			if (value < _size)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException(nameof(Capacity));
			}

			if (value != _items.Length)
			{
				if (value > 0)
				{
					var newItems = ArrayPool<T>.Shared.Rent(value);
					if (_size > 0)
					{
						Array.Copy(_items, 0, newItems, 0, _size);
					}

					ArrayPool<T>.Shared.Return(_items);
					_items = newItems;
				}
				else
				{
					ArrayPool<T>.Shared.Return(_items);
					_items = Array.Empty<T>();
				}
			}
		}
	}

	public T this[int index]
	{
		get
		{
			if ((uint)index >= (uint)_size)
			{
				ThrowHelper.ThrowIndexOutOfRangeException();
			}

			return DisposeCheck(_items)[index];
		}
		set
		{
			if ((uint)index >= (uint)_size)
			{
				ThrowHelper.ThrowIndexOutOfRangeException();
			}

			_version++;
			DisposeCheck(_items)[index] = value;
		}
	}

	public void Clear()
	{
		_version++;
		_size = 0;
	}

	public bool Contains(T item)
	{
		return _size != 0 && IndexOf(item) != -1;
	}

	public int IndexOf(T item)
	{
		return Array.IndexOf(DisposeCheck(_items), item, 0, _size);
	}

	public void CopyTo(T[] array, int arrayIndex)
	{
		Array.Copy(DisposeCheck(_items), 0, array, arrayIndex, _size);
	}

	public T[] ToArray()
	{
		var array = new T[_size];
		CopyTo(array, 0);

		return array;
	}

	public ReadOnlySpan<T> AsSpan()
	{
		return _items.AsSpan(0, _size);
	}

	public IEnumerator<T> GetEnumerator()
	{
		for (var i = 0; i < _size; i++)
		{
			yield return _items[i];
		}
	}

	public bool Remove(T item)
	{
		var index = IndexOf(item);

		if (index >= 0)
		{
			RemoveAt(index);
		}

		return index != -1 && index < _size;
	}

	public void Insert(int index, T item)
	{
		throw new NotImplementedException();
	}

	public void RemoveAt(int index)
	{
		if ((uint)index >= (uint)_size)
		{
			throw new ArgumentOutOfRangeException(nameof(index), "index must be in the correct range");
		}

		_size--;

		if (index < _size)
		{
			Array.Copy(_items, index + 1, _items, index, _size - index);
		}

		if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
		{
			_items[_size] = default!;
		}

		_version++;
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private static T[] ThrowObjectDisposedException()
	{
		ThrowHelper.ThrowObjectDisposedException(nameof(ArrayPoolList<T>));
		return null!; // just to make compiler happy
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static T[] DisposeCheck(T[]? array)
	{
		return array ?? ThrowObjectDisposedException();
	}

	static class ThrowHelper
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void ThrowArgumentOutOfRangeException(string paramName)
		{
			throw new ArgumentOutOfRangeException(paramName);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void ThrowIndexOutOfRangeException()
		{
			throw new IndexOutOfRangeException();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void ThrowObjectDisposedException(string objectName)
		{
			throw new ObjectDisposedException(objectName);
		}
	}
}