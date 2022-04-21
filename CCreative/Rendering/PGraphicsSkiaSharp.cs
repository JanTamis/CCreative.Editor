using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using CCreative.Helpers;
using Silk.NET.Core.Contexts;
using Silk.NET.OpenGL;
using SkiaSharp;

// ReSharper disable ConvertToUsingDeclaration
#pragma warning disable CS1591

namespace CCreative.Rendering;

[RequiresPreviewFeatures]
public class PGraphicsSkiaSharp : PGraphics
{
	private GRGlInterface glInterface;
	private GRContext context;

	private SKSurface surface;
	private GL gl;
	private SKFont font;

	private SKImageInfo defaultImageInfo => new(Width, Height, SKColorType.Bgra8888, SKAlphaType.Premul);

	private readonly SKPaint stroke = new()
	{
		Style = SKPaintStyle.Stroke,
		Color = SKColors.Black,
		StrokeWidth = 1,
		TextSize = 100,
		IsAntialias = true,
	};

	private readonly SKPaint fill = new()
	{
		Style = SKPaintStyle.Fill,
		Color = SKColors.White,
		TextSize = 100,
		IsAntialias = true,
	};

	public PGraphicsSkiaSharp(int width, int height, int pixelDensity, IGLContextSource window)
	{
		using var grGlInterface =
			GRGlInterface.Create(name => window.GLContext!.TryGetProcAddress(name, out var addr) ? addr : (IntPtr)0);
		grGlInterface.Validate();

		context = GRContext.CreateGl(grGlInterface);
		var renderTarget =
			new GRBackendRenderTarget(height, width, 0, 8, new GRGlFramebufferInfo(0, 0x8058)); // 0x8058 = GL_RGBA8`
		surface = SKSurface.Create(context, renderTarget, GRSurfaceOrigin.BottomLeft, SKColorType.Rgba8888);

		gl = window.CreateOpenGL();

		Width = width;
		Height = height;
		PixelDensity = pixelDensity;
		Pixels = new Color[width * height * pixelDensity];
		font = stroke.ToFont();
	}

	public void Dispose()
	{
		fill.Dispose();
		stroke.Dispose();
		glInterface.Dispose();
		context.Dispose();
	}

	public Color[]? Pixels { get; set; }

	public int Width { get; }

	public int Height { get; }

	public int PixelDensity { get; private set; }

	public void LoadPixels()
	{
		var info = defaultImageInfo;
		var size = Width * Height * PixelDensity;

		if (Pixels is null || Pixels.Length != size)
		{
			Pixels = GC.AllocateUninitializedArray<Color>(size);
		}

		using var pin = new AutoPinner(Pixels);

		surface.ReadPixels(info, pin, info.RowBytes, 0, 0);
	}

	public void UpdatePixels(int x, int y, int w, int h)
	{
		using var pinner = new AutoPinner(Pixels);

		var bitmap = new SKBitmap(defaultImageInfo);
		bitmap.SetPixels(pinner);
	}

	public void UpdatePixels()
	{
		using var pinner = new AutoPinner(Pixels);
		using var img = SKImage.FromPixels(defaultImageInfo, pinner);

		surface.Canvas.DrawImage(img, 0, 0);
	}

	public void Resize(int width, int height)
	{
	}

	public Color Get(int x, int y)
	{
		var pixelSpan = surface.PeekPixels().GetPixelSpan();
		var span = MemoryMarshal.Cast<byte, Color>(pixelSpan);

		return span[x * Width + y];
	}

	public Image Get(int x, int y, int w, int h)
	{
		throw new NotImplementedException();
	}

	public Image Get()
	{
		return new SkiaImage(surface.Snapshot());
	}

	public Image Copy()
	{
		return new SkiaImage(surface.Snapshot());
	}

	public void Set(int x, int y, Color color)
	{
		if (Pixels is not null)
		{
			var index = x * Width + y;

			Pixels[index] = color;
		}
	}

