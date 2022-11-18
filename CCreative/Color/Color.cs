using System;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;

namespace CCreative;

[StructLayout(LayoutKind.Sequential)]
public struct Color : IEqualityOperators<Color, Color, bool>
{
	/// <summary>
	/// the blue component of the color
	/// </summary>
	public byte B;

	/// <summary>
	/// the green component of the color
	/// </summary>
	public byte G;

	/// <summary>
	/// the red component of the color
	/// </summary>
	public byte R;

	/// <summary>
	/// the alpha component of the color
	/// </summary>
	public byte A;

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

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(Color other)
	{
		return GetHashCode() == other.GetHashCode();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals(object? obj)
	{
		return obj is Color clr && clr.Equals(this);
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

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(Color left, Color right)
	{
		return left.Equals(right);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(Color left, Color right)
	{
		return !(left == right);
	}
}