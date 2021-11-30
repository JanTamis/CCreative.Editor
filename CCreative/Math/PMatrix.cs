namespace CCreative
{
	public interface PMatrix
	{
		public void Reset();

		public PMatrix Get();

		public float[] Get(float[] target);

		public void Set(PMatrix src);

		public void Set(float[] source);

		public void Set(float m00, float m01, float m02,
										float m10, float m11, float m12);

		public void Set(float m00, float m01, float m02, float m03,
										float m10, float m11, float m12, float m13,
										float m20, float m21, float m22, float m23,
										float m30, float m31, float m32, float m33);


		public void Translate(float tx, float ty);

		public void Translate(float tx, float ty, float tz);

		public void Rotate(float angle);

		public void RotateX(float angle);

		public void RotateY(float angle);

		public void RotateZ(float angle);

		public void Rotate(float angle, float v0, float v1, float v2);

		public void Scale(float s);

		public void Scale(float sx, float sy);

		public void Scale(float x, float y, float z);

		public void ShearX(float angle);

		public void ShearY(float angle);

		public void Apply(PMatrix source);

		public void Apply(PMatrix2D source);

		public void Apply(PMatrix3D source);

		public void Apply(float n00, float n01, float n02,
											float n10, float n11, float n12);

		public void Apply(float n00, float n01, float n02, float n03,
											float n10, float n11, float n12, float n13,
											float n20, float n21, float n22, float n23,
											float n30, float n31, float n32, float n33);

		public void PreApply(PMatrix left);

		public void PreApply(PMatrix2D left);

		public void PreApply(PMatrix3D left);

		public void PreApply(float n00, float n01, float n02,
												 float n10, float n11, float n12);

		public void PreApply(float n00, float n01, float n02, float n03,
												 float n10, float n11, float n12, float n13,
												 float n20, float n21, float n22, float n23,
												 float n30, float n31, float n32, float n33);

		public void Print();
		
		public float[] Mult(float[] source, float[] target);

		public void Transpose();

		public bool Invert();

		public float Determinant();
	}
}