	public void Set(int x, int y, Image img)
	{
		Image(img, x, y);
	}

	public void Mask(Color[] maskArray)
	{
		throw new NotImplementedException();
	}

	public void Mask(Image img)
	{
		throw new NotImplementedException();
	}

	public void Filter(FilterTypes kind)
	{
		using var paint = new SKPaint();

		switch (kind)
		{
			case FilterTypes.Blur:
				paint.ImageFilter = SKImageFilter.CreateBlur(1, 1, SKShaderTileMode.Clamp);
				break;
			case FilterTypes.Invert:
				paint.ColorFilter = SKColorFilter.CreateColorMatrix(new[]
				{
					-1.0f, 0.0f, 0.0f, 1.0f, 0.0f,
					0.0f, -1.0f, 0.0f, 1.0f, 0.0f,
					0.0f, 0.0f, -1.0f, 1.0f, 0.0f,
					1.0f, 1.0f, 1.0f, 1.0f, 0.0f,
				});
				break;
			case FilterTypes.Gray:
				paint.ColorFilter = SKColorFilter.CreateColorMatrix(new[]
				{
					0.21f, 0.72f, 0.07f, 0, 0,
					0.21f, 0.72f, 0.07f, 0, 0,
					0.21f, 0.72f, 0.07f, 0, 0,
					0, 0, 0, 1, 0,
				});
				break;
			case FilterTypes.Threshold:
				break;
			case FilterTypes.Opaque:
				break;
			case FilterTypes.Posterize:
				break;
			case FilterTypes.Erode:
				break;
			case FilterTypes.Dilate:
				break;
			case FilterTypes.Sepia:
				break;
			case FilterTypes.Jitter:
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
		}

		surface.Canvas.DrawSurface(surface, 0, 0, paint);
	}

	public void Filter(FilterTypes kind, float param)
	{
		using var paint = new SKPaint();

		switch (kind)
		{
			case FilterTypes.Blur:
				paint.ImageFilter = SKImageFilter.CreateBlur(param, param, SKShaderTileMode.Clamp);
				break;
			case FilterTypes.Threshold:
				break;
			case FilterTypes.Opaque:
				break;
			case FilterTypes.Posterize:
				break;
			case FilterTypes.Erode:
				break;
			case FilterTypes.Dilate:
				break;
			case FilterTypes.Sepia:
				break;
			case FilterTypes.Jitter:
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
		}

		surface.Canvas.DrawSurface(surface, 0, 0, paint);
	}

	public void Copy(int sx, int sy, int sw, int sh, int dx, int dy, int dw, int dh)
	{
		var img = surface.Snapshot(new SKRectI(sx, sy, sw, sh));

		surface.Canvas.DrawImage(img, new SKRect(dx, dy, dw, dh));
	}

	public void Copy(Image src, int sx, int sy, int sw, int sh, int dx, int dy, int dw, int dh)
	{
		if (src is SkiaImage skiaImage)
		{
			surface.Canvas.DrawImage(skiaImage.skImage, new SKRect(sx, sy, sw, sh), new SKRect(dx, dy, dw, dh));
		}
	}

	public Color BlendColor(Color c1, Color c2, BlendModes mode)
	{
		throw new NotImplementedException();
	}

	public void Blend(int sx, int sy, int sw, int sh, int dx, int dy, int dw, int dh, BlendModes mode)
	{
		throw new NotImplementedException();
	}

	public void Blend(Image src, int sx, int sy, int sw, int sh, int dx, int dy, int dw, int dh, BlendModes mode)
	{
		throw new NotImplementedException();
	}

