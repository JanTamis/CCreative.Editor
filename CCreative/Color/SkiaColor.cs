using SkiaSharp;

namespace CCreative;

public struct SkiaColor : Color
{
	internal SKColor skColor;


	/// <inheritdoc />
	public byte B
	{
		get => skColor.Blue;
		set => skColor = skColor.WithBlue(value);
	}

	/// <inheritdoc />
	public byte G
	{
		get => skColor.Green;
		set => skColor = skColor.WithGreen(value);
	}

	/// <inheritdoc />
	public byte R
	{
		get => skColor.Red;
		set => skColor = skColor.WithRed(value);
	}

	/// <inheritdoc />
	public byte A
	{
		get => skColor.Alpha;
		set => skColor = skColor.WithAlpha(value);
	}

	internal SkiaColor(SKColor color)
	{
		skColor = color;
	}

	internal SkiaColor(byte alpha, byte red, byte green, byte blue)
	{
		skColor =  new SKColor(red, green, blue, alpha);
	}

	public override string ToString()
	{
		return skColor.ToString();
	}
}