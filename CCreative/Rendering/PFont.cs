using System.Linq;
using SixLabors.Fonts;

namespace CCreative
{
	public class PFont
	{
		internal readonly Font font;

		internal PFont(Font font)
		{
			this.font = font;
		}

		public short Ascent()
		{
			return 0;
		}

		public short Descent()
		{
			return 0;
		}

		public double Width(char c)
		{
			// return TextMeasurer.MeasureBounds(c.ToString(), new RendererOptions(font.Family.CreateFont(1)))
			// 									 .Width;
			return 0;
		}

		public static string[] List()
		{
			return SystemFonts.Families.Select(x => x.Name)
																 .ToArray();
		}
	}
}
