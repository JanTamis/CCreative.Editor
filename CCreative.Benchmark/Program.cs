using System.Runtime.Versioning;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;
using Math = CCreative.Math;

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
	private static float[] numbers;

	[Params(64, 128, 256, 512, 1024, 2048, 4096, 8192, 16384, 32768)]
	public long amount;

	[GlobalSetup]
	public void GlobalSetup()
	{
		numbers = new float[amount];

		for (var i = 0; i < numbers.Length; i++)
		{
			numbers[i] = Math.Random(amount);
		}
	}

	[Benchmark(Baseline = true)]
	public float CCreative()
	{
		return Math.Sum(numbers);
	}

	[Benchmark]
	public float Base()
	{
		var sum = 0f;
		var index = 0;

		while (index + 8 < numbers.Length)
		{
			sum += numbers[index + 7];
			sum += numbers[index + 6];
			sum += numbers[index + 5];
			sum += numbers[index + 4];
			sum += numbers[index + 3];
			sum += numbers[index + 2];
			sum += numbers[index + 1];
			sum += numbers[index + 0];

			index += 8;
		}

		while (index + 4 < numbers.Length)
		{
			sum += numbers[index + 3];
			sum += numbers[index + 2];
			sum += numbers[index + 1];
			sum += numbers[index + 0];

			index += 4;
		}

		while (index < numbers.Length)
		{
			sum += numbers[index++];
		}

		return sum;
	}
}