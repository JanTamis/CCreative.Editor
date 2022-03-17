using System;

namespace CCreative.Delegates;

public delegate TResult VectorDelegate<T, out TResult>(Span<System.Numerics.Vector<T>> data) where T : struct;

public delegate void VectorDelegate<T>(Span<System.Numerics.Vector<T>> data, T state) where T : struct;

public delegate TResult SpanDelegate<T, out TResult>(Span<T> data, T vectorResult) where T : struct;

public delegate void SpanDelegate<T>(Span<T> data) where T : struct;