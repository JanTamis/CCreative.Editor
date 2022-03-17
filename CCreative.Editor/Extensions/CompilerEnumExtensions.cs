using System;
using Avalonia;
using Avalonia.Media;
using CCreative.Compilers.Enums;

namespace CCreative.Editor.Extensions;

public static class CompilerEnumExtensions
{
	public static IBrush GetForeground(this ClassifierType type)
	{
		switch (type)
		{
			case ClassifierType.Comment:
				return new SolidColorBrush(Avalonia.Media.Color.Parse("#57A64A"));
			case ClassifierType.ExcludedCode:
				break;
			case ClassifierType.Identifier:
				break;
			case ClassifierType.Keyword:
			case ClassifierType.ControlKeyword:
				return new SolidColorBrush(Avalonia.Media.Color.Parse("#569CD6"));
			case ClassifierType.NumericLiteral:
				return new SolidColorBrush(Avalonia.Media.Color.Parse("#B5CEA8"));
			case ClassifierType.Operator:
				break;
			case ClassifierType.OperatorOverloaded:
				break;
			case ClassifierType.PreprocessorKeyword:
				return new SolidColorBrush(Avalonia.Media.Color.Parse("#569CD6"));

			case ClassifierType.StringLiteral:
				return new SolidColorBrush(Avalonia.Media.Color.Parse("#D69D85"));

			case ClassifierType.StaticSymbol:
				break;
			case ClassifierType.PreprocessorText:
				break;
			case ClassifierType.VerbatimStringLiteral:
				break;
			case ClassifierType.StringEscapeCharacter:
				return new SolidColorBrush(Avalonia.Media.Color.Parse("#E07A00"));

			case ClassifierType.ClassName:
			case ClassifierType.RecordStructName:
			case ClassifierType.RecordClassName:
			case ClassifierType.DelegateName:
			case ClassifierType.StructName:
			case ClassifierType.TypeParameterName:
				return new SolidColorBrush(Avalonia.Media.Color.Parse("#4EC9B0"));

			case ClassifierType.EnumName:
			case ClassifierType.InterfaceName:
				return new SolidColorBrush(Avalonia.Media.Color.Parse("#B8D7A3"));

			case ClassifierType.ModuleName:
				break;
			case ClassifierType.FieldName:
				break;
			case ClassifierType.EnumMemberName:
				break;
			case ClassifierType.ConstantName:
				break;
			case ClassifierType.LocalName:
			case ClassifierType.ParameterName:
				return new SolidColorBrush(Avalonia.Media.Color.Parse("#9CDCFE"));

			case ClassifierType.MethodName:
			case ClassifierType.ExtensionMethodName:
				return new SolidColorBrush(Avalonia.Media.Color.Parse("#DCDCAA"));

			case ClassifierType.PropertyName:
				break;
			case ClassifierType.EventName:
				break;
			case ClassifierType.NamespaceName:
				break;
			case ClassifierType.LabelName:
				break;
			case ClassifierType.XmlDocCommentAttributeName:
				break;
			case ClassifierType.XmlDocCommentAttributeQuotes:
				break;
			case ClassifierType.XmlDocCommentAttributeValue:
				break;
			case ClassifierType.XmlDocCommentCDataSection:
				break;
			case ClassifierType.XmlDocCommentComment:
				break;
			case ClassifierType.XmlDocCommentDelimiter:
				break;
			case ClassifierType.XmlDocCommentEntityReference:
				break;
			case ClassifierType.XmlDocCommentName:
				break;
			case ClassifierType.XmlDocCommentProcessingInstruction:
				break;
			case ClassifierType.XmlDocCommentText:
				break;
			case ClassifierType.XmlLiteralAttributeName:
				break;
			case ClassifierType.XmlLiteralAttributeQuotes:
				break;
			case ClassifierType.XmlLiteralAttributeValue:
				break;
			case ClassifierType.XmlLiteralCDataSection:
				break;
			case ClassifierType.XmlLiteralComment:
				break;
			case ClassifierType.XmlLiteralDelimiter:
				break;
			case ClassifierType.XmlLiteralEntityReference:
				break;
			case ClassifierType.XmlLiteralName:
				break;
			case ClassifierType.XmlLiteralProcessingInstruction:
				break;
			case ClassifierType.XmlLiteralText:
				break;
			case ClassifierType.RegexComment:
				break;
			case ClassifierType.RegexCharacterClass:
				break;
			case ClassifierType.RegexAnchor:
				break;
			case ClassifierType.RegexQuantifier:
				break;
			case ClassifierType.RegexGrouping:
				break;
			case ClassifierType.RegexAlternation:
				break;
			case ClassifierType.RegexText:
				break;
			case ClassifierType.RegexSelfEscapedCharacter:
				break;
			case ClassifierType.RegexOtherEscape:
				break;
		}

		return Application.Current.Resources["MaterialDesignBody"] as IBrush;
	}

	public static IBrush GetForeground(this TagType type)
	{
		switch (type)
		{
			case TagType.Alias:
				break;
			case TagType.Assembly:
				break;
			case TagType.ErrorType:
				break;
			case TagType.Event:
				break;
			case TagType.Field:
				break;
			case TagType.Interface:
				break;
			case TagType.Keyword:
				return new SolidColorBrush(Avalonia.Media.Color.Parse("#569CD6"));

			case TagType.Label:
				break;
			case TagType.LineBreak:
				break;
			case TagType.NumericLiteral:
				break;
			case TagType.StringLiteral:
				break;
			case TagType.Local:
				break;
			case TagType.Module:
				break;
			case TagType.Namespace:
				break;
			case TagType.Operator:
				break;
			case TagType.Parameter:
			case TagType.Property:
				return new SolidColorBrush(Avalonia.Media.Color.Parse("#9CDCFE"));

			case TagType.AnonymousTypeIndicator:
				break;
			case TagType.RangeVariable:
				break;
			case TagType.EnumMember:
				break;
			case TagType.ExtensionMethod:
			case TagType.Method:
				return new SolidColorBrush(Avalonia.Media.Color.Parse("#DCDCAA"));

			case TagType.Constant:
				break;
			case TagType.Class:
			case TagType.Delegate:
			case TagType.Enum:
			case TagType.Struct:
			case TagType.TypeParameter:
			case TagType.Record:
			case TagType.RecordStruct:
				return new SolidColorBrush(Avalonia.Media.Color.Parse("#4EC9B0"));
		}

		return Application.Current.Resources["MaterialDesignBody"] as IBrush;
	}
}