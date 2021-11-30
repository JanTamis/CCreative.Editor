using System;
using CCreative.Rendering;

namespace CCreative
{
	public partial class PApplet : PGraphics
	{
		public void Dispose()
		{
			graphics.Dispose();
		}

		public byte[]? Pixels
		{
			get => graphics.Pixels; 
			set => graphics.Pixels = value;
		}
		public int PixelDensity => graphics.PixelDensity;

		public void LoadPixels()
		{
			graphics.LoadPixels();
		}

		public void UpdatePixels(int x, int y, int w, int h)
		{
			graphics.UpdatePixels(x, y, w, h);
		}

		public void UpdatePixels()
		{
			graphics.UpdatePixels();
		}

		public void Resize(int width, int height)
		{
			graphics.Resize(width, height);
		}

		public Color Get(int x, int y)
		{
			return graphics.Get(x, y);
		}

		public PImage Get(int x, int y, int w, int h)
		{
			return graphics.Get(x, y, w, h);
		}

		public PImage Get()
		{
			return graphics.Get();
		}

		public PImage Copy()
		{
			return graphics.Copy();
		}

		public void Set(int x, int y, Color color)
		{
			graphics.Set(x, y, color);
		}

		public void Set(int x, int y, PImage img)
		{
			graphics.Set(x, y, img);
		}

		public void Mask(Color[] maskArray)
		{
			graphics.Mask(maskArray);
		}

		public void Mask(PImage img)
		{
			graphics.Mask(img);
		}

		public void Filter(FilterTypes kind)
		{
			graphics.Filter(kind);
		}

		public void Filter(FilterTypes type, double param)
		{
			graphics.Filter(type, param);
		}

		public void Copy(int sx, int sy, int sw, int sh, int dx, int dy, int dw, int dh)
		{
			graphics.Copy(sx, sy, sw, sh, dx, dy, dw, dh);
		}

		public void Copy(PImage src, int sx, int sy, int sw, int sh, int dx, int dy, int dw, int dh)
		{
			graphics.Copy(src, sx, sy, sw, sh, dx, dy, dw, dh);
		}

		public Color BlendColor(Color c1, Color c2, BlendModes mode)
		{
			return graphics.BlendColor(c1, c2, mode);
		}

		public void Blend(int sx, int sy, int sw, int sh, int dx, int dy, int dw, int dh, BlendModes mode)
		{
			graphics.Blend(sx, sy, sw, sh, dx, dy, dw, dh, mode);
		}

		public void Blend(PImage src, int sx, int sy, int sw, int sh, int dx, int dy, int dw, int dh, BlendModes mode)
		{
			graphics.Blend(src, sx, sy, sw, sh, dx, dy, dw, dh, mode);
		}

		public bool TrySave(string filename)
		{
			return graphics.TrySave(filename);
		}

		public Color Background(Color color)
		{
			return graphics.Background(color);
		}

		public void Background(PImage image)
		{
			graphics.Background(image);
		}

		public void ColorMode(ColorModes mode, double max1, double max2, double max3)
		{
			graphics.ColorMode(mode, max1, max2, max3);
		}

		public void ColorMode(ColorModes mode, double max1, double max2, double max3, double maxA)
		{
			graphics.ColorMode(mode, max1, max2, max3, maxA);
		}

		public Color Color(double gray)
		{
			return graphics.Color(gray);
		}

		public Color Color(Color color, double alpha)
		{
			return graphics.Color(color, alpha);
		}

		public Color Color(double gray, double alpha)
		{
			return graphics.Color(gray, alpha);
		}

		public Color Color(double v1, double v2, double v3)
		{
			return graphics.Color(v1, v2, v3);
		}

		public Color Color(double v1, double v2, double v3, double a)
		{
			return graphics.Color(v1, v2, v3, a);
		}

		public double Alpha(Color color)
		{
			return graphics.Alpha(color);
		}

