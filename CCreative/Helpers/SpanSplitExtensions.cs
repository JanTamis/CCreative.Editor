using System;
using System.Runtime.CompilerServices;

namespace CCreative.Helpers;

internal ref struct Enumerable1<T> where T : IEquatable<T>
{
	public Enumerable1(ReadOnlySpan<T> span, T separator)
	{
		Span = span;
		Separator = separator;
	}

	private ReadOnlySpan<T> Span { get; }
	private T Separator { get; }

	public Enumerator1<T> GetEnumerator()
	{
		return new Enumerator1<T>(Span, Separator);
	}
}

internal ref struct Enumerable2<T> where T : IEquatable<T>
{
	public Enumerable2(ReadOnlySpan<T> span, T separator1, T separator2)
	{
		Span = span;
		Separator1 = separator1;
		Separator2 = separator2;
	}

	private ReadOnlySpan<T> Span { get; }
	private T Separator1 { get; }
	private T Separator2 { get; }

	public Enumerator2<T> GetEnumerator()
	{
		return new Enumerator2<T>(Span, Separator1, Separator2);
	}
}

public ref struct Enumerable3<T> where T : IEquatable<T>
{
	public Enumerable3(ReadOnlySpan<T> span, T separator1, T separator2, T separator3)
	{
		Span = span;
		Separator1 = separator1;
		Separator2 = separator2;
		Separator3 = separator3;
	}

	private ReadOnlySpan<T> Span { get; }
	private T Separator1 { get; }
	private T Separator2 { get; }
	private T Separator3 { get; }

	public Enumerator3<T> GetEnumerator()
	{
		return new Enumerator3<T>(Span, Separator1, Separator2, Separator3);
	}
}

internal ref struct EnumerableN<T> where T : IEquatable<T>
{
	public EnumerableN(ReadOnlySpan<T> span, ReadOnlySpan<T> separators)
	{
		Span = span;
		Separators = separators;
	}

	private ReadOnlySpan<T> Span { get; }
	private ReadOnlySpan<T> Separators { get; }

	public EnumeratorN<T> GetEnumerator()
	{
		return new EnumeratorN<T>(Span, Separators);
	}
}

internal ref struct Enumerator1<T> where T : IEquatable<T>
{
	public Enumerator1(ReadOnlySpan<T> span, T separator)
	{
		Span = span;
		Separator = separator;
		Current = default;

		if (Span.IsEmpty)
		{
			TrailingEmptyItem = true;
		}
	}

	private ReadOnlySpan<T> Span { get; set; }
	private T Separator { get; }
	private readonly int SeparatorLength => 1;

	private ReadOnlySpan<T> TrailingEmptyItemSentinel => Unsafe.As<T[]>(nameof(TrailingEmptyItemSentinel)).AsSpan();

	private bool TrailingEmptyItem
	{
		get => Span == TrailingEmptyItemSentinel;
		set => Span = value ? TrailingEmptyItemSentinel : default;
	}

	public bool MoveNext()
	{
		if (TrailingEmptyItem)
		{
			TrailingEmptyItem = false;
			Current = default;
			return true;
		}

		if (Span.IsEmpty)
		{
			Span = Current = default;
			return false;
		}

		var idx = Span.IndexOf(Separator);

		if (idx < 0)
		{
			Current = Span;
			Span = default;
		}
		else
		{
			Current = Span[..idx];
			Span = Span[(idx + SeparatorLength)..];

			if (Span.IsEmpty)
			{
				TrailingEmptyItem = true;
			}
		}

		return true;
	}

	public ReadOnlySpan<T> Current { get; private set; }
}

public ref struct Enumerator2<T> where T : IEquatable<T>
{
	public Enumerator2(ReadOnlySpan<T> span, T separator1, T separator2)
	{
		Span = span;
		Separator1 = separator1;
		Separator2 = separator2;
		Current = default;

		if (Span.IsEmpty)
		{
			TrailingEmptyItem = true;
		}
	}

	private ReadOnlySpan<T> Span { get; set; }
	private T Separator1 { get; }
	private T Separator2 { get; }
	private readonly int SeparatorLength => 1;

	private ReadOnlySpan<T> TrailingEmptyItemSentinel => Unsafe.As<T[]>(nameof(TrailingEmptyItemSentinel)).AsSpan();

	private bool TrailingEmptyItem
	{
		get => Span == TrailingEmptyItemSentinel;
		set => Span = value ? TrailingEmptyItemSentinel : default;
	}

	public bool MoveNext()
	{
		if (TrailingEmptyItem)
		{
			TrailingEmptyItem = false;
			Current = default;
			return true;
		}

		if (Span.IsEmpty)
		{
			Span = Current = default;
			return false;
		}

		var idx = Span.IndexOfAny(Separator1, Separator2);
		if (idx < 0)
		{
			Current = Span;
			Span = default;
		}
		else
		{
			Current = Span.Slice(0, idx);
			Span = Span[(idx + SeparatorLength)..];
			if (Span.IsEmpty)
			{
				TrailingEmptyItem = true;
			}
		}

		return true;
	}

	public ReadOnlySpan<T> Current { get; private set; }
}

