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
	internal static T Min<T>(ref T first, int length) where T : struct, INumber<T>, IMinMaxValue<T>
	{
		nint index = 0;
		var min = T.MaxValue;
		
		if (Vector256IsSupported<T>())
		{
			var resultVector = Vector256.Create(T.MaxValue);

			while (length >= Vector256<T>.Count * 4)
			{
				var vector1 = Vector256.LoadUnsafe(ref Unsafe.Add(ref first, index));
				var vector2 = Vector256.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector256<T>.Count));
				var vector3 = Vector256.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector256<T>.Count * 2));
				var vector4 = Vector256.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector256<T>.Count * 3));
				
				resultVector = Vector256.Min(vector1, resultVector);
				resultVector = Vector256.Min(vector2, resultVector);
				resultVector = Vector256.Min(vector3, resultVector);
				resultVector = Vector256.Min(vector4, resultVector);

				index += Vector256<T>.Count * 4;
				length -= Vector256<T>.Count * 4;
			}

			while (length >= Vector256<T>.Count)
			{
				resultVector = Vector256.Min(Vector256.LoadUnsafe(ref Unsafe.Add(ref first, index)), resultVector);

				index += Vector256<T>.Count;
				length -= Vector256<T>.Count;
			}

			for (var i = 0; i < Vector256<T>.Count; i++)
				min = Min(min, resultVector.GetElement(i));
		}

		if (Vector128IsSupported<T>())
		{
			var resultVector = Vector128.Create(T.MaxValue);

			while (length >= Vector128<T>.Count * 4)
			{
				var vector1 = Vector128.LoadUnsafe(ref Unsafe.Add(ref first, index));
				var vector2 = Vector128.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector128<T>.Count));
				var vector3 = Vector128.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector128<T>.Count * 2));
				var vector4 = Vector128.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector128<T>.Count * 3));

				resultVector = Vector128.Min(vector1, resultVector);
				resultVector = Vector128.Min(vector2, resultVector);
				resultVector = Vector128.Min(vector3, resultVector);
				resultVector = Vector128.Min(vector4, resultVector);

				index += Vector128<T>.Count * 4;
				length -= Vector128<T>.Count * 4;
			}

			while (length >= Vector128<T>.Count)
			{
				resultVector = Vector128.Min(Vector128.LoadUnsafe(ref Unsafe.Add(ref first, index)), resultVector);

				index += Vector128<T>.Count;
				length -= Vector128<T>.Count;
			}

			for (var i = 0; i < Vector128<T>.Count; i++)
				min = Min(min, resultVector.GetElement(i));
		}

		if (Vector64IsSupported<T>())
		{
			var resultVector = Vector64.Create(T.MaxValue);

			while (length >= Vector64<T>.Count * 4)
			{
				var vector1 = Vector64.LoadUnsafe(ref Unsafe.Add(ref first, index));
				var vector2 = Vector64.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector64<T>.Count));
				var vector3 = Vector64.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector64<T>.Count * 2));
				var vector4 = Vector64.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector64<T>.Count * 3));

				resultVector = Vector64.Min(vector1, resultVector);
				resultVector = Vector64.Min(vector2, resultVector);
				resultVector = Vector64.Min(vector3, resultVector);
				resultVector = Vector64.Min(vector4, resultVector);

				index += Vector64<T>.Count * 4;
				length -= Vector64<T>.Count * 4;
			}

			while (length >= Vector64<T>.Count)
			{
				resultVector = Vector64.Min(Vector64.LoadUnsafe(ref Unsafe.Add(ref first, index)), resultVector);

				index += Vector64<T>.Count;
				length -= Vector64<T>.Count;
			}

			for (var i = 0; i < Vector64<T>.Count; i++)
				min = Min(min, resultVector.GetElement(i));
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
	internal static T Max<T>(ref T first, int length) where T : struct, INumber<T>, IMinMaxValue<T>
	{
		nint index = 0;
		var max = T.MinValue;
		
		if (Vector256IsSupported<T>())
		{
			var resultVector = Vector256.Create(T.MaxValue);

			while (length >= Vector256<T>.Count * 4)
			{
				var vector1 = Vector256.LoadUnsafe(ref Unsafe.Add(ref first, index));
				var vector2 = Vector256.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector256<T>.Count));
				var vector3 = Vector256.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector256<T>.Count * 2));
				var vector4 = Vector256.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector256<T>.Count * 3));
				
				resultVector = Vector256.Max(vector1, resultVector);
				resultVector = Vector256.Max(vector2, resultVector);
				resultVector = Vector256.Max(vector3, resultVector);
				resultVector = Vector256.Max(vector4, resultVector);

				index += Vector256<T>.Count * 4;
				length -= Vector256<T>.Count * 4;
			}

			while (length >= Vector256<T>.Count)
			{
				resultVector = Vector256.Max(Vector256.LoadUnsafe(ref Unsafe.Add(ref first, index)), resultVector);

				index += Vector256<T>.Count;
				length -= Vector256<T>.Count;
			}

			for (var i = 0; i < Vector256<T>.Count; i++)
				max = Max(max, resultVector.GetElement(i));
		}

		if (Vector128IsSupported<T>())
		{
			var resultVector = Vector128.Create(T.MaxValue);

			while (length >= Vector128<T>.Count * 4)
			{
				var vector1 = Vector128.LoadUnsafe(ref Unsafe.Add(ref first, index));
				var vector2 = Vector128.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector128<T>.Count));
				var vector3 = Vector128.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector128<T>.Count * 2));
				var vector4 = Vector128.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector128<T>.Count * 3));

				resultVector = Vector128.Max(vector1, resultVector);
				resultVector = Vector128.Max(vector2, resultVector);
				resultVector = Vector128.Max(vector3, resultVector);
				resultVector = Vector128.Max(vector4, resultVector);

				index += Vector128<T>.Count * 4;
				length -= Vector128<T>.Count * 4;
			}

			while (length >= Vector128<T>.Count)
			{
				resultVector = Vector128.Max(Vector128.LoadUnsafe(ref Unsafe.Add(ref first, index)), resultVector);

				index += Vector128<T>.Count;
				length -= Vector128<T>.Count;
			}

			for (var i = 0; i < Vector128<T>.Count; i++)
				max = Max(max, resultVector.GetElement(i));
		}

		if (Vector64IsSupported<T>())
		{
			var resultVector = Vector64.Create(T.MaxValue);

			while (length >= Vector64<T>.Count * 4)
			{
				var vector1 = Vector64.LoadUnsafe(ref Unsafe.Add(ref first, index));
				var vector2 = Vector64.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector64<T>.Count));
				var vector3 = Vector64.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector64<T>.Count * 2));
				var vector4 = Vector64.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector64<T>.Count * 3));

				resultVector = Vector64.Max(vector1, resultVector);
				resultVector = Vector64.Max(vector2, resultVector);
				resultVector = Vector64.Max(vector3, resultVector);
				resultVector = Vector64.Max(vector4, resultVector);

				index += Vector64<T>.Count * 2;
				length -= Vector64<T>.Count * 2;
			}

			while (length >= Vector64<T>.Count)
			{
				resultVector = Vector64.Max(Vector64.LoadUnsafe(ref Unsafe.Add(ref first, index)), resultVector);

				index += Vector64<T>.Count;
				length -= Vector64<T>.Count;
			}

			for (var i = 0; i < Vector64<T>.Count; i++)
				max = Max(max, resultVector.GetElement(i));
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
		var pointer = (T*)Unsafe.AsPointer(ref first);

		if (Vector256.IsHardwareAccelerated)
		{
			var resultVector = Vector256<T>.Zero;
			
			while (length >= Vector256<T>.Count * 4)
			{
				var vector1 = Vector256.LoadUnsafe(ref Unsafe.Add(ref first, index));
				var vector2 = Vector256.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector256<T>.Count));
				var vector3 = Vector256.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector256<T>.Count * 2));
				var vector4 = Vector256.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector256<T>.Count * 3));

				resultVector += vector1;
				resultVector += vector2;
				resultVector += vector3;
				resultVector += vector4;

				index += Vector256<T>.Count * 4;
				length -= Vector256<T>.Count * 4;
			}

			while (length > Vector256<T>.Count)
			{
				resultVector += Vector256.LoadUnsafe(ref Unsafe.Add(ref first, index));

				index += Vector256<T>.Count;
				length -= Vector256<T>.Count;
			}

			resultVector += Vector256.LoadAligned(pointer + index);

			return Vector256.Sum(resultVector);
		}

		if (Vector128.IsHardwareAccelerated)
		{
			var resultVector = Vector128<T>.Zero;
		
			while (length >= Vector128<T>.Count * 4)
			{
				var vector1 = Vector128.LoadUnsafe(ref Unsafe.Add(ref first, index));
				var vector2 = Vector128.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector128<T>.Count));
				var vector3 = Vector128.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector128<T>.Count * 2));
				var vector4 = Vector128.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector128<T>.Count * 3));
		
				resultVector += vector1;
				resultVector += vector2;
				resultVector += vector3;
				resultVector += vector4;
		
				index += Vector128<T>.Count * 4;
				length -= Vector128<T>.Count * 4;
			}
		
			while (length > Vector128<T>.Count)
			{
				resultVector += Vector128.LoadUnsafe(ref Unsafe.Add(ref first, index));
		
				index += Vector128<T>.Count;
				length -= Vector128<T>.Count;
			}
		
			resultVector += Vector128.LoadAligned(pointer + index);
			
			return Vector128.Sum(resultVector);
		}
		
		if (Vector64.IsHardwareAccelerated)
		{
			var resultVector = Vector64<T>.Zero;
		
			while (length >= Vector64<T>.Count * 4)
			{
				var vector1 = Vector64.LoadUnsafe(ref Unsafe.Add(ref first, index));
				var vector2 = Vector64.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector64<T>.Count));
				var vector3 = Vector64.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector64<T>.Count * 2));
				var vector4 = Vector64.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector64<T>.Count * 3));
		
				resultVector += vector1;
				resultVector += vector2;
				resultVector += vector3;
				resultVector += vector4;
		
				index += Vector64<T>.Count * 4;
				length -= Vector64<T>.Count * 4;
			}
		
			while (length > Vector64<T>.Count)
			{
				resultVector += Vector64.LoadUnsafe(ref Unsafe.Add(ref first, index));
		
				index += Vector64<T>.Count;
				length -= Vector64<T>.Count;
			}
		
			resultVector += Vector64.LoadAligned(pointer + index);
		
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
		
		if (Vector256IsSupported<T>())
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

		if (Vector128IsSupported<T>())
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

		if (Vector64IsSupported<T>())
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
		
		if (Vector256IsSupported<T>())
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

		if (Vector128IsSupported<T>())
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

		if (Vector64IsSupported<T>())
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
		
		if (Vector256IsSupported<T>())
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

		if (Vector128IsSupported<T>())
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

		if (Vector64IsSupported<T>())
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
		
		if (Vector256IsSupported<T>())
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

		if (Vector128IsSupported<T>())
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

		if (Vector64IsSupported<T>())
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
	/// calculate the square root of the numbers
	/// </summary>
	/// <param name="first">reference to the first element of the numbers to get the square root of</param>
	/// <param name="length">the length of the numbers</param>
	[MethodImpl(MethodImplOptions.AggressiveOptimization)]
	internal static void Sqrt<T>(ref T first, int length) where T : struct, IRootFunctions<T>
	{
		nint index = 0;
		
		if (Vector256IsSupported<T>())
		{
			while (length >= Vector256<T>.Count * 2)
			{
				var vector1 = Vector256.LoadUnsafe(ref Unsafe.Add(ref first, index));
				var vector2 = Vector256.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector256<T>.Count));
				
				var resultVector1 = Vector256.Sqrt(vector1);
				var resultVector2 = Vector256.Sqrt(vector2);
				
				resultVector1.StoreUnsafe(ref Unsafe.Add(ref first, index));
				resultVector2.StoreUnsafe(ref Unsafe.Add(ref first, index + Vector256<T>.Count));

				index += Vector256<T>.Count * 2;
				length -= Vector256<T>.Count * 2;
			}

			while (length >= Vector256<T>.Count)
			{
				Vector256.Sqrt(Vector256.LoadUnsafe(ref Unsafe.Add(ref first, index)))
					.StoreUnsafe(ref Unsafe.Add(ref first, index));

				index += Vector256<T>.Count;
				length -= Vector256<T>.Count;
			}
		}

		if (Vector128IsSupported<T>())
		{
			while (length >= Vector128<T>.Count * 2)
			{
				var vector1 = Vector128.LoadUnsafe(ref Unsafe.Add(ref first, index));
				var vector2 = Vector128.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector128<T>.Count));

				var resultVector1 = Vector128.Sqrt(vector1);
				var resultVector2 = Vector128.Sqrt(vector2);

				resultVector1.StoreUnsafe(ref Unsafe.Add(ref first, index));
				resultVector2.StoreUnsafe(ref Unsafe.Add(ref first, index + Vector128<T>.Count));

				index += Vector128<T>.Count * 2;
				length -= Vector128<T>.Count * 2;
			}

			while (length >= Vector128<T>.Count)
			{
				Vector128.Sqrt(Vector128.LoadUnsafe(ref Unsafe.Add(ref first, index)))
					.StoreUnsafe(ref Unsafe.Add(ref first, index));

				index += Vector128<T>.Count;
				length -= Vector128<T>.Count;
			}
		}

		if (Vector64IsSupported<T>())
		{
			while (length >= Vector64<T>.Count * 2)
			{
				var vector1 = Vector64.LoadUnsafe(ref Unsafe.Add(ref first, index));
				var vector2 = Vector64.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector128<T>.Count));

				var resultVector1 = Vector64.Sqrt(vector1);
				var resultVector2 = Vector64.Sqrt(vector2);

				resultVector1.StoreUnsafe(ref Unsafe.Add(ref first, index));
				resultVector2.StoreUnsafe(ref Unsafe.Add(ref first, index + Vector128<T>.Count));

				index += Vector256<T>.Count * 2;
				length -= Vector256<T>.Count * 2;
			}

			while (length >= Vector64<T>.Count)
			{
				Vector64.Sqrt(Vector64.LoadUnsafe(ref Unsafe.Add(ref first, index)))
					.StoreUnsafe(ref Unsafe.Add(ref first, index));

				index += Vector64<T>.Count;
				length -= Vector64<T>.Count;
			}
		}

		while (length > 0)
		{
			Unsafe.Add(ref first, index) = Sqrt(Unsafe.Add(ref first, index));

			length--;
			index++;
		}
	}

	/// <summary>
	/// calculate the square of the numbers
	/// </summary>
	/// <param name="first">reference to the first element of the numbers to get the square root of</param>
	/// <param name="length">the length of the numbers</param>
	[MethodImpl(MethodImplOptions.AggressiveOptimization)]
	internal static void Sq<T>(ref T first, int length) where T : struct, IMultiplyOperators<T, T, T>
	{
		nint index = 0;
		
		if (Vector256IsSupported<T>())
		{
			while (length >= Vector256<T>.Count * 2)
			{
				var vector1 = Vector256.LoadUnsafe(ref Unsafe.Add(ref first, index));
				var vector2 = Vector256.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector256<T>.Count));
				
				var resultVector1 = Vector256.Multiply(vector1, vector1);
				var resultVector2 = Vector256.Multiply(vector2, vector2);
				
				resultVector1.StoreUnsafe(ref Unsafe.Add(ref first, index));
				resultVector2.StoreUnsafe(ref Unsafe.Add(ref first, index + Vector256<T>.Count));

				index += Vector256<T>.Count * 2;
				length -= Vector256<T>.Count * 2;
			}

			while (length >= Vector256<T>.Count)
			{
				var vector1 = Vector256.LoadUnsafe(ref Unsafe.Add(ref first, index));
				
				Vector256.Multiply(vector1, vector1)
					.StoreUnsafe(ref Unsafe.Add(ref first, index));

				index += Vector256<T>.Count;
				length -= Vector256<T>.Count;
			}
		}

		if (Vector128IsSupported<T>())
		{
			while (length >= Vector128<T>.Count * 2)
			{
				var vector1 = Vector128.LoadUnsafe(ref Unsafe.Add(ref first, index));
				var vector2 = Vector128.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector128<T>.Count));

				var resultVector1 = Vector128.Multiply(vector1, vector1);
				var resultVector2 = Vector128.Multiply(vector2, vector2);

				resultVector1.StoreUnsafe(ref Unsafe.Add(ref first, index));
				resultVector2.StoreUnsafe(ref Unsafe.Add(ref first, index + Vector128<T>.Count));

				index += Vector128<T>.Count * 2;
				length -= Vector128<T>.Count * 2;
			}

			while (length >= Vector128<T>.Count)
			{
				var vector1 = Vector128.LoadUnsafe(ref Unsafe.Add(ref first, index));

				Vector128.Multiply(vector1, vector1)
					.StoreUnsafe(ref Unsafe.Add(ref first, index));

				index += Vector128<T>.Count;
				length -= Vector128<T>.Count;
			}
		}

		if (Vector64IsSupported<T>())
		{
			while (length >= Vector64<T>.Count * 2)
			{
				var vector1 = Vector64.LoadUnsafe(ref Unsafe.Add(ref first, index));
				var vector2 = Vector64.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector64<T>.Count));

				var resultVector1 = Vector64.Multiply(vector1, vector1);
				var resultVector2 = Vector64.Multiply(vector2, vector2);

				resultVector1.StoreUnsafe(ref Unsafe.Add(ref first, index));
				resultVector2.StoreUnsafe(ref Unsafe.Add(ref first, index + Vector64<T>.Count));

				index += Vector64<T>.Count * 2;
				length -= Vector64<T>.Count * 2;
			}

			while (length >= Vector64<T>.Count)
			{
				var vector1 = Vector64.LoadUnsafe(ref Unsafe.Add(ref first, index));

				Vector64.Multiply(vector1, vector1)
					.StoreUnsafe(ref Unsafe.Add(ref first, index));

				index += Vector64<T>.Count;
				length -= Vector64<T>.Count;
			}
		}

		while (length > 0)
		{
			Unsafe.Add(ref first, index) = Sq(Unsafe.Add(ref first, index));

			length--;
			index++;
		}
	}

	/// <summary>
	/// calculate the absolute value of the numbers
	/// </summary>
	/// <param name="first">reference to the first element of the numbers to get the absolute value of</param>
	/// <param name="length">the length of the numbers</param>
	[MethodImpl(MethodImplOptions.AggressiveOptimization)]
	internal static void Abs<T>(ref T first, int length) where T : struct, INumber<T>
	{
		nint index = 0;
		
		if (Vector256IsSupported<T>())
		{
			while (length >= Vector256<T>.Count * 2)
			{
				var vector1 = Vector256.LoadUnsafe(ref Unsafe.Add(ref first, index));
				var vector2 = Vector256.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector256<T>.Count));
				
				var resultVector1 = Vector256.Abs(vector1);
				var resultVector2 = Vector256.Abs(vector2);
				
				resultVector1.StoreUnsafe(ref Unsafe.Add(ref first, index));
				resultVector2.StoreUnsafe(ref Unsafe.Add(ref first, index + Vector256<T>.Count));

				index += Vector256<T>.Count * 2;
				length -= Vector256<T>.Count * 2;
			}

			while (length >= Vector256<T>.Count)
			{
				Vector256.Abs(Vector256.LoadUnsafe(ref Unsafe.Add(ref first, index)))
					.StoreUnsafe(ref Unsafe.Add(ref first, index));

				index += Vector256<T>.Count;
				length -= Vector256<T>.Count;
			}
		}

		if (Vector128IsSupported<T>())
		{
			while (length >= Vector128<T>.Count * 2)
			{
				var vector1 = Vector128.LoadUnsafe(ref Unsafe.Add(ref first, index));
				var vector2 = Vector128.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector128<T>.Count));

				var resultVector1 = Vector128.Abs(vector1);
				var resultVector2 = Vector128.Abs(vector2);

				resultVector1.StoreUnsafe(ref Unsafe.Add(ref first, index));
				resultVector2.StoreUnsafe(ref Unsafe.Add(ref first, index + Vector128<T>.Count));

				index += Vector128<T>.Count * 2;
				length -= Vector128<T>.Count * 2;
			}

			while (length >= Vector128<T>.Count)
			{
				Vector128.Abs(Vector128.LoadUnsafe(ref Unsafe.Add(ref first, index)))
					.StoreUnsafe(ref Unsafe.Add(ref first, index));

				index += Vector128<T>.Count;
				length -= Vector128<T>.Count;
			}
		}

		if (Vector64IsSupported<T>())
		{
			while (length >= Vector64<T>.Count * 2)
			{
				var vector1 = Vector64.LoadUnsafe(ref Unsafe.Add(ref first, index));
				var vector2 = Vector64.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector64<T>.Count));

				var resultVector1 = Vector64.Abs(vector1);
				var resultVector2 = Vector64.Abs(vector2);
				
				resultVector1.StoreUnsafe(ref Unsafe.Add(ref first, index));
				resultVector2.StoreUnsafe(ref Unsafe.Add(ref first, index + Vector64<T>.Count));

				index += Vector64<T>.Count * 2;
				length -= Vector64<T>.Count * 2;
			}

			while (length >= Vector64<T>.Count)
			{
				Vector64.Abs(Vector64.LoadUnsafe(ref Unsafe.Add(ref first, index)))
					.StoreUnsafe(ref Unsafe.Add(ref first, index));

				index += Vector64<T>.Count;
				length -= Vector64<T>.Count;
			}
		}

		while (length > 0)
		{
			Unsafe.Add(ref first, index) = Abs(Unsafe.Add(ref first, index));

			length--;
			index++;
		}
	}

	/// <summary>
	/// calculate the absolute value of the numbers
	/// </summary>
	/// <param name="first">reference to the first element of the numbers to get the absolute value of</param>
	/// <param name="length">the length of the numbers</param>
	[MethodImpl(MethodImplOptions.AggressiveOptimization)]
	internal static void Floor(ref float first, int length)
	{
		nint index = 0;
		
		if (Vector256.IsHardwareAccelerated)
		{
			while (length >= Vector256<float>.Count * 2)
			{
				var vector1 = Vector256.LoadUnsafe(ref Unsafe.Add(ref first, index));
				var vector2 = Vector256.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector256<float>.Count));
				
				var resultVector1 = Vector256.Floor(vector1);
				var resultVector2 = Vector256.Floor(vector2);
				
				resultVector1.StoreUnsafe(ref Unsafe.Add(ref first, index));
				resultVector2.StoreUnsafe(ref Unsafe.Add(ref first, index + Vector256<float>.Count));

				index += Vector256<float>.Count * 2;
				length -= Vector256<float>.Count * 2;
			}

			while (length >= Vector256<float>.Count)
			{
				Vector256.Floor(Vector256.LoadUnsafe(ref Unsafe.Add(ref first, index)))
					.StoreUnsafe(ref Unsafe.Add(ref first, index));

				index += Vector256<float>.Count;
				length -= Vector256<float>.Count;
			}
		}

		if (Vector128.IsHardwareAccelerated)
		{
			while (length >= Vector128<float>.Count * 2)
			{
				var vector1 = Vector128.LoadUnsafe(ref Unsafe.Add(ref first, index));
				var vector2 = Vector128.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector256<float>.Count));

				var resultVector1 = Vector128.Floor(vector1);
				var resultVector2 = Vector128.Floor(vector2);

				resultVector1.StoreUnsafe(ref Unsafe.Add(ref first, index));
				resultVector2.StoreUnsafe(ref Unsafe.Add(ref first, index + Vector256<float>.Count));

				index += Vector128<float>.Count * 2;
				length -= Vector128<float>.Count * 2;
			}

			while (length >= Vector128<float>.Count)
			{
				Vector128.Floor(Vector128.LoadUnsafe(ref Unsafe.Add(ref first, index)))
					.StoreUnsafe(ref Unsafe.Add(ref first, index));

				index += Vector128<float>.Count;
				length -= Vector128<float>.Count;
			}
		}

		if (Vector64.IsHardwareAccelerated)
		{
			while (length >= Vector64<float>.Count * 2)
			{
				var vector1 = Vector64.LoadUnsafe(ref Unsafe.Add(ref first, index));
				var vector2 = Vector64.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector256<float>.Count));

				var resultVector1 = Vector64.Floor(vector1);
				var resultVector2 = Vector64.Floor(vector2);

				resultVector1.StoreUnsafe(ref Unsafe.Add(ref first, index));
				resultVector2.StoreUnsafe(ref Unsafe.Add(ref first, index + Vector256<float>.Count));

				index += Vector64<float>.Count * 2;
				length -= Vector64<float>.Count * 2;
			}

			while (length >= Vector64<float>.Count)
			{
				Vector64.Floor(Vector64.LoadUnsafe(ref Unsafe.Add(ref first, index)))
					.StoreUnsafe(ref Unsafe.Add(ref first, index));

				index += Vector64<float>.Count;
				length -= Vector64<float>.Count;
			}
		}

		while (length > 0)
		{
			Unsafe.Add(ref first, index) = MathF.Floor(Unsafe.Add(ref first, index));

			length--;
			index++;
		}
	}

	/// <summary>
	/// calculate the absolute value of the numbers
	/// </summary>
	/// <param name="first">reference to the first element of the numbers to get the absolute value of</param>
	/// <param name="length">the length of the numbers</param>
	[MethodImpl(MethodImplOptions.AggressiveOptimization)]
	internal static void Floor(ref double first, int length)
	{
		nint index = 0;
		
		if (Vector256.IsHardwareAccelerated)
		{
			while (length >= Vector256<double>.Count * 2)
			{
				var vector1 = Vector256.LoadUnsafe(ref Unsafe.Add(ref first, index));
				var vector2 = Vector256.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector256<double>.Count));
				
				var resultVector1 = Vector256.Floor(vector1);
				var resultVector2 = Vector256.Floor(vector2);
				
				resultVector1.StoreUnsafe(ref Unsafe.Add(ref first, index));
				resultVector2.StoreUnsafe(ref Unsafe.Add(ref first, index + Vector256<double>.Count));

				index += Vector256<double>.Count * 2;
				length -= Vector256<double>.Count * 2;
			}

			while (length >= Vector256<double>.Count)
			{
				Vector256.Floor(Vector256.LoadUnsafe(ref Unsafe.Add(ref first, index)))
					.StoreUnsafe(ref Unsafe.Add(ref first, index));

				index += Vector256<double>.Count;
				length -= Vector256<double>.Count;
			}
		}

		if (Vector128.IsHardwareAccelerated)
		{
			while (length >= Vector128<double>.Count * 2)
			{
				var vector1 = Vector128.LoadUnsafe(ref Unsafe.Add(ref first, index));
				var vector2 = Vector128.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector256<double>.Count));

				var resultVector1 = Vector128.Floor(vector1);
				var resultVector2 = Vector128.Floor(vector2);

				resultVector1.StoreUnsafe(ref Unsafe.Add(ref first, index));
				resultVector2.StoreUnsafe(ref Unsafe.Add(ref first, index + Vector256<double>.Count));

				index += Vector128<double>.Count * 2;
				length -= Vector128<double>.Count * 2;
			}

			while (length >= Vector128<double>.Count)
			{
				Vector128.Floor(Vector128.LoadUnsafe(ref Unsafe.Add(ref first, index)))
					.StoreUnsafe(ref Unsafe.Add(ref first, index));

				index += Vector128<double>.Count;
				length -= Vector128<double>.Count;
			}
		}

		while (length > 0)
		{
			Unsafe.Add(ref first, index) = System.Math.Floor(Unsafe.Add(ref first, index));

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
		
		if (Vector256IsSupported<T>())
		{
			var resultVector = Vector256<T>.Zero;
			var numberVector = Vector256.Create(number);

			while (length >= Vector256<T>.Count * 2)
			{
				var vector1 = Vector256.LoadUnsafe(ref Unsafe.Add(ref first, index));
				var vector2 = Vector256.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector256<T>.Count));
				
				resultVector += Vector256.Equals(vector1, numberVector);
				resultVector += Vector256.Equals(vector2, numberVector);

				index += Vector256<T>.Count * 2;
				length -= Vector256<T>.Count * 2;
			}

			while (length >= Vector256<T>.Count)
			{
				resultVector = Vector256.Add(Vector256.LoadUnsafe(ref Unsafe.Add(ref first, index)), resultVector);

				index += Vector256<T>.Count;
				length -= Vector256<T>.Count;
			}

			result += Vector256.Sum(resultVector);
		}

		if (Vector128IsSupported<T>())
		{
			var resultVector = Vector128<T>.Zero;
			var numberVector = Vector128.Create(number);

			while (length >= Vector128<T>.Count * 2)
			{
				var vector1 = Vector128.LoadUnsafe(ref Unsafe.Add(ref first, index));
				var vector2 = Vector128.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector128<T>.Count));

				resultVector += Vector128.Equals(vector1, numberVector);
				resultVector += Vector128.Equals(vector2, numberVector);

				index += Vector128<T>.Count * 2;
				length -= Vector128<T>.Count * 2;
			}

			while (length >= Vector128<T>.Count)
			{
				resultVector = Vector128.Add(Vector128.LoadUnsafe(ref Unsafe.Add(ref first, index)), resultVector);

				index += Vector128<T>.Count;
				length -= Vector128<T>.Count;
			}

			result += Vector128.Sum(resultVector);
		}

		if (Vector64IsSupported<T>())
		{
			var resultVector = Vector64<T>.Zero;
			var numberVector = Vector64.Create(number);

			while (length >= Vector64<T>.Count * 2)
			{
				var vector1 = Vector64.LoadUnsafe(ref Unsafe.Add(ref first, index));
				var vector2 = Vector64.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector64<T>.Count));

				resultVector += Vector64.Equals(vector1, numberVector);
				resultVector += Vector64.Equals(vector2, numberVector);

				index += Vector64<T>.Count * 2;
				length -= Vector64<T>.Count * 2;
			}

			while (length >= Vector64<T>.Count)
			{
				resultVector = Vector64.Add(Vector64.LoadUnsafe(ref Unsafe.Add(ref first, index)), resultVector);

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
		
		if (Vector256IsSupported<T>())
		{
			var resultVector = Vector256<T>.Zero;
			var numberVector = Vector256.Create(number);

			while (length >= Vector256<T>.Count * 2)
			{
				var vector1 = Vector256.LoadUnsafe(ref Unsafe.Add(ref first, index));
				var vector2 = Vector256.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector256<T>.Count));
				
				resultVector += Vector256.Equals(vector1, numberVector);
				resultVector += Vector256.Equals(vector2, numberVector);

				index += Vector256<T>.Count * 2;
				length -= Vector256<T>.Count * 2;
			}

			while (length >= Vector256<T>.Count)
			{
				resultVector = Vector256.Add(Vector256.LoadUnsafe(ref Unsafe.Add(ref first, index)), resultVector);

				index += Vector256<T>.Count;
				length -= Vector256<T>.Count;
			}

			if (resultVector != Vector256<T>.Zero)
			{
				return true;
			}
		}

		if (Vector128IsSupported<T>())
		{
			var resultVector = Vector128<T>.Zero;
			var numberVector = Vector128.Create(number);

			while (length >= Vector128<T>.Count * 2)
			{
				var vector1 = Vector128.LoadUnsafe(ref Unsafe.Add(ref first, index));
				var vector2 = Vector128.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector128<T>.Count));

				resultVector += Vector128.Equals(vector1, numberVector);
				resultVector += Vector128.Equals(vector2, numberVector);

				index += Vector128<T>.Count * 2;
				length -= Vector128<T>.Count * 2;
			}

			while (length >= Vector128<T>.Count)
			{
				resultVector = Vector128.Add(Vector128.LoadUnsafe(ref Unsafe.Add(ref first, index)), resultVector);

				index += Vector128<T>.Count;
				length -= Vector128<T>.Count;
			}

			if (resultVector != Vector128<T>.Zero)
			{
				return true;
			}
		}

		if (Vector64IsSupported<T>())
		{
			var resultVector = Vector64<T>.Zero;
			var numberVector = Vector64.Create(number);

			while (length >= Vector64<T>.Count * 2)
			{
				var vector1 = Vector64.LoadUnsafe(ref Unsafe.Add(ref first, index));
				var vector2 = Vector64.LoadUnsafe(ref Unsafe.Add(ref first, index + Vector64<T>.Count));

				resultVector += Vector64.Equals(vector1, numberVector);
				resultVector += Vector64.Equals(vector2, numberVector);

				index += Vector64<T>.Count * 2;
				length -= Vector64<T>.Count * 2;
			}

			while (length >= Vector64<T>.Count)
			{
				resultVector = Vector64.Add(Vector64.LoadUnsafe(ref Unsafe.Add(ref first, index)), resultVector);

				index += Vector64<T>.Count;
				length -= Vector64<T>.Count;
			}

			if (resultVector != Vector64<T>.Zero)
			{
				return true;
			}
		}

		while (length > 0)
		{
			if (Unsafe.Add(ref first, index).Equals(number))
			{
				return true;
			}

			length--;
			index++;
		}

		return false;
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