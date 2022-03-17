using System;
using System.IO;
using CCreative.Helpers;
using Silk.NET.OpenGL;
using SkiaSharp;

namespace CCreative;

public class SkiaImage : Image
{
	internal SKImage skImage;

	public Color[]? Pixels { get; set; }
	public int Width => skImage.Width;
	public int Height => skImage.Height;
	public int PixelDensity => 1;

	internal SkiaImage(SKImage skBitmap)
	{
		this.skImage = skBitmap ?? throw new ArgumentNullException(nameof(skBitmap));
	}

	public void LoadPixels()
	{
		Pixels = GC.AllocateUninitializedArray<Color>(skImage.Info.Width * skImage.Info.Height);

		using var pinner = new AutoPinner(Pixels);

		skImage.ReadPixels(skImage.Info, pinner);
	}

	public void UpdatePixels(int x, int y, int w, int h)
	{
		throw new NotImplementedException();
	}

	public void UpdatePixels()
	{
		if (Pixels is not null)
		{
			using var pixmap = skImage.PeekPixels();
			var span = pixmap.GetPixelSpan<Color>();

			Pixels.CopyTo(span);
		}
	}

	public void Resize(int width, int height)
	{
		throw new NotImplementedException();
	}

	public Color Get(int x, int y)
	{
		using var pixMap = skImage.PeekPixels();
		var color = pixMap.GetPixelColor(x, y);

		return new Color
		{
			A = color.Alpha,
			R = color.Red,
			G = color.Green,
			B = color.Blue,
		};
	}

	public Image Get(int x, int y, int w, int h)
	{
		throw new NotImplementedException();
	}

	public Image Get()
	{
		using var pixMap = skImage.PeekPixels();

		return new SkiaImage(SKImage.FromPixels(pixMap));
	}

	public Image Copy()
	{
		using var pixMap = skImage.PeekPixels();
		return new SkiaImage(SKImage.FromPixels(pixMap));
	}

	public void Set(int x, int y, Color color)
	{
		using var pixMap = skImage.PeekPixels();
		var span = pixMap.GetPixelSpan<SKColor>();

		span[y * Width + x] = new SKColor((uint)color.GetHashCode());
	}

	public void Set(int x, int y, Image img)
	{
		throw new NotImplementedException();
	}

	public void Mask(Color[] maskArray)
	{
		throw new NotImplementedException();
	}

	public void Mask(Image img)
	{
		throw new NotImplementedException();
	}

	public void Filter(FilterTypes kind)
	{
		SKImageFilter filter = null;

		switch (kind)
		{
			case FilterTypes.Threshold:
				break;
			case FilterTypes.Gray:
				filter = SKImageFilter.CreateColorFilter(SKColorFilter.CreateColorMatrix(new[]
				{
					0.21f, 0.72f, 0.07f, 0, 0,
					0.21f, 0.72f, 0.07f, 0, 0,
					0.21f, 0.72f, 0.07f, 0, 0,
					0, 0, 0, 1, 0,
				}));
				break;
			case FilterTypes.Opaque:
				break;
			case FilterTypes.Invert:
				break;
			case FilterTypes.Posterize:
				break;
			case FilterTypes.Blur:
				filter = SKImageFilter.CreateBlur(5, 5);
				break;
			case FilterTypes.Erode:
				filter = SKImageFilter.CreateErode(5, 5);
				break;
			case FilterTypes.Dilate:
				filter = SKImageFilter.CreateDilate(5, 5);
				break;
			case FilterTypes.Sepia:
				break;
			case FilterTypes.Jitter:
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
		}

		var rect = new SKRectI(0, 0, Width, Height);
		var tempImg = skImage.ApplyImageFilter(filter, rect, rect, out _, out SKPoint point);

		skImage.Dispose();
		skImage = tempImg;
	}

	public void Filter(FilterTypes type, float param)
	{
		throw new NotImplementedException();
	}

	public void Copy(int sx, int sy, int sw, int sh, int dx, int dy, int dw, int dh)
	{
		throw new NotImplementedException();
	}

	public void Copy(Image src, int sx, int sy, int sw, int sh, int dx, int dy, int dw, int dh)
	{
		throw new NotImplementedException();
	}

	public Color BlendColor(Color c1, Color c2, BlendModes mode)
	{
		throw new NotImplementedException();
	}

	public void Blend(int sx, int sy, int sw, int sh, int dx, int dy, int dw, int dh, BlendModes mode)
	{
		throw new NotImplementedException();
	}

	public void Blend(Image src, int sx, int sy, int sw, int sh, int dx, int dy, int dw, int dh, BlendModes mode)
	{
		throw new NotImplementedException();
	}

	public bool TrySave(string filename)
	{
		try
		{
			using var stream = File.OpenWrite(filename);

			var data = skImage.Encode(SKEncodedImageFormat.Gif, 100);
			stream.Write(data.Span);
		}
		catch
		{
			return false;
		}

		return true;
	}

	public void Dispose()
	{
		skImage.Dispose();
	}
}