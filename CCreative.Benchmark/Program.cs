using System.Runtime.Versioning;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;
using Math = CCreative.Math;
using CCreative;

[assembly: RequiresPreviewFeatures]
public static class Program
{
	public static void Main(string[] args)
	{
		BenchmarkRunner.Run<SIMDTest>();
	}
}

[MemoryDiagnoser]
// [DisassemblyDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
public class SIMDTest
{
	private static float[] numbers;

	[Params(8, 64, 256, 1024, 4096, 16384)]
	// [Params(0X7FFFFFC7)]
	public long amount;

	[GlobalSetup]
	public void GlobalSetup()
	{
		numbers = new float[amount];

		for (var i = 0; i < numbers.Length; i++)
		{
			numbers[i] = Math.RandomInt(int.MaxValue);
		}
	}

	[Benchmark(Baseline = true)]
	public void Ccreative()
	{
		numbers.Multiply(5);
	}

	[Benchmark]
	public void Base()
	{
		for (var i = 0; i < numbers.Length; i++)
		{
			numbers[i] *= 5;
		}
	}
}