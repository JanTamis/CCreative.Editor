using CCreative;
using static CCreative.Math;

var test = new Test();

public class Test : PApplet
{
	public override void Setup()
	{
		Size(1000, 1000);
	}

	public override void Draw()
	{
		Background(Color(51));

		Title(RandomString(10));
	}
}