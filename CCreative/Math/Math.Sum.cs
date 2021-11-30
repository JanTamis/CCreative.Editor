using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace CCreative
{
	public static partial class Math
	{
		/// <summary>
		/// Determines the sum of a sequence of numbers
		/// </summary>
		/// <param name="numbers">array of get the sum of</param>
		/// <returns>returns the sum</returns>
		public static T Sum<T>(params T[] numbers) where T : unmanaged, INumber<T>
		{
			var vSum = Vector<T>.Zero;
			var count = Vector<T>.Count;

			var sum = T.Zero;
			var i = 0;

			if (Vector.IsHardwareAccelerated && count >= numbers.Length)
			{
				var vsArray = MemoryMarshal.Cast<T, Vector<T>>(numbers);

				for (i = 0; i < vsArray.Length; i++)
				{
					vSum += vsArray[i];
				}

				sum = Vector.Dot(vSum, Vector<T>.One);

				i *= count;
			}

			for (; i < numbers.Length; i++)
			{
				sum += numbers[i];
			}

			return sum;
		}
	}
}