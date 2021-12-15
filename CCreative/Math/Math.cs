using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using static System.MathF;
// ReSharper disable MemberCanBePrivate.Global

namespace CCreative
{
	public static partial class Math
	{
		private static Random rng => System.Random.Shared;

		#region Calculations

		/// <summary>
		/// Calculates the closest int value that is greater than or equal to the value of the parameter
		/// </summary>
		/// <param name="number">number to round up</param>
		/// <returns>the result of the calculation</returns>
		public static int Ceil<T>(T number) where T : IFloatingPoint<T>
		{
			return ConvertNumber<int, T>(T.Ceiling(number));
		}

		/// <summary>
		/// Constrains a value to not exceed a minimum and maximum value
		/// </summary>
		/// <param name="value">the value to constrain</param>
		/// <param name="low">minimum limit</param>
		/// <param name="high">maximum limit</param>
		/// <returns>the constraint value</returns>
		public static T Constrain<T>(T value, T low, T high) where T : INumber<T>
		{
			return T.Clamp(value, low, high);
		}

		/// <summary>
		/// Calculates the distance between two PVectors
		/// </summary>
		/// <param name="beginPoint">the first PVector</param>
		/// <param name="endPoint">the second PVector</param>
		/// <returns>the distance between the points</returns>
		public static float Dist(PVector beginPoint, PVector endPoint)
		{
			return PVector.Dist(beginPoint, endPoint);
		}

		/// <summary>
		/// Calculate the distance between the given points
		/// </summary>
		/// <param name="points">the points to calculate the distance with</param>
		/// <returns>the distance between the points</returns>
		public static float Dist(params PVector[] points)
		{
			float d = 0;

			for (var i = 0; i < points.Length - 1; i++)
			{
				var current = points[i];
				var next = points[i + 1];

				d += Dist(current, next);
			}

			return d;
		}

		/// <summary>
		/// Calculate the distance between the given points
		/// </summary>
		/// <param name="beginX">x-coordinate of the first point</param>
		/// <param name="beginY">y-coordinate of the first point</param>
		/// <param name="endX">x-coordinate of the second point</param>
		/// <param name="endY">y-coordinate of the second point</param>
		/// <returns>the distance between the points</returns>
		public static T Dist<T>(T beginX, T beginY, T endX, T endY) where T : IFloatingPoint<T>
		{
			// return Sqrt(Sq(beginX - endX) + Sq(beginY - endY));
			return Sqrt(T.FusedMultiplyAdd(beginX - endX, beginX - endX, Sq(beginY - endY)));
		}

		/// <summary>
		/// Calculate the distance between the given points
		/// </summary>
		/// <param name="beginX">x-coordinate of the first point</param>
		/// <param name="beginY">y-coordinate of the first point</param>
		/// <param name="beginZ">z-coordinate of the first point</param>
		/// <param name="endX">x-coordinate of the second point</param>
		/// <param name="endY">y-coordinate of the second point</param>
		/// <param name="endZ">z-coordinate of the second point</param>
		/// <returns>the distance between the points</returns>
		public static T Dist<T>(T beginX, T beginY, T beginZ, T endX, T endY, T endZ) where T : IFloatingPoint<T>
		{
			// return Sqrt(Sq(beginX - endX) + Sq(beginY - endY) + Sq(beginZ - endZ));
			return Sqrt(T.FusedMultiplyAdd(beginX - endX, beginX - endX,
				T.FusedMultiplyAdd(beginY - endY, beginY - endY, Sq(beginZ - endZ))));
		}

		/// <summary>
		/// Calculates the closest int value that is less than or equal to the value of the parameter
		/// </summary>
		/// <param name="number">number to round down</param>
		/// <returns>the result of the calculation</returns>
		public static int Floor<T>(T number) where T : IFloatingPoint<T>
		{
			return ConvertNumber<int, T>(T.Floor(number));
		}

		/// <summary>
		/// Calculates a number between two numbers at a specific increment
		/// </summary>
		/// <param name="start">first value</param>
		/// <param name="stop">second value</param>
		/// <param name="atm">number between 0.0 and 1.0</param>
		/// <returns>the result of the calculation</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T Lerp<T>(T start, T stop, T atm) where T : IFloatingPoint<T>
		{
			return T.FusedMultiplyAdd(atm, stop - start, start);
		}

