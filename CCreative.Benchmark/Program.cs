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
[DisassemblyDiagnoser(exportHtml: true, printSource: true)]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
public class SIMDTest
{
	private static int[] numbers;
	private static int _toFind;

	[Params(8, 64, 256, 1024, 4096, 16384)]
	// [Params(0X7FFFFFC7 / 2)]
	public long amount;

	[GlobalSetup]
	public void GlobalSetup()
	{
		numbers = new int[amount];

		for (var i = 0; i < numbers.Length; i++)
		{
			numbers[i] = Math.RandomInt();
		}

		_toFind = Math.RandomByte();
	}

	[Benchmark(Baseline = true)]
	public int Ccreative()
	{
		return numbers.Sum();
	}

	[Benchmark]
	public int Base()
	{
		//ref var first = ref MemoryMarshal.GetArrayDataReference(numbers);

		//var index = 0;
		//var count = 0;

		//while (index < numbers.Length)
		//{
		//	if (first.Equals(_toFind))
		//	{
		//		count += first;
		//	}

		//	first = ref Unsafe.Add(ref first, 1);
		//	index++;
		//}

		//return count;

		var result = 0;

		foreach (var number in numbers)
		{
			result += number;
		}

		return result;
	}
}