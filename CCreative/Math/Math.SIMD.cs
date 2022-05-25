using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;

// ReSharper disable CheckNamespace

namespace CCreative;

public static partial class Math
{
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
	public static T Min<T>(List<T> numbers) where T : unmanaged, INumber<T>, IMinMaxValue<T>
	{
		return Min<T>(CollectionsMarshal.AsSpan(numbers));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Min<T>(T[] numbers) where T : unmanaged, INumber<T>, IMinMaxValue<T>
	{
		return Min(ref MemoryMarshal.GetArrayDataReference(numbers), numbers.Length);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Min<T>(ReadOnlySpan<T> numbers) where T : unmanaged, INumber<T>, IMinMaxValue<T>
	{
		return Min(ref MemoryMarshal.GetReference(numbers), numbers.Length);
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

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Max<T>(List<T> numbers) where T : unmanaged, INumber<T>, IMinMaxValue<T>
	{
		return Max(ref GetReference(numbers), numbers.Count);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Max<T>(T[] numbers) where T : unmanaged, INumber<T>, IMinMaxValue<T>
	{
		return Max(ref GetReference(numbers), numbers.Length);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Max<T>(ReadOnlySpan<T> numbers) where T : unmanaged, INumber<T>, IMinMaxValue<T>
	{
		return Max(ref GetReference(numbers), numbers.Length);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Sum<T>(List<T> numbers) where T : unmanaged, INumber<T>, IMinMaxValue<T>
	{
		return Sum(ref GetReference(numbers), numbers.Count);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Sum<T>(T[] numbers) where T : unmanaged, INumber<T>, IMinMaxValue<T>
	{
		return Sum(ref GetReference(numbers), numbers.Length);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Sum<T>(ReadOnlySpan<T> numbers) where T : unmanaged, INumber<T>, IMinMaxValue<T>
	{
		return Sum(ref GetReference(numbers), numbers.Length);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Average<T>(List<T> numbers) where T : unmanaged, INumber<T>, IMinMaxValue<T>
	{
		return Average(ref GetReference(numbers), numbers.Count);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Average<T>(T[] numbers) where T : unmanaged, INumber<T>, IMinMaxValue<T>
	{
		return Average(ref GetReference(numbers), numbers.Length);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Average<T>(ReadOnlySpan<T> numbers) where T : unmanaged, INumber<T>, IMinMaxValue<T>
	{
		return Average(ref GetReference(numbers), numbers.Length);
	}

	/// <summary>
	/// Determines the smallest value in a sequence of numbers
	/// </summary>
	/// <param name="first">reference to the first element of the numbers to compare</param>
	/// <param name="length">the length of the numbers</param>
	/// <returns>returns the minimum value</returns>
	[MethodImpl(MethodImplOptions.AggressiveOptimization)]
	public static T Min<T>(ref T first, int length) where T : unmanaged, INumber<T>, IMinMaxValue<T>
	{
		var index = 0;
		var min = T.MaxValue;

		if (Vector256.IsHardwareAccelerated && length >= Vector256<T>.Count * 2)
		{
			var result = Vector256.LoadUnsafe(ref first);

			while ((index += Vector256<T>.Count) < length)
			{
				result = Vector256.Min(Vector256.LoadUnsafe(ref first), result);
				first = ref Unsafe.Add(ref first, Vector256<T>.Count);
			}

			for (var i = 0; i < Vector256<T>.Count; i++)
			{
				min = Min(min, result[i]);
			}
		}

		if (Vector128.IsHardwareAccelerated && length - index >= Vector256<T>.Count * 2)
		{
			var result = Vector128.LoadUnsafe(ref first);

			while ((index += Vector128<T>.Count) < length)
			{
				result = Vector128.Min(Vector128.LoadUnsafe(ref first), result);
				first = ref Unsafe.Add(ref first, Vector128<T>.Count);
			}

			for (var i = 0; i < Vector128<T>.Count; i++)
			{
				min = Min(min, result[i]);
			}
		}

		if (Vector64.IsHardwareAccelerated && length - index >= Vector64<T>.Count * 2)
		{
			var result = Vector64.LoadUnsafe(ref first);

			while ((index += Vector64<T>.Count) < length)
			{
				result = Vector64.Min(Vector64.LoadUnsafe(ref first), result);
				first = ref Unsafe.Add(ref first, Vector64<T>.Count);
			}
			
			for (var i = 0; i < Vector64<T>.Count; i++)
			{
				min = Min(min, result[i]);
			}
		}

		for (; index < length; index++)
		{
			min = Min(first, min);
			first = ref Unsafe.Add(ref first, 1);
		}

		return min;
	}

	/// <summary>
	/// Determines the biggest value in a sequence of numbers
	/// </summary>
	/// <param name="first">reference to the first element of the numbers to compare</param>
	/// <param name="length">the length of the numbers</param>
	/// <returns>returns the maximum value</returns>
	public static T Max<T>(ref T first, int length) where T : unmanaged, INumber<T>, IMinMaxValue<T>
	{
		var index = 0;
		var max = T.MinValue;

		if (Vector256.IsHardwareAccelerated && length >= Vector256<T>.Count * 2)
		{
			var result = Vector256.LoadUnsafe(ref first);

			while ((index += Vector256<T>.Count) < length)
			{
				result = Vector256.Max(Vector256.LoadUnsafe(ref first), result);
				first = ref Unsafe.Add(ref first, Vector256<T>.Count);
			}

			for (var i = 0; i < Vector256<T>.Count; i++)
			{
				max = Max(max, result[i]);
			}
		}

		if (Vector128.IsHardwareAccelerated && length - index >= Vector256<T>.Count * 2)
		{
			var result = Vector128.LoadUnsafe(ref first);

			while ((index += Vector128<T>.Count) < length)
			{
				result = Vector128.Max(Vector128.LoadUnsafe(ref first), result);
				first = ref Unsafe.Add(ref first, Vector128<T>.Count);
			}

			for (var i = 0; i < Vector128<T>.Count; i++)
			{
				max = Max(max, result[i]);
			}
		}

		if (Vector64.IsHardwareAccelerated && length - index >= Vector64<T>.Count * 2)
		{
			var result = Vector64.LoadUnsafe(ref first);

			while ((index += Vector64<T>.Count) < length)
			{
				result = Vector64.Max(Vector64.LoadUnsafe(ref first), result);
				first = ref Unsafe.Add(ref first, Vector64<T>.Count);
			}

			for (var i = 0; i < Vector64<T>.Count; i++)
			{
				max = Max(max, result[i]);
			}
		}

		for (; index < length; index++)
		{
			max = Max(first, max);
			first = ref Unsafe.Add(ref first, 1);
		}

		return max;
	}

	/// <summary>
	/// Determines the sum of a sequence of numbers
	/// </summary>
	/// <param name="first">reference to the first element of the numbers to sum</param>
	/// <param name="length">the length of the numbers</param>
	/// <returns>returns the sum</returns>
	public static T Sum<T>(ref T first, int length) where T : unmanaged, INumber<T>
	{
		var index = 0;
		var sum = T.Zero;

		if (Vector256.IsHardwareAccelerated && length >= Vector256<T>.Count)
		{
			var result = Vector256.LoadUnsafe(ref first);

			while ((index += Vector256<T>.Count) < length)
			{
				Vector256.Add(Vector256.LoadUnsafe(ref first), result)
					.StoreUnsafe(ref first);

				first = ref Unsafe.Add(ref first, Vector256<T>.Count);
			}

			sum += Vector256.Sum(result);
		}

		if (Vector128.IsHardwareAccelerated && length - index >= Vector128<T>.Count)
		{
			var result = Vector128.LoadUnsafe(ref first);

			while ((index += Vector128<T>.Count) < length)
			{
				Vector128.Add(Vector128.LoadUnsafe(ref first), result)
					.StoreUnsafe(ref first);

				first = ref Unsafe.Add(ref first, Vector128<T>.Count);
			}

			sum += Vector128.Sum(result);
		}

		if (Vector64.IsHardwareAccelerated && length - index >= Vector64<T>.Count)
		{
			var result = Vector64.LoadUnsafe(ref first);

			while ((index += Vector64<T>.Count) < length)
			{
				Vector64.Add(Vector64.LoadUnsafe(ref first), result)
					.StoreUnsafe(ref first);

				first = ref Unsafe.Add(ref first, Vector64<T>.Count);
			}

			sum += Vector64.Sum(result);
		}

		for (; index < length; index++)
		{
			sum += first;
			first = ref Unsafe.Add(ref first, 1);
		}

		return sum;
	}

	/// <summary>
	/// Determines the average value in a sequence of numbers
	/// </summary>
	/// <param name="first">reference to the first element of the numbers to get the average of</param>
	/// <param name="length">the length of the numbers</param>
	/// <returns>returns the average value</returns>
	public static T Average<T>(ref T first, int length) where T : unmanaged, INumber<T>
	{
		return Sum(ref first, length) / T.CreateTruncating(length);
	}

	/// <summary>
	/// Adds a number to every element of the array
	/// </summary>
	/// <param name="numbers">the numbers to add the number to</param>
	/// <param name="number">the number to add to every element</param>
	public static void Add<T>(Span<T> numbers, T number) where T : unmanaged, INumber<T>
	{
		ref var first = ref MemoryMarshal.GetReference(numbers);
		var index = 0;

		if (Vector256.IsHardwareAccelerated)
		{
			var scalarResult = Vector256.Create(number);

			for (; index < numbers.Length; index += Vector256<T>.Count)
			{
				Vector256.Add(Vector256.LoadUnsafe(ref first), scalarResult)
					.StoreUnsafe(ref first);

				first = ref Unsafe.Add(ref first, Vector256<T>.Count);
			}
		}

		if (Vector128.IsHardwareAccelerated)
		{
			var scalarResult = Vector128.Create(number);

			for (; index < numbers.Length; index += Vector128<T>.Count)
			{
				Vector128.Add(Vector128.LoadUnsafe(ref first), scalarResult)
					.StoreUnsafe(ref first);

				first = ref Unsafe.Add(ref first, Vector128<T>.Count);
			}
		}

		if (Vector64.IsHardwareAccelerated)
		{
			var scalarResult = Vector64.Create(number);

			for (; index < numbers.Length; index += Vector64<T>.Count)
			{
				Vector64.Add(Vector64.LoadUnsafe(ref first), scalarResult)
					.StoreUnsafe(ref first);

				first = ref Unsafe.Add(ref first, Vector64<T>.Count);
			}
		}

		for (; index < numbers.Length; index++)
		{
			first += number;
			first = ref Unsafe.Add(ref first, 1);
		}
	}

	/// <summary>
	/// Subtracts a number to the array
	/// </summary>
	/// <param name="numbers">the numbers to subtract the number from from</param>
	/// <param name="number"></param>
	public static void Subtract<T>(Span<T> numbers, T number) where T : unmanaged, INumber<T>
	{
		ref var first = ref MemoryMarshal.GetReference(numbers);
		var index = 0;

		if (Vector256.IsHardwareAccelerated)
		{
			var scalarResult = Vector256.Create(number);

			for (; index < numbers.Length; index += Vector256<T>.Count)
			{
				Vector256.Subtract(Vector256.LoadUnsafe(ref first), scalarResult)
					.StoreUnsafe(ref first);

				first = ref Unsafe.Add(ref first, Vector256<T>.Count);
			}
		}

		if (Vector128.IsHardwareAccelerated)
		{
			var scalarResult = Vector128.Create(number);

			for (; index < numbers.Length; index += Vector128<T>.Count)
			{
				Vector128.Subtract(Vector128.LoadUnsafe(ref first), scalarResult)
					.StoreUnsafe(ref first);

				first = ref Unsafe.Add(ref first, Vector128<T>.Count);
			}
		}

		if (Vector64.IsHardwareAccelerated)
		{
			var scalarResult = Vector64.Create(number);

			for (; index < numbers.Length; index += Vector64<T>.Count)
			{
				Vector64.Subtract(Vector64.LoadUnsafe(ref first), scalarResult)
					.StoreUnsafe(ref first);

				first = ref Unsafe.Add(ref first, Vector64<T>.Count);
			}
		}

		for (; index < numbers.Length; index++)
		{
			first -= number;
			first = ref Unsafe.Add(ref first, 1);
		}
	}

	/// <summary>
	/// Multiplies a number with the numbers
	/// </summary>
	/// <param name="numbers">the numbers to multiply the number with</param>
	/// <param name="number">the number to multiply the numbers with</param>
	public static void Multiply<T>(Span<T> numbers, T number) where T : unmanaged, INumber<T>
	{
		ref var first = ref MemoryMarshal.GetReference(numbers);
		var index = 0;

		if (Vector256.IsHardwareAccelerated)
		{
			var scalarResult = Vector256.Create(number);

			for (; index < numbers.Length; index += Vector256<T>.Count)
			{
				Vector256.Multiply(Vector256.LoadUnsafe(ref first), scalarResult)
					.StoreUnsafe(ref first);

				first = ref Unsafe.Add(ref first, Vector256<T>.Count);
			}
		}

		if (Vector128.IsHardwareAccelerated)
		{
			var scalarResult = Vector128.Create(number);

			for (; index < numbers.Length; index += Vector128<T>.Count)
			{
				Vector128.Multiply(Vector128.LoadUnsafe(ref first), scalarResult)
					.StoreUnsafe(ref first);

				first = ref Unsafe.Add(ref first, Vector128<T>.Count);
			}
		}

		if (Vector64.IsHardwareAccelerated)
		{
			var scalarResult = Vector64.Create(number);

			for (; index < numbers.Length; index += Vector64<T>.Count)
			{
				Vector64.Multiply(Vector64.LoadUnsafe(ref first), scalarResult)
					.StoreUnsafe(ref first);

				first = ref Unsafe.Add(ref first, Vector64<T>.Count);
			}
		}

		for (; index < numbers.Length; index++)
		{
			first *= number;
			first = ref Unsafe.Add(ref first, 1);
		}
	}

	/// <summary>
	/// Multiplies a number with the numbers
	/// </summary>
	/// <param name="numbers">the numbers to multiply the number with</param>
	/// <param name="number">the number to multiply the numbers with</param>
	public static void Divide<T>(Span<T> numbers, T number) where T : unmanaged, INumber<T>
	{
		ref var first = ref MemoryMarshal.GetReference(numbers);
		var index = 0;

		if (Vector256.IsHardwareAccelerated)
		{
			var scalarResult = Vector256.Create(number);

			for (; index < numbers.Length; index += Vector256<T>.Count)
			{
				Vector256.Divide(Vector256.LoadUnsafe(ref first), scalarResult)
					.StoreUnsafe(ref first);

				first = ref Unsafe.Add(ref first, Vector256<T>.Count);
			}
		}

		if (Vector128.IsHardwareAccelerated)
		{
			var scalarResult = Vector128.Create(number);

			for (; index < numbers.Length; index += Vector128<T>.Count)
			{
				Vector128.Divide(Vector128.LoadUnsafe(ref first), scalarResult)
					.StoreUnsafe(ref first);

				first = ref Unsafe.Add(ref first, Vector128<T>.Count);
			}
		}

		if (Vector64.IsHardwareAccelerated)
		{
			var scalarResult = Vector64.Create(number);

			for (; index < numbers.Length; index += Vector64<T>.Count)
			{
				Vector64.Divide(Vector64.LoadUnsafe(ref first), scalarResult)
					.StoreUnsafe(ref first);

				first = ref Unsafe.Add(ref first, Vector64<T>.Count);
			}
		}

		for (; index < numbers.Length; index++)
		{
			first *= number;
			first = ref Unsafe.Add(ref first, 1);
		}
	}

	/// <summary>
	/// Divide the numbers with the given number
	/// </summary>
	/// <param name="first">reference to the first element of the numbers to compare</param>
	/// /// <param name="second">reference to the first element of the second numbers to compare</param>
	/// <param name="length">the length of the numbers</param>
	/// <returns>the dot product</returns>
	public static T Dot<T>(ref T first, ref T second, int length) where T : unmanaged, INumber<T>
	{
		var index = 0;
		var sum = T.Zero;

		if (Vector256.IsHardwareAccelerated && length >= Vector256<T>.Count)
		{
			for (; index < length; index += Vector256<T>.Count)
			{
				sum += Vector256.Dot(
					Vector256.LoadUnsafe(ref first),
					Vector256.LoadUnsafe(ref second));

				first = ref Unsafe.Add(ref first, Vector256<T>.Count);
				second = ref Unsafe.Add(ref second, Vector256<T>.Count);
			}
		}

		if (Vector128.IsHardwareAccelerated && length - index >= Vector128<T>.Count)
		{
			for (; index < length; index += Vector128<T>.Count)
			{
				sum += Vector128.Dot(
					Vector128.LoadUnsafe(ref first),
					Vector128.LoadUnsafe(ref second));

				first = ref Unsafe.Add(ref first, Vector128<T>.Count);
				second = ref Unsafe.Add(ref second, Vector128<T>.Count);
			}
		}

		if (Vector64.IsHardwareAccelerated && length - index >= Vector64<T>.Count)
		{
			for (; index < length; index += Vector64<T>.Count)
			{
				sum += Vector64.Dot(
					Vector64.LoadUnsafe(ref first),
					Vector64.LoadUnsafe(ref second));

				first = ref Unsafe.Add(ref first, Vector64<T>.Count);
				second = ref Unsafe.Add(ref second, Vector64<T>.Count);
			}
		}

		for (; index < length; index++)
		{
			sum += first * second;
			
			first = ref Unsafe.Add(ref first, Vector64<T>.Count);
			second = ref Unsafe.Add(ref second, Vector64<T>.Count);
		}

		return sum;
	}

	/// <summary>
	/// calculate the square root of the numbers
	/// </summary>
	/// <param name="first">reference to the first element of the numbers to get the square root of</param>
	/// <param name="length">the length of the numbers</param>
	[MethodImpl(MethodImplOptions.AggressiveOptimization)]
	public static void Sqrt<T>(ref T first, int length) where T : unmanaged, IBinaryFloatingPointIeee754<T>
	{
		var index = 0;

		if (Vector256.IsHardwareAccelerated && length >= Vector256<T>.Count)
		{
			for (; index < length; index += Vector256<T>.Count)
			{
				Vector256.Sqrt(Vector256.LoadUnsafe(ref first))
					.StoreUnsafe(ref first);
				
				first = ref Unsafe.Add(ref first, Vector256<T>.Count);
			}
		}

		if (Vector128.IsHardwareAccelerated && length - index >= Vector256<T>.Count)
		{
			for (; index < length; index += Vector128<T>.Count)
			{
				Vector128.Sqrt(Vector128.LoadUnsafe(ref first))
					.StoreUnsafe(ref first);

				first = ref Unsafe.Add(ref first, Vector128<T>.Count);
			}
		}

		if (Vector64.IsHardwareAccelerated && length - index >= Vector64<T>.Count)
		{
			for (; index < length; index += Vector64<T>.Count)
			{
				Vector64.Sqrt(Vector64.LoadUnsafe(ref first))
					.StoreUnsafe(ref first);
				
				first = ref Unsafe.Add(ref first, Vector64<T>.Count);
			}
		}

		for (; index < length; index++)
		{
			first = Sqrt(first);
			first = ref Unsafe.Add(ref first, 1);
		}
	}

	/// <summary>
	/// calculate the square of the numbers
	/// </summary>
	/// <param name="first">reference to the first element of the numbers to get the square root of</param>
	/// <param name="length">the length of the numbers</param>
	[MethodImpl(MethodImplOptions.AggressiveOptimization)]
	public static void Sq<T>(ref T first, int length) where T : unmanaged, INumber<T>
	{
		var index = 0;

		if (Vector256.IsHardwareAccelerated && length >= Vector256<T>.Count)
		{
			for (; index < length; index += Vector256<T>.Count)
			{
				var vector = Vector256.LoadUnsafe(ref first);
				
				Vector256.Multiply(vector, vector)
					.StoreUnsafe(ref first);

				first = ref Unsafe.Add(ref first, Vector256<T>.Count);
			}
		}

		if (Vector128.IsHardwareAccelerated && length - index >= Vector256<T>.Count)
		{
			for (; index < length; index += Vector128<T>.Count)
			{
				var vector = Vector128.LoadUnsafe(ref first); 
				
				Vector128.Multiply(vector, vector)
					.StoreUnsafe(ref first);

				first = ref Unsafe.Add(ref first, Vector128<T>.Count);
			}
		}

		if (Vector64.IsHardwareAccelerated && length - index >= Vector64<T>.Count)
		{
			for (; index < length; index += Vector64<T>.Count)
			{
				var vector = Vector64.LoadUnsafe(ref first);
				
				Vector64.Multiply(vector, vector)
					.StoreUnsafe(ref first);

				first = ref Unsafe.Add(ref first, Vector64<T>.Count);
			}
		}

		for (; index < length; index++)
		{
			first = Sq(first);
			first = ref Unsafe.Add(ref first, 1);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ref T GetReference<T>(Span<T> data)
	{
		return ref MemoryMarshal.GetReference(data);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ref T GetReference<T>(ReadOnlySpan<T> data)
	{
		return ref MemoryMarshal.GetReference(data);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ref T GetReference<T>(T[] data)
	{
		return ref MemoryMarshal.GetArrayDataReference(data);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ref T GetReference<T>(List<T>? data)
	{
		return ref MemoryMarshal.GetReference(CollectionsMarshal.AsSpan(data));
	}

	#region Fused Multiply-Add

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

			return AdvSimd.FusedMultiplySubtractScalar(minus, left, right).ToScalar();
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
			return (T)(object)FusedMultiplySubtract((double)(object)a, (double)(object)b, (double)(object)minuend);
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

	#endregion
}