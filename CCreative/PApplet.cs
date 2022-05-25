using System;
using CCreative.Rendering;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Silk.NET.Maths;
using Silk.NET.Windowing;
using SixLabors.Fonts;

#pragma warning disable CS1591

// ReSharper disable MemberCanBeprivate static.Global

namespace CCreative;

[RequiresPreviewFeatures]
public static partial class PApplet
{
	private static PSurface? surface;
	private static PGraphics graphics;
	private static PeriodicTimer currentTimer;

	private static IProgram program;

	public static event Action<Exception>? OnError;

	public static int FrameCount { get; private set; }
	public static bool MousePressed => MouseButton != MouseButtons.None;
	public static MouseButtons MouseButton { get; private set; } = MouseButtons.None;

	public static char Key { get; private set; }
	public static KeyCodes KeyCode { get; private set; }

	public static readonly Version CSharpVersion = new(10, 0);

	public static readonly string OPERATING_SYSTEM =
		OperatingSystem.IsWindows() ? "Windows" :
		OperatingSystem.IsLinux() ? "Linux" :
		OperatingSystem.IsMacOS() ? "MacOS" : String.Empty;

	public static int DisplayDensity { get; private set; }

	public static float FrameRate
	{
		get => _frameRate;
		set => surface.FrameRate = value;
	}

	public static TimeSpan Elapsed => TimeSpan.FromSeconds(surface.window.Time);

	public static int Width => surface.Size.X * DisplayDensity;
	public static int Height => surface.Size.Y * DisplayDensity;

	public static int ScreenWidth => surface.window.Monitor.Bounds.Size.X;
	public static int ScreenHeight => surface.window.Monitor.Bounds.Size.Y;

	private static RenderTypes currentRenderer;

	private static float _frameRate;
	private static double lastTime;

	static PApplet()
	{
		colorModeX = colorModeY = colorModeZ = colorModeA = 255;
	}

	public static void Initialize(IProgram program)
	{
		PApplet.program = program;

		FrameCount = 0;

		program.Setup();

		if (surface is not null)
		{
			surface.window.Center();
			surface.Run();
		}
	}

	public static void Stop()
	{
		if (surface is not null)
		{
			surface.Close();
			surface.window?.Dispose();
		}
	}

	public static async void TimerInterval(int milliseconds)
	{
		currentTimer?.Dispose();
		currentTimer = new PeriodicTimer(TimeSpan.FromMilliseconds(milliseconds));

		try
		{
			while (await currentTimer.WaitForNextTickAsync().ConfigureAwait(true))
			{
				await Task.Factory.StartNew(() => program?.Timer(), default, TaskCreationOptions.None, TaskScheduler.Default);
			}
		}
		catch (Exception e)
		{
		}
	}

	/// <summary>
	/// Defines the dimension of the display window width and height in units of pixels
	/// </summary>
	/// <param name="width">the width of the renderer</param>
	/// <param name="height">the height of the renderer</param>
	/// <param name="render">the renderer to use for this sketch</param>
	public static async void Size(int width, int height, RenderTypes render = RenderTypes.P2D)
	{
		surface = new PSurface(width, height)
		{
			Resize = delegate { graphics.Resize(Width, Height); },
			RenderFrame = Wnd_RenderFrame,
		};

		surface.Initialize();

		surface.MouseMove = position => { (MouseX, MouseY) = position; };

		surface.window.MakeCurrent();

		DisplayDensity = (surface.window.FramebufferSize.X / width + surface.window.FramebufferSize.Y / height) / 2;
		surface.DisplayDensity = DisplayDensity;

		graphics = render switch
		{
			RenderTypes.P2D => new PGraphicsSkiaSharp(surface.window.FramebufferSize.X, surface.window.FramebufferSize.Y,
				DisplayDensity, surface.window),
			// RenderTypes.P3D => new PGraphicsOpenGL(),
			_ => throw new ArgumentException("Please select a valid render type", nameof(render)),
		};

		currentTimer = new PeriodicTimer(TimeSpan.FromSeconds(1));

		currentRenderer = render;

		while (await currentTimer.WaitForNextTickAsync())
		{
			program?.Timer();
		}
	}

