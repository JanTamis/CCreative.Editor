using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using static CCreative.Math;

namespace CCreative;

[SkipLocalsInit]
public readonly record struct Vector : IEnumerable<float>
{
	public readonly float X;
	public readonly float Y;
	public readonly float Z;

	public static readonly Vector Zero = new();

	public Vector()
	{
		X = Y = Z = 0f;
	}

	public Vector(float number)
	{
		X = Y = Z = number;
	}
	
	public Vector(float x, float y)
	{
		X = x;
		Y = y;
		Z = 0f;
	}

	public Vector(float x, float y, float z)
	{
		X = x;
		Y = y;
		Z = z;
	}

	public Vector(ReadOnlySpan<float> values)
	{
		if (values.Length < 3)
		{
			throw new ArgumentOutOfRangeException(nameof(values), $"{nameof(values)} must at least contain 3 elements");
		}
		
		this = Unsafe.As<float, Vector>(ref MemoryMarshal.GetReference(values));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private Vector(in Vector128<float> vector)
	{
		AsVector(in vector, out this);
	}

	public float this[int index]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return index switch
			{
				0 => X,
				1 => Y,
				2 => Z,
				_ => throw new IndexOutOfRangeException(),
			};
		}
	}

	#region Instance Functions
	
	public float[] ToArray()
	{
		return new[] { X, Y, Z };
	}

	public ReadOnlySpan<float> AsSpan()
	{
		return MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef(in X), 3);
	}

	public Vector Copy()
	{
		return this;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public float Heading()
	{
		return MathF.Atan2(Y, X);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public float Magnitude()
	{
		return Sqrt(MagnitudeSquared());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public float MagnitudeSquared()
	{
		return Dot(GetVector128(in this));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void CopyTo(Span<float> destination)
	{
		if (destination.Length < 3)
		{
			throw new ArgumentException($"Length of {nameof(destination)} must be at least 3 elements long", nameof(destination));
		}

		Unsafe.WriteUnaligned(ref Unsafe.As<float, byte>(ref MemoryMarshal.GetReference(destination)), this);
	}

	#endregion

	#region Static Functions

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Add(in Vector v1, in Vector v2, out Vector result)
	{
		var vectorResult = GetVector128(in v1) + GetVector128(in v2);
		
		AsVector(in vectorResult, out result);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Add(in Vector v1, float number, out Vector result)
	{
		var vectorResult = GetVector128(in v1) + Vector128.Create(number);

		AsVector(in vectorResult, out result);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Subtract(in Vector v1, in Vector v2, out Vector result)
	{
		var vectorResult = GetVector128(in v1) - GetVector128(in v2);

		AsVector(in vectorResult, out result);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Subtract(in Vector v1, float number, out Vector result)
	{
		var vectorResult = GetVector128(in v1) - Vector128.Create(number);

		AsVector(in vectorResult, out result);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Multiply(in Vector v1, in Vector v2, out Vector result)
	{
		var vectorResult = GetVector128(in v1) * GetVector128(in v2);

		AsVector(in vectorResult, out result);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Multiply(in Vector v1, float number, out Vector result)
	{
		var vectorResult = GetVector128(in v1) * Vector128.Create(number);
		
		AsVector(in vectorResult, out result);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Divide(in Vector v1, in Vector v2, out Vector result)
	{
		var vectorResult = GetVector128(in v1) / GetVector128(in v2);

		AsVector(in vectorResult, out result);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Divide(in Vector v, float number, out Vector result)
	{
		var vectorResult = GetVector128(in v) / Vector128.Create(number);

		AsVector(in vectorResult, out result);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float AngleBetween(in Vector v1, in Vector v2)
	{
		// We get NaN if we pass in a zero vector which can cause problems
		// Zero seems like a reasonable angle between a (0,0,0) vector and something else
		if (v1.Equals(default) || v2.Equals(default))
		{
			return 0;
		}

		var dot = Dot(v1, v2);
		var v1Mag = v1.Magnitude();
		var v2Mag = v2.Magnitude();

		// This should be a number between -1 and 1, since it's "normalized"
		var amt = dot / (v1Mag * v2Mag);
		// But if it's not due to rounding error, then we need to fix it
		// http://code.google.com/p/processing/issues/detail?id=340
		// Otherwise if outside the range, acos() will return NaN
		// http://www.cppreference.com/wiki/c/math/acos

		return amt switch
		{
			<= -1 => MathF.PI,
			>= 1 => 0,
			_ => MathF.Acos(amt),
		};
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Cross(in Vector v1, in Vector v2, out Vector result)
	{
		if (Sse.IsSupported)
		{
			var left = GetVector128(in v1);
			var right = GetVector128(in v2);

			// Cross product of A(x, y, z, _) and B(x, y, z, _) is
			//                    0  1  2  3        0  1  2  3
			//
			// '(X = (Ay * Bz) - (Az * By), Y = (Az * Bx) - (Ax * Bz), Z = (Ax * By) - (Ay * Bx)'
			//           1           2              1           2              1            2
			// So we can do (Ay, Az, Ax, _) * (Bz, Bx, By, _) (last elem is irrelevant, as this is for Vector3)
			// which leaves us with a of the first subtraction element for each (marked 1 above)
			// Then we repeat with the right hand of subtractions (Az, Ax, Ay, _) * (By, Bz, Bx, _)
			// which leaves us with the right hand sides (marked 2 above)
			// Then we subtract them to get the correct vector
			// We then mask out W to zero, because that is required for the Vector3 representation

			// lhs1 goes from x, y, z, _ to y, z, x, _
			// rhs1 goes from x, y, z, _ to z, x, y, _
			var leftHandSide1 = Sse.Shuffle(left, left, 201);
			var rightHandSide1 = Sse.Shuffle(right, right, 210);

			// lhs2 goes from x, y, z, _ to z, x, y, _
			// rhs2 goes from x, y, z, _ to y, z, x, _
			var leftHandSide2 = Sse.Shuffle(left, left, 210);
			var rightHandSide2 = Sse.Shuffle(right, right, 201);

			var mul1 = Sse.Multiply(leftHandSide1, rightHandSide1);
			var mul2 = Sse.Multiply(leftHandSide2, rightHandSide2);

			var resultNonMaskedW = Vector128.Subtract(mul1, mul2);
			var vectorResult = Sse.And(resultNonMaskedW, Vector128.Create(-1f, -1f, -1f, 0));

			AsVector(in vectorResult, out result);
		}

		var (x1, y1, z1) = v1;
		var (x2, y2, z2) = v2;

		result = new Vector(
			FusedMultiplySubtract(y1, z2, z1 * y2),
			FusedMultiplySubtract(z1, x2, x1 * z2),
			FusedMultiplySubtract(x1, y2, y1 * x2));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float Distance(in Vector v1, in Vector v2)
	{
		return Sqrt(DistanceSquared(in v1, in v2));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float DistanceSquared(in Vector v1, in Vector v2)
	{
		var difference = GetVector128(in v1) - GetVector128(in v2);
		return Dot(in difference);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float Dot(in Vector v1, in Vector v2)
	{
		return Vector128.Dot(GetVector128(in v1), GetVector128(in v2));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector FromAngle(float angle)
	{
		var (sin, cos) = MathF.SinCos(angle);

		return new Vector(cos, sin);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Lerp(in Vector v1, in Vector v2, float amt, out Vector result)
	{
		var first = GetVector128(in v1);
		var second = GetVector128(in v2);

		var temp = Fma.MultiplyAdd(
			first,
			second - first,
			Vector128.Create(amt));

		AsVector(in temp, out result);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector Lerp(in Vector v1, in Vector v2, in Vector amt)
	{
		var first = GetVector128(in v1);
		var second = GetVector128(in v2);

		var result = Fma.MultiplyAdd(
			first,
			second - first,
			GetVector128(in amt));

		return new Vector(result);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector Abs(in Vector v)
	{
		var result = Vector128.Abs(GetVector128(in v));
		
		return new Vector(result);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector Min(in Vector v1, in Vector v2)
	{
		var result = Vector128.Min(GetVector128(in v1), GetVector128(in v2));
		
		return new Vector(result);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector Max(in Vector v1, in Vector v2)
	{
		var result = Vector128.Max(GetVector128(in v1), GetVector128(in v2));
		
		return new Vector(result);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector Clamp(in Vector v, in Vector min, in Vector max)
	{
		var temp = Vector128.Max(GetVector128(in v), GetVector128(in min));
		var result = Vector128.Min(temp, GetVector128(in max));

		return new Vector(result);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector Limit(in Vector vector, float max)
	{
		return vector.MagnitudeSquared() > Sq(max)
			? Normalize(in vector) * max
			: vector;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector Normalize(in Vector vector)
	{
		var mag = vector.Magnitude();

		if (mag <= 1)
		{
			return vector;
		}

		return vector / mag;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector Random2D()
	{
		return FromAngle(Random(PConstants.TWO_PI));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector Random3D()
	{
		var angle = Random(PConstants.TWO_PI);
		var (sin, cos) = MathF.SinCos(angle);

		var vz = Random(-1, 1);

		var temp = Sqrt(FusedMultiplySubtract(vz, vz, 1));

		var vx = temp * cos;
		var vy = temp * sin;

		return new Vector(vx, vy, vz);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector Rotate(in Vector vector, float theta)
	{
		var (sin, cos) = MathF.SinCos(theta);
		var (x, y) = vector;

		var xResult = FusedMultiplySubtract(x, cos, y * sin);
		var yResult = FusedMultiplyAdd(x, sin, y * cos);

		return new Vector(xResult, yResult);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector Reflect(in Vector inDirection, in Vector inNormal)
	{
		var factor = Vector128.Create(2f * Dot(inDirection, inNormal));
		var result = Fma.MultiplyAdd(factor, GetVector128(in inDirection) - factor, GetVector128(in inNormal));

		return new Vector(result);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector Project(in Vector vector, in Vector onNormal)
	{
		var sqrMag = Dot(GetVector128(in onNormal));
		var dot = Dot(vector, onNormal);

		return onNormal * dot / sqrMag;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector SetMagnitude(in Vector vector, float mag)
	{
		return Normalize(in vector) * mag;
	}

	#endregion

	#region Operators

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector operator +(in Vector v1, in Vector v2)
	{
		Add(in v1, in v2, out var result);
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector operator +(in Vector v, float amt)
	{
		return new Vector(GetVector128(in v) + Vector128.Create(amt));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector operator -(in Vector v1, in Vector v2)
	{
		return new Vector(GetVector128(in v1) - GetVector128(in v2));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector operator -(in Vector v, float amt)
	{
		return new Vector(GetVector128(in v) + Vector128.Create(amt));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector operator *(in Vector v1, in Vector v2)
	{
		return new Vector(GetVector128(in v1) * GetVector128(in v2));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector operator *(in Vector v, float amt)
	{
		return new Vector(GetVector128(in v) + Vector128.Create(amt));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector operator /(in Vector v1, in Vector v2)
	{
		return new Vector(GetVector128(in v1) / GetVector128(in v2));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector operator /(in Vector v, float amt)
	{
		return new Vector(GetVector128(in v) + Vector128.Create(amt));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(in Vector other)
	{
		return Vector128.EqualsAll(GetVector128(in this), GetVector128(in other));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public void Deconstruct(out float x, out float y)
	{
		x = X;
		y = Y;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public void Deconstruct(out float x, out float y, out float z)
	{
		x = X;
		y = Y;
		z = Z;
	}

	public IEnumerator<float> GetEnumerator()
	{
		yield return X;
		yield return Y;
		yield return Z;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode()
	{
		return HashCode.Combine(X, Y, Z);
	}

	/// <summary>Returns the string representation of the current instance using default formatting.</summary>
	/// <returns>The string representation of the current instance.</returns>
	public override string ToString()
	{
		return $"<{X:G}, {Y:G}, {Z:G}>";
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static unsafe Vector128<float> GetVector128(in Vector vector)
	{
		if (IntPtr.Size is 8 && Sse.IsSupported)
		{
			var pointer = Unsafe.AsPointer(ref Unsafe.AsRef(vector));
		
			return Sse.Shuffle(
				Sse2.LoadScalarVector128((double*)pointer).AsSingle(),
				Sse.LoadScalarVector128((float*)pointer + 2),
				0x44);
		}

		return Unsafe.As<Vector, Vector128<float>>(ref Unsafe.AsRef(in vector));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	private static unsafe void AsVector(in Vector128<float> vector, out Vector result)
	{
		Unsafe.SkipInit(out result);

		var pointer = Unsafe.AsPointer(ref result);

		Sse2.StoreScalar((double*)pointer, vector.AsDouble());
		Sse.StoreScalar((float*)pointer + 2, Sse.Shuffle(vector, vector, 2));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static float Dot(in Vector128<float> vector)
	{
		return Vector128.Dot(vector, vector);
	}

	#endregion
}