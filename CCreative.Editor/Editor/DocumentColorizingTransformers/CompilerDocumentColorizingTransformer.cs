using Avalonia.Media;
using AvaloniaEdit.Document;
using AvaloniaEdit.Rendering;
using CCreative.Compilers;
using CCreative.Editor.Extensions;
using Microsoft.CodeAnalysis;

namespace CCreative.Editor.Editor.DocumentColorizingTransformers;

public class CompilerDocumentColorizingTransformer : DocumentColorizingTransformer
{
	public Compiler Compiler { get; }
	public DocumentId Id { get; }

	public CompilerDocumentColorizingTransformer(Compiler compiler, DocumentId id)
	{
		Compiler = compiler;
		Id = id;
	}

	protected override async void ColorizeLine(DocumentLine line)
	{
		if (!line.IsDeleted)
		{
			var classifiers = Compiler.GetClassifiers(Id, line.Offset, line.Length);
			var errors = Compiler.GetErrors(Id);

			await foreach (var classifier in classifiers)
			{
				ChangeLinePart(classifier.Start, classifier.End, element => { element.TextRunProperties.ForegroundBrush = classifier.Type.GetForeground(); });
			}
		}
	}
}