using System;
using System.Diagnostics.CodeAnalysis;
using CCreative.Rendering;

namespace CCreative;

public static partial class PApplet
{
	public static void Dispose()
	{
		graphics.Dispose();
	}

	public static Color[]? Pixels
	{
		get => graphics.Pixels;
		set => graphics.Pixels = value;
	}

	public static int PixelDensity => graphics.PixelDensity;

	public static void LoadPixels()
	{
		graphics.LoadPixels();
	}

	public static void UpdatePixels(int x, int y, int w, int h)
	{
		graphics.UpdatePixels(x, y, w, h);
	}

	public static void UpdatePixels()
	{
		graphics.UpdatePixels();
	}

	public static void Resize(int width, int height)
	{
		graphics.Resize(width, height);
	}

	public static Color Get(int x, int y)
	{
		return graphics.Get(x, y);
	}

	public static Image Get(int x, int y, int w, int h)
	{
		return graphics.Get(x, y, w, h);
	}

	public static Image Get()
	{
		return graphics.Get();
	}

	public static Image Copy()
	{
		return graphics.Copy();
	}

	public static void Set(int x, int y, Color color)
	{
		graphics.Set(x, y, color);
	}

	public static void Set(int x, int y, Image img)
	{
		graphics.Set(x, y, img);
	}

	public static void Mask(Color[] maskArray)
	{
		graphics.Mask(maskArray);
	}

	public static void Mask(Image img)
	{
		graphics.Mask(img);
	}

	public static void Filter(FilterTypes kind)
	{
		graphics.Filter(kind);
	}

	public static void Filter(FilterTypes type, float param)
	{
		graphics.Filter(type, param);
	}

	public static void Copy(int sx, int sy, int sw, int sh, int dx, int dy, int dw, int dh)
	{
		graphics.Copy(sx, sy, sw, sh, dx, dy, dw, dh);
	}

	public static void Copy(Image src, int sx, int sy, int sw, int sh, int dx, int dy, int dw, int dh)
	{
		graphics.Copy(src, sx, sy, sw, sh, dx, dy, dw, dh);
	}

	public static Color BlendColor(Color c1, Color c2, BlendModes mode)
	{
		return graphics.BlendColor(c1, c2, mode);
	}

	public static void Blend(int sx, int sy, int sw, int sh, int dx, int dy, int dw, int dh, BlendModes mode)
	{
		graphics.Blend(sx, sy, sw, sh, dx, dy, dw, dh, mode);
	}

	public static void Blend(Image src, int sx, int sy, int sw, int sh, int dx, int dy, int dw, int dh, BlendModes mode)
	{
		graphics.Blend(src, sx, sy, sw, sh, dx, dy, dw, dh, mode);
	}

	public static bool TrySave(string filename)
	{
		return graphics.TrySave(filename);
	}

	public static Color Background(Color color)
	{
		return graphics.Background(color);
	}

	public static void Background(Image image)
	{
		graphics.Background(image);
	}

	public static void ColorMode(ColorModes mode, float max1, float max2, float max3)
	{
		graphics.ColorMode(mode, max1, max2, max3);
	}

	public static void ColorMode(ColorModes mode, float max1, float max2, float max3, float maxA)
	{
		graphics.ColorMode(mode, max1, max2, max3, maxA);
	}

	public static Color Color(float gray)
	{
		return graphics.Color(gray);
	}

	public static Color Color(Color color, float alpha)
	{
		return graphics.Color(color, alpha);
	}

	public static Color Color(float gray, float alpha)
	{
		return graphics.Color(gray, alpha);
	}

	public static Color Color(float v1, float v2, float v3)
	{
		return graphics.Color(v1, v2, v3);
	}

	public static Color Color(float v1, float v2, float v3, float a)
	{
		return graphics.Color(v1, v2, v3, a);
	}

	public static float Alpha(Color color)
	{
		return graphics.Alpha(color);
	}