		public double Red(Color color)
		{
			return graphics.Red(color);
		}

		public double Green(Color color)
		{
			return graphics.Green(color);
		}

		public double Blue(Color color)
		{
			return graphics.Blue(color);
		}

		public double Hue(Color color)
		{
			return graphics.Hue(color);
		}

		public double Saturation(Color color)
		{
			return graphics.Saturation(color);
		}

		public double Brightness(Color color)
		{
			return graphics.Brightness(color);
		}

		public Color ContrastColor(Color color)
		{
			return graphics.ContrastColor(color);
		}

		public Color LerpColor(Color c1, Color c2, double amt)
		{
			return graphics.LerpColor(c1, c2, amt);
		}

		public void NoStroke()
		{
			graphics.NoStroke();
		}

		public Color Stroke(Color color)
		{
			return graphics.Stroke(color);
		}

		public void NoFill()
		{
			graphics.NoFill();
		}

		public Color Fill(Color color)
		{
			return graphics.Fill(color);
		}

		public void NoTint()
		{
			graphics.NoTint();
		}

		public Color Tint(Color color)
		{
			return graphics.Tint(color);
		}

		public void Ambient(Color color)
		{
			graphics.Ambient(color);
		}

		public void AmbientLight(Color color)
		{
			graphics.AmbientLight(color);
		}

		public void ApplyMatrix(double n00, double n01, double n02, double n10, double n11, double n12)
		{
			graphics.ApplyMatrix(n00, n01, n02, n10, n11, n12);
		}

		public void ApplyMatrix(double n00, double n01, double n02, double n03, double n10, double n11, double n12,
			double n13,
			double n20, double n21, double n22, double n23, double n30, double n31, double n32, double n33)
		{
			graphics.ApplyMatrix(n00, n01, n02, n03, n10, n11, n12, n13, n20, n21, n22, n23, n30, n31, n32, n33);
		}

		public void ApplyMatrix(PMatrix matrix)
		{
			graphics.ApplyMatrix(matrix);
		}

		public void Arc(double a, double b, double c, double d, double start, double stop)
		{
			graphics.Arc(a, b, c, d, start, stop);
		}

		public void BeginCamera()
		{
			graphics.BeginCamera();
		}

		public void BeginContour()
		{
			graphics.BeginContour();
		}

		public void BeginDraw()
		{
			graphics.BeginDraw();
		}

		public void BeginShape()
		{
			graphics.BeginShape();
		}

		public void BeginShape(ShapeTypes type)
		{
			graphics.BeginShape(type);
		}

		public void Bezier(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4)
		{
			graphics.Bezier(x1, y1, x2, y2, x3, y3, x4, y4);
		}

		public void Bezier(double x1, double y1, double z1, double x2, double y2, double z2, double x3, double y3,
			double z3,
			double x4, double y4, double z4)
		{
			graphics.Bezier(x1, y1, z1, x2, y2, z2, x3, y3, z3, x4, y4, z4);
		}

		public void BezierDetail(int detail)
		{
			graphics.BezierDetail(detail);
		}

		public double BezierPoint(double a, double b, double c, double d, double t)
		{
			return graphics.BezierPoint(a, b, c, d, t);
		}

		public double BezierTangent(double a, double b, double c, double d, double t)
		{
			return graphics.BezierTangent(a, b, c, d, t);
		}

		public void BezierVertex(double x2, double y2, double x3, double y3, double x4, double y4)
		{
			graphics.BezierVertex(x2, y2, x3, y3, x4, y4);
		}

		public void BezierVertex(double x2, double y2, double z2, double x3, double y3, double z3, double x4, double y4,
			double z4)
		{
			graphics.BezierVertex(x2, y2, z2, x3, y3, z3, x4, y4, z4);
		}

		public void BlendMode(int mode)
		{
			graphics.BlendMode(mode);
		}

		public void Box(double w, double h, double d)
		{
			graphics.Box(w, h, d);
		}

