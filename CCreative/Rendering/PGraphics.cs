using System;
// ReSharper disable InconsistentNaming
#pragma warning disable 1591

namespace CCreative.Rendering
{
	/// <summary>
	/// Main graphics and rendering context
	/// </summary>
	public interface PGraphics : Image
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
		void Background(Image image)
		{
			Clear();
			Image(image, 0, 0);
		}

		/// <summary>
		/// Sets the background of the window and erases all drawings on the screen
		/// </summary>
		/// <param name="gray">specifies a value between white and black</param>
		/// <returns>the window background color</returns>
		public Color Background(float gray)
		{
			return Background(Color(gray));
		}

		/// <summary>
		/// Sets the background of the window and erases all drawings on the screen
		/// </summary>
		/// <param name="gray">specifies a value between white and black</param>
		/// <param name="alpha">the opacity of the background</param>
		/// <returns>the window background color</returns>
		public Color Background(float gray, float alpha)
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
		public Color Background(float v1, float v2, float v3)
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
		public Color Background(float v1, float v2, float v3, float alpha)
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
		public void ColorMode(ColorModes mode, float max)
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
		void ColorMode(ColorModes mode, float max1, float max2, float max3);

		/// <summary>
		/// Changes the way CCreative interprets color data
		/// </summary>
		/// <param name="mode">Either <see cref="PConstants.RGB"/> or <see cref="PConstants.RGB"/>, corresponding to Red/Green/Blue and Hue/Saturation/Brightness</param>
		/// <param name="max1">range for the red or hue depending on the current color mode</param>
		/// <param name="max2">range for the green or saturation depending on the current color mode</param>
		/// <param name="max3">range for the blue or brightness depending on the current color mode</param>
		/// <param name="maxA">range for the alpha</param>
		void ColorMode(ColorModes mode, float max1, float max2, float max3, float maxA);

		/// <summary>
		/// Creates a grayscale color
		/// </summary>
		/// <param name="gray">number specifying value between white and black</param>
		/// <returns>the created color</returns>
		Color Color(float gray);

		/// <summary>
		/// Creates a new color based on <paramref name="color"/> with a given transparency
		/// </summary>
		/// <param name="color">the base color to change the alpha value of</param>
		/// <param name="alpha">the opacity of the color</param>
		/// <returns>a new color based from <paramref name="color"/> with the given alpha</returns>
		Color Color(Color color, float alpha);

		/// <summary>
		/// Creates a grayscale color with a given transparency
		/// </summary>
		/// <param name="gray">number specifying value between white and black</param>
		/// <param name="alpha">the opacity of the color</param>
		/// <returns>a new grayscale color with the given alpha</returns>
		Color Color(float gray, float alpha);

		/// <summary>
		/// Creates a color from RGB values or HSB value based on the given colormode, use <see cref="ColorMode(ColorModes, float)"/> to change the colormode
		/// </summary>
		/// <param name="v1">the red or hue value of the color</param>
		/// <param name="v2">the green of saturation value of the color</param>
		/// <param name="v3">the red or brightness value of the color</param>
		/// <returns>the created color</returns>
		Color Color(float v1, float v2, float v3);

		/// <summary>
		/// Creates a color from RGB values or HSB value based on the given colormode, use <see cref="ColorMode(ColorModes, float)"/> to change the colormode
		/// </summary>
		/// <param name="v1">the red or hue value of the color</param>
		/// <param name="v2">the green of saturation value of the color</param>
		/// <param name="v3">the red or brightness value of the color</param>
		/// <param name="a">the opacity of the color</param>
		/// <returns>the created color</returns>
		Color Color(float v1, float v2, float v3, float a);

		/// <summary>
		/// Returns the alpha (transparency) value of the color
		/// </summary>
		/// <param name="color">the color to get the alpha (transparency) value of</param>
		/// <returns>the opacity value of the color</returns>
		float Alpha(Color color) => color.G;

		/// <summary>
		/// Returns the red component of the color
		/// </summary>
		/// <param name="color">the color to get the red component of</param>
		/// <returns>the red component of the color</returns>
		float Red(Color color) => color.R;

		/// <summary>
		/// Returns the green component of the color
		/// </summary>
		/// <param name="color">the color to get the green component of</param>
		/// <returns>the green component of the color</returns>
		float Green(Color color) => color.G;

		/// <summary>
		/// Returns the blue value of the color
		/// </summary>
		/// <param name="color">the color to get the blue value of</param>
		/// <returns>the blue value of the color</returns>
		float Blue(Color color) => color.B;