		/// <summary>
		/// Calculates the square root of the specified number
		/// </summary>
		/// <typeparam name="T">the type of the floating point number</typeparam>
		/// <param name="number">the specified number</param>
		/// <returns>the square root of <paramref name="number"/></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T Sqrt<T>(T number) where T : IFloatingPoint<T>
		{
			return T.Sqrt(number);
		}

		/// <summary>
		/// Calculates the power of <paramref name="base"/> raised to <paramref name="exponent"/>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="base"></param>
		/// <param name="exponent"></param>
		/// <returns></returns>
		public static T Pow<T>(T @base, T exponent) where T : IFloatingPoint<T>
		{
			return T.Pow(@base, exponent);
		}

		/// <summary>
		/// Calculates the magnitude (or length) of a vector
		/// </summary>
		/// <param name="numberX">x-axis of the vector</param>
		/// <param name="numberY">y-axis of the vector</param>
		/// <returns>the magnitude (or length) of the vector</returns>
		public static T Mag<T>(T numberX, T numberY) where T : IFloatingPoint<T>
		{
			return Sqrt(T.FusedMultiplyAdd(numberX, numberX, Sq(numberY)));
		}

		/// <summary>
		/// Re-maps a number from one range to another
		/// </summary>
		/// <param name="value">the incoming value to be converted</param>
		/// <param name="start1">lower bound of the value's current range</param>
		/// <param name="stop1">upper bound of the value's current range</param>
		/// <param name="start2">lower bound of the value's target range</param>
		/// <param name="stop2">upper bound of the value's target range</param>
		/// <returns>the remapped number</returns>
		public static T Map<T>(T value, T start1, T stop1, T start2, T stop2) where T : IFloatingPoint<T>
		{
			// return start2 + (stop2 - start2) * Norm(value, start1, stop1);
			return T.FusedMultiplyAdd(stop2 - start2,  Norm(value, start1, stop1), start2);
		}

		/// <summary>
		/// Normalizes a number from another range into a value between 0 and 1
		/// </summary>
		/// <param name="value">the incoming value to be converted</param>
		/// <param name="start">lower bound of the value's current range</param>
		/// <param name="stop">upper bound of the value's current range</param>
		/// <returns>the normalized value</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T Norm<T>(T value, T start, T stop) where T : IFloatingPoint<T>
		{
			return (value - start) / (stop - start);
		}

		/// <summary>
		/// Calculates the integer closest to the given number
		/// </summary>
		/// <param name="number">number to round</param>
		/// <returns>returns the rounded number</returns>
		public static int Round<T>(T number) where T : IFloatingPoint<T>
		{
			return ConvertNumber<int, T>(T.Round(number));
		}

		/// <summary>
		/// Squares a number (multiplies a number by itself)
		/// </summary>
		/// <param name="number">number to square</param>
		/// <returns>returns the squared number</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T Sq<T>(T number) where T : INumber<T>
		{
			return number * number;
		}

		/// <summary>
		/// Calculates 1 / sqrt(x) of the given number
		/// </summary>
		/// <param name="x">the number to calculate the inverse square root of</param>
		/// <returns>1 / sqrt(x)</returns>
		public static T InverseSqrt<T>(T x) where T : IFloatingPoint<T>
		{
			return T.One / T.Sqrt(x);
		}

		/// <summary>
		/// Calculates the log2 of <see cref="x"/>
		/// </summary>
		/// <param name="x">number to find the log2 of</param>
		/// <returns>returns the log2 of <see cref="x"/></returns>
		public static T Log2<T>(T x) where T : IFloatingPoint<T>
		{
			return T.Log2(x);
		}

		#endregion

		#region Noise

		/// <summary>
		/// Returns the Perlin noise value at specified coordinates
		/// </summary>
		/// <remarks>
		/// Perlin noise is a random sequence generator producing a more natural, harmonic succession of numbers than that of the standard <see cref="Random(float, float)"/> function
		/// </remarks>
		/// <param name="x">x-coordinate in noise space</param>
		/// <returns>the Perlin noise value at the specified coordinates</returns>
		public static float Noise(float x)
		{
			return (NoiseMaker.Noise1(x) + 1) / 2;
		}

