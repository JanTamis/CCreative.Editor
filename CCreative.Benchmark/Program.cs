using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;
using System.Threading.Tasks.Dataflow;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;
using Microsoft.Toolkit.HighPerformance.Helpers;

BenchmarkRunner.Run<SIMDTest>();

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
public class SIMDTest
{
	private float a = 10f;

	[GlobalSetup]
	public void GlobalSetup()
	{
		a = Random.Shared.NextSingle() * 100;
	}

	[Benchmark]
	public float Reciprocal()
	{
		return 1.0f / a;
	}

	[Benchmark]
	public float ReciprocalSimd()
	{
		if (AdvSimd.IsSupported)
		{
			return AdvSimd.ReciprocalEstimate(Vector128.CreateScalarUnsafe(a)).ToScalar();
		}

		if (Sse.IsSupported)
		{
			return Sse.Reciprocal(Vector128.CreateScalarUnsafe(a)).ToScalar();
		}

		return 1.0f / a;
	}

	[Benchmark]
	public float ReciprocalSimdScalar()
	{
		if (AdvSimd.IsSupported)
		{
			return AdvSimd.ReciprocalEstimate(Vector128.CreateScalarUnsafe(a)).ToScalar();
		}

		if (Sse.IsSupported)
		{
			return Sse.ReciprocalScalar(Vector128.CreateScalarUnsafe(a)).ToScalar();
		}

		return 1.0f / a;
	}

	[Benchmark]
	public float ReciprocalMath()
	{
		return MathF.ReciprocalEstimate(a);
	}
}