public ref struct Enumerator3<T> where T : IEquatable<T>
{
	public Enumerator3(ReadOnlySpan<T> span, T separator1, T separator2, T separator3)
	{
		Span = span;
		Separator1 = separator1;
		Separator2 = separator2;
		Separator3 = separator3;
		Current = default;

		if (Span.IsEmpty)
		{
			TrailingEmptyItem = true;
		}
	}

	private ReadOnlySpan<T> Span { get; set; }
	private T Separator1 { get; }
	private T Separator2 { get; }
	private T Separator3 { get; }
	private readonly int SeparatorLength => 1;

	private ReadOnlySpan<T> TrailingEmptyItemSentinel => Unsafe.As<T[]>(nameof(TrailingEmptyItemSentinel)).AsSpan();

	private bool TrailingEmptyItem
	{
		get => Span == TrailingEmptyItemSentinel;
		set => Span = value ? TrailingEmptyItemSentinel : default;
	}

	public bool MoveNext()
	{
		if (TrailingEmptyItem)
		{
			TrailingEmptyItem = false;
			Current = default;
			return true;
		}

		if (Span.IsEmpty)
		{
			Span = Current = default;
			return false;
		}

		var idx = Span.IndexOfAny(Separator1, Separator2, Separator3);
		if (idx < 0)
		{
			Current = Span;
			Span = default;
		}
		else
		{
			Current = Span.Slice(0, idx);
			Span = Span[(idx + SeparatorLength)..];
			if (Span.IsEmpty)
			{
				TrailingEmptyItem = true;
			}
		}

		return true;
	}

	public ReadOnlySpan<T> Current { get; private set; }
}

public ref struct EnumeratorN<T> where T : IEquatable<T>
{
	public EnumeratorN(ReadOnlySpan<T> span, ReadOnlySpan<T> separators)
	{
		Span = span;
		Separators = separators;
		Current = default;

		if (Span.IsEmpty)
		{
			TrailingEmptyItem = true;
		}
	}

	private ReadOnlySpan<T> Span { get; set; }
	private ReadOnlySpan<T> Separators { get; }
	private readonly int SeparatorLength => 1;

	private ReadOnlySpan<T> TrailingEmptyItemSentinel => Unsafe.As<T[]>(nameof(TrailingEmptyItemSentinel)).AsSpan();

	private bool TrailingEmptyItem
	{
		get => Span == TrailingEmptyItemSentinel;
		set => Span = value ? TrailingEmptyItemSentinel : default;
	}

	public bool MoveNext()
	{
		if (TrailingEmptyItem)
		{
			TrailingEmptyItem = false;
			Current = default;
			return true;
		}

		if (Span.IsEmpty)
		{
			Span = Current = default;
			return false;
		}

		var idx = Span.IndexOfAny(Separators);
		if (idx < 0)
		{
			Current = Span;
			Span = default;
		}
		else
		{
			Current = Span.Slice(0, idx);
			Span = Span[(idx + SeparatorLength)..];
			if (Span.IsEmpty)
			{
				TrailingEmptyItem = true;
			}
		}

		return true;
	}

	public ReadOnlySpan<T> Current { get; private set; }
}

internal static class SpanSplitExtensions
{
	public static Enumerable1<T> Split<T>(this ReadOnlySpan<T> span, T separator) where T : IEquatable<T>
	{
		return new Enumerable1<T>(span, separator);
	}

	public static Enumerable2<T> Split<T>(this ReadOnlySpan<T> span, T separator1, T separator2) where T : IEquatable<T>
	{
		return new Enumerable2<T>(span, separator1, separator2);
	}

	public static Enumerable3<T> Split<T>(this ReadOnlySpan<T> span, T separator1, T separator2, T separator3) where T : IEquatable<T>
	{
		return new Enumerable3<T>(span, separator1, separator2, separator3);
	}

	public static EnumerableN<T> Split<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> values) where T : IEquatable<T>
	{
		return new EnumerableN<T>(span, values);
	}
}