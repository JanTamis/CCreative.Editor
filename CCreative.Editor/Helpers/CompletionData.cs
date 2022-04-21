using System;
using Avalonia.Media.Imaging;
using AvaloniaEdit.CodeCompletion;
using AvaloniaEdit.Document;
using AvaloniaEdit.Editing;
using CCreative.Compilers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Completion;

namespace CCreative.Editor.Helpers;

public class CompletionData : ICompletionData
{
	private CompletionItem item;
	private DocumentId id;
	private Compiler compiler;


	public IBitmap Image { get; }
	public string Text => item.DisplayText;

	public object Content => item.DisplayText;
	public object Description { get; }
	public double Priority => item.Rules.MatchPriority;

	public CompletionData(CompletionItem item, Compiler compiler, DocumentId id)
	{
		this.item = item;
		this.compiler = compiler;
		this.id = id;
	}

	public async void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
	{
		var changes = await compiler.GetChanges(id, item).ConfigureAwait(true);

		var textChange = changes.TextChange;
		var document = textArea.Document;

		using (document.RunUpdate())
		{
			// we may need to remove a few typed chars since the Roslyn document isn't updated
			// while the completion window is open
			if (completionSegment.EndOffset > textChange.Span.End)
			{
				document.Replace(
					new TextSegment { StartOffset = textChange.Span.End, EndOffset = completionSegment.EndOffset },
					string.Empty);
			}

			document.Replace(textChange.Span.Start, textChange.Span.Length,
				new StringTextSource(textChange.NewText));
		}

		if (changes.NewPosition != null)
		{
			textArea.Caret.Offset = changes.NewPosition.Value;
		}
	}
}