	/// <summary>
	/// Sets the cursor to a predefined symbol or an image, or makes it visible if already hidden
	/// </summary>
	/// <param name="img">the image to use as cursor</param>
	/// <param name="x">the horizontal active spot of the cursor</param>
	/// <param name="y">the vertical active spot of the cursor</param>
	public static void Cursor(Image img, int x, int y)
	{
		// img.LoadPixels();

		// surface.GameWindow.Cursor = new MouseCursor(x, y, img.Width, img.Height, img.Pixels);
	}

	private static void Wnd_RenderFrame(double obj)
	{
		if (surface.window.GLContext is { IsCurrent: false })
		{
			surface.window.MakeCurrent();
		}

		graphics.BeginDraw();

		program.Draw();

		graphics.EndDraw();

		surface.SwapBuffers();

		_frameRate = (float)(1 / (surface.window.Time - lastTime));
		lastTime = surface.window.Time;

		FrameCount++;

		PmouseX = MouseX;
		PmouseY = MouseY;
	}

	/// <summary>
	/// Writes data to the console (the ToString() method will be used)
	/// </summary>
	/// <param name="toPrint">data to print to console</param>
	public static void Print<T>(T toPrint)
	{
		Console.Write(toPrint);
	}

	/// <summary>
	/// Writes data to the console and adds a newline to the console (the ToString() method will be used)
	/// </summary>
	/// <param name="toPrint">data to print to console</param>
	public static void PrintLine<T>(T toPrint)
	{
		Console.WriteLine(toPrint);
	}

	/// <summary>
	/// Writes a list of data to the console and adds a newline to the console (the ToString() method will be used)
	/// </summary>
	/// <param name="toPrint">list of data to print to console</param>
	public static void PrintArray<T>(IEnumerable<T> toPrint)
	{
		var index = 0;

		foreach (var item in toPrint)
		{
			PrintLine($"[{index++}] {item}");
		}
	}

	/// <summary>
	/// Sets the title of the window
	/// </summary>
	/// <param name="newTitle">the new title of the window</param>
	public static void Title<T>(T newTitle)
	{
		surface.Title = newTitle?.ToString() ?? String.Empty;
	}

	/// <summary>
	/// Sets the location of the window
	/// </summary>
	/// <param name="x">the horizontal location of the window</param>
	/// <param name="y">the vertical location of the window</param>
	public static void Location(int x, int y)
	{
		surface.Position = new Vector2D<int>(x, y);
	}

	/// <summary>
	/// Sets the location of the window
	/// </summary>
	/// <param name="location">the new locatio of the window</param>
	public static void Location(Vector location)
	{
		surface.Position = new Vector2D<int>(Math.Round(location.X), Math.Round(location.Y));
	}

	/// <summary>
	/// Sets if the window can re resized
	/// </summary>
	/// <param name="resizeable">true if the window can re resizable or false to fix the size of the window</param>
	public static void Resizeable(bool resizeable)
	{
		surface.Resizable = resizeable;
	}

	/// <summary>
	/// Returns the current year of the system
	/// </summary>
	/// <returns>the current year of the system</returns>
	public static int Year()
	{
		return DateTime.Now.Year;
	}

	/// <summary>
	/// Returns the current month of the system
	/// </summary>
	/// <returns>the current month of the system</returns>
	public static int Month()
	{
		return DateTime.Now.Month;
	}

	/// <summary>
	/// Returns the current day of the system
	/// </summary>
	/// <returns>the current day of the system</returns>
	public static int Day()
	{
		return DateTime.Now.Day;
	}

