using CCreative.Compilers.Enums;

namespace CCreative.Compilers.Models;

public class QuickInfoGroupModel
{
	public QuickInfoSectionType SectionType { get; init; }
	public IEnumerable<QuickInfoModel>? Infos { get; init; }
}