		public void Camera()
		{
			graphics.Camera();
		}

		public void Camera(double eyeX, double eyeY, double eyeZ, double centerX, double centerY, double centerZ,
			double upX,
			double upY, double upZ)
		{
			graphics.Camera(eyeX, eyeY, eyeZ, centerX, centerY, centerZ, upX, upY, upZ);
		}

		public void Circle(double x, double y, double extent)
		{
			graphics.Circle(x, y, extent);
		}

		public void Clear()
		{
			graphics.Clear();
		}

		public void Clip(int a, int b, int c, int d)
		{
			graphics.Clip(a, b, c, d);
		}

		public void Curve(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4)
		{
			graphics.Curve(x1, y1, x2, y2, x3, y3, x4, y4);
		}

		public void Curve(double x1, double y1, double z1, double x2, double y2, double z2, double x3, double y3, double z3,
			double x4,
			double y4, double z4)
		{
			graphics.Curve(x1, y1, z1, x2, y2, z2, x3, y3, z3, x4, y4, z3);
		}

		public void CurveDetail(int detail)
		{
			graphics.CurveDetail(detail);
		}

		public void CurvePoint(double a, double b, double c, double d, double t)
		{
			graphics.CurvePoint(a, b, c, d, t);
		}

		public void CurveTangent(double a, double b, double c, double d, double t)
		{
			graphics.CurveTangent(a, b, c, d, t);
		}

		public void CurveTightness(double tightness)
		{
			graphics.CurveTightness(tightness);
		}

		public void CurveVertex(double x, double y)
		{
			graphics.CurveVertex(x, y);
		}

		public void CurveVertex(double x, double y, double z)
		{
			graphics.CurveVertex(x, y, z);
		}

		public void DirectionalLight(double v1, double v2, double v3, double nx, double ny, double nz)
		{
			graphics.DirectionalLight(v1, v2, v3, nx, ny, nz);
		}

		public bool Displayable()
		{
			return true;
		}

		public void Edge(bool edge)
		{
			graphics.Edge(edge);
		}

		public void Ellipse(double a, double b, double c, double d)
		{
			graphics.Ellipse(a, b, c, d);
		}

		public void EllipseMode(int mode)
		{
			graphics.EllipseMode(mode);
		}

		public void Emissive(double gray)
		{
			graphics.Emissive(gray);
		}

		public void Emissive(double v1, double v2, double v3)
		{
			graphics.Emissive(v1, v2, v3);
		}

		public void Emissive(Color color)
		{
			graphics.Emissive(color);
		}

		public void EndCamera()
		{
			graphics.EndCamera();
		}

		public void EndContour()
		{
			graphics.EndContour();
		}

		public void EndDraw()
		{
			throw new NotImplementedException();
		}

		public void EndShape(CloseType close)
		{
			graphics.EndShape(close);
		}

		public void Flush()
		{
			throw new NotImplementedException();
		}

		public void Frustum(double left, double right, double bottom, double top, double near, double far)
		{
			graphics.Frustum(left, right, bottom, top, near, far);
		}

		public PMatrix GetMatrix()
		{
			return graphics.GetMatrix();
		}

		public PMatrix2D GetMatrix(PMatrix2D target)
		{
			return graphics.GetMatrix(target);
		}

		public PMatrix3D GetMatrix(PMatrix3D target)
		{
			return graphics.GetMatrix(target);
		}

		public PStyle GetStyle()
		{
			return graphics.GetStyle();
		}

		public PStyle GetStyle(PStyle target)
		{
			return graphics.GetStyle(target);
		}

		public void Hint(int which)
		{
			graphics.Hint(which);
		}

		public void Image(PImage img, double a, double b)
		{
			graphics.Image(img, a, b);
		}

		public void Image(PImage img, double a, double b, double c, double d)
		{
			graphics.Image(img, a, b, c, d);
		}

