using System;
using static System.MathF;

namespace CCreative
{
	public class PMatrix3D : PMatrix
	{
		public float m00, m01, m02, m03;
		public float m10, m11, m12, m13;
		public float m20, m21, m22, m23;
		public float m30, m31, m32, m33;

		public PMatrix3D()
		{
			Reset();
		}

		public PMatrix3D(float m00, float m01, float m02,
										 float m10, float m11, float m12)
		{
			Set(m00, m01, m02, 0,
					m10, m11, m12, 0,
					0, 0, 1, 0,
					0, 0, 0, 1);
		}

		public PMatrix3D(float m00, float m01, float m02, float m03,
										 float m10, float m11, float m12, float m13,
										 float m20, float m21, float m22, float m23,
										 float m30, float m31, float m32, float m33)
		{
			Set(m00, m01, m02, m03,
					m10, m11, m12, m13,
					m20, m21, m22, m23,
					m30, m31, m32, m33);
		}

		public PMatrix3D(PMatrix matrix)
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
			Apply(source.m00, source.m01, 0, source.m02,
						source.m10, source.m11, 0, source.m12,
						0, 0, 1, 0,
						0, 0, 0, 1);
		}

		public void Apply(PMatrix3D source)
		{
			Apply(source.m00, source.m01, source.m02, source.m03,
						source.m10, source.m11, source.m12, source.m13,
						source.m20, source.m21, source.m22, source.m23,
						source.m30, source.m31, source.m32, source.m33);
		}

		public void Apply(float n00, float n01, float n02,
											float n10, float n11, float n12)
		{
			Apply(n00, n01, 0, n02,
					n10, n11, 0, n12,
					0, 0, 1, 0,
					0, 0, 0, 1);
		}

		public void Apply(float n00, float n01, float n02, float n03,
											float n10, float n11, float n12, float n13,
											float n20, float n21, float n22, float n23,
											float n30, float n31, float n32, float n33)
		{
			var r00 = m00 * n00 + m01 * n10 + m02 * n20 + m03 * n30;
			var r01 = m00 * n01 + m01 * n11 + m02 * n21 + m03 * n31;
			var r02 = m00 * n02 + m01 * n12 + m02 * n22 + m03 * n32;
			var r03 = m00 * n03 + m01 * n13 + m02 * n23 + m03 * n33;

			var r10 = m10 * n00 + m11 * n10 + m12 * n20 + m13 * n30;
			var r11 = m10 * n01 + m11 * n11 + m12 * n21 + m13 * n31;
			var r12 = m10 * n02 + m11 * n12 + m12 * n22 + m13 * n32;
			var r13 = m10 * n03 + m11 * n13 + m12 * n23 + m13 * n33;

			var r20 = m20 * n00 + m21 * n10 + m22 * n20 + m23 * n30;
			var r21 = m20 * n01 + m21 * n11 + m22 * n21 + m23 * n31;
			var r22 = m20 * n02 + m21 * n12 + m22 * n22 + m23 * n32;
			var r23 = m20 * n03 + m21 * n13 + m22 * n23 + m23 * n33;

			var r30 = m30 * n00 + m31 * n10 + m32 * n20 + m33 * n30;
			var r31 = m30 * n01 + m31 * n11 + m32 * n21 + m33 * n31;
			var r32 = m30 * n02 + m31 * n12 + m32 * n22 + m33 * n32;
			var r33 = m30 * n03 + m31 * n13 + m32 * n23 + m33 * n33;

			m00 = r00; m01 = r01; m02 = r02; m03 = r03;
			m10 = r10; m11 = r11; m12 = r12; m13 = r13;
			m20 = r20; m21 = r21; m22 = r22; m23 = r23;
			m30 = r30; m31 = r31; m32 = r32; m33 = r33;
		}

