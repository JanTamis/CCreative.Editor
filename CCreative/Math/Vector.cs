using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;

namespace CCreative;

[SkipLocalsInit]
public readonly struct Vector : IEnumerable<float>
{
	public readonly float X;
	public readonly float Y;
	public readonly float Z;

	public static readonly Vector Zero = new();

	public Vector(float number) : this(number, number, number)
	{

	}

	public Vector(float x, float y) : this(x, y, 0)
	{

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

		this = Unsafe.ReadUnaligned<Vector>(ref Unsafe.As<float, byte>(ref MemoryMarshal.GetReference(values)));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private Vector(in Vector128<float> vector)
	{
		Z = vector.GetElement(2);
		Y = vector.GetElement(1);
		X = vector.GetElement(0);
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
		return new Vector(X, Y, Z);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public float Heading()
	{
		return MathF.Atan2(Y, X);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public float Length()
	{
		return Math.Sqrt(LengthSquared());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public float LengthSquared()
	{
		var v = GetVector128();
		return Vector128.Dot(v, v);
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
	public static float AngleBetween(in Vector v1, in Vector v2)
	{
		var first = v1.GetVector128();
		var second = v2.GetVector128();

		// We get NaN if we pass in a zero vector which can cause problems
		// Zero seems like a reasonable angle between a (0,0,0) vector and something else
		if (first.Equals(Vector128<float>.Zero) || second.Equals(Vector128<float>.Zero))
		{
			return 0;
		}

		var dot = Vector128.Dot(first, second);
		var v1Mag = Vector128.Dot(first, first);
		var v2Mag = Vector128.Dot(second, second);

		// This should be a number between -1 and 1, since it's "normalized"
		var amt = dot / Math.Sqrt(v1Mag * v2Mag);
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
	public static Vector Cross(in Vector v1, in Vector v2)
	{
		if (Vector128.IsHardwareAccelerated)
		{
			var left = v1.GetVector128();
			var right = v2.GetVector128();

			var leftHandSide1 = Vector128.Shuffle(left, Vector128.Create(1, 2, 0, 0));
			var rightHandSide1 = Vector128.Shuffle(right, Vector128.Create(2, 0, 1, 0));

			var leftHandSide2 = Vector128.Shuffle(left, Vector128.Create(2, 0, 1, 0));
			var rightHandSide2 = Vector128.Shuffle(left, Vector128.Create(1, 2, 0, 0));

			var mul1 = Vector128.Multiply(leftHandSide1, rightHandSide1);
			var mul2 = Vector128.Multiply(leftHandSide2, rightHandSide2);

			var resultNonMaskedW = Vector128.Subtract(mul1, mul2);
			var vectorResult = Vector128.BitwiseAnd(resultNonMaskedW, Vector128.Create(-1f, -1f, -1f, 0));

			return new Vector(vectorResult);
		}

		var (x1, y1, z1) = v1;
		var (x2, y2, z2) = v2;

		return new Vector(
			y1 * z2 + z1 * y2,
			z1 * x2 + x1 * z2,
			x1 * y2 + y1 * x2);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float Distance(in Vector v1, in Vector v2)
	{
		return Math.Sqrt(DistanceSquared(in v1, in v2));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float DistanceSquared(in Vector v1, in Vector v2)
	{
		var difference = v1.GetVector128() - v2.GetVector128();

		return Vector128.Dot(difference, difference);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float Dot(in Vector v1, in Vector v2)
	{
		return Vector128.Dot(v1.GetVector128(), v2.GetVector128());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector FromAngle(float angle)
	{
		var (sin, cos) = MathF.SinCos(angle);

		return new Vector(cos, sin);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector Lerp(in Vector v1, in Vector v2, float amt)
	{
		if (Vector128.IsHardwareAccelerated)
		{
			var first = v1.GetVector128();
			var second = v2.GetVector128();

			Unsafe.SkipInit<Vector128<float>>(out var result);

			if (Fma.IsSupported)
			{
				result = Fma.MultiplyAdd(first, Vector128.Create(1f - amt), second * Vector128.Create(amt));
			}
			else
			{
				result = first * Vector128.Create(1f - amt) + second * Vector128.Create(amt);
			}

			return new Vector(result);
		}

		return v1 * (1f - amt) + v2 * amt;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector Limit(in Vector vector, float max)
	{
		return vector.LengthSquared() > Math.Sq(max)
			? Normalize(in vector) * max
			: vector;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector Normalize(in Vector vector)
	{
		return vector / vector.Length();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector Random2D()
	{
		return FromAngle(Math.Random(PConstants.TWO_PI));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector Random3D()
	{
		var angle = Math.Random(PConstants.TWO_PI);
		var (sin, cos) = MathF.SinCos(angle);

		var vz = Math.Random(-1, 1);
		vz = Math.Sqrt(Math.FusedMultiplySubtract(vz, vz, 1));

		var vx = vz * cos;
		var vy = vz * sin;

		return new Vector(vx, vy, vz);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector Rotate(in Vector vector, float theta)
	{
		var (sin, cos) = MathF.SinCos(theta);
		var (x, y) = vector;

		var xResult = Math.FusedMultiplySubtract(x, cos, y * sin);
		var yResult = Math.FusedMultiplyAdd(x, sin, y * cos);

		return new Vector(xResult, yResult);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector Reflect(in Vector inDirection, in Vector inNormal)
	{
		var factor = Vector128.Create(2f * Dot(inDirection, inNormal));
		var result = Fma.MultiplyAdd(factor, inDirection.GetVector128() - factor, inNormal.GetVector128());

		return new Vector(result);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector Project(in Vector vector, in Vector onNormal)
	{
		return onNormal * Dot(vector, onNormal) / onNormal.LengthSquared();
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
		return Add(v1, v2);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector operator +(in Vector v, float amt)
	{
		return new Vector(v.GetVector128() + Vector128.Create(amt));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector Add(in Vector v1, in Vector v2)
	{
		var first = v1.GetVector128();
		var second = v2.GetVector128();

		return new Vector(first + second);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector Add(in Vector v, float amt)
	{
		return new Vector(v.GetVector128() + Vector128.Create(amt));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector Add(ReadOnlySpan<Vector> vectors)
	{
		var result = Vector128<float>.Zero;

		for (var i = 0; i < vectors.Length; i++)
		{
			result += vectors[i].GetVector128();
		}

		return new Vector(result);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector operator -(in Vector v1, in Vector v2)
	{
		var first = v1.GetVector128();
		var second = v2.GetVector128();

		return new Vector(first - second);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector operator -(in Vector v, float amt)
	{
		return new Vector(v.GetVector128() - Vector128.Create(amt));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector operator *(in Vector v1, in Vector v2)
	{
		var first = v1.GetVector128();
		var second = v2.GetVector128();

		return new Vector(first * second);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector operator *(in Vector v, float amt)
	{
		return new Vector(v.GetVector128() * Vector128.Create(amt));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector operator /(in Vector v1, in Vector v2)
	{
		var first = v1.GetVector128();
		var second = v2.GetVector128();

		return new Vector(first / second);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector operator /(in Vector v, float amt)
	{
		return new Vector(v.GetVector128() / Vector128.Create(amt));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(in Vector other)
	{
		var first = GetVector128();
		var second = other.GetVector128();

		return Vector128.EqualsAny(first, second);
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

	[EditorBrowsable(EditorBrowsableState.Never)]
	public IEnumerator<float> GetEnumerator()
	{
		yield return X;
		yield return Y;
		yield return Z;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode()
	{
		return HashCode.Combine(X, Y, Z);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private unsafe Vector128<float> GetVector128()
	{
		var pointer = Unsafe.AsPointer(ref Unsafe.AsRef(this));

		if (IntPtr.Size is 8 && Sse2.IsSupported)
		{
			return Sse.Shuffle(
				Sse2.LoadScalarVector128((double*)pointer).AsSingle(),
				Sse.LoadScalarVector128((float*)pointer + 2),
				0x44);
		}

		if (AdvSimd.IsSupported)
		{
			return AdvSimd.LoadAndInsertScalar(AdvSimd.LoadVector64((float*)pointer).ToVector128(), 2, (float*)pointer + 2);
		}

		return Unsafe.As<Vector, Vector128<float>>(ref Unsafe.AsRef(this));
	}

	#endregion
}