		public void Image(PImage img, double a, double b, double c, double d, int u1, int v1, int u2, int v2)
		{
			graphics.Image(img, a, b, c, d, u1, v1, v2, u2);
		}

		public void ImageMode(int mode)
		{
			graphics.ImageMode(mode);
		}

		public bool Is2D()
		{
			return graphics.Is2D();
		}

		public bool Is3D()
		{
			return graphics.Is3D();
		}

		public bool IsGL()
		{
			return graphics.IsGL();
		}

		public void LightFalloff(double constant, double linear, double quadratic)
		{
			graphics.LightFalloff(constant, linear, quadratic);
		}

		public void Lights()
		{
			graphics.Lights();
		}

		public void LightSpecular(double v1, double v2, double v3)
		{
			graphics.LightSpecular(v1, v2, v3);
		}

		public void Line(double x1, double y1, double x2, double y2)
		{
			graphics.Line(x1, y1, x2, y2);
		}

		public void Line(double x1, double y1, double z1, double x2, double y2, double z2)
		{
			graphics.Line(x1, y1, z1, x2, y2, z2);
		}

		public double ModelX(double x, double y, double z)
		{
			return graphics.ModelX(x, y, z);
		}

		public double ModelY(double x, double y, double z)
		{
			return graphics.ModelY(x, y, z);
		}

		public double ModelZ(double x, double y, double z)
		{
			return graphics.ModelZ(x, y, z);
		}

		public void NoClip()
		{
			graphics.NoClip();
		}

		public void NoLights()
		{
			graphics.NoLights();
		}

		public void Normal(double nx, double ny, double nz)
		{
			graphics.Normal(nx, ny, nz);
		}

		public void NoSmooth()
		{
			graphics.NoSmooth();
		}

		public void NoTexture()
		{
			graphics.NoTexture();
		}

		public void Ortho()
		{
			graphics.Ortho();
		}

		public void Ortho(double left, double right, double bottom, double top)
		{
			graphics.Ortho(left, right, bottom, top);
		}

		public void Ortho(double left, double right, double bottom, double top, double near, double far)
		{
			graphics.Ortho(left, right, bottom, top, near, far);
		}

		public void Perspective()
		{
			graphics.Perspective();
		}

		public void Perspective(double fovY, double aspect, double zNear, double zFar)
		{
			graphics.Perspective(fovY, aspect, zNear, zFar);
		}

		public void Point(double x, double y)
		{
			graphics.Point(x, y);
		}

		public void Point(double x, double y, double z)
		{
			graphics.Point(x, y, z);
		}

		public void PointLight(double v1, double v2, double v3, double x, double y, double z)
		{
			graphics.PointLight(v1, v2, v3, x, y, z);
		}

		public void Pop()
		{
			graphics.Pop();
		}

		public void PopMatrix()
		{
			graphics.PopMatrix();
		}

		public void PopStyle()
		{
			graphics.PopStyle();
		}

		public void PrintCamera()
		{
			graphics.PrintCamera();
		}

		public void PrintMatrix()
		{
			graphics.PrintMatrix();
		}

		public void PrintProjection()
		{
			graphics.PrintProjection();
		}

		public void Push()
		{
			graphics.Push();
		}

		public void PushMatrix()
		{
			graphics.PushMatrix();
		}

		public void PushStyle()
		{
			graphics.PushStyle();
		}

		public void Quad(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4)
		{
			graphics.Quad(x1, y1, x2, y2, x3, y3, x4, y4);
		}

		public void QuadraticVertex(double cx, double cy, double x3, double y3)
		{
			graphics.QuadraticVertex(cx, cy, x3, y3);
		}

		public void QuadraticVertex(double cx, double cy, double cz, double x3, double y3, double z3)
		{
			graphics.QuadraticVertex(cx, cy, cz, x3, y3, z3);
		}

		public void Rect(double a, double b, double c, double d)
		{
			graphics.Rect(a, b, c, d);
		}

