using System;
using System.Buffers;

namespace CCreative
{
	internal static class PixelHelper
	{
		private static readonly uint ALPHA_MASK = 0xff000000;
		private static readonly uint RED_MASK = 0x00ff0000;
		private static readonly uint GREEN_MASK = 0x0000ff00;
		private static readonly uint BLUE_MASK = 0x000000ff;

		private static readonly float currentR = -1;

		private static readonly ArrayPool<int> pool = ArrayPool<int>.Shared;

		public static void GrayScale(Span<int> pixels)
		{
			for (int i = 0; i < pixels.Length; i++)
			{
				int col = pixels[i];
				// luminance = 0.3*red + 0.59*green + 0.11*blue
				// 0.30 * 256 =  77
				// 0.59 * 256 = 151
				// 0.11 * 256 =  28
				int lum = (77 * (col >> 16 & 0xff) + 151 * (col >> 8 & 0xff) + 28 * (col & 0xff)) >> 8;
				pixels[i] = (int)((col & ALPHA_MASK) | lum << 16 | lum << 8 | lum);
			}
		}

		public static void Threshold(Span<int> pixels, double threshold = 0.5)
		{
			byte thresh = (byte)(threshold * 255);

			for (int i = 0; i < pixels.Length; i++)
			{
				var col = pixels[i];

				var max = System.Math.Max((col & RED_MASK) >> 16, System.Math.Max((col & GREEN_MASK) >> 8, (col & BLUE_MASK)));

				pixels[i] = (int)((col & ALPHA_MASK) | ((max < thresh) ? 0x000000
																															 : 0xffffff));
			}
		}

		public static void Invert(Span<int> pixels)
		{
			for (int i = 0; i < pixels.Length; i++)
			{
				pixels[i] ^= 0xffffff;
			}
		}

		public static void Opaque(Span<uint> pixels)
		{
			for (int i = 0; i < pixels.Length; i++)
			{
				pixels[i] |= 0xff000000;
			}
		}

		public static void Posterize(Span<int> pixels, int levels)
		{
			if ((levels < 2) || (levels > Byte.MaxValue))
			{
				throw new ArgumentException("Levels must be between 2 and 255", nameof(levels));
			}

			levels--;

			for (int i = 0; i < pixels.Length; i++)
			{
				int col = pixels[i];

				int rlevel = (col >> 16) & 0xff;
				int glevel = (col >> 8) & 0xff;
				int blevel = col & 0xff;

				rlevel = (((rlevel * levels) >> 8) * 255) / levels;
				glevel = (((glevel * levels) >> 8) * 255) / levels;
				blevel = (((blevel * levels) >> 8) * 255) / levels;
				pixels[i] = (int)(((0xff000000 & col) | (rlevel << 16) | (glevel << 8) | blevel));
			}
		}

		public static void Dilate(Span<int> pixels, int width)
		{
			int index = 0;
			int maxIndex = pixels.Length;
			Span<int> outgoing = stackalloc int[maxIndex];

			while (index < maxIndex)
			{
				int curRowIndex = index;
				int maxRowIndex = index + width;

				while (index < maxRowIndex)
				{
					int orig = pixels[index];
					int result = orig;
					int idxLeft = index - 1;
					int idxRight = index + 1;
					int idxUp = index - width;
					int idxDown = index + width;

					if (idxLeft < curRowIndex)
						idxLeft = index;

					if (idxRight >= maxRowIndex)
						idxRight = index;

					if (idxUp < 0)
						idxUp = index;

					if (idxDown >= maxIndex)
						idxDown = index;

					int colUp = pixels[idxUp];
					int colLeft = pixels[idxLeft];
					int colDown = pixels[idxDown];
					int colRight = pixels[idxRight];

					// compute luminance
					int currLum = 77 * (orig >> 16 & 0xff) + 151 * (orig >> 8 & 0xff) + 28 * (orig & 0xff);
					int lumLeft = 77 * (colLeft >> 16 & 0xff) + 151 * (colLeft >> 8 & 0xff) + 28 * (colLeft & 0xff);
					int lumRight = 77 * (colRight >> 16 & 0xff) + 151 * (colRight >> 8 & 0xff) + 28 * (colRight & 0xff);
					int lumUp = 77 * (colUp >> 16 & 0xff) + 151 * (colUp >> 8 & 0xff) + 28 * (colUp & 0xff);
					int lumDown = 77 * (colDown >> 16 & 0xff) + 151 * (colDown >> 8 & 0xff) + 28 * (colDown & 0xff);

					if (lumLeft < currLum)
					{
						result = colLeft;
						currLum = lumLeft;
					}
					if (lumRight < currLum)
					{
						result = colRight;
						currLum = lumRight;
					}
					if (lumUp < currLum)
					{
						result = colUp;
						currLum = lumUp;
					}
					if (lumDown < currLum)
					{
						result = colDown;
						currLum = lumDown;
					}

					outgoing[index++] = result;
				}
			}

			outgoing.CopyTo(pixels);
		}