	public static int Hour()
	{
		return DateTime.Now.Hour;
	}

	public static int Minute()
	{
		return DateTime.Now.Minute;
	}

	public static int Second()
	{
		return DateTime.Now.Second;
	}

	public static double Millis()
	{
		return surface.window.Time * 1000;
	}

	public static Task Thread(Action method)
	{
		return Task.Run(method);
	}

	public static Task Thread(Action<object?> method, object? parameter)
	{
		return Task.Factory.StartNew(method, parameter);
	}

	public static void Delay(int milliSeconds)
	{
		System.Threading.Thread.Sleep(milliSeconds);
	}

	public static byte[] LoadBytes(string path)
	{
		return File.ReadAllBytes(path);
	}

	public static string[] LoadStrings(string path)
	{
		return File.ReadAllLines(path);
	}

	public static void SaveBytes(string path, params byte[] data)
	{
		File.WriteAllBytes(path, data);
	}

	public static void SaveStream(string target, string source)
	{
		File.Copy(source, target);
	}

	public static void SaveStrings(string path, params string[] data)
	{
		File.WriteAllLines(path, data);
	}

	public static Image? LoadImage(string path)
	{
		return graphics.LoadImage(path);
	}

	public static Image CreateImage(int width, int height)
	{
		return graphics.CreateImage(width, height);
	}

	public static async Task RequestImage(string filename, Action<Image?> callback)
	{
		var image = await Task.Run(() => LoadImage(filename));

		callback(image);
	}

	public static void Exit()
	{
		graphics.Dispose();
		surface?.Close();
	}

	public static string ToJSON<T>(T target)
	{
		return JsonSerializer.Serialize(target, new JsonSerializerOptions
		{
			WriteIndented = true,
		});
	}

	public static T? FromJSON<T>(string json)
	{
		return JsonSerializer.Deserialize<T>(json);
	}

	public static string ToXML<T>(T target)
	{
		using var stringWriter = new StringWriter();

		var serializer = new XmlSerializer(typeof(T));
		serializer.Serialize(stringWriter, target, new XmlSerializerNamespaces());
		return stringWriter.ToString();
	}

	public static T? FromXML<T>(string xml)
	{
		using var stringReader = new StringReader(xml);

		var serializer = new XmlSerializer(typeof(T));

		return (T?)serializer.Deserialize(stringReader);
	}

	#region Array Functions

	public static T[] Reverse<T>(T[] array)
	{
		if (array is null)
		{
			throw new ArgumentException("The array can't be null", nameof(array));
		}

		var outgoing = new T[array.Length];
		ArrayCopy(array, outgoing);

		Array.Reverse(outgoing);

		return outgoing;
	}

	public static void ArrayCopy<T>(T[] src, int srcPosition, T[] dst, int dstPosition, int length)
	{
		Array.Copy(src, srcPosition, dst, dstPosition, length);
	}

	public static void ArrayCopy<T>(T[] src, T[] dst, int length)
	{
		Array.Copy(src, dst, length);
	}

	public static void ArrayCopy<T>(T[] src, T[] dst)
	{
		Array.Copy(src, dst, src.Length);
	}

	public static T[] Concat<T>(T[] a, T[] b)
	{
		var z = new T[a.Length + b.Length];
		a.CopyTo(z, 0);
		b.CopyTo(z, a.Length);

		return z;
	}

	public static T[] Concat<T>(T[] a, T[] b, T[] c)
	{
		var z = new T[a.Length + b.Length + c.Length];
		a.CopyTo(z, 0);
		b.CopyTo(z, a.Length);
		c.CopyTo(z, a.Length + b.Length);

		return z;
	}

	public static T[] Concat<T>(params T[][] arrays)
	{
		var outgoing = new T[arrays.Sum(a => a.Length)];
		var offset = 0;

		for (var x = 0; x < arrays.Length; x++)
		{
			arrays[x].CopyTo(outgoing, offset);
			offset += arrays[x].Length;
		}

		return outgoing;
	}

