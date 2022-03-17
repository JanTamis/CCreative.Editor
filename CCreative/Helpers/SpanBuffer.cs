using System;
using System.Buffers;

namespace CCreative.Helpers;

internal readonly struct SpanBuffer<T> : IDisposable
{
	private readonly T[] owner;

	public SpanBuffer(int count)
	{
		owner = ArrayPool<T>.Shared.Rent(count);
	}

	public static implicit operator Span<T>(SpanBuffer<T> buffer)
	{
		return buffer.owner;
	}

	public static explicit operator Memory<T>(SpanBuffer<T> buffer)
	{
		return buffer.owner;
	}

	public static explicit operator T[](SpanBuffer<T> buffer)
	{
		return buffer.owner;
	}

	public void Dispose()
	{
		ArrayPool<T>.Shared.Return(owner);
	}
}