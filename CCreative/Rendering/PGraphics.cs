using System;
// ReSharper disable InconsistentNaming
#pragma warning disable 1591

namespace CCreative.Rendering
{
	/// <summary>
	/// Main graphics and rendering context
	/// </summary>
	public interface PGraphics : PImage
	{
		#region Background

		/// <summary>
		/// Sets the background of the window and erases all drawings on the screen
		/// </summary>
		/// <remarks>
		/// The default background is light gray. This function is typically used within draw() to clear the display window at the beginning of each frame, but it can be used inside setup() to set the background on the first frame of animation or if the backgound need only be set once.
		/// </remarks>
		/// <param name="color">the background color</param>
		/// <returns>the window background color</returns>
		Color Background(Color color);

		/// <summary>
		/// Sets the background of the window to a image and erases all drawings on the screen
		/// </summary>
		/// <param name="image">PImage to set as background</param>
		void Background(PImage image);

		/// <summary>
		/// Sets the background of the window and erases all drawings on the screen
		/// </summary>
		/// <param name="gray">specifies a value between white and black</param>
		/// <returns>the window background color</returns>
		public Color Background(double gray)
		{
			return Background(Color(gray));
		}

		/// <summary>
		/// Sets the background of the window and erases all drawings on the screen
		/// </summary>
		/// <param name="gray">specifies a value between white and black</param>
		/// <param name="alpha">the opacity of the background</param>
		/// <returns>the window background color</returns>
		public Color Background(double gray, double alpha)
		{
			return Background(Color(gray, alpha));
		}

		/// <summary>
		/// Sets the background of the window and erases all drawings on the screen
		/// </summary>
		/// <param name="v1">the red or hue value of the background</param>
		/// <param name="v2">the green of saturation value of the background</param>
		/// <param name="v3">the red or brightness value of the background</param>
		/// <returns>the window background color</returns>
		public Color Background(double v1, double v2, double v3)
		{
			return Background(Color(v1, v2, v3));
		}

		/// <summary>
		/// Sets the background of the window and erases all drawings on the screen
		/// </summary>
		/// <param name="v1">the red or hue value of the background</param>
		/// <param name="v2">the green of saturation value of the background</param>
		/// <param name="v3">the red or brightness value of the background</param>
		/// <param name="alpha">the opacity of the background</param>
		/// <returns>the window background color</returns>
		public Color Background(double v1, double v2, double v3, double alpha)
		{
			return Background(Color(v1, v2, v3, alpha));
		}

		#endregion

		#region Color

		/// <summary>
		/// Changes the way CCreative interprets color data
		/// </summary>
		/// <param name="mode">Either <see cref="PConstants.RGB"/> or <see cref="PConstants.RGB"/>, corresponding to Red/Green/Blue and Hue/Saturation/Brightness</param>
		/// <param name="max">range for all color elements</param>
		public void ColorMode(ColorModes mode, double max)
		{
			ColorMode(mode, max, max, max, max);
		}

		/// <summary>
		/// Changes the way CCreative interprets color data
		/// </summary>
		/// <param name="mode">Either <see cref="PConstants.RGB"/> or <see cref="PConstants.RGB"/>, corresponding to Red/Green/Blue and Hue/Saturation/Brightness</param>
		/// <param name="max1">range for the red or hue depending on the current color mode</param>
		/// <param name="max2">range for the green or saturation depending on the current color mode</param>
		/// <param name="max3">range for the blue or brightness depending on the current color mode</param>
		void ColorMode(ColorModes mode, double max1, double max2, double max3);

		/// <summary>
		/// Changes the way CCreative interprets color data
		/// </summary>
		/// <param name="mode">Either <see cref="PConstants.RGB"/> or <see cref="PConstants.RGB"/>, corresponding to Red/Green/Blue and Hue/Saturation/Brightness</param>
		/// <param name="max1">range for the red or hue depending on the current color mode</param>
		/// <param name="max2">range for the green or saturation depending on the current color mode</param>
		/// <param name="max3">range for the blue or brightness depending on the current color mode</param>
		/// <param name="maxA">range for the alpha</param>
		void ColorMode(ColorModes mode, double max1, double max2, double max3, double maxA);

		/// <summary>
		/// Creates a grayscale color
		/// </summary>
		/// <param name="gray">number specifying value between white and black</param>
		/// <returns>the created color</returns>
		Color Color(double gray);

