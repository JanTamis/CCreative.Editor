using Avalonia.Media;
using AvaloniaEdit.Document;
using AvaloniaEdit.Rendering;
using CCreative.Compilers;
using Microsoft.CodeAnalysis;

namespace CCreative.Editor.Editor.DocumentColorizingTransformers;

public class ErrorTransformer : DocumentColorizingTransformer
{
	public Compiler Compiler { get; }

	public DocumentId Id { get; }

	public ErrorTransformer(Compiler compiler, DocumentId id)
	{
		Compiler = compiler;
		Id = id;
	}

	protected override async void ColorizeLine(DocumentLine line)
	{
		if (!line.IsDeleted)
		{
			var errors = Compiler.GetErrors(Id);

			await foreach (var error in errors)
			{
				if (error.Severity is DiagnosticSeverity.Error)
				{
					var location = error.Location.SourceSpan;

					if (location.Start >= line.Offset && location.End < line.EndOffset)
					{
						ChangeLinePart(location.Start, location.End, element => { element.TextRunProperties.ForegroundBrush = Brushes.Red; });
					}
				}
			}
		}
	}
}