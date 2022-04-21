using System;

namespace CCreative;

/// <summary>
/// Datatype for storing images. Images may be displayed in 2D and 3D space
/// </summary>
public interface Image : IDisposable
{
	/// <summary>
	/// The pixels of the image
	/// </summary>
	Color[]? Pixels { get; set; }

	/// <summary>
	/// The width of the image in units of pixels
	/// </summary>
	int Width { get; }

	/// <summary>
	/// The height of the image in units of pixels
	/// </summary>
	int Height { get; }

	/// <summary>
	/// The pixel density of the image
	/// </summary>
	int PixelDensity { get; }

	/// <summary>
	/// The width of the image in pixels multiplied by the density
	/// </summary>
	int PixelWidth => Width * PixelDensity;

	/// <summary>
	/// The height of the image in pixels multiplied by the density
	/// </summary>
	int PixelHeight => Height * PixelDensity;

	/// <summary>
	/// Loads the pixel data for the image into its <see cref="Pixels"/> array
	/// </summary>
	void LoadPixels();

	/// <summary>
	/// Updates the image with the data in its <see cref="Pixels"/> array
	/// </summary>
	/// <param name="x">x-coordinate of the upper-left corner</param>
	/// <param name="y">y-coordinate of the upper-left corner</param>
	/// <param name="w">the width</param>
	/// <param name="h">the height</param>
	void UpdatePixels(int x, int y, int w, int h);

	/// <summary>
	/// Updates the image with the data in its <see cref="Pixels"/> array
	/// </summary>
	void UpdatePixels();

	/// <summary>
	/// Resize the image to a new width and height
	/// </summary>
	/// <param name="width">the resized image width</param>
	/// <param name="height">the resized image height</param>
	void Resize(int width, int height);

	/// <summary>
	/// Reads the color of any pixel
	/// </summary>
	/// <param name="x">x-coordinate of the pixel</param>
	/// <param name="y">y-coordinate of the pixel</param>
	/// <returns>the pixel at the coordinates</returns>
	public Color Get(int x, int y);

	/// <summary>
	/// Reads a block of pixels
	/// </summary>
	/// <param name="x">x-coordinate of the upper-left corner</param>
	/// <param name="y">y-coordinate of the upper-left corner</param>
	/// <param name="w">the width of the block</param>
	/// <param name="h">the height of the block</param>
	/// <returns>the block of pixels</returns>
	Image Get(int x, int y, int w, int h);

	/// <summary>
	/// Gets a copy of the image
	/// </summary>
	/// <returns></returns>
	Image Get();

	/// <summary>
	/// Copies the entire image
	/// </summary>
	/// <returns>the copied image</returns>
	Image Copy();

	/// <summary>
	/// Writes a color to any pixel
	/// </summary>
	/// <param name="x">x-coordinate of the pixel</param>
	/// <param name="y">y-coordinate of the pixel</param>
	/// <param name="color">the color to set to the location</param>
	void Set(int x, int y, Color color);

	/// <summary>
	/// Writes a block of pixels
	/// </summary>
	/// <param name="x">x-coordinate of the upper-left corner</param>
	/// <param name="y">y-coordinate of the upper-left corner</param>
	/// <param name="img">image to copy into the original image</param>
	void Set(int x, int y, Image img);

	/// <summary>
	/// Masks part of an image with pixels as an alpha channel
	/// </summary>
	/// <param name="maskArray">pixels to use as the mask</param>
	void Mask(Color[] maskArray);

	/// <summary>
	/// Masks part of an image from displaying by loading another image and using it as an alpha channel
	/// </summary>
	/// <param name="img">image to use as the mask</param>
	void Mask(Image img);

	void Filter(FilterTypes kind);

	void Filter(FilterTypes kind, float param);

	void Copy(int sx, int sy, int sw, int sh, int dx, int dy, int dw, int dh);

	void Copy(Image src, int sx, int sy, int sw, int sh, int dx, int dy, int dw, int dh);

	Color BlendColor(Color c1, Color c2, BlendModes mode);

	void Blend(int sx, int sy, int sw, int sh, int dx, int dy, int dw, int dh, BlendModes mode);

	void Blend(Image src, int sx, int sy, int sw, int sh, int dx, int dy, int dw, int dh, BlendModes mode);

	bool TrySave(string filename);
}