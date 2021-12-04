using System;
using CCreative.Rendering;
using SixLabors.Fonts;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Silk.NET.Maths;
using Silk.NET.Windowing;
using Timer = System.Timers.Timer;

// ReSharper disable MemberCanBePrivate.Global

namespace CCreative
{
	public partial class PApplet : PConstants
	{
		private PSurface surface;
		private PGraphics graphics;
		private PeriodicTimer currentTimer;

		public virtual void Setup()
		{
		}

		public virtual void Draw()
		{
		}

		public virtual void Timer()
		{
		}

		public virtual void MouseClicked()
		{
		}

		public virtual void MouseDragged()
		{
		}

		public virtual void MouseMove()
		{
		}

		public virtual void Resize()
		{
		}

		public event Action<Exception>? OnError;

		public int FrameCount { get; private set; }
		public bool MousePressed => MouseButton != MouseButtons.None;
		public MouseButtons MouseButton { get; private set; } = MouseButtons.None;

		public char Key { get; private set; }
		public KeyCodes KeyCode { get; private set; }

		public readonly Version CSharpVersion = new(10, 0);

		public readonly string OPERATING_SYSTEM = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "Windows" :
			RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "Linux" :
			RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? "MacOS" : String.Empty;

		public int DisplayDensity { get; private set; }

		public double FrameRate
		{
			get => _frameRate;
			set => surface.FrameRate = value;
		}

		public TimeSpan Elapsed => TimeSpan.FromSeconds(surface.window.Time);

		public int Width => surface.Size.X * DisplayDensity;
		public int Height => surface.Size.Y * DisplayDensity;

		public int ScreenWidth => surface.window.Monitor.Bounds.Size.X;
		public int ScreenHeight => surface.window.Monitor.Bounds.Size.Y;

		private RenderTypes currentRenderer;

		private double _frameRate;
		private double lastTime;

		public PApplet()
		{
			colorModeX = colorModeY = colorModeZ = colorModeA = 255;

			Setup();

			if (surface is not null)
			{
				surface.window.Center();
				surface.Run();
			}
		}

		public async void TimerInterval(int milliseconds)
		{
			currentTimer?.Dispose();

			currentTimer = new PeriodicTimer(TimeSpan.FromMilliseconds(milliseconds));

			while (await currentTimer.WaitForNextTickAsync())
			{
				Timer();
			}
		}

		/// <summary>
		/// Defines the dimension of the display window width and height in units of pixels
		/// </summary>
		/// <param name="width">the width of the renderer</param>
		/// <param name="height">the height of the renderer</param>
		public void Size(int width, int height)
		{
			Size(width, height, RenderTypes.P2D);
		}

		/// <summary>
		/// Defines the dimension of the display window width and height in units of pixels
		/// </summary>
		/// <param name="width">the width of the renderer</param>
		/// <param name="height">the height of the renderer</param>
		/// <param name="render">the renderer to use for this sketch</param>
		public async void Size(int width, int height, RenderTypes render)
		{
			surface = new PSurface(width, height);

			surface.Resize = delegate
			{
				graphics.Resize(Width, Height);
				Resize();
			};
			surface.RenderFrame = Wnd_RenderFrame;
			surface.Initialize();

			surface.MouseMove = (position) =>
			{
				MouseX = (int)position.X;
				MouseY = (int)position.Y;

				MouseMove();
			};
			
			surface.window.MakeCurrent();

			DisplayDensity = (surface.window.FramebufferSize.X / width + surface.window.FramebufferSize.Y / height) / 2;
			surface.DisplayDensity = DisplayDensity;

			graphics = render switch
			{
				RenderTypes.P2D => new PGraphicsSkiaSharp(surface.window.FramebufferSize.X, surface.window.FramebufferSize.Y,
					DisplayDensity, surface.window),
				// RenderTypes.P3D => new PGraphicsOpenGL(),
				_ => throw new ArgumentException("Please select a vaid render type", nameof(render)),
			};

			currentTimer = new PeriodicTimer(TimeSpan.FromSeconds(1));
			
			currentRenderer = render;

			while (await currentTimer.WaitForNextTickAsync())
			{
				Timer();
			}
		}

		/// <summary>
		/// Sets the cursor to a predefined symbol or an image, or makes it visible if already hidden
		/// </summary>
		/// <param name="img">the image to use as cursor</param>
		/// <param name="x">the horizontal active spot of the cursor</param>
		/// <param name="y">the vertical active spot of the cursor</param>
		public void Cursor(PImage img, int x, int y)
		{
			// img.LoadPixels();

			// surface.GameWindow.Cursor = new MouseCursor(x, y, img.Width, img.Height, img.Pixels);
		}

