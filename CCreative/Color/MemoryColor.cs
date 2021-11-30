using System;
using System.Runtime.InteropServices;

namespace CCreative;

[StructLayout(LayoutKind.Sequential)]
public struct MemoryColor : Color
{
	private byte b, g, r, a;

	public byte B
	{
		get => b;
		set => b = value;
	}

	public byte G
	{
		get => g;
		set => g = value;
	}

	public byte R
	{
		get => r;
		set => r = value;
	}

	public byte A
	{
		get => a;
		set => a = value;
	}

	public MemoryColor(byte a, byte r, byte g, byte b)
	{
		this.b = b;
		this.g = g;
		this.r = r;
		this.a = a;
	}
}