using System;
using System.IO;
using System.Runtime.InteropServices;
using CCreative.ObjectTK;
using Silk.NET.Core.Contexts;
using SkiaSharp;

namespace CCreative.Rendering;

public class PGraphicsSkiaSharp : PGraphics
{
	private GRGlInterface glInterface;
	private GRContext context;

	private SKSurface surface;

	private SKPaint stroke = new SKPaint()
	{
		Style = SKPaintStyle.Stroke,
		Color = SKColors.Black,
		StrokeWidth = 1,
	};
	
	private SKPaint fill = new SKPaint()
	{
		Style = SKPaintStyle.Fill,
		Color = SKColors.White,
	};

	public PGraphicsSkiaSharp(int width, int height, int pixelDensity, IGLContextSource window)
	{
		using var grGlInterface = GRGlInterface.Create(name => window.GLContext!.TryGetProcAddress(name, out var addr) ? addr : (IntPtr)0);
		grGlInterface.Validate();
		context = GRContext.CreateGl(grGlInterface);
		var renderTarget =
			new GRBackendRenderTarget(height, width, 0, 8, new GRGlFramebufferInfo(0, 0x8058)); // 0x8058 = GL_RGBA8`
		surface = SKSurface.Create(context, renderTarget, GRSurfaceOrigin.BottomLeft, SKColorType.Rgba8888);

		Width = width;
		Height = height;
		PixelDensity = pixelDensity;
	}

	public void Dispose()
	{
		fill.Dispose();
		stroke.Dispose();
		glInterface.Dispose();
		context.Dispose();
	}

	public byte[]? Pixels { get; set; }

	public int Width { get; }

	public int Height { get; }

	public int PixelDensity { get; private set; }

	public void LoadPixels()
	{
		var info = new SKImageInfo(Width, Height, SKColorType.Bgra8888, SKAlphaType.Premul);
		var size = Width * Height * PixelDensity * info.BitsPerPixel;

		if (Pixels is null || Pixels.Length != size)
		{
			Pixels = new byte[size];
		}

		// using var pin = new AutoPinner(Pixels);
		//
		// surface.ReadPixels(info, pin, info.RowBytes, 0, 0);
	}

	public void UpdatePixels(int x, int y, int w, int h)
	{
		using var pinner = new AutoPinner(Pixels);
		
		var bitmap = new SKBitmap(Width, Height, SKColorType.Bgra8888, SKAlphaType.Premul);
		bitmap.SetPixels(pinner);
	}

	public unsafe void UpdatePixels()
	{
		using var pinner = new AutoPinner(Pixels);

		var img = SKImage.FromPixels(new SKImageInfo(Width, Height, SKColorType.Bgra8888, SKAlphaType.Premul), pinner);
		surface.Canvas.DrawImage(img, 0, 0);
	}

	public void Resize(int width, int height)
	{
		
	}

	public Color Get(int x, int y)
	{
		var span = MemoryMarshal.Cast<byte, MemoryColor>(Pixels);

		return span[x * Width + y];
	}

	public PImage Get(int x, int y, int w, int h)
	{
		throw new NotImplementedException();
	}

	public PImage Get()
	{
		throw new NotImplementedException();
	}

	public PImage Copy()
	{
		throw new NotImplementedException();
	}

	public void Set(int x, int y, Color color)
	{
		throw new NotImplementedException();
	}

	public void Set(int x, int y, PImage img)
	{
		throw new NotImplementedException();
	}

	public void Mask(Color[] maskArray)
	{
		throw new NotImplementedException();
	}

	public void Mask(PImage img)
	{
		throw new NotImplementedException();
	}

	public void Filter(FilterTypes kind)
	{
		throw new NotImplementedException();
	}

	public void Filter(FilterTypes type, double param)
	{
		throw new NotImplementedException();
	}

	public void Copy(int sx, int sy, int sw, int sh, int dx, int dy, int dw, int dh)
	{
		throw new NotImplementedException();
	}

	public void Copy(PImage src, int sx, int sy, int sw, int sh, int dx, int dy, int dw, int dh)
	{
		throw new NotImplementedException();
	}

