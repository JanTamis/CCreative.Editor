using System;
using static System.MathF;

namespace CCreative
{
	public class PMatrix2D : PMatrix
	{
		public float m00, m01, m02;
		public float m10, m11, m12;

		public PMatrix2D()
		{
			Reset();
		}

		public PMatrix2D(float m00, float m01, float m02,
										 float m10, float m11, float m12)
		{
			Set(m00, m01, m02,
					m10, m11, m12);

		}

		public PMatrix2D(PMatrix matrix)
		{
			Set(matrix);
		}

		public void Apply(PMatrix source)
		{
			switch (source)
			{
				case PMatrix2D matrix2D:
					Apply(matrix2D);
					break;
				case PMatrix3D matrix3D:
					Apply(matrix3D);
					break;
			}
		}

		public void Apply(PMatrix2D source)
		{
			Apply(source.m00, source.m01, source.m02,
						source.m10, source.m11, source.m12);
		}

		public void Apply(PMatrix3D source)
		{
			throw new NotSupportedException("Cannot use apply(PMatrix3D) on a PMatrix2D.");
		}

		public void Apply(float n00, float n01, float n02, float n10, float n11, float n12)
		{
			var t0 = m00;
			var t1 = m01;
			m00 = n00 * t0 + n10 * t1;
			m01 = n01 * t0 + n11 * t1;
			m02 += n02 * t0 + n12 * t1;

			t0 = m10;
			t1 = m11;
			m10 = n00 * t0 + n10 * t1;
			m11 = n01 * t0 + n11 * t1;
			m12 += n02 * t0 + n12 * t1;
		}

		public void Apply(float n00, float n01, float n02, float n03, float n10, float n11, float n12, float n13, float n20, float n21, float n22, float n23, float n30, float n31, float n32, float n33)
		{
			throw new NotSupportedException("Cannot use this version of apply() on a PMatrix2D.");
		}

		public float Determinant()
		{
			return m00 * m11 - m01 * m10;
		}

		public PMatrix Get()
		{
			var outgoing = new PMatrix2D();
			outgoing.Set(this);
			return outgoing;
		}

		public float[] Get(float[] target)
		{
			if ((target is null) || (target.Length != 6))
			{
				target = new float[6];
			}

			target[0] = m00;
			target[1] = m01;
			target[2] = m02;

			target[3] = m10;
			target[4] = m11;
			target[5] = m12;

			return target;
		}

		public bool Invert()
		{
			var determinantValue = Determinant();

			if (Abs(determinantValue) <= Single.MinValue)
			{
				return false;
			}

			var t00 = m00;
			var t01 = m01;
			var t02 = m02;
			var t10 = m10;
			var t11 = m11;
			var t12 = m12;

			m00 = t11 / determinantValue;
			m10 = -t10 / determinantValue;
			m01 = -t01 / determinantValue;
			m11 = t00 / determinantValue;
			m02 = (t01 * t12 - t11 * t02) / determinantValue;
			m12 = (t10 * t02 - t00 * t12) / determinantValue;

			return true;
		}

		protected bool IsIdentity()
		{
			return ((m00 == 1) && (m01 == 0) && (m02 == 0) &&
							(m10 == 0) && (m11 == 1) && (m12 == 0));
		}

		protected bool IsWarped()
		{
			return ((m00 != 1) || (m01 != 0) &&
							(m10 != 0) || (m11 != 1));
		}

		public PVector Mult(PVector source)
		{
			var x = m00 * source.X + m01 * source.Y + m02;
			var y = m10 * source.X + m11 * source.Y + m12;

			return new PVector(x, y);
		}

		public float[] Mult(float[] source, float[] target)
		{
			if (target is not { Length: 2 })
			{
				target = new float[2];
			}

			if (source == target)
			{
				var tx = m00 * source[0] + m01 * source[1] + m02;
				var ty = m10 * source[0] + m11 * source[1] + m12;

				target[0] = tx;
				target[1] = ty;

			}
			else
			{
				target[0] = m00 * source[0] + m01 * source[1] + m02;
				target[1] = m10 * source[0] + m11 * source[1] + m12;
			}

			return target;
		}

		public float MultX(float x, float y)
		{
			return m00 * x + m01 * y + m02;
		}

		public float MultY(float x, float y)
		{
			return m10 * x + m11 * y + m12;
		}


		public void PreApply(PMatrix left)
		{
			switch (left)
			{
				case PMatrix2D matrix2D:
					PreApply(matrix2D);
					break;
				case PMatrix3D matrix3D:
					PreApply(matrix3D);
					break;
			}
		}

		public void PreApply(PMatrix2D left)
		{
			PreApply(left.m00, left.m01, left.m02,
							 left.m10, left.m11, left.m12);
		}

		public void PreApply(PMatrix3D left)
		{
			throw new NotSupportedException("Cannot use preApply(PMatrix3D) on a PMatrix2D.");
		}