	public bool TrySave(string filename)
	{
		try
		{
			using var stream = File.OpenWrite(filename);

			surface.Canvas.Flush();

			using var snapshot = surface.Snapshot();
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
		surface.Canvas.Clear(new SKColor((uint)color.GetHashCode()));

		return color;
	}

	public void ColorMode(ColorModes mode, float max1, float max2, float max3)
	{
		throw new NotImplementedException();
	}

	public void ColorMode(ColorModes mode, float max1, float max2, float max3, float maxA)
	{
		throw new NotImplementedException();
	}

	public Color Color(float gray)
	{
		var correctGray = (byte)gray;

		return new Color()
		{
			A = Byte.MaxValue,
			R = correctGray,
			G = correctGray,
			B = correctGray,
		};
	}

	public Color Color(Color color, float alpha)
	{
		return color with { A = (byte)alpha };
	}

	public Color Color(float gray, float alpha)
	{
		throw new NotImplementedException();
	}

	public Color Color(float v1, float v2, float v3)
	{
		throw new NotImplementedException();
	}

	public Color Color(float v1, float v2, float v3, float a)
	{
		throw new NotImplementedException();
	}

	public void NoStroke()
	{
		stroke.Color = SKColor.Empty;
	}

	public Color Stroke(Color color)
	{
		stroke.Color = Unsafe.As<Color, SKColor>(ref color);

		return color;
	}

	public void NoFill()
	{
		fill.Color = SKColor.Empty;
	}

	public Color Fill(Color color)
	{
		fill.Color = Unsafe.As<Color, SKColor>(ref color);

		return color;
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

	public void ApplyMatrix(float n00, float n01, float n02, float n10, float n11, float n12)
	{
		throw new NotImplementedException();
	}

	public void ApplyMatrix(float n00, float n01, float n02, float n03, float n10, float n11, float n12,
		float n13,
		float n20, float n21, float n22, float n23, float n30, float n31, float n32, float n33)
	{
		throw new NotImplementedException();
	}

	public void ApplyMatrix(Matrix matrix)
	{
		throw new NotImplementedException();
	}

	public void ApplyMatrix(Matrix2D matrix)
	{
		surface.Canvas.SetMatrix(new SKMatrix(matrix.Get(null)));
	}

	public void ApplyMatrix(Matrix3D matrix)
	{
		surface.Canvas.SetMatrix(new SKMatrix(matrix.Get(null)));
	}

	public void Arc(float a, float b, float c, float d, float start, float stop)
	{
		surface.Canvas.DrawArc(new SKRect(a, b, c, d), start, stop, true, fill);
		surface.Canvas.DrawArc(new SKRect(a, b, c, d), start, stop, true, stroke);
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

	public void Bezier(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4)
	{
		using (var path = new SKPath())
		{
			path.MoveTo(x1, y1);
			path.CubicTo(x2, y2, x3, y3, x4, y4);

			surface.Canvas.DrawPath(path, fill);
			surface.Canvas.DrawPath(path, stroke);
		}
	}

	public void Bezier(float x1, float y1, float z1, float x2, float y2, float z2, float x3, float y3, float z3,
		float x4, float y4, float z4)
	{
		throw new NotSupportedException("this method is only supported of the 3D renderer");
	}

	public void BezierDetail(int detail)
	{
		throw new NotImplementedException();
	}

	public float BezierPoint(float a, float b, float c, float d, float t)
	{
		throw new NotImplementedException();
	}

	public float BezierTangent(float a, float b, float c, float d, float t)
	{
		throw new NotImplementedException();
	}

	public void BezierVertex(float x2, float y2, float x3, float y3, float x4, float y4)
	{
		throw new NotImplementedException();
	}

	public void BezierVertex(float x2, float y2, float z2, float x3, float y3, float z3, float x4, float y4,
		float z4)
	{
		throw new NotImplementedException();
	}

	public void BlendMode(int mode)
	{
		throw new NotImplementedException();
	}

	public void Box(float w, float h, float d)
	{
		throw new NotImplementedException();
	}

	public void Camera()
	{
		throw new NotImplementedException();
	}

	public void Camera(float eyeX, float eyeY, float eyeZ, float centerX, float centerY, float centerZ, float upX,
		float upY, float upZ)
	{
		throw new NotImplementedException();
	}

	public void Circle(float x, float y, float extent)
	{
		surface.Canvas.DrawCircle(x, y, extent, fill);
		surface.Canvas.DrawCircle(x, y, extent, stroke);
	}

	public void Clear()
	{
		surface.Canvas.Clear();
	}

	public void Clip(int a, int b, int c, int d)
	{
		surface.Canvas.ClipRect(new SKRect(a, b, c, d));
	}

	public Image CreateImage(int width, int height)
	{
		return new SkiaImage(SKImage.Create(new SKImageInfo(width * PixelDensity, height * PixelDensity,
			SKColorType.Bgra8888, SKAlphaType.Premul)));
	}

	public Image LoadImage(string path)
	{
		using var stream = File.OpenRead(path);

		return new SkiaImage(SKImage.FromEncodedData(stream));
	}

	public void Curve(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4)
	{
		throw new NotImplementedException();
	}

	public void Curve(float x1, float y1, float z1, float x2, float y2, float z2, float x3, float y3, float z3,
		float x4,
		float y4, float z4)
	{
		throw new NotImplementedException();
	}

	public void CurveDetail(int detail)
	{
		throw new NotImplementedException();
	}

	public void CurvePoint(float a, float b, float c, float d, float t)
	{
		throw new NotImplementedException();
	}

	public void CurveTangent(float a, float b, float c, float d, float t)
	{
		throw new NotImplementedException();
	}

	public void CurveTightness(float tightness)
	{
		throw new NotImplementedException();
	}

	public void CurveVertex(float x, float y)
	{
		throw new NotImplementedException();
	}

	public void CurveVertex(float x, float y, float z)
	{
		throw new NotImplementedException();
	}

	public void DirectionalLight(float v1, float v2, float v3, float nx, float ny, float nz)
	{
		throw new NotImplementedException();
	}

	public bool Displayable()
	{
		return true;
	}

	public void Edge(bool edge)
	{
		throw new NotImplementedException();
	}

	public void Ellipse(float a, float b, float c, float d)
	{
		surface.Canvas.DrawOval(a, b, c, d, fill);
		surface.Canvas.DrawOval(a, b, c, d, stroke);
	}

	public void EllipseMode(int mode)
	{
		throw new NotImplementedException();
	}

	public void Emissive(float gray)
	{
		throw new NotImplementedException();
	}

	public void Emissive(float v1, float v2, float v3)
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
		surface.Flush();
	}

	public void Frustum(float left, float right, float bottom, float top, float near, float far)
	{
		throw new NotImplementedException();
	}

	public Matrix GetMatrix()
	{
		throw new NotImplementedException();
	}

	public Matrix2D GetMatrix(Matrix2D target)
	{
		throw new NotImplementedException();
	}

	public Matrix3D GetMatrix(Matrix3D target)
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

	public void Image(Image img, float a, float b)
	{
		if (img is SkiaImage skImage)
		{
			surface.Canvas.DrawImage(skImage.skImage, a, b);
		}
	}

	public void Image(Image img, float a, float b, float c, float d)
	{
		if (img is SkiaImage skImage)
		{
			surface.Canvas.DrawImage(skImage.skImage, new SKRect(0, 0, img.Width, img.Height), new SKRect(a, b, c, d));
		}
	}

	public void Image(Image img, float a, float b, float c, float d, int u1, int v1, int u2, int v2)
	{
		throw new NotImplementedException();
	}

	public void ImageMode(int mode)
	{
		throw new NotImplementedException();
	}

	public bool Is2D()
	{
		return true;
	}

	public bool Is3D()
	{
		return false;
	}

	public bool IsGL()
	{
		return true;
	}

	public void LightFalloff(float constant, float linear, float quadratic)
	{
		throw new NotImplementedException();
	}

	public void Lights()
	{
		throw new NotImplementedException();
	}

	public void LightSpecular(float v1, float v2, float v3)
	{
		throw new NotImplementedException();
	}

	public void Line(float x1, float y1, float x2, float y2)
	{
		surface.Canvas.DrawLine(x1, y1, x2, y2, stroke);
	}

	public void Line(float x1, float y1, float z1, float x2, float y2, float z2)
	{
		throw new NotImplementedException();
	}

	public float ModelX(float x, float y, float z)
	{
		throw new NotImplementedException();
	}

	public float ModelY(float x, float y, float z)
	{
		throw new NotImplementedException();
	}

	public float ModelZ(float x, float y, float z)
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

	public void Normal(float nx, float ny, float nz)
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

	public void Ortho(float left, float right, float bottom, float top)
	{
		throw new NotImplementedException();
	}

	public void Ortho(float left, float right, float bottom, float top, float near, float far)
	{
		throw new NotImplementedException();
	}

	public void Perspective()
	{
		throw new NotImplementedException();
	}

	public void Perspective(float fovY, float aspect, float zNear, float zFar)
	{
		throw new NotImplementedException();
	}

	public void Point(float x, float y)
	{
		surface.Canvas.DrawPoint(x, y, fill);
	}

	public void Point(float x, float y, float z)
	{
		throw new NotImplementedException();
	}

	public void PointLight(float v1, float v2, float v3, float x, float y, float z)
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

	public void QuadraticVertex(float cx, float cy, float x3, float y3)
	{
		throw new NotImplementedException();
	}

	public void QuadraticVertex(float cx, float cy, float cz, float x3, float y3, float z3)
	{
		throw new NotImplementedException();
	}

	public void Rect(float a, float b, float c, float d)
	{
		throw new NotImplementedException();
	}

	public void Rect(float a, float b, float c, float d, float r)
	{
		throw new NotImplementedException();
	}

	public void Rect(float a, float b, float c, float d, float tl, float tr, float br, float bl)
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

	public void Rotate(float angle, float x, float y, float z)
	{
		throw new NotImplementedException();
	}

	public void RotateX(float angle)
	{
		throw new NotImplementedException();
	}

	public void RotateY(float angle)
	{
		throw new NotImplementedException();
	}

	public void RotateZ(float angle)
	{
		throw new NotImplementedException();
	}

	public void Scale(float s)
	{
		throw new NotImplementedException();
	}

	public void Scale(float x, float y)
	{
		throw new NotImplementedException();
	}

	public void Scale(float x, float y, float z)
	{
		throw new NotImplementedException();
	}

	public float ScreenX(float x, float y)
	{
		throw new NotImplementedException();
	}

	public float ScreenX(float x, float y, float z)
	{
		throw new NotImplementedException();
	}

	public float ScreenY(float x, float y)
	{
		throw new NotImplementedException();
	}

	public float ScreenY(float x, float y, float z)
	{
		throw new NotImplementedException();
	}

	public float ScreenZ(float x, float y)
	{
		throw new NotImplementedException();
	}

	public float ScreenZ(float x, float y, float z)
	{
		throw new NotImplementedException();
	}

	public void SetMatrix(Matrix source)
	{
		throw new NotImplementedException();
	}

	public void SetMatrix(Matrix2D source)
	{
		throw new NotImplementedException();
	}

	public void SetMatrix(Matrix3D source)
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

	public void ShearX(float angle)
	{
		throw new NotImplementedException();
	}

	public void ShearY(float angle)
	{
		throw new NotImplementedException();
	}

	public void Shininess(float shine)
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

	public void Specular(float gray)
	{
		throw new NotImplementedException();
	}

	public void Specular(float v1, float v2, float v3)
	{
		throw new NotImplementedException();
	}

	public void Specular(Color color)
	{
		throw new NotImplementedException();
	}

	public void Sphere(float r)
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

	public void SpotLight(float v1, float v2, float v3, float x, float y, float z, float nx, float ny, float nz,
		float angle, float concentration)
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

	public void StrokeWeight(float weight)
	{
		stroke.StrokeWidth = weight;
	}

	public void Style(PStyle style)
	{
		throw new NotImplementedException();
	}

	public void Text(string text, Range range, float x, float y)
	{
		throw new NotImplementedException();
	}

	public void Text(string text, Range range, float x, float y, float z)
	{
		throw new NotImplementedException();
	}

	public void Text(char[] chars, Range range, float x, float y)
	{
		throw new NotImplementedException();
	}

	public void Text(char[] chars, Range range, float x, float y, float z)
	{
		throw new NotImplementedException();
	}

	public void Text<T>(T num, float x, float y)
	{
		string? text = null;

		if (num is string s)
		{
			text = s;
		}
		else if (num is not null)
		{
			text = num.ToString();
		}

		if (!String.IsNullOrWhiteSpace(text))
		{
			Text(text, x, y, font, fill);
			Text(text, x, y, font, stroke);
		}
	}

	public void Text<T>(T num, float x, float y, float z)
	{
		throw new NotImplementedException();
	}

	public void Text(string text, float x1, float y1, float x2, float y2)
	{
		throw new NotImplementedException();
	}

	private void Text(ReadOnlySpan<char> text, float x, float y, SKFont textFont, SKPaint paint)
	{
		if (paint.TextAlign != SKTextAlign.Left)
		{
			var num = textFont.MeasureText(MemoryMarshal.Cast<char, ushort>(text));

			if (paint.TextAlign == SKTextAlign.Center)
			{
				num *= 0.5f;
			}

			x -= num;
		}

		y += textFont.Size;

		using (var textBlob = SKTextBlob.Create(text, textFont))
		{
			surface.Canvas.DrawText(textBlob, x, y, paint);
		}
	}

	public void TextAlign(int alignX)
	{
		throw new NotImplementedException();
	}

	public void TextAlign(int alignX, int alignY)
	{
		throw new NotImplementedException();
	}

	public float TextAcent()
	{
		font.GetFontMetrics(out var metrics);

		return metrics.Ascent;
	}

	public float TextDecent()
	{
		font.GetFontMetrics(out var metrics);

		return metrics.Descent;
	}

	public void TextFont(PFont which)
	{
		throw new NotImplementedException();
	}

	public void TextFont(PFont which, int size)
	{
		throw new NotImplementedException();
	}

	public void TextLeading(float leading)
	{
		throw new NotImplementedException();
	}

	public void TextMode(int mode)
	{
		throw new NotImplementedException();
	}

	public void TextSize(float size)
	{
		font.Size = size;
	}

	public void Texture(Image texture)
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

	public float TextWidth(char c)
	{
		throw new NotImplementedException();
	}

	public float TextWidth(char[] chars, int start, int length)
	{
		throw new NotImplementedException();
	}

	public float TextWidth(string text)
	{
		throw new NotImplementedException();
	}

	public void Translate(float x, float y)
	{
		throw new NotImplementedException();
	}

	public void Translate(float x, float y, float z)
	{
		throw new NotImplementedException();
	}

	public void Vertex(float x, float y)
	{
		throw new NotImplementedException();
	}

	public void Vertex(float x, float y, float z)
	{
		throw new NotImplementedException();
	}

	public void Vertex(float x, float y, float z, float u, float v)
	{
		throw new NotImplementedException();
	}

	public void Vertex(float[] v)
	{
		throw new NotImplementedException();
	}

	public void DrawShape(Span<float> vertecies)
	{
		using (var path = new SKPath())
		{
			path.AddPoly(MemoryMarshal.Cast<float, SKPoint>(vertecies).ToArray());

			surface.Canvas.DrawPath(path, fill);
			surface.Canvas.DrawPath(path, stroke);
		}
	}
}