		/// <summary>
		/// Returns the Perlin noise value at specified coordinates
		/// </summary>
		/// <remarks>
		/// Perlin noise is a random sequence generator producing a more natural, harmonic succession of numbers than that of the standard <see cref="Random(float, float)"/> function
		/// </remarks>
		/// <param name="x">x-coordinate in noise space</param>
		/// <param name="y">y-coordinate in noise space</param>
		/// <returns>the Perlin noise value at the specified coordinates</returns>
		public static float Noise(float x, float y)
		{
			return (NoiseMaker.Noise2(x, y) + 1) / 2;
			//if (noiseChanged)
			//{
			//	float total = 0;
			//	float frequency = 1;
			//	float amplitude = 1;
			//	float maxValue = 0;            // Used for normalizing result to 0.0 - 1.0

			//	for (int i = 0; i < octaves; i++)
			//	{
			//		total += (Noise.Evaluate(x * frequency, y * frequency)) * amplitude;

			//		maxValue += amplitude;

			//		amplitude *= persistence;
			//		frequency *= (float)lacunarity;
			//	}

			//	return (total / maxValue);
			//}

			//return Noise.Evaluate(x, y);
		}

		/// <summary>
		/// Returns the Perlin noise value at specified coordinates
		/// </summary>
		/// <remarks>
		/// Perlin noise is a random sequence generator producing a more natural, harmonic succession of numbers than that of the standard <see cref="Random(float, float)"/> function
		/// </remarks>
		/// <param name="x">x-coordinate in noise space</param>
		/// <param name="y">y-coordinate in noise space</param>
		/// <param name="z">z-coordinate in noise space</param>
		/// <returns>the Perlin noise value at the specified coordinates</returns>
		public static float Noise(float x, float y, float z)
		{
			return (NoiseMaker.Noise3(x, y, z) + 1) / 2;

			//if (noiseChanged)
			//{
			//	float total = 0;
			//	float frequency = 1;
			//	float amplitude = 1;
			//	float maxValue = 0;            // Used for normalizing result to 0.0 - 1.0

			//	for (int i = 0; i < octaves; i++)
			//	{
			//		total += (Noise.Evaluate((float)(x * frequency), (float)(y * frequency), (float)(z * frequency))) * amplitude;

			//		maxValue += amplitude;

			//		amplitude *= persistence;
			//		frequency *= lacunarity;
			//	}

			//	return (total / maxValue);
			//}

			//return (Noise.Evaluate(x, y, z));
		}

		/// <summary>
		/// Sets the seed value for <see cref="Noise(float)"/>, <see cref="Noise(float, float)"/> and <see cref="Noise(float, float, float)"/>
		/// </summary>
		/// <remarks>
		/// By default, noise() produces different results each time the program is run. Set the seed parameter to a constant to return the same pseudo-random numbers each time the software is run
		/// </remarks>
		/// <param name="seed">seed value</param>
		public static void NoiseSeed(int seed)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Adjusts the character and level of detail produced by the Perlin noise function
		/// </summary>
		/// <remarks>
		/// Similar to harmonics in physics, noise is computed over several octaves. Lower octaves contribute more to the output signal and as such define the overall intensity of the noise, whereas higher octaves create finer-grained details in the noise sequence.
		///
		/// By default, noise is computed over 4 octaves with each octave contributing exactly half than its predecessor, starting at 50% strength for the first octave.This falloff amount can be changed by adding an additional function parameter.For example, a falloff factor of 0.75 means each octave will now have 75% impact (25% less) of the previous lower octave.While any number between 0.0 and 1.0 is valid, note that values greater than 0.5 may result in noise() returning values greater than 1.0
		/// </remarks>
		/// <param name="lod">number of octaves to be used by the noise</param>
		public static void NoiseDetail(int lod)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Adjusts the character and level of detail produced by the Perlin noise function
		/// </summary>
		/// <remarks>
		/// Similar to harmonics in physics, noise is computed over several octaves. Lower octaves contribute more to the output signal and as such define the overall intensity of the noise, whereas higher octaves create finer-grained details in the noise sequence.
		///
		/// By default, noise is computed over 4 octaves with each octave contributing exactly half than its predecessor, starting at 50% strength for the first octave.This falloff amount can be changed by adding an additional function parameter.For example, a falloff factor of 0.75 means each octave will now have 75% impact (25% less) of the previous lower octave.While any number between 0.0 and 1.0 is valid, note that values greater than 0.5 may result in noise() returning values greater than 1.0
		/// </remarks>
		/// <param name="lod">number of octaves to be used by the noise</param>
		/// <param name="falloff">falloff factor for each octave</param>
		public static void NoiseDetail(int lod, float falloff)
		{
			throw new NotImplementedException();
		}