		public void PreApply(float n00, float n01, float n02,
												float n10, float n11, float n12)
		{
			var t0 = m02;
			var t1 = m12;
			n02 += t0 * n00 + t1 * n01;
			n12 += t0 * n10 + t1 * n11;

			m02 = n02;
			m12 = n12;

			t0 = m00;
			t1 = m10;
			m00 = t0 * n00 + t1 * n01;
			m10 = t0 * n10 + t1 * n11;

			t0 = m01;
			t1 = m11;
			m01 = t0 * n00 + t1 * n01;
			m11 = t0 * n10 + t1 * n11;
		}

		public void PreApply(float n00, float n01, float n02, float n03,
												 float n10, float n11, float n12, float n13,
												 float n20, float n21, float n22, float n23,
												 float n30, float n31, float n32, float n33)
		{
			throw new NotSupportedException("Cannot use this version of preApply() on a PMatrix2D.");
		}

		public void Print()
		{
			var big = (int)Abs(Max(Max(Abs(m00), Max(Abs(m01), Abs(m02))),
															Max(Abs(m10), Max(Abs(m11), Abs(m12)))));

			var digits = 1;
			if (float.IsNaN(big) || float.IsInfinity(big))
			{  // avoid infinite loop
				digits = 5;
			}
			else
			{
				while ((big /= 10) != 0) digits++;  // cheap log()
			}

			Console.WriteLine(PApplet.Nfs(m00, digits, 4) + " " +
												 PApplet.Nfs(m01, digits, 4) + " " +
												 PApplet.Nfs(m02, digits, 4));

			Console.WriteLine(PApplet.Nfs(m10, digits, 4) + " " +
												 PApplet.Nfs(m11, digits, 4) + " " +
												 PApplet.Nfs(m12, digits, 4));

			Console.WriteLine();
		}

		public void Reset()
		{
			Set(1, 0, 0,
					0, 1, 0);
		}

		public void Rotate(float angle)
		{
			float s = Sin(angle);
			float c = Cos(angle);

			var temp1 = m00;
			var temp2 = m01;
			m00 = c * temp1 + s * temp2;
			m01 = -s * temp1 + c * temp2;
			temp1 = m10;
			temp2 = m11;
			m10 = c * temp1 + s * temp2;
			m11 = -s * temp1 + c * temp2;
		}

		public void Rotate(float angle, float v0, float v1, float v2)
		{
			throw new NotSupportedException("Cannot use this version of rotate() on a PMatrix2D.");
		}

		public void RotateX(float angle)
		{
			throw new NotSupportedException("Cannot use rotateX() on a PMatrix2D.");

		}

		public void RotateY(float angle)
		{
			throw new NotSupportedException("Cannot use rotateY() on a PMatrix2D.");

		}

		public void RotateZ(float angle)
		{
			Rotate(angle);
		}

		public void Scale(float s)
		{
			Scale(s, s);
		}

		public void Scale(float sx, float sy)
		{
			m00 *= sx; m01 *= sy;
			m10 *= sx; m11 *= sy;
		}

		public void Scale(float x, float y, float z)
		{
			throw new NotSupportedException("Cannot use this version of scale() on a PMatrix2D.");
		}

		public void Set(PMatrix matrix)
		{
			if (matrix is PMatrix2D src)
			{
				Set(src.m00, src.m01, src.m02,
						src.m10, src.m11, src.m12);
			}
			else
			{
				throw new ArgumentException("PMatrix2D.set() only accepts PMatrix2D objects.");
			}
		}

		public void Set(float[] source)
		{
			m00 = source[0];
			m01 = source[1];
			m02 = source[2];

			m10 = source[3];
			m11 = source[4];
			m12 = source[5];
		}

		public void Set(float m00, float m01, float m02,
										float m10, float m11, float m12)
		{
			this.m00 = m00; this.m01 = m01; this.m02 = m02;
			this.m10 = m10; this.m11 = m11; this.m12 = m12;
		}

		public void Set(float m00, float m01, float m02, float m03,
										float m10, float m11, float m12, float m13,
										float m20, float m21, float m22, float m23,
										float m30, float m31, float m32, float m33)
		{
			throw new NotSupportedException("This function is not supported on a PMatrix2D");
		}

		public void ShearX(float angle)
		{
			Apply(1, 0, 1, Tan(angle), 0, 0);
		}

		public void ShearY(float angle)
		{
			Apply(1, 0, 1, 0, Tan(angle), 0);
		}

		public void Translate(float tx, float ty)
		{
			m02 = tx * m00 + ty * m01 + m02;
			m12 = tx * m10 + ty * m11 + m12;
		}

		public void Translate(float tx, float ty, float tz)
		{
			throw new NotSupportedException("Cannot use translate(x, y, z) on a PMatrix2D.");
		}

		public void Transpose()
		{
			throw new NotSupportedException("Transpose is not supported on a PMatrix2D");
		}
	}
}