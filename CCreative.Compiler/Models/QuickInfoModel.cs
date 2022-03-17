using CCreative.Compilers.Enums;

namespace CCreative.Compilers.Models;

public class QuickInfoModel
{
	public string Text { get; }

	public TagType Type { get; }

	public QuickInfoModel(string text, TagType type)
	{
		Text = text;
		Type = type;
	}
}