	public Color BlendColor(Color c1, Color c2, BlendModes mode)
	{
		throw new NotImplementedException();
	}

	public void Blend(int sx, int sy, int sw, int sh, int dx, int dy, int dw, int dh, BlendModes mode)
	{
		throw new NotImplementedException();
	}

	public void Blend(PImage src, int sx, int sy, int sw, int sh, int dx, int dy, int dw, int dh, BlendModes mode)
	{
		throw new NotImplementedException();
	}

	public bool TrySave(string filename)
	{
		try
		{
			using var stream = File.OpenWrite(filename);

			surface.Canvas.Flush();

			var snapshot = surface.Snapshot();
			var data = snapshot.Encode(SKEncodedImageFormat.Png, 100);
			data.SaveTo(stream);

			return true;
		}
		catch
		{
			return false;
		}
	}

	public Color Background(Color color)
	{
		if (color is SkiaColor skiaColor)
		{
			surface.Canvas.Clear(skiaColor.skColor);
		}

		return color;
	}

	public void Background(PImage image)
	{
		throw new NotImplementedException();
	}

	public void ColorMode(ColorModes mode, double max1, double max2, double max3)
	{
		throw new NotImplementedException();
	}

	public void ColorMode(ColorModes mode, double max1, double max2, double max3, double maxA)
	{
		throw new NotImplementedException();
	}

	public Color Color(double gray)
	{
		var correctGray = (byte)gray;

		return new SkiaColor(new SKColor(correctGray, correctGray, correctGray, Byte.MaxValue));
	}

	public Color Color(Color color, double alpha)
	{
		var correctAlpha = (byte)alpha;
		return new SkiaColor(correctAlpha, color.R, color.G, color.B);
	}

	public Color Color(double gray, double alpha)
	{
		throw new NotImplementedException();
	}

	public Color Color(double v1, double v2, double v3)
	{
		throw new NotImplementedException();
	}

	public Color Color(double v1, double v2, double v3, double a)
	{
		throw new NotImplementedException();
	}

	public double Alpha(Color color)
	{
		throw new NotImplementedException();
	}

	public double Red(Color color)
	{
		throw new NotImplementedException();
	}

	public double Green(Color color)
	{
		throw new NotImplementedException();
	}

	public double Blue(Color color)
	{
		throw new NotImplementedException();
	}

	public double Hue(Color color)
	{
		throw new NotImplementedException();
	}

	public double Saturation(Color color)
	{
		throw new NotImplementedException();
	}

	public double Brightness(Color color)
	{
		throw new NotImplementedException();
	}

	public Color ContrastColor(Color color)
	{
		throw new NotImplementedException();
	}

	public Color LerpColor(Color c1, Color c2, double amt)
	{
		throw new NotImplementedException();
	}

	public void NoStroke()
	{
		stroke.Color = SKColor.Empty;
	}

	public Color Stroke(Color color)
	{
		throw new NotImplementedException();
	}

	public void NoFill()
	{
		fill.Color = SKColor.Empty;
	}

	public Color Fill(Color color)
	{
		throw new NotImplementedException();
	}

	public void NoTint()
	{
		throw new NotImplementedException();
	}

	public Color Tint(Color color)
	{
		throw new NotImplementedException();
	}

	public void Ambient(Color color)
	{
		throw new NotImplementedException();
	}

	public void AmbientLight(Color color)
	{
		throw new NotImplementedException();
	}

	public void ApplyMatrix(double n00, double n01, double n02, double n10, double n11, double n12)
	{
		throw new NotImplementedException();
	}

	public void ApplyMatrix(double n00, double n01, double n02, double n03, double n10, double n11, double n12,
		double n13,
		double n20, double n21, double n22, double n23, double n30, double n31, double n32, double n33)
	{
		throw new NotImplementedException();
	}

	public void ApplyMatrix(PMatrix matrix)
	{
		throw new NotImplementedException();
	}

	public void ApplyMatrix(PMatrix2D matrix)
	{
		throw new NotImplementedException();
	}