		/// <summary>
		/// Returns the hue component of the color
		/// </summary>
		/// <param name="color">the color to get the hue component of</param>
		/// <returns>the hue component of the color</returns>
		float Hue(Color color)
		{
			var (r, g, b) = color;
			byte min;
			byte max;
			
			if (r == g && g == b)
				return 0f;
 
			if (r > g)
			{
				max = r;
				min = g;
			}
			else
			{
				max = g;
				min = r;
			}
			
			if (b > max)
			{
				max = b;
			}
			else if (b < min)
			{
				min = b;
			}
 
			float delta = max - min;
			float hue;
 
			if (r == max)
				hue = (g - b) / delta;
			else if (g == max)
				hue = (b - r) / delta + 2f;
			else
				hue = (r - g) / delta + 4f;
 
			hue *= 60f;
			if (hue < 0f)
				hue += 360f;
 
			return hue;
		}

		/// <summary>
		/// Returns the saturation component of the color
		/// </summary>
		/// <param name="color">the color to get the saturation component of</param>
		/// <returns>the saturation component of the color</returns>
		float Saturation(Color color)
		{
			var (r, g, b) = color;
			byte min;
			byte max;
			
			if (r == g && g == b)
				return 0f;
 
			if (r > g)
			{
				max = r;
				min = g;
			}
			else
			{
				max = g;
				min = r;
			}
			
			if (b > max)
			{
				max = b;
			}
			else if (b < min)
			{
				min = b;
			}
 
			var div = max + min;
			
			if (div > byte.MaxValue)
				div = byte.MaxValue * 2 - max - min;
 
			return (max - min) / (float)div;
		}

		/// <summary>
		/// Returns the brightness component of the color
		/// </summary>
		/// <param name="color">the color to get the brightness component of</param>
		/// <returns>the brightness component of the color</returns>
		float Brightness(Color color)
		{
			var (r, g, b) = color;
			byte min;
			byte max;
 
			if (r > g)
			{
				max = r;
				min = g;
			}
			else
			{
				max = g;
				min = r;
			}
			
			if (b > max)
			{
				max = b;
			}
			else if (b < min)
			{
				min = b;
			}

			return (max + min) / (Byte.MaxValue * 2f);
		}

		/// <summary>
		/// Gets the contrast color (black or white) of the given color
		/// </summary>
		/// <param name="color">the color to calculate the contrast color of</param>
		/// <returns>the contrast color</returns>
		Color ContrastColor(Color color)
		{
			var (r, g, b) = color;

			var l = 0.2126 * r + 0.7152 * g + 0.0722 * b;

			return l < 0.5 ? Color(Byte.MaxValue) : Color(Byte.MinValue);
		}

		/// <summary>
		/// Calculates a color between two colors at a specific increment
		/// </summary>
		/// <param name="c1">the first color</param>
		/// <param name="c2">the second color</param>
		/// <param name="amt">the specified increment between 0 and 1</param>
		/// <returns>the interpolated color</returns>
		Color LerpColor(Color c1, Color c2, float amt)
		{
			var (r1, g1, b1) = c1;
			var (r2, g2, b2) = c2;

			var r = Math.Lerp(r1, r2, amt);
			var g = Math.Lerp(g1, g2, amt);
			var b = Math.Lerp(b1, b2, amt);

			return Color(r, g, b);
		}

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
		public Color Stroke(float gray)
		{
			return Stroke(Color(gray));
		}

		/// <summary>
		/// Sets the color used to draw lines and borders around shapes
		/// </summary>
		/// <param name="gray">number specifying value between white and black</param>
		/// <param name="alpha">the alpha (transparent) component of the new stroke color</param>
		/// <returns>the new stroke color</returns>
		public Color Stroke(float gray, float alpha)
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
		public Color Stroke(float v1, float v2, float v3)
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
		public Color Stroke(float v1, float v2, float v3, float alpha)
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
		public Color Fill(float gray)
		{
			return Fill(Color(gray));
		}

		/// <summary>
		/// Sets the color used to fill shapes
		/// </summary>
		/// <param name="gray">number specifying value between white and black</param>
		/// <param name="alpha">the opacity of the color</param>
		/// <returns>the new fill color</returns>
		public Color Fill(float gray, float alpha)
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
		public Color Fill(float v1, float v2, float v3)
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
		public Color Fill(float v1, float v2, float v3, float alpha)
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
		public Color Tint(float gray)
		{
			return Tint(Color(gray));
		}

