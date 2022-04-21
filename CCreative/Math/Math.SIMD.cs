using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;
using System.Threading.Tasks.Dataflow;

// ReSharper disable CheckNamespace

namespace CCreative;

internal delegate TResult VectorDelegate<T, out TResult>(Span<Vector<T>> data) where T : struct;

internal delegate void VectorDelegate<T>(Span<Vector<T>> data, T state) where T : struct;

internal delegate TResult SpanDelegate<T, out TResult>(Span<T> data, T vectorResult) where T : struct;

internal delegate void SpanDelegate<T>(Span<T> data) where T : struct;

public static partial class Math
{
	private static int _degreeOfParallelism = 1; // BitOperations.Log2((uint)Environment.ProcessorCount);

	private const int SIZE_RESTRAINT = 1024 * 64;

	// private static readonly ConcurrentExclusiveSchedulerPair concurrentExclusiveScheduler = new(TaskScheduler.Default, Environment.ProcessorCount / 2); // BitOperations.Log2((uint)Environment.ProcessorCount));

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Min<T>(T num1, T num2) where T : INumber<T>
	{
		return T.Min(num1, num2);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Min<T>(T num1, T num2, T num3) where T : INumber<T>
	{
		return Min(Min(num1, num2), num3);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Max<T>(T num1, T num2) where T : INumber<T>
	{
		return T.Max(num1, num2);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Max<T>(T num1, T num2, T num3) where T : INumber<T>
	{
		return Max(Max(num1, num2), num3);
	}

	public static T Min<T>(params T[] numbers) where T : unmanaged, INumber<T>, IMinMaxValue<T>
	{
		if (numbers.Length >= SIZE_RESTRAINT && _degreeOfParallelism > 1)
		{
			var result = GC.AllocateUninitializedArray<T>(_degreeOfParallelism);

			var block = new ActionBlock<(T[] array, T[] temp, int index, int length)>(tuple =>
			{
				var (tempArray, temp, index, length) = tuple;
				temp[index] = Min(tempArray.AsSpan(index * _degreeOfParallelism, length));
			}, new ExecutionDataflowBlockOptions
			{
				EnsureOrdered = false,
				MaxDegreeOfParallelism = _degreeOfParallelism,
			});

			for (var i = 0; i < _degreeOfParallelism; i++)
			{
				block.Post((numbers, result, i, numbers.Length / _degreeOfParallelism));
			}

			block.Complete();
			block.Completion.Wait();

			return Min(result.AsSpan());
		}

		return Min(numbers.AsSpan());
	}

	public static T Max<T>(T[] numbers) where T : unmanaged, INumber<T>, IMinMaxValue<T>
	{
		if (numbers.Length >= SIZE_RESTRAINT && _degreeOfParallelism > 1)
		{
			var result = GC.AllocateUninitializedArray<T>(_degreeOfParallelism);

			var block = new ActionBlock<(T[] array, T[] temp, int index, int length)>(tuple =>
			{
				var (tempArray, temp, index, length) = tuple;
				temp[index] = Max(tempArray.AsSpan(index * _degreeOfParallelism, length));
			}, new ExecutionDataflowBlockOptions
			{
				EnsureOrdered = false,
				MaxDegreeOfParallelism = _degreeOfParallelism,
			});

			for (var i = 0; i < _degreeOfParallelism; i++)
			{
				block.Post((numbers, result, i, numbers.Length / _degreeOfParallelism));
			}

			block.Complete();
			block.Completion.Wait();

			return Max(result.AsSpan());
		}

		return Max(numbers.AsSpan());
	}

	public static T Max<T>(List<T> numbers) where T : unmanaged, INumber<T>, IMinMaxValue<T>
	{
		return Max(CollectionsMarshal.AsSpan(numbers));
	}

	public static T Sum<T>(T[] numbers) where T : unmanaged, INumber<T>
	{
		if (numbers.Length >= SIZE_RESTRAINT && _degreeOfParallelism > 1)
		{
			var result = GC.AllocateUninitializedArray<T>(_degreeOfParallelism);

			var block = new ActionBlock<(T[] array, T[] temp, int index, int length)>(tuple =>
			{
				var (tempArray, temp, index, length) = tuple;
				temp[index] = Sum(tempArray.AsSpan(index * _degreeOfParallelism, length));
			}, new ExecutionDataflowBlockOptions
			{
				EnsureOrdered = false,
				MaxDegreeOfParallelism = _degreeOfParallelism,
			});

			for (var i = 0; i < _degreeOfParallelism; i++)
			{
				block.Post((numbers, result, i, numbers.Length / _degreeOfParallelism));
			}

			block.Complete();
			block.Completion.Wait();

			return Sum(result.AsSpan());
		}

		return Sum(numbers.AsSpan());
	}

	/// <summary>
	/// Determines the smallest value in a sequence of numbers
	/// </summary>
	/// <param name="numbers">array of numbers to compare</param>
	/// <returns>returns the minimum value</returns>
	internal static T Min<T>(Span<T> numbers) where T : unmanaged, INumber<T>, IMinMaxValue<T>
	{
		return Aggregate(numbers, vectors =>
		{
			var result = vectors[0];
			var min = T.MaxValue;

			for (var i = 1; i < vectors.Length; i++)
			{
				result = System.Numerics.Vector.Min(result, vectors[i]);
			}

			for (var i = 0; i < Vector<T>.Count; i++)
			{
				min = Min(min, result[i]);
			}

			return min;
		}, (data, min) =>
		{
			foreach (var number in data)
			{
				min = Min(number, min);
			}

			return min;
		});
	}

	/// <summary>
	/// Determines the biggest value in a sequence of numbers
	/// </summary>
	/// <param name="numbers">array of numbers to compare</param>
	/// <returns>returns the maximum value</returns>
	public static T Max<T>(Span<T> numbers) where T : unmanaged, INumber<T>, IMinMaxValue<T>
	{
		return Aggregate(numbers, vectors =>
		{
			var result = vectors[0];
			var max = T.MinValue;

			for (var i = 1; i < vectors.Length; i++)
			{
				result = System.Numerics.Vector.Max(result, vectors[i]);
			}

			for (var i = 0; i < Vector<T>.Count; i++)
			{
				max = Max(max, result[i]);
			}

			return max;
		}, (data, max) =>
		{
			foreach (var number in data)
			{
				max = Max(number, max);
			}

			return max;
		});
	}

	/// <summary>
	/// Determines the sum of a sequence of numbers
	/// </summary>
	/// <param name="numbers">array of get the sum of</param>
	/// <returns>returns the sum</returns>
	internal static T Sum<T>(Span<T> numbers) where T : unmanaged, INumber<T>
	{
		return Aggregate(numbers, vectors =>
		{
			var vSum = Vector<T>.Zero;

			for (var i = 1; i < vectors.Length; i++)
			{
				vSum = System.Numerics.Vector.Add(vSum, vectors[i]);
			}

			return System.Numerics.Vector.Sum(vSum);
		}, (data, sum) =>
		{
			foreach (var number in data)
			{
				sum += number;
			}

			return sum;
		});
	}

	/// <summary>
	/// Determines the average value in a sequence of numbers
	/// </summary>
	/// <param name="numbers">array of numbers to compare</param>
	/// <returns>returns the average value</returns>
	public static T Average<T>(T[] numbers) where T : unmanaged, INumber<T>
	{
		return Sum(numbers) / T.Create(numbers.Length);
	}

	/// <summary>
	/// Adds a number to every element of the array
	/// </summary>
	/// <param name="numbers">the numbers to add the number to</param>
	/// <param name="number">the number to add to every element</param>
	public static void Add<T>(T[] numbers, T number) where T : unmanaged, INumber<T>
	{
		Aggregate(numbers, (vectors, state) =>
		{
			var vValue = new Vector<T>(state);

			for (var i = 0; i < vectors.Length; i++)
			{
				vectors[i] += vValue;
			}
		}, number);
	}

	/// <summary>
	/// Subtracts a number to the array
	/// </summary>
	/// <param name="numbers">the numbers to subtract the number from from</param>
	/// <param name="number"></param>
	public static void Subtract<T>(T[] numbers, T number) where T : unmanaged, INumber<T>
	{
		Aggregate(numbers, (vectors, state) =>
		{
			var vValue = new Vector<T>(state);

			for (var i = 0; i < vectors.Length; i++)
			{
				vectors[i] -= vValue;
			}
		}, number);
	}

	/// <summary>
	/// Multiplies a number with the numbers
	/// </summary>
	/// <param name="numbers">the numbers to multiply the number with</param>
	/// <param name="number">the number to multiply the numbers with</param>
	public static void Multiply<T>(T[] numbers, T number) where T : unmanaged, INumber<T>
	{
		Aggregate(numbers, (vectors, state) =>
		{
			var vValue = new Vector<T>(state);

			for (var i = 0; i < vectors.Length; i++)
			{
				vectors[i] *= vValue;
			}
		}, number);
	}

	/// <summary>
	/// Divide the numbers with the given number
	/// </summary>
	/// <param name="numbers">the numbers to divide the number with</param>
	/// <param name="number">the number to divide the numbers with</param>
	public static void Divide<T>(T[] numbers, T number) where T : unmanaged, INumber<T>
	{
		Aggregate(numbers, (vectors, state) =>
		{
			var vValue = new Vector<T>(state);

			for (var i = 0; i < vectors.Length; i++)
			{
				vectors[i] /= vValue;
			}
		}, number);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static T Aggregate<T>(Span<T> numbers, VectorDelegate<T, T> vectorAction, SpanDelegate<T, T> spanAction) where T : unmanaged, INumber<T>
	{
		var count = Vector<T>.Count;
		var result = default(T);
		var index = 0;

		if (System.Numerics.Vector.IsHardwareAccelerated && numbers.Length >= count * 2)
		{
			var vectors = MemoryMarshal.Cast<T, Vector<T>>(numbers);

			result = vectorAction(vectors);
			index = count * vectors.Length;
		}

		if (index < numbers.Length - 1)
		{
			result = spanAction(numbers[index..], result);
		}

		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T FusedMultiplyAdd<T>(T a, T b, T addend) where T :
		IMultiplyOperators<T, T, T>,
		IAdditionOperators<T, T, T>
	{
		if (typeof(T) == typeof(float))
		{
			if (AdvSimd.IsSupported)
			{
				var left = Vector64.CreateScalarUnsafe(Unsafe.As<T, float>(ref a));
				var right = Vector64.CreateScalarUnsafe(Unsafe.As<T, float>(ref b));
				var add = Vector64.CreateScalarUnsafe(Unsafe.As<T, float>(ref addend));

				return (T)(object)AdvSimd.FusedMultiplyAddScalar(add, left, right).ToScalar();
			}

			if (Fma.IsSupported)
			{
				var left = Vector128.CreateScalarUnsafe(Unsafe.As<T, float>(ref a));
				var right = Vector128.CreateScalarUnsafe(Unsafe.As<T, float>(ref b));
				var add = Vector128.CreateScalarUnsafe(Unsafe.As<T, float>(ref addend));

				return (T)(object)Fma.MultiplyAddScalar(add, left, right).ToScalar();
			}
		}
		else if (typeof(T) == typeof(double))
		{
			if (AdvSimd.Arm64.IsSupported)
			{
				var left = Vector128.CreateScalarUnsafe(Unsafe.As<T, double>(ref a));
				var right = Vector128.CreateScalarUnsafe(Unsafe.As<T, double>(ref b));
				var add = Vector128.CreateScalarUnsafe(Unsafe.As<T, double>(ref addend));

				return (T)(object)AdvSimd.Arm64.FusedMultiplyAdd(add, left, right).ToScalar();
			}

			if (Fma.IsSupported)
			{
				var left = Vector128.CreateScalarUnsafe(Unsafe.As<T, double>(ref a));
				var right = Vector128.CreateScalarUnsafe(Unsafe.As<T, double>(ref b));
				var add = Vector128.CreateScalarUnsafe(Unsafe.As<T, double>(ref addend));

				return (T)(object)Fma.MultiplyAddScalar(add, left, right).ToScalar();
			}
		}
		else if (typeof(T) == typeof(Vector) && Fma.IsSupported)
		{
			var left = Unsafe.As<T, Vector128<float>>(ref a);
			var right = Unsafe.As<T, Vector128<float>>(ref b);
			var add = Unsafe.As<T, Vector128<float>>(ref addend);

			return (T)(object)Fma.MultiplyAdd(add, left, right);
		}

		return a * b + addend;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float FusedMultiplyAdd(float a, float b, float addend)
	{
		return MathF.FusedMultiplyAdd(a, b, addend);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double FusedMultiplyAdd(double a, double b, double addend)
	{
		return System.Math.FusedMultiplyAdd(a, b, addend);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float FusedMultiplySubtract(float a, float b, float minuend)
	{
		if (AdvSimd.IsSupported)
		{
			var left = Vector64.CreateScalarUnsafe(a);
			var right = Vector64.CreateScalarUnsafe(b);
			var minus = Vector64.CreateScalarUnsafe(minuend);

			return AdvSimd.FusedMultiplySubtract(minus, left, right).ToScalar();
		}

		if (Fma.IsSupported)
		{
			var left = Vector128.CreateScalarUnsafe(a);
			var right = Vector128.CreateScalarUnsafe(b);
			var minus = Vector128.CreateScalarUnsafe(minuend);

			return Fma.MultiplySubtract(minus, left, right).ToScalar();
		}

		return a * b - minuend;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double FusedMultiplySubtract(double a, double b, double minuend)
	{
		if (AdvSimd.Arm64.IsSupported)
		{
			var left = Vector128.CreateScalarUnsafe(a);
			var right = Vector128.CreateScalarUnsafe(b);
			var minus = Vector128.CreateScalarUnsafe(minuend);

			return AdvSimd.Arm64.FusedMultiplySubtract(minus, left, right).ToScalar();
		}

		if (Fma.IsSupported)
		{
			var left = Vector128.CreateScalarUnsafe(a);
			var right = Vector128.CreateScalarUnsafe(b);
			var minus = Vector128.CreateScalarUnsafe(minuend);

			return Fma.MultiplySubtractScalar(minus, left, right).ToScalar();
		}

		return a * b - minuend;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T FusedMultiplySubtract<T>(T a, T b, T minuend) where T :
		ISubtractionOperators<T, T, T>,
		IMultiplyOperators<T, T, T>
	{
		if (typeof(T) == typeof(float))
		{
			return (T)(object)FusedMultiplySubtract((float)(object)a, (float)(object)b, (float)(object)minuend);
		}

		if (typeof(T) == typeof(double))
		{
			return (T)(object)FusedMultiplySubtract((float)(object)a, (float)(object)b, (float)(object)minuend);
		}

		if (typeof(T) == typeof(Vector))
		{
			if (Fma.IsSupported)
			{
				var left = Unsafe.As<T, Vector128<float>>(ref a);
				var right = Unsafe.As<T, Vector128<float>>(ref b);
				var add = Unsafe.As<T, Vector128<float>>(ref minuend);

				return (T)(object)Fma.MultiplySubtractScalar(add, left, right);
			}
		}

		return a * b - minuend;
	}

	private static unsafe void Aggregate<T>(Span<T> numbers, VectorDelegate<T> vectorAction, T state) where T : unmanaged, INumber<T>
	{
		var vectors = new Span<Vector<T>>(Unsafe.AsPointer(ref numbers.GetPinnableReference()), Ceil((float)numbers.Length / Vector<T>.Count));

		vectorAction(vectors, state);
	}

	/// <summary>
	/// Use this method to set the concurrency level of a few methods in the Math methods
	/// </summary>
	/// <remarks>Only Use this method if you know what you are doing!!!</remarks>
	/// <param name="concurrencyLevel">the maximum concurrency level</param>
	internal static void SetMaxConcurrencyLevel(int concurrencyLevel)
	{
		_degreeOfParallelism = Min(concurrencyLevel, Environment.ProcessorCount);
	}
}