		public void Rect(double a, double b, double c, double d, double r)
		{
			graphics.Rect(a, b, c, d);
		}

		public void Rect(double a, double b, double c, double d, double tl, double tr, double br, double bl)
		{
			graphics.Rect(a, b, c, d, tl, tr, br, bl);
		}

		public void RectMode(int mode)
		{
			graphics.RectMode(mode);
		}

		public void ResetMatrix()
		{
			graphics.ResetMatrix();
		}

		public void ResetShader()
		{
			graphics.ResetShader();
		}

		public void ResetShader(int kind)
		{
			graphics.ResetShader(kind);
		}

		public void Rotate(double angle, double x, double y, double z)
		{
			graphics.Rotate(angle, x, y, z);
		}

		public void RotateX(double angle)
		{
			graphics.RotateX(angle);
		}

		public void RotateY(double angle)
		{
			graphics.RotateY(angle);
		}

		public void RotateZ(double angle)
		{
			graphics.RotateZ(angle);
		}

		public void Scale(double s)
		{
			graphics.Scale(s);
		}

		public void Scale(double x, double y)
		{
			graphics.Scale(x, y);
		}

		public void Scale(double x, double y, double z)
		{
			graphics.Scale(x, y, z);
		}

		public double ScreenX(double x, double y)
		{
			return graphics.ScreenX(x, y);
		}

		public double ScreenX(double x, double y, double z)
		{
			return graphics.ScreenX(x, y, z);
		}

		public double ScreenY(double x, double y)
		{
			return graphics.ScreenY(x, y);
		}

		public double ScreenY(double x, double y, double z)
		{
			return ScreenY(x, y, z);
		}

		public double ScreenZ(double x, double y)
		{
			return graphics.ScreenZ(x, y);
		}

		public double ScreenZ(double x, double y, double z)
		{
			return graphics.ScreenZ(x, y, z);
		}

		public void SetMatrix(PMatrix source)
		{
			graphics.SetMatrix(source);
		}

		public void SetMatrix(PMatrix2D source)
		{
			graphics.SetMatrix(source);
		}

		public void SetMatrix(PMatrix3D source)
		{
			graphics.SetMatrix(source);
		}

		public void SetParent(PApplet parent)
		{
			throw new NotImplementedException();
		}

		public void SetPath(string path)
		{
			graphics.SetPath(path);
		}

		public void SetPrimary(bool primary)
		{
			graphics.SetPrimary(primary);
		}

		public void SetSize(int w, int h)
		{
			graphics.SetSize(w, h);
		}

		public void ShapeMode(int kind)
		{
			graphics.ShapeMode(kind);
		}

		public void ShearX(double angle)
		{
			graphics.ShearX(angle);
		}

		public void ShearY(double angle)
		{
			graphics.ShearY(angle);
		}

		public void Shininess(double shine)
		{
			Shininess(shine);
		}

		public void Smooth()
		{
			graphics.Smooth();
		}

		public void Smooth(int quality)
		{
			graphics.Smooth(quality);
		}

		public void Specular(double gray)
		{
			graphics.Specular(gray);
		}

		public void Specular(double v1, double v2, double v3)
		{
			graphics.Specular(v1, v2, v3);
		}

		public void Specular(Color color)
		{
			graphics.Specular(color);
		}

		public void Sphere(double r)
		{
			graphics.Sphere(r);
		}

		public void SphereDetail(int res)
		{
			graphics.SphereDetail(res);
		}

		public void SphereDetail(int ures, int vres)
		{
			graphics.SphereDetail(ures, vres);
		}

		public void SpotLight(double v1, double v2, double v3, double x, double y, double z, double nx, double ny,
			double nz,
			double angle, double concentration)
		{
			graphics.SpotLight(v1, v2, v3, x, y, z, nx, ny, nz, angle, concentration);
		}

		public void Square(double x, double y, double extent)
		{
			graphics.Square(x, y, extent);
		}

