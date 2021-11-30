using System;
using System.Runtime.InteropServices;
using CCreative.ObjectTK;
using SkiaSharp;

namespace CCreative;

public class SkiaImage : PImage
{
	private SKBitmap skBitmap;
	public byte[]? Pixels { get; set; }
	public int Width => skBitmap.Width;
	public int Height => Height;
	public int PixelDensity => 1;

	internal SkiaImage(SKBitmap skBitmap)
	{
		this.skBitmap = skBitmap ?? throw new ArgumentNullException(nameof(skBitmap));
	}

	public void LoadPixels()
	{
		Pixels = skBitmap.Bytes;
	}

	public void UpdatePixels(int x, int y, int w, int h)
	{
		throw new System.NotImplementedException();
	}

	public void UpdatePixels()
	{
		var pinnedArray = new AutoPinner(Pixels);

		skBitmap.InstallPixels(skBitmap.Info, pinnedArray);
		skBitmap.NotifyPixelsChanged();
	}

	public void Resize(int width, int height)
	{
		var newImage = new SKBitmap(width, height, skBitmap.ColorType, skBitmap.AlphaType);
		skBitmap.ScalePixels(newImage, SKFilterQuality.High);
		
		skBitmap.Dispose();
		skBitmap = newImage;
	}

	public Color Get(int x, int y)
	{
		var span = MemoryMarshal.Cast<byte, MemoryColor>(Pixels);

		return span[x * Width + y];
	}

	public PImage Get(int x, int y, int w, int h)
	{
		throw new System.NotImplementedException();
	}

	public PImage Get()
	{
		var newImage = new SKBitmap(Width, Height, skBitmap.ColorType, skBitmap.AlphaType);

		skBitmap.CopyTo(newImage);

		return new SkiaImage(newImage);
	}

	public PImage Copy()
	{
		var newImage = new SKBitmap(Width, Height, skBitmap.ColorType, skBitmap.AlphaType);

		skBitmap.CopyTo(newImage);

		return new SkiaImage(newImage);
	}

	public void Set(int x, int y, Color color)
	{
		throw new System.NotImplementedException();
	}

	public void Set(int x, int y, PImage img)
	{
		throw new System.NotImplementedException();
	}

	public void Mask(Color[] maskArray)
	{
		throw new System.NotImplementedException();
	}

	public void Mask(PImage img)
	{
		throw new System.NotImplementedException();
	}

	public void Filter(FilterTypes kind)
	{
		throw new System.NotImplementedException();
	}

	public void Filter(FilterTypes type, double param)
	{
		throw new System.NotImplementedException();
	}

	public void Copy(int sx, int sy, int sw, int sh, int dx, int dy, int dw, int dh)
	{
		throw new System.NotImplementedException();
	}

	public void Copy(PImage src, int sx, int sy, int sw, int sh, int dx, int dy, int dw, int dh)
	{
		throw new System.NotImplementedException();
	}

	public Color BlendColor(Color c1, Color c2, BlendModes mode)
	{
		throw new System.NotImplementedException();
	}

	public void Blend(int sx, int sy, int sw, int sh, int dx, int dy, int dw, int dh, BlendModes mode)
	{
		throw new System.NotImplementedException();
	}

	public void Blend(PImage src, int sx, int sy, int sw, int sh, int dx, int dy, int dw, int dh, BlendModes mode)
	{
		throw new System.NotImplementedException();
	}

	public bool TrySave(string filename)
	{
		throw new System.NotImplementedException();
	}
	
	public void Dispose()
	{
		throw new System.NotImplementedException();
	}
}