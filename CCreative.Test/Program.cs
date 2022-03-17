using System.Runtime.Versioning;
using CCreative;
using static CCreative.Math;
using static CCreative.PApplet;

[assembly: RequiresPreviewFeatures]

Initialize(new Test());

public class Test : IProgram
{
	private float increment = 0.02f;

	public void Setup()
	{
		Size(500, 500);
	}

	public void Draw()
	{
		LoadPixels();

		var xoff = 0.0f; // Start xoff at 0
		// var detail = Map(MouseX, 0, Width, 0.1f, 0.6f);
		// noiseDetail(8, detail);

		// For every x,y coordinate in a 2D space, calculate a noise value and produce a brightness value
		for (var x = 0; x < Width; x++)
		{
			xoff += increment; // Increment xoff 
			var yoff = 0.0f; // For every xoff, start yoff at 0

			for (var y = 0; y < Height; y++)
			{
				yoff += increment; // Increment yoff

				// Calculate noise and scale by 255
				var bright = Noise(xoff, yoff) * 255;

				// Try using this line instead
				//float bright = random(0,255);

				// Set each pixel onscreen to a grayscale value
				Pixels[x + y * Width] = Color(bright);
			}
		}

		UpdatePixels();
		
		Title(FrameRate);
	}
}