		/// <summary>
		/// Creates a new color based on <paramref name="color"/> with a given transparency
		/// </summary>
		/// <param name="color">the base color to change the alpha value of</param>
		/// <param name="alpha">the opacity of the color</param>
		/// <returns>a new color based from <paramref name="color"/> with the given alpha</returns>
		Color Color(Color color, double alpha);

		/// <summary>
		/// Creates a grayscale color with a given transparency
		/// </summary>
		/// <param name="gray">number specifying value between white and black</param>
		/// <param name="alpha">the opacity of the color</param>
		/// <returns>a new grayscale color with the given alpha</returns>
		Color Color(double gray, double alpha);

		/// <summary>
		/// Creates a color from RGB values or HSB value based on the given colormode, use <see cref="ColorMode(ColorModes, double)"/> to change the colormode
		/// </summary>
		/// <param name="v1">the red or hue value of the color</param>
		/// <param name="v2">the green of saturation value of the color</param>
		/// <param name="v3">the red or brightness value of the color</param>
		/// <returns>the created color</returns>
		Color Color(double v1, double v2, double v3);

		/// <summary>
		/// Creates a color from RGB values or HSB value based on the given colormode, use <see cref="ColorMode(ColorModes, double)"/> to change the colormode
		/// </summary>
		/// <param name="v1">the red or hue value of the color</param>
		/// <param name="v2">the green of saturation value of the color</param>
		/// <param name="v3">the red or brightness value of the color</param>
		/// <param name="a">the opacity of the color</param>
		/// <returns>the created color</returns>
		Color Color(double v1, double v2, double v3, double a);

		/// <summary>
		/// Returns the alpha (transparency) value of the color
		/// </summary>
		/// <param name="color">the color to get the alpha (transparency) value of</param>
		/// <returns>the opacity value of the color</returns>
		double Alpha(Color color);

		/// <summary>
		/// Returns the red component of the color
		/// </summary>
		/// <param name="color">the color to get the red component of</param>
		/// <returns>the red component of the color</returns>
		double Red(Color color);

		/// <summary>
		/// Returns the green component of the color
		/// </summary>
		/// <param name="color">the color to get the green component of</param>
		/// <returns>the green component of the color</returns>
		double Green(Color color);

		/// <summary>
		/// Returns the blue value of the color
		/// </summary>
		/// <param name="color">the color to get the blue value of</param>
		/// <returns>the blue value of the color</returns>
		double Blue(Color color);

		/// <summary>
		/// Returns the hue component of the color
		/// </summary>
		/// <param name="color">the color to get the hue component of</param>
		/// <returns>the hue component of the color</returns>
		double Hue(Color color);

		/// <summary>
		/// Returns the saturation component of the color
		/// </summary>
		/// <param name="color">the color to get the saturation component of</param>
		/// <returns>the saturation component of the color</returns>
		double Saturation(Color color);

		/// <summary>
		/// Returns the brightness component of the color
		/// </summary>
		/// <param name="color">the color to get the brightness component of</param>
		/// <returns>the brightness component of the color</returns>
		double Brightness(Color color);

		/// <summary>
		/// Gets the contrast color (black or white) of the given color
		/// </summary>
		/// <param name="color">the color to calculate the contrast color of</param>
		/// <returns>the contrast color</returns>
		Color ContrastColor(Color color);

		/// <summary>
		/// Calculates a color between two colors at a specific increment
		/// </summary>
		/// <param name="c1">the first color</param>
		/// <param name="c2">the second color</param>
		/// <param name="amt">the specified increment between 0 and 1</param>
		/// <returns>the interpolated color</returns>
		Color LerpColor(Color c1, Color c2, double amt);

		#endregion

		#region Stroke

		/// <summary>
		/// Disables drawing the stroke (outline)
		/// </summary>
		void NoStroke();

		/// <summary>
		/// Sets the color used to draw lines and borders around shapes
		/// </summary>
		/// <param name="color">the color to use</param>
		/// <returns>the new stroke color</returns>
		Color Stroke(Color color);

		/// <summary>
		/// Sets the color used to draw lines and borders around shapes
		/// </summary>
		/// <param name="gray">number specifying value between white and black</param>
		/// <returns>the new stroke color</returns>
		public Color Stroke(double gray)
		{
			return Stroke(Color(gray));
		}