		public float Determinant()
		{
			var f =
			m00
			* ((m11 * m22 * m33 + m12 * m23 * m31 + m13 * m21 * m32)
				 - m13 * m22 * m31
				 - m11 * m23 * m32
				 - m12 * m21 * m33);
			f -= m01
				* ((m10 * m22 * m33 + m12 * m23 * m30 + m13 * m20 * m32)
					 - m13 * m22 * m30
					 - m10 * m23 * m32
					 - m12 * m20 * m33);
			f += m02
				* ((m10 * m21 * m33 + m11 * m23 * m30 + m13 * m20 * m31)
					 - m13 * m21 * m30
					 - m10 * m23 * m31
					 - m11 * m20 * m33);
			f -= m03
				* ((m10 * m21 * m32 + m11 * m22 * m30 + m12 * m20 * m31)
					 - m12 * m21 * m30
					 - m10 * m22 * m31
					 - m11 * m20 * m32);
			return f;
		}

		public PMatrix Get()
		{
			var outgoing = new PMatrix3D();
			outgoing.Set(this);

			return outgoing;
		}

		public float[] Get(float[] target)
		{
			if (!(target is float[] { Length: 16 }))
			{
				target = new float[16];
			}

			target[0] = m00;
			target[1] = m01;
			target[2] = m02;
			target[3] = m03;

			target[4] = m10;
			target[5] = m11;
			target[6] = m12;
			target[7] = m13;

			target[8] = m20;
			target[9] = m21;
			target[10] = m22;
			target[11] = m23;

			target[12] = m30;
			target[13] = m31;
			target[14] = m32;
			target[15] = m33;

			return target;
		}

		public bool Invert()
		{
			var determinantValue = Determinant();
			if (determinantValue == 0)
			{
				return false;
			}

			// first row
			var t00 = Determinant3x3(m11, m12, m13, m21, m22, m23, m31, m32, m33);
			var t01 = -Determinant3x3(m10, m12, m13, m20, m22, m23, m30, m32, m33);
			var t02 = Determinant3x3(m10, m11, m13, m20, m21, m23, m30, m31, m33);
			var t03 = -Determinant3x3(m10, m11, m12, m20, m21, m22, m30, m31, m32);

			// second row
			var t10 = -Determinant3x3(m01, m02, m03, m21, m22, m23, m31, m32, m33);
			var t11 = Determinant3x3(m00, m02, m03, m20, m22, m23, m30, m32, m33);
			var t12 = -Determinant3x3(m00, m01, m03, m20, m21, m23, m30, m31, m33);
			var t13 = Determinant3x3(m00, m01, m02, m20, m21, m22, m30, m31, m32);

			// third row
			var t20 = Determinant3x3(m01, m02, m03, m11, m12, m13, m31, m32, m33);
			var t21 = -Determinant3x3(m00, m02, m03, m10, m12, m13, m30, m32, m33);
			var t22 = Determinant3x3(m00, m01, m03, m10, m11, m13, m30, m31, m33);
			var t23 = -Determinant3x3(m00, m01, m02, m10, m11, m12, m30, m31, m32);

			// fourth row
			var t30 = -Determinant3x3(m01, m02, m03, m11, m12, m13, m21, m22, m23);
			var t31 = Determinant3x3(m00, m02, m03, m10, m12, m13, m20, m22, m23);
			var t32 = -Determinant3x3(m00, m01, m03, m10, m11, m13, m20, m21, m23);
			var t33 = Determinant3x3(m00, m01, m02, m10, m11, m12, m20, m21, m22);

			// transpose and divide by the determinant
			m00 = t00 / determinantValue;
			m01 = t10 / determinantValue;
			m02 = t20 / determinantValue;
			m03 = t30 / determinantValue;

			m10 = t01 / determinantValue;
			m11 = t11 / determinantValue;
			m12 = t21 / determinantValue;
			m13 = t31 / determinantValue;

			m20 = t02 / determinantValue;
			m21 = t12 / determinantValue;
			m22 = t22 / determinantValue;
			m23 = t32 / determinantValue;

			m30 = t03 / determinantValue;
			m31 = t13 / determinantValue;
			m32 = t23 / determinantValue;
			m33 = t33 / determinantValue;

			return true;
		}

		private float Determinant3x3(float t00, float t01, float t02,
																 float t10, float t11, float t12,
																 float t20, float t21, float t22)
		{
			return (t00 * (t11 * t22 - t12 * t21) +
							t01 * (t12 * t20 - t10 * t22) +
							t02 * (t10 * t21 - t11 * t20));
		}

