using System.Numerics;
using System.Runtime.Versioning;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;
using Math = CCreative.Math;
using Vector = CCreative.Vector;

[RequiresPreviewFeatures]
public static class Program
{
	public static void Main(string[] args)
	{
		BenchmarkRunner.Run<SIMDTest>();
	}
}

[MemoryDiagnoser]
[DisassemblyDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RequiresPreviewFeatures]
public class SIMDTest
{
	Vector a = new Vector(1, 2, 3);
	Vector b = new Vector(4, 5, 6);

	Vector3 c = new Vector3(1, 2, 3);
	Vector3 d = new Vector3(4, 5, 6);
	
	[GlobalSetup]
	public void GlobalSetup()
	{
	}


	[Benchmark]
	public Vector Base()
	{
		return Vector.Add(a, b);
	}
	
	[Benchmark]
	public Vector3 SIMD()
	{
		return Vector3.Add(c, d);
	}
}