	public void ApplyMatrix(PMatrix3D matrix)
	{
		throw new NotImplementedException();
	}

	public void Arc(double a, double b, double c, double d, double start, double stop)
	{
		throw new NotImplementedException();
	}

	public void BeginCamera()
	{
		throw new NotImplementedException();
	}

	public void BeginContour()
	{
		throw new NotImplementedException();
	}

	public void BeginDraw()
	{
		
	}

	public void BeginShape()
	{
		throw new NotImplementedException();
	}

	public void BeginShape(ShapeTypes type)
	{
		throw new NotImplementedException();
	}

	public void Bezier(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4)
	{
		throw new NotImplementedException();
	}

	public void Bezier(double x1, double y1, double z1, double x2, double y2, double z2, double x3, double y3, double z3,
		double x4, double y4, double z4)
	{
		throw new NotImplementedException();
	}

	public void BezierDetail(int detail)
	{
		throw new NotImplementedException();
	}

	public double BezierPoint(double a, double b, double c, double d, double t)
	{
		throw new NotImplementedException();
	}

	public double BezierTangent(double a, double b, double c, double d, double t)
	{
		throw new NotImplementedException();
	}

	public void BezierVertex(double x2, double y2, double x3, double y3, double x4, double y4)
	{
		throw new NotImplementedException();
	}

	public void BezierVertex(double x2, double y2, double z2, double x3, double y3, double z3, double x4, double y4,
		double z4)
	{
		throw new NotImplementedException();
	}

	public void BlendMode(int mode)
	{
		throw new NotImplementedException();
	}

	public void Box(double w, double h, double d)
	{
		throw new NotImplementedException();
	}

	public void Camera()
	{
		throw new NotImplementedException();
	}

	public void Camera(double eyeX, double eyeY, double eyeZ, double centerX, double centerY, double centerZ, double upX,
		double upY, double upZ)
	{
		throw new NotImplementedException();
	}

	public void Circle(double x, double y, double extent)
	{
		surface.Canvas.DrawCircle((float)x, (float)y, (float)extent, fill);
		surface.Canvas.DrawCircle((float)x, (float)y, (float)extent, stroke);
	}

	public void Clear()
	{
		surface.Canvas.Clear();
	}

	public void Clip(int a, int b, int c, int d)
	{
		throw new NotImplementedException();
	}

	public PImage CreateImage(int width, int height)
	{
		throw new NotImplementedException();
	}

	public PImage LoadImage(string path)
	{
		throw new NotImplementedException();
	}

	public void Curve(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4)
	{
		throw new NotImplementedException();
	}

	public void Curve(double x1, double y1, double z1, double x2, double y2, double z2, double x3, double y3, double z3,
		double x4,
		double y4, double z4)
	{
		throw new NotImplementedException();
	}

	public void CurveDetail(int detail)
	{
		throw new NotImplementedException();
	}

	public void CurvePoint(double a, double b, double c, double d, double t)
	{
		throw new NotImplementedException();
	}

	public void CurveTangent(double a, double b, double c, double d, double t)
	{
		throw new NotImplementedException();
	}

	public void CurveTightness(double tightness)
	{
		throw new NotImplementedException();
	}

	public void CurveVertex(double x, double y)
	{
		throw new NotImplementedException();
	}

	public void CurveVertex(double x, double y, double z)
	{
		throw new NotImplementedException();
	}

	public void DirectionalLight(double v1, double v2, double v3, double nx, double ny, double nz)
	{
		throw new NotImplementedException();
	}

	public bool Displayable()
	{
		throw new NotImplementedException();
	}

	public void Edge(bool edge)
	{
		throw new NotImplementedException();
	}

	public void Ellipse(double a, double b, double c, double d)
	{
		throw new NotImplementedException();
	}

	public void EllipseMode(int mode)
	{
		throw new NotImplementedException();
	}

	public void Emissive(double gray)
	{
		throw new NotImplementedException();
	}

	public void Emissive(double v1, double v2, double v3)
	{
		throw new NotImplementedException();
	}

