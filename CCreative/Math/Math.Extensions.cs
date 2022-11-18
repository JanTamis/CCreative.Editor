using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using SixLabors.ImageSharp.Processing;

namespace CCreative;

public static partial class Math
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Sum<T>(this T[] numbers) where T : unmanaged, INumberBase<T>
	{
		return Sum(ref GetReference(numbers), numbers.Length);
	}

	public static T Average<T>(this T[] numbers) where T : unmanaged, INumber<T>
	{
		return Sum(numbers) / ConvertNumber<int, T>(numbers.Length);
	}

	public static int Count<T>(this T[] numbers, T number) where T : IEquatable<T>
	{
		ArgumentNullException.ThrowIfNull(numbers);

		var length = numbers!.Length;
		ref var pointer = ref GetReference(numbers);

		return Unsafe.SizeOf<T>() switch
		{
			sizeof(byte) => Count(ref Unsafe.As<T, byte>(ref pointer), length, Unsafe.As<T, byte>(ref number)),
			sizeof(short) => Count(ref Unsafe.As<T, short>(ref pointer), length, Unsafe.As<T, short>(ref number)),
			sizeof(int) => Count(ref Unsafe.As<T, int>(ref pointer), length, Unsafe.As<T, int>(ref number)),
			sizeof(long) => Count(ref Unsafe.As<T, long>(ref pointer), length, Unsafe.As<T, long>(ref number)),
			_ => SoftwareFallback(ref pointer),
		};

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		int SoftwareFallback(ref T pointer)
		{
			var index = 0;
			var count = 0;

			while (index < length)
			{
				if (pointer.Equals(number))
				{
					count++;
				}

				pointer = ref Unsafe.Add(ref pointer, (IntPtr)Unsafe.SizeOf<T>());
				index++;
			}

			return count;
		}
	}

	public static bool Contains<T>(this T[] numbers, T number) where T : IEquatable<T>
	{
		ArgumentNullException.ThrowIfNull(numbers);

		var length = numbers!.Length;
		ref var pointer = ref GetReference(numbers);

		return Unsafe.SizeOf<T>() switch
		{
			sizeof(byte) => Contains(ref Unsafe.As<T, byte>(ref pointer), length, Unsafe.As<T, byte>(ref number)),
			sizeof(short) => Contains(ref Unsafe.As<T, short>(ref pointer), length, Unsafe.As<T, short>(ref number)),
			sizeof(int) => Contains(ref Unsafe.As<T, int>(ref pointer), length, Unsafe.As<T, int>(ref number)),
			sizeof(long) => Contains(ref Unsafe.As<T, long>(ref pointer), length, Unsafe.As<T, long>(ref number)),
			_ => SoftwareFallback(ref pointer),
		};

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		bool SoftwareFallback(ref T pointer)
		{
			var index = 0;

			while (length > 0)
			{
				if (number.Equals(Unsafe.Add(ref pointer, index)))
				{
					return true;
				}

				index++;
				length--;
			}

			return false;
		}
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
}