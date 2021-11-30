using System;
using System.Runtime.Intrinsics;
using static System.MathF;
using Vector = MathSharp.Vector;

namespace CCreative
{
	[Serializable]
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
			return new PVector(Vector.Add(vector, Vector128.Create(x, y, z, 0)));
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
			return new PVector(Vector.CrossProduct3D(vector, v.vector));
		}

		public float Dist(PVector v)
		{
			return Vector
				.Distance3D(vector, v.vector)
				.ToScalar();
		}

		public PVector Div(float n)
		{
			return new PVector(Vector.Divide(vector, n));
		}

		public float Dot(PVector v)
		{
			return Vector
				.DotProduct3D(vector, v.vector)
				.ToScalar();
			//return FusedMultiplyAdd(X, v.X, FusedMultiplyAdd(Y, v.Y, Z * v.Z));
		}

		public float Dot(float x, float y, float z)
		{
			return Vector
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
				return new[] { X, Y, Z };
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
			return new PVector(Vector.Lerp(vector, Vector128.Create(x, y, z, 0), amt));
		}

		public PVector Lerp(PVector v, float amt)
		{
			return new PVector(Vector.Lerp(vector, v.vector, amt));
		}

		public PVector Limit(float max)
		{
			return MagSq() > max * max ? new PVector(Vector.Multiply(Vector.Normalize3D(vector), max)) : this;
		}

		public float Mag()
		{
			return Vector
				.Length3D(vector)
				.ToScalar();
		}

		public float MagSq()
		{
			return Vector
				.LengthSquared3D(vector)
				.ToScalar();
		}

		public PVector Mult(float n)
		{
			return new PVector(Vector.Multiply(vector, n));
		}

		public PVector Normalize()
		{
			return new PVector(Vector.Normalize3D(vector));
		}

		public PVector Rotate(float theta)
		{
			var temp = X;
			
			// Might need to check for rounding errors like with angleBetween function?
			var x = X * Cos(theta) - Y * Sin(theta);
			var y = temp * Sin(theta) + Y * Cos(theta);
			
			return new PVector(x, y);
		}

		public PVector SetMag(float len)
		{
			return new PVector(Vector.Multiply(Vector.Normalize3D(vector), len));
		}

		public PVector Sub(PVector v)
		{
			return new PVector(Vector.Subtract(vector, v.vector));
		}

		public PVector Sub(float x, float y)
		{
			return new PVector(Vector.Subtract(vector, Vector128.Create(x, y, 0, 0)));
		}

		public PVector Sub(float x, float y, float z)
		{
			return new PVector(Vector.Subtract(vector, Vector128.Create(x, y, z, 0)));
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
			return new PVector(Vector.Add(v1.vector, v2.vector));
		}

		public static float AngleBetween(PVector v1, PVector v2)
		{
			// We get NaN if we pass in a zero vector which can cause problems
			// Zero seems like a reasonable angle between a (0,0,0) vector and something else
			if (v1.vector.Equals(Vector128<float>.Zero) || v2.vector.Equals(Vector128<float>.Zero)) 
				return 0;

			var dot = v1.Dot(v2);
			var v1mag = v1.Mag();
			var v2mag = v2.Mag();

			// This should be a number between -1 and 1, since it's "normalized"
			var amt = dot / (v1mag * v2mag);
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
			return new PVector(Vector.CrossProduct3D(v1.vector, v2.vector));
		}

		public static float Dist(PVector v1, PVector v2)
		{
			return Vector
				.Distance4D(v1.vector, v2.vector)
				.ToScalar();
		}

		public static PVector Div(PVector v, float n)
		{
			return new PVector(Vector.Divide(v.vector, n));
		}

		public static PVector FromAngle(float angle)
		{
			return new PVector(Cos(angle), Sin(angle));
		}

		public static PVector Lerp(PVector v1, PVector v2, float amt)
		{
			return new PVector(Vector.Lerp(v1.vector, v2.vector, amt));
		}

		public static PVector Mult(PVector v, float n)
		{
			return new PVector(Vector.Multiply(v.vector, n));
		}

		public static PVector Random2D()
		{
			return FromAngle(Math.Random(PConstants.TWO_PI));
		}

		static public PVector Random3D()
		{
			return Random3D(default);
		}

		public static PVector Random3D(PVector target)
		{
			var angle = Math.Random(PConstants.TWO_PI);
			var vz = Math.Random(-1, 1);

			var vx = Sqrt(1 - vz * vz) * Cos(angle);
			var vy = Sqrt(1 - vz * vz) * Sin(angle);

			return new PVector(vx, vy, vz);
		}

		public static PVector Sub(PVector v1, PVector v2)
		{
			return new PVector(Vector.Subtract(v1.vector, v2.vector));
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
		public static PVector operator /(PVector a, float scalar) => Div(a, scalar);

		public static PVector operator +(PVector a, PVector b) => Add(a, b);
		public static PVector operator -(PVector a, PVector b) => Sub(a, b);

		#endregion
	}
}