	public static float Red(Color color)
	{
		return graphics.Red(color);
	}

	public static float Green(Color color)
	{
		return graphics.Green(color);
	}

	public static float Blue(Color color)
	{
		return graphics.Blue(color);
	}

	public static float Hue(Color color)
	{
		return graphics.Hue(color);
	}

	public static float Saturation(Color color)
	{
		return graphics.Saturation(color);
	}

	public static float Brightness(Color color)
	{
		return graphics.Brightness(color);
	}

	public static Color ContrastColor(Color color)
	{
		return graphics.ContrastColor(color);
	}

	public static Color LerpColor(Color c1, Color c2, float amt)
	{
		return graphics.LerpColor(c1, c2, amt);
	}

	public static void NoStroke()
	{
		graphics.NoStroke();
	}

	public static Color Stroke(Color color)
	{
		return graphics.Stroke(color);
	}

	public static void NoFill()
	{
		graphics.NoFill();
	}

	public static Color Fill(Color color)
	{
		return graphics.Fill(color);
	}

	public static void NoTint()
	{
		graphics.NoTint();
	}

	public static Color Tint(Color color)
	{
		return graphics.Tint(color);
	}

	public static void Ambient(Color color)
	{
		graphics.Ambient(color);
	}

	public static void AmbientLight(Color color)
	{
		graphics.AmbientLight(color);
	}

	public static void ApplyMatrix(float n00, float n01, float n02, float n10, float n11, float n12)
	{
		graphics.ApplyMatrix(n00, n01, n02, n10, n11, n12);
	}

	public static void ApplyMatrix(float n00, float n01, float n02, float n03, float n10, float n11, float n12,
		float n13,
		float n20, float n21, float n22, float n23, float n30, float n31, float n32, float n33)
	{
		graphics.ApplyMatrix(n00, n01, n02, n03, n10, n11, n12, n13, n20, n21, n22, n23, n30, n31, n32, n33);
	}

	public static void ApplyMatrix(Matrix matrix)
	{
		graphics.ApplyMatrix(matrix);
	}

	public static void Arc(float a, float b, float c, float d, float start, float stop)
	{
		graphics.Arc(a, b, c, d, start, stop);
	}

	public static void BeginCamera()
	{
		graphics.BeginCamera();
	}

	public static void BeginContour()
	{
		graphics.BeginContour();
	}

	public static void BeginDraw()
	{
		graphics.BeginDraw();
	}

	public static void BeginShape()
	{
		graphics.BeginShape();
	}

	public static void BeginShape(ShapeTypes type)
	{
		graphics.BeginShape(type);
	}

	public static void Bezier(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4)
	{
		graphics.Bezier(x1, y1, x2, y2, x3, y3, x4, y4);
	}

	public static void Bezier(float x1, float y1, float z1, float x2, float y2, float z2, float x3, float y3,
		float z3,
		float x4, float y4, float z4)
	{
		graphics.Bezier(x1, y1, z1, x2, y2, z2, x3, y3, z3, x4, y4, z4);
	}

	public static void BezierDetail(int detail)
	{
		graphics.BezierDetail(detail);
	}

	public static float BezierPoint(float a, float b, float c, float d, float t)
	{
		return graphics.BezierPoint(a, b, c, d, t);
	}

	public static float BezierTangent(float a, float b, float c, float d, float t)
	{
		return graphics.BezierTangent(a, b, c, d, t);
	}

	public static void BezierVertex(float x2, float y2, float x3, float y3, float x4, float y4)
	{
		graphics.BezierVertex(x2, y2, x3, y3, x4, y4);
	}

	public static void BezierVertex(float x2, float y2, float z2, float x3, float y3, float z3, float x4, float y4,
		float z4)
	{
		graphics.BezierVertex(x2, y2, z2, x3, y3, z3, x4, y4, z4);
	}

