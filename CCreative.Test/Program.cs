using CCreative;
using Math = CCreative.Math;

Console.WriteLine("Hello, World!");

var test = new Test();

Console.ReadLine();

public class Test : PApplet
{
	public override void Setup()
	{
		Size(1000, 1000);
	}

	public override void Draw()
	{
		Background(Color(51));
		
		Circle(MouseX, MouseY, 200);
	}
}