	public void Emissive(Color color)
	{
		throw new NotImplementedException();
	}

	public void EndCamera()
	{
		throw new NotImplementedException();
	}

	public void EndContour()
	{
		throw new NotImplementedException();
	}

	public void EndDraw()
	{
		surface.Flush();
	}

	public void EndShape(CloseType close)
	{
		throw new NotImplementedException();
	}

	public void Flush()
	{
		throw new NotImplementedException();
	}

	public void Frustum(double left, double right, double bottom, double top, double near, double far)
	{
		throw new NotImplementedException();
	}

	public PMatrix GetMatrix()
	{
		throw new NotImplementedException();
	}

	public PMatrix2D GetMatrix(PMatrix2D target)
	{
		throw new NotImplementedException();
	}

	public PMatrix3D GetMatrix(PMatrix3D target)
	{
		throw new NotImplementedException();
	}

	public PStyle GetStyle()
	{
		throw new NotImplementedException();
	}

	public PStyle GetStyle(PStyle target)
	{
		throw new NotImplementedException();
	}

	public void Hint(int which)
	{
		throw new NotImplementedException();
	}

	public void Image(PImage img, double a, double b)
	{
		throw new NotImplementedException();
	}

	public void Image(PImage img, double a, double b, double c, double d)
	{
		throw new NotImplementedException();
	}

	public void Image(PImage img, double a, double b, double c, double d, int u1, int v1, int u2, int v2)
	{
		throw new NotImplementedException();
	}

	public void ImageMode(int mode)
	{
		throw new NotImplementedException();
	}

	public bool Is2D()
	{
		throw new NotImplementedException();
	}

	public bool Is3D()
	{
		throw new NotImplementedException();
	}

	public bool IsGL()
	{
		throw new NotImplementedException();
	}

	public void LightFalloff(double constant, double linear, double quadratic)
	{
		throw new NotImplementedException();
	}

	public void Lights()
	{
		throw new NotImplementedException();
	}

	public void LightSpecular(double v1, double v2, double v3)
	{
		throw new NotImplementedException();
	}

	public void Line(double x1, double y1, double x2, double y2)
	{
		surface.Canvas.DrawLine((float)x1, (float)y1, (float)x2, (float)y2, stroke);
	}

	public void Line(double x1, double y1, double z1, double x2, double y2, double z2)
	{
		throw new NotImplementedException();
	}

	public double ModelX(double x, double y, double z)
	{
		throw new NotImplementedException();
	}

	public double ModelY(double x, double y, double z)
	{
		throw new NotImplementedException();
	}

	public double ModelZ(double x, double y, double z)
	{
		throw new NotImplementedException();
	}

	public void NoClip()
	{
		throw new NotImplementedException();
	}

	public void NoLights()
	{
		throw new NotImplementedException();
	}

	public void Normal(double nx, double ny, double nz)
	{
		throw new NotImplementedException();
	}

	public void NoSmooth()
	{
		throw new NotImplementedException();
	}

	public void NoTexture()
	{
		throw new NotImplementedException();
	}

	public void Ortho()
	{
		throw new NotImplementedException();
	}

	public void Ortho(double left, double right, double bottom, double top)
	{
		throw new NotImplementedException();
	}

	public void Ortho(double left, double right, double bottom, double top, double near, double far)
	{
		throw new NotImplementedException();
	}

	public void Perspective()
	{
		throw new NotImplementedException();
	}

	public void Perspective(double fovY, double aspect, double zNear, double zFar)
	{
		throw new NotImplementedException();
	}

	public void Point(double x, double y)
	{
		throw new NotImplementedException();
	}

	public void Point(double x, double y, double z)
	{
		throw new NotImplementedException();
	}

	public void PointLight(double v1, double v2, double v3, double x, double y, double z)
	{
		throw new NotImplementedException();
	}

	public void Pop()
	{
		throw new NotImplementedException();
	}

	public void PopMatrix()
	{
		throw new NotImplementedException();
	}

