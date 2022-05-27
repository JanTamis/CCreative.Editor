using System.Numerics;
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
			numbers[i] = (short)Math.RandomInt(5);
		}
	}

	[Benchmark(Baseline = true)]
	public float Ccreative()
	{
		return numbers.Sum();
	}

	[Benchmark]
	public float Base()
	{
		float count = 0;

		for (var i = 0; i < numbers.Length; i++)
		{
			count += numbers[i];
		}

		return count;
	}

	[Benchmark]
	public float VectorT()
	{
		float count = 0;
		var index = 0;

		if (System.Numerics.Vector.IsHardwareAccelerated && numbers.Length >= Vector<float>.Count)
		{
			var result = Vector<float>.Zero;

			for (; index < numbers.Length; index += Vector<float>.Count)
			{
				result += new Vector<float>(numbers, index);
			}

			count += System.Numerics.Vector.Sum(result);
		}
		
		for (; index < numbers.Length; index++)
		{
				count+= numbers[index];
		}

		return count;
	}

	[Benchmark]
	public float Linq()
	{
		return Enumerable.Sum(numbers.AsEnumerable());
	}
}