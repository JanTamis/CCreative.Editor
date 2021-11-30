typedef struct {
	uchar b, g, r, a;
} Color;

kernel void Gray(global Color* pixels)
{
	const int i = get_global_id(0);

	Color tempColor = pixels[i];

	uchar gray = tempColor.r * 0.3f + tempColor.g * 0.59f + tempColor.b * 0.11f;

	tempColor.r = gray;
	tempColor.g = gray;
	tempColor.b = gray;

	pixels[i] = tempColor;
}

kernel void Threshold(global Color* pixels, int thresh)
{
	const int i = get_global_id(0);

	Color tempColor = pixels[i];

	uchar maxValue = max(tempColor.r, max(tempColor.g, tempColor.b));
	uchar gray = 0;

	if (maxValue < thresh) {
		gray = 255;
	}

	tempColor.r = gray;
	tempColor.g = gray;
	tempColor.b = gray;

	pixels[i] = tempColor;
}

kernel void Invert(global Color* pixels)
{
	const int i = get_global_id(0);

	Color tempColor = pixels[i];

	tempColor.r = 255 - tempColor.r;
	tempColor.g = 255 - tempColor.g;
	tempColor.b = 255 - tempColor.b;

	pixels[i] = tempColor;
}

kernel void Opaque(global Color* pixels)
{
	const int i = get_global_id(0);

	Color tempColor = pixels[i];

	tempColor.a = 255;

	pixels[i] = tempColor;
}

// kernel void Sepia(global Color* pixels, float depth)
// {
//   const int i = get_global_id(0);

//   Color tempColor = pixels[i];

//   uchar red = tempColor.r + (depth * 2);
//   uchar green = tempColor.g + (depth * 2);
//   uchar blue = tempColor.b * 0.114f + tempColor.g * 0.587f + tempColor.b * 0.255f;

//   red += depth * 2;
//   green += depth * 2;
//   blue += depth * 2;

//   red = clamp(red, 0, 255);
//   green = clamp(green, 0, 255);
//   blue = clamp(blue, 0, 255);

//   tempColor.r = red;
//   tempColor.g = green;
//   tempColor.b = blue;

//   pixels[i] = tempColor;
// }

kernel void Blur1DVertical(global float4 *in, global float4 *out, constant float* filter, int filterSize)
{
	const int x = get_global_id(0);
	const int y = get_global_id(1);
	const int width = get_global_size(0);
	const int height = get_global_size(1);
	const int xMiddle = filterSize / 2;

	float4 sum = (float4)0.0f;
	float percentage = 0.0f;

	for	(int i = 0; i < filterSize; i++) 
	{
		int y0 = y - xMiddle + i;

		if (y0 >= 0 && y0 < height)
		{
			sum += in[width * y0 + x] * filter[i];
			percentage += filter[i];
		}
	}

	out[width * y + x] = sum * (1.0f / percentage);
}

kernel void Blur1DHorizontal(global float4 *in, global float4 *out, constant float* filter, int filterSize)
{
	const int x = get_global_id(0);
	const int y = get_global_id(1);
	const int width = get_global_size(0);
	const int xMiddle = filterSize / 2;

	float4 sum = (float4)0.0f;
	float percentage = 0.0f;

	for	(int i = 0; i < filterSize; i++) 
	{
		int x0 = x - xMiddle + i;

		if (x0 >= 0 && x0 < width)
        {
			sum += in[width * y + x0] * filter[i];

			percentage += filter[i];
	    }
	}

	out[width * y + x] = sum * (1.0f / percentage);
}

kernel void Posterize(global float4* in, float levels)
{
  int gid = get_global_id(0);
  float4 in_v = in[gid];

  in_v.xyz = trunc(in_v.xyz * levels + (float3)(0.5f)) / levels;
  in[gid] = in_v;
}

//noise values in range if 0.0 to 1.0
static float noise3D(float x, float y, float z)
{
	float ptr = 0.0f;
	return fract(sin(x * 112.9898f + y * 179.233f + z * 237.212f), &ptr);
}