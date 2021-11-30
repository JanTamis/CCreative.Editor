using System;

namespace CCreative
{
	/// <summary>
	/// Datatype for storing color values
	/// </summary>
	public interface Color
	{
		/// <summary>
		/// the blue component of the color
		/// </summary>
		byte B { get; set; }

		/// <summary>
		/// the green component of the color
		/// </summary>
		byte G { get; set; }

		/// <summary>
		/// the red component of the color
		/// </summary>
		byte R { get; set; }

		/// <summary>
		/// the alpha component of the color
		/// </summary>
		public byte A { get; set; }

		/// <summary>
		/// Returns the color as a hexadecimal string
		/// </summary>
		/// <returns> the hexadecimal string</returns>
		public string ToString()
		{
			return A is Byte.MaxValue ? $"#{R:X2}{G:X2}{B:X2}" : $"#{A:X2}{R:X2}{G:X2}{B:X2}";
		}
	}
}