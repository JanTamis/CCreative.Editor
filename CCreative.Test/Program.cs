using CCreative;
using CCreative.Data;
using static CCreative.Math;
using Random = System.Random;

var test = new Test();

public class Test : PApplet
{
	public override void Setup()
	{
		Size(1000, 1000);

		FrameRate = float.MaxValue;
	}

	public override void Draw()
	{
		Background(Color(51));

		Circle(MouseX, MouseY, 200);

		Title(FrameRate);

		Filter(FilterTypes.Blur, 25);
	}
}