using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
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

		for (var n = Count; n > 1; n--)
		{
			var k = rng.Next(n + 1);
			
			(this[k], this[n]) = (this[n], this[k]);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Append(T item)
	{
		base.Add(item);
	}

	public new void Add(T value)
	{
		Math.Add(ref Math.GetReference(this), Count, value);
	}

	public void Subtract(T value)
	{
		Math.Subtract(ref Math.GetReference(this), Count, value);
	}

	public void Divide(T value)
	{
		Math.Divide(ref Math.GetReference(this), Count, value);
	}

	public void Multiply(T value)
	{
		Math.Multiply(ref Math.GetReference(this), Count, value);
	}

	public T Min()
	{
		return Math.Min(ref Math.GetReference(this), Count);
	}

	public T Max()
	{
		return Math.Max(ref Math.GetReference(this), Count);
	}

	public T Sum()
	{
		return Math.Sum(ref Math.GetReference(this), Count);
	}

	public T Average()
	{
		return Sum() / T.CreateSaturating(Count);
	}

	public bool HasValue(T value)
	{
		return Contains(value);
	}
}