		public PVector Mult(PVector source)
		{
			var x = m00 * source.X + m01 * source.Y + m02 * source.Z + m03;
			var y = m10 * source.X + m11 * source.Y + m12 * source.Z + m13;
			var z = m20 * source.X + m21 * source.Y + m22 * source.Z + m23;

			return new PVector(x, y, z);
		}

		public float[] Mult(float[] source, float[] target)
		{
			if (target is not { Length: >= 3})
			{
				target = new float[3];
			}

			if (source == target)
			{
				throw new ArgumentException("The source and target vectors used in " +
																		"PMatrix3D.mult() cannot be identical.");
			}

			switch (target.Length)
			{
				case 3:
					target[0] = m00 * source[0] + m01 * source[1] + m02 * source[2] + m03;
					target[1] = m10 * source[0] + m11 * source[1] + m12 * source[2] + m13;
					target[2] = m20 * source[0] + m21 * source[1] + m22 * source[2] + m23;
					break;
				case > 3:
					target[0] = m00 * source[0] + m01 * source[1] + m02 * source[2] + m03 * source[3];
					target[1] = m10 * source[0] + m11 * source[1] + m12 * source[2] + m13 * source[3];
					target[2] = m20 * source[0] + m21 * source[1] + m22 * source[2] + m23 * source[3];
					target[3] = m30 * source[0] + m31 * source[1] + m32 * source[2] + m33 * source[3];
					break;
			}

			return target;
		}

		public float MultX(float x, float y)
		{
			return m00 * x + m01 * y + m03;
		}

		public float MultY(float x, float y)
		{
			return m10 * x + m11 * y + m13;
		}

		public float MultX(float x, float y, float z)
		{
			return m00 * x + m01 * y + m02 * z + m03;
		}

		public float MultY(float x, float y, float z)
		{
			return m10 * x + m11 * y + m12 * z + m13;
		}

		public float MultZ(float x, float y, float z)
		{
			return m20 * x + m21 * y + m22 * z + m23;
		}

		public float MultW(float x, float y, float z)
		{
			return m30 * x + m31 * y + m32 * z + m33;
		}

		public float MultX(float x, float y, float z, float w)
		{
			return m00 * x + m01 * y + m02 * z + m03 * w;
		}

		public float MultY(float x, float y, float z, float w)
		{
			return m10 * x + m11 * y + m12 * z + m13 * w;
		}

		public float MultZ(float x, float y, float z, float w)
		{
			return m20 * x + m21 * y + m22 * z + m23 * w;
		}

		public float MultW(float x, float y, float z, float w)
		{
			return m30 * x + m31 * y + m32 * z + m33 * w;
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
			PreApply(left.m00, left.m01, 0, left.m02,
							 left.m10, left.m11, 0, left.m12,
							 0, 0, 1, 0,
							 0, 0, 0, 1);
		}

		public void PreApply(PMatrix3D left)
		{
			PreApply(left.m00, left.m01, left.m02, left.m03,
							 left.m10, left.m11, left.m12, left.m13,
							 left.m20, left.m21, left.m22, left.m23,
							 left.m30, left.m31, left.m32, left.m33);
		}

		public void PreApply(float n00, float n01, float n02,
												 float n10, float n11, float n12)
		{
			PreApply(n00, n01, 0, n02,
							 n10, n11, 0, n12,
							 0, 0, 1, 0,
							 0, 0, 0, 1);
		}

