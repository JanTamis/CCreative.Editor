using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using static CCreative.Math;

namespace CCreative;

// we need 16 byte for the SIMD registers
[StructLayout(LayoutKind.Sequential, Size = 16)]
public struct Vector : IEquatable<Vector>
{
	public float X, Y, Z;

	public static readonly Vector Zero;

	public Vector(float number)
	{
		X = Y = Z = number;
	}

	public Vector(float x, float y, float z = 0)
	{
		X = x;
		Y = y;
		Z = z;
	}

	public Vector(ReadOnlySpan<float> values)
	{
		if (values.Length < 3)
		{
			throw new ArgumentException($"{nameof(values)} must at least contain 3 elements", nameof(values));
		}
		
		this = Unsafe.As<float, Vector>(ref MemoryMarshal.GetReference(values));
	}

	public void Set(float x, float y, float z = 0)
	{
		X = x;
		Y = y;
		Z = z;
	}

	public void Set(Vector vector)
	{
		(X, Y, Z) = vector;
	}

	public void Set(ReadOnlySpan<float> data)
	{
		if (data.Length >= 2)
		{
			X = data[0];
			Y = data[1];
		}

		Z = data.Length >= 3
			? data[2]
			: 0f;
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

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public void Add(float number)
	{
		if (Vector128.IsHardwareAccelerated)
		{
			var result = Vector128.Add(
				Vector128.LoadUnsafe(ref X),
				Vector128.Create(number));

			result.StoreUnsafe(ref X);
		}
		else
		{
			X += number;
			Y += number;
			Z += number;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public void Add(Vector vector)
	{
		if (Vector128.IsHardwareAccelerated)
		{
			var result = Vector128.Add(
				Vector128.LoadUnsafe(ref X),
				Vector128.LoadUnsafe(ref vector.X));

			result.StoreUnsafe(ref X);
		}
		else
		{
			X += vector.X;
			Y += vector.Y;
			Z += vector.Z;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public void Subtract(float number)
	{
		if (Vector128.IsHardwareAccelerated)
		{
			var result = Vector128.Subtract(
				Vector128.LoadUnsafe(ref X),
				Vector128.Create(number));

			result.StoreUnsafe(ref X);
		}
		else
		{
			X -= number;
			Y -= number;
			Z -= number;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public void Subtract(Vector vector)
	{
		if (Vector128.IsHardwareAccelerated)
		{
			var result = Vector128.Subtract(
				Vector128.LoadUnsafe(ref X),
				Vector128.LoadUnsafe(ref vector.X));

			result.StoreUnsafe(ref X);
		}
		else
		{
			X -= vector.X;
			Y -= vector.Y;
			Z -= vector.Z;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public void Multiply(float number)
	{
		if (Vector128.IsHardwareAccelerated)
		{
			var result = Vector128.Multiply(
				Vector128.LoadUnsafe(ref X),
				Vector128.Create(number));

			result.StoreUnsafe(ref X);
		}
		else
		{
			X *= number;
			Y *= number;
			Z *= number;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public void Multiply(Vector vector)
	{
		if (Vector128.IsHardwareAccelerated)
		{
			var result = Vector128.Multiply(
				Vector128.LoadUnsafe(ref X),
				Vector128.LoadUnsafe(ref vector.X));

			result.StoreUnsafe(ref X);
		}
		else
		{
			X *= vector.X;
			Y *= vector.Y;
			Z *= vector.Z;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public void Divide(float number)
	{
		if (Vector128.IsHardwareAccelerated)
		{
			var result = Vector128.Divide(
				Vector128.LoadUnsafe(ref X),
				Vector128.Create(number));

			result.StoreUnsafe(ref X);
		}
		else
		{
			X /= number;
			Y /= number;
			Z /= number;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public void Divide(Vector vector)
	{
		if (Vector128.IsHardwareAccelerated)
		{
			var result = Vector128.Divide(
				Vector128.LoadUnsafe(ref X),
				Vector128.LoadUnsafe(ref vector.X));

			result.StoreUnsafe(ref X);
		}
		else
		{
			X /= vector.X;
			Y /= vector.Y;
			Z /= vector.Z;
		}
	}

	public float[] ToArray()
	{
		return new[] { X, Y, Z };
	}

	public Span<float> AsSpan()
	{
		return MemoryMarshal.CreateSpan(ref X, 3);
	}

	public Vector Copy()
	{
		return this;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public float Heading()
	{
		return MathF.Atan2(Y, X);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public float Magnitude()
	{
		return Sqrt(MagnitudeSquared());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public float MagnitudeSquared()
	{
		return Dot(this);
	}

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

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static Vector Add(Vector v1, Vector v2)
	{
		var result = Vector128.Add(
			Vector128.LoadUnsafe(ref v1.X),
			Vector128.LoadUnsafe(ref v2.X));

		result.StoreUnsafe(ref v1.X);

		return v1;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static Vector Add(Vector v, float m)
	{
		var result = Vector128.Add(
			Vector128.LoadUnsafe(ref v.X),
			Vector128.Create(m));

		result.StoreUnsafe(ref v.X);

		return v;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static Vector Subtract(Vector v1, Vector v2)
	{
		var result = Vector128.Subtract(
			Vector128.LoadUnsafe(ref v1.X),
			Vector128.LoadUnsafe(ref v2.X));

		result.StoreUnsafe(ref v1.X);

		return v1;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static Vector Subtract(Vector v, float number)
	{
		var result = Vector128.Subtract(
			Vector128.LoadUnsafe(ref v.X),
			Vector128.Create(number));

		result.StoreUnsafe(ref v.X);

		return v;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static Vector Divide(Vector v1, Vector v2)
	{
		var result = Vector128.Divide(
			Vector128.LoadUnsafe(ref v1.X),
			Vector128.LoadUnsafe(ref v2.X));

		result.StoreUnsafe(ref v1.X);

		return v1;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static Vector Divide(Vector v, float number)
	{
		var result = Vector128.Divide(
			Vector128.LoadUnsafe(ref v.X),
			Vector128.Create(number));

		Vector128.StoreUnsafe(result, ref v.X);

		return v;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static Vector Multiply(Vector v1, Vector v2)
	{
		var result = Vector128.Multiply(
			Vector128.LoadUnsafe(ref v1.X),
			Vector128.LoadUnsafe(ref v2.X));

		result.StoreUnsafe(ref v1.X);

		return v1;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static Vector Multiply(Vector v, float number)
	{
		var result = Vector128.Multiply(
			Vector128.LoadUnsafe(ref v.X),
			Vector128.Create(number));

		Vector128.StoreUnsafe(result, ref v.X);

		return v;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static float AngleBetween(Vector v1, Vector v2)
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

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static Vector Cross(Vector v1, Vector v2)
	{
		if (Sse.IsSupported)
		{
			var left = Vector128.LoadUnsafe(ref v1.X);
			var right = Vector128.LoadUnsafe(ref v2.X);

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

			var resultNonMaskedW = Sse.Subtract(mul1, mul2);

			var result = Sse.And(resultNonMaskedW, Vector128.Create(-1f, -1f, -1f, 0));

			result.StoreUnsafe(ref v1.X);

			return v1;
		}

		var (x1, y1, z1) = v1;
		var (x2, y2, z2) = v2;

		return new Vector(
			FusedMultiplySubtract(y1, z2, z1 * y2),
			FusedMultiplySubtract(z1, x2, x1 * z2),
			FusedMultiplySubtract(x1, y2, y1 * x2));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static float Distance(Vector v1, Vector v2)
	{
		return Sqrt(DistanceSquared(v1, v2));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static float DistanceSquared(Vector v1, Vector v2)
	{
		var difference = v1 - v2;
		return Dot(difference);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static float Dot(Vector v1, Vector v2)
	{
		return Vector128.Dot(
			Vector128.LoadUnsafe(ref v1.X),
			Vector128.LoadUnsafe(ref v2.X));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	private static float Dot(Vector v)
	{
		var vector = Vector128.LoadUnsafe(ref v.X);

		return Vector128.Dot(vector, vector);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static Vector FromAngle(float angle)
	{
		var (sin, cos) = MathF.SinCos(angle);

		return new Vector(cos, sin);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static Vector Lerp(Vector v1, Vector v2, float amt)
	{
		return FusedMultiplyAdd(v1, v2 - v1, new Vector(amt));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static Vector Lerp(Vector v1, Vector v2, Vector amt)
	{
		return FusedMultiplyAdd(v1, v2 - v1, amt);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static Vector Abs(Vector v)
	{
		Vector128
			.Abs(Vector128.LoadUnsafe(ref v.X))
			.StoreUnsafe(ref v.X);

		return v;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static Vector Min(Vector value1, Vector value2)
	{
		var result = Vector128.Min(
			Vector128.LoadUnsafe(ref value1.X),
			Vector128.LoadUnsafe(ref value2.X));

		Vector128.StoreUnsafe(result, ref value1.X);

		return value1;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static Vector Max(Vector value1, Vector value2)
	{
		var result = Vector128.Max(
			Vector128.LoadUnsafe(ref value1.X),
			Vector128.LoadUnsafe(ref value2.X));

		Vector128.StoreUnsafe(result, ref value1.X);

		return value1;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static Vector Clamp(Vector value1, Vector min, Vector max)
	{
		var result = Vector128.Min(
			Vector128.Max(
				Vector128.LoadUnsafe(ref value1.X),
				Vector128.LoadUnsafe(ref min.X)),
			Vector128.LoadUnsafe(ref max.X));

		Vector128.StoreUnsafe(result, ref value1.X);

		return value1;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static Vector SquareRoot(Vector value)
	{
		Vector128
			.Sqrt(Vector128.LoadUnsafe(ref value.X))
			.StoreUnsafe(ref value.X);

		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static Vector Square(Vector value)
	{
		var temp = Vector128.LoadUnsafe(ref value.X);

		Vector128
			.Multiply(temp, temp)
			.StoreUnsafe(ref value.X);

		return value;
	}

	public static float Sum(Vector value)
	{
		return Vector128.Sum(Vector128.LoadUnsafe(ref value.X));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static Vector Limit(Vector vector, float max)
	{
		return vector.MagnitudeSquared() > Sq(max)
			? Multiply(Normalize(vector), max)
			: vector;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static Vector Normalize(Vector vector)
	{
		var mag = vector.Magnitude();

		if (mag <= 1)
		{
			return vector;
		}

		return vector / mag;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static Vector Random2D()
	{
		return FromAngle(Random(PConstants.TWO_PI));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static Vector Random3D()
	{
		var angle = Random(PConstants.TWO_PI);
		var (sin, cos) = MathF.SinCos(angle);

		var vz = Random(-1, 1);

		var vx = Sqrt(FusedMultiplySubtract(vz, vz, 1)) * cos;
		var vy = Sqrt(FusedMultiplySubtract(vz, vz, 1)) * sin;

		return new Vector(vx, vy, vz);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static Vector Rotate(Vector vector, float theta)
	{
		var (sin, cos) = MathF.SinCos(theta);
		var (x, y) = vector;

		var xResult = Math.FusedMultiplySubtract(x, cos, y * sin);
		var yResult = Math.FusedMultiplyAdd(x, sin, y * cos);

		return new Vector(xResult, yResult);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static Vector Reflect(Vector inDirection, Vector inNormal)
	{
		var factor = new Vector(2f * Dot(inDirection, inNormal));

		return FusedMultiplyAdd(factor, inDirection - factor, inNormal);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static Vector Project(Vector vector, Vector onNormal)
	{
		var sqrMag = Dot(onNormal);
		var dot = Dot(vector, onNormal);

		return onNormal * dot / sqrMag;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static Vector SetMagnitude(Vector vector, float mag)
	{
		return Multiply(Normalize(vector), mag);
	}

	#endregion

	#region Operators

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static Vector operator *(Vector a, float scalar)
	{
		return Multiply(a, scalar);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static Vector operator *(Vector a, Vector b)
	{
		return Multiply(a, b);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static Vector operator /(Vector a, float scalar)
	{
		return Divide(a, scalar);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static Vector operator /(Vector a, Vector b)
	{
		return Divide(a, b);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static Vector operator +(Vector a, Vector b)
	{
		return Add(a, b);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static Vector operator +(Vector a, float amt)
	{
		return Add(a, amt);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static Vector operator -(Vector a, Vector b)
	{
		return Subtract(a, b);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static Vector operator -(Vector a, float amt)
	{
		return Subtract(a, amt);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static bool operator ==(Vector a, Vector b)
	{
		return a.Equals(b);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static bool operator !=(Vector a, Vector b)
	{
		return !(a == b);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public bool Equals(Vector other)
	{
		return Vector128.EqualsAll(
			Vector128.LoadUnsafe(ref X),
			Vector128.LoadUnsafe(ref other.X));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public override bool Equals(object? obj)
	{
		return obj is Vector v && Equals(v);
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

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public override int GetHashCode()
	{
		return HashCode.Combine(X, Y, Z);
	}

	/// <summary>Returns the string representation of the current instance using default formatting.</summary>
	/// <returns>The string representation of the current instance.</returns>
	/// <remarks>This method returns a string in which each element of the vector is formatted using the "G" (general) format string and the formatting conventions of the current thread culture. The "&lt;" and "&gt;" characters are used to begin and end the string, and the current culture's <see cref="System.Globalization.NumberFormatInfo.NumberGroupSeparator" /> property followed by a space is used to separate each element.</remarks>
	public override string ToString()
	{
		return $"<{X:G}, {Y:G}, {Z:G}>";
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	private static Vector FusedMultiplyAdd(Vector a, Vector b, Vector addend)
	{
		if (Fma.IsSupported)
		{
			var result = Fma.MultiplyAdd(
				Vector128.LoadUnsafe(ref a.X),
				Vector128.LoadUnsafe(ref b.X),
				Vector128.LoadUnsafe(ref addend.X));

			result.StoreUnsafe(ref addend.X);

			return addend;
		}

		return a * b + addend;
	}

	#endregion
}