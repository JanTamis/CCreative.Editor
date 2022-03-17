using System;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.Windowing;

namespace CCreative
{
	internal class PSurface
	{
		public const int MIN_WINDOW_WIDTH = 256;
		public const int MIN_WINDOW_HEIGHT = 256;

		internal IWindow window;
		private IInputContext inputContext;

		public int DisplayDensity { get; set; }

		public string Title
		{
			get => window.Title;
			set => window.Title = value;
		}

		public Vector2D<int> Position
		{
			get => window.Position;
			set => window.Position = value;
		}

		public Vector2D<int> Size
		{
			get => window.Size;
			set => window.Size = value;
		}

		public float FrameRate
		{
			get => (float)window.FramesPerSecond;
			set => window.FramesPerSecond = value;
		}

		public bool Resizable
		{
			get => window.WindowBorder == WindowBorder.Resizable;
			set => window.WindowBorder = value ? WindowBorder.Resizable : WindowBorder.Fixed;
		}

		public Action<Vector2D<int>> Resize
		{
			set => window.Resize += value;
		}

		public Action<double> RenderFrame
		{
			set => window.Render += value;
		}

		public Action Load
		{
			set => window.Load += value;
		}

		public Action<Vector> MouseMove
		{
			set
			{
				foreach (var mouse in inputContext.Mice)
				{
					if (mouse.IsConnected)
					{
						mouse.MouseMove += (_, vector2) => value(new Vector(vector2.X * DisplayDensity, vector2.Y * DisplayDensity));
					}
				}
			}
		}

		public PSurface(int width, int height)
		{
			var options = WindowOptions.Default;
			options.Size = new Vector2D<int>(width, height);
			options.Title = "CCreative";
			options.ShouldSwapAutomatically = false;
			options.VSync = false;
			options.FramesPerSecond = 60;
			options.Samples = 1;
			options.PreferredStencilBufferBits = 8;
			options.WindowBorder = WindowBorder.Fixed;
			options.PreferredBitDepth = new Vector4D<int>(8, 8, 8, 8);
			
			window = Window.Create(options);
		}

		public void Close()
		{
			window.Close();
		}

		public void SwapBuffers()
		{
			window.SwapBuffers();
		}

		public void Initialize()
		{
			window.Initialize();
			
			inputContext = window.CreateInput();
		}

		public void Run()
		{
			window.Run();
		}
	}
}
