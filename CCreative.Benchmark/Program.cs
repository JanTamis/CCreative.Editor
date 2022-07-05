using System.Numerics;
using System.Runtime.Versioning;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;
using Vector = CCreative.Vector;

[assembly: RequiresPreviewFeatures]
public static class Program
{
	public static void Main()
	{
		BenchmarkRunner.Run<SimdTest>();
		//Vector.Add(new Vector(1, 2, 3), new Vector(4, 5, 6), out var result);
		
		//Console.WriteLine(result);
	}
}

// [MemoryDiagnoser]
// [DisassemblyDiagnoser(exportHtml: true, printSource: true)]
// [ShortRunJob]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
public class SimdTest
{
	private Vector3 v31 = new Vector3(1, 2, 3);
	private Vector3 v32 = new Vector3(4, 5, 6);

	private Vector v1 = new Vector(1, 2, 3);
	private Vector v2 = new Vector(4, 5, 6);

	private Vector result;
	private Vector3 result3;

	[Benchmark]
	public void CCreative()
	{
		Vector.Add(v1, v2, out result);
	}

	[Benchmark(Baseline = true)]
	public void Microsoft()
	{
		result3 = v31 + v32;
	}
}