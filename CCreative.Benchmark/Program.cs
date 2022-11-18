using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Versioning;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using CCreative;
using Microsoft.Toolkit.HighPerformance.Helpers;
using Math = CCreative.Math;

[assembly: RequiresPreviewFeatures]

public static class Program
{
	public static void Main()
	{
		BenchmarkRunner.Run<SimdTest>();
	}
}

[MemoryDiagnoser]
[Config(typeof(MyConfig))]
public class SimdTest
{
	private class MyConfig : ManualConfig
	{
		public MyConfig()
		{
			SummaryStyle = BenchmarkDotNet.Reports.SummaryStyle.Default.WithRatioStyle(RatioStyle.Trend);
		}
	}

	private VectorTemp vTemp = new(1, 2, 3);
	private VectorTemp vTemp2 = new(4, 5, 6);

	private Vector vCcreative = new(1, 2, 3);
	private Vector vCcreative2 = new(4, 5, 6);

	[GlobalSetup]
	public void Initialize()
	{

	}

	[Benchmark]
	public Vector CCreative()
	{
		return vCcreative + vCcreative2;
	}

	[Benchmark(Baseline = true)]
	public VectorTemp Base()
	{
		var result = Vector128.Add(
			Unsafe.As<VectorTemp, Vector128<float>>(ref Unsafe.AsRef(in vTemp)),
			Unsafe.As<VectorTemp, Vector128<float>>(ref Unsafe.AsRef(in vTemp2)));

		return Unsafe.As<Vector128<float>, VectorTemp>(ref result);
	}
}

[SkipLocalsInit]
[StructLayout(LayoutKind.Auto, Size = 12)]
public readonly struct VectorTemp
{
	public readonly float X;
	public readonly float Y;
	public readonly float Z;

	public VectorTemp(float x, float y, float z)
	{
		X = x;
		Y = y;
		Z = z;
	}
}