		#endregion

		/// <summary> Converts degrees to radians. </summary>
		/// <param name="radians"> The radians to convert. </param>
		/// <returns>degrees</returns>
		public static T Degrees<T>(T radians) where T : IFloatingPoint<T>
		{
			return radians * (T.Create(180) / T.Pi);
		}

		/// <summary>
		/// Converts radians to degrees
		/// </summary>
		/// <param name="degrees">the radians</param>
		/// <returns>radians</returns>
		public static T Radians<T>(T degrees) where T : IFloatingPoint<T>
		{
			return degrees * (T.Pi / T.Create(180));
		}

		#region Algorithms

		/// <summary>
		/// Returns a random string with a given length</summary>
		/// <param name="length">the length</param>
		/// <returns>a random string</returns>
		public static string RandomString(int length)
		{
			return String.Create(length, System.Random.Shared, (span, random) =>
			{
				for (var i = 0; i < span.Length; i++)
				{
					span[i] = (char)random.Next('A', 'Z');
				}
			});
		}

		/// <summary>
		/// Return a random float floating point number between the given range
		/// </summary>
		/// <param name="low">the lower bound (inclusive)</param>
		/// <param name="high">the upper bound (exclusive)</param>
		/// <returns>a random number</returns>
		public static float Random(float low, float high)
		{
			if (low >= high)
				return low;

			var diff = high - low;
			float value;

			// because of rounding error, can't just add low, otherwise it may hit high
			// https://github.com/processing/processing/issues/4551
			do
			{
				value = Random(diff) + low;
			} while (Abs(value - high) < float.Epsilon);

			return value;
		}

		/// <summary>
		/// Return a random float floating point number
		/// </summary>
		/// <param name="high">the upper bound (exclusive)</param>
		/// <returns>a random number</returns>
		public static float Random(float high)
		{
			// avoid an infinite loop when 0 or NaN are passed in
			if (high is 0 or Single.NaN)
				return 0;

			// for some reason (rounding error?) Math.random() * 3
			// can sometimes return '3' (once in ~30 million tries)
			// so a check was added to avoid the inclusion of 'howbig'
			float value;
			do
			{
				value = Random() * high;
			} while (Abs(value - high) < float.Epsilon);

			return value;
		}

		/// <summary>
		/// Return a random float floating point number between 0 and 1
		/// </summary>
		/// <returns>a random float-point number between 0 and 1</returns>
		public static float Random()
		{
			return rng.NextSingle();
		}

		public static void Random(byte[] array)
		{
			rng.NextBytes(array);
		}

		/// <summary>
		/// Sets the seed value for random()
		/// </summary>
		/// <remarks>
		/// By default, random() produces different results each time the program is run. Set the seed parameter to a constant to return the same pseudo-random numbers each time the software is run
		/// </remarks>
		/// <param name="seed"></param>
		public static void RandomSeed(int seed)
		{
			// rng = new Random(seed);
		}

		/// <summary>
		/// Returns a random item from the list
		/// </summary>
		/// <param name="list">the array to pick a random item from</param>
		/// <returns>a rando item from the list</returns>
		public static T? Random<T>(IEnumerable<T> list)
		{
			// https://stackoverflow.com/a/648240/6448711

			var current = default(T);
			var count = 0;

			foreach (var element in list)
			{
				count++;

				if (RandomInt(count) == 0)
					current = element;
			}

			if (count == 0)
				throw new InvalidOperationException("Sequence was empty");

			return current;
		}