		public static void Erode(Span<int> pixels, int width)
		{
			int index = 0;
			int maxIndex = pixels.Length;
			int[] outgoing = pool.Rent(maxIndex);

			while (index < maxIndex)
			{
				int curRowIndex = index;
				int maxRowIndex = index + width;

				while (index < maxRowIndex)
				{
					int orig = pixels[index];
					int result = orig;
					int idxLeft = index - 1;
					int idxRight = index + 1;
					int idxUp = index - width;
					int idxDown = index + width;

					if (idxLeft < curRowIndex)
						idxLeft = index;

					if (idxRight >= maxRowIndex)
						idxRight = index;

					if (idxUp < 0)
						idxUp = index;

					if (idxDown >= maxIndex)
						idxDown = index;

					int colUp = pixels[idxUp];
					int colLeft = pixels[idxLeft];
					int colDown = pixels[idxDown];
					int colRight = pixels[idxRight];

					// compute luminance
					int currLum = 77 * (orig >> 16 & 0xff) + 151 * (orig >> 8 & 0xff) + 28 * (orig & 0xff);
					int lumLeft = 77 * (colLeft >> 16 & 0xff) + 151 * (colLeft >> 8 & 0xff) + 28 * (colLeft & 0xff);
					int lumRight = 77 * (colRight >> 16 & 0xff) + 151 * (colRight >> 8 & 0xff) + 28 * (colRight & 0xff);
					int lumUp = 77 * (colUp >> 16 & 0xff) + 151 * (colUp >> 8 & 0xff) + 28 * (colUp & 0xff);
					int lumDown = 77 * (colDown >> 16 & 0xff) + 151 * (colDown >> 8 & 0xff) + 28 * (colDown & 0xff);

					if (lumLeft < currLum)
					{
						result = colLeft;
						currLum = lumLeft;
					}
					if (lumRight < currLum)
					{
						result = colRight;
						currLum = lumRight;
					}
					if (lumUp < currLum)
					{
						result = colUp;
						currLum = lumUp;
					}
					if (lumDown < currLum)
					{
						result = colDown;
					}
					outgoing[index++] = result;
				}
			}

			outgoing.CopyTo(pixels);

			pool.Return(outgoing);
		}

		public static void Sepia(Span<byte> pixels, int bits, int depth)
		{
			for (int i = 0; i < pixels.Length; i++)
			{
				var index = i / bits;
				pixels[index] = (byte)((.299 * pixels[index + 2]) + (pixels[index + 1] * .587) + (pixels[index] * .114));

				int pixel = pixels[index + 2] + (depth * 2);

				if (pixel > 255)
					pixel = 255;
				pixels[index + 2] = (byte)pixel;

				pixel = pixels[index + 1] + depth;
				if (pixel > 255)
					pixel = 255;
				pixels[index + 1] = (byte)pixel;
			}
		}

		public static void Jitter(Span<int> pixels, int pixelWidth, int pixelHeight, int degree)
		{
			short Half = (short)(degree / 2);
			int[] temp = pool.Rent(pixels.Length);

			for (int i = 0; i < pixels.Length; i++)
			{
				var X = i % pixelWidth;
				var Y = i / pixelHeight;

				var XVal = X + (Math.RandomInt(degree) - Half);
				var YVal = Y + (Math.RandomInt(degree) - Half);

				if (XVal > 0 && XVal < pixelWidth && YVal > 0 && YVal < pixelHeight)
				{
					var Val = (YVal * pixelWidth) + XVal;

					temp[i] = pixels[Val];
				}
			}

			temp.AsSpan(0, pixels.Length).CopyTo(pixels);

			pool.Return(temp);
		}
	}
}
