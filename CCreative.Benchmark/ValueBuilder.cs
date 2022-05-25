using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// ReSharper disable MemberCanBePrivate.Global

namespace FileExplorerCore.Helpers;

public ref struct ValueBuilder<T> where T : unmanaged
{
	private T[]? _arrayToReturnToPool;
	private Span<T> _chars;
	public int Position;

	public ValueBuilder(Span<T> initialBuffer)
	{
		_arrayToReturnToPool = null;
		_chars = initialBuffer;
		Position = 0;
	}

	public ValueBuilder(int initialCapacity)
	{
		_arrayToReturnToPool = ArrayPool<T>.Shared.Rent(initialCapacity);
		_chars = _arrayToReturnToPool;
		Position = 0;
	}

	public int Length => Position;

	public int Capacity => _chars.Length;

	public void EnsureCapacity(int capacity)
	{
		// If the caller has a bug and calls this with negative capacity, make sure to call Grow to throw an exception.
		if ((uint)capacity > (uint)_chars.Length)
			Grow(capacity - Position);
	}

	/// <summary>
	/// Get a pinnable reference to the builder.
	/// Does not ensure there is a null T after <see cref="Length"/>
	/// This overload is pattern matched in the C# 7.3+ compiler so you can omit
	/// the explicit method call, and write eg "fixed (T* c = builder)"
	/// </summary>
	public ref T GetPinnableReference()
	{
		return ref MemoryMarshal.GetReference(_chars);
	}

	public ref T this[int index] => ref _chars[index];

	public ReadOnlySpan<T> this[Range range]
	{
		get
		{
			if (Position != 0)
			{
				var (offset, length) = range.GetOffsetAndLength(Position);

				return AsSpan(offset, length);
			}

			return ReadOnlySpan<T>.Empty;
		}
	}

	/// <summary>Returns the underlying storage of the builder.</summary>
	public Span<T> RawChars => _chars;

	public ReadOnlySpan<T> AsSpan() => _chars[..Position];
	public ReadOnlySpan<T> AsSpan(int start) => _chars[start..Position];
	public ReadOnlySpan<T> AsSpan(int start, int length) => _chars.Slice(start, length);

	public bool TryCopyTo(Span<T> destination, out int charsWritten)
	{
		if (_chars[..Position].TryCopyTo(destination))
		{
			charsWritten = Position;
			Dispose();
			return true;
		}

		charsWritten = 0;
		Dispose();
		return false;
	}

	public void Insert(int index, T value, int count)
	{
		if (Position > _chars.Length - count)
		{
			Grow(count);
		}

		var remaining = Position - index;
		_chars.Slice(index, remaining).CopyTo(_chars[(index + count)..]);
		_chars.Slice(index, count).Fill(value);
		Position += count;
	}

	public void Insert(int index, T value)
	{
		if (Position > _chars.Length - 1)
		{
			Grow(1);
		}

		var remaining = Position - index;
		_chars.Slice(index, remaining).CopyTo(_chars[(index + 1)..]);
		_chars[index] = value;
		Position++;
	}

	public void Insert(int index, ReadOnlySpan<T> s)
	{
		var count = s.Length;

		if (Position > _chars.Length - count)
		{
			Grow(count);
		}

		var remaining = Position - index;
		_chars.Slice(index, remaining).CopyTo(_chars[(index + count)..]);
		s.CopyTo(_chars[index..]);
		Position += count;
	}

	public void SetPosition(int position)
	{
		Position = position;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Append(T c)
	{
		var pos = Position;
		if ((uint)pos < (uint)_chars.Length)
		{
			_chars[pos] = c;
			Position = pos + 1;
		}
		else
		{
			GrowAndAppend(c);
		}
	}

	public void Append(T c, int count)
	{
		if (Position > _chars.Length - count)
		{
			Grow(count);
		}

		var dst = _chars.Slice(Position, count);
		for (var i = 0; i < dst.Length; i++)
		{
			dst[i] = c;
		}

		Position += count;
	}

	public unsafe void Append(T* value, int length)
	{
		var pos = Position;
		if (pos > _chars.Length - length)
		{
			Grow(length);
		}

		var dst = _chars.Slice(Position, length);
		for (var i = 0; i < dst.Length; i++)
		{
			dst[i] = *value++;
		}

		Position += length;
	}

	public void Append(ReadOnlySpan<T> value)
	{
		var pos = Position;
		if (pos > _chars.Length - value.Length)
		{
			Grow(value.Length);
		}

		value.CopyTo(_chars[Position..]);
		Position += value.Length;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Span<T> AppendSpan(int length)
	{
		var origPos = Position;
		if (origPos > _chars.Length - length)
		{
			Grow(length);
		}

		Position = origPos + length;
		return _chars.Slice(origPos, length);
	}

	public T[] ToArray()
	{
		return AsSpan().ToArray();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void AppendSpan(ReadOnlySpan<T> data)
	{
		var origPos = Position;
		if (origPos > _chars.Length - data.Length)
		{
			Grow(data.Length);
		}

		Position = origPos + data.Length;
		data.CopyTo(_chars.Slice(origPos, data.Length));
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void GrowAndAppend(T c)
	{
		Grow(1);
		Append(c);
	}

	/// <summary>
	/// Resize the internal buffer either by doubling current buffer size or
	/// by adding <paramref name="additionalCapacityBeyondPos"/> to
	/// <see cref="Position"/> whichever is greater.
	/// </summary>
	/// <param name="additionalCapacityBeyondPos">
	/// Number of chars requested beyond current position.
	/// </param>
	[MethodImpl(MethodImplOptions.NoInlining)]
	private void Grow(int additionalCapacityBeyondPos)
	{
		// Make sure to let Rent throw an exception if the caller has a bug and the desired capacity is negative
		var poolArray = ArrayPool<T>.Shared.Rent((int)Math.Max((uint)(Position + additionalCapacityBeyondPos), (uint)_chars.Length * 2));

		_chars[..Position].CopyTo(poolArray);

		var toReturn = _arrayToReturnToPool;
		_chars = _arrayToReturnToPool = poolArray;

		if (toReturn != null)
		{
			ArrayPool<T>.Shared.Return(toReturn);
		}
	}

	public void Replace(T source, T replace)
	{
		for (var i = 0; i < Length; i++)
		{
			if (_chars[i].Equals(source))
			{
				_chars[i] = replace;
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Dispose()
	{
		var toReturn = _arrayToReturnToPool;
		this = default; // for safety, to avoid using pooled array if this instance is erroneously appended to again

		if (toReturn is not null)
		{
			ArrayPool<T>.Shared.Return(toReturn);
		}
	}
}