	public static void BlendMode(int mode)
	{
		graphics.BlendMode(mode);
	}

	public static void Box(float w, float h, float d)
	{
		graphics.Box(w, h, d);
	}

	public static void Camera()
	{
		graphics.Camera();
	}

	public static void Camera(float eyeX, float eyeY, float eyeZ, float centerX, float centerY, float centerZ,
		float upX,
		float upY, float upZ)
	{
		graphics.Camera(eyeX, eyeY, eyeZ, centerX, centerY, centerZ, upX, upY, upZ);
	}

	public static void Circle(float x, float y, float extent)
	{
		graphics.Circle(x, y, extent);
	}

	public static void Clear()
	{
		graphics.Clear();
	}

	public static void Clip(int a, int b, int c, int d)
	{
		graphics.Clip(a, b, c, d);
	}

	public static void Curve(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4)
	{
		graphics.Curve(x1, y1, x2, y2, x3, y3, x4, y4);
	}

	public static void Curve(float x1, float y1, float z1, float x2, float y2, float z2, float x3, float y3, float z3,
		float x4,
		float y4, float z4)
	{
		graphics.Curve(x1, y1, z1, x2, y2, z2, x3, y3, z3, x4, y4, z3);
	}

	public static void CurveDetail(int detail)
	{
		graphics.CurveDetail(detail);
	}

	public static void CurvePoint(float a, float b, float c, float d, float t)
	{
		graphics.CurvePoint(a, b, c, d, t);
	}

	public static void CurveTangent(float a, float b, float c, float d, float t)
	{
		graphics.CurveTangent(a, b, c, d, t);
	}

	public static void CurveTightness(float tightness)
	{
		graphics.CurveTightness(tightness);
	}

	public static void CurveVertex(float x, float y)
	{
		graphics.CurveVertex(x, y);
	}

	public static void CurveVertex(float x, float y, float z)
	{
		graphics.CurveVertex(x, y, z);
	}

	public static void DirectionalLight(float v1, float v2, float v3, float nx, float ny, float nz)
	{
		graphics.DirectionalLight(v1, v2, v3, nx, ny, nz);
	}

	public static bool Displayable()
	{
		return true;
	}

	public static void Edge(bool edge)
	{
		graphics.Edge(edge);
	}

	public static void Ellipse(float a, float b, float c, float d)
	{
		graphics.Ellipse(a, b, c, d);
	}

	public static void EllipseMode(int mode)
	{
		graphics.EllipseMode(mode);
	}

	public static void Emissive(float gray)
	{
		graphics.Emissive(gray);
	}

	public static void Emissive(float v1, float v2, float v3)
	{
		graphics.Emissive(v1, v2, v3);
	}

	public static void Emissive(Color color)
	{
		graphics.Emissive(color);
	}

	public static void EndCamera()
	{
		graphics.EndCamera();
	}

	public static void EndContour()
	{
		graphics.EndContour();
	}

	public static void EndDraw()
	{
		throw new NotImplementedException();
	}

	public static void EndShape(CloseType close)
	{
		graphics.EndShape(close);
	}

	public static void Flush()
	{
		throw new NotImplementedException();
	}

	public static void Frustum(float left, float right, float bottom, float top, float near, float far)
	{
		graphics.Frustum(left, right, bottom, top, near, far);
	}

	public static Matrix GetMatrix()
	{
		return graphics.GetMatrix();
	}

	public static Matrix2D GetMatrix(Matrix2D target)
	{
		return graphics.GetMatrix(target);
	}

	public static Matrix3D GetMatrix(Matrix3D target)
	{
		return graphics.GetMatrix(target);
	}

	public static PStyle GetStyle()
	{
		return graphics.GetStyle();
	}

	public static PStyle GetStyle(PStyle target)
	{
		return graphics.GetStyle(target);
	}

	public static void Hint(int which)
	{
		graphics.Hint(which);
	}