		public void StrokeCap(int cap)
		{
			graphics.StrokeCap(cap);
		}

		public void StrokeJoin(int @join)
		{
			graphics.StrokeJoin(join);
		}

		public void StrokeWeight(double weight)
		{
			graphics.StrokeWeight(weight);
		}

		public void Style(PStyle style)
		{
			graphics.Style(style);
		}

		public void Text(string text, Index start, Index end, double x, double y)
		{
			graphics.Text(text, start, end, x, y);
		}

		public void Text(string text, Index start, Index end, double x, double y, double z)
		{
			graphics.Text(text, start, end, x, y, z);
		}

		public void Text(string text, Range range, double x, double y)
		{
			graphics.Text(text, range, x, y);
		}

		public void Text(string text, Range range, double x, double y, double z)
		{
			graphics.Text(text, range, x, y, z);
		}

		public void Text(char[] chars, Index start, Index end, double x, double y)
		{
			graphics.Text(chars, start, end, x, y);
		}

		public void Text(char[] chars, Index start, Index end, double x, double y, double z)
		{
			graphics.Text(chars, start, end, x, y, z);
		}

		public void Text(char[] chars, Range range, double x, double y)
		{
			graphics.Text(chars, range, x, y);
		}

		public void Text(char[] chars, Range range, double x, double y, double z)
		{
			graphics.Text(chars, range, x, y, z);
		}

		public void Text<T>(T num, double x, double y) where T : IConvertible, IFormattable
		{
			graphics.Text(num, x, y);
		}

		public void Text<T>(T num, double x, double y, double z) where T : IConvertible, IFormattable
		{
			graphics.Text(num, x, y, z);
		}

		public void Text(string text, double x1, double y1, double x2, double y2)
		{
			graphics.Text(text, x1, y1, x2, y2);
		}

		public void TextAlign(int alignX)
		{
			graphics.TextAlign(alignX);
		}

		public void TextAlign(int alignX, int alignY)
		{
			graphics.TextAlign(alignX, alignY);
		}

		public short TextAcent()
		{
			return graphics.TextAcent();
		}

		public short TextDecent()
		{
			return graphics.TextDecent();
		}

		public void TextFont(PFont which)
		{
			graphics.TextFont(which);
		}

		public void TextFont(PFont which, int size)
		{
			graphics.TextFont(which, size);
		}

		public void TextLeading(double leading)
		{
			graphics.TextLeading(leading);
		}

		public void TextMode(int mode)
		{
			graphics.TextMode(mode);
		}

		public void TextSize(double size)
		{
			graphics.TextSize(size);
		}

		public void Texture(PImage texture)
		{
			graphics.Texture(texture);
		}

		public void TextureMode(int mode)
		{
			graphics.TextureMode(mode);
		}

		public void TextureWrap(int wrap)
		{
			graphics.TextureWrap(wrap);
		}

		public double TextWidth(char c)
		{
			return graphics.TextWidth(c);
		}

		public double TextWidth(char[] chars, int start, int length)
		{
			return graphics.TextWidth(chars, start, length);
		}

		public double TextWidth(string text)
		{
			return TextWidth(text);
		}

		public void Translate(double x, double y)
		{
			graphics.Translate(x, y);
		}

		public void Translate(double x, double y, double z)
		{
			graphics.Translate(x, y, z);
		}

		public void Triangle(double x1, double y1, double x2, double y2, double x3, double y3)
		{
			graphics.Triangle(x1, y1, x2, y2, x3, y3);
		}

		public void Vertex(double x, double y)
		{
			graphics.Vertex(x, y);
		}

		public void Vertex(double x, double y, double z)
		{
			graphics.Vertex(x, y, z);
		}

		public void Vertex(double x, double y, double z, double u, double v)
		{
			graphics.Vertex(x, y, z, u, v);
		}

		public void Vertex(double[] v)
		{
			graphics.Vertex(v);
		}
	}
}