		/// <summary> Returns a random item form the array. </summary>
		/// <param name="array"> The array to pick a random item from.</param>
		/// <returns></returns>
		public static T Random<T>(IList<T> array)
		{
			return array[RandomInt(array.Count)];
		}

		/// <summary> Return a random int number. </summary>
		/// <param name="lowerBound"> The lower bound (inclusive). </param>
		/// <param name="upperBound"> The upper bound (exclusive). </param>
		/// <returns> A random int between the given range. </returns>
		public static int RandomInt(int lowerBound, int upperBound)
		{
			return rng.Next(lowerBound, upperBound);
		}

		/// <summary> Return a random int number. </summary>
		/// <param name="upperBound">  the upper bound (exclusive). </param>
		/// <returns> System.Single. </returns>
		public static int RandomInt(int upperBound)
		{
			return rng.Next(upperBound);
		}

		/// <summary> Return a random int number between 0 and <see cref="Int32.MaxValue"/> (exclusive)</summary>
		/// 
		/// <returns> System.Single. </returns>
		public static int RandomInt()
		{
			return rng.Next();
		}

		/// <summary> Return a random byte number. </summary>
		/// <returns> System.Single. </returns>
		public static byte RandomByte()
		{
			return (byte)rng.Next(Byte.MaxValue);
		}

		/// <summary>
		/// Fills the provided byte array with random bytes.
		/// </summary>
		/// <param name="buffer"></param>
		public static void RandomBytes(byte[] buffer)
		{
			rng.NextBytes(buffer);
		}

		/// <summary> Returns a random number fitting a Gaussian, or normal, distribution. There is theoretically no minimum or maximum value that randomGaussian() might return.  </summary>
		/// <param name="mean"> The mean. </param>
		/// <param name="sd"> The standard deviation. </param>
		/// <returns> A float. </returns>
		public static float RandomGaussian(float mean, float sd)
		{
			var u1 = Random();
			var u2 = Random();

			var randStdNormal = Sqrt(-2.0f * Log(u1)) * Sin(PConstants.TWO_PI * u2);
			var randNormal = mean + sd * randStdNormal;

			return randNormal;
		}

		/// <summary> Returns a random number fitting a Gaussian, or normal, distribution. There is theoretically no minimum or maximum value that randomGaussian() might return. </summary>
		/// <returns> A Gaussian of mean 0 and deviation of 1. </returns>
		public static float RandomGaussian()
		{
			return RandomGaussian(0, 1);
		}

		/// <summary> Returns a true or false the chance is 50-50. </summary>
		/// <returns> The result. </returns>
		public static bool RandomBoolean()
		{
			return rng.Next() > (Int32.MaxValue / 2);
		}

		/// <summary>
		/// Returns a random color with the alpha channel set to <see cref="byte.MaxValue"/>
		/// </summary>
		/// <returns></returns>
		public static Color RandomColor()
		{
			Span<byte> bytes = stackalloc byte[3];
			rng.NextBytes(bytes);

			return new MemoryColor(Byte.MaxValue, bytes[0], bytes[1], bytes[2]);
		}

		/// <summary>
		/// Calculate a random long between <paramref name="min"/> and <paramref name="max"/>
		/// </summary>
		/// <param name="min">the minimum value of the bounds</param>
		/// <param name="max"> the maximum value of the bounds</param>
		/// <returns>a random long between <paramref name="min"/> and <paramref name="max"/></returns>
		public static long RandomLong(long min, long max)
		{
			return rng.NextInt64(min, max);
		}

		/// <summary>
		/// Calculate a random long between 0 and <paramref name="max"/>
		/// </summary>
		/// <param name="max"> the maximum value of the bounds</param>
		/// <returns>a random long between 0 and <paramref name="max"/></returns>
		public static long RandomLong(long max)
		{
			if (max == 0)
				throw new ArgumentException("max can't be 0 (Zero)", nameof(max));

			return RandomLong(0, max);
		}

		/// <summary>
		/// Calculate a random long between <see cref="Int64.MinValue"/> and <see cref="Int64.MaxValue"/>
		/// </summary>
		/// <returns>a random long between <see cref="Int64.MinValue"/> and <see cref="Int64.MaxValue"/></returns>
		public static long RandomLong()
		{
			return rng.NextInt64();
		}