	public static void Image(Image img, float a, float b)
	{
		graphics.Image(img, a, b);
	}

	public static void Image(Image img, float a, float b, float c, float d)
	{
		graphics.Image(img, a, b, c, d);
	}

	public static void Image(Image img, float a, float b, float c, float d, int u1, int v1, int u2, int v2)
	{
		graphics.Image(img, a, b, c, d, u1, v1, v2, u2);
	}

	public static void ImageMode(int mode)
	{
		graphics.ImageMode(mode);
	}

	public static bool Is2D()
	{
		return graphics.Is2D();
	}

	public static bool Is3D()
	{
		return graphics.Is3D();
	}

	public static bool IsGL()
	{
		return graphics.IsGL();
	}

	public static void LightFalloff(float constant, float linear, float quadratic)
	{
		graphics.LightFalloff(constant, linear, quadratic);
	}

	public static void Lights()
	{
		graphics.Lights();
	}

	public static void LightSpecular(float v1, float v2, float v3)
	{
		graphics.LightSpecular(v1, v2, v3);
	}

	public static void Line(float x1, float y1, float x2, float y2)
	{
		graphics.Line(x1, y1, x2, y2);
	}

	public static void Line(float x1, float y1, float z1, float x2, float y2, float z2)
	{
		graphics.Line(x1, y1, z1, x2, y2, z2);
	}

	public static float ModelX(float x, float y, float z)
	{
		return graphics.ModelX(x, y, z);
	}

	public static float ModelY(float x, float y, float z)
	{
		return graphics.ModelY(x, y, z);
	}

	public static float ModelZ(float x, float y, float z)
	{
		return graphics.ModelZ(x, y, z);
	}

	public static void NoClip()
	{
		graphics.NoClip();
	}

	public static void NoLights()
	{
		graphics.NoLights();
	}

	public static void Normal(float nx, float ny, float nz)
	{
		graphics.Normal(nx, ny, nz);
	}

	public static void NoSmooth()
	{
		graphics.NoSmooth();
	}

	public static void NoTexture()
	{
		graphics.NoTexture();
	}

	public static void Ortho()
	{
		graphics.Ortho();
	}

	public static void Ortho(float left, float right, float bottom, float top)
	{
		graphics.Ortho(left, right, bottom, top);
	}

	public static void Ortho(float left, float right, float bottom, float top, float near, float far)
	{
		graphics.Ortho(left, right, bottom, top, near, far);
	}

	public static void Perspective()
	{
		graphics.Perspective();
	}

	public static void Perspective(float fovY, float aspect, float zNear, float zFar)
	{
		graphics.Perspective(fovY, aspect, zNear, zFar);
	}

	public static void Point(float x, float y)
	{
		graphics.Point(x, y);
	}

	public static void Point(float x, float y, float z)
	{
		graphics.Point(x, y, z);
	}

	public static void PointLight(float v1, float v2, float v3, float x, float y, float z)
	{
		graphics.PointLight(v1, v2, v3, x, y, z);
	}

	public static void Pop()
	{
		graphics.Pop();
	}

	public static void PopMatrix()
	{
		graphics.PopMatrix();
	}

	public static void PopStyle()
	{
		graphics.PopStyle();
	}

	public static void PrintCamera()
	{
		graphics.PrintCamera();
	}

	public static void PrintMatrix()
	{
		graphics.PrintMatrix();
	}

	public static void PrintProjection()
	{
		graphics.PrintProjection();
	}

	public static void Push()
	{
		graphics.Push();
	}

	public static void PushMatrix()
	{
		graphics.PushMatrix();
	}

	public static void PushStyle()
	{
		graphics.PushStyle();
	}

	public static void Quad(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4)
	{
		graphics.Quad(x1, y1, x2, y2, x3, y3, x4, y4);
	}

	public static void QuadraticVertex(float cx, float cy, float x3, float y3)
	{
		graphics.QuadraticVertex(cx, cy, x3, y3);
	}