		private void Wnd_RenderFrame(double obj)
		{
			if (surface.window.GLContext is { IsCurrent: false })
			{
				surface.window.MakeCurrent();
			}

			graphics.BeginDraw();

			Draw();

			graphics.EndDraw();

			surface.SwapBuffers();

			_frameRate = 1 / (surface.window.Time - lastTime);
			lastTime = surface.window.Time;

			FrameCount++;

			PmouseX = MouseX;
			PmouseY = MouseY;
		}

		/// <summary>
		/// Writes data to the console (the ToString() method will be used)
		/// </summary>
		/// <param name="toPrint">data to print to console</param>
		public void Print<T>(T toPrint)
		{
			Console.Write(toPrint);
		}

		/// <summary>
		/// Writes data to the console and adds a newline to the console (the ToString() method will be used)
		/// </summary>
		/// <param name="toPrint">data to print to console</param>
		public void Println<T>(T toPrint)
		{
			Console.WriteLine(toPrint);
		}

		/// <summary>
		/// Writes a list of data to the console and adds a newline to the console (the ToString() method will be used)
		/// </summary>
		/// <param name="toPrint">list of data to print to console</param>
		public void PrintArray(IEnumerable toPrint)
		{
			var index = 0;

			foreach (var item in toPrint)
			{
				Println($"[{index++}] {item}");
			}
		}

		/// <summary>
		/// Sets the title of the window
		/// </summary>
		/// <param name="newTitle">the new title of the window</param>
		public void Title<T>(T newTitle)
		{
			surface.Title = newTitle?.ToString() ?? String.Empty;
		}


		/// <summary>
		/// Sets the location of the window
		/// </summary>
		/// <param name="x">the horizontal location of the window</param>
		/// <param name="y">the vertical location of the window</param>
		public void Location(int x, int y)
		{
			surface.Position = new Vector2D<int>(x, y);
		}

		/// <summary>
		/// Sets the location of the window
		/// </summary>
		/// <param name="location">the new locatio of the window</param>
		public void Location(PVector location)
		{
			surface.Position = new Vector2D<int>(Math.Round(location.X), Math.Round(location.Y));
		}

		/// <summary>
		/// Sets if the window can re resized
		/// </summary>
		/// <param name="resizeable">true if the window can re resizable or false to fix the size of the window</param>
		public void Resizeable(bool resizeable)
		{
			surface.Resizable = resizeable;
		}

		/// <summary>
		/// Returns the current year of the system
		/// </summary>
		/// <returns>the current year of the system</returns>
		public int Year()
		{
			return DateTime.Now.Year;
		}

		/// <summary>
		/// Returns the current month of the system
		/// </summary>
		/// <returns>the current month of the system</returns>
		public int Month()
		{
			return DateTime.Now.Month;
		}

		/// <summary>
		/// Returns the current day of the system
		/// </summary>
		/// <returns>the current day of the system</returns>
		public int Day()
		{
			return DateTime.Now.Day;
		}

		public int Hour()
		{
			return DateTime.Now.Hour;
		}

		public int Minute()
		{
			return DateTime.Now.Minute;
		}

		public int Second()
		{
			return DateTime.Now.Second;
		}

		public double Millis()
		{
			return surface.window.Time * 1000;
		}

		public Task Thread(Action method)
		{
			return Task.Run(method);
		}

		public Task Thread(Action<object?> method, object? parameter)
		{
			return Task.Factory.StartNew(method, parameter);
		}

		public void Delay(int milliSeconds)
		{
			System.Threading.Thread.Sleep(milliSeconds);
		}

		public byte[] LoadBytes(string path)
		{
			return File.ReadAllBytes(path);
		}

		public string[] LoadStrings(string path)
		{
			return File.ReadAllLines(path);
		}

		public void SaveBytes(string path, params byte[] data)
		{
			File.WriteAllBytes(path, data);
		}

		public void SaveStream(string target, string source)
		{
			File.Copy(source, target);
		}

		public void SaveStrings(string path, params string[] data)
		{
			File.WriteAllLines(path, data);
		}

		public PImage? LoadImage(string path)
		{
			return graphics.LoadImage(path);
		}

		public PImage CreateImage(int width, int height)
		{
			return graphics.CreateImage(width, height);
		}

		public async Task RequestImage(string filename, Action<PImage?> callback)
		{
			var image = await Task.Run(() => LoadImage(filename));

			callback(image);
		}

		public void Exit()
		{
			graphics.Dispose();
			surface?.Close();
		}

		public string ToJSON<T>(T target)
		{
			return JsonSerializer.Serialize(target, new JsonSerializerOptions()
			{
				WriteIndented = true,
			});
		}

		public T? FromJSON<T>(string json)
		{
			return JsonSerializer.Deserialize<T>(json);
		}

		public string ToXML<T>(T target)
		{
			using var stringwriter = new StringWriter();


			var serializer = new XmlSerializer(typeof(T));
			serializer.Serialize(stringwriter, target, new XmlSerializerNamespaces());
			return stringwriter.ToString();
		}

