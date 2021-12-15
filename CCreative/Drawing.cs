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

		public void Filter(FilterTypes type, float param)
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

		public void ColorMode(ColorModes mode, float max1, float max2, float max3)
		{
			graphics.ColorMode(mode, max1, max2, max3);
		}

		public void ColorMode(ColorModes mode, float max1, float max2, float max3, float maxA)
		{
			graphics.ColorMode(mode, max1, max2, max3, maxA);
		}

		public Color Color(float gray)
		{
			return graphics.Color(gray);
		}

		public Color Color(Color color, float alpha)
		{
			return graphics.Color(color, alpha);
		}

		public Color Color(float gray, float alpha)
		{
			return graphics.Color(gray, alpha);
		}

		public Color Color(float v1, float v2, float v3)
		{
			return graphics.Color(v1, v2, v3);
		}

		public Color Color(float v1, float v2, float v3, float a)
		{
			return graphics.Color(v1, v2, v3, a);
		}

		public float Alpha(Color color)
		{
			return graphics.Alpha(color);
		}

		public float Red(Color color)
		{
			return graphics.Red(color);
		}

		public float Green(Color color)
		{
			return graphics.Green(color);
		}

		public float Blue(Color color)
		{
			return graphics.Blue(color);
		}

		public float Hue(Color color)
		{
			return graphics.Hue(color);
		}

		public float Saturation(Color color)
		{
			return graphics.Saturation(color);
		}

		public float Brightness(Color color)
		{
			return graphics.Brightness(color);
		}

		public Color ContrastColor(Color color)
		{
			return graphics.ContrastColor(color);
		}

		public Color LerpColor(Color c1, Color c2, float amt)
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

		public void ApplyMatrix(float n00, float n01, float n02, float n10, float n11, float n12)
		{
			graphics.ApplyMatrix(n00, n01, n02, n10, n11, n12);
		}

		public void ApplyMatrix(float n00, float n01, float n02, float n03, float n10, float n11, float n12,
			float n13,
			float n20, float n21, float n22, float n23, float n30, float n31, float n32, float n33)
		{
			graphics.ApplyMatrix(n00, n01, n02, n03, n10, n11, n12, n13, n20, n21, n22, n23, n30, n31, n32, n33);
		}

		public void ApplyMatrix(PMatrix matrix)
		{
			graphics.ApplyMatrix(matrix);
		}

		public void Arc(float a, float b, float c, float d, float start, float stop)
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

		public void Bezier(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4)
		{
			graphics.Bezier(x1, y1, x2, y2, x3, y3, x4, y4);
		}

		public void Bezier(float x1, float y1, float z1, float x2, float y2, float z2, float x3, float y3,
			float z3,
			float x4, float y4, float z4)
		{
			graphics.Bezier(x1, y1, z1, x2, y2, z2, x3, y3, z3, x4, y4, z4);
		}

		public void BezierDetail(int detail)
		{
			graphics.BezierDetail(detail);
		}

		public float BezierPoint(float a, float b, float c, float d, float t)
		{
			return graphics.BezierPoint(a, b, c, d, t);
		}

		public float BezierTangent(float a, float b, float c, float d, float t)
		{
			return graphics.BezierTangent(a, b, c, d, t);
		}

		public void BezierVertex(float x2, float y2, float x3, float y3, float x4, float y4)
		{
			graphics.BezierVertex(x2, y2, x3, y3, x4, y4);
		}

		public void BezierVertex(float x2, float y2, float z2, float x3, float y3, float z3, float x4, float y4,
			float z4)
		{
			graphics.BezierVertex(x2, y2, z2, x3, y3, z3, x4, y4, z4);
		}

		public void BlendMode(int mode)
		{
			graphics.BlendMode(mode);
		}

		public void Box(float w, float h, float d)
		{
			graphics.Box(w, h, d);
		}

		public void Camera()
		{
			graphics.Camera();
		}

		public void Camera(float eyeX, float eyeY, float eyeZ, float centerX, float centerY, float centerZ,
			float upX,
			float upY, float upZ)
		{
			graphics.Camera(eyeX, eyeY, eyeZ, centerX, centerY, centerZ, upX, upY, upZ);
		}

		public void Circle(float x, float y, float extent)
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

		public void Curve(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4)
		{
			graphics.Curve(x1, y1, x2, y2, x3, y3, x4, y4);
		}

		public void Curve(float x1, float y1, float z1, float x2, float y2, float z2, float x3, float y3, float z3,
			float x4,
			float y4, float z4)
		{
			graphics.Curve(x1, y1, z1, x2, y2, z2, x3, y3, z3, x4, y4, z3);
		}

		public void CurveDetail(int detail)
		{
			graphics.CurveDetail(detail);
		}

		public void CurvePoint(float a, float b, float c, float d, float t)
		{
			graphics.CurvePoint(a, b, c, d, t);
		}

		public void CurveTangent(float a, float b, float c, float d, float t)
		{
			graphics.CurveTangent(a, b, c, d, t);
		}

		public void CurveTightness(float tightness)
		{
			graphics.CurveTightness(tightness);
		}

		public void CurveVertex(float x, float y)
		{
			graphics.CurveVertex(x, y);
		}

		public void CurveVertex(float x, float y, float z)
		{
			graphics.CurveVertex(x, y, z);
		}

		public void DirectionalLight(float v1, float v2, float v3, float nx, float ny, float nz)
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

		public void Ellipse(float a, float b, float c, float d)
		{
			graphics.Ellipse(a, b, c, d);
		}

		public void EllipseMode(int mode)
		{
			graphics.EllipseMode(mode);
		}

		public void Emissive(float gray)
		{
			graphics.Emissive(gray);
		}

		public void Emissive(float v1, float v2, float v3)
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

		public void Frustum(float left, float right, float bottom, float top, float near, float far)
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

		public void Image(PImage img, float a, float b)
		{
			graphics.Image(img, a, b);
		}

		public void Image(PImage img, float a, float b, float c, float d)
		{
			graphics.Image(img, a, b, c, d);
		}

		public void Image(PImage img, float a, float b, float c, float d, int u1, int v1, int u2, int v2)
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

		public void LightFalloff(float constant, float linear, float quadratic)
		{
			graphics.LightFalloff(constant, linear, quadratic);
		}

		public void Lights()
		{
			graphics.Lights();
		}

		public void LightSpecular(float v1, float v2, float v3)
		{
			graphics.LightSpecular(v1, v2, v3);
		}

		public void Line(float x1, float y1, float x2, float y2)
		{
			graphics.Line(x1, y1, x2, y2);
		}

		public void Line(float x1, float y1, float z1, float x2, float y2, float z2)
		{
			graphics.Line(x1, y1, z1, x2, y2, z2);
		}

		public float ModelX(float x, float y, float z)
		{
			return graphics.ModelX(x, y, z);
		}

		public float ModelY(float x, float y, float z)
		{
			return graphics.ModelY(x, y, z);
		}

		public float ModelZ(float x, float y, float z)
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

		public void Normal(float nx, float ny, float nz)
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

		public void Ortho(float left, float right, float bottom, float top)
		{
			graphics.Ortho(left, right, bottom, top);
		}

		public void Ortho(float left, float right, float bottom, float top, float near, float far)
		{
			graphics.Ortho(left, right, bottom, top, near, far);
		}

		public void Perspective()
		{
			graphics.Perspective();
		}

		public void Perspective(float fovY, float aspect, float zNear, float zFar)
		{
			graphics.Perspective(fovY, aspect, zNear, zFar);
		}

		public void Point(float x, float y)
		{
			graphics.Point(x, y);
		}

		public void Point(float x, float y, float z)
		{
			graphics.Point(x, y, z);
		}

		public void PointLight(float v1, float v2, float v3, float x, float y, float z)
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

		public void Quad(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4)
		{
			graphics.Quad(x1, y1, x2, y2, x3, y3, x4, y4);
		}

		public void QuadraticVertex(float cx, float cy, float x3, float y3)
		{
			graphics.QuadraticVertex(cx, cy, x3, y3);
		}

		public void QuadraticVertex(float cx, float cy, float cz, float x3, float y3, float z3)
		{
			graphics.QuadraticVertex(cx, cy, cz, x3, y3, z3);
		}

		public void Rect(float a, float b, float c, float d)
		{
			graphics.Rect(a, b, c, d);
		}

		public void Rect(float a, float b, float c, float d, float r)
		{
			graphics.Rect(a, b, c, d);
		}

		public void Rect(float a, float b, float c, float d, float tl, float tr, float br, float bl)
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

		public void Rotate(float angle, float x, float y, float z)
		{
			graphics.Rotate(angle, x, y, z);
		}

		public void RotateX(float angle)
		{
			graphics.RotateX(angle);
		}

		public void RotateY(float angle)
		{
			graphics.RotateY(angle);
		}

		public void RotateZ(float angle)
		{
			graphics.RotateZ(angle);
		}

		public void Scale(float s)
		{
			graphics.Scale(s);
		}

		public void Scale(float x, float y)
		{
			graphics.Scale(x, y);
		}

		public void Scale(float x, float y, float z)
		{
			graphics.Scale(x, y, z);
		}

		public float ScreenX(float x, float y)
		{
			return graphics.ScreenX(x, y);
		}

		public float ScreenX(float x, float y, float z)
		{
			return graphics.ScreenX(x, y, z);
		}

		public float ScreenY(float x, float y)
		{
			return graphics.ScreenY(x, y);
		}

		public float ScreenY(float x, float y, float z)
		{
			return ScreenY(x, y, z);
		}

		public float ScreenZ(float x, float y)
		{
			return graphics.ScreenZ(x, y);
		}

		public float ScreenZ(float x, float y, float z)
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

		public void ShearX(float angle)
		{
			graphics.ShearX(angle);
		}

		public void ShearY(float angle)
		{
			graphics.ShearY(angle);
		}

		public void Shininess(float shine)
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

		public void Specular(float gray)
		{
			graphics.Specular(gray);
		}

		public void Specular(float v1, float v2, float v3)
		{
			graphics.Specular(v1, v2, v3);
		}

		public void Specular(Color color)
		{
			graphics.Specular(color);
		}

		public void Sphere(float r)
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

		public void SpotLight(float v1, float v2, float v3, float x, float y, float z, float nx, float ny,
			float nz,
			float angle, float concentration)
		{
			graphics.SpotLight(v1, v2, v3, x, y, z, nx, ny, nz, angle, concentration);
		}

		public void Square(float x, float y, float extent)
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

		public void StrokeWeight(float weight)
		{
			graphics.StrokeWeight(weight);
		}

		public void Style(PStyle style)
		{
			graphics.Style(style);
		}

		public void Text(string text, Index start, Index end, float x, float y)
		{
			graphics.Text(text, start, end, x, y);
		}

		public void Text(string text, Index start, Index end, float x, float y, float z)
		{
			graphics.Text(text, start, end, x, y, z);
		}

		public void Text(string text, Range range, float x, float y)
		{
			graphics.Text(text, range, x, y);
		}

		public void Text(string text, Range range, float x, float y, float z)
		{
			graphics.Text(text, range, x, y, z);
		}

		public void Text(char[] chars, Index start, Index end, float x, float y)
		{
			graphics.Text(chars, start, end, x, y);
		}

		public void Text(char[] chars, Index start, Index end, float x, float y, float z)
		{
			graphics.Text(chars, start, end, x, y, z);
		}

		public void Text(char[] chars, Range range, float x, float y)
		{
			graphics.Text(chars, range, x, y);
		}

		public void Text(char[] chars, Range range, float x, float y, float z)
		{
			graphics.Text(chars, range, x, y, z);
		}

		public void Text<T>(T num, float x, float y)
		{
			graphics.Text(num, x, y);
		}

		public void Text<T>(T num, float x, float y, float z)
		{
			graphics.Text(num, x, y, z);
		}

		public void Text(string text, float x1, float y1, float x2, float y2)
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

		public void TextLeading(float leading)
		{
			graphics.TextLeading(leading);
		}

		public void TextMode(int mode)
		{
			graphics.TextMode(mode);
		}

		public void TextSize(float size)
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

		public float TextWidth(char c)
		{
			return graphics.TextWidth(c);
		}

		public float TextWidth(char[] chars, int start, int length)
		{
			return graphics.TextWidth(chars, start, length);
		}

		public float TextWidth(string text)
		{
			return TextWidth(text);
		}

		public void Translate(float x, float y)
		{
			graphics.Translate(x, y);
		}

		public void Translate(float x, float y, float z)
		{
			graphics.Translate(x, y, z);
		}

		public void Triangle(float x1, float y1, float x2, float y2, float x3, float y3)
		{
			graphics.Triangle(x1, y1, x2, y2, x3, y3);
		}

		public void Vertex(float x, float y)
		{
			graphics.Vertex(x, y);
		}

		public void Vertex(float x, float y, float z)
		{
			graphics.Vertex(x, y, z);
		}

		public void Vertex(float x, float y, float z, float u, float v)
		{
			graphics.Vertex(x, y, z, u, v);
		}

		public void Vertex(float[] v)
		{
			graphics.Vertex(v);
		}

		public void DrawShape(Span<float> vertecies)
		{
			graphics.DrawShape(vertecies);
		}
	}
}