	public static void QuadraticVertex(float cx, float cy, float cz, float x3, float y3, float z3)
	{
		graphics.QuadraticVertex(cx, cy, cz, x3, y3, z3);
	}

	public static void Rect(float a, float b, float c, float d)
	{
		graphics.Rect(a, b, c, d);
	}

	public static void Rect(float a, float b, float c, float d, float r)
	{
		graphics.Rect(a, b, c, d);
	}

	public static void Rect(float a, float b, float c, float d, float tl, float tr, float br, float bl)
	{
		graphics.Rect(a, b, c, d, tl, tr, br, bl);
	}

	public static void RectMode(int mode)
	{
		graphics.RectMode(mode);
	}

	public static void ResetMatrix()
	{
		graphics.ResetMatrix();
	}

	public static void ResetShader()
	{
		graphics.ResetShader();
	}

	public static void ResetShader(int kind)
	{
		graphics.ResetShader(kind);
	}

	public static void Rotate(float angle, float x, float y, float z)
	{
		graphics.Rotate(angle, x, y, z);
	}

	public static void RotateX(float angle)
	{
		graphics.RotateX(angle);
	}

	public static void RotateY(float angle)
	{
		graphics.RotateY(angle);
	}

	public static void RotateZ(float angle)
	{
		graphics.RotateZ(angle);
	}

	public static void Scale(float s)
	{
		graphics.Scale(s);
	}

	public static void Scale(float x, float y)
	{
		graphics.Scale(x, y);
	}

	public static void Scale(float x, float y, float z)
	{
		graphics.Scale(x, y, z);
	}

	public static float ScreenX(float x, float y)
	{
		return graphics.ScreenX(x, y);
	}

	public static float ScreenX(float x, float y, float z)
	{
		return graphics.ScreenX(x, y, z);
	}

	public static float ScreenY(float x, float y)
	{
		return graphics.ScreenY(x, y);
	}

	public static float ScreenY(float x, float y, float z)
	{
		return ScreenY(x, y, z);
	}

	public static float ScreenZ(float x, float y)
	{
		return graphics.ScreenZ(x, y);
	}

	public static float ScreenZ(float x, float y, float z)
	{
		return graphics.ScreenZ(x, y, z);
	}

	public static void SetMatrix(Matrix source)
	{
		graphics.SetMatrix(source);
	}

	public static void SetMatrix(Matrix2D source)
	{
		graphics.SetMatrix(source);
	}

	public static void SetMatrix(Matrix3D source)
	{
		graphics.SetMatrix(source);
	}

	public static void SetPath(string path)
	{
		graphics.SetPath(path);
	}

	public static void SetPrimary(bool primary)
	{
		graphics.SetPrimary(primary);
	}

	public static void SetSize(int w, int h)
	{
		graphics.SetSize(w, h);
	}

	public static void ShapeMode(int kind)
	{
		graphics.ShapeMode(kind);
	}

	public static void ShearX(float angle)
	{
		graphics.ShearX(angle);
	}

	public static void ShearY(float angle)
	{
		graphics.ShearY(angle);
	}

	public static void Shininess(float shine)
	{
		Shininess(shine);
	}

	public static void Smooth()
	{
		graphics.Smooth();
	}

	public static void Smooth(int quality)
	{
		graphics.Smooth(quality);
	}

	public static void Specular(float gray)
	{
		graphics.Specular(gray);
	}

	public static void Specular(float v1, float v2, float v3)
	{
		graphics.Specular(v1, v2, v3);
	}

	public static void Specular(Color color)
	{
		graphics.Specular(color);
	}

	public static void Sphere(float r)
	{
		graphics.Sphere(r);
	}

	public static void SphereDetail(int res)
	{
		graphics.SphereDetail(res);
	}

	public static void SphereDetail(int ures, int vres)
	{
		graphics.SphereDetail(ures, vres);
	}

