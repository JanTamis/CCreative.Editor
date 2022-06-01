using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using CCreative.Helpers;

namespace CCreative;

[RequiresPreviewFeatures]
public static partial class Math
{
	private static Random Rng { get; set; } = System.Random.Shared;

	#region Calculations

	/// <summary>
	/// Calculates the closest int value that is greater than or equal to the value of the parameter
	/// </summary>
	/// <param name="number">number to round up</param>
	/// <returns>the result of the calculation</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Ceil<T>(T number) where T : IFloatingPoint<T>
	{
		return Int(T.Ceiling(number));
	}

	/// <summary>
	/// Calculates the integer closest to the given number
	/// </summary>
	/// <param name="number">number to round</param>
	/// <returns>returns the rounded number</returns>
	public static int Round<T>(T number) where T : IFloatingPoint<T>
	{
		return Int(T.Round(number));
	}

	/// <summary>
	/// Calculates the closest int value that is less than or equal to the value of the parameter
	/// </summary>
	/// <param name="number">number to round down</param>
	/// <returns>the result of the calculation</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Floor<T>(T number) where T : IFloatingPoint<T>
	{
		return Int(T.Floor(number));
	}

	/// <summary>
	/// Constrains a value to not exceed a minimum and maximum value
	/// </summary>
	/// <param name="value">the value to constrain</param>
	/// <param name="low">minimum limit</param>
	/// <param name="high">maximum limit</param>
	/// <returns>the constraint value</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Constrain<T>(T value, T low, T high) where T : IComparisonOperators<T, T>
	{
		if (value < low) return low;
		if (value > high) return high;

		return value;
	}

	/// <summary>
	/// Calculates the distance between two PVectors
	/// </summary>
	/// <param name="beginPoint">the first PVector</param>
	/// <param name="endPoint">the second PVector</param>
	/// <returns>the distance between the points</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float Dist(Vector beginPoint, Vector endPoint)
	{
		return Vector.Distance(beginPoint, endPoint);
	}