		/// <summary>
		/// Sets the color used to draw lines and borders around shapes
		/// </summary>
		/// <param name="gray">number specifying value between white and black</param>
		/// <param name="alpha">the alpha (transparent) component of the new stroke color</param>
		/// <returns>the new stroke color</returns>
		public Color Stroke(double gray, double alpha)
		{
			return Stroke(Color(gray, alpha));
		}

		/// <summary>
		/// Sets the color used to draw lines and borders around shapes
		/// </summary>
		/// <param name="v1">the red or hue value of the new stroke color</param>
		/// <param name="v2">the green of saturation value of the new stroke color</param>
		/// <param name="v3">the red or brightness value of the new stroke color</param>
		/// <returns>the new stroke color</returns>
		public Color Stroke(double v1, double v2, double v3)
		{
			return Stroke(Color(v1, v2, v3));
		}

		/// <summary>
		/// Sets the color used to draw lines and borders around shapes
		/// </summary>
		/// <param name="v1">the red or hue value of the new stroke color</param>
		/// <param name="v2">the green of saturation value of the new stroke color</param>
		/// <param name="v3">the red or brightness value of the new color</param>
		/// <param name="alpha">the alpha (transparency) of the new stroke color</param>
		/// <returns></returns>
		public Color Stroke(double v1, double v2, double v3, double alpha)
		{
			return Stroke(Color(v1, v2, v3, alpha));
		}

		#endregion

		#region Fill

		/// <summary>
		/// Disables filling geometry
		/// </summary>
		public void NoFill();

		/// <summary>
		/// Sets the color used to fill shapes
		/// </summary>
		/// <param name="color">the new stroke </param>
		/// <returns>the new fill color</returns>
		public Color Fill(Color color);

		/// <summary>
		/// Sets the color used to fill shapes
		/// </summary>
		/// <param name="gray">number specifying value between white and black</param>
		/// <returns>the new fill color</returns>
		public Color Fill(double gray)
		{
			return Fill(Color(gray));
		}

		/// <summary>
		/// Sets the color used to fill shapes
		/// </summary>
		/// <param name="gray">number specifying value between white and black</param>
		/// <param name="alpha">the opacity of the color</param>
		/// <returns>the new fill color</returns>
		public Color Fill(double gray, double alpha)
		{
			return Fill(Color(gray, alpha));
		}

		/// <summary>
		/// Sets the color used to fill shapes
		/// </summary>
		/// <param name="v1">the red or hue value of the fill color</param>
		/// <param name="v2">the green of saturation value of the fill color</param>
		/// <param name="v3">the red or brightness value of the fill color</param>
		/// <returns>the new fill color</returns>
		public Color Fill(double v1, double v2, double v3)
		{
			return Fill(Color(v1, v2, v3));
		}

		/// <summary>
		/// Sets the color used to fill shapes
		/// </summary>
		/// <param name="v1">the red or hue value of the fill color</param>
		/// <param name="v2">the green of saturation value of the fill color</param>
		/// <param name="v3">the red or brightness value of the fill color</param>
		/// <param name="alpha">the opacity of the fill color</param>
		/// <returns></returns>
		public Color Fill(double v1, double v2, double v3, double alpha)
		{
			return Fill(Color(v1, v2, v3, alpha));
		}

		#endregion

		#region Tint

		/// <summary>
		/// Removes the current fill value for displaying images and reverts to displaying images with their original hues
		/// </summary>
		void NoTint();

		/// <summary>
		/// Sets the fill value for displaying images
		/// </summary>
		/// <param name="color">the new tint color</param>
		/// <returns>the new tint color</returns>
		Color Tint(Color color);

		/// <summary>
		/// Sets the fill value for displaying images
		/// </summary>
		/// <param name="gray">specifies a value between white and black</param>
		/// <returns>the new tint color</returns>
		public Color Tint(double gray)
		{
			return Tint(Color(gray));
		}

		/// <summary>
		/// Sets the fill value for displaying images
		/// </summary>
		/// <param name="gray">specifies a value between white and black</param>
		/// <param name="alpha">the opacity of the images</param>
		/// <returns></returns>
		public Color Tint(double gray, double alpha)
		{
			return Tint(Color(gray, alpha));
		}

