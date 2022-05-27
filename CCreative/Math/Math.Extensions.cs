using System.Numerics;

namespace CCreative;

public static partial class Math
{
	public static T Sum<T>(this T[] numbers) where T : unmanaged, INumber<T>
	{
		return Sum(ref GetReference(numbers), numbers.Length);
	}

	public static int Count<T>(this T[] numbers, T number) where T : unmanaged, INumber<T>
	{
		return Count(ref GetReference(numbers), numbers.Length, number);
	}
}