	public static T[] Expand<T>(T[] array, int newSize)
	{
		var outgoing = new T[newSize];

		ArrayCopy(array, 0, outgoing, 0, Math.Min(newSize, array.Length));

		return outgoing;
	}

	public static T[] Expand<T>(T[] array)
	{
		return Expand(array, array.Length > 0 ? array.Length << 1 : 1);
	}

	public static T[] Append<T>(T[] array, T value)
	{
		array = Expand(array, array.Length + 1);
		array[^1] = value;

		return array;
	}

	public static T[] Subset<T>(T[] array, int start, int count)
	{
		var result = new T[count];
		ArrayCopy(array, start, result, 0, count);
		return result;
	}

	public static T[] Subset<T>(T[] array, Range range)
	{
		return array[range];
	}

	public static T[] Subset<T>(T[] array, int start)
	{
		return Subset(array, start, array.Length - start);
	}

	public static T[] Shorten<T>(T[] array)
	{
		return Subset(array, 0, array.Length - 1);
	}

	public static T[] Sort<T>(T[] array, int index, int count)
	{
		var outgoing = Subset(array, index, count);

		Array.Sort(outgoing);

		return outgoing;
	}

	public static T[] Sort<T>(T[] array, int count)
	{
		return Sort(array, 0, count);
	}

	public static T[] Sort<T>(T[] array)
	{
		return Sort(array, 0, array.Length);
	}

	public static T[] Splice<T>(T[] array, T[] value, int index)
	{
		var outgoing = GC.AllocateUninitializedArray<T>(array.Length + value.Length);

		ArrayCopy(array, 0, outgoing, 0, index);
		ArrayCopy(value, 0, outgoing, index, value.Length);
		ArrayCopy(array, index, outgoing, index + value.Length, array.Length - index);

		return outgoing;
	}

	public static T[] Splice<T>(T[] array, T value, int index)
	{
		var outgoing = GC.AllocateUninitializedArray<T>(array.Length + 1);
		ArrayCopy(array, 0, outgoing, 0, index);

		outgoing[index] = value;
		ArrayCopy(array, index, outgoing, index + 1, array.Length - index);

		return outgoing;
	}

	#endregion

	#region String Functions

	public static string StringConcat(params string[] strings)
	{
		return String.Concat(strings);
	}

	public static string Join(char separator, params string[] strings)
	{
		return String.Join(separator, strings);
	}
	
	public static bool IsMatch(string input, [StringSyntax(StringSyntaxAttribute.Regex)] string pattern)
	{
		return Regex.IsMatch(input, pattern);
	}

	public static string[] Match(string str, [StringSyntax(StringSyntaxAttribute.Regex)] string regexp)
	{
		var matches = Regex.Matches(str, regexp);
		var outgoing = new string[matches.Count];

		for (var i = 0; i < outgoing.Length; i++)
		{
			outgoing[i] = matches[i].Value;
		}

		return outgoing;
	}

	public static string[][] MatchAll(string str, [StringSyntax(StringSyntaxAttribute.Regex)] string regexp)
	{
		var matches = Regex.Matches(str, regexp);

		var outgoing = new string[matches.Count][];

		for (var i = 0; i < outgoing.Length; i++)
		{
			var match = matches[i].Groups;
			var results = new string[match.Count];

			for (var j = 0; j < results.Length; j++)
			{
				results[j] = match[j].Value;
			}

			outgoing[i] = results;
		}

		return outgoing;
	}

	public static string Nf<T>(T num) where T : IConvertible, IFormattable
	{
		return num?.ToString("N", null) ?? String.Empty;
	}

	public static string[] Nf<T>(T[] num) where T : IConvertible, IFormattable
	{
		var result = new string[num.Length];

		for (var i = 0; i < result.Length; i++)
		{
			result[i] = Nf(num[i]);
		}

		return result;
	}

