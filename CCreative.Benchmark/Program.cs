using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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
	private static byte[] numbers;
	private static byte _toFind;

	[Params(8, 64, 256, 1024, 4096, 16384)]
	// [Params(0X7FFFFFC7)]
	public long amount;

	[GlobalSetup]
	public void GlobalSetup()
	{
		numbers = new Byte[amount];

		// for (var i = 0; i < numbers.Length; i++)
		// {
		// 	numbers[i] = Math.RandomBoolean();
		// }
		
		Math.RandomBytes(numbers);

		_toFind = Math.RandomByte();
	}

	[Benchmark(Baseline = true)]
	public bool Ccreative()
	{
		return numbers.Contains(_toFind);
	}

	[Benchmark]
	public bool Base()
	{
		ref var first = ref MemoryMarshal.GetArrayDataReference(numbers);

		var index = 0;

		while (index < numbers.Length)
		{
			if (first.Equals(_toFind))
			{
				return true;
			}

			first = ref Unsafe.Add(ref first, 1);
			index++;
		}

		return false;
	}
}