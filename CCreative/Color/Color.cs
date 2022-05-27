using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace CCreative;

[StructLayout(LayoutKind.Sequential)]
public struct Color
{
	/// <summary>
	/// the blue component of the color
	/// </summary>
	public byte B { get; init; }

	/// <summary>
	/// the green component of the color
	/// </summary>
	public byte G { get; init; }

	/// <summary>
	/// the red component of the color
	/// </summary>
	public byte R { get; init; }

	/// <summary>
	/// the alpha component of the color
	/// </summary>
	public byte A { get; init; }

	public Color(byte a, byte r, byte g, byte b)
	{
		A = a;
		R = r;
		G = g;
		B = b;
	}

	public Color(byte r, byte g, byte b)
	{
		A = Byte.MaxValue;
		R = r;
		G = g;
		B = b;
	}

	/// <summary>
	/// Returns the color as a hexadecimal string
	/// </summary>
	/// <returns> the hexadecimal string</returns>
	public override string ToString()
	{
		return String.Create(9, GetHashCode(), (span, code) =>
		{
			span[0] = '#';
			code.TryFormat(span[1..], out _, "X8");
		});
	}

	/// <inheritdoc />
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode()
	{
		return Unsafe.As<Color, Int32>(ref this);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public void Deconstruct(out byte a, out byte r, out byte g, out byte b)
	{
		a = A;
		r = R;
		g = G;
		b = B;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public void Deconstruct(out byte r, out byte g, out byte b)
	{
		r = R;
		g = G;
		b = B;
	}
}