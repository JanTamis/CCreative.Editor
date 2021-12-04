using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks.Dataflow;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

var result = BenchmarkRunner.Run<SIMDTest>();

[ThreadingDiagnoser]
[MemoryDiagnoser]
public class SIMDTest
{
	[Params(1000000)] public int N;

	private static int degreeOfParallelism = Environment.ProcessorCount;

	private double[] array;

	[GlobalSetup]
	public void GlobalSetup()
	{
		array = new double[N]; // executed once per each N value

		for (var i = 0; i < array.Length; i++)
		{
			array[i] = Random.Shared.Next();
		}
	}

	[Benchmark(Baseline = true)]
	public double Baseline()
	{
		var min = 0d;

		for (var i = 0; i < array.Length; i++)
		{
			min += array[i];
		}

		return min;
	}

	[Benchmark]
	public double SimdParallel()
	{
		if (array.Length >= 65536)
		{
			var temp = GC.AllocateUninitializedArray<double>(degreeOfParallelism);

			var block = new ActionBlock<(double[] array, int index, int length)>(tuple =>
			{
				var (tempArray, index, length) = tuple;
				temp[index] = Sum(tempArray.AsSpan(index * degreeOfParallelism, length));
			}, new ExecutionDataflowBlockOptions()
			{
				EnsureOrdered = false,
				MaxDegreeOfParallelism = degreeOfParallelism,
			});

			for (var i = 0; i < degreeOfParallelism; i++)
			{
				block.Post((array, i, array.Length / degreeOfParallelism));
			}
			
			block.Complete();
			block.Completion.Wait();

			return Sum(temp.AsSpan());
		}
		
		return Sum(array.AsSpan());
	}
	
	[Benchmark]
	public double SimdBase()
	{
		return Sum(array.AsSpan());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static double Sum(Span<double> numbers)
	{
		var vSum = Vector<double>.Zero;
		var count = Vector<double>.Count;

		var sum = 0d;
		var i = 0;

		if (Vector.IsHardwareAccelerated && numbers.Length >= count * 2)
		{
			var vsArray = MemoryMarshal.Cast<double, Vector<double>>(numbers);

			for (i = 0; i < vsArray.Length; i++)
			{
				vSum = Vector.Add(vSum, vsArray[i]);
			}

			sum = Vector.Sum(vSum);

			i *= count;
		}

		for (; i < numbers.Length; i++)
			sum += numbers[i];

		return sum;
	}
}