		/// <summary>
		/// Calculate a random decimal between <paramref name="minValue"/> and <paramref name="maxValue"/>
		/// </summary>
		/// <param name="minValue">the minimum value of the bounds</param>
		/// <param name="maxValue">the maximum value of the bounds</param>
		/// <returns>a random decimal between <paramref name="minValue"/> and <paramref name="maxValue"/></returns>
		public static decimal RandomDecimal(decimal minValue, decimal maxValue)
		{
			decimal value;

			do
			{
				var nextDecimalSample = RandomDecimal();
				value = maxValue * nextDecimalSample + minValue * (1 - nextDecimalSample);
			} while (value == maxValue);

			return value;
		}

		/// <summary>
		/// Calculate a random decimal between <see cref="Decimal.Zero"/> and <paramref name="maxValue"/>		
		/// </summary>
		/// <param name="maxValue">the maximum value of the bounds</param>
		/// <returns>a random decimal between <see cref="Decimal.Zero"/> and <paramref name="maxValue"/></returns>
		public static decimal RandomDecimal(decimal maxValue)
		{
			return RandomDecimal(Decimal.Zero, maxValue);
		}

		/// <summary>
		/// Calculate a random decimal between <see cref="Decimal.Zero"/> and <see cref="Decimal.One"/>
		/// </summary>
		/// <returns>a random decimal between <see cref="Decimal.Zero"/> and <see cref="Decimal.One"/> </returns>
		public static decimal RandomDecimal()
		{
			decimal sample;

			//After ~200 million tries this never took more than one attempt but it is possible to generate combinations of a, b, and c with the approach below resulting in a sample >= 1.
			do
			{
				var a = RandomInt();
				var b = RandomInt();
				//The high bits of 0.9999999999999999999999999999m are 542101086.
				var c = RandomInt(542101087);
				sample = new Decimal(a, b, c, false, 28);
			} while (sample >= 1m);

			return sample;
		}

		/// <summary> returns the sign of a number, indicating whether the number is positive, negative or zero </summary>
		/// <param name="number"> The number to check. </param>
		/// <returns> -1 if lower than 0, 0 if equal to 0 and 1 if higher that 0. </returns>
		public static int Sign<T>(T number) where T : INumber<T>
		{
			return ConvertNumber<int, T>(T.Sign(number));
		}

		/// <summary> Returns the fibonacci of the given number. </summary>
		/// <param name="number"> The number. </param>
		/// <returns> A float. </returns>
		public static T Fibonacci<T>(T number) where T : INumber<T>
		{
			var a = T.Zero;
			var b = T.One;

			// In N steps compute Fibonacci sequence iteratively.
			for (var i = T.Zero; i < number; i++)
			{
				var temp = a;
				a = b;
				b += temp;
			}

			return a;
		}

		/// <summary>
		/// Shuffles the given list and returns a copy which can be iterated over
		/// </summary>
		/// <param name="list">the items ot shuffle</param>
		/// <returns>a shuffled copy of the list</returns>
		public static IEnumerable<T> Shuffle<T>(IEnumerable<T> list)
		{
			var array = list.ToArray(); // keeps track of count items shuffled

			for (var i = 0; i < array.Length; i++)
			{
				var j = RandomInt(i, array.Length);

				yield return array[j];

				array[j] = array[i];
			}
		}

		/// <summary>
		/// Search for a value in a sorted list
		/// </summary>
		/// <param name="values">the sorted list to search though</param>
		/// <param name="value">the value to search for</param>
		/// <returns>the index of <paramref name="value"/></returns>
		public static int BinarySearch<T>(IList<T> values, T value) where T : INumber<T>
		{
			var lo = 0;
			var hi = values.Count - 1;

			while (lo <= hi)
			{
				var i = lo + ((hi - lo) >> 1);

				if (values[i] == value)
					return i;

				if (values[i] < value)
				{
					lo = i + 1;
				}
				else
				{
					hi = i - 1;
				}
			}

			return ~lo;
		}

		/// <summary> Converts an int, byte, or a long to a String containing the equivalent binary notation </summary>
		/// <param name="value"> Value to convert </param>
		/// <returns> Returns the binary string </returns>
		public static string Binary(long value)
		{
			return System.Convert.ToString(value, 2);
		}

		/// <summary> Converts an char to a String containing the equivalent binary notation </summary>
		/// <param name="value"> Value to convert </param>
		/// <returns> Returns the binary string </returns>
		public static string Binary(char value)
		{
			return System.Convert.ToString(value, 2);
		}

		/// <summary> Converts an char to a String containing the equivalent binary notation </summary>
		/// <param name="value"> Value to convert </param>
		/// <returns> Returns the binary string </returns>
		public static string Binary(Color value)
		{
			return System.Convert.ToString(value.GetHashCode(), 2);
		}

		/// <summary> Converts a String representation of a binary number to its equivalent integer value </summary>
		/// <param name="value"> String to convert to an integer </param>
		/// <returns> Returns the result of the conversion </returns>
		public static int Unbinary(string value)
		{
			return System.Convert.ToInt32(value, 2);
		}

		/// <summary> Converts an int or a byte to a String containing the equivalent hexadecimal notation </summary>
		/// <param name="value"> The value to convert to a hex value </param>
		/// <returns> Returns the hex value </returns>
		public static string Hex<T>(T value) where T : IConvertible
		{
			return System.Convert.ToString(value.ToInt64(null), 16).ToUpper();
		}

		/// <summary> Converts an char to a String containing the equivalent hexadecimal notation </summary>
		/// <param name="value"> The value to convert to a hex value </param>
		/// <returns> Returns the hex value </returns>
		public static string Hex(char value)
		{
			return System.Convert.ToString(value, 16).ToUpper();
		}

		/// <summary> Converts an int or a byte to a String containing the equivalent hexadecimal notation </summary>
		/// <param name="value"> The value to convert to a hex value </param>
		/// <returns> Returns the hex value </returns>
		public static string Hex(Color value)
		{
			return System.Convert.ToString(value.GetHashCode(), 16).ToUpper();
		}

		/// <summary> Converts a String representation of a hexadecimal number to its equivalent integer value </summary>
		/// <param name="value"> String to convert to an integer </param>
		/// <returns> Returns a integer from a hexadecimal number </returns>
		public static int Unhex(string value)
		{
			return System.Convert.ToInt32(value, 16);
		}

		/// <summary>
		/// Converts the given value to a string, returns <see cref="String.Empty"/> if the value is null
		/// </summary>
		/// <param name="value">the value to convert</param>
		/// <returns>the converted value</returns>
		public static string Str<T>(T? value)
		{
			return value?.ToString() ?? String.Empty;
		}

		/// <summary>
		/// Converts a value to a <see cref="Int32"/>
		/// </summary>
		/// <param name="value">the value to convert</param>
		/// <returns>the converted value</returns>
		public static int Int<T>(T value) where T : IConvertible
		{
			return value?.ToInt32(null) ?? default;
		}

		/// <summary>
		/// Converts a value to a <see cref="System.Char"/>
		/// </summary>
		/// <param name="value">the value to convert</param>
		/// <returns>the converted value</returns>
		public static char Char<T>(T value) where T : IConvertible
		{
			return value?.ToChar(null) ?? default;
		}

		/// <summary>
		/// Converts a value to a <see cref="System.Boolean"/>
		/// </summary>
		/// <param name="value">the value to convert</param>
		/// <returns>the converted value</returns>
		public static bool Boolean<T>(T value) where T : IConvertible
		{
			return value?.ToBoolean(null) ?? default;
		}

		/// <summary>
		/// Converts the given value to a float
		/// </summary>
		/// <param name="value">the value to convert to a float</param>
		/// <returns>the converted value</returns>
		public static float Float<T>(T value) where T : IConvertible
		{
			return value?.ToSingle(null) ?? default;
		}

		/// <summary> Converts a value to a different type </summary>
		/// <param name="value"> The value to convert </param>
		/// 
		/// <returns> Returns the converted value </returns>
		public static T Convert<T, U>(U value)
		{
			return (T)System.Convert.ChangeType(value, typeof(T))!;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static TResult ConvertNumber<TResult, TOrigin>(TOrigin origin) where TResult : INumber<TResult>
			where TOrigin : INumber<TOrigin>
		{
			return TResult.Create(origin);
		}

		#endregion
	}
}