	public void PopStyle()
	{
		throw new NotImplementedException();
	}

	public void PrintCamera()
	{
		throw new NotImplementedException();
	}

	public void PrintMatrix()
	{
		throw new NotImplementedException();
	}

	public void PrintProjection()
	{
		throw new NotImplementedException();
	}

	public void Push()
	{
		throw new NotImplementedException();
	}

	public void PushMatrix()
	{
		throw new NotImplementedException();
	}

	public void PushStyle()
	{
		throw new NotImplementedException();
	}

	public void Quad(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4)
	{
		throw new NotImplementedException();
	}

	public void QuadraticVertex(double cx, double cy, double x3, double y3)
	{
		throw new NotImplementedException();
	}

	public void QuadraticVertex(double cx, double cy, double cz, double x3, double y3, double z3)
	{
		throw new NotImplementedException();
	}

	public void Rect(double a, double b, double c, double d)
	{
		throw new NotImplementedException();
	}

	public void Rect(double a, double b, double c, double d, double r)
	{
		throw new NotImplementedException();
	}

	public void Rect(double a, double b, double c, double d, double tl, double tr, double br, double bl)
	{
		throw new NotImplementedException();
	}

	public void RectMode(int mode)
	{
		throw new NotImplementedException();
	}

	public void ResetMatrix()
	{
		throw new NotImplementedException();
	}

	public void ResetShader()
	{
		throw new NotImplementedException();
	}

	public void ResetShader(int kind)
	{
		throw new NotImplementedException();
	}

	public void Rotate(double angle, double x, double y, double z)
	{
		throw new NotImplementedException();
	}

	public void RotateX(double angle)
	{
		throw new NotImplementedException();
	}

	public void RotateY(double angle)
	{
		throw new NotImplementedException();
	}

	public void RotateZ(double angle)
	{
		throw new NotImplementedException();
	}

	public void Scale(double s)
	{
		throw new NotImplementedException();
	}

	public void Scale(double x, double y)
	{
		throw new NotImplementedException();
	}

	public void Scale(double x, double y, double z)
	{
		throw new NotImplementedException();
	}

	public double ScreenX(double x, double y)
	{
		throw new NotImplementedException();
	}

	public double ScreenX(double x, double y, double z)
	{
		throw new NotImplementedException();
	}

	public double ScreenY(double x, double y)
	{
		throw new NotImplementedException();
	}

	public double ScreenY(double x, double y, double z)
	{
		throw new NotImplementedException();
	}

	public double ScreenZ(double x, double y)
	{
		throw new NotImplementedException();
	}

	public double ScreenZ(double x, double y, double z)
	{
		throw new NotImplementedException();
	}

	public void SetMatrix(PMatrix source)
	{
		throw new NotImplementedException();
	}

	public void SetMatrix(PMatrix2D source)
	{
		throw new NotImplementedException();
	}

	public void SetMatrix(PMatrix3D source)
	{
		throw new NotImplementedException();
	}

	public void SetParent(PApplet parent)
	{
		throw new NotImplementedException();
	}

	public void SetPath(string path)
	{
		throw new NotImplementedException();
	}

	public void SetPrimary(bool primary)
	{
		throw new NotImplementedException();
	}

	public void SetSize(int w, int h)
	{
		throw new NotImplementedException();
	}

	public void ShapeMode(int kind)
	{
		throw new NotImplementedException();
	}

	public void ShearX(double angle)
	{
		throw new NotImplementedException();
	}

	public void ShearY(double angle)
	{
		throw new NotImplementedException();
	}

	public void Shininess(double shine)
	{
		throw new NotImplementedException();
	}

	public void Smooth()
	{
		throw new NotImplementedException();
	}

	public void Smooth(int quality)
	{
		throw new NotImplementedException();
	}

	public void Specular(double gray)
	{
		throw new NotImplementedException();
	}

	public void Specular(double v1, double v2, double v3)
	{
		throw new NotImplementedException();
	}

	public void Specular(Color color)
	{
		throw new NotImplementedException();
	}

	public void Sphere(double r)
	{
		throw new NotImplementedException();
	}

