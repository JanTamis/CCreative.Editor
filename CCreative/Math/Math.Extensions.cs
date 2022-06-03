﻿using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace CCreative;

public static partial class Math
{
	public static T Sum<T>(this T[] numbers) where T : struct, INumber<T>
	{
		return Sum(ref GetReference(numbers), numbers.Length);
	}

	public static T Average<T>(this T[] numbers) where T : struct, INumber<T>
	{
		return Sum(numbers) / ConvertNumber<int, T>(numbers.Length);
	}

	public static int Count<T>(this T[] numbers, T number) where T : struct, IEquatable<T>
	{
		if (Unsafe.SizeOf<byte>() == sizeof(byte))
		{
			return Count(ref Unsafe.As<T, byte>(ref GetReference(numbers)), numbers.Length, Unsafe.As<T, byte>(ref number));
		}

		if (Unsafe.SizeOf<T>() == sizeof(ushort))
		{
			return Count(ref Unsafe.As<T, ushort>(ref GetReference(numbers)), numbers.Length, Unsafe.As<T, ushort>(ref number));
		}

		if (Unsafe.SizeOf<T>() == sizeof(uint))
		{
			return Count(ref Unsafe.As<T, uint>(ref GetReference(numbers)), numbers.Length, Unsafe.As<T, uint>(ref number));
		}

		if (Unsafe.SizeOf<T>() == sizeof(ulong))
		{
			return Count(ref Unsafe.As<T, ulong>(ref GetReference(numbers)), numbers.Length, Unsafe.As<T, ulong>(ref number));
		}

		var index = 0;
		var count = 0;
		
		ref var first = ref GetReference(numbers);

		while (index < numbers.Length)
		{
			if (first.Equals(number))
			{
				count++;
			}

			first = ref Unsafe.Add(ref first, (IntPtr)Unsafe.SizeOf<T>());
			index++;
		}

		return count;
	}

	public static bool Contains<T>(this T[] numbers, T number) where T : struct, IEquatable<T>
	{
		if (Unsafe.SizeOf<byte>() == sizeof(byte))
		{
			return Contains(ref Unsafe.As<T, byte>(ref GetReference(numbers)), numbers.Length, Unsafe.As<T, byte>(ref number));
		}
		if (Unsafe.SizeOf<T>() == sizeof(ushort))
		{
			return Contains(ref Unsafe.As<T, ushort>(ref GetReference(numbers)), numbers.Length, Unsafe.As<T, ushort>(ref number));
		}
		if (Unsafe.SizeOf<T>() == sizeof(uint))
		{
			return Contains(ref Unsafe.As<T, uint>(ref GetReference(numbers)), numbers.Length, Unsafe.As<T, uint>(ref number));
		}
		if (Unsafe.SizeOf<T>() == sizeof(ulong))
		{
			return Contains(ref Unsafe.As<T, ulong>(ref GetReference(numbers)), numbers.Length, Unsafe.As<T, ulong>(ref number));
		}
		
		return Contains(ref GetReference(numbers), numbers.Length, number);
	}

	public static T Min<T>(this T[] numbers) where T : struct, INumber<T>, IMinMaxValue<T>
	{
		return Min(ref GetReference(numbers), numbers.Length);
	}

	public static T Max<T>(this T[] numbers) where T : struct, INumber<T>, IMinMaxValue<T>
	{
		return Max(ref GetReference(numbers), numbers.Length);
	}

	public static T StandardDeviation<T>(this T[] numbers) where T : struct, INumber<T>, IRootFunctions<T>
	{
		return StandardDeviation(ref GetReference(numbers), numbers.Length);
	}

	public static void Add<T>(this T[] numbers, T number) where T : struct, IAdditionOperators<T, T, T>
	{
		Add(ref GetReference(numbers), numbers.Length, number);
	}

	public static void Subtract<T>(this T[] numbers, T number) where T : struct, ISubtractionOperators<T, T, T>
	{
		Subtract(ref GetReference(numbers), numbers.Length, number);
	}

	public static void Multiply<T>(this T[] numbers, T number) where T : struct, IMultiplyOperators<T, T, T>
	{
		Multiply(ref GetReference(numbers), numbers.Length, number);
	}

	public static void Divide<T>(this T[] numbers, T number) where T : struct, IDivisionOperators<T, T, T>
	{
		Divide(ref GetReference(numbers), numbers.Length, number);
	}

	public static void Fill<T>(this T[] numbers, T number) where T : struct
	{
		Fill(ref GetReference(numbers), numbers.Length, number);
	}
}