		public void PreApply(float n00, float n01, float n02, float n03,
												 float n10, float n11, float n12, float n13,
												 float n20, float n21, float n22, float n23,
												 float n30, float n31, float n32, float n33)
		{
			var r00 = n00 * m00 + n01 * m10 + n02 * m20 + n03 * m30;
			var r01 = n00 * m01 + n01 * m11 + n02 * m21 + n03 * m31;
			var r02 = n00 * m02 + n01 * m12 + n02 * m22 + n03 * m32;
			var r03 = n00 * m03 + n01 * m13 + n02 * m23 + n03 * m33;

			var r10 = n10 * m00 + n11 * m10 + n12 * m20 + n13 * m30;
			var r11 = n10 * m01 + n11 * m11 + n12 * m21 + n13 * m31;
			var r12 = n10 * m02 + n11 * m12 + n12 * m22 + n13 * m32;
			var r13 = n10 * m03 + n11 * m13 + n12 * m23 + n13 * m33;

			var r20 = n20 * m00 + n21 * m10 + n22 * m20 + n23 * m30;
			var r21 = n20 * m01 + n21 * m11 + n22 * m21 + n23 * m31;
			var r22 = n20 * m02 + n21 * m12 + n22 * m22 + n23 * m32;
			var r23 = n20 * m03 + n21 * m13 + n22 * m23 + n23 * m33;

			var r30 = n30 * m00 + n31 * m10 + n32 * m20 + n33 * m30;
			var r31 = n30 * m01 + n31 * m11 + n32 * m21 + n33 * m31;
			var r32 = n30 * m02 + n31 * m12 + n32 * m22 + n33 * m32;
			var r33 = n30 * m03 + n31 * m13 + n32 * m23 + n33 * m33;

			m00 = r00; m01 = r01; m02 = r02; m03 = r03;
			m10 = r10; m11 = r11; m12 = r12; m13 = r13;
			m20 = r20; m21 = r21; m22 = r22; m23 = r23;
			m30 = r30; m31 = r31; m32 = r32; m33 = r33;
		}

		public void Print()
		{
			var big = (int)Abs(Max(Max(Max(Max(Abs(m00), Abs(m01)),
																					 Max(Abs(m02), Abs(m03))),
																			 Max(Max(Abs(m10), Abs(m11)),
																					 Max(Abs(m12), Abs(m13)))),
																	 Max(Max(Max(Abs(m20), Abs(m21)),
																					 Max(Abs(m22), Abs(m23))),
																			 Max(Max(Abs(m30), Abs(m31)),
																					 Max(Abs(m32), Abs(m33))))));

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
												 PApplet.Nfs(m02, digits, 4) + " " +
												 PApplet.Nfs(m03, digits, 4));

			Console.WriteLine(PApplet.Nfs(m10, digits, 4) + " " +
												 PApplet.Nfs(m11, digits, 4) + " " +
												 PApplet.Nfs(m12, digits, 4) + " " +
												 PApplet.Nfs(m13, digits, 4));

			Console.WriteLine(PApplet.Nfs(m20, digits, 4) + " " +
												 PApplet.Nfs(m21, digits, 4) + " " +
												 PApplet.Nfs(m22, digits, 4) + " " +
												 PApplet.Nfs(m23, digits, 4));

			Console.WriteLine(PApplet.Nfs(m30, digits, 4) + " " +
												 PApplet.Nfs(m31, digits, 4) + " " +
												 PApplet.Nfs(m32, digits, 4) + " " +
												 PApplet.Nfs(m33, digits, 4));