		public T? FromXML<T>(string xml)
		{
			using var stringReader = new StringReader(xml);

			var serializer = new XmlSerializer(typeof(T));

			return (T?)serializer.Deserialize(stringReader);
		}

		#region Array Functions

		public T[] Reverse<T>(T[] array)
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

		public void ArrayCopy<T>(T[] src, int srcPosition, T[] dst, int dstPosition, int length)
		{
			Array.Copy(src, srcPosition, dst, dstPosition, length);
		}

		public void ArrayCopy<T>(T[] src, T[] dst, int length)
		{
			Array.Copy(src, dst, length);
		}

		public void ArrayCopy<T>(T[] src, T[] dst)
		{
			Array.Copy(src, dst, src.Length);
		}

		public T[] Concat<T>(T[] a, T[] b)
		{
			var z = new T[a.Length + b.Length];
			a.CopyTo(z, 0);
			b.CopyTo(z, a.Length);

			return z;
		}

		public T[] Concat<T>(T[] a, T[] b, T[] c)
		{
			var z = new T[a.Length + b.Length + c.Length];
			a.CopyTo(z, 0);
			b.CopyTo(z, a.Length);
			c.CopyTo(z, a.Length + b.Length);

			return z;
		}

		public T[] Concat<T>(params T[][] arrays)
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

		public T[] Expand<T>(T[] array, int newSize)
		{
			var outgoing = new T[newSize];

			ArrayCopy(array, 0, outgoing, 0, Math.Min(newSize, array.Length));

			return outgoing;
		}

		public T[] Expand<T>(T[] array)
		{
			return Expand(array, array.Length > 0 ? array.Length << 1 : 1);
		}

		public T[] Append<T>(T[] array, T value)
		{
			array = Expand(array, array.Length + 1);
			array[^1] = value;

			return array;
		}

		public T[] Subset<T>(T[] array, int start, int count)
		{
			var result = new T[count];
			ArrayCopy(array, start, result, 0, count);
			return result;
		}

		public T[] Subset<T>(T[] array, Range range)
		{
			return array[range];
		}

		public T[] Subset<T>(T[] array, int start)
		{
			return Subset(array, start, array.Length - start);
		}

		public T[] Shorten<T>(T[] array)
		{
			return Subset(array, 0, array.Length - 1);
		}

		public T[] Sort<T>(T[] array, int index, int count)
		{
			var outgoing = Subset(array, index, count);

			Array.Sort(outgoing);

			return outgoing;
		}

		public T[] Sort<T>(T[] array, int count)
		{
			return Sort(array, 0, count);
		}

		public T[] Sort<T>(T[] array)
		{
			return Sort(array, 0, array.Length);
		}

		public T[] Splice<T>(T[] array, T[] value, int index)
		{
			var outgoing = GC.AllocateUninitializedArray<T>(array.Length + value.Length);

			ArrayCopy(array, 0, outgoing, 0, index);
			ArrayCopy(value, 0, outgoing, index, value.Length);
			ArrayCopy(array, index, outgoing, index + value.Length, array.Length - index);

			return outgoing;
		}

		public T[] Splice<T>(T[] array, T value, int index)
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

		public string Join(char seperator, params string[] strings)
		{
			return String.Join(seperator, strings);
		}

		public static string[] Match(string str, string regexp)
		{
			var matches = Regex.Matches(str, regexp);
			var outgoing = new string[matches.Count];

			for (var i = 0; i < outgoing.Length; i++)
			{
				outgoing[i] = matches[i].Value;
			}

			return outgoing;
		}

		public static string[][] MatchAll(string str, string regexp)
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
			return num?.ToString() ?? String.Empty;
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
			return num.ToString("N" + digits, null);
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
			var result = num?.ToString() ?? String.Empty;

			if (result.Contains(decSeparator))
			{
				var digits = result.IndexOf(decSeparator, StringComparison.Ordinal);

				var decimals = result[(digits + 1)..];
				var wholeNumber = result[0..digits].PadLeft(left, '0');

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

		public string[] Split(string value, char delim = ',')
		{
			return value.Split(delim, StringSplitOptions.RemoveEmptyEntries);
		}

		public string[] SplitToken(string value, string delim)
		{
			return value.Split(delim, StringSplitOptions.RemoveEmptyEntries);
		}

		public string Trim(string str)
		{
			return str.Trim();
		}

		public string[] Trim(params string[] array)
		{
			var outgoing = new string[array.Length];

			for (var i = 0; i < outgoing.Length; i++)
			{
				outgoing[i] = Trim(array[i]);
			}

			return outgoing;
		}

		#endregion

		public Process Launch(string fileName, string arguments)
		{
			return Process.Start(fileName, arguments);
		}

		public PFont CreateFont(string name, double size)
		{
			var family = SystemFonts.Find(name);
			var font = family.CreateFont((float)size);

			return new PFont(font);
		}

		public PFont LoadFont(string filename)
		{
			throw new NotImplementedException();
		}
	}
}