	public static string Nf<T>(T num, int digits) where T : IFormattable
	{
		return num.ToString($"N{digits}" + digits, null);
	}

	public static string[] Nf<T>(T[] num, int digits) where T : IFormattable
	{
		var result = new string[num.Length];

		for (var i = 0; i < result.Length; i++)
		{
			result[i] = Nf(num[i], digits);
		}

		return result;
	}

	public static string Nf<T>(T num, int left, int right) where T : IFormattable
	{
		var decSeparator = System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;
		var result = num.ToString() ?? String.Empty;

		if (result.Contains(decSeparator))
		{
			var digits = result.IndexOf(decSeparator, StringComparison.Ordinal);

			var decimals = result[(digits + 1)..];
			var wholeNumber = result[..digits].PadLeft(left, '0');

			if (right == 0)
			{
				right = 3;
			}

			if (right > decimals.Length)
			{
				decimals = decimals.PadRight(right, '0');
			}
			else if (right < decimals.Length)
			{
				decimals = decimals[..right];
			}

			return String.Concat(wholeNumber, decSeparator, decimals);
		}
		else
		{
			return String.Concat(Nf(num, left), decSeparator, new String('0', right));
		}
	}

	public static string[] Nf<T>(T[] num, int left, int right) where T : IConvertible, IFormattable
	{
		var result = new string[num.Length];

		for (var i = 0; i < result.Length; i++)
		{
			result[i] = Nf(num[i], left, right);
		}

		return result;
	}

	public static string Nfc<T>(T num) where T : IConvertible, IFormattable
	{
		return num.ToString("N", null);
	}

	public static string[] Nfc<T>(T[] num) where T : IConvertible, IFormattable
	{
		var result = new string[num.Length];

		for (var i = 0; i < result.Length; i++)
		{
			result[i] = Nfc(num[i]);
		}

		return result;
	}

	public static string Nfc<T>(T num, int right) where T : IConvertible, IFormattable
	{
		return num.ToString($"N{right}", null);
	}

	public static string[] Nfc<T>(T[] num, int right) where T : IConvertible, IFormattable
	{
		var result = new string[num.Length];

		for (var i = 0; i < result.Length; i++)
		{
			result[i] = Nfc(num[i], right);
		}

		return result;
	}

	public static string Nfp<T>(T num) where T : IConvertible, IFormattable
	{
		return $"{num:+;-;+}{Nf(num)}";
	}

	public static string[] Nfp<T>(T[] num) where T : IConvertible, IFormattable
	{
		var result = new string[num.Length];

		for (var i = 0; i < result.Length; i++)
		{
			result[i] = Nfp(num[i]);
		}

		return result;
	}

	public static string Nfp<T>(T num, int digits) where T : IConvertible, IFormattable
	{
		return $"{num:+;-;+}{Nf(num, digits)}";
	}

	public static string[] Nfp<T>(T[] num, int digits) where T : IConvertible, IFormattable
	{
		var result = new string[num.Length];

		for (var i = 0; i < result.Length; i++)
		{
			result[i] = Nfp(num[i], digits);
		}

		return result;
	}

	public static string Nfp<T>(T num, int left, int right) where T : IConvertible, IFormattable
	{
		return num?.ToString("+;-;+", null) + Nf(num, left, right);
	}

	public static string[] Nfp<T>(T[] num, int left, int right) where T : IConvertible, IFormattable
	{
		var result = new string[num.Length];

		for (var i = 0; i < result.Length; i++)
		{
			result[i] = Nfp(num[i], left, right);
		}

		return result;
	}

	public static string Nfs<T>(T num) where T : IConvertible, IFormattable
	{
		return num?.ToString(" ;''", null) + Nf(num);
	}

	public static string[] Nfs<T>(T[] num) where T : IConvertible, IFormattable
	{
		var result = new string[num.Length];

		for (var i = 0; i < result.Length; i++)
		{
			result[i] = Nfs(num[i]);
		}

		return result;
	}

