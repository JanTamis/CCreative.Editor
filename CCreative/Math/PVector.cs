using System;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using static System.MathF;
using VectorMath = MathSharp.Vector;

// ReSharper disable ConvertIfStatementToReturnStatement
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable IdentifierTypo

namespace CCreative
{
	public readonly struct PVector : IEquatable<PVector>
	{
		private readonly Vector128<float> vector;

		public float X => vector.GetElement(0);
		public float Y => vector.GetElement(1);
		public float Z => vector.GetElement(2);

		public PVector(float x, float y) : this(x, y, 0)
		{
		}

		public PVector(float x, float y, float z)
		{
			vector = Vector128.Create(x, y, z, 0);
		}

		private PVector(Vector128<float> vector)
		{
			this.vector = vector;
		}

		#region Instance Functions

		public PVector Add(float x, float y)
		{
			return Add(x, y, 0);
		}

		public PVector Add(float x, float y, float z)
		{
			return new PVector(VectorMath.Add(vector, Vector128.Create(x, y, z, 0)));
		}

		public float[] Array()
		{
			return new[] { X, Y, Z };
		}

		public PVector Copy()
		{
			return new PVector(vector);
		}

		public PVector Cross(PVector v)
		{
			return Cross(v, this);
		}

		public float Dist(PVector v)
		{
			return Dist(v, this);
		}

		public PVector Div(float n)
		{
			return new PVector(VectorMath.Divide(vector, n));
		}

		public float Dot(PVector v)
		{
			return VectorMath
				.DotProduct3D(vector, v.vector)
				.ToScalar();
			//return FusedMultiplyAdd(X, v.X, FusedMultiplyAdd(Y, v.Y, Z * v.Z));
		}

		public float Dot(float x, float y, float z)
		{
			return VectorMath
				.DotProduct3D(vector, Vector128.Create(x, y, z, 0))
				.ToScalar();
		}

		public static float Dot(PVector v1, PVector v2)
		{
			return v1.Dot(v2);
		}

		public float[] Get()
		{
			return Array();
		}

		public float[] Get(float[]? target)
		{
			if (target is null)
			{
				throw new ArgumentNullException(nameof(target),
					"please use the Get() method to get a new array of the component");
			}

			if (target.Length >= 2)
			{
				target[0] = X;
				target[1] = Y;
			}

			if (target.Length >= 3)
			{
				target[2] = Z;
			}

			return target;
		}

		public float Heading()
		{
			return Atan2(Y, X);
		}

		public PVector Lerp(float x, float y, float z, float amt)
		{
			var v2 = Vector128.Create(x, y, z, 0f);

			if (Fma.IsSupported)
			{
				return new PVector(Fma.MultiplyAdd(Vector128.Create(amt), Sse.Subtract(v2, vector), vector));
			}

			return new PVector(VectorMath.Lerp(vector, v2, amt));
		}

		public PVector Lerp(PVector v, float amt)
		{
			if (Fma.IsSupported)
			{
				return new PVector(Fma.MultiplyAdd(Vector128.Create(amt), Sse.Subtract(v.vector, vector), vector));
			}

			return new PVector(VectorMath.Lerp(vector, v.vector, amt));
		}

		public PVector Limit(float max)
		{
			return MagSq() > Math.Sq(max) ? new PVector(VectorMath.Multiply(VectorMath.Normalize3D(vector), max)) : this;
		}

		public float Mag()
		{
			return VectorMath
				.Length3D(vector)
				.ToScalar();
		}

		public float MagSq()
		{
			return VectorMath
				.LengthSquared3D(vector)
				.ToScalar();
		}

		public PVector Mult(float n)
		{
			return new PVector(VectorMath.Multiply(vector, n));
		}

		public PVector Normalize()
		{
			return new PVector(VectorMath.Normalize3D(vector));
		}

		public PVector Rotate(float theta)
		{
			var (sin, cos) = SinCos(theta);

			// Might need to check for rounding errors like with angleBetween function?
			var x = X * cos - Y * sin;
			var y = FusedMultiplyAdd(X, sin, Y * cos);

			return new PVector(x, y);
		}

		public PVector SetMag(float len)
		{
			return new PVector(VectorMath.Multiply(VectorMath.Normalize3D(vector), len));
		}

		public PVector Sub(PVector v)
		{
			return new PVector(VectorMath.Subtract(vector, v.vector));
		}

		public PVector Sub(float x, float y)
		{
			return new PVector(VectorMath.Subtract(vector, Vector128.Create(x, y, 0, 0)));
		}

		public PVector Sub(float x, float y, float z)
		{
			return new PVector(VectorMath.Subtract(vector, Vector128.Create(x, y, z, 0)));
		}

		public override int GetHashCode()
		{
			return vector.GetHashCode();
		}

		public override string ToString()
		{
			return $"[{X}, {Y}, {Z}]";
		}

		public bool Equals(PVector other)
		{
			return vector.Equals(other.vector);
		}

		public override bool Equals(object? obj)
		{
			return obj is PVector v && Equals(v);
		}

		#endregion

		#region Static Functions

		public static PVector Add(PVector v1, PVector v2)
		{
			return new PVector(VectorMath.Add(v1.vector, v2.vector));
		}

		public static float AngleBetween(PVector v1, PVector v2)
		{
			// We get NaN if we pass in a zero vector which can cause problems
			// Zero seems like a reasonable angle between a (0,0,0) vector and something else
			if (v1.vector.Equals(Vector128<float>.Zero) || v2.vector.Equals(Vector128<float>.Zero))
				return 0;

			var dot = v1.Dot(v2);
			var v1Mag = v1.Mag();
			var v2Mag = v2.Mag();

			// This should be a number between -1 and 1, since it's "normalized"
			var amt = dot / (v1Mag * v2Mag);
			// But if it's not due to rounding error, then we need to fix it
			// http://code.google.com/p/processing/issues/detail?id=340
			// Otherwise if outside the range, acos() will return NaN
			// http://www.cppreference.com/wiki/c/math/acos

			return amt switch
			{
				<= -1 => PI,
				>= 1 => 0,
				_ => Acos(amt)
			};
		}

		public static PVector Cross(PVector v1, PVector v2)
		{
			return new PVector(VectorMath.CrossProduct3D(v1.vector, v2.vector));
		}

		public static float Dist(PVector v1, PVector v2)
		{
			return VectorMath
				.Distance3D(v1.vector, v2.vector)
				.ToScalar();
		}

		public static PVector Div(PVector v, float n)
		{
			return new PVector(VectorMath.Divide(v.vector, n));
		}
		
		public static PVector Div(PVector v1, PVector v2)
		{
			return new PVector(VectorMath.Divide(v1.vector, v2.vector));
		}

		public static PVector FromAngle(float angle)
		{
			var (sin, cos) = SinCos(angle);
			return new PVector(cos, sin);
		}

		public static PVector Lerp(PVector v1, PVector v2, float amt)
		{
			if (Fma.IsSupported)
			{
				return new PVector(Fma.MultiplyAdd(Vector128.Create(amt), Sse.Subtract(v1.vector, v2.vector), v1.vector));
			}

			return new PVector(VectorMath.Lerp(v1.vector, v2.vector, amt));
		}

		public static PVector Mult(PVector v, float n)
		{
			return new PVector(VectorMath.Multiply(v.vector, n));
		}
		
		public static PVector Mult(PVector v1, PVector v2)
		{
			return new PVector(VectorMath.Multiply(v1.vector, v2.vector));
		}

		public static PVector Random2D()
		{
			return FromAngle(Math.Random(PConstants.TWO_PI));
		}

		public static PVector Random3D()
		{
			var angle = Math.Random(PConstants.TWO_PI);
			var vz = Math.Random(-1, 1);

			var vx = Sqrt(1 - vz * vz) * Cos(angle);
			var vy = Sqrt(1 - vz * vz) * Sin(angle);

			return new PVector(vx, vy, vz);
		}

		public static PVector Sub(PVector v1, PVector v2)
		{
			return new PVector(VectorMath.Subtract(v1.vector, v2.vector));
		}

		public void Deconstruct(out float x, out float y, out float z)
		{
			x = X;
			y = Y;
			z = Z;
		}

		#endregion

		#region Operators

		public static PVector operator *(PVector a, float scalar) => Mult(a, scalar);
		public static PVector operator *(PVector a, PVector b) => Mult(a, b);
		
		public static PVector operator /(PVector a, float scalar) => Div(a, scalar);
		public static PVector operator /(PVector a, PVector b) => Div(a, b);

		public static PVector operator +(PVector a, PVector b) => Add(a, b);
		public static PVector operator -(PVector a, PVector b) => Sub(a, b);

		public static bool operator ==(PVector left, PVector right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(PVector left, PVector right)
		{
			return !(left == right);
		}

		#endregion
	}
}