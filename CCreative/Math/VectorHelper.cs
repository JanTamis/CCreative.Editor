using System;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;

namespace CCreative;

internal static class VectorHelper
{
	public static Vector128<float> Add(Vector128<float> left, Vector128<float> right)
	{
		if (Sse.IsSupported)
		{
			return Sse.Add(left, right);
		}

		if (AdvSimd.IsSupported)
		{
			return AdvSimd.Add(left, right);
		}

		throw new NotSupportedException("Simd is not supported on this system");
	}

	public static Vector128<float> Subtract(Vector128<float> left, Vector128<float> right)
	{
		if (Sse.IsSupported)
		{
			return Sse.Subtract(left, right);
		}

		if (AdvSimd.IsSupported)
		{
			return AdvSimd.Subtract(left, right);
		}

		throw new NotSupportedException("Simd is not supported on this system");
	}

	public static Vector128<float> Multiply(Vector128<float> left, Vector128<float> right)
	{
		if (Sse.IsSupported)
		{
			return Sse.Multiply(left, right);
		}

		if (AdvSimd.IsSupported)
		{
			return AdvSimd.Multiply(left, right);
		}

		throw new NotSupportedException("Simd is not supported on this system");
	}

	public static Vector128<float> Divide(Vector128<float> left, Vector128<float> right)
	{
		if (Sse.IsSupported)
		{
			return Sse.Divide(left, right);
		}

		throw new NotSupportedException("Simd is not supported on this system");
	}
}