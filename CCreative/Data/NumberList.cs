using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

// ReSharper disable MemberCanBePrivate.Global

namespace CCreative.Data;

public class NumberList<T> : List<T> where T : unmanaged, INumber<T>, IMinMaxValue<T>
{
	public NumberList(IEnumerable<T> collection) : base(collection)
	{
	}

	public NumberList(Span<T> span) : base(span.Length)
	{
		for (var i = 0; i < span.Length; i++)
		{
			Add(span[i]);
		}
	}

	public NumberList(int capacity) : base(capacity)
	{
	}

	public void Append(T value)
	{
		base.Add(value);
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
		for (var i = 0; i < Count; i++)
		{
			this[i] += value;
		}
	}

	public void Sub(T value)
	{
		for (var i = 0; i < Count; i++)
		{
			this[i] -= value;
		}
	}

	public void Div(T value)
	{
		for (var i = 0; i < Count; i++)
		{
			this[i] /= value;
		}
	}

	public void Mult(T value)
	{
		for (var i = 0; i < Count; i++)
		{
			this[i] *= value;
		}
	}

	public T Min()
	{
		return Math.Min((Span<T>)this);
	}

	public T Max()
	{
		return Math.Max((Span<T>)this);
	}

	public T Sum()
	{
		return Math.Sum((Span<T>)this);
	}

	public T Average()
	{
		return Sum() / T.Create(Count);
	}

	public bool HasValue(T value)
	{
		return Contains(value);
	}

	public T[] Array()
	{
		return ToArray();
	}

	public static implicit operator Span<T>(NumberList<T> list)
	{
		return CollectionsMarshal.AsSpan(list);
	}
}