		/// <summary>
		/// Sets the fill value for displaying images
		/// </summary>
		/// <param name="v1">the red or hue value of the tint color</param>
		/// <param name="v2">the green of saturation value of the tint color</param>
		/// <param name="v3">the red or brightness value of the tint color</param>
		/// <returns></returns>
		public Color Tint(double v1, double v2, double v3)
		{
			return Tint(Color(v1, v2, v3));
		}

		/// <summary>
		/// Sets the fill value for displaying images
		/// </summary>
		/// <param name="v1">the red or hue value of the tint color</param>
		/// <param name="v2">the green of saturation value of the tint color</param>
		/// <param name="v3">the red or brightness value of the tint color</param>
		/// <param name="alpha">the opacity of the images</param>
		/// <returns></returns>
		public Color Tint(double v1, double v2, double v3, double alpha)
		{
			return Tint(Color(v1, v2, v3, alpha));
		}

		#endregion

		/// <summary>
		/// Sets the ambient reflectance for shapes drawn to the screen
		/// </summary>
		/// <param name="color">the reflectance color</param>
		void Ambient(Color color);

		/// <summary>
		/// Sets the ambient reflectance for shapes drawn to the screen
		/// </summary>
		/// <param name="gray">number specifying value between white and black</param>
		public void Ambient(double gray)
		{
			Ambient(Color(gray));
		}

		/// <summary>
		/// Sets the ambient reflectance for shapes drawn to the screen
		/// </summary>
		/// <param name="v1">the red or hue value of the ambient reflectance color</param>
		/// <param name="v2">the green of saturation value of the ambient reflectance color</param>
		/// <param name="v3">the red or brightness value of the ambient reflectance color</param>
		public void Ambient(double v1, double v2, double v3)
		{
			Ambient(Color(v1, v2, v3));
		}

		/// <summary>
		/// Adds an ambient light
		/// </summary>
		/// <param name="color">the color of the ambient light</param>
		void AmbientLight(Color color);

		/// <summary>
		/// Adds an ambient light
		/// </summary>
		/// <remarks>
		/// Ambient light doesn't come from a specific direction, the rays of light have bounced around so much that objects are evenly lit from all sides
		/// </remarks>
		/// <param name="v1">the red or hue value of the ambient color</param>
		/// <param name="v2">the green of saturation value of the ambient color</param>
		/// <param name="v3">the red or brightness value of the ambient color</param>
		public void AmbientLight(double v1, double v2, double v3)
		{
			AmbientLight(Color(v1, v2, v3));
		}

		/// <summary>
		/// Multiplies the current matrix by the one specified through the parameters
		/// </summary>
		/// <param name="n00">numbers which define the 4x4 matrix to be multiplied</param>
		/// <param name="n01">numbers which define the 4x4 matrix to be multiplied</param>
		/// <param name="n02">numbers which define the 4x4 matrix to be multiplied</param>
		/// <param name="n10">numbers which define the 4x4 matrix to be multiplied</param>
		/// <param name="n11">numbers which define the 4x4 matrix to be multiplied</param>
		/// <param name="n12">numbers which define the 4x4 matrix to be multiplied</param>
		void ApplyMatrix(double n00, double n01, double n02, double n10, double n11, double n12);

		/// <summary>
		/// Multiplies the current matrix by the one specified through the parameters
		/// </summary>
		/// <param name="n00">numbers which define the 4x4 matrix to be multiplied</param>
		/// <param name="n01">numbers which define the 4x4 matrix to be multiplied</param>
		/// <param name="n02">numbers which define the 4x4 matrix to be multiplied</param>
		/// <param name="n03">numbers which define the 4x4 matrix to be multiplied</param>
		/// <param name="n10">numbers which define the 4x4 matrix to be multiplied</param>
		/// <param name="n11">numbers which define the 4x4 matrix to be multiplied</param>
		/// <param name="n12">numbers which define the 4x4 matrix to be multiplied</param>
		/// <param name="n13">numbers which define the 4x4 matrix to be multiplied</param>
		/// <param name="n20">numbers which define the 4x4 matrix to be multiplied</param>
		/// <param name="n21">numbers which define the 4x4 matrix to be multiplied</param>
		/// <param name="n22">numbers which define the 4x4 matrix to be multiplied</param>
		/// <param name="n23">numbers which define the 4x4 matrix to be multiplied</param>
		/// <param name="n30">numbers which define the 4x4 matrix to be multiplied</param>
		/// <param name="n31">numbers which define the 4x4 matrix to be multiplied</param>
		/// <param name="n32">numbers which define the 4x4 matrix to be multiplied</param>
		/// <param name="n33">numbers which define the 4x4 matrix to be multiplied</param>
		void ApplyMatrix(double n00, double n01, double n02, double n03, double n10, double n11, double n12, double n13, double n20, double n21, double n22, double n23, double n30, double n31, double n32, double n33);

