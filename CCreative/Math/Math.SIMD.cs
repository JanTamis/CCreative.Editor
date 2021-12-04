using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks.Dataflow;
// ReSharper disable CheckNamespace

namespace CCreative
{
	public static partial class Math
	{
		private static int _degreeOfParallelism = Floor(System.Math.Log2(Environment.ProcessorCount));
		
		public static T Min<T>(T num1, T num2) where T : INumber<T>
		{
			return T.Min(num1, num2);
		}
		
		public static T Min<T>(T num1, T num2, T num3) where T : INumber<T>
		{
			return T.Min(T.Min(num1, num2), num3);
		}

		public static T Min<T>(T[] numbers) where T : unmanaged, INumber<T>, IMinMaxValue<T>
		{
			if (numbers.Length * Unsafe.SizeOf<T>() >= 65536)
			{
				var temp = GC.AllocateUninitializedArray<T>(_degreeOfParallelism);

				var block = new ActionBlock<(T[] array, int index, int length)>(tuple =>
				{
					var (tempArray, index, length) = tuple;
					temp[index] = Min(tempArray.AsSpan(index * _degreeOfParallelism, length));
				}, new ExecutionDataflowBlockOptions()
				{
					EnsureOrdered = false,
					MaxDegreeOfParallelism = _degreeOfParallelism,
				});

				for (var i = 0; i < _degreeOfParallelism; i++)
				{
					block.Post((numbers, i, numbers.Length / _degreeOfParallelism));
				}
			
				block.Complete();
				block.Completion.Wait();

				return Min(temp.AsSpan());
			}
		
			return Min(numbers.AsSpan());
		}

		public static T Max<T>(T num1, T num2) where T : unmanaged, INumber<T>
		{
			return T.Max(num1, num2);
		}
		
		public static T Max<T>(T num1, T num2, T num3) where T : INumber<T>
		{
			return T.Max(T.Max(num1, num2), num3);
		}

		public static T Max<T>(T[] numbers) where T : unmanaged, INumber<T>, IMinMaxValue<T>
		{
			if (numbers.Length * Unsafe.SizeOf<T>() >= 65536)
			{
				var temp = GC.AllocateUninitializedArray<T>(_degreeOfParallelism);

				var block = new ActionBlock<(T[] array, int index, int length)>(tuple =>
				{
					var (tempArray, index, length) = tuple;
					temp[index] = Max(tempArray.AsSpan(index * _degreeOfParallelism, length));
				}, new ExecutionDataflowBlockOptions()
				{
					EnsureOrdered = false,
					MaxDegreeOfParallelism = _degreeOfParallelism,
				});

				for (var i = 0; i < _degreeOfParallelism; i++)
				{
					block.Post((numbers, i, numbers.Length / _degreeOfParallelism));
				}
			
				block.Complete();
				block.Completion.Wait();

				return Max(temp.AsSpan());
			}
		
			return Max(numbers.AsSpan());
		}

		public static T Max<T>(List<T> numbers) where T : unmanaged, INumber<T>, IMinMaxValue<T>
		{
			return Max(CollectionsMarshal.AsSpan(numbers));
		}

		public static T Sum<T>(T[] numbers) where T : unmanaged, INumber<T>
		{
			if (numbers.Length * Unsafe.SizeOf<T>() >= 65536)
			{
				var temp = GC.AllocateUninitializedArray<T>(_degreeOfParallelism);

				var block = new ActionBlock<(T[] array, int index, int length)>(tuple =>
				{
					var (tempArray, index, length) = tuple;
					temp[index] = Sum(tempArray.AsSpan(index * _degreeOfParallelism, length));
				}, new ExecutionDataflowBlockOptions()
				{
					EnsureOrdered = false,
					MaxDegreeOfParallelism = _degreeOfParallelism,
				});

				for (var i = 0; i < _degreeOfParallelism; i++)
				{
					block.Post((numbers, i, numbers.Length / _degreeOfParallelism));
				}
			
				block.Complete();
				block.Completion.Wait();

				return Sum(temp.AsSpan());
			}
		
			return Sum(numbers.AsSpan());
		}

		/// <summary>
		/// Determines the smallest value in a sequence of numbers
		/// </summary>
		/// <param name="numbers">array of numbers to compare</param>
		/// <returns>returns the minimum value</returns>
		[MethodImpl(MethodImplOptions.AggressiveOptimization)]
		internal static T Min<T>(Span<T> numbers) where T : unmanaged, INumber<T>, IMinMaxValue<T>
		{
			var min = T.MaxValue;
			var j = 0;
			var count = Vector<T>.Count;

			if (Vector.IsHardwareAccelerated && numbers.Length >= count * 2)
			{
				var setVectors = MemoryMarshal.Cast<T, Vector<T>>(numbers);

				var result = setVectors[0];

				for (var i = 1; i < setVectors.Length; i++)
				{
					result = Vector.Min(result, setVectors[i]);
				}

				for (var i = 0; i < count; i++)
				{
					min = T.Min(min, result[i]);

					j += count;
				}
			}

			for (; j < numbers.Length; j++)
				min = T.Min(min, numbers[j]);

			return min;
		}

		/// <summary>
		/// Determines the biggest value in a sequence of numbers
		/// </summary>
		/// <param name="numbers">array of numbers to compare</param>
		/// <returns>returns the maximum value</returns>
		[MethodImpl(MethodImplOptions.AggressiveOptimization)]
		public static T Max<T>(Span<T> numbers) where T : unmanaged, INumber<T>, IMinMaxValue<T>
		{
			var max = T.MinValue;
			var j = 0;
			var count = Vector<T>.Count;

			if (Vector.IsHardwareAccelerated && numbers.Length >= count * 2)
			{
				var setVectors = MemoryMarshal.Cast<T, Vector<T>>(numbers);
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

		/// <summary>
		/// Determines the sum of a sequence of numbers
		/// </summary>
		/// <param name="numbers">array of get the sum of</param>
		/// <returns>returns the sum</returns>
		[MethodImpl(MethodImplOptions.AggressiveOptimization)]
		internal static T Sum<T>(Span<T> numbers) where T : unmanaged, INumber<T>
		{
			var vSum = Vector<T>.Zero;
			var count = Vector<T>.Count;

			var sum = T.Zero;
			var i = 0;

			if (Vector.IsHardwareAccelerated && numbers.Length >= count)
			{
				var vsArray = MemoryMarshal.Cast<T, Vector<T>>(numbers);

				for (i = 0; i < vsArray.Length; i++)
				{
					vSum = Vector.Add(vSum, vsArray[i]);
				}

				sum = Vector.Sum(vSum);

				i *= count;
			}

			for (; i < numbers.Length; i++)
			{
				sum += numbers[i];
			}

			return sum;
		}

		/// <summary>
		/// Determines the average value in a sequence of numbers
		/// </summary>
		/// <param name="numbers">array of numbers to compare</param>
		/// <returns>returns the average value</returns>
		public static T Average<T>(T[] numbers) where T : unmanaged, INumber<T>
		{
			return Sum(numbers) / ConvertNumber<T, int>(numbers.Length);
		}
		
		/// <summary>
		/// Use this method to set the concurrency level of a few methods in the Math methods
		/// </summary>
		/// <remarks>Only Use this method if you know what you are doing!!!</remarks>
		/// <param name="concurrencyLevel">the maximum concurrency level</param>
		public static void SetMaxConcurrencyLevel(int concurrencyLevel)
		{
			_degreeOfParallelism = concurrencyLevel;
		}
	}
}