	public static void SpotLight(float v1, float v2, float v3, float x, float y, float z, float nx, float ny,
		float nz,
		float angle, float concentration)
	{
		graphics.SpotLight(v1, v2, v3, x, y, z, nx, ny, nz, angle, concentration);
	}

	public static void Square(float x, float y, float extent)
	{
		graphics.Square(x, y, extent);
	}

	public static void StrokeCap(int cap)
	{
		graphics.StrokeCap(cap);
	}

	public static void StrokeJoin(int @join)
	{
		graphics.StrokeJoin(join);
	}

	public static void StrokeWeight(float weight)
	{
		graphics.StrokeWeight(weight);
	}

	public static void Style(PStyle style)
	{
		graphics.Style(style);
	}

	public static void Text(string text, Range range, float x, float y)
	{
		graphics.Text(text, range, x, y);
	}

	public static void Text(string text, Range range, float x, float y, float z)
	{
		graphics.Text(text, range, x, y, z);
	}

	public static void Text(char[] chars, Range range, float x, float y)
	{
		graphics.Text(chars, range, x, y);
	}

	public static void Text(char[] chars, Range range, float x, float y, float z)
	{
		graphics.Text(chars, range, x, y, z);
	}

	public static void Text<T>(T num, float x, float y)
	{
		graphics.Text(num, x, y);
	}

	public static void Text<T>(T num, float x, float y, float z)
	{
		graphics.Text(num, x, y, z);
	}

	public static void Text(string text, float x1, float y1, float x2, float y2)
	{
		graphics.Text(text, x1, y1, x2, y2);
	}

	public static void TextAlign(int alignX)
	{
		graphics.TextAlign(alignX);
	}

	public static void TextAlign(int alignX, int alignY)
	{
		graphics.TextAlign(alignX, alignY);
	}

	public static float TextAcent()
	{
		return graphics.TextAcent();
	}

	public static float TextDecent()
	{
		return graphics.TextDecent();
	}

	public static void TextFont(PFont which)
	{
		graphics.TextFont(which);
	}

	public static void TextFont(PFont which, int size)
	{
		graphics.TextFont(which, size);
	}

	public static void TextLeading(float leading)
	{
		graphics.TextLeading(leading);
	}

	public static void TextMode(int mode)
	{
		graphics.TextMode(mode);
	}

	public static void TextSize(float size)
	{
		graphics.TextSize(size);
	}

	public static void Texture(Image texture)
	{
		graphics.Texture(texture);
	}

	public static void TextureMode(int mode)
	{
		graphics.TextureMode(mode);
	}

	public static void TextureWrap(int wrap)
	{
		graphics.TextureWrap(wrap);
	}

	public static float TextWidth(char c)
	{
		return graphics.TextWidth(c);
	}

	public static float TextWidth(char[] chars, int start, int length)
	{
		return graphics.TextWidth(chars, start, length);
	}

	public static float TextWidth(string text)
	{
		return TextWidth(text);
	}

	public static void Translate(float x, float y)
	{
		graphics.Translate(x, y);
	}

	public static void Translate(float x, float y, float z)
	{
		graphics.Translate(x, y, z);
	}

	public static void Triangle(float x1, float y1, float x2, float y2, float x3, float y3)
	{
		graphics.Triangle(x1, y1, x2, y2, x3, y3);
	}

	public static void Vertex(float x, float y)
	{
		graphics.Vertex(x, y);
	}

	public static void Vertex(float x, float y, float z)
	{
		graphics.Vertex(x, y, z);
	}

	public static void Vertex(float x, float y, float z, float u, float v)
	{
		graphics.Vertex(x, y, z, u, v);
	}

	public static void Vertex(float[] v)
	{
		graphics.Vertex(v);
	}

	public static void DrawShape(Span<float> vertecies)
	{
		graphics.DrawShape(vertecies);
	}
}