	public void SphereDetail(int res)
	{
		throw new NotImplementedException();
	}

	public void SphereDetail(int ures, int vres)
	{
		throw new NotImplementedException();
	}

	public void SpotLight(double v1, double v2, double v3, double x, double y, double z, double nx, double ny, double nz,
		double angle, double concentration)
	{
		throw new NotImplementedException();
	}

	public void Square(double x, double y, double extent)
	{
		throw new NotImplementedException();
	}

	public void StrokeCap(int cap)
	{
		throw new NotImplementedException();
	}

	public void StrokeJoin(int @join)
	{
		throw new NotImplementedException();
	}

	public void StrokeWeight(double weight)
	{
		throw new NotImplementedException();
	}

	public void Style(PStyle style)
	{
		throw new NotImplementedException();
	}
	
	public void Text(string text, Index start, Index end, double x, double y)
	{
		throw new NotImplementedException();
	}

	public void Text(string text, Index start, Index end, double x, double y, double z)
	{
		throw new NotImplementedException();
	}

	public void Text(string text, Range range, double x, double y)
	{
		throw new NotImplementedException();
	}

	public void Text(string text, Range range, double x, double y, double z)
	{
		throw new NotImplementedException();
	}

	public void Text(char[] chars, Index start, Index end, double x, double y)
	{
		throw new NotImplementedException();
	}

	public void Text(char[] chars, Index start, Index end, double x, double y, double z)
	{
		throw new NotImplementedException();
	}

	public void Text(char[] chars, Range range, double x, double y)
	{
		throw new NotImplementedException();
	}

	public void Text(char[] chars, Range range, double x, double y, double z)
	{
		throw new NotImplementedException();
	}

	public void Text<T>(T num, double x, double y) where T : IConvertible, IFormattable
	{
		throw new NotImplementedException();
	}

	public void Text<T>(T num, double x, double y, double z) where T : IConvertible, IFormattable
	{
		throw new NotImplementedException();
	}

	public void Text(string text, double x1, double y1, double x2, double y2)
	{
		throw new NotImplementedException();
	}

	public void TextAlign(int alignX)
	{
		throw new NotImplementedException();
	}

	public void TextAlign(int alignX, int alignY)
	{
		throw new NotImplementedException();
	}

	public short TextAcent()
	{
		throw new NotImplementedException();
	}

	public short TextDecent()
	{
		throw new NotImplementedException();
	}

	public void TextFont(PFont which)
	{
		throw new NotImplementedException();
	}

	public void TextFont(PFont which, int size)
	{
		throw new NotImplementedException();
	}

	public void TextLeading(double leading)
	{
		throw new NotImplementedException();
	}

	public void TextMode(int mode)
	{
		throw new NotImplementedException();
	}

	public void TextSize(double size)
	{
		throw new NotImplementedException();
	}

	public void Texture(PImage texture)
	{
		throw new NotImplementedException();
	}

	public void TextureMode(int mode)
	{
		throw new NotImplementedException();
	}

	public void TextureWrap(int wrap)
	{
		throw new NotImplementedException();
	}

	public double TextWidth(char c)
	{
		throw new NotImplementedException();
	}

	public double TextWidth(char[] chars, int start, int length)
	{
		throw new NotImplementedException();
	}

	public double TextWidth(string text)
	{
		throw new NotImplementedException();
	}

	public void Translate(double x, double y)
	{
		throw new NotImplementedException();
	}

	public void Translate(double x, double y, double z)
	{
		throw new NotImplementedException();
	}

	public void Triangle(double x1, double y1, double x2, double y2, double x3, double y3)
	{
		throw new NotImplementedException();
	}

	public void Vertex(double x, double y)
	{
		throw new NotImplementedException();
	}

	public void Vertex(double x, double y, double z)
	{
		throw new NotImplementedException();
	}

	public void Vertex(double x, double y, double z, double u, double v)
	{
		throw new NotImplementedException();
	}

	public void Vertex(double[] v)
	{
		throw new NotImplementedException();
	}
}