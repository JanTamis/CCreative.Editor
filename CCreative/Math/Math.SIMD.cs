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
	/// <summary>
	/// Determines the smallest value in a sequence of numbers
	/// </summary>
	/// <param name="first">reference to the first element of the numbers to compare</param>
	/// <param name="length">the length of the numbers</param>
	/// <returns>returns the minimum value</returns>
	[MethodImpl(MethodImplOptions.AggressiveOptimization)]
	public static T Min<T>(ref T first, int length) where T : struct, INumber<T>, IMinMaxValue<T>
	{
		var index = 0;
		var min = T.MaxValue;

		if (Vector256IsSupported<T>() && length >= Vector256<T>.Count * 2)
		{
			var result = Vector256.LoadUnsafe(ref first);

			while ((index += Vector256<T>.Count) < length)
			{
				first = ref Unsafe.Add(ref first, Vector256<T>.Count);
				result = Vector256.Min(Vector256.LoadUnsafe(ref first), result);
			}

			for (var i = 0; i < Vector256<T>.Count; i++)
			{
				min = Min(min, result[i]);
			}
		}

		if (Vector128IsSupported<T>() && length - index >= Vector128<T>.Count * 2)
		{
			var result = Vector128.LoadUnsafe(ref first);

			while ((index += Vector128<T>.Count) < length)
			{
				first = ref Unsafe.Add(ref first, Vector128<T>.Count);
				result = Vector128.Min(Vector128.LoadUnsafe(ref first), result);
			}

			for (var i = 0; i < Vector128<T>.Count; i++)
			{
				min = Min(min, result[i]);
			}
		}

		if (Vector64IsSupported<T>() && length - index >= Vector64<T>.Count * 2)
		{
			var result = Vector64.LoadUnsafe(ref first);

			while ((index += Vector64<T>.Count) < length)
			{
				first = ref Unsafe.Add(ref first, Vector64<T>.Count);
				result = Vector64.Min(Vector64.LoadUnsafe(ref first), result);
			}

			for (var i = 0; i < Vector64<T>.Count; i++)
			{
				min = Min(min, result[i]);
			}
		}

		while (index < length)
		{
			min = Min(first, min);
			first = ref Unsafe.AddByteOffset(ref first, (IntPtr)Unsafe.SizeOf<T>());

			index++;
		}

		return min;
	}

	/// <summary>
	/// Determines the biggest value in a sequence of numbers
	/// </summary>
	/// <param name="first">reference to the first element of the numbers to compare</param>
	/// <param name="length">the length of the numbers</param>
	/// <returns>returns the maximum value</returns>
	public static T Max<T>(ref T first, int length) where T : struct, INumber<T>, IMinMaxValue<T>
	{
		var index = 0;
		var max = T.MinValue;

		if (Vector256IsSupported<T>() && length >= Vector256<T>.Count * 2)
		{
			var result = Vector256.LoadUnsafe(ref first);

			while ((index += Vector256<T>.Count) < length)
			{
				first = ref Unsafe.Add(ref first, Vector256<T>.Count);
				result = Vector256.Max(Vector256.LoadUnsafe(ref first), result);
			}

			for (var i = 0; i < Vector256<T>.Count; i++)
			{
				max = Max(max, result[i]);
			}
		}

		if (Vector128IsSupported<T>() && length - index >= Vector128<T>.Count * 2)
		{
			var result = Vector128.LoadUnsafe(ref first);

			while ((index += Vector128<T>.Count) < length)
			{
				first = ref Unsafe.Add(ref first, Vector128<T>.Count);
				result = Vector128.Max(Vector128.LoadUnsafe(ref first), result);
			}

			for (var i = 0; i < Vector128<T>.Count; i++)
			{
				max = Max(max, result[i]);
			}
		}

		if (Vector64IsSupported<T>() && length - index >= Vector64<T>.Count * 2)
		{
			var result = Vector64.LoadUnsafe(ref first);

			while ((index += Vector64<T>.Count) < length)
			{
				first = ref Unsafe.Add(ref first, Vector64<T>.Count);
				result = Vector64.Max(Vector64.LoadUnsafe(ref first), result);
			}

			for (var i = 0; i < Vector64<T>.Count; i++)
			{
				max = Max(max, result[i]);
			}
		}

		while (index < length)
		{
			max = Max(first, max);
			first = ref Unsafe.AddByteOffset(ref first, (IntPtr)Unsafe.SizeOf<T>());

			index++;
		}

		return max;
	}

	/// <summary>
	/// Determines the sum of a sequence of numbers
	/// </summary>
	/// <param name="first">reference to the first element of the numbers to sum</param>
	/// <param name="length">the length of the numbers</param>
	/// <returns>returns the sum</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveInlining)]
	public static T Sum<T>(ref T first, int length) where T : struct, INumber<T>
	{
		var index = 0;
		var sum = T.Zero;

		if (Vector256IsSupported<T>() && length >= Vector256<T>.Count)
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

		if (Vector128IsSupported<T>() && length - index >= Vector128<T>.Count)
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

		if (Vector64IsSupported<T>() && length - index >= Vector64<T>.Count)
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

		while (index < length)
		{
			sum += first;
			first = ref Unsafe.AddByteOffset(ref first, (IntPtr)Unsafe.SizeOf<T>());

			index++;
		}

		return sum;
	}

	/// <summary>
	/// Determines the average value in a sequence of numbers
	/// </summary>
	/// <param name="first">reference to the first element of the numbers to get the average of</param>
	/// <param name="length">the length of the numbers</param>
	/// <returns>returns the average value</returns>
	public static T Average<T>(ref T first, int length) where T : struct, INumber<T>
	{
		return Sum(ref first, length) / T.CreateTruncating(length);
	}

	/// <summary>
	/// Adds a number to every element of the array
	/// </summary>
	/// <param name="first">reference to the first element of the numbers to add the number to</param>
	/// <param name="length">the length of the numbers</param>
	/// <param name="number">the number to add to every element</param>
	public static void Add<T>(ref T first, int length, T number) where T : struct, IAdditionOperators<T, T, T>
	{
		var index = 0;

		if (Vector256IsSupported<T>() && length >= Vector256<T>.Count)
		{
			var scalarResult = Vector256.Create(number);

			for (; index < length; index += Vector256<T>.Count)
			{
				Vector256.Add(Vector256.LoadUnsafe(ref first), scalarResult)
					.StoreUnsafe(ref first);

				first = ref Unsafe.Add(ref first, Vector256<T>.Count);
			}
		}

		if (Vector128IsSupported<T>() && length - index >= Vector128<T>.Count)
		{
			var scalarResult = Vector128.Create(number);

			for (; index < length; index += Vector128<T>.Count)
			{
				Vector128.Add(Vector128.LoadUnsafe(ref first), scalarResult)
					.StoreUnsafe(ref first);

				first = ref Unsafe.Add(ref first, Vector128<T>.Count);
			}
		}

		if (Vector64IsSupported<T>() && length - index >= Vector64<T>.Count)
		{
			var scalarResult = Vector64.Create(number);

			for (; index < length; index += Vector64<T>.Count)
			{
				Vector64.Add(Vector64.LoadUnsafe(ref first), scalarResult)
					.StoreUnsafe(ref first);

				first = ref Unsafe.Add(ref first, Vector64<T>.Count);
			}
		}

		while (index < length)
		{
			first += number;
			first = ref Unsafe.AddByteOffset(ref first, (IntPtr)Unsafe.SizeOf<T>());

			index++;
		}
	}

	/// <summary>
	/// Subtracts a number to the array
	/// </summary>
	/// <param name="first">reference to the first element of the numbers to subtract the number to</param>
	/// <param name="length">the length of the numbers</param>
	/// <param name="number"></param>
	public static void Subtract<T>(ref T first, int length, T number) where T : struct, ISubtractionOperators<T, T, T>
	{
		var index = 0;

		if (Vector256IsSupported<T>() && length >= Vector256<T>.Count)
		{
			var scalarResult = Vector256.Create(number);

			for (; index < length; index += Vector256<T>.Count)
			{
				Vector256.Subtract(Vector256.LoadUnsafe(ref first), scalarResult)
					.StoreUnsafe(ref first);

				first = ref Unsafe.Add(ref first, Vector256<T>.Count);
			}
		}

		if (Vector128IsSupported<T>() && length - index >= Vector128<T>.Count)
		{
			var scalarResult = Vector128.Create(number);

			for (; index < length; index += Vector128<T>.Count)
			{
				Vector128.Subtract(Vector128.LoadUnsafe(ref first), scalarResult)
					.StoreUnsafe(ref first);

				first = ref Unsafe.Add(ref first, Vector128<T>.Count);
			}
		}

		if (Vector64IsSupported<T>() && length - index >= Vector64<T>.Count)
		{
			var scalarResult = Vector64.Create(number);

			for (; index < length; index += Vector64<T>.Count)
			{
				Vector64.Subtract(Vector64.LoadUnsafe(ref first), scalarResult)
					.StoreUnsafe(ref first);

				first = ref Unsafe.Add(ref first, Vector64<T>.Count);
			}
		}

		while (index < length)
		{
			first -= number;
			first = ref Unsafe.AddByteOffset(ref first, (IntPtr)Unsafe.SizeOf<T>());

			index++;
		}
	}

	/// <summary>
	/// Multiplies a number with the numbers
	/// </summary>
	/// <param name="first">reference to the first element of the numbers to multiply the number to</param>
	/// <param name="length">the length of the numbers</param>
	/// <param name="number">the number to multiply the numbers with</param>
	public static void Multiply<T>(ref T first, int length, T number) where T : struct, IMultiplyOperators<T, T, T>
	{
		var index = 0;

		if (Vector256IsSupported<T>() && length >= Vector256<T>.Count)
		{
			var scalarResult = Vector256.Create(number);

			for (; index < length; index += Vector256<T>.Count)
			{
				Vector256.Multiply(Vector256.LoadUnsafe(ref first), scalarResult)
					.StoreUnsafe(ref first);

				first = ref Unsafe.Add(ref first, Vector256<T>.Count);
			}
		}

		if (Vector128IsSupported<T>() && length - index >= Vector128<T>.Count)
		{
			var scalarResult = Vector128.Create(number);

			for (; index < length; index += Vector128<T>.Count)
			{
				Vector128.Multiply(Vector128.LoadUnsafe(ref first), scalarResult)
					.StoreUnsafe(ref first);

				first = ref Unsafe.Add(ref first, Vector128<T>.Count);
			}
		}

		if (Vector64IsSupported<T>() && length - index >= Vector64<T>.Count)
		{
			var scalarResult = Vector64.Create(number);

			for (; index < length; index += Vector64<T>.Count)
			{
				Vector64.Multiply(Vector64.LoadUnsafe(ref first), scalarResult)
					.StoreUnsafe(ref first);

				first = ref Unsafe.Add(ref first, Vector64<T>.Count);
			}
		}

		while (index < length)
		{
			first *= number;
			first = ref Unsafe.AddByteOffset(ref first, (IntPtr)Unsafe.SizeOf<T>());

			index++;
		}
	}

	/// <summary>
	/// Multiplies a number with the numbers
	/// </summary>
	/// <param name="first">reference to the first element of the numbers to multiply the number to</param>
	/// <param name="length">the length of the numbers</param>
	/// <param name="number">the number to divide the numbers with</param>
	public static void Divide<T>(ref T first, int length, T number) where T : struct, IDivisionOperators<T, T, T>
	{
		var index = 0;

		if (Vector256IsSupported<T>() && length >= Vector256<T>.Count)
		{
			var scalarResult = Vector256.Create(number);

			for (; index < length; index += Vector256<T>.Count)
			{
				Vector256.Divide(Vector256.LoadUnsafe(ref first), scalarResult)
					.StoreUnsafe(ref first);

				first = ref Unsafe.Add(ref first, Vector256<T>.Count);
			}
		}

		if (Vector128IsSupported<T>() && length - index >= Vector128<T>.Count)
		{
			var scalarResult = Vector128.Create(number);

			for (; index < length; index += Vector128<T>.Count)
			{
				Vector128.Divide(Vector128.LoadUnsafe(ref first), scalarResult)
					.StoreUnsafe(ref first);

				first = ref Unsafe.Add(ref first, Vector128<T>.Count);
			}
		}

		if (Vector64IsSupported<T>() && length - index >= Vector64<T>.Count)
		{
			var scalarResult = Vector64.Create(number);

			for (; index < length; index += Vector64<T>.Count)
			{
				Vector64.Divide(Vector64.LoadUnsafe(ref first), scalarResult)
					.StoreUnsafe(ref first);

				first = ref Unsafe.Add(ref first, Vector64<T>.Count);
			}
		}

		while (index < length)
		{
			first /= number;
			first = ref Unsafe.AddByteOffset(ref first, (IntPtr)Unsafe.SizeOf<T>());

			index++;
		}
	}

	/// <summary>
	/// Divide the numbers with the given number
	/// </summary>
	/// <param name="first">reference to the first element of the numbers to compare</param>
	/// /// <param name="second">reference to the first element of the second numbers to compare</param>
	/// <param name="length">the length of the numbers</param>
	/// <returns>the dot product</returns>
	public static T Dot<T>(ref T first, ref T second, int length) where T : struct, INumber<T>
	{
		var index = 0;
		var sum = T.Zero;

		if (Vector256IsSupported<T>() && length >= Vector256<T>.Count)
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

		if (Vector128IsSupported<T>() && length - index >= Vector128<T>.Count)
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

		if (Vector64IsSupported<T>() && length - index >= Vector64<T>.Count)
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

		while (index < length)
		{
			sum += first * second;

			first = ref Unsafe.Add(ref first, Vector64<T>.Count);
			second = ref Unsafe.Add(ref second, Vector64<T>.Count);

			index++;
		}

		return sum;
	}

	/// <summary>
	/// calculate the square root of the numbers
	/// </summary>
	/// <param name="first">reference to the first element of the numbers to get the square root of</param>
	/// <param name="length">the length of the numbers</param>
	[MethodImpl(MethodImplOptions.AggressiveOptimization)]
	public static void Sqrt<T>(ref T first, int length) where T : struct, IRootFunctions<T>
	{
		var index = 0;

		if (Vector256IsSupported<T>() && length >= Vector256<T>.Count)
		{
			for (; index < length; index += Vector256<T>.Count)
			{
				Vector256.Sqrt(Vector256.LoadUnsafe(ref first))
					.StoreUnsafe(ref first);

				first = ref Unsafe.Add(ref first, Vector256<T>.Count);
			}
		}

		if (Vector128IsSupported<T>() && length - index >= Vector128<T>.Count)
		{
			for (; index < length; index += Vector128<T>.Count)
			{
				Vector128.Sqrt(Vector128.LoadUnsafe(ref first))
					.StoreUnsafe(ref first);

				first = ref Unsafe.Add(ref first, Vector128<T>.Count);
			}
		}

		if (Vector64IsSupported<T>() && length - index >= Vector64<T>.Count)
		{
			for (; index < length; index += Vector64<T>.Count)
			{
				Vector64.Sqrt(Vector64.LoadUnsafe(ref first))
					.StoreUnsafe(ref first);

				first = ref Unsafe.Add(ref first, Vector64<T>.Count);
			}
		}

		while (index < length)
		{
			first = Sqrt(first);

			first = ref Unsafe.Add(ref first, 1);
			index++;
		}
	}

	/// <summary>
	/// calculate the square of the numbers
	/// </summary>
	/// <param name="first">reference to the first element of the numbers to get the square root of</param>
	/// <param name="length">the length of the numbers</param>
	[MethodImpl(MethodImplOptions.AggressiveOptimization)]
	public static void Sq<T>(ref T first, int length) where T : struct, IMultiplyOperators<T, T, T>
	{
		var index = 0;

		if (Vector256IsSupported<T>() && length >= Vector256<T>.Count)
		{
			for (; index < length; index += Vector256<T>.Count)
			{
				var vector = Vector256.LoadUnsafe(ref first);

				Vector256.Multiply(vector, vector)
					.StoreUnsafe(ref first);

				first = ref Unsafe.Add(ref first, Vector256<T>.Count);
			}
		}

		if (Vector128IsSupported<T>() && length - index >= Vector256<T>.Count)
		{
			for (; index < length; index += Vector128<T>.Count)
			{
				var vector = Vector128.LoadUnsafe(ref first);

				Vector128.Multiply(vector, vector)
					.StoreUnsafe(ref first);

				first = ref Unsafe.Add(ref first, Vector128<T>.Count);
			}
		}

		if (Vector64IsSupported<T>() && length - index >= Vector64<T>.Count)
		{
			for (; index < length; index += Vector64<T>.Count)
			{
				var vector = Vector64.LoadUnsafe(ref first);

				Vector64.Multiply(vector, vector)
					.StoreUnsafe(ref first);

				first = ref Unsafe.Add(ref first, Vector64<T>.Count);
			}
		}

		while (index < length)
		{
			first = Sq(first);
			first = ref Unsafe.Add(ref first, 1);

			index++;
		}
	}

	/// <summary>
	/// calculate the absolute value of the numbers
	/// </summary>
	/// <param name="first">reference to the first element of the numbers to get the absolute value of</param>
	/// <param name="length">the length of the numbers</param>
	[MethodImpl(MethodImplOptions.AggressiveOptimization)]
	public static void Abs<T>(ref T first, int length) where T : struct, INumber<T>
	{
		var index = 0;

		if (Vector256IsSupported<T>() && length >= Vector256<T>.Count)
		{
			for (; index < length; index += Vector256<T>.Count)
			{
				Vector256.Abs(Vector256.LoadUnsafe(ref first))
					.StoreUnsafe(ref first);

				first = ref Unsafe.Add(ref first, Vector256<T>.Count);
			}
		}

		if (Vector128IsSupported<T>() && length - index >= Vector256<T>.Count)
		{
			for (; index < length; index += Vector128<T>.Count)
			{
				Vector128.Abs(Vector128.LoadUnsafe(ref first))
					.StoreUnsafe(ref first);

				first = ref Unsafe.Add(ref first, Vector128<T>.Count);
			}
		}

		if (Vector64IsSupported<T>() && length - index >= Vector64<T>.Count)
		{
			for (; index < length; index += Vector64<T>.Count)
			{
				Vector64.Abs(Vector64.LoadUnsafe(ref first))
					.StoreUnsafe(ref first);

				first = ref Unsafe.Add(ref first, Vector64<T>.Count);
			}
		}

		while (index < length)
		{
			first = Abs(first);
			first = ref Unsafe.Add(ref first, 1);

			index++;
		}
	}

	/// <summary>
	/// calculate the absolute value of the numbers
	/// </summary>
	/// <param name="first">reference to the first element of the numbers to get the absolute value of</param>
	/// <param name="length">the length of the numbers</param>
	[MethodImpl(MethodImplOptions.AggressiveOptimization)]
	public static void Floor(ref float first, int length)
	{
		var index = 0;

		if (Vector256IsSupported<float>() && length >= Vector256<float>.Count)
		{
			for (; index < length; index += Vector256<float>.Count)
			{
				Vector256.Floor(Vector256.LoadUnsafe(ref first))
					.StoreUnsafe(ref first);

				first = ref Unsafe.Add(ref first, Vector256<float>.Count);
			}
		}

		if (Vector128IsSupported<float>() && length - index >= Vector256<float>.Count)
		{
			for (; index < length; index += Vector128<float>.Count)
			{
				Vector128.Floor(Vector128.LoadUnsafe(ref first))
					.StoreUnsafe(ref first);

				first = ref Unsafe.Add(ref first, Vector128<float>.Count);
			}
		}

		if (Vector64IsSupported<float>() && length - index >= Vector64<float>.Count)
		{
			for (; index < length; index += Vector64<float>.Count)
			{
				Vector64.Floor(Vector64.LoadUnsafe(ref first))
					.StoreUnsafe(ref first);

				first = ref Unsafe.Add(ref first, Vector64<float>.Count);
			}
		}

		while (index < length)
		{
			first = MathF.Floor(first);
			first = ref Unsafe.Add(ref first, 1);

			index++;
		}
	}

	/// <summary>
	/// calculate the absolute value of the numbers
	/// </summary>
	/// <param name="first">reference to the first element of the numbers to get the absolute value of</param>
	/// <param name="length">the length of the numbers</param>
	[MethodImpl(MethodImplOptions.AggressiveOptimization)]
	public static void Floor(ref double first, int length)
	{
		var index = 0;

		if (Vector256IsSupported<double>() && length >= Vector256<double>.Count)
		{
			for (; index < length; index += Vector256<double>.Count)
			{
				Vector256.Floor(Vector256.LoadUnsafe(ref first))
					.StoreUnsafe(ref first);

				first = ref Unsafe.Add(ref first, Vector256<double>.Count);
			}
		}

		if (Vector128IsSupported<double>() && length - index >= Vector256<double>.Count)
		{
			for (; index < length; index += Vector128<double>.Count)
			{
				Vector128.Floor(Vector128.LoadUnsafe(ref first))
					.StoreUnsafe(ref first);

				first = ref Unsafe.Add(ref first, Vector128<double>.Count);
			}
		}

		while (index < length)
		{
			first = System.Math.Floor(first);
			first = ref Unsafe.Add(ref first, 1);

			index++;
		}
	}

	/// <summary>
	/// calculate the amount of times the numbers is in the sequence
	/// </summary>
	/// <param name="first">reference to the first element of the sequence</param>
	/// <param name="length">the length of the sequence</param>
	/// <param name="number">the number to search</param>
	[MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
	public static int Count<T>(ref T first, int length, T number) where T : struct, INumber<T>
	{
		var count = T.Zero;
		var index = 0;

		if (Vector256IsSupported<T>() && length >= Vector256<T>.Count)
		{
			var scalarResult = Vector256.Create(number);
			var result = Vector256<T>.Zero;

			for (; index < length; index += Vector256<T>.Count)
			{
				result += Vector256.Equals(Vector256.LoadUnsafe(ref first), scalarResult);

				first = ref Unsafe.Add(ref first, Vector256<T>.Count);
			}

			count = Abs(Vector256.Sum(result));
		}

		if (Vector128IsSupported<T>() && length - index >= Vector128<T>.Count)
		{
			var scalarResult = Vector128.Create(number);
			var result = Vector128<T>.Zero;

			for (; index < length; index += Vector128<T>.Count)
			{
				result += Vector128.Equals(Vector128.LoadUnsafe(ref first), scalarResult);

				first = ref Unsafe.Add(ref first, Vector256<T>.Count);
			}

			count = Abs(Vector128.Sum(result));
		}

		if (Vector64IsSupported<T>() && length - index >= Vector64<T>.Count)
		{
			var scalarResult = Vector64.Create(number);
			var result = Vector64<T>.Zero;

			for (; index < length; index += Vector64<T>.Count)
			{
				result += Vector64.Equals(Vector64.LoadUnsafe(ref first), scalarResult);

				first = ref Unsafe.Add(ref first, Vector64<T>.Count);
			}

			count = Abs(Vector64.Sum(result));
		}

		while (index < length)
		{
			if (first == number)
			{
				count++;
			}

			first = ref Unsafe.Add(ref first, 1);

			index++;
		}

		return ConvertNumber<T, int>(count);
	}

	/// <summary>
	/// calculate the absolute value of the numbers
	/// </summary>
	/// <param name="first">reference to the first element of the numbers to get the absolute value of</param>
	/// <param name="length">the length of the numbers</param>
	/// <param name="number">the number to search</param>
	[MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
	public static bool Contains<T>(ref T first, int length, T number) where T : struct, IEqualityOperators<T, T>
	{
		var index = 0;

		if (Vector256IsSupported<T>() && length >= Vector256<T>.Count)
		{
			var scalarResult = Vector256.Create(number);

			for (; index < length; index += Vector256<T>.Count)
			{
				if (Vector256.EqualsAny(Vector256.LoadUnsafe(ref first), scalarResult))
				{
					return true;
				}

				first = ref Unsafe.Add(ref first, Vector256<T>.Count);
			}
		}

		if (Vector128IsSupported<T>() && length - index >= Vector128<T>.Count)
		{
			var scalarResult = Vector128.Create(number);

			for (; index < length; index += Vector128<T>.Count)
			{
				if (Vector128.EqualsAny(Vector128.LoadUnsafe(ref first), scalarResult))
				{
					return true;
				}

				first = ref Unsafe.Add(ref first, Vector256<T>.Count);
			}
		}

		if (Vector64IsSupported<T>() && length - index >= Vector64<T>.Count)
		{
			var scalarResult = Vector64.Create(number);

			for (; index < length; index += Vector64<T>.Count)
			{
				if (Vector64.EqualsAny(Vector64.LoadUnsafe(ref first), scalarResult))
				{
					return true;
				}

				first = ref Unsafe.Add(ref first, Vector64<T>.Count);
			}
		}

		while (index < length)
		{
			if (first == number)
			{
				return true;
			}

			first = ref Unsafe.Add(ref first, 1);

			index++;
		}

		return false;
	}

	/// <summary>
	/// calculate the absolute value of the numbers
	/// </summary>
	/// <param name="first">reference to the first element of the numbers to get the absolute value of</param>
	/// <param name="length">the length of the numbers</param>
	[MethodImpl(MethodImplOptions.AggressiveOptimization)]
	public static T StandardDeviation<T>(ref T first, int length) where T : struct, INumber<T>, IRootFunctions<T>
	{
		var index = 0;

		var average = Average(ref first, length);
		var sum = T.Zero;

		if (Vector256IsSupported<T>() && length >= Vector256<T>.Count)
		{
			var resultVector = Vector256<T>.Zero;
			var averageVector = Vector256.Create(average);

			for (; index < length; index += Vector256<T>.Count)
			{
				var vector = Vector256.LoadUnsafe(ref first);

				vector = Vector256.Subtract(vector, averageVector);
				vector = Vector256.Multiply(vector, vector);

				resultVector = Vector256.Add(resultVector, vector);

				first = ref Unsafe.Add(ref first, Vector256<T>.Count);
			}

			sum += Vector256.Sum(resultVector);
		}

		if (Vector128IsSupported<T>() && length - index >= Vector128<T>.Count)
		{
			var resultVector = Vector128<T>.Zero;
			var averageVector = Vector128.Create(average);

			for (; index < length; index += Vector128<T>.Count)
			{
				var vector = Vector128.LoadUnsafe(ref first);

				vector = Vector128.Subtract(vector, averageVector);
				vector = Vector128.Multiply(vector, vector);

				resultVector = Vector128.Add(resultVector, vector);

				first = ref Unsafe.Add(ref first, Vector128<T>.Count);
			}

			sum += Vector128.Sum(resultVector);
		}

		if (Vector64IsSupported<T>() && length - index >= Vector64<T>.Count)
		{
			var resultVector = Vector64<T>.Zero;
			var averageVector = Vector64.Create(average);

			for (; index < length; index += Vector64<T>.Count)
			{
				var vector = Vector64.LoadUnsafe(ref first);

				vector = Vector64.Subtract(vector, averageVector);
				vector = Vector64.Multiply(vector, vector);

				resultVector = Vector64.Add(resultVector, vector);

				first = ref Unsafe.Add(ref first, Vector64<T>.Count);
			}

			sum += Vector64.Sum(resultVector);
		}

		while (index < length)
		{
			sum += Sq(first - average);

			first = ref Unsafe.Add(ref first, 1);

			index++;
		}

		return Sqrt(sum / ConvertNumber<int, T>(length));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static ref T GetReference<T>(Span<T> data)
	{
		return ref MemoryMarshal.GetReference(data);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static ref T GetReference<T>(ReadOnlySpan<T> data)
	{
		return ref MemoryMarshal.GetReference(data);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static ref T GetReference<T>(T[] data)
	{
		return ref MemoryMarshal.GetArrayDataReference(data);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static ref T GetReference<T>(List<T>? data)
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

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool Vector256IsSupported<T>()
	{
		return Vector256.IsHardwareAccelerated &&
		       (typeof(T) == typeof(byte) ||
		        typeof(T) == typeof(double) ||
		        typeof(T) == typeof(short) ||
		        typeof(T) == typeof(int) ||
		        typeof(T) == typeof(long) ||
		        typeof(T) == typeof(nint) ||
		        typeof(T) == typeof(nuint) ||
		        typeof(T) == typeof(sbyte) ||
		        typeof(T) == typeof(float) ||
		        typeof(T) == typeof(ushort) ||
		        typeof(T) == typeof(uint) ||
		        typeof(T) == typeof(ulong));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool Vector128IsSupported<T>()
	{
		return Vector128.IsHardwareAccelerated &&
		       (typeof(T) == typeof(byte) ||
		        typeof(T) == typeof(double) ||
		        typeof(T) == typeof(short) ||
		        typeof(T) == typeof(int) ||
		        typeof(T) == typeof(long) ||
		        typeof(T) == typeof(nint) ||
		        typeof(T) == typeof(nuint) ||
		        typeof(T) == typeof(sbyte) ||
		        typeof(T) == typeof(float) ||
		        typeof(T) == typeof(ushort) ||
		        typeof(T) == typeof(uint) ||
		        typeof(T) == typeof(ulong));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool Vector64IsSupported<T>()
	{
		return Vector64.IsHardwareAccelerated &&
		       (typeof(T) == typeof(byte) ||
		        typeof(T) == typeof(double) ||
		        typeof(T) == typeof(short) ||
		        typeof(T) == typeof(int) ||
		        typeof(T) == typeof(long) ||
		        typeof(T) == typeof(nint) ||
		        typeof(T) == typeof(nuint) ||
		        typeof(T) == typeof(sbyte) ||
		        typeof(T) == typeof(float) ||
		        typeof(T) == typeof(ushort) ||
		        typeof(T) == typeof(uint) ||
		        typeof(T) == typeof(ulong));
	}
}