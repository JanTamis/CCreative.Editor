using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace CCreative;

// https://gist.github.com/jackmott/38a26cd2934c23a161490044e64170c6
internal static class NoiseMaker
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static float Fade(float t)
	{
		return t * t * t * MathF.FusedMultiplyAdd(t, t * 6f - 15f, 10f);
	}

	private static readonly Vector<float> S6F = new(6.0f);
	private static readonly Vector<float> S15F = new(16.0f);
	private static readonly Vector<float> S10F = new(10.0f);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static Vector<float> FadeSimd(Vector<float> t)
	{
		return t * t * t * (t * (t * S6F - S15F) + S10F);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static Vector<float> LerpSimd(Vector<float> t, Vector<float> a, Vector<float> b)
	{
		return a + t * (b - a);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static Vector<float> FloorSimd(Vector<float> f)
	{
		var ft = (Vector<float>)(Vector<int>)f;
		return ft - Vector.BitwiseAnd(Vector.LessThan<float>(f, ft), Vector<float>.One);
	}

	//---------------------------------------------------------------------
	// Static data
	/*
   * Permutation table. This is just a random jumble of all numbers 0-255,
   * repeated twice to avoid wrapping the index at 255 for each lookup.
   * This needs to be exactly the same for all instances on all platforms,
   * so it's easiest to just keep it as static explicit data.
   * This also removes the need for any initialisation of this class.
   *
   * Note that making this an int[] instead of a char[] might make the
   * code run faster on platforms with a high penalty for unaligned single
   * byte addressing. Intel x86 is generally single-byte-friendly, but
   * some other CPUs are faster with 4-aligned reads.
   * However, a char[] is smaller, which avoids cache trashing, and that
   * is probably the most important aspect on most architectures.
   * This array is accessed a *lot* by the noise functions.
   * A vector-valued noise over 3D accesses it 96 times, and a
   * float-valued 4D noise 64 times. We want this to fit in the cache!
   */

	// C# no-alloc optimization that directly wraps the data section of the dll (similar to string constants)
	// https://github.com/dotnet/roslyn/pull/24621
	private static ReadOnlySpan<byte> _perm => new byte[]
	{
		151, 160, 137, 91, 90, 15,
		131, 13, 201, 95, 96, 53, 194, 233, 7, 225, 140, 36, 103, 30, 69, 142, 8, 99, 37, 240, 21, 10, 23,
		190, 6, 148, 247, 120, 234, 75, 0, 26, 197, 62, 94, 252, 219, 203, 117, 35, 11, 32, 57, 177, 33,
		88, 237, 149, 56, 87, 174, 20, 125, 136, 171, 168, 68, 175, 74, 165, 71, 134, 139, 48, 27, 166,
		77, 146, 158, 231, 83, 111, 229, 122, 60, 211, 133, 230, 220, 105, 92, 41, 55, 46, 245, 40, 244,
		102, 143, 54, 65, 25, 63, 161, 1, 216, 80, 73, 209, 76, 132, 187, 208, 89, 18, 169, 200, 196,
		135, 130, 116, 188, 159, 86, 164, 100, 109, 198, 173, 186, 3, 64, 52, 217, 226, 250, 124, 123,
		5, 202, 38, 147, 118, 126, 255, 82, 85, 212, 207, 206, 59, 227, 47, 16, 58, 17, 182, 189, 28, 42,
		223, 183, 170, 213, 119, 248, 152, 2, 44, 154, 163, 70, 221, 153, 101, 155, 167, 43, 172, 9,
		129, 22, 39, 253, 19, 98, 108, 110, 79, 113, 224, 232, 178, 185, 112, 104, 218, 246, 97, 228,
		251, 34, 242, 193, 238, 210, 144, 12, 191, 179, 162, 241, 81, 51, 145, 235, 249, 14, 239, 107,
		49, 192, 214, 31, 181, 199, 106, 157, 184, 84, 204, 176, 115, 121, 50, 45, 127, 4, 150, 254,
		138, 236, 205, 93, 222, 114, 67, 29, 24, 72, 243, 141, 128, 195, 78, 66, 215, 61, 156, 180,
		151, 160, 137, 91, 90, 15,
		131, 13, 201, 95, 96, 53, 194, 233, 7, 225, 140, 36, 103, 30, 69, 142, 8, 99, 37, 240, 21, 10, 23,
		190, 6, 148, 247, 120, 234, 75, 0, 26, 197, 62, 94, 252, 219, 203, 117, 35, 11, 32, 57, 177, 33,
		88, 237, 149, 56, 87, 174, 20, 125, 136, 171, 168, 68, 175, 74, 165, 71, 134, 139, 48, 27, 166,
		77, 146, 158, 231, 83, 111, 229, 122, 60, 211, 133, 230, 220, 105, 92, 41, 55, 46, 245, 40, 244,
		102, 143, 54, 65, 25, 63, 161, 1, 216, 80, 73, 209, 76, 132, 187, 208, 89, 18, 169, 200, 196,
		135, 130, 116, 188, 159, 86, 164, 100, 109, 198, 173, 186, 3, 64, 52, 217, 226, 250, 124, 123,
		5, 202, 38, 147, 118, 126, 255, 82, 85, 212, 207, 206, 59, 227, 47, 16, 58, 17, 182, 189, 28, 42,
		223, 183, 170, 213, 119, 248, 152, 2, 44, 154, 163, 70, 221, 153, 101, 155, 167, 43, 172, 9,
		129, 22, 39, 253, 19, 98, 108, 110, 79, 113, 224, 232, 178, 185, 112, 104, 218, 246, 97, 228,
		251, 34, 242, 193, 238, 210, 144, 12, 191, 179, 162, 241, 81, 51, 145, 235, 249, 14, 239, 107,
		49, 192, 214, 31, 181, 199, 106, 157, 184, 84, 204, 176, 115, 121, 50, 45, 127, 4, 150, 254,
		138, 236, 205, 93, 222, 114, 67, 29, 24, 72, 243, 141, 128, 195, 78, 66, 215, 61, 156, 180
	};

	//---------------------------------------------------------------------

	/*
   * Helper functions to compute gradients-dot-residualvectors (1D to 4D)
   * Note that these generate gradients of more than unit length. To make
   * a close match with the value range of classic Perlin noise, the final
   * noise values need to be rescaled. To match the RenderMan noise in a
   * statistical sense, the approximate scaling values (empirically
   * determined from test renderings) are:
   * 1D noise needs rescaling with 0.188
   * 2D noise needs rescaling with 0.507
   * 3D noise needs rescaling with 0.936
   * 4D noise needs rescaling with 0.87
   * Note that these noise functions are the most practical and useful
   * signed version of Perlin noise. To return values according to the
   * RenderMan specification from the SL noise() and pnoise() functions,
   * the noise values need to be scaled and offset to [0,1], like this:
   * float SLnoise = (noise3(x,y,z) + 1.0) * 0.5;
   */

	private static float Grad1(int h, float x)
	{
		h &= 15;
		var grad = 1.0f + (h & 7); // Gradient value 1.0, 2.0, ..., 8.0

		if ((h & 8) == 1)
			grad = -grad; // and a random sign for the gradient

		return grad * x; // Multiply the gradient with the distance
	}

	private static float Grad2(int h, float x, float y)
	{
		h &= 7; // Convert low 3 bits of hash code

		var u = h < 4 ? x : y; // into 8 simple gradient directions,
		var v = h < 4 ? y : x; // and compute the dot product with (x,y).

		if ((h & 1) == 1)
			u = -u;

		var n = 2.0f * v;

		if ((h & 2) == 1)
			n = -2.0f * v;

		return u + n;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static float Grad3(int h, float x, float y, float z)
	{
		h &= 15; // Convert low 4 bits of hash code into 12 simple

		var u = h < 8 ? x : y; // gradient directions, and compute dot product.
		var v = h < 4 ? y : h is 12 or 14 ? x : z; // Fix repeats at h = 12 to 15

		if ((h & 1) != 0)
			u = -u;

		if ((h & 2) != 0)
			v = -v;

		return u + v;
	}

	private static readonly Vector<int> S15 = new(15);
	private static readonly Vector<int> S14 = new(14);
	private static readonly Vector<int> S12 = new(12);
	private static readonly Vector<int> S8 = new(8);
	private static readonly Vector<int> S4 = new(4);
	private static readonly Vector<int> S2 = new(2);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static Vector<float> Grad3Simd(Vector<int> h, Vector<float> x, Vector<float> y, Vector<float> z)
	{
		h = Vector.BitwiseAnd(h, S15);
		var h1 = Vector.Equals(Vector<int>.Zero, Vector.BitwiseAnd(h, Vector<int>.One));
		var h2 = Vector.Equals(Vector<int>.Zero, Vector.BitwiseAnd(h, S2));

		var u = Vector.ConditionalSelect(Vector.LessThan(h, S8), x, y);

		var orh = Vector.BitwiseOr(Vector.Equals(h, S12), Vector.Equals(h, S14));
		var xorz = Vector.ConditionalSelect(orh, x, z);
		var v = Vector.ConditionalSelect(Vector.LessThan(h, S4), y, xorz);

		u = Vector.ConditionalSelect(h1, u, Vector<float>.Zero - u);
		v = Vector.ConditionalSelect(h2, v, Vector<float>.Zero - v);

		return u + v;
	}

	private static float Grad4(int h, float x, float y, float z, float t)
	{
		h &= 31; // Convert low 5 bits of hash code into 32 simple

		var u = h < 24 ? x : y; // gradient directions, and compute dot product.
		var v = h < 16 ? y : z;
		var w = h < 8 ? z : t;

		if ((h & 1) == 1)
			u = -u;

		if ((h & 2) == 1)
			v = -v;

		if ((h & 4) == 1)
			w = -w;

		return u + v + w;
	}

	//---------------------------------------------------------------------
	/** 1D float Perlin noise, SL "noise()" */
	public static float Noise1(float x)
	{
		int ix0, ix1;
		float fx0, fx1;
		float s;
		float n0, n1;

		ix0 = x > 0 ? (int)x : (int)x - 1; //FASTFLOOR(x); // Integer part of x
		fx0 = x - ix0; // Fractional part of x
		fx1 = fx0 - 1.0f;
		ix1 = (ix0 + 1) & 0xff;
		ix0 &= 0xff; // Wrap to 0..255

		s = Fade(fx0);

		n0 = Grad1(_perm[ix0], fx0);
		n1 = Grad1(_perm[ix1], fx1);
		return 0.188f * Math.Lerp(n0, n1, s);
	}

	//---------------------------------------------------------------------
	/** 1D float Perlin periodic noise, SL "pnoise()" */
	public static float Pnoise1(float x, int px)
	{
		int ix0, ix1;
		float fx0, fx1;
		float s;
		float n0, n1;

		ix0 = x > 0 ? (int)x : (int)x - 1; //FASTFLOOR(x); // Integer part of x
		fx0 = x - ix0; // Fractional part of x
		fx1 = fx0 - 1.0f;
		ix1 = ((ix0 + 1) % px) & 0xff; // Wrap to 0..px-1 *and* wrap to 0..255
		ix0 = (ix0 % px) & 0xff; // (because px might be greater than 256)

		s = Fade(fx0);

		n0 = Grad1(_perm[ix0], fx0);
		n1 = Grad1(_perm[ix1], fx1);
		return 0.188f * Math.Lerp(n0, n1, s);
	}

	//---------------------------------------------------------------------
	/** 2D float Perlin noise. */
	public static float Noise2(float x, float y)
	{
		int ix0, iy0, ix1, iy1;
		float fx0, fy0, fx1, fy1;
		float s, t;
		float nx0, nx1, n0, n1;

		ix0 = x > 0 ? (int)x : (int)(x - 1);
		iy0 = y > 0 ? (int)y : (int)(y - 1);
		fx0 = x - ix0; // Fractional part of x
		fy0 = y - iy0; // Fractional part of y
		fx1 = fx0 - 1.0f;
		fy1 = fy0 - 1.0f;
		ix1 = (ix0 + 1) & 0xff; // Wrap to 0..255
		iy1 = (iy0 + 1) & 0xff;
		ix0 &= 0xff;
		iy0 &= 0xff;

		t = Fade(fy0);
		s = Fade(fx0);

		nx0 = Grad2(_perm[ix0 + _perm[iy0]], fx0, fy0);
		nx1 = Grad2(_perm[ix0 + _perm[iy1]], fx0, fy1);
		n0 = Math.Lerp(nx0, nx1, t);

		nx0 = Grad2(_perm[ix1 + _perm[iy0]], fx1, fy0);
		nx1 = Grad2(_perm[ix1 + _perm[iy1]], fx1, fy1);
		n1 = Math.Lerp(nx0, nx1, t);

		return 0.507f * Math.Lerp(n0, n1, s);
	}

	//---------------------------------------------------------------------
	/** 2D float Perlin periodic noise. */
	public static float Pnoise2(float x, float y, int px, int py)
	{
		int ix0, iy0, ix1, iy1;
		float fx0, fy0, fx1, fy1;
		float s, t;
		float nx0, nx1, n0, n1;

		ix0 = x > 0 ? (int)x : (int)(x - 1);
		iy0 = y > 0 ? (int)y : (int)(y - 1);
		fx0 = x - ix0; // Fractional part of x
		fy0 = y - iy0; // Fractional part of y
		fx1 = fx0 - 1.0f;
		fy1 = fy0 - 1.0f;
		ix1 = ((ix0 + 1) % px) & 0xff; // Wrap to 0..px-1 and wrap to 0..255
		iy1 = ((iy0 + 1) % py) & 0xff; // Wrap to 0..py-1 and wrap to 0..255
		ix0 = (ix0 % px) & 0xff;
		iy0 = (iy0 % py) & 0xff;

		t = fy0 * fy0 * fy0 * (fy0 * (fy0 * 6 - 15) + 10);
		s = fx0 * fx0 * fx0 * (fx0 * (fx0 * 6 - 15) + 10);

		nx0 = Grad2(_perm[ix0 + _perm[iy0]], fx0, fy0);
		nx1 = Grad2(_perm[ix0 + _perm[iy1]], fx0, fy1);
		n0 = Math.Lerp(nx0, nx1, t);

		nx0 = Grad2(_perm[ix1 + _perm[iy0]], fx1, fy0);
		nx1 = Grad2(_perm[ix1 + _perm[iy1]], fx1, fy1);
		n1 = Math.Lerp(nx0, nx1, t);

		return 0.446f * Math.Lerp(n0, n1, s) + .5f;
	}

	private static int[] _a = new int[Vector<float>.Count];
	private static int[] _b = new int[Vector<float>.Count];
	private static int[] _c = new int[Vector<float>.Count];
	private static int[] _d = new int[Vector<float>.Count];
	private static int[] _e = new int[Vector<float>.Count];
	private static int[] _f = new int[Vector<float>.Count];
	private static int[] _g = new int[Vector<float>.Count];
	private static int[] _h = new int[Vector<float>.Count];

	private static readonly Vector<int> S255 = new(255);
	private static readonly Vector<float> Scale = new(0.935f);

	public static Vector<float> Noise3Simd(Vector<float> x, Vector<float> y, Vector<float> z)
	{
		var ix0 = (Vector<int>)FloorSimd(x);
		var iy0 = (Vector<int>)FloorSimd(y);
		var iz0 = (Vector<int>)FloorSimd(z);

		var fx0 = x - (Vector<float>)ix0;
		var fy0 = y - (Vector<float>)iy0;
		var fz0 = z - (Vector<float>)iz0;

		var fx1 = fx0 - Vector<float>.One;
		var fy1 = fy0 - Vector<float>.One;
		var fz1 = fz0 - Vector<float>.One;

		var ix1 = Vector.BitwiseAnd(ix0 + Vector<int>.One, S255);
		var iy1 = Vector.BitwiseAnd(iy0 + Vector<int>.One, S255);
		var iz1 = Vector.BitwiseAnd(iz0 + Vector<int>.One, S255);

		ix0 = Vector.BitwiseAnd(ix0, S255);
		iy0 = Vector.BitwiseAnd(iy0, S255);
		iz0 = Vector.BitwiseAnd(iz0, S255);

		var r = FadeSimd(fz0);
		var t = FadeSimd(fy0);
		var s = FadeSimd(fx0);

		var permp = _perm;

		for (var i = 0; i < Vector<float>.Count; i++)
		{
			var x0 = ix0[i];
			var y0 = iy0[i];
			var z0 = iz0[i];
			var x1 = ix1[i];
			var y1 = iy1[i];
			var z1 = iz1[i];

			var pz1 = permp[z1];
			var pz0 = permp[z0];

			var y00 = permp[y0 + pz0];
			var y01 = permp[y0 + pz1];
			var y10 = permp[y1 + pz0];
			var y11 = permp[y1 + pz1];

			_a[i] = permp[x0 + y00];
			_b[i] = permp[x0 + y01];
			_c[i] = permp[x0 + y10];
			_d[i] = permp[x0 + y11];
			_e[i] = permp[x1 + y00];
			_f[i] = permp[x1 + y01];
			_g[i] = permp[x1 + y10];
			_h[i] = permp[x1 + y11];
		}

		var nxy0I = new Vector<int>(_a, 0);
		var nxy1I = new Vector<int>(_b, 0);
		var nxy0 = Grad3Simd(nxy0I, fx0, fy0, fz0);
		var nxy1 = Grad3Simd(nxy1I, fx0, fy0, fz1);
		var nx0 = LerpSimd(r, nxy0, nxy1);

		nxy0I = new Vector<int>(_c, 0);
		nxy1I = new Vector<int>(_d, 0);

		nxy0 = Grad3Simd(nxy0I, fx0, fy1, fz0);
		nxy1 = Grad3Simd(nxy1I, fx0, fy1, fz1);
		var nx1 = LerpSimd(r, nxy0, nxy1);
		var n0 = LerpSimd(t, nx0, nx1);

		nxy0I = new Vector<int>(_e, 0);
		nxy1I = new Vector<int>(_f, 0);

		nxy0 = Grad3Simd(nxy0I, fx1, fy0, fz0);
		nxy1 = Grad3Simd(nxy1I, fx1, fy0, fz1);
		nx0 = LerpSimd(r, nxy0, nxy1);

		nxy0I = new Vector<int>(_g, 0);
		nxy1I = new Vector<int>(_h, 0);

		nxy0 = Grad3Simd(nxy0I, fx1, fy1, fz0);
		nxy1 = Grad3Simd(nxy1I, fx1, fy1, fz1);
		nx1 = LerpSimd(r, nxy0, nxy1);

		var n1 = LerpSimd(t, nx0, nx1);
		return LerpSimd(s, n0, n1) * Scale;
	}

	//---------------------------------------------------------------------
	/** 3D float Perlin noise. */
	public static float Noise3(float x, float y, float z)
	{
		int ix0, iy0, iz0, ix1, iy1, iz1;
		float fx0, fy0, fz0, fx1, fy1, fz1;
		float s, t, r;
		float nxy0, nxy1, nx0, nx1, n0, n1;

		ix0 = x > 0 ? (int)x : (int)x - 1; //FASTFLOOR(x); // Integer part of x
		iy0 = y > 0 ? (int)y : (int)y - 1; //FASTFLOOR(y); // Integer part of y
		iz0 = z > 0 ? (int)z : (int)z - 1; //FASTFLOOR(z); // Integer part of z
		fx0 = x - ix0; // Fractional part of x
		fy0 = y - iy0; // Fractional part of y
		fz0 = z - iz0; // Fractional part of z
		fx1 = fx0 - 1.0f;
		fy1 = fy0 - 1.0f;
		fz1 = fz0 - 1.0f;
		ix1 = (ix0 + 1) & 0xff; // Wrap to 0..255
		iy1 = (iy0 + 1) & 0xff;
		iz1 = (iz0 + 1) & 0xff;
		ix0 &= 0xff;
		iy0 &= 0xff;
		iz0 &= 0xff;

		r = Fade(fz0);
		t = Fade(fy0);
		s = Fade(fx0);

		nxy0 = Grad3(_perm[ix0 + _perm[iy0 + _perm[iz0]]], fx0, fy0, fz0);
		nxy1 = Grad3(_perm[ix0 + _perm[iy0 + _perm[iz1]]], fx0, fy0, fz1);
		nx0 = Math.Lerp(nxy0, nxy1, r);

		nxy0 = Grad3(_perm[ix0 + _perm[iy1 + _perm[iz0]]], fx0, fy1, fz0);
		nxy1 = Grad3(_perm[ix0 + _perm[iy1 + _perm[iz1]]], fx0, fy1, fz1);
		nx1 = Math.Lerp(nxy0, nxy1, r);

		n0 = Math.Lerp(nx0, nx1, t);

		nxy0 = Grad3(_perm[ix1 + _perm[iy0 + _perm[iz0]]], fx1, fy0, fz0);
		nxy1 = Grad3(_perm[ix1 + _perm[iy0 + _perm[iz1]]], fx1, fy0, fz1);
		nx0 = Math.Lerp(nxy0, nxy1, r);

		nxy0 = Grad3(_perm[ix1 + _perm[iy1 + _perm[iz0]]], fx1, fy1, fz0);
		nxy1 = Grad3(_perm[ix1 + _perm[iy1 + _perm[iz1]]], fx1, fy1, fz1);
		nx1 = Math.Lerp(nxy0, nxy1, r);

		n1 = Math.Lerp(nx0, nx1, t);

		return Math.Lerp(n0, n1, s) * .936f;
	}

	//---------------------------------------------------------------------
	/** 3D float Perlin periodic noise. */
	public static float Pnoise3(float x, float y, float z, int px, int py, int pz)
	{
		int ix0, iy0, iz0, ix1, iy1, iz1;
		float fx0, fy0, fz0, fx1, fy1, fz1;
		float s, t, r;
		float nxy0, nxy1, nx0, nx1, n0, n1;

		ix0 = x > 0 ? (int)x : (int)x - 1; //FASTFLOOR(x); // Integer part of x
		iy0 = y > 0 ? (int)y : (int)y - 1; //FASTFLOOR(y); // Integer part of y
		iz0 = z > 0 ? (int)z : (int)z - 1; //FASTFLOOR(z); // Integer part of z
		fx0 = x - ix0; // Fractional part of x
		fy0 = y - iy0; // Fractional part of y
		fz0 = z - iz0; // Fractional part of z
		fx1 = fx0 - 1.0f;
		fy1 = fy0 - 1.0f;
		fz1 = fz0 - 1.0f;
		ix1 = ((ix0 + 1) % px) & 0xff; // Wrap to 0..px-1 and wrap to 0..255
		iy1 = ((iy0 + 1) % py) & 0xff; // Wrap to 0..py-1 and wrap to 0..255
		iz1 = ((iz0 + 1) % pz) & 0xff; // Wrap to 0..pz-1 and wrap to 0..255
		ix0 = (ix0 % px) & 0xff;
		iy0 = (iy0 % py) & 0xff;
		iz0 = (iz0 % pz) & 0xff;

		r = Fade(fz0);
		t = Fade(fy0);
		s = Fade(fx0);

		nxy0 = Grad3(_perm[ix0 + _perm[iy0 + _perm[iz0]]], fx0, fy0, fz0);
		nxy1 = Grad3(_perm[ix0 + _perm[iy0 + _perm[iz1]]], fx0, fy0, fz1);
		nx0 = Math.Lerp(nxy0, nxy1, r);

		nxy0 = Grad3(_perm[ix0 + _perm[iy1 + _perm[iz0]]], fx0, fy1, fz0);
		nxy1 = Grad3(_perm[ix0 + _perm[iy1 + _perm[iz1]]], fx0, fy1, fz1);
		nx1 = Math.Lerp(nxy0, nxy1, r);

		n0 = Math.Lerp(nx0, nx1, t);

		nxy0 = Grad3(_perm[ix1 + _perm[iy0 + _perm[iz0]]], fx1, fy0, fz0);
		nxy1 = Grad3(_perm[ix1 + _perm[iy0 + _perm[iz1]]], fx1, fy0, fz1);
		nx0 = Math.Lerp(nxy0, nxy1, r);

		nxy0 = Grad3(_perm[ix1 + _perm[iy1 + _perm[iz0]]], fx1, fy1, fz0);
		nxy1 = Grad3(_perm[ix1 + _perm[iy1 + _perm[iz1]]], fx1, fy1, fz1);
		nx1 = Math.Lerp(nxy0, nxy1, r);

		n1 = Math.Lerp(nx0, nx1, r);

		return 0.936f * Math.Lerp(n0, n1, s);
	}

	//---------------------------------------------------------------------
	/** 4D float Perlin noise. */
	public static float Noise4(float x, float y, float z, float w)
	{
		int ix0, iy0, iz0, iw0, ix1, iy1, iz1, iw1;
		float fx0, fy0, fz0, fw0, fx1, fy1, fz1, fw1;
		float s, t, r, q;
		float nxyz0, nxyz1, nxy0, nxy1, nx0, nx1, n0, n1;

		ix0 = x > 0 ? (int)x : (int)x - 1; //FASTFLOOR(x); // Integer part of x
		iy0 = y > 0 ? (int)y : (int)y - 1; //FASTFLOOR(y); // Integer part of y
		iz0 = z > 0 ? (int)z : (int)z - 1; //FASTFLOOR(z); // Integer part of z
		iw0 = w > 0 ? (int)w : (int)w - 1; //FASTFLOOR(w); // Integer part of w
		fx0 = x - ix0; // Fractional part of x
		fy0 = y - iy0; // Fractional part of y
		fz0 = z - iz0; // Fractional part of z
		fw0 = w - iw0; // Fractional part of w
		fx1 = fx0 - 1.0f;
		fy1 = fy0 - 1.0f;
		fz1 = fz0 - 1.0f;
		fw1 = fw0 - 1.0f;
		ix1 = (ix0 + 1) & 0xff; // Wrap to 0..255
		iy1 = (iy0 + 1) & 0xff;
		iz1 = (iz0 + 1) & 0xff;
		iw1 = (iw0 + 1) & 0xff;
		ix0 &= 0xff;
		iy0 &= 0xff;
		iz0 &= 0xff;
		iw0 &= 0xff;

		q = Fade(fw0);
		r = Fade(fz0);
		t = Fade(fy0);
		s = Fade(fx0);

		nxyz0 = Grad4(_perm[ix0 + _perm[iy0 + _perm[iz0 + _perm[iw0]]]], fx0, fy0, fz0, fw0);
		nxyz1 = Grad4(_perm[ix0 + _perm[iy0 + _perm[iz0 + _perm[iw1]]]], fx0, fy0, fz0, fw1);
		nxy0 = Math.Lerp(nxyz0, nxyz1, q);

		nxyz0 = Grad4(_perm[ix0 + _perm[iy0 + _perm[iz1 + _perm[iw0]]]], fx0, fy0, fz1, fw0);
		nxyz1 = Grad4(_perm[ix0 + _perm[iy0 + _perm[iz1 + _perm[iw1]]]], fx0, fy0, fz1, fw1);
		nxy1 = Math.Lerp(nxyz0, nxyz1, q);

		nx0 = Math.Lerp(nxy0, nxy1, r);

		nxyz0 = Grad4(_perm[ix0 + _perm[iy1 + _perm[iz0 + _perm[iw0]]]], fx0, fy1, fz0, fw0);
		nxyz1 = Grad4(_perm[ix0 + _perm[iy1 + _perm[iz0 + _perm[iw1]]]], fx0, fy1, fz0, fw1);
		nxy0 = Math.Lerp(nxyz0, nxyz1, q);

		nxyz0 = Grad4(_perm[ix0 + _perm[iy1 + _perm[iz1 + _perm[iw0]]]], fx0, fy1, fz1, fw0);
		nxyz1 = Grad4(_perm[ix0 + _perm[iy1 + _perm[iz1 + _perm[iw1]]]], fx0, fy1, fz1, fw1);
		nxy1 = Math.Lerp(nxyz0, nxyz1, q);

		nx1 = Math.Lerp(nxy0, nxy1, r);

		n0 = Math.Lerp(nx0, nx1, t);

		nxyz0 = Grad4(_perm[ix1 + _perm[iy0 + _perm[iz0 + _perm[iw0]]]], fx1, fy0, fz0, fw0);
		nxyz1 = Grad4(_perm[ix1 + _perm[iy0 + _perm[iz0 + _perm[iw1]]]], fx1, fy0, fz0, fw1);
		nxy0 = Math.Lerp(nxyz0, nxyz1, q);

		nxyz0 = Grad4(_perm[ix1 + _perm[iy0 + _perm[iz1 + _perm[iw0]]]], fx1, fy0, fz1, fw0);
		nxyz1 = Grad4(_perm[ix1 + _perm[iy0 + _perm[iz1 + _perm[iw1]]]], fx1, fy0, fz1, fw1);
		nxy1 = Math.Lerp(nxyz0, nxyz1, q);

		nx0 = Math.Lerp(nxy0, nxy1, r);

		nxyz0 = Grad4(_perm[ix1 + _perm[iy1 + _perm[iz0 + _perm[iw0]]]], fx1, fy1, fz0, fw0);
		nxyz1 = Grad4(_perm[ix1 + _perm[iy1 + _perm[iz0 + _perm[iw1]]]], fx1, fy1, fz0, fw1);
		nxy0 = Math.Lerp(nxyz0, nxyz1, q);

		nxyz0 = Grad4(_perm[ix1 + _perm[iy1 + _perm[iz1 + _perm[iw0]]]], fx1, fy1, fz1, fw0);
		nxyz1 = Grad4(_perm[ix1 + _perm[iy1 + _perm[iz1 + _perm[iw1]]]], fx1, fy1, fz1, fw1);
		nxy1 = Math.Lerp(nxyz0, nxyz1, q);

		nx1 = Math.Lerp(nxy0, nxy1, r);

		n1 = Math.Lerp(nx0, nx1, t);

		return 0.87f * Math.Lerp(n0, n1, s);
	}

	//---------------------------------------------------------------------
	/** 4D float Perlin periodic noise. */
	public static float Pnoise4(float x, float y, float z, float w,
		int px, int py, int pz, int pw)
	{
		int ix0, iy0, iz0, iw0, ix1, iy1, iz1, iw1;
		float fx0, fy0, fz0, fw0, fx1, fy1, fz1, fw1;
		float s, t, r, q;
		float nxyz0, nxyz1, nxy0, nxy1, nx0, nx1, n0, n1;

		ix0 = x > 0 ? (int)x : (int)x - 1; //FASTFLOOR(x); // Integer part of x
		iy0 = y > 0 ? (int)y : (int)y - 1; //FASTFLOOR(y); // Integer part of y
		iz0 = z > 0 ? (int)z : (int)z - 1; //FASTFLOOR(z); // Integer part of z
		iw0 = w > 0 ? (int)w : (int)w - 1; //FASTFLOOR(w); // Integer part of w
		fx0 = x - ix0; // Fractional part of x
		fy0 = y - iy0; // Fractional part of y
		fz0 = z - iz0; // Fractional part of z
		fw0 = w - iw0; // Fractional part of w
		fx1 = fx0 - 1.0f;
		fy1 = fy0 - 1.0f;
		fz1 = fz0 - 1.0f;
		fw1 = fw0 - 1.0f;
		ix1 = ((ix0 + 1) % px) & 0xff; // Wrap to 0..px-1 and wrap to 0..255
		iy1 = ((iy0 + 1) % py) & 0xff; // Wrap to 0..py-1 and wrap to 0..255
		iz1 = ((iz0 + 1) % pz) & 0xff; // Wrap to 0..pz-1 and wrap to 0..255
		iw1 = ((iw0 + 1) % pw) & 0xff; // Wrap to 0..pw-1 and wrap to 0..255
		ix0 = (ix0 % px) & 0xff;
		iy0 = (iy0 % py) & 0xff;
		iz0 = (iz0 % pz) & 0xff;
		iw0 = (iw0 % pw) & 0xff;

		q = Fade(fw0);
		r = Fade(fz0);
		t = Fade(fy0);
		s = Fade(fx0);

		nxyz0 = Grad4(_perm[ix0 + _perm[iy0 + _perm[iz0 + _perm[iw0]]]], fx0, fy0, fz0, fw0);
		nxyz1 = Grad4(_perm[ix0 + _perm[iy0 + _perm[iz0 + _perm[iw1]]]], fx0, fy0, fz0, fw1);
		nxy0 = Math.Lerp(nxyz0, nxyz1, q);

		nxyz0 = Grad4(_perm[ix0 + _perm[iy0 + _perm[iz1 + _perm[iw0]]]], fx0, fy0, fz1, fw0);
		nxyz1 = Grad4(_perm[ix0 + _perm[iy0 + _perm[iz1 + _perm[iw1]]]], fx0, fy0, fz1, fw1);
		nxy1 = Math.Lerp(nxyz0, nxyz1, q);

		nx0 = Math.Lerp(nxy0, nxy1, r);

		nxyz0 = Grad4(_perm[ix0 + _perm[iy1 + _perm[iz0 + _perm[iw0]]]], fx0, fy1, fz0, fw0);
		nxyz1 = Grad4(_perm[ix0 + _perm[iy1 + _perm[iz0 + _perm[iw1]]]], fx0, fy1, fz0, fw1);
		nxy0 = Math.Lerp(nxyz0, nxyz1, q);

		nxyz0 = Grad4(_perm[ix0 + _perm[iy1 + _perm[iz1 + _perm[iw0]]]], fx0, fy1, fz1, fw0);
		nxyz1 = Grad4(_perm[ix0 + _perm[iy1 + _perm[iz1 + _perm[iw1]]]], fx0, fy1, fz1, fw1);
		nxy1 = Math.Lerp(nxyz0, nxyz1, q);

		nx1 = Math.Lerp(nxy0, nxy1, r);

		n0 = Math.Lerp(nx0, nx1, t);

		nxyz0 = Grad4(_perm[ix1 + _perm[iy0 + _perm[iz0 + _perm[iw0]]]], fx1, fy0, fz0, fw0);
		nxyz1 = Grad4(_perm[ix1 + _perm[iy0 + _perm[iz0 + _perm[iw1]]]], fx1, fy0, fz0, fw1);
		nxy0 = Math.Lerp(nxyz0, nxyz1, q);

		nxyz0 = Grad4(_perm[ix1 + _perm[iy0 + _perm[iz1 + _perm[iw0]]]], fx1, fy0, fz1, fw0);
		nxyz1 = Grad4(_perm[ix1 + _perm[iy0 + _perm[iz1 + _perm[iw1]]]], fx1, fy0, fz1, fw1);
		nxy1 = Math.Lerp(nxyz0, nxyz1, q);

		nx0 = Math.Lerp(nxy0, nxy1, r);

		nxyz0 = Grad4(_perm[ix1 + _perm[iy1 + _perm[iz0 + _perm[iw0]]]], fx1, fy1, fz0, fw0);
		nxyz1 = Grad4(_perm[ix1 + _perm[iy1 + _perm[iz0 + _perm[iw1]]]], fx1, fy1, fz0, fw1);
		nxy0 = Math.Lerp(nxyz0, nxyz1, q);

		nxyz0 = Grad4(_perm[ix1 + _perm[iy1 + _perm[iz1 + _perm[iw0]]]], fx1, fy1, fz1, fw0);
		nxyz1 = Grad4(_perm[ix1 + _perm[iy1 + _perm[iz1 + _perm[iw1]]]], fx1, fy1, fz1, fw1);
		nxy1 = Math.Lerp(nxyz0, nxyz1, q);

		nx1 = Math.Lerp(nxy0, nxy1, r);

		n1 = Math.Lerp(nx0, nx1, t);

		return 0.87f * Math.Lerp(n0, n1, s);
	}

	private static float Pfbm2(float x, float y, int px, int py, int octaves, float alpha, float omega)
	{
		var sum = 0f;
		float a = 1;
		float b = 1;
		for (var i = 1; i <= octaves; i++)
		{
			sum += 1f / a * Pnoise2(x * b, y * b, px, py);
			a *= alpha;
			b *= omega;
		}

		return 1.0f;
		//return Mathf.Clamp(sum * (1f - (1f / alpha)) * 1.4f, 0, 1);
	}

	private static float Fbm2(float x, float y, int octaves, float alpha, float omega)
	{
		var sum = 0f;
		float a = 1;
		float b = 1;
		for (var i = 1; i <= octaves; i++)
		{
			sum += 1f / a * Noise2(x * b, y * b);
			a *= alpha;
			b *= omega;
		}

		return 1.0f;
		//return Mathf.Clamp(sum * (1f - (1f / alpha)) * 1.4f, 0, 1);
	}

	//private static float[] fmb3Offset = new float[] {0f,     0f, .0737f, .1189f, .1440f, .1530f};
	//private static float[] fmb3Scale = new float[]  {0f, 1.066f, .8584f, .8120f, .8083f, .8049f };    
	private static float[] _fmb3Offset = { 0f, 0f, .0737f, .1189f, .1440f, .1530f };
	private static float[] _fmb3Scale = { 0f, 1.066f, .8584f, .8120f, .8083f, .8049f };

	private static float Fbm3(float x, float y, float z, int octaves, float lacunarity, float gain)
	{
		var sum = 0f;
		float frequency = 1;
		float amplitude = 1;

		for (var i = 0; i < octaves; i++)
		{
			sum += frequency * Noise3(x * amplitude, y * amplitude, z * amplitude);
			frequency /= lacunarity;
			amplitude *= gain;
		}

		sum = (sum - _fmb3Offset[octaves]) * _fmb3Scale[octaves];
		//clamp it
		if (sum > 1) sum = 1;
		if (sum < 0) sum = 0;
		//look it up in the gradient and return the color
		return 1.0f;
		// return Gradient[(int)(sum * (Gradient.Length - 1))];
	}
}