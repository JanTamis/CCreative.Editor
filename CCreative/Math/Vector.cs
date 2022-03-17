using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;
using System.Runtime.Versioning;
using CCreative.Helpers;
using static CCreative.Math;

namespace CCreative
{
	[RequiresPreviewFeatures]
	public struct Vector : IEquatable<Vector>,
		ISpanParseable<Vector>,
		IMultiplyOperators<Vector, Vector, Vector>,
		IMultiplyOperators<Vector, float, Vector>,
		IAdditionOperators<Vector, Vector, Vector>,
		IAdditionOperators<Vector, float, Vector>,
		IDivisionOperators<Vector, Vector, Vector>,
		IDivisionOperators<Vector, float, Vector>,
		ISubtractionOperators<Vector, Vector, Vector>,
		ISubtractionOperators<Vector, float, Vector>
	{
		public float X { get; set; }
		public float Y { get; set; }
		public float Z { get; set; }

		public Vector(float number)
		{
			X = number;
			Y = number;
			Z = number;
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
				throw new ArgumentException("Values must contain 3 elements", nameof(values));
			}

			this = Unsafe.ReadUnaligned<Vector>(ref Unsafe.As<float, byte>(ref MemoryMarshal.GetReference(values)));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Set(float x, float y)
		{
			Set(x, y, 0);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Set(float x, float y, float z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		public void Set(ReadOnlySpan<float> values)
		{
			if (values.Length >= 2)
			{
				X = values[0];
				Y = values[1];
			}

			if (values.Length >= 3)
			{
				Z = values[2];
			}
			else
			{
				Z = 0f;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Set(Vector vector)
		{
			(X, Y, Z) = vector;
		}

		public void Set(float[]? data)
		{
			if (data is not null)
			{
				Set(data.AsSpan());
			}
		}

		public float this[int index] => index switch
		{
			0 => X,
			1 => Y,
			2 => Z,
			_ => throw new ArgumentException("Invalid index, must be between 0 and 2 (including)", nameof(index)),
		};

		#region Instance Functions

		public float[] ToArray()
		{
			return new[] { X, Y, Z };
		}

		public ReadOnlySpan<float> AsSpan()
		{
			return MemoryMarshal.Cast<Vector, float>(MemoryMarshal.CreateReadOnlySpan(ref this, 1));
		}

		public Vector Copy()
		{
			return this;
		}

		public float Heading()
		{
			return MathF.Atan2(Y, X);
		}

		public float Magnitude()
		{
			return Sqrt(MagnitudeSquared());
		}

		public float MagnitudeSquared()
		{
			return Dot(this, this);
		}

		public void CopyTo(Span<float> destination)
		{
			if (destination.Length < 3)
			{
				throw new ArgumentException("Length of destination must be at least 3 elements long", nameof(destination));
			}

			Unsafe.WriteUnaligned(ref Unsafe.As<float, byte>(ref MemoryMarshal.GetReference(destination)), this);
		}
		
		#endregion

		#region Static Functions

		public static Vector Add(Vector v1, Vector v2)
		{
			var temp1 = Unsafe.As<Vector, Vector128<float>>(ref v1);
			var temp2 = Unsafe.As<Vector, Vector128<float>>(ref v2);

			if (Sse.IsSupported)
			{
				var result = Sse.Add(temp1, temp2);

				return Unsafe.As<Vector128<float>, Vector>(ref result);
			}

			if (AdvSimd.IsSupported)
			{
				var result = AdvSimd.Add(temp1, temp2);

				return Unsafe.As<Vector128<float>, Vector>(ref result);
			}

			var (x1, y1, z1) = v1;
			var (x2, y2, z2) = v2;

			return new Vector(
				x1 + x2,
				y1 + y2,
				z1 + z2);
		}

		public static Vector Add(Vector v1, float amt)
		{
			return Add(v1, new Vector(amt));
		}

		public static Vector Subtract(Vector v1, Vector v2)
		{
			var temp1 = Unsafe.As<Vector, Vector128<float>>(ref v1);
			var temp2 = Unsafe.As<Vector, Vector128<float>>(ref v2);

			if (Sse.IsSupported)
			{
				var result = Sse.Subtract(temp1, temp2);

				return Unsafe.As<Vector128<float>, Vector>(ref result);
			}

			if (AdvSimd.IsSupported)
			{
				var result = AdvSimd.Subtract(temp1, temp2);

				return Unsafe.As<Vector128<float>, Vector>(ref result);
			}

			var (x1, y1, z1) = v1;
			var (x2, y2, z2) = v2;

			return new Vector(
				x1 - x2,
				y1 - y2,
				z1 - z2);
		}

		public static Vector Subtract(Vector v1, float amt)
		{
			return Subtract(v1, new Vector(amt));
		}

		public static Vector Divide(Vector v1, Vector v2)
		{
			var temp1 = Unsafe.As<Vector, Vector128<float>>(ref v1);
			var temp2 = Unsafe.As<Vector, Vector128<float>>(ref v2);

			if (Sse.IsSupported)
			{
				var result = Sse.Divide(temp1, temp2);

				return Unsafe.As<Vector128<float>, Vector>(ref result);
			}

			var (x1, y1, z1) = v1;
			var (x2, y2, z2) = v2;

			return new Vector(
				x1 / x2,
				y1 / y2,
				z1 / z2);
		}

		public static Vector Divide(Vector v, float n)
		{
			return Divide(v, new Vector(n));
		}

		public static Vector Multiply(Vector v1, Vector v2)
		{
			var temp1 = Unsafe.As<Vector, Vector128<float>>(ref v1);
			var temp2 = Unsafe.As<Vector, Vector128<float>>(ref v2);

			if (Sse.IsSupported)
			{
				var result = Sse.Multiply(temp1, temp2);

				return Unsafe.As<Vector128<float>, Vector>(ref result);
			}

			if (AdvSimd.IsSupported)
			{
				var result = AdvSimd.Multiply(temp1, temp2);

				return Unsafe.As<Vector128<float>, Vector>(ref result);
			}

			var (x1, y1, z1) = v1;
			var (x2, y2, z2) = v2;

			return new Vector(
				x1 * x2,
				y1 * y2,
				z1 * z2);
		}

		public static Vector Multiply(Vector v, float n)
		{
			return Multiply(v, new Vector(n));
		}

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

		public static Vector Cross(Vector v1, Vector v2)
		{
			if (Sse.IsSupported)
			{
				var temp1 = Unsafe.As<Vector, Vector128<float>>(ref v1);
				var temp2 = Unsafe.As<Vector, Vector128<float>>(ref v2);

				var left1 = Sse.Shuffle(temp1, temp1, 201);
				var vector128 = Sse.Shuffle(temp2, temp2, 210);
				var left2 = Sse.Shuffle(temp1, temp1, 210);
				var right1 = Sse.Shuffle(temp2, temp2, 201);

				var result = Sse.And(Sse.Subtract(Sse.Multiply(left1, vector128), Sse.Multiply(left2, right1)), MathSharp.Vector.SingleConstants.MaskW);

				return Unsafe.As<Vector128<float>, Vector>(ref result);
			}

			var (x1, y1, z1) = v1;
			var (x2, y2, z2) = v2;

			return new Vector(
				FusedMultiplySubtract(y1, z2, z1 * y2),
				FusedMultiplySubtract(z1, x2, x1 * z2),
				FusedMultiplySubtract(x1, y2, y1 * x2));
		}

		public static float Distance(Vector v1, Vector v2)
		{
			return Sqrt(DistanceSquared(v1, v2));
		}

		public static float DistanceSquared(Vector v1, Vector v2)
		{
			var difference = v1 - v2;
			return Dot(difference, difference);
		}

		public static float Dot(Vector v1, Vector v2)
		{
			var temp1 = Unsafe.As<Vector, Vector128<float>>(ref v1);
			var temp2 = Unsafe.As<Vector, Vector128<float>>(ref v2);

			if (Sse41.IsSupported)
			{
				return Sse41.DotProduct(temp1, temp2, 127).ToScalar();
			}

			if (Sse3.IsSupported)
			{
				var result1 = Sse.And(Sse.Multiply(temp1, temp2), MathSharp.Vector.SingleConstants.MaskW);
				var result2 = Sse3.HorizontalAdd(result1, result1);
				return Sse3.HorizontalAdd(result2, result2).ToScalar();
			}

			if (Sse.IsSupported)
			{
				var result1 = Sse.Multiply(temp1, temp2);
				var result2 = Sse.Shuffle(result1, result1, 153);
				var result3 = Sse.AddScalar(Sse.AddScalar(result1, result2), Sse.Shuffle(result2, result2, 85));

				return Sse.Shuffle(result3, result3, 0).ToScalar();
			}

			var (x1, y1, z1) = v1;
			var (x2, y2, z2) = v2;

			return FusedMultiplyAdd(x1, x2, FusedMultiplyAdd(y1, y2, z1 * z2));
		}

		public static Vector FromAngle(float angle)
		{
			var (sin, cos) = MathF.SinCos(angle);

			return new Vector(cos, sin);
		}

		public static Vector Lerp(Vector v1, Vector v2, float amt)
		{
			return FusedMultiplyAdd(v1, v2 - v1, new Vector(amt));
		}

		public static Vector Limit(Vector vector, float max)
		{
			return vector.MagnitudeSquared() > Sq(max)
				? Multiply(Normalize(vector), max)
				: vector;
		}

		public static Vector Normalize(Vector vector)
		{
			return vector / vector.Magnitude();
		}

		public static Vector Random2D()
		{
			return FromAngle(Random(PConstants.TWO_PI));
		}

		public static Vector Random3D()
		{
			var angle = Random(PConstants.TWO_PI);
			var (sin, cos) = MathF.SinCos(angle);

			var vz = Random(-1, 1);

			var vx = Sqrt(FusedMultiplySubtract(vz, vz, 1)) * cos;
			var vy = Sqrt(FusedMultiplySubtract(vz, vz, 1)) * sin;

			return new Vector(vx, vy, vz);
		}

		public static Vector Rotate(Vector vector, float theta)
		{
			var (sin, cos) = MathF.SinCos(theta);
			var (x, y) = vector;

			var xResult = FusedMultiplySubtract(x, cos, y * sin);
			var yResult = FusedMultiplyAdd(x, sin, y * cos);

			return new Vector(xResult, yResult);
		}

		public static Vector Reflect(Vector inDirection, Vector inNormal)
		{
			var factor = new Vector(2f * Dot(inDirection, inNormal));

			return FusedMultiplyAdd(factor, inDirection - factor, inNormal);
		}

		public static Vector Project(Vector vector, Vector onNormal)
		{
			var sqrMag = Dot(onNormal, onNormal);
			var dot = Dot(vector, onNormal);

			return onNormal * dot / sqrMag;
		}

		public static Vector Min(Vector v1, Vector v2)
		{
			if (Sse.IsSupported)
			{
				var temp1 = Unsafe.As<Vector, Vector128<float>>(ref v1);
				var temp2 = Unsafe.As<Vector, Vector128<float>>(ref v2);

				var result = Sse.Min(temp1, temp2);

				return Unsafe.As<Vector128<float>, Vector>(ref result);
			}

			if (AdvSimd.IsSupported)
			{
				var temp1 = Unsafe.As<Vector, Vector128<float>>(ref v1);
				var temp2 = Unsafe.As<Vector, Vector128<float>>(ref v2);

				var result = AdvSimd.Min(temp1, temp2);

				return Unsafe.As<Vector128<float>, Vector>(ref result);
			}

			var (x1, y1, z1) = v1;
			var (x2, y2, z2) = v2;

			return new Vector(
				MathF.Min(x1, x2),
				MathF.Min(y1, y2),
				MathF.Min(z1, z2));
		}

		public static Vector Max(Vector v1, Vector v2)
		{
			var temp1 = Unsafe.As<Vector, Vector128<float>>(ref v1);
			var temp2 = Unsafe.As<Vector, Vector128<float>>(ref v2);

			if (Sse.IsSupported)
			{
				var result = Sse.Max(temp1, temp2);

				return Unsafe.As<Vector128<float>, Vector>(ref result);
			}

			if (AdvSimd.IsSupported)
			{
				var result = AdvSimd.Max(temp1, temp2);

				return Unsafe.As<Vector128<float>, Vector>(ref result);
			}

			var (x1, y1, z1) = v1;
			var (x2, y2, z2) = v2;

			return new Vector(
				MathF.Max(x1, x2),
				MathF.Max(y1, y2),
				MathF.Max(z1, z2));
		}

		public static Vector SetMagnitude(Vector vector, float mag)
		{
			return Multiply(Normalize(vector), mag);
		}

		#endregion

		#region Operators

		public static Vector operator *(Vector a, float scalar)
		{
			return Multiply(a, scalar);
		}

		public static Vector operator *(Vector a, Vector b)
		{
			return Multiply(a, b);
		}

		public static Vector operator /(Vector a, float scalar)
		{
			return Divide(a, scalar);
		}

		public static Vector operator /(Vector a, Vector b)
		{
			return Divide(a, b);
		}

		public static Vector operator +(Vector a, Vector b)
		{
			return Add(a, b);
		}

		public static Vector operator +(Vector a, float amt)
		{
			return Add(a, amt);
		}

		public static Vector operator -(Vector a, Vector b)
		{
			return Subtract(a, b);
		}

		public static Vector operator -(Vector a, float amt)
		{
			return Subtract(a, amt);
		}

		public static bool operator ==(Vector a, Vector b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(Vector a, Vector b)
		{
			return !(a == b);
		}

		public bool Equals(Vector other)
		{
			var (x1, y1, z1) = other;

			return X.Equals(x1) && Y.Equals(y1) && Z.Equals(z1);
		}

		public override bool Equals(object? obj)
		{
			return obj is Vector v && Equals(v);
		}

		public void Deconstruct(out float x, out float y)
		{
			x = X;
			y = Y;
		}

		public void Deconstruct(out float x, out float y, out float z)
		{
			x = X;
			y = Y;
			z = Z;
		}

		public static implicit operator (float x, float y)(Vector vector)
		{
			return (vector.X, vector.Y);
		}

		public static implicit operator (float x, float y, float z)(Vector vector)
		{
			return (vector.X, vector.Y, vector.Z);
		}

		public static implicit operator Vector((float x, float y) data)
		{
			return new Vector(data.x, data.y, 0);
		}

		public static implicit operator Vector((float x, float y, float z) data)
		{
			return new Vector(data.x, data.y, data.z);
		}

		public override int GetHashCode()
		{
			var result = 1f;

			result = FusedMultiplyAdd(31f, result, X);
			result = FusedMultiplyAdd(31f, result, Y);
			result = FusedMultiplyAdd(31f, result, Z);

			return BitConverter.SingleToInt32Bits(result);
		}

		/// <summary>Returns the string representation of the current instance using default formatting.</summary>
		/// <returns>The string representation of the current instance.</returns>
		/// <remarks>This method returns a string in which each element of the vector is formatted using the "G" (general) format string and the formatting conventions of the current thread culture. The "&lt;" and "&gt;" characters are used to begin and end the string, and the current culture's <see cref="System.Globalization.NumberFormatInfo.NumberGroupSeparator" /> property followed by a space is used to separate each element.</remarks>
		public override string ToString()
		{
			return ToString("G");
		}

		/// <summary>Returns the string representation of the current instance using the specified format string to format individual elements.</summary>
		/// <param name="format">A standard or custom numeric format string that defines the format of individual elements.</param>
		/// <returns>The string representation of the current instance.</returns>
		/// <remarks>This method returns a string in which each element of the vector is formatted using <paramref name="format" /> and the current culture's formatting conventions. The "&lt;" and "&gt;" characters are used to begin and end the string, and the current culture's <see cref="System.Globalization.NumberFormatInfo.NumberGroupSeparator" /> property followed by a space is used to separate each element.</remarks>
		public string ToString(string? format)
		{
			return ToString(format, CultureInfo.CurrentCulture);
		}

		/// <summary>Returns the string representation of the current instance using the specified format string to format individual elements and the specified format provider to define culture-specific formatting.</summary>
		/// <param name="format">A standard or custom numeric format string that defines the format of individual elements.</param>
		/// <param name="formatProvider">A format provider that supplies culture-specific formatting information.</param>
		/// <returns>The string representation of the current instance.</returns>
		/// <remarks>This method returns a string in which each element of the vector is formatted using <paramref name="format" /> and <paramref name="formatProvider" />. The "&lt;" and "&gt;" characters are used to begin and end the string, and the format provider's <see cref="System.Globalization.NumberFormatInfo.NumberGroupSeparator" /> property followed by a space is used to separate each element.</remarks>
		public string ToString(string? format, IFormatProvider? formatProvider)
		{
			var separator = NumberFormatInfo.GetInstance(formatProvider).NumberGroupSeparator;
			var handler = new DefaultInterpolatedStringHandler(6, 3, formatProvider, stackalloc char[256]);

			handler.AppendLiteral("<");
			handler.AppendFormatted(X.ToString(format, formatProvider));
			handler.AppendLiteral(separator);
			handler.AppendLiteral(" ");
			handler.AppendFormatted(Y.ToString(format, formatProvider));
			handler.AppendLiteral(separator);
			handler.AppendLiteral(" ");
			handler.AppendFormatted(Z.ToString(format, formatProvider));
			handler.AppendLiteral(">");
			return handler.ToStringAndClear();
		}

		#endregion

		public static Vector Parse(string s, IFormatProvider? provider)
		{
			return Parse(s.AsSpan(), provider);
		}

		public static bool TryParse(string? s, IFormatProvider? provider, out Vector result)
		{
			return TryParse(s.AsSpan(), provider, out result);
		}

		public static Vector Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
		{
			Span<float> data = stackalloc float[3];
			var index = 0;

			foreach (var stringNumber in s.Split(','))
			{
				if (index < data.Length)
				{
					data[index++] = Single.Parse(stringNumber);
				}
			}

			return new Vector(data);
		}

		public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out Vector result)
		{
			Span<float> data = stackalloc float[3];
			var index = 0;

			foreach (var stringNumber in s.Split(','))
			{
				if (index < data.Length && Single.TryParse(stringNumber, out var number))
				{
					data[index++] = number;
				}
				else
				{
					result = default;
					return false;
				}
			}

			result = new Vector(data);
			return true;
		}
	}
}