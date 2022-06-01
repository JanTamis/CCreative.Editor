using System.Numerics;

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

	public static int Count<T>(this T[] numbers, T number) where T : struct, INumber<T>
	{
		return Count(ref GetReference(numbers), numbers.Length, number);
	}

	public static bool Contains<T>(this T[] numbers, T number) where T : struct, IEqualityOperators<T, T>
	{
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
}