			Console.WriteLine();
		}

		public void Reset()
		{
			Set(1, 0, 0, 0,
					0, 1, 0, 0,
					0, 0, 1, 0,
					0, 0, 0, 1);
		}

		public void Rotate(float angle)
		{
			RotateZ(angle);
		}

		public void Rotate(float angle, float v0, float v1, float v2)
		{
			var norm2 = v0 * v0 + v1 * v1 + v2 * v2;

			if (norm2 < float.Epsilon)
			{
				// The vector is zero, cannot apply rotation.
				return;
			}

			if (Abs(norm2 - 1) > float.Epsilon)
			{
				// The rotation vector is not normalized.
				var norm = Sqrt(norm2);
				v0 /= norm;
				v1 /= norm;
				v2 /= norm;
			}

			var c = Cos(angle);
			var s = Sin(angle);
			var t = 1.0f - c;

			Apply((t * v0 * v0) + c, (t * v0 * v1) - (s * v2), (t * v0 * v2) + (s * v1), 0,
						(t * v0 * v1) + (s * v2), (t * v1 * v1) + c, (t * v1 * v2) - (s * v0), 0,
						(t * v0 * v2) - (s * v1), (t * v1 * v2) + (s * v0), (t * v2 * v2) + c, 0,
						0, 0, 0, 1);
		}

		public void RotateX(float angle)
		{
			var c = Cos(angle);
			var s = Sin(angle);

			Apply(1, 0, 0, 0,
						0, c, -s, 0,
						0, s, c, 0,
						0, 0, 0, 1);
		}

		public void RotateY(float angle)
		{
			var c = Cos(angle);
			var s = Sin(angle);

			Apply(c, 0, s, 0,
						0, 1, 0, 0,
						-s, 0, c, 0,
						0, 0, 0, 1);
		}

		public void RotateZ(float angle)
		{
			var c = Cos(angle);
			var s = Sin(angle);

			Apply(c, -s, 0, 0,
						s, c, 0, 0,
						0, 0, 1, 0,
						0, 0, 0, 1);
		}

		public void Scale(float s)
		{
			Scale(s, s, s);
		}

		public void Scale(float sx, float sy)
		{
			Scale(sx, sy, 1);
		}

		public void Scale(float x, float y, float z)
		{
			m00 *= x; m01 *= y; m02 *= z;
			m10 *= x; m11 *= y; m12 *= z;
			m20 *= x; m21 *= y; m22 *= z;
			m30 *= x; m31 *= y; m32 *= z;
		}

		public void Set(PMatrix matrix)
		{
			switch (matrix)
			{
				case PMatrix2D src:
					Set(src.m00, src.m01, 0, src.m02,
							src.m10, src.m11, 0, src.m12,
							0, 0, 1, 0,
							0, 0, 0, 1);
					break;
				case PMatrix3D src:
					Set(src.m00, src.m01, src.m02, src.m03,
							src.m10, src.m11, src.m12, src.m13,
							src.m20, src.m21, src.m22, src.m23,
							src.m30, src.m31, src.m32, src.m33);
					break;
			}
		}

		public void Set(float[] source)
		{
			if (source.Length == 6)
			{
				Set(source[0], source[1], source[2],
						source[3], source[4], source[5]);

			}
			else if (source.Length == 16)
			{
				m00 = source[0];
				m01 = source[1];
				m02 = source[2];
				m03 = source[3];

				m10 = source[4];
				m11 = source[5];
				m12 = source[6];
				m13 = source[7];

				m20 = source[8];
				m21 = source[9];
				m22 = source[10];
				m23 = source[11];

				m30 = source[12];
				m31 = source[13];
				m32 = source[14];
				m33 = source[15];
			}
		}

		public void Set(float m00, float m01, float m02,
										float m10, float m11, float m12)
		{
			Set(m00, m01, 0, m02,
					m10, m11, 0, m12,
					0, 0, 1, 0,
					0, 0, 0, 1);
		}

		public void Set(float m00, float m01, float m02, float m03,
										float m10, float m11, float m12, float m13,
										float m20, float m21, float m22, float m23,
										float m30, float m31, float m32, float m33)
		{
			this.m00 = m00; this.m01 = m01; this.m02 = m02; this.m03 = m03;
			this.m10 = m10; this.m11 = m11; this.m12 = m12; this.m13 = m13;
			this.m20 = m20; this.m21 = m21; this.m22 = m22; this.m23 = m23;
			this.m30 = m30; this.m31 = m31; this.m32 = m32; this.m33 = m33;
		}

		public void ShearX(float angle)
		{
			var t = Tan(angle);

			Apply(1, t, 0, 0,
						0, 1, 0, 0,
						0, 0, 1, 0,
						0, 0, 0, 1);
		}

		public void ShearY(float angle)
		{
			var t = Tan(angle);

			Apply(1, 0, 0, 0,
						t, 1, 0, 0,
						0, 0, 1, 0,
						0, 0, 0, 1);
		}

		public void Translate(float tx, float ty)
		{
			Translate(tx, ty, 0);
		}

		public void Translate(float tx, float ty, float tz)
		{
			m03 += tx * m00 + ty * m01 + tz * m02;
			m13 += tx * m10 + ty * m11 + tz * m12;
			m23 += tx * m20 + ty * m21 + tz * m22;
			m33 += tx * m30 + ty * m31 + tz * m32;
		}

		public void Transpose()
		{
			float temp;
			temp = m01; m01 = m10; m10 = temp;
			temp = m02; m02 = m20; m20 = temp;
			temp = m03; m03 = m30; m30 = temp;
			temp = m12; m12 = m21; m21 = temp;
			temp = m13; m13 = m31; m31 = temp;
			temp = m23; m23 = m32; m32 = temp;
		}
	}
}