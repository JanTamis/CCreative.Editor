using System;

namespace CCreative;

public static class Errors
{
	public static NullReferenceException PixelsNull => new("Please call LoadPixels() before manipulating pixels");
	public static NullReferenceException ImagePixelsNull => new("Please call LoadPixels() on the image before manipulating pixels");
}