	public static string Nfs<T>(T num, int digits) where T : IConvertible, IFormattable
	{
		return num?.ToString(" ;''", null) + Nf(num, digits);
	}

	public static string[] Nfs<T>(T[] num, int digits) where T : IConvertible, IFormattable
	{
		var result = new string[num.Length];

		for (var i = 0; i < result.Length; i++)
		{
			result[i] = Nfs(num[i], digits);
		}

		return result;
	}

	public static string Nfs<T>(T num, int left, int right) where T : IConvertible, IFormattable
	{
		return num?.ToString(" ;''", null) + Nf(num, left, right);
	}

	public static string[] Nfs<T>(T[] num, int left, int right) where T : IConvertible, IFormattable
	{
		var result = new string[num.Length];

		for (var i = 0; i < result.Length; i++)
		{
			result[i] = Nfp(num[i], left, right);
		}

		return result;
	}

	public static string[] Split(string value, char delim = ',')
	{
		return value.Split(delim, StringSplitOptions.RemoveEmptyEntries);
	}

	public static string[] SplitToken(string value, string delim)
	{
		return value.Split(delim, StringSplitOptions.RemoveEmptyEntries);
	}

	public static string Trim(string str)
	{
		return str.Trim();
	}

	public static string[] Trim(params string[] array)
	{
		var outgoing = new string[array.Length];

		for (var i = 0; i < outgoing.Length; i++)
		{
			outgoing[i] = Trim(array[i]);
		}

		return outgoing;
	}

	#endregion

	public static Process Launch(string fileName, string arguments)
	{
		return Process.Start(fileName, arguments);
	}

	public static PFont CreateFont(string name, double size)
	{
		// var family = SystemFonts.Find(name);
		// var font = family.CreateFont((float)size);
		//
		// return new PFont(font);
		return null;
	}
	
	

	public static PFont LoadFont(string filename)
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// The horizontal location of the mouse on the window
	/// </summary>
	public static float MouseX { get; private set; }

	/// <summary>
	/// The vertical location of the mouse on the window
	/// </summary>
	public static float MouseY { get; private set; }

	/// <summary>
	/// The current location of the mouse on the window
	/// </summary>
	public static Vector MousePos => new(MouseX, MouseY);

	/// <summary>
	/// The vertical location of the mouse on the window on the previous frame
	/// </summary>
	public static float PmouseX { get; private set; }

	/// <summary>
	/// The horizontal location of the mouse on the window on the previous frame
	/// </summary>
	public static float PmouseY { get; private set; }

	public const float TAU = TWO_PI;
	public const float TWO_PI = MathF.Tau;
	public const float PI = MathF.PI;
	public const float HALF_PI = PI / 2;
	public const float QUARTER_PI = PI / 4;

	public const RenderTypes P2D = RenderTypes.P3D;
	public const RenderTypes P3D = RenderTypes.P3D;

	public const MouseButtons LEFTMOUSE = MouseButtons.Left;
	public const MouseButtons CENTERMOUSE = MouseButtons.Center;
	public const MouseButtons RIGHTMOUSE = MouseButtons.Right;

	public const ShapeTypes POINTS = ShapeTypes.Points;
	public const ShapeTypes PATCHES = ShapeTypes.Patches;
	public const ShapeTypes LINE_STRIP = ShapeTypes.LineStrip;
	public const ShapeTypes LINES = ShapeTypes.Lines;
	public const ShapeTypes LINE_LOOP = ShapeTypes.LineLoop;
	public const ShapeTypes TRIANGLES = ShapeTypes.Triangles;
	public const ShapeTypes TRIANGLE_FAN = ShapeTypes.TriangleFan;
	public const ShapeTypes TRIANGLE_STRIP = ShapeTypes.TriangleStrip;
	public const ShapeTypes QUADS = ShapeTypes.Quads;
	public const ShapeTypes QUAD_STRIP = ShapeTypes.QuadStrip;
	public const ShapeTypes POLYGON = ShapeTypes.Polygon;

