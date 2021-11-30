namespace CCreative.Rendering
{
	public class PStyle
	{
		public DrawTypes imageMode = DrawTypes.Corner;
		public DrawTypes rectMode = DrawTypes.Corner;
		public DrawTypes ellipseMode = DrawTypes.Center;
		public DrawTypes shapeMode = DrawTypes.Corner;

		public int blendMode;

		public int colorMode;
		public float colorModeX;
		public float colorModeY;
		public float colorModeZ;
		public float colorModeA;

		public bool tint = false;
		public Color tintColor;
		public bool fill = true;
		public Color fillColor;
		public bool stroke = true;
		public Color strokeColor;
		public float strokeWeight = 1;
		public int strokeCap;
		public int strokeJoin;

		// TODO these fellas are inconsistent, and may need to go elsewhere
		public float ambientR, ambientG, ambientB;
		public float specularR, specularG, specularB;
		public float emissiveR, emissiveG, emissiveB;
		public float shininess;

		public PFont? textFont;
		public int textAlign;
		public int textAlignY;
		public int textMode;
		public float textSize;
		public float textLeading;
	}
}