		/// <summary>
		/// Multiplies the current matrix by the one specified through the parameter
		/// </summary>
		/// <param name="matrix">the matrix to multiply the current with</param>
		void ApplyMatrix(PMatrix matrix);
		
		/// <summary>
		/// Draws an arc to the screen
		/// </summary>
		/// <param name="a">x-coordinate of the arc's ellipse</param>
		/// <param name="b">y-coordinate of the arc's ellipse</param>
		/// <param name="c">width of the arc's ellipse by default</param>
		/// <param name="d">height of the arc's ellipse by default</param>
		/// <param name="start">angle to start the arc, specified in radians</param>
		/// <param name="stop">angle to stop the arc, specified in radians</param>
		void Arc(double a, double b, double c, double d, double start, double stop);

		/// <summary>
		/// The beginCamera() and endCamera() functions enable advanced customization of the camera space
		/// </summary>
		void BeginCamera();

		void BeginContour();

		void BeginDraw();

		void BeginShape();
		void BeginShape(ShapeTypes type);

		void Bezier(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4);
		void Bezier(double x1, double y1, double z1, double x2, double y2, double z2, double x3, double y3, double z3, double x4, double y4, double z4);

		void BezierDetail(int detail);

		double BezierPoint(double a, double b, double c, double d, double t);

		double BezierTangent(double a, double b, double c, double d, double t);

		void BezierVertex(double x2, double y2, double x3, double y3, double x4, double y4);
		void BezierVertex(double x2, double y2, double z2, double x3, double y3, double z3, double x4, double y4, double z4);

		void BlendMode(int mode);

		public void Box(double size)
		{
			Box(size, size, size);
		}

		void Box(double w, double h, double d);

		void Camera();
		void Camera(double eyeX, double eyeY, double eyeZ, double centerX, double centerY, double centerZ, double upX, double upY, double upZ);

		void Circle(double x, double y, double extent);

		void Clear();

		void Clip(int a, int b, int c, int d);

		PImage CreateImage(int width, int height);
		PImage? LoadImage(string path);

		void Curve(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4);
		void Curve(double x1, double y1, double z1, double x2, double y2, double z2, double x3, double y3, double z3, double x4, double y4, double z4);

		void CurveDetail(int detail);

		void CurvePoint(double a, double b, double c, double d, double t);

		void CurveTangent(double a, double b, double c, double d, double t);

		void CurveTightness(double tightness);

		void CurveVertex(double x, double y);
		void CurveVertex(double x, double y, double z);

		void DirectionalLight(double v1, double v2, double v3, double nx, double ny, double nz);

		bool Displayable();

		void Edge(bool edge);

		void Ellipse(double a, double b, double c, double d);

		void EllipseMode(int mode);

		void Emissive(double gray);
		void Emissive(double v1, double v2, double v3);
		void Emissive(Color color);

		void EndCamera();

		void EndContour();

		void EndDraw();

		void EndShape(CloseType close);

		void Flush();

		void Frustum(double left, double right, double bottom, double top, double near, double far);

		PMatrix GetMatrix();
		PMatrix2D GetMatrix(PMatrix2D target);
		PMatrix3D GetMatrix(PMatrix3D target);

		PStyle GetStyle();
		PStyle GetStyle(PStyle target);

		void Hint(int which);

		void Image(PImage img, double a, double b);
		void Image(PImage img, double a, double b, double c, double d);
		void Image(PImage img, double a, double b, double c, double d, int u1, int v1, int u2, int v2);

		void ImageMode(int mode);

		bool Is2D();
		bool Is3D();
		bool IsGL();

		void LightFalloff(double constant, double linear, double quadratic);

		void Lights();

		void LightSpecular(double v1, double v2, double v3);

		void Line(double x1, double y1, double x2, double y2);
		void Line(double x1, double y1, double z1, double x2, double y2, double z2);

		double ModelX(double x, double y, double z);
		double ModelY(double x, double y, double z);
		double ModelZ(double x, double y, double z);

		void NoClip();

		void NoLights();

		void Normal(double nx, double ny, double nz);

		void NoSmooth();

		void NoTexture();

		void Ortho();
		void Ortho(double left, double right, double bottom, double top);
		void Ortho(double left, double right, double bottom, double top, double near, double far);

		void Perspective();
		void Perspective(double fovY, double aspect, double zNear, double zFar);

		void Point(double x, double y);
		void Point(double x, double y, double z);

		void PointLight(double v1, double v2, double v3, double x, double y, double z);

		void Pop();
		void PopMatrix();
		void PopStyle();

		void PrintCamera();
		void PrintMatrix();
		void PrintProjection();

		void Push();
		void PushMatrix();
		void PushStyle();

		void Quad(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4);

		void QuadraticVertex(double cx, double cy, double x3, double y3);
		void QuadraticVertex(double cx, double cy, double cz, double x3, double y3, double z3);

		void Rect(double a, double b, double c, double d);
		void Rect(double a, double b, double c, double d, double r);
		void Rect(double a, double b, double c, double d, double tl, double tr, double br, double bl);

		void RectMode(int mode);

		void ResetMatrix();
		void ResetShader();
		void ResetShader(int kind);

		void Rotate(double angle, double x, double y, double z);

		void RotateX(double angle);
		void RotateY(double angle);
		void RotateZ(double angle);

		void Scale(double s);
		void Scale(double x, double y);
		void Scale(double x, double y, double z);

		double ScreenX(double x, double y);
		double ScreenX(double x, double y, double z);

		double ScreenY(double x, double y);
		double ScreenY(double x, double y, double z);

		double ScreenZ(double x, double y);
		double ScreenZ(double x, double y, double z);

		void SetMatrix(PMatrix source);
		void SetMatrix(PMatrix2D source);
		void SetMatrix(PMatrix3D source);

		void SetParent(PApplet parent);
		void SetPath(string path);
		void SetPrimary(bool primary);
		void SetSize(int w, int h);

		void ShapeMode(int kind);

		void ShearX(double angle);
		void ShearY(double angle);

		void Shininess(double shine);

		void Smooth();
		void Smooth(int quality);

		void Specular(double gray);
		void Specular(double v1, double v2, double v3);
		void Specular(Color color);

		void Sphere(double r);

		void SphereDetail(int res);
		void SphereDetail(int ures, int vres);

		void SpotLight(double v1, double v2, double v3, double x, double y, double z, double nx, double ny, double nz, double angle, double concentration);

		void Square(double x, double y, double extent);

		void StrokeCap(int cap);

		void StrokeJoin(int join);

		void StrokeWeight(double weight);

		void Style(PStyle style);

		void Text(string text, Index start, Index end, double x, double y);
		void Text(string text, Index start, Index end, double x, double y, double z);
		void Text(string text, Range range, double x, double y);
		void Text(string text, Range range, double x, double y, double z);

		void Text(char[] chars, Index start, Index end, double x, double y);
		void Text(char[] chars, Index start, Index end, double x, double y, double z);
		void Text(char[] chars, Range range, double x, double y);
		void Text(char[] chars, Range range, double x, double y, double z);

		void Text<T>(T num, double x, double y) where T : IConvertible, IFormattable;
		void Text<T>(T num, double x, double y, double z) where T : IConvertible, IFormattable;

		void Text(string text, double x1, double y1, double x2, double y2);

		void TextAlign(int alignX);
		void TextAlign(int alignX, int alignY);

		short TextAcent();
		short TextDecent();

		void TextFont(PFont which);
		void TextFont(PFont which, int size);

		void TextLeading(double leading);
		void TextMode(int mode);
		void TextSize(double size);

		void Texture(PImage texture);
		void TextureMode(int mode);
		void TextureWrap(int wrap);

		double TextWidth(char c);
		double TextWidth(char[] chars, int start, int length);
		double TextWidth(string text);

		void Translate(double x, double y);
		void Translate(double x, double y, double z);

		void Triangle(double x1, double y1, double x2, double y2, double x3, double y3);

		void Vertex(double x, double y);
		void Vertex(double x, double y, double z);
		void Vertex(double x, double y, double z, double u, double v);
		void Vertex(double[] v);
	}
}