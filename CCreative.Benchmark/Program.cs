using System.Numerics;
using System.Runtime.Versioning;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;
using Math = CCreative.Math;

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
	private static short[] numbers;

	[Params(8, 64, 256, 1024, 4096, 16384)]
	// [Params(0X7FFFFFC7)]
	public long amount;

	[GlobalSetup]
	public void GlobalSetup()
	{
		numbers = new short[amount];

		for (var i = 0; i < numbers.Length; i++)
		{
			numbers[i] = (short)Math.RandomInt(5);
		}
	}

	[Benchmark(Baseline = true)]
	public int CCreative()
	{
		return Math.Count<short>(ref Math.GetReference(numbers), numbers.Length, 3);
	}

	[Benchmark]
	public int Base()
	{
		var count = 0;

		for (var i = 0; i < numbers.Length; i++)
		{
			if (numbers[i] == 3)
			{
				count++;
			}
		}

		return count;
	}

	[Benchmark]
	public int VectorT()
	{
		var count = 0;
		var index = 0;

		if (Vector.IsHardwareAccelerated && numbers.Length >= Vector<short>.Count)
		{
			var scalarResult = new Vector<short>(3);
			var result = Vector<short>.Zero;

			for (; index < numbers.Length; index += Vector<short>.Count)
			{
				result += Vector.Equals(new Vector<short>(numbers, index), scalarResult);
			}

			count = Math.Abs(Vector.Sum(result));
		}
		
		for (; index < numbers.Length; index++)
		{
			if (numbers[index] == 3)
			{
				count++;
			}
		}

		return count;
	}
}