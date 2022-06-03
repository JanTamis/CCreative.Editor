using System.Runtime.Versioning;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;
using Math = CCreative.Math;
using CCreative;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

[assembly: RequiresPreviewFeatures]
public static class Program
{
	public static void Main(string[] args)
	{
		BenchmarkRunner.Run<SIMDTest>();

		Console.WriteLine(Math.Measure(() => true).TotalNanoseconds);
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
	public long amount = 256;

	[GlobalSetup]
	public void GlobalSetup()
	{
		numbers = new int[amount];

		for (var i = 0; i < numbers.Length; i++)
		{
			numbers[i] = Math.RandomInt();
		}

		_toFind = Math.RandomInt();
	}

	[Benchmark(Baseline = true)]
	public int Ccreative()
	{
		return numbers.Min();
	}

	[Benchmark]
	public int Base()
	{
		ref var first = ref MemoryMarshal.GetArrayDataReference(numbers);

		var index = 0;
		var min = Int32.MaxValue;

		while (index < numbers.Length)
		{
			min = Math.Min(min, first);

			first = ref Unsafe.Add(ref first, 1);
			index++;
		}

		return min;
	}
}