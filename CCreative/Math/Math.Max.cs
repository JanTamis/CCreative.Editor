using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace CCreative
{
	public static partial class Math
	{
		/// <summary>
		/// Determines the biggest value in a sequence of numbers
		/// </summary>
		/// <param name="numbers">array of numbers to compare</param>
		/// <returns>returns the maximum value</returns>
		public static T Max<T>(params T[] numbers) where T : unmanaged, INumber<T>, IMinMaxValue<T>
		{
			switch (numbers.Length)
			{
				case 0:
					throw new ArgumentException("The Array must contain elements", nameof(numbers));
				case 1:
					return numbers[0];
				case 2:
					return T.Max(numbers[0], numbers[1]);
			}

			var max = T.MinValue;
			var j = 0;

			if (Vector.IsHardwareAccelerated)
			{
				var setVectors = MemoryMarshal.Cast<T, Vector<T>>(numbers);
				var count = Vector<T>.Count;

				var result = setVectors[0];

				for (var i = 1; i < setVectors.Length; i++)
				{
					result = Vector.Max(result, setVectors[i]);
				}

				for (var i = 0; i < count; i++)
				{
					max = T.Max(max, result[i]);

					j += count;
				}
			}

			for (; j < numbers.Length; j++)
				max = T.Max(max, numbers[j]);

			return max;
		}
	}
}