		/// <summary>
		/// Sets the fill value for displaying images
		/// </summary>
		/// <param name="gray">specifies a value between white and black</param>
		/// <param name="alpha">the opacity of the images</param>
		/// <returns></returns>
		public Color Tint(float gray, float alpha)
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
		public Color Tint(float v1, float v2, float v3)
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
		public Color Tint(float v1, float v2, float v3, float alpha)
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
		public void Ambient(float gray)
		{
			Ambient(Color(gray));
		}

		/// <summary>
		/// Sets the ambient reflectance for shapes drawn to the screen
		/// </summary>
		/// <param name="v1">the red or hue value of the ambient reflectance color</param>
		/// <param name="v2">the green of saturation value of the ambient reflectance color</param>
		/// <param name="v3">the red or brightness value of the ambient reflectance color</param>
		public void Ambient(float v1, float v2, float v3)
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
		public void AmbientLight(float v1, float v2, float v3)
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
		void ApplyMatrix(float n00, float n01, float n02, float n10, float n11, float n12);

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
		void ApplyMatrix(float n00, float n01, float n02, float n03, float n10, float n11, float n12, float n13, float n20, float n21, float n22, float n23, float n30, float n31, float n32, float n33);

		/// <summary>
		/// Multiplies the current matrix by the one specified through the parameter
		/// </summary>
		/// <param name="matrix">the matrix to multiply the current with</param>
		void ApplyMatrix(Matrix matrix);
		
		/// <summary>
		/// Draws an arc to the screen
		/// </summary>
		/// <param name="a">x-coordinate of the arc's ellipse</param>
		/// <param name="b">y-coordinate of the arc's ellipse</param>
		/// <param name="c">width of the arc's ellipse by default</param>
		/// <param name="d">height of the arc's ellipse by default</param>
		/// <param name="start">angle to start the arc, specified in radians</param>
		/// <param name="stop">angle to stop the arc, specified in radians</param>
		void Arc(float a, float b, float c, float d, float start, float stop);

		/// <summary>
		/// The beginCamera() and endCamera() functions enable advanced customization of the camera space
		/// </summary>
		void BeginCamera();

		void BeginContour();

		void BeginDraw();

		void BeginShape();
		void BeginShape(ShapeTypes type);

		void Bezier(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4);
		void Bezier(float x1, float y1, float z1, float x2, float y2, float z2, float x3, float y3, float z3, float x4, float y4, float z4);

		void BezierDetail(int detail);

		float BezierPoint(float a, float b, float c, float d, float t);

		float BezierTangent(float a, float b, float c, float d, float t);

		void BezierVertex(float x2, float y2, float x3, float y3, float x4, float y4);
		void BezierVertex(float x2, float y2, float z2, float x3, float y3, float z3, float x4, float y4, float z4);

		void BlendMode(int mode);

		public void Box(float size)
		{
			Box(size, size, size);
		}

		void Box(float w, float h, float d);

		void Camera();
		void Camera(float eyeX, float eyeY, float eyeZ, float centerX, float centerY, float centerZ, float upX, float upY, float upZ);

		void Circle(float x, float y, float extent) => Ellipse(x, y, extent, extent);

		void Clear();

		void Clip(int a, int b, int c, int d);

		Image CreateImage(int width, int height);
		Image? LoadImage(string path);

		void Curve(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4);
		void Curve(float x1, float y1, float z1, float x2, float y2, float z2, float x3, float y3, float z3, float x4, float y4, float z4);

		void CurveDetail(int detail);

		void CurvePoint(float a, float b, float c, float d, float t);

		void CurveTangent(float a, float b, float c, float d, float t);

		void CurveTightness(float tightness);

		void CurveVertex(float x, float y);
		void CurveVertex(float x, float y, float z);

		void DirectionalLight(float v1, float v2, float v3, float nx, float ny, float nz);

		bool Displayable();

		void Edge(bool edge);

		void Ellipse(float a, float b, float c, float d);

		void EllipseMode(int mode);

		void Emissive(float gray);
		void Emissive(float v1, float v2, float v3);
		void Emissive(Color color);

		void EndCamera();

		void EndContour();

		void EndDraw();

		void EndShape(CloseType close);

		void Flush();

		void Frustum(float left, float right, float bottom, float top, float near, float far);

		Matrix GetMatrix();
		Matrix2D GetMatrix(Matrix2D target);
		Matrix3D GetMatrix(Matrix3D target);

		PStyle GetStyle();
		PStyle GetStyle(PStyle target);

		void Hint(int which);

		void Image(Image img, float a, float b);
		void Image(Image img, float a, float b, float c, float d);
		void Image(Image img, float a, float b, float c, float d, int u1, int v1, int u2, int v2);

		void ImageMode(int mode);

		bool Is2D();
		bool Is3D();
		bool IsGL();

