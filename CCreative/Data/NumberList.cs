using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

// ReSharper disable MemberCanBePrivate.Global

namespace CCreative.Data;

[RequiresPreviewFeatures]
public class NumberList<T> : List<T> where T : unmanaged, INumber<T>, IMinMaxValue<T>
{
	public NumberList(IEnumerable<T> collection) : base(collection)
	{
	}

	public NumberList(ReadOnlySpan<T> span) : base(span.Length)
	{
		for (var i = 0; i < span.Length; i++)
		{
			base.Add(span[i]);
		}
	}

	public NumberList(int capacity) : base(capacity)
	{
	}

	public void SortReverse()
	{
		Sort((x, y) => y.CompareTo(x));
	}

	public void Shuffle()
	{
		var rng = Random.Shared;

		for (var n = Count; n > 1;)
		{
			n--;
			var k = rng.Next(n + 1);

			(this[k], this[n]) = (this[n], this[k]);
		}
	}

	public new void Add(T value)
	{
		Math.Add((Span<T>)this, value);
	}

	public void Subtract(T value)
	{
		Math.Subtract((Span<T>)this, value);
	}

	public void Divide(T value)
	{
		Math.Divide((Span<T>)this, value);
	}

	public void Multiply(T value)
	{
		Math.Multiply((Span<T>)this, value);
	}

	public T Min()
	{
		return Math.Min<T>((Span<T>)this);
	}

	public T Max()
	{
		return Math.Max<T>((Span<T>)this);
	}

	public T Sum()
	{
		return Math.Sum<T>((Span<T>)this);
	}

	public T Average()
	{
		return Sum() / T.CreateChecked(Count);
	}

	public bool HasValue(T value)
	{
		return Contains(value);
	}

	public static explicit operator Span<T>(NumberList<T> list)
	{
		return CollectionsMarshal.AsSpan(list);
	}
}