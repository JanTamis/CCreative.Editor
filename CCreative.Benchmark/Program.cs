using System.Runtime.Versioning;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;
using CCreative;

[assembly: RequiresPreviewFeatures]
public static class Program
{
	public static void Main()
	{
		BenchmarkRunner.Run<SimdTest>();
	}
}

// [MemoryDiagnoser]
[ShortRunJob]
// [DisassemblyDiagnoser(exportHtml: true, printSource: true)]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
public class SimdTest
{
	private static float[] numbers;

	[Params(8, 64, 256, 1024, 4096, 16384)]
	// [Params(0X7FFFFFC7)]
	public int amount;

	[GlobalSetup]
	public void GlobalSetup()
	{
		numbers = new float[amount];

		for (var i = 0; i < numbers.Length; i++)
		{
			numbers[i] = i;
		}
	}
	
	[Benchmark]
	public float Base()
	{
		float result = 0;
		
		for (var i = 0; i < numbers.Length; i++)
		{
			result += numbers[i];
		}
		
		return result;
	}

	[Benchmark(Baseline = true)]
	public float CCreative()
	{
		return numbers.Sum();
	}
}