	/// <summary>
	/// Calculate the distance between the given points
	/// </summary>
	/// <param name="points">the points to calculate the distance with</param>
	/// <returns>the distance between the points</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float Dist(params Vector[] points)
	{
		var d = 0f;

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
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Dist<T>(T beginX, T beginY, T endX, T endY) where T : IFloatingPointIeee754<T>
	{
		// return Sqrt(Sq(beginX - endX) + Sq(beginY - endY));
		return T.Sqrt(T.FusedMultiplyAdd(beginX - endX, beginX - endX, Sq(beginY - endY)));
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
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Dist<T>(T beginX, T beginY, T beginZ, T endX, T endY, T endZ) where T : IFloatingPointIeee754<T>
	{
		// return Sqrt(Sq(beginX - endX) + Sq(beginY - endY) + Sq(beginZ - endZ));
		return T.Sqrt(T.FusedMultiplyAdd(beginX - endX, beginX - endX, T.FusedMultiplyAdd(beginY - endY, beginY - endY, Sq(beginZ - endZ))));
	}

	/// <summary>
	/// Calculate the distance between the given points squared
	/// </summary>
	/// <param name="beginX">x-coordinate of the first point</param>
	/// <param name="beginY">y-coordinate of the first point</param>
	/// <param name="beginZ">z-coordinate of the first point</param>
	/// <param name="endX">x-coordinate of the second point</param>
	/// <param name="endY">y-coordinate of the second point</param>
	/// <param name="endZ">z-coordinate of the second point</param>
	/// <returns>the distance between the points</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T DistSq<T>(T beginX, T beginY, T beginZ, T endX, T endY, T endZ) where T : IFloatingPointIeee754<T>
	{
		// return Sq(beginX - endX) + Sq(beginY - endY) + Sq(beginZ - endZ);
		return T.FusedMultiplyAdd(beginX - endX, beginX - endX, T.FusedMultiplyAdd(beginY - endY, beginY - endY, Sq(beginZ - endZ)));
	}

	/// <summary>
	/// Calculate the distance between the given points squared
	/// </summary>
	/// <param name="beginX">x-coordinate of the first point</param>
	/// <param name="beginY">y-coordinate of the first point</param>
	/// <param name="endX">x-coordinate of the second point</param>
	/// <param name="endY">y-coordinate of the second point</param>
	/// <returns>the distance between the points</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T DistSq<T>(T beginX, T beginY, T endX, T endY) where T : INumber<T>
	{
		// return Sq(beginX - endX) + Sq(beginY - endY);
		return FusedMultiplyAdd(beginX - endX, beginX - endX, Sq(beginY - endY));
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
		return FusedMultiplyAdd(atm, stop - start, start);
	}

	/// <summary>
	/// Calculates the square root of the specified number
	/// </summary>
	/// <typeparam name="T">the type of the floating point number</typeparam>
	/// <param name="number">the specified number</param>
	/// <returns>the square root of <paramref name="number"/></returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Sqrt<T>(T number) where T : IRootFunctions<T>
	{
		return T.Sqrt(number);
	}

	/// <summary>
	/// Calculates the cube root of the specified number
	/// </summary>
	/// <typeparam name="T">the type of the floating point number</typeparam>
	/// <param name="number">the specified number</param>
	/// <returns>the cube root of <paramref name="number"/></returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Cbrt<T>(T number) where T : IRootFunctions<T>
	{
		return T.Cbrt(number);
	}

	/// <summary>
	/// Calculates the power of <paramref name="base"/> raised to <paramref name="exponent"/>
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="base"></param>
	/// <param name="exponent"></param>
	/// <returns></returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Pow<T>(T @base, T exponent) where T : IPowerFunctions<T>
	{
		return T.Pow(@base, exponent);
	}

	/// <summary>
	/// Calculates the magnitude (or length) of a vector
	/// </summary>
	/// <param name="numberX">x-axis of the vector</param>
	/// <param name="numberY">y-axis of the vector</param>
	/// <returns>the magnitude (or length) of the vector</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Mag<T>(T numberX, T numberY) where T : IFloatingPointIeee754<T>
	{
		return T.Sqrt(T.FusedMultiplyAdd(numberX, numberX, Sq(numberY)));
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
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Map<T>(T value, T start1, T stop1, T start2, T stop2) where T : INumber<T>
	{
		return FusedMultiplyAdd((value - start1) / (stop1 - start1), stop2 - start2, start2);
	}

	/// <summary>
	/// Normalizes a number from another range into a value between 0 and 1
	/// </summary>
	/// <param name="value">the incoming value to be converted</param>
	/// <param name="start">lower bound of the value's current range</param>
	/// <param name="stop">upper bound of the value's current range</param>
	/// <returns>the normalized value</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Norm<T>(T value, T start, T stop) where T : IFloatingPointIeee754<T>
	{
		return (value - start) / (stop - start);
	}

	/// <summary>
	/// Squares a number (multiplies a number by itself)
	/// </summary>
	/// <param name="number">number to square</param>
	/// <returns>returns the squared number</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Sq<T>(T number) where T : IMultiplyOperators<T, T, T>
	{
		return number * number;
	}

	/// <summary>
	/// Cubes a number (number * number * number)
	/// </summary>
	/// <param name="number">number to square</param>
	/// <returns>returns the squared number</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Cb<T>(T number) where T : IMultiplyOperators<T, T, T>
	{
		return number * number * number;
	}

	/// <summary>
	/// Calculates 1 / sqrt(x) of the given number
	/// </summary>
	/// <param name="x">the number to calculate the inverse square root of</param>
	/// <returns>1 / sqrt(x)</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T ReciprocalSqrtEstimate<T>(T x) where T : IFloatingPointIeee754<T>
	{
		return T.ReciprocalSqrtEstimate(x);
	}

	/// <summary>
	/// Calculates 1 / sqrt(x) of the given number
	/// </summary>
	/// <param name="x">the number to calculate the inverse square root of</param>
	/// <returns>1 / sqrt(x)</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T ReciprocalEstimate<T>(T x) where T : IFloatingPointIeee754<T>
	{
		return T.ReciprocalEstimate(x);
	}

	/// <summary>
	/// Calculates the log2 of <see cref="x"/>
	/// </summary>
	/// <param name="x">number to find the log2 of</param>
	/// <returns>returns the log2 of <see cref="x"/></returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Log2<T>(T x) where T : IBinaryNumber<T>
	{
		return T.Log2(x);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Log<T>(T x) where T : ILogarithmicFunctions<T>
	{
		return T.Log(x);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Sin<T>(T x) where T : ITrigonometricFunctions<T>
	{
		return T.Sin(x);
	}
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Cos<T>(T x) where T : ITrigonometricFunctions<T>
	{
		return T.Cos(x);
	}
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static (float Sin, float Cos) SinCos(float x)
	{
		return MathF.SinCos(x);
	}
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static (double Sin, double Cos) SinCos(double x)
	{
		return System.Math.SinCos(x);
	}
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Tan<T>(T x) where T : ITrigonometricFunctions<T>
	{
		return T.Tan(x);
	}
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Asin<T>(T x) where T : ITrigonometricFunctions<T>
	{
		return T.Asin(x);
	}
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Acos<T>(T x) where T : ITrigonometricFunctions<T>
	{
		return T.Acos(x);
	}
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Atan<T>(T x) where T : ITrigonometricFunctions<T>
	{
		return T.Atan(x);
	}
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Atan2<T>(T y, T x) where T : ITrigonometricFunctions<T>
	{
		return T.Atan2(y, x);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Abs<T>(T x) where T : INumber<T>
	{
		return T.Abs(x);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Factorial<T>(T number) where T : INumber<T>
	{
		var fact = T.One;

		if (number > fact)
		{
			for (var k = T.CreateChecked(2); k <= number; k++)
			{
				fact *= k;
			}
		}

		return fact;
	}

	public static (T Quotient, T Remainder) DivRem<T>(T left, T right) where T : INumber<T>
	{
		var quotient = left / right;
		return (quotient, FusedMultiplySubtract(quotient, right, left));
	}

	public static T MinMagnitude<T>(T x, T y) where T : INumber<T>
	{
		return T.MinMagnitude(x, y);
	}

	public static T MaxMagnitude<T>(T x, T y) where T : INumber<T>
	{
		return T.MaxMagnitude(x, y);
	}

	public static TFloat ScaleB<TFloat>(TFloat x, int n) where TFloat : IFloatingPointIeee754<TFloat>
	{
		return TFloat.ScaleB(x, n);
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
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
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
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
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
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
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
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Degrees<T>(T radians) where T : IFloatingPointIeee754<T>
	{
		return radians * (T.CreateChecked(180) / T.Pi);
	}

	/// <summary>
	/// Converts radians to degrees
	/// </summary>
	/// <param name="degrees">the radians</param>
	/// <returns>radians</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Radians<T>(T degrees) where T : IFloatingPointIeee754<T>
	{
		return degrees * (T.Pi / T.CreateChecked(180));
	}

	#region Algorithms

	/// <summary>
	/// Returns a random string with a given length</summary>
	/// <param name="length">the length</param>
	/// <returns>a random string</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string RandomString(int length)
	{
		return System.String.Create(length, System.Random.Shared, (span, random) =>
		{
			for (var i = 0; i < span.Length; i++)
			{
				var value = random.Next(33, 127);

				// the code 33 and 126 are in the range of ascii readable characters (see https://www.rapidtables.com/code/text/ascii-table.html)
				span[i] = Unsafe.As<Int32, Char>(ref value);
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
		{
			return low;
		}

		var diff = high - low;
		float value;

		// because of rounding error, can't just add low, otherwise it may hit high
		// https://github.com/processing/processing/issues/4551
		do
		{
			value = FusedMultiplyAdd(Random(), diff, low);
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
		{
			return 0;
		}

		float value;

		// for some reason (rounding error?) Math.random() * 3
		// can sometimes return '3' (once in ~30 million tries)
		// so a check was added to avoid the inclusion of 'howbig'
		// float value;
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
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float Random()
	{
		return Rng.NextSingle();
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
		Rng = new Random(seed);
	}

	/// <summary>
	/// Returns a random item from the list
	/// </summary>
	/// <param name="enumerable">the array to pick a random item from</param>
	/// <returns>a random item from the list, if the list is empty de default value will be return</returns>
	public static T? Random<T>(IEnumerable<T?> enumerable)
	{
		// https://stackoverflow.com/a/648240/6448711

		if (enumerable is IList<T> list)
		{
			return list[RandomInt(list.Count)];
		}

		var current = default(T);
		var count = 0;

		foreach (var element in enumerable)
		{
			count++;

			if (RandomInt(count) is 0)
			{
				current = element;
			}
		}

		return current;
	}

	/// <summary> Return a random int number. </summary>
	/// <param name="lowerBound"> The lower bound (inclusive). </param>
	/// <param name="upperBound"> The upper bound (exclusive). </param>
	/// <returns> A random int between the given range. </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int RandomInt(int lowerBound, int upperBound)
	{
		return Rng.Next(lowerBound, upperBound);
	}

	/// <summary> Return a random int number. </summary>
	/// <param name="upperBound">  the upper bound (exclusive). </param>
	/// <returns> System.Single. </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int RandomInt(int upperBound)
	{
		return Rng.Next(upperBound);
	}

	/// <summary> Return a random int number between 0 and <see cref="Int32.MaxValue"/> (exclusive)</summary>
	/// <returns> System.Single. </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int RandomInt()
	{
		return Rng.Next();
	}

	/// <summary> Return a random byte number. </summary>
	/// <returns> System.Single. </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static byte RandomByte()
	{
		var value = RandomInt(Byte.MaxValue);
		return Unsafe.As<Int32, Byte>(ref value);
	}

	/// <summary>
	/// Fills the provided byte array with random bytes.
	/// </summary>
	/// <param name="buffer">the buffer to fill with random data</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void RandomBytes(Span<byte> buffer)
	{
		Rng.NextBytes(buffer);
	}

	/// <summary> Returns a random number fitting a Gaussian, or normal, distribution. There is theoretically no minimum or maximum value that randomGaussian() might return.  </summary>
	/// <param name="mean"> The mean. </param>
	/// <param name="sd"> The standard deviation. </param>
	/// <returns> A float. </returns>
	public static float RandomGaussian(float mean = 0, float sd = 1)
	{
		var u1 = Random();
		var u2 = Random();

		var randStdNormal = Sqrt(-2.0f * Log(u1)) * Sin(PConstants.TWO_PI * u2);
		var randNormal = FusedMultiplyAdd(sd, randStdNormal, mean);

		return randNormal;
	}

	/// <summary> Returns a true or false the chance is 50-50. </summary>
	/// <returns> The result. </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool RandomBoolean()
	{
		return Rng.Next() > (Int32.MaxValue - 1) / 2;
	}

	/// <summary>
	/// Returns a random color with the alpha channel set to <see cref="Byte.MaxValue"/>
	/// </summary>
	/// <returns></returns>
	public static Color RandomColor()
	{
		var result = Rng.Next(Int32.MinValue, Int32.MaxValue);
		result |= Byte.MaxValue << 24;

		return Unsafe.As<Int32, Color>(ref result);
	}

	/// <summary>
	/// Calculate a random long between <paramref name="min"/> and <paramref name="max"/>
	/// </summary>
	/// <param name="min">the minimum value of the bounds</param>
	/// <param name="max"> the maximum value of the bounds</param>
	/// <returns>a random long between <paramref name="min"/> and <paramref name="max"/></returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static long RandomLong(long min, long max)
	{
		return Rng.NextInt64(min, max);
	}

	/// <summary>
	/// Calculate a random long between 0 and <paramref name="max"/>
	/// </summary>
	/// <param name="max"> the maximum value of the bounds</param>
	/// <returns>a random long between 0 and <paramref name="max"/></returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static long RandomLong(long max)
	{
		return RandomLong(0, max);
	}

	/// <summary>
	/// Calculate a random long between <see cref="Int64.MinValue"/> and <see cref="Int64.MaxValue"/>
	/// </summary>
	/// <returns>a random long between <see cref="Int64.MinValue"/> and <see cref="Int64.MaxValue"/></returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static long RandomLong()
	{
		return Rng.NextInt64();
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
			value = FusedMultiplyAdd(maxValue, nextDecimalSample, minValue * (1 - nextDecimalSample));
		} while (value == maxValue);

		return value;
	}

	/// <summary>
	/// Calculate a random decimal between <see cref="Decimal.Zero"/> and <paramref name="maxValue"/>		
	/// </summary>
	/// <param name="maxValue">the maximum value of the bounds</param>
	/// <returns>a random decimal between <see cref="Decimal.Zero"/> and <paramref name="maxValue"/></returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
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
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Sign<T>(T number) where T : INumber<T>, IConvertible
	{
		return Int(T.Sign(number));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Min<T>(T num1, T num2) where T : INumber<T>
	{
		return T.Min(num1, num2);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Min<T>(T num1, T num2, T num3) where T : INumber<T>
	{
		return Min(Min(num1, num2), num3);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Max<T>(T num1, T num2) where T : INumber<T>
	{
		return T.Max(num1, num2);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Max<T>(T num1, T num2, T num3) where T : INumber<T>
	{
		return Max(Max(num1, num2), num3);
	}

	/// <summary> Returns the fibonacci of the given number </summary>
	/// <param name="number"> the number to get the fibonacci of </param>
	/// <returns> fibonacci of the given number  </returns>
	public static T Fibonacci<T>(T number) where T : IBinaryInteger<T>
	{
		var a = T.Zero;
		var b = T.One;

		// In N steps compute Fibonacci sequence iteratively.
		for (var i = T.Zero; i < number; i++)
		{
			(a, b) = (b, b + a);
		}

		return a;
	}

	/// <summary>
	/// Shuffles the given list and returns a copy which can be iterated over
	/// </summary>
	/// <param name="list">the items ot shuffle</param>
	/// <returns>a shuffled copy of the list</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static IEnumerable<T> Shuffle<T>(IEnumerable<T> list)
	{
		var array = list.ToArray(); // keeps track of count items shuffled

		Shuffle(array);

		return array;
	}

	/// <summary>
	/// Shuffles the given list
	/// </summary>
	/// <param name="list">the items ot shuffle</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Shuffle<T>(IList<T> list)
	{
		for (var i = 0; i < list.Count; i++)
		{
			var j = RandomInt(i, list.Count);

			list[j] = list[i];
		}
	}

	/// <summary>
	/// Shuffles the given data
	/// </summary>
	/// <param name="list">the data ot shuffle</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Shuffle<T>(Span<T> list)
	{
		for (var i = 0; i < list.Length; i++)
		{
			var j = RandomInt(i, list.Length);

			list[j] = list[i];
		}
	}

	/// <summary>
	/// Search for a value in a sorted list
	/// </summary>
	/// <param name="values">the sorted list to search though</param>
	/// <param name="value">the value to search for</param>
	/// <returns>the index of <paramref name="value"/></returns>
	public static int BinarySearch<T>(IList<T> values, T value) where T : IEqualityOperators<T, T>, IComparisonOperators<T, T>
	{
		var lo = 0;
		var hi = values.Count - 1;

		while (lo <= hi)
		{
			var i = lo + ((hi - lo) >> 1);

			if (values[i] == value)
			{
				return i;
			}

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

	/// <summary>
	/// Search for a value in a sorted list
	/// </summary>
	/// <param name="values">the sorted list to search though</param>
	/// <param name="value">the value to search for</param>
	/// <returns>the index of <paramref name="value"/></returns>
	public static int BinarySearch<T>(ReadOnlySpan<T> values, T value) where T : IEqualityOperators<T, T>, IComparisonOperators<T, T>
	{
		return MemoryExtensions.BinarySearch(values, value);
	}

	/// <summary>
	/// Search for a value in a sorted list
	/// </summary>
	/// <param name="values">the sorted list to search though</param>
	/// <param name="value">the value to search for</param>
	/// <returns>the index of <paramref name="value"/></returns>
	public static int InterpolationSearch<T>(IList<T> values, T value) where T : struct, INumber<T>, IConvertible
	{
		var lo = 0;
		var hi = values.Count - 1;

		while (lo <= hi)
		{
			var i = lo + hi - lo / Int(values[hi] - values[lo]) * Int(value - values[lo]);

			if (values[i] == value)
			{
				return i;
			}

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

	/// <summary>
	/// Search for a value in a sorted list
	/// </summary>
	/// <param name="values">the sorted list to search though</param>
	/// <param name="value">the value to search for</param>
	/// <returns>the index of <paramref name="value"/></returns>
	public static int InterpolationSearch<T>(ReadOnlySpan<T> values, T value) where T : struct, INumber<T>, IConvertible
	{
		var lo = 0;
		var hi = values.Length;

		while (lo <= hi)
		{
			var i = lo + hi - lo / Int(values[hi] - values[lo]) * Int(value - values[lo]);

			if (values[i] == value)
			{
				return i;
			}

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

	/// <summary> Converts an char to a String containing the equivalent binary notation </summary>
	/// <param name="value"> Value to convert </param>
	/// <returns> Returns the binary string </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Binary(Color value)
	{
		return Binary(value.GetHashCode());
	}

	/// <summary> Converts an object to a String containing the equivalent binary notation </summary>
	/// <param name="value"> Value to convert </param>
	/// <returns> Returns the binary string </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Binary(object? value)
	{
		return value is not null
			? Binary(value.GetHashCode())
			: System.String.Empty;
	}

	/// <summary> Converts an number to a String containing the equivalent binary notation </summary>
	/// <param name="value"> Value to convert </param>
	/// <returns> Returns the binary string </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Binary(Half value)
	{
		return Binary(BitConverter.HalfToInt16Bits(value));
	}

	/// <summary> Converts an number to a String containing the equivalent binary notation </summary>
	/// <param name="value"> Value to convert </param>
	/// <returns> Returns the binary string </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Binary(float value)
	{
		return Binary(BitConverter.SingleToInt32Bits(value));
	}

	/// <summary> Converts an number to a String containing the equivalent binary notation </summary>
	/// <param name="value"> Value to convert </param>
	/// <returns> Returns the binary string </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Binary(double value)
	{
		return Binary(BitConverter.DoubleToInt64Bits(value));
	}

	/// <summary> Converts an number to a String containing the equivalent binary notation </summary>
	/// <param name="value"> Value to convert </param>
	/// <returns> Returns the binary string </returns>
	public static string Binary<T>(T value) where T : IBinaryInteger<T>, IConvertible
	{
		return string.Create(Unsafe.SizeOf<T>() * 8, value, (buffer, number) =>
		{
			for (var i = buffer.Length - 1; i >= 0; i--)
			{
				buffer[i] = Char((number & T.One) + T.CreateChecked('0'));
				number >>= 1;
			}
		});
	}

	/// <summary> Converts a String representation of a binary number to its equivalent integer value </summary>
	/// <param name="value"> String to convert to an integer (must only contain 0 and 1's) </param>
	/// <returns> Returns the result of the conversion </returns>
	public static T UnBinary<T>(ReadOnlySpan<char> value) where T : IBinaryInteger<T>
	{
		var result = T.Zero;

		for (var i = value.Length - 1; i >= 0; i--)
		{
			var isOne = value[i] is '1';

			result += Unsafe.As<Boolean, T>(ref isOne) << (value.Length - 1 - i);
		}

		return result;
	}

	/// <summary> Converts an int or a byte to a String containing the equivalent hexadecimal notation </summary>
	/// <param name="value"> The value to convert to a hex value </param>
	/// <returns> Returns the hex value </returns>
	public static string Hex<T>(T value) where T : IBinaryInteger<T>, IConvertible
	{
		const string characters = "0123456789ABCDEF";

		var buffer = new ValueStringBuilder(stackalloc char[256]);
		var baseNumber = T.CreateChecked(characters.Length);

		while (value > T.Zero)
		{
			(value, var remainder) = T.DivRem(value, baseNumber);

			buffer.Insert(0, characters[Int(remainder)]);
		}

		return new string(buffer.AsSpan());
	}

	/// <summary> Converts an int or a byte to a String containing the equivalent hexadecimal notation </summary>
	/// <param name="value"> The value to convert to a hex value </param>
	/// <returns> Returns the hex value </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Hex(Color value)
	{
		return Hex(Unsafe.As<Color, Int32>(ref value));
	}

	/// <summary> Converts a String representation of a hexadecimal number to its equivalent integer value </summary>
	/// <param name="value"> String to convert to an integer </param>
	/// <returns> Returns a integer from a hexadecimal number </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T UnHex<T>(ReadOnlySpan<char> value) where T : INumber<T>
	{
		const string characters = "0123456789ABCDEF";

		long result = 0;
		long power = 1;

		for (var i = value.Length - 1; i >= 0; i--)
		{
			var index = characters.IndexOf(char.ToUpper(value[i]));

			if (index is -1)
			{
				throw new ArgumentException("Value must must only hex characters", nameof(value));
			}

			result += index * power;
			power *= 16;
		}

		return T.CreateChecked(result);
	}

	/// <summary>
	/// Converts the given value to a string, returns <see cref="String.Empty"/> if the value is null
	/// </summary>
	/// <param name="value">the value to convert</param>
	/// <returns>the converted value</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string? String<T>(T value) where T : notnull
	{
		return value.ToString();
	}

	/// <summary>
	/// Converts a value to a <see cref="Int32"/>
	/// </summary>
	/// <param name="value">the value to convert</param>
	/// <returns>the converted value</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Int<T>(T value) where T : INumber<T>
	{
		return ConvertNumber<T, Int32>(value);
	}

	/// <summary>
	/// Converts a value to a <see cref="System.Char"/>
	/// </summary>
	/// <param name="value">the value to convert</param>
	/// <returns>the converted value</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static char Char<T>(T value) where T : IConvertible
	{
		return value.ToChar(CultureInfo.InvariantCulture);
	}

	/// <summary>
	/// Converts a value to a <see cref="System.Boolean"/>
	/// </summary>						
	/// <param name="value">the value to convert</param>
	/// <returns>the converted value</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool Boolean<T>(T value) where T : IConvertible
	{
		return value.ToBoolean(CultureInfo.InvariantCulture);
	}

	/// <summary>
	/// Converts the given value to a float
	/// </summary>
	/// <param name="value">the value to convert to a float</param>
	/// <returns>the converted value</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float Float<T>(T value) where T : INumber<T>
	{
		return ConvertNumber<T, Single>(value);
	}

	/// <summary>
	/// Parses the given text to the given value
	/// </summary>
	/// <param name="text">the text to parse</param>
	/// <param name="result">the result of the parse</param>
	/// <returns>if the <see cref="text"/> can be parsed</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool TryParse<T>(ReadOnlySpan<char> text, out T result) where T : ISpanParsable<T>
	{
		return T.TryParse(text, null, out result);
	}

	/// <summary>
	/// Parses the given text to the given value
	/// </summary>
	/// <param name="text">the text to parse</param>
	/// <returns>the result of the parse</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Parse<T>(ReadOnlySpan<char> text) where T : ISpanParsable<T>
	{
		return T.Parse(text, CultureInfo.InvariantCulture);
	}

	/// <summary>
	/// Tries the estimate the time to finish the progress
	/// </summary>
	/// <param name="elapsedTime">the time that has elapsed</param>
	/// <param name="progress">the progress between 0 and 1</param>
	/// <returns>the estimated time</returns>
	public static TimeSpan EstimateTime(TimeSpan elapsedTime, float progress)
	{
		return ReciprocalEstimate(progress) * elapsedTime - elapsedTime;
	}

	/// <summary>
	/// Swaps two variables
	/// </summary>
	/// <param name="value1">variable one to swap</param>
	/// <param name="value2">variable tow to swap</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Swap<T>(ref T value1, ref T value2)
	{
		(value1, value2) = (value2, value1);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static TOther ConvertNumber<TNumber, TOther>(TNumber number) where TNumber : INumber<TNumber> where TOther : INumber<TOther>
	{
		return TOther.CreateChecked(number);
	}

	#endregion
}