	public const CloseType CLOSE = CloseType.Close;

	public const FilterTypes THRESHOLD = FilterTypes.Threshold;
	public const FilterTypes GRAY = FilterTypes.Gray;
	public const FilterTypes OPAQUE = FilterTypes.Opaque;
	public const FilterTypes INVERT = FilterTypes.Invert;
	public const FilterTypes POSTERIZE = FilterTypes.Posterize;
	public const FilterTypes BLUR = FilterTypes.Blur;
	public const FilterTypes ERODE = FilterTypes.Erode;
	public const FilterTypes DILATE = FilterTypes.Dilate;
	public const FilterTypes SEPIA = FilterTypes.Sepia;
	public const FilterTypes JITTER = FilterTypes.Jitter;

	public const DrawTypes CORNER = DrawTypes.Corner;
	public const DrawTypes CORNERS = DrawTypes.Corners;
	public const DrawTypes CENTER = DrawTypes.Center;
	public const DrawTypes RADIUS = DrawTypes.Radius;

	public const KeyCodes UP = KeyCodes.Up;
	public const KeyCodes DOWN = KeyCodes.Down;
	public const KeyCodes LEFT = KeyCodes.Left;
	public const KeyCodes RIGHT = KeyCodes.Right;
	public const KeyCodes ALT = KeyCodes.Alt;
	public const KeyCodes CONTROL = KeyCodes.Control;
	public const KeyCodes SHIFT = KeyCodes.Shift;
	public const KeyCodes BACKSPACE = KeyCodes.Backspace;
	public const KeyCodes TAB = KeyCodes.Tab;
	public const KeyCodes ENTER = KeyCodes.Enter;
	public const KeyCodes ESC = KeyCodes.Escape;
	public const KeyCodes DELETE = KeyCodes.Delete;

	public const BlendModes REPLACE = BlendModes.Replace;
	public const BlendModes BLEND = BlendModes.Blend;
	public const BlendModes ADD = BlendModes.Add;
	public const BlendModes SUBTRACT = BlendModes.Subtract;
	public const BlendModes DARKEST = BlendModes.Darkest;
	public const BlendModes LIGHTEST = BlendModes.Lightest;
	public const BlendModes DIFFERENCE = BlendModes.Difference;
	public const BlendModes EXCLUSION = BlendModes.Exclusion;
	public const BlendModes MULTIPLY = BlendModes.Multiply;
	public const BlendModes SCREEN = BlendModes.Screen;
	public const BlendModes OVERLAY = BlendModes.Overlay;
	public const BlendModes HARD_LIGHT = BlendModes.Hard_Light;
	public const BlendModes SOFT_LIGHT = BlendModes.Soft_Light;
	public const BlendModes DODGE = BlendModes.Dodge;
	public const BlendModes BURN = BlendModes.Burn;

	public const string LINUX = "Linux";
	public const string WINDOWS = "Windows";
	public const string OSX = "macOS";

	private static float colorModeX = 255, colorModeY = 255, colorModeZ = 255, colorModeA = 255;

	private static ColorModes currentColorMode = ColorModes.RGB;
	private static ShapeTypes currentShapeMode;

	private static DrawTypes currentRectMode = DrawTypes.Corner;
	private static DrawTypes currentEllipseMode = DrawTypes.Center;
	private static DrawTypes currentImageMode = DrawTypes.Corner;

	public const ImageFormats ALPHA = ImageFormats.Alpha;
	public const ImageFormats ARGB = ImageFormats.Argb;
	public const ImageFormats RGB = ImageFormats.Rgb;

	private static bool colorModeScale = true;
	private static bool colorModeDefault = true;
}