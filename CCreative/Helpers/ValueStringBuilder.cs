using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// ReSharper disable MemberCanBePrivate.Global

namespace CCreative.Helpers;

internal ref struct ValueStringBuilder
{
	private char[]? _arrayToReturnToPool;
	private Span<char> _chars;
	public int Position;

	public ValueStringBuilder(Span<char> initialBuffer)
	{
		_arrayToReturnToPool = null;
		_chars = initialBuffer;
		Position = 0;
	}

	public ValueStringBuilder(int initialCapacity)
	{
		_arrayToReturnToPool = ArrayPool<char>.Shared.Rent(initialCapacity);
		_chars = _arrayToReturnToPool;
		Position = 0;
	}

	public int Length => Position;

	public int Capacity => _chars.Length;

	public void EnsureCapacity(int capacity)
	{
		// If the caller has a bug and calls this with negative capacity, make sure to call Grow to throw an exception.
		if ((uint)capacity > (uint)_chars.Length)
		{
			Grow(capacity - Position);
		}
	}

	/// <summary>
	/// Get a pinnable reference to the builder.
	/// Does not ensure there is a null char after <see cref="Length"/>
	/// This overload is pattern matched in the C# 7.3+ compiler so you can omit
	/// the explicit method call, and write eg "fixed (char* c = builder)"
	/// </summary>
	public ref char GetPinnableReference()
	{
		return ref MemoryMarshal.GetReference(_chars);
	}

	/// <summary>
	/// Get a pinnable reference to the builder.
	/// </summary>
	/// <param name="terminate">Ensures that the builder has a null char after <see cref="Length"/></param>
	public ref char GetPinnableReference(bool terminate)
	{
		if (terminate)
		{
			EnsureCapacity(Length + 1);
			_chars[Length] = '\0';
		}

		return ref MemoryMarshal.GetReference(_chars);
	}

	public ref char this[int index] => ref _chars[index];

	public ReadOnlySpan<char> this[Range range]
	{
		get
		{
			if (Position != 0)
			{
				var (offset, length) = range.GetOffsetAndLength(Position);

				return AsSpan(offset, length);
			}

			return ReadOnlySpan<char>.Empty;
		}
	}

	// public override string ToString()
	// {
	// 	var s = _chars[..Position].ToString();
	// 	Dispose();
	// 	return s;
	// }

	/// <summary>Returns the underlying storage of the builder.</summary>
	public Span<char> RawChars => _chars;

	/// <summary>
	/// Returns a span around the contents of the builder.
	/// </summary>
	/// <param name="terminate">Ensures that the builder has a null char after <see cref="Length"/></param>
	public ReadOnlySpan<char> AsSpan(bool terminate)
	{
		if (terminate)
		{
			EnsureCapacity(Length + 1);
			_chars[Length] = '\0';
		}

		return _chars[..Position];
	}

	public ReadOnlySpan<char> AsSpan()
	{
		return _chars[..Position];
	}

	public ReadOnlySpan<char> AsSpan(int start)
	{
		return _chars[start..Position];
	}

	public ReadOnlySpan<char> AsSpan(int start, int length)
	{
		return _chars.Slice(start, length);
	}

	public bool TryCopyTo(Span<char> destination, out int charsWritten)
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

	public void Insert(int index, char value, int count)
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

	public void Insert(int index, char value)
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

	public void Insert(int index, string? s)
	{
		if (s == null)
		{
			return;
		}

		var count = s.Length;

		if (Position > _chars.Length - count)
		{
			Grow(count);
		}

		var remaining = Position - index;
		_chars.Slice(index, remaining).CopyTo(_chars[(index + count)..]);
		s
#if !NET6_0_OR_GREATER
                .AsSpan()
#endif
			.CopyTo(_chars[index..]);
		Position += count;
	}

	public void Insert(int index, ReadOnlySpan<char> s)
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
	public void Append(char c)
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

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Append(string? s)
	{
		if (s is null)
		{
			return;
		}

		var pos = Position;
		if (s.Length == 1 &&
		    (uint)pos < (uint)_chars
			    .Length) // very common case, e.g. appending strings from NumberFormatInfo like separators, percent symbols, etc.
		{
			_chars[pos] = s[0];
			Position = pos + 1;
		}
		else
		{
			AppendSlow(s);
		}
	}

	private void AppendSlow(string s)
	{
		var pos = Position;
		if (pos > _chars.Length - s.Length)
		{
			Grow(s.Length);
		}

		s
#if !NET6_0_OR_GREATER
                .AsSpan()
#endif
			.CopyTo(_chars[pos..]);
		Position += s.Length;
	}

	public void Append(char c, int count)
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

	public unsafe void Append(char* value, int length)
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

	public void Append(ReadOnlySpan<char> value)
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
	public Span<char> AppendSpan(int length)
	{
		var origPos = Position;
		if (origPos > _chars.Length - length)
		{
			Grow(length);
		}

		Position = origPos + length;
		return _chars.Slice(origPos, length);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void GrowAndAppend(char c)
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
		var poolArray = ArrayPool<char>.Shared.Rent((int)Math.Max((uint)(Position + additionalCapacityBeyondPos), (uint)_chars.Length * 2));

		_chars[..Position].CopyTo(poolArray);

		var toReturn = _arrayToReturnToPool;
		_chars = _arrayToReturnToPool = poolArray;

		if (toReturn != null)
		{
			ArrayPool<char>.Shared.Return(toReturn);
		}
	}

	public void Replace(char source, char replace)
	{
		for (var i = 0; i < Length; i++)
		{
			if (_chars[i] == source)
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
			ArrayPool<char>.Shared.Return(toReturn);
		}
	}
}