		void LightFalloff(float constant, float linear, float quadratic);

		void Lights();

		void LightSpecular(float v1, float v2, float v3);

		void Line(float x1, float y1, float x2, float y2);
		void Line(float x1, float y1, float z1, float x2, float y2, float z2);

		float ModelX(float x, float y, float z);
		float ModelY(float x, float y, float z);
		float ModelZ(float x, float y, float z);

		void NoClip();

		void NoLights();

		void Normal(float nx, float ny, float nz);

		void NoSmooth();

		void NoTexture();

		void Ortho();
		void Ortho(float left, float right, float bottom, float top);
		void Ortho(float left, float right, float bottom, float top, float near, float far);

		void Perspective();
		void Perspective(float fovY, float aspect, float zNear, float zFar);

		void Point(float x, float y);
		void Point(float x, float y, float z);

		void PointLight(float v1, float v2, float v3, float x, float y, float z);

		void Pop();
		void PopMatrix();
		void PopStyle();

		void PrintCamera();
		void PrintMatrix();
		void PrintProjection();

		void Push();
		void PushMatrix();
		void PushStyle();

		void Quad(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4)
		{
			Span<float> span = stackalloc float[] { x1, y1, x2, y2, x3, y3, x4, y4 };

			DrawShape(span);
		}

		void QuadraticVertex(float cx, float cy, float x3, float y3);
		void QuadraticVertex(float cx, float cy, float cz, float x3, float y3, float z3);

		void Rect(float a, float b, float c, float d);
		void Rect(float a, float b, float c, float d, float r);
		void Rect(float a, float b, float c, float d, float tl, float tr, float br, float bl);

		void RectMode(int mode);

		void ResetMatrix();
		void ResetShader();
		void ResetShader(int kind);

		void Rotate(float angle, float x, float y, float z);

		void RotateX(float angle);
		void RotateY(float angle);
		void RotateZ(float angle);

		void Scale(float s);
		void Scale(float x, float y);
		void Scale(float x, float y, float z);

		float ScreenX(float x, float y);
		float ScreenX(float x, float y, float z);

		float ScreenY(float x, float y);
		float ScreenY(float x, float y, float z);

		float ScreenZ(float x, float y);
		float ScreenZ(float x, float y, float z);

		void SetMatrix(Matrix source);
		void SetMatrix(Matrix2D source);
		void SetMatrix(Matrix3D source);

		void SetPath(string path);
		void SetPrimary(bool primary);
		void SetSize(int w, int h);

		void ShapeMode(int kind);

		void ShearX(float angle);
		void ShearY(float angle);

		void Shininess(float shine);

		void Smooth();
		void Smooth(int quality);

		void Specular(float gray);
		void Specular(float v1, float v2, float v3);
		void Specular(Color color);

		void Sphere(float r);

		void SphereDetail(int res);
		void SphereDetail(int ures, int vres);

		void SpotLight(float v1, float v2, float v3, float x, float y, float z, float nx, float ny, float nz, float angle, float concentration);

		void Square(float x, float y, float extent) => Rect(x, y, extent, extent);

		void StrokeCap(int cap);

		void StrokeJoin(int join);

		void StrokeWeight(float weight);

		void Style(PStyle style);

		void Text(string text, Range range, float x, float y);
		void Text(string text, Range range, float x, float y, float z);

		void Text(char[] chars, Range range, float x, float y);
		void Text(char[] chars, Range range, float x, float y, float z);

		void Text<T>(T num, float x, float y);
		void Text<T>(T num, float x, float y, float z);

		void Text(string text, float x1, float y1, float x2, float y2);

		void TextAlign(int alignX);
		void TextAlign(int alignX, int alignY);

		float TextAcent();
		float TextDecent();

		void TextFont(PFont which);
		void TextFont(PFont which, int size);

		void TextLeading(float leading);
		void TextMode(int mode);
		void TextSize(float size);

		void Texture(Image texture);
		void TextureMode(int mode);
		void TextureWrap(int wrap);

		float TextWidth(char c);
		float TextWidth(char[] chars, int start, int length);
		float TextWidth(string text);

		void Translate(float x, float y);
		void Translate(float x, float y, float z);

		void Triangle(float x1, float y1, float x2, float y2, float x3, float y3)
		{
			Span<float> span = stackalloc float[] { x1, y1, x2, y2, x3, y3 };

			DrawShape(span);
		}

		void Vertex(float x, float y);
		void Vertex(float x, float y, float z);
		void Vertex(float x, float y, float z, float u, float v);
		void Vertex(float[] v);

		void DrawShape(Span<float> vertecies);
	}
}