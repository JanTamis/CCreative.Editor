using System;
using System.Runtime.Versioning;

namespace CCreative
{
	public class PConstants
	{
		/// <summary>
		/// The horizontal location of the mouse on the window
		/// </summary>
		public float MouseX { get; protected set; }

		/// <summary>
		/// The vertical location of the mouse on the window
		/// </summary>
		public float MouseY { get; protected set; }

		/// <summary>
		/// The current location of the mouse on the window
		/// </summary>
		public Vector MousePos => new(MouseX, MouseY);

		/// <summary>
		/// The vertical location of the mouse on the window on the previous frame
		/// </summary>
		public float PmouseX { get; protected set; }

		/// <summary>
		/// The horizontal location of the mouse on the window on the previous frame
		/// </summary>
		public float PmouseY { get; protected set; }

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

		protected float colorModeX = 255, colorModeY = 255, colorModeZ = 255, colorModeA = 255;

		protected ColorModes currentColorMode = ColorModes.RGB;
		protected ShapeTypes currentShapeMode;

		protected DrawTypes currentRectMode = DrawTypes.Corner;
		protected DrawTypes currentEllipseMode = DrawTypes.Center;
		protected DrawTypes currentImageMode = DrawTypes.Corner;

		public const ImageFormats ALPHA = ImageFormats.Alpha;
		public const ImageFormats ARGB = ImageFormats.Argb;
		public const ImageFormats RGB = ImageFormats.Rgb;

		protected bool colorModeScale = true;
		protected bool colorModeDefault = true;
	}

	public enum ColorModes
	{
		RGB,
		HSB,
	}

	public enum MouseButtons
	{
		None = -1,
		Left = 0,
		Right = 1,
		Center = 2,
	}

	[Flags]
	public enum ShapeTypes
	{
		Points = 1,
		LineStrip = 2,
		Lines = 4,
		LineLoop = 8,
		Triangles = 16,
		TriangleFan = 32,
		TriangleStrip = 64,
		Quads = 128,
		QuadStrip = 256,
		Polygon = 512,
		Patches = 1024,
	}

	public enum CloseType
	{
		Close,
		None,
	}

	[Flags]
	public enum FilterTypes
	{
		Threshold = 1,
		Gray = 2,
		Opaque = 4,
		Invert = 8,
		Posterize = 16,
		Blur = 32,
		Erode = 64,
		Dilate = 128,
		Sepia = 256,
		Jitter = 512,
	}

	public enum DrawTypes
	{
		Corner,
		Corners,
		Center,
		Radius
	}

	public enum KeyCodes
	{
		None,
		Up,
		Down,
		Left,
		Right,
		Alt,
		Control,
		Shift,
		Backspace,
		Tab,
		Enter,
		Escape,
		Delete,
	}
	public enum ImageFormats
	{
		Alpha,
		Rgb,
		Argb
	}

	public enum BlendModes
	{
		Replace,
		Blend,
		Add,
		Subtract,
		Darkest,
		Lightest,
		Difference,
		Exclusion,
		Multiply,
		Screen,
		Overlay,
		Hard_Light,
		Soft_Light,
		Dodge,
		Burn,
	}
}
