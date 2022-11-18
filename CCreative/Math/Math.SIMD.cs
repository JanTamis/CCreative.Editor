using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;

namespace CCreative;

public static partial class Math
{
	/// <summary>
	/// Determines the smallest value in a sequence of numbers
	/// </summary>
	/// <param name="first">reference to the first element of the numbers to compare</param>
	/// <param name="length">the length of the numbers</param>
	/// <returns>returns the minimum value</returns>
	[MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
	internal static T Min<T>(ref T first, int length) where T : struct, INumber<T>, IMinMaxValue<T>
	{
		nint index = 0;
		var min = T.MaxValue;

		if (Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && length >= Vector256<T>.Count * 2)
		{
			var result256 = Vector256.Create(min);

			while (length >= Vector256<T>.Count * 4)
			{
				var vector1 = Vector256.LoadUnsafe(ref Unsafe.Add(ref first, index));
				var vector2 = Vector256.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector256<T>.Count));
				var vector3 = Vector256.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector256<T>.Count * 2));
				var vector4 = Vector256.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector256<T>.Count * 3));

				result256 = Vector256.Min(Vector256.Min(Vector256.Min(vector1, vector2), Vector256.Min(vector3, vector4)), result256);

				index += Vector256<T>.Count * 4;
				length -= Vector256<T>.Count * 4;
			}

			while (length >= Vector256<T>.Count * 2)
			{
				var vector1 = Vector256.LoadUnsafe(ref Unsafe.Add(ref first, index));
				var vector2 = Vector256.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector256<T>.Count));

				result256 = Vector256.Min(Vector256.Min(vector1, vector2), result256);

				index += Vector256<T>.Count * 2;
				length -= Vector256<T>.Count * 2;
			}

			while (length >= Vector256<T>.Count)
			{
				result256 = Vector256.Min(Vector256.LoadUnsafe(ref Unsafe.Add(ref first, index)), result256);

				index += Vector256<T>.Count;
				length -= Vector256<T>.Count;
			}

			for (var i = 0; i < Vector256<T>.Count; i++)
			{
				min = Min(min, result256.GetElement(i));
			}
		}

		if (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && length >= Vector128<T>.Count * 2)
		{
			var result128 = Vector128.Create(min);

			while (length >= Vector128<T>.Count * 4)
			{
				var vector1 = Vector128.LoadUnsafe(ref Unsafe.Add(ref first, index));
				var vector2 = Vector128.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector128<T>.Count));
				var vector3 = Vector128.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector128<T>.Count * 2));
				var vector4 = Vector128.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector128<T>.Count * 3));

				result128 = Vector128.Min(Vector128.Min(Vector128.Min(vector1, vector2), Vector128.Min(vector3, vector4)), result128);

				index += Vector128<T>.Count * 4;
				length -= Vector128<T>.Count * 4;
			}

			while (length >= Vector128<T>.Count * 2)
			{
				var vector1 = Vector128.LoadUnsafe(ref Unsafe.Add(ref first, index));
				var vector2 = Vector128.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector128<T>.Count));

				result128 = Vector128.Min(Vector128.Min(vector1, vector2), result128);

				index += Vector128<T>.Count * 2;
				length -= Vector128<T>.Count * 2;
			}

			while (length >= Vector128<T>.Count)
			{
				result128 = Vector128.Min(Vector128.LoadUnsafe(ref Unsafe.Add(ref first, index)), result128);

				index += Vector128<T>.Count;
				length -= Vector128<T>.Count;
			}

			for (var i = 0; i < Vector128<T>.Count; i++)
			{
				min = Min(min, result128.GetElement(i));
			}
		}

		if (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && length >= Vector64<T>.Count * 2)
		{
			var result64 = Vector64.Create(min);

			while (length >= Vector64<T>.Count * 4)
			{
				var vector1 = Vector64.LoadUnsafe(ref Unsafe.Add(ref first, index));
				var vector2 = Vector64.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector64<T>.Count));
				var vector3 = Vector64.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector64<T>.Count * 2));
				var vector4 = Vector64.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector64<T>.Count * 3));

				result64 = Vector64.Min(Vector64.Min(Vector64.Min(vector1, vector2), Vector64.Min(vector3, vector4)), result64);

				index += Vector64<T>.Count * 4;
				length -= Vector64<T>.Count * 4;
			}

			while (length >= Vector64<T>.Count * 2)
			{
				var vector1 = Vector64.LoadUnsafe(ref Unsafe.Add(ref first, index));
				var vector2 = Vector64.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector64<T>.Count));

				result64 = Vector64.Min(Vector64.Min(vector1, vector2), result64);

				index += Vector64<T>.Count * 2;
				length -= Vector64<T>.Count * 2;
			}

			while (length >= Vector64<T>.Count)
			{
				result64 = Vector64.Min(Vector64.LoadUnsafe(ref Unsafe.Add(ref first, index)), result64);

				index += Vector64<T>.Count;
				length -= Vector64<T>.Count;
			}

			for (var i = 0; i < Vector64<T>.Count; i++)
			{
				min = Min(min, result64.GetElement(i));
			}
		}

		while (length > 0)
		{
			min = Min(Unsafe.Add(ref first, index), min);

			length--;
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
	[MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
	internal static T Max<T>(ref T first, int length) where T : struct, INumber<T>, IMinMaxValue<T>
	{
		nint index = 0;
		var max = T.MinValue;

		if (Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && length >= Vector256<T>.Count * 2)
		{
			var result256 = Vector256.Create(max);

			while (length >= Vector256<T>.Count * 4)
			{
				var vector1 = Vector256.LoadUnsafe(ref Unsafe.Add(ref first, index));
				var vector2 = Vector256.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector256<T>.Count));
				var vector3 = Vector256.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector256<T>.Count * 2));
				var vector4 = Vector256.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector256<T>.Count * 3));

				result256 = Vector256.Max(Vector256.Max(Vector256.Max(vector1, vector2), Vector256.Max(vector3, vector4)), result256);

				index += Vector256<T>.Count * 4;
				length -= Vector256<T>.Count * 4;
			}

			while (length >= Vector256<T>.Count * 2)
			{
				var vector1 = Vector256.LoadUnsafe(ref Unsafe.Add(ref first, index));
				var vector2 = Vector256.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector256<T>.Count));

				result256 = Vector256.Max(Vector256.Max(vector1, vector2), result256);

				index += Vector256<T>.Count * 2;
				length -= Vector256<T>.Count * 2;
			}

			while (length >= Vector256<T>.Count)
			{
				result256 = Vector256.Max(Vector256.LoadUnsafe(ref Unsafe.Add(ref first, index)), result256);

				index += Vector256<T>.Count;
				length -= Vector256<T>.Count;
			}

			for (var i = 0; i < Vector256<T>.Count; i++)
			{
				max = Max(max, result256.GetElement(i));
			}
		}

		if (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && length >= Vector128<T>.Count * 2)
		{
			var result128 = Vector128.Create(max);

			while (length >= Vector128<T>.Count * 4)
			{
				var vector1 = Vector128.LoadUnsafe(ref Unsafe.Add(ref first, index));
				var vector2 = Vector128.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector128<T>.Count));
				var vector3 = Vector128.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector128<T>.Count * 2));
				var vector4 = Vector128.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector128<T>.Count * 3));

				result128 = Vector128.Max(Vector128.Max(Vector128.Max(vector1, vector2), Vector128.Max(vector3, vector4)), result128);

				index += Vector128<T>.Count * 4;
				length -= Vector128<T>.Count * 4;
			}

			while (length >= Vector128<T>.Count * 2)
			{
				var vector1 = Vector128.LoadUnsafe(ref Unsafe.Add(ref first, index));
				var vector2 = Vector128.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector128<T>.Count));

				result128 = Vector128.Max(Vector128.Max(vector1, vector2), result128);

				index += Vector128<T>.Count * 2;
				length -= Vector128<T>.Count * 2;
			}

			while (length >= Vector128<T>.Count)
			{
				result128 = Vector128.Max(Vector128.LoadUnsafe(ref Unsafe.Add(ref first, index)), result128);

				index += Vector128<T>.Count;
				length -= Vector128<T>.Count;
			}

			for (var i = 0; i < Vector128<T>.Count; i++)
			{
				max = Max(max, result128.GetElement(i));
			}
		}

		if (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && length >= Vector64<T>.Count * 2)
		{
			var result64 = Vector64.Create(max);

			while (length >= Vector64<T>.Count * 4)
			{
				var vector1 = Vector64.LoadUnsafe(ref Unsafe.Add(ref first, index));
				var vector2 = Vector64.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector64<T>.Count));
				var vector3 = Vector64.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector64<T>.Count * 2));
				var vector4 = Vector64.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector64<T>.Count * 3));

				result64 = Vector64.Max(Vector64.Max(Vector64.Max(vector1, vector2), Vector64.Max(vector3, vector4)), result64);

				index += Vector64<T>.Count * 4;
				length -= Vector64<T>.Count * 4;
			}

			while (length >= Vector64<T>.Count * 2)
			{
				var vector1 = Vector64.LoadUnsafe(ref Unsafe.Add(ref first, index));
				var vector2 = Vector64.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector64<T>.Count));

				result64 = Vector64.Max(Vector64.Max(vector1, vector2), result64);

				index += Vector64<T>.Count * 2;
				length -= Vector64<T>.Count * 2;
			}

			while (length >= Vector64<T>.Count)
			{
				result64 = Vector64.Max(Vector64.LoadUnsafe(ref Unsafe.Add(ref first, index)), result64);

				index += Vector64<T>.Count;
				length -= Vector64<T>.Count;
			}

			for (var i = 0; i < Vector64<T>.Count; i++)
			{
				max = Max(max, result64.GetElement(i));
			}
		}

		while (length > 0)
		{
			max = Max(Unsafe.Add(ref first, index), max);

			length--;
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
	[MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
	internal static unsafe T Sum<T>(ref T first, int length) where T : unmanaged, INumberBase<T>
	{
		nint index = 0;

		if (Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported)
		{
			var resultVector = Vector256<T>.Zero;

			while (length >= Vector256<T>.Count * 4)
			{
				var vector1 = Vector256.LoadUnsafe(ref Unsafe.Add(ref first, index));
				var vector2 = Vector256.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector256<T>.Count));
				var vector3 = Vector256.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector256<T>.Count * 2));
				var vector4 = Vector256.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector256<T>.Count * 3));

				resultVector += Vector256.Add(Vector256.Add(vector1, vector2), Vector256.Add(vector3, vector4));

				index += Vector256<T>.Count * 4;
				length -= Vector256<T>.Count * 4;
			}

			while (length > 0)
			{
				resultVector += Vector256.LoadAligned((T*)Unsafe.AsPointer(ref first) + index);

				index += Vector256<T>.Count;
				length -= Vector256<T>.Count;
			}

			return Vector256.Sum(resultVector);
		}

		if (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported)
		{
			var resultVector = Vector128<T>.Zero;

			while (length >= Vector128<T>.Count * 4)
			{
				var vector1 = Vector128.LoadUnsafe(ref Unsafe.Add(ref first, index));
				var vector2 = Vector128.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector128<T>.Count));
				var vector3 = Vector128.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector128<T>.Count * 2));
				var vector4 = Vector128.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector128<T>.Count * 3));

				resultVector += Vector128.Add(Vector128.Add(vector1, vector2), Vector128.Add(vector3, vector4));

				index += Vector128<T>.Count * 4;
				length -= Vector128<T>.Count * 4;
			}

			while (length > 0)
			{
				resultVector += Vector128.LoadAligned((T*)Unsafe.AsPointer(ref first) + index);

				index += Vector128<T>.Count;
				length -= Vector128<T>.Count;
			}

			return Vector128.Sum(resultVector);
		}

		if (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported)
		{
			var resultVector = Vector64<T>.Zero;

			while (length >= Vector64<T>.Count * 4)
			{
				var vector1 = Vector64.LoadUnsafe(ref Unsafe.Add(ref first, index));
				var vector2 = Vector64.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector64<T>.Count));
				var vector3 = Vector64.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector64<T>.Count * 2));
				var vector4 = Vector64.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector64<T>.Count * 3));

				resultVector += Vector64.Add(Vector64.Add(vector1, vector2), Vector64.Add(vector3, vector4));

				index += Vector64<T>.Count * 4;
				length -= Vector64<T>.Count * 4;
			}

			while (length > 0)
			{
				resultVector += Vector64.LoadAligned((T*)Unsafe.AsPointer(ref first) + index);

				index += Vector64<T>.Count;
				length -= Vector64<T>.Count;
			}

			return Vector64.Sum(resultVector);
		}

		var result = T.Zero;

		while (length > 0)
		{
			result += Unsafe.Add(ref first, index);

			length--;
			index++;
		}

		return result;
	}

	/// <summary>
	/// Determines the average value in a sequence of numbers
	/// </summary>
	/// <param name="first">reference to the first element of the numbers to get the average of</param>
	/// <param name="length">the length of the numbers</param>
	/// <returns>returns the average value</returns>
	internal static T Average<T>(ref T first, int length) where T : unmanaged, INumber<T>
	{
		return Sum(ref first, length) / ConvertNumber<int, T>(length);
	}

	/// <summary>
	/// Adds a number to every element of the array
	/// </summary>
	/// <param name="first">reference to the first element of the numbers to add the number to</param>
	/// <param name="length">the length of the numbers</param>
	/// <param name="number">the number to add to every element</param>
	internal static void Add<T>(ref T first, int length, T number) where T : struct, IAdditionOperators<T, T, T>
	{
		nint index = 0;

		if (Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported)
		{
			var resultVector = Vector256.Create(number);

			while (length >= Vector256<T>.Count * 2)
			{
				var vector1 = Vector256.LoadUnsafe(ref Unsafe.Add(ref first, index));
				var vector2 = Vector256.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector256<T>.Count));

				var resultVector1 = Vector256.Add(vector1, resultVector);
				var resultVector2 = Vector256.Add(vector2, resultVector);

				resultVector1.StoreUnsafe(ref Unsafe.Add(ref first, index));
				resultVector2.StoreUnsafe(ref Unsafe.Add(ref first, index + Vector256<T>.Count));

				index += Vector256<T>.Count * 2;
				length -= Vector256<T>.Count * 2;
			}

			while (length >= Vector256<T>.Count)
			{
				Vector256.Add(Vector256.LoadUnsafe(ref Unsafe.Add(ref first, index)), resultVector)
					.StoreUnsafe(ref Unsafe.Add(ref first, index));

				index += Vector256<T>.Count;
				length -= Vector256<T>.Count;
			}
		}

		if (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported)
		{
			var resultVector = Vector128.Create(number);

			while (length >= Vector128<T>.Count * 2)
			{
				var vector1 = Vector128.LoadUnsafe(ref Unsafe.Add(ref first, index));
				var vector2 = Vector128.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector128<T>.Count));

				var resultVector1 = Vector128.Add(vector1, resultVector);
				var resultVector2 = Vector128.Add(vector2, resultVector);

				resultVector1.StoreUnsafe(ref Unsafe.Add(ref first, index));
				resultVector2.StoreUnsafe(ref Unsafe.Add(ref first, index + Vector128<T>.Count));

				index += Vector128<T>.Count * 2;
				length -= Vector128<T>.Count * 2;
			}

			while (length >= Vector128<T>.Count)
			{
				Vector128.Add(Vector128.LoadUnsafe(ref Unsafe.Add(ref first, index)), resultVector)
					.StoreUnsafe(ref Unsafe.Add(ref first, index));

				index += Vector128<T>.Count;
				length -= Vector128<T>.Count;
			}
		}

		if (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported)
		{
			var resultVector = Vector64.Create(number);

			while (length >= Vector64<T>.Count * 2)
			{
				var vector1 = Vector64.LoadUnsafe(ref Unsafe.Add(ref first, index));
				var vector2 = Vector64.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector128<T>.Count));

				var resultVector1 = Vector64.Add(vector1, resultVector);
				var resultVector2 = Vector64.Add(vector2, resultVector);

				resultVector1.StoreUnsafe(ref Unsafe.Add(ref first, index));
				resultVector2.StoreUnsafe(ref Unsafe.Add(ref first, index + Vector128<T>.Count));

				index += Vector256<T>.Count * 2;
				length -= Vector256<T>.Count * 2;
			}

			while (length >= Vector64<T>.Count)
			{
				Vector64.Add(Vector64.LoadUnsafe(ref Unsafe.Add(ref first, index)), resultVector)
					.StoreUnsafe(ref Unsafe.Add(ref first, index));

				index += Vector64<T>.Count;
				length -= Vector64<T>.Count;
			}
		}

		while (length > 0)
		{
			Unsafe.Add(ref first, index) += number;

			length--;
			index++;
		}
	}

	/// <summary>
	/// Subtracts a number to the array
	/// </summary>
	/// <param name="first">reference to the first element of the numbers to subtract the number to</param>
	/// <param name="length">the length of the numbers</param>
	/// <param name="number"></param>
	internal static void Subtract<T>(ref T first, int length, T number) where T : struct, ISubtractionOperators<T, T, T>
	{
		nint index = 0;

		if (Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported)
		{
			var resultVector = Vector256.Create(number);

			while (length >= Vector256<T>.Count * 2)
			{
				var vector1 = Vector256.LoadUnsafe(ref Unsafe.Add(ref first, index));
				var vector2 = Vector256.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector256<T>.Count));

				var resultVector1 = Vector256.Subtract(vector1, resultVector);
				var resultVector2 = Vector256.Subtract(vector2, resultVector);

				resultVector1.StoreUnsafe(ref Unsafe.Add(ref first, index));
				resultVector2.StoreUnsafe(ref Unsafe.Add(ref first, index + Vector256<T>.Count));

				index += Vector256<T>.Count * 2;
				length -= Vector256<T>.Count * 2;
			}

			while (length >= Vector256<T>.Count)
			{
				Vector256.Subtract(Vector256.LoadUnsafe(ref Unsafe.Add(ref first, index)), resultVector)
					.StoreUnsafe(ref Unsafe.Add(ref first, index));

				index += Vector256<T>.Count;
				length -= Vector256<T>.Count;
			}
		}

		if (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported)
		{
			var resultVector = Vector128.Create(number);

			while (length >= Vector128<T>.Count * 2)
			{
				var vector1 = Vector128.LoadUnsafe(ref Unsafe.Add(ref first, index));
				var vector2 = Vector128.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector128<T>.Count));

				var resultVector1 = Vector128.Subtract(vector1, resultVector);
				var resultVector2 = Vector128.Subtract(vector2, resultVector);

				resultVector1.StoreUnsafe(ref Unsafe.Add(ref first, index));
				resultVector2.StoreUnsafe(ref Unsafe.Add(ref first, index + Vector128<T>.Count));

				index += Vector128<T>.Count * 2;
				length -= Vector128<T>.Count * 2;
			}

			while (length >= Vector128<T>.Count)
			{
				Vector128.Subtract(Vector128.LoadUnsafe(ref Unsafe.Add(ref first, index)), resultVector)
					.StoreUnsafe(ref Unsafe.Add(ref first, index));

				index += Vector128<T>.Count;
				length -= Vector128<T>.Count;
			}
		}

		if (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported)
		{
			var resultVector = Vector64.Create(number);

			while (length >= Vector64<T>.Count * 2)
			{
				var vector1 = Vector64.LoadUnsafe(ref Unsafe.Add(ref first, index));
				var vector2 = Vector64.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector128<T>.Count));

				var resultVector1 = Vector64.Subtract(vector1, resultVector);
				var resultVector2 = Vector64.Subtract(vector2, resultVector);

				resultVector1.StoreUnsafe(ref Unsafe.Add(ref first, index));
				resultVector2.StoreUnsafe(ref Unsafe.Add(ref first, index + Vector128<T>.Count));

				index += Vector256<T>.Count * 2;
				length -= Vector256<T>.Count * 2;
			}

			while (length >= Vector64<T>.Count)
			{
				Vector64.Subtract(Vector64.LoadUnsafe(ref Unsafe.Add(ref first, index)), resultVector)
					.StoreUnsafe(ref Unsafe.Add(ref first, index));

				index += Vector64<T>.Count;
				length -= Vector64<T>.Count;
			}
		}

		while (length > 0)
		{
			Unsafe.Add(ref first, index) -= number;

			length--;
			index++;
		}
	}

	/// <summary>
	/// Multiplies a number with the numbers
	/// </summary>
	/// <param name="first">reference to the first element of the numbers to multiply the number to</param>
	/// <param name="length">the length of the numbers</param>
	/// <param name="number">the number to multiply the numbers with</param>
	internal static void Multiply<T>(ref T first, int length, T number) where T : struct, IMultiplyOperators<T, T, T>
	{
		nint index = 0;

		if (Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported)
		{
			var resultVector = Vector256.Create(number);

			while (length >= Vector256<T>.Count * 2)
			{
				var vector1 = Vector256.LoadUnsafe(ref Unsafe.Add(ref first, index));
				var vector2 = Vector256.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector256<T>.Count));

				var resultVector1 = Vector256.Multiply(vector1, resultVector);
				var resultVector2 = Vector256.Multiply(vector2, resultVector);

				resultVector1.StoreUnsafe(ref Unsafe.Add(ref first, index));
				resultVector2.StoreUnsafe(ref Unsafe.Add(ref first, index + Vector256<T>.Count));

				index += Vector256<T>.Count * 2;
				length -= Vector256<T>.Count * 2;
			}

			while (length >= Vector256<T>.Count)
			{
				Vector256.Multiply(Vector256.LoadUnsafe(ref Unsafe.Add(ref first, index)), resultVector)
					.StoreUnsafe(ref Unsafe.Add(ref first, index));

				index += Vector256<T>.Count;
				length -= Vector256<T>.Count;
			}
		}

		if (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported)
		{
			var resultVector = Vector128.Create(number);

			while (length >= Vector128<T>.Count * 2)
			{
				var vector1 = Vector128.LoadUnsafe(ref Unsafe.Add(ref first, index));
				var vector2 = Vector128.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector128<T>.Count));

				var resultVector1 = Vector128.Multiply(vector1, resultVector);
				var resultVector2 = Vector128.Multiply(vector2, resultVector);

				resultVector1.StoreUnsafe(ref Unsafe.Add(ref first, index));
				resultVector2.StoreUnsafe(ref Unsafe.Add(ref first, index + Vector128<T>.Count));

				index += Vector128<T>.Count * 2;
				length -= Vector128<T>.Count * 2;
			}

			while (length >= Vector128<T>.Count)
			{
				Vector128.Multiply(Vector128.LoadUnsafe(ref Unsafe.Add(ref first, index)), resultVector)
					.StoreUnsafe(ref Unsafe.Add(ref first, index));

				index += Vector128<T>.Count;
				length -= Vector128<T>.Count;
			}
		}

		if (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported)
		{
			var resultVector = Vector64.Create(number);

			while (length >= Vector64<T>.Count * 2)
			{
				var vector1 = Vector64.LoadUnsafe(ref Unsafe.Add(ref first, index));
				var vector2 = Vector64.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector128<T>.Count));

				var resultVector1 = Vector64.Multiply(vector1, resultVector);
				var resultVector2 = Vector64.Multiply(vector2, resultVector);

				resultVector1.StoreUnsafe(ref Unsafe.Add(ref first, index));
				resultVector2.StoreUnsafe(ref Unsafe.Add(ref first, index + Vector128<T>.Count));

				index += Vector256<T>.Count * 2;
				length -= Vector256<T>.Count * 2;
			}

			while (length >= Vector64<T>.Count)
			{
				Vector64.Multiply(Vector64.LoadUnsafe(ref Unsafe.Add(ref first, index)), resultVector)
					.StoreUnsafe(ref Unsafe.Add(ref first, index));

				index += Vector64<T>.Count;
				length -= Vector64<T>.Count;
			}
		}

		while (length > 0)
		{
			Unsafe.Add(ref first, index) *= number;

			length--;
			index++;
		}
	}

	/// <summary>
	/// Multiplies a number with the numbers
	/// </summary>
	/// <param name="first">reference to the first element of the numbers to multiply the number to</param>
	/// <param name="length">the length of the numbers</param>
	/// <param name="number">the number to divide the numbers with</param>
	internal static void Divide<T>(ref T first, int length, T number) where T : struct, IDivisionOperators<T, T, T>
	{
		nint index = 0;

		if (Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported)
		{
			var resultVector = Vector256.Create(number);

			while (length >= Vector256<T>.Count * 2)
			{
				var vector1 = Vector256.LoadUnsafe(ref Unsafe.Add(ref first, index));
				var vector2 = Vector256.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector256<T>.Count));

				var resultVector1 = Vector256.Divide(vector1, resultVector);
				var resultVector2 = Vector256.Divide(vector2, resultVector);

				resultVector1.StoreUnsafe(ref Unsafe.Add(ref first, index));
				resultVector2.StoreUnsafe(ref Unsafe.Add(ref first, index + Vector256<T>.Count));

				index += Vector256<T>.Count * 2;
				length -= Vector256<T>.Count * 2;
			}

			while (length >= Vector256<T>.Count)
			{
				Vector256.Divide(Vector256.LoadUnsafe(ref Unsafe.Add(ref first, index)), resultVector)
					.StoreUnsafe(ref Unsafe.Add(ref first, index));

				index += Vector256<T>.Count;
				length -= Vector256<T>.Count;
			}
		}

		if (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported)
		{
			var resultVector = Vector128.Create(number);

			while (length >= Vector128<T>.Count * 2)
			{
				var vector1 = Vector128.LoadUnsafe(ref Unsafe.Add(ref first, index));
				var vector2 = Vector128.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector128<T>.Count));

				var resultVector1 = Vector128.Divide(vector1, resultVector);
				var resultVector2 = Vector128.Divide(vector2, resultVector);

				resultVector1.StoreUnsafe(ref Unsafe.Add(ref first, index));
				resultVector2.StoreUnsafe(ref Unsafe.Add(ref first, index + Vector128<T>.Count));

				index += Vector128<T>.Count * 2;
				length -= Vector128<T>.Count * 2;
			}

			while (length >= Vector128<T>.Count)
			{
				Vector128.Divide(Vector128.LoadUnsafe(ref Unsafe.Add(ref first, index)), resultVector)
					.StoreUnsafe(ref Unsafe.Add(ref first, index));

				index += Vector128<T>.Count;
				length -= Vector128<T>.Count;
			}
		}

		if (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported)
		{
			var resultVector = Vector64.Create(number);

			while (length >= Vector64<T>.Count * 2)
			{
				var vector1 = Vector64.LoadUnsafe(ref Unsafe.Add(ref first, index));
				var vector2 = Vector64.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector64<T>.Count));

				var resultVector1 = Vector64.Divide(vector1, resultVector);
				var resultVector2 = Vector64.Divide(vector2, resultVector);

				resultVector1.StoreUnsafe(ref Unsafe.Add(ref first, index));
				resultVector2.StoreUnsafe(ref Unsafe.Add(ref first, index + Vector64<T>.Count));

				index += Vector256<T>.Count * 2;
				length -= Vector256<T>.Count * 2;
			}

			while (length >= Vector64<T>.Count)
			{
				Vector64.Divide(Vector64.LoadUnsafe(ref Unsafe.Add(ref first, index)), resultVector)
					.StoreUnsafe(ref Unsafe.Add(ref first, index));

				index += Vector64<T>.Count;
				length -= Vector64<T>.Count;
			}
		}

		while (length > 0)
		{
			Unsafe.Add(ref first, index) /= number;

			length--;
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
	internal static int Count<T>(ref T first, int length, T number) where T : struct, INumber<T>
	{
		nint index = 0;
		var result = T.Zero;

		if (Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && length >= Vector256<T>.Count * 2)
		{
			var resultVector = Vector256<T>.Zero;
			var numberVector = Vector256.Create(number);

			do
			{
				var vector1 = Vector256.LoadUnsafe(ref Unsafe.Add(ref first, index));
				var vector2 = Vector256.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector256<T>.Count));

				resultVector += Vector256.Add(Vector256.Equals(vector1, numberVector), Vector256.Equals(vector2, numberVector));

				index += Vector256<T>.Count * 2;
				length -= Vector256<T>.Count * 2;
			} while (length >= Vector256<T>.Count * 2);

			while (length >= Vector256<T>.Count)
			{
				resultVector += Vector256.Equals(Vector256.LoadUnsafe(ref Unsafe.Add(ref first, index)), numberVector);

				index += Vector256<T>.Count;
				length -= Vector256<T>.Count;
			}

			result += Vector256.Sum(resultVector);
		}

		if (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && length >= Vector128<T>.Count * 2)
		{
			var resultVector = Vector128<T>.Zero;
			var numberVector = Vector128.Create(number);

			do
			{
				var vector1 = Vector128.LoadUnsafe(ref Unsafe.Add(ref first, index));
				var vector2 = Vector128.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector128<T>.Count));

				resultVector += Vector128.Add(Vector128.Equals(vector1, numberVector), Vector128.Equals(vector2, numberVector));

				index += Vector128<T>.Count * 2;
				length -= Vector128<T>.Count * 2;
			} while (length >= Vector128<T>.Count * 2);

			while (length >= Vector128<T>.Count)
			{
				resultVector += Vector128.Equals(Vector128.LoadUnsafe(ref Unsafe.Add(ref first, index)), numberVector);

				index += Vector128<T>.Count;
				length -= Vector128<T>.Count;
			}

			result += Vector128.Sum(resultVector);
		}

		if (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && length >= Vector64<T>.Count * 2)
		{
			var resultVector = Vector64<T>.Zero;
			var numberVector = Vector64.Create(number);

			do
			{
				var vector1 = Vector64.LoadUnsafe(ref Unsafe.Add(ref first, index));
				var vector2 = Vector64.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector64<T>.Count));

				resultVector += Vector64.Add(Vector64.Equals(vector1, numberVector), Vector64.Equals(vector2, numberVector));

				index += Vector64<T>.Count * 2;
				length -= Vector64<T>.Count * 2;
			} while (length >= Vector64<T>.Count * 2);

			while (length >= Vector64<T>.Count)
			{
				resultVector = Vector64.Equals(Vector64.LoadUnsafe(ref Unsafe.Add(ref first, index)), numberVector);

				index += Vector64<T>.Count;
				length -= Vector64<T>.Count;
			}

			result += Vector64.Sum(resultVector);
		}

		while (length > 0)
		{
			if (Unsafe.Add(ref first, index) == number)
			{
				result++;
			}

			length--;
			index++;
		}

		return ConvertNumber<T, int>(result);
	}

	/// <summary>
	/// calculate the absolute value of the numbers
	/// </summary>
	/// <param name="first">reference to the first element of the numbers to get the absolute value of</param>
	/// <param name="length">the length of the numbers</param>
	/// <param name="number">the number to search</param>
	[MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
	internal static bool Contains<T>(ref T first, int length, T number) where T : struct, IEquatable<T>
	{
		nint index = 0;

		if (Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported)
		{
			var resultVector = Vector256<T>.Zero;
			var numberVector = Vector256.Create(number);

			if ((uint)length % (uint)Vector256<T>.Count != 0)
			{
				resultVector = Vector256.Equals(Vector256.LoadUnsafe(ref first), resultVector);

				if (resultVector != Vector256<T>.Zero)
				{
					return true;
				}

				index += length % Vector256<T>.Count;
				length -= length % Vector256<T>.Count;
			}

			while (length >= Vector256<T>.Count * 4)
			{
				var vector1 = Vector256.LoadUnsafe(ref Unsafe.Add(ref first, index));
				var vector2 = Vector256.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector256<T>.Count));
				var vector3 = Vector256.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector256<T>.Count * 2));
				var vector4 = Vector256.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector256<T>.Count * 3));

				resultVector = Vector256.Add(
					Vector256.Add(Vector256.Equals(vector1, numberVector), Vector256.Equals(vector2, numberVector)),
					Vector256.Add(Vector256.Equals(vector3, numberVector), Vector256.Equals(vector4, numberVector)));

				if (resultVector != Vector256<T>.Zero)
				{
					return true;
				}

				index += Vector256<T>.Count * 4;
				length -= Vector256<T>.Count * 4;
			}

			while (length >= Vector256<T>.Count)
			{
				resultVector = Vector256.Equals(Vector256.LoadUnsafe(ref Unsafe.Add(ref first, index)), numberVector);

				if (resultVector != Vector256<T>.Zero)
				{
					return true;
				}

				index += Vector256<T>.Count;
				length -= Vector256<T>.Count;
			}

			return false;
		}

		if (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported)
		{
			var resultVector = Vector128<T>.Zero;
			var numberVector = Vector128.Create(number);

			if ((uint)length % (uint)Vector128<T>.Count != 0)
			{
				resultVector = Vector128.Equals(Vector128.LoadUnsafe(ref first), resultVector);

				if (resultVector != Vector128<T>.Zero)
				{
					return true;
				}

				index += length % Vector128<T>.Count;
				length -= length % Vector128<T>.Count;
			}

			while (length >= Vector128<T>.Count * 4)
			{
				var vector1 = Vector128.LoadUnsafe(ref Unsafe.Add(ref first, index));
				var vector2 = Vector128.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector128<T>.Count));
				var vector3 = Vector128.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector128<T>.Count * 2));
				var vector4 = Vector128.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector128<T>.Count * 3));

				resultVector = Vector128.Add(
					Vector128.Add(Vector128.Equals(vector1, numberVector), Vector128.Equals(vector2, numberVector)),
					Vector128.Add(Vector128.Equals(vector3, numberVector), Vector128.Equals(vector4, numberVector)));

				if (resultVector != Vector128<T>.Zero)
				{
					return true;
				}

				index += Vector128<T>.Count * 4;
				length -= Vector128<T>.Count * 4;
			}

			while (length >= Vector128<T>.Count)
			{
				resultVector = Vector128.Equals(Vector128.LoadUnsafe(ref Unsafe.Add(ref first, index)), numberVector);

				if (resultVector != Vector128<T>.Zero)
				{
					return true;
				}

				index += Vector256<T>.Count;
				length -= Vector256<T>.Count;
			}

			return false;
		}

		if (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported)
		{
			var resultVector = Vector64<T>.Zero;
			var numberVector = Vector64.Create(number);

			if ((uint)length % (uint)Vector64<T>.Count != 0)
			{
				resultVector = Vector64.Equals(Vector64.LoadUnsafe(ref first), resultVector);

				if (resultVector != Vector64<T>.Zero)
				{
					return true;
				}

				index += length % Vector64<T>.Count;
				length -= length % Vector64<T>.Count;
			}

			while (length >= Vector64<T>.Count * 4)
			{
				var vector1 = Vector64.LoadUnsafe(ref Unsafe.Add(ref first, index));
				var vector2 = Vector64.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector64<T>.Count));
				var vector3 = Vector64.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector64<T>.Count * 2));
				var vector4 = Vector64.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector64<T>.Count * 3));

				resultVector = Vector64.Add(
					Vector64.Add(Vector64.Equals(vector1, numberVector), Vector64.Equals(vector2, numberVector)),
					Vector64.Add(Vector64.Equals(vector3, numberVector), Vector64.Equals(vector4, numberVector)));

				if (resultVector != Vector64<T>.Zero)
				{
					return true;
				}

				index += Vector64<T>.Count * 4;
				length -= Vector64<T>.Count * 4;
			}

			while (length >= Vector64<T>.Count)
			{
				resultVector = Vector64.Equals(Vector64.LoadUnsafe(ref Unsafe.Add(ref first, index)), numberVector);

				if (resultVector != Vector64<T>.Zero)
				{
					return true;
				}

				index += Vector256<T>.Count;
				length -= Vector256<T>.Count;
			}

			return false;
		}

		while (length > 0)
		{
			if (number.Equals(Unsafe.Add(ref first, index)))
			{
				return true;
			}

			length--;
			index++;
		}

		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
	internal static void CopyTo<T>(ref T source, ref T destination, int length) where T : struct, INumber<T>, IMinMaxValue<T>
	{
		nint index = 0;

		if (Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && length >= Vector256<T>.Count)
		{
			while (length >= Vector256<T>.Count)
			{
				Vector256
					.LoadUnsafe(ref Unsafe.Add(ref source, index))
					.StoreUnsafe(ref Unsafe.Add(ref destination, index));

				index += Vector256<T>.Count;
				length -= Vector256<T>.Count;
			}
		}

		if (Vector128.IsHardwareAccelerated && Vector128<T>.IsSupported && length >= Vector128<T>.Count)
		{
			while (length >= Vector128<T>.Count)
			{
				Vector128
					.LoadUnsafe(ref Unsafe.Add(ref source, index))
					.StoreUnsafe(ref Unsafe.Add(ref destination, index));

				index += Vector128<T>.Count;
				length -= Vector128<T>.Count;
			}
		}

		if (Vector64.IsHardwareAccelerated && Vector64<T>.IsSupported && length >= Vector64<T>.Count)
		{
			while (length >= Vector64<T>.Count)
			{
				Vector64
					.LoadUnsafe(ref Unsafe.Add(ref source, index))
					.StoreUnsafe(ref Unsafe.Add(ref destination, index));

				index += Vector64<T>.Count;
				length -= Vector64<T>.Count;
			}
		}

		while (length > 0)
		{
			Unsafe.Add(ref source, index) = Unsafe.Add(ref destination, index);

			length--;
			index++;
		}
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

		if (typeof(T) == typeof(Vector) && Fma.IsSupported)
		{
			var left = Unsafe.As<T, Vector128<float>>(ref a);
			var right = Unsafe.As<T, Vector128<float>>(ref b);
			var add = Unsafe.As<T, Vector128<float>>(ref minuend);

			return (T)(object)Fma.MultiplySubtractScalar(add, left, right);
		}

		return a * b - minuend;
	}

	#endregion
}