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

		var xoff = 0.0f;

		for (var x = 0; x < Width; x++)
		{
			xoff += increment;
			var yoff = 0.0f;

			for (var y = 0; y < Height; y++)
			{
				yoff += increment;

				// Calculate noise and scale by 255
				var bright = Noise(xoff, yoff) * 255;

				Pixels[x + y * Width] = Color(bright);
			}
		}

		UpdatePixels();
		
		Title(FrameRate);
		
		// Filter(BLUR, 20);
	}
}