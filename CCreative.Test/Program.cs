using CCreative;
using Math = CCreative.Math;

Console.WriteLine("Hello, World!");

var test = new Test();

Console.ReadLine();

public class Test : PApplet
{
	private float[] temp = new float[10000000];

	public override void Setup()
	{
		for (int i = 0; i < temp.Length; i++)
		{
			temp[i] = Math.Random();
		}
		
		Size(1000, 1000);
	}

	public override void Draw()
	{
		Background(Color(51));

		StrokeWeight(5);
		Stroke(Color(255));
		NoFill();

		var max = Math.Max(temp);

		Text(max, MouseX, MouseY);
		
		Title(Math.Floor(FrameRate));
	}
}