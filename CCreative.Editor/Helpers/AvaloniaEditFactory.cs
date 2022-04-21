using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.VisualTree;
using AvaloniaEdit;
using AvaloniaEdit.CodeCompletion;
using CCreative.Compilers;
using CCreative.Compilers.Models;
using CCreative.Editor.Editor.DocumentColorizingTransformers;
using CCreative.Editor.Extensions;
using Material.Styles;
using Material.Styles.Assists;
using Microsoft.CodeAnalysis;

namespace CCreative.Editor.Helpers;

public static class TextEditorFactory
{
	public static Card tooltip;

	static TextEditorFactory()
	{
		tooltip = new Card
		{
			IsVisible = false,
			IsHitTestVisible = false,
			Content = new StackPanel(),
			ClipToBounds = true,
			Margin = new Thickness(5),
		};
		ShadowAssist.SetShadowDepth(tooltip, ShadowDepth.CenterDepth1);
	}

	public static TextEditor CreateTextEditor()
	{
		var editor = new TextEditor
		{
			Background = Brushes.Transparent,
			FontFamily = new FontFamily("Cascadia Code"),
			ShowLineNumbers = true,
			FontSize = 16,
			HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
			VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
		};

		var options = new TextEditorOptions(editor.Options)
		{
			IndentationSize = 2,
			AllowScrollBelowDocument = true,
		};

		editor.Options = options;

		return editor;
	}

	public static void RegisterLayers(TextEditor editor, DocumentId id, Compiler compiler)
	{
		editor.TextArea.TextView.LineTransformers.Add(new CompilerDocumentColorizingTransformer(compiler, id));
		editor.TextArea.TextView.LineTransformers.Add(new ErrorTransformer(compiler, id));
	}

	public static void RegisterEvents(TextEditor editor, DocumentId id, Compiler compiler)
	{
		editor.AttachedToVisualTree += delegate
		{
			var layer = OverlayLayer.GetOverlayLayer(editor);

			if (!layer.Children.Contains(tooltip))
			{
				layer.Children.Add(tooltip);
			}
		};

		editor.PointerHover += async (_, args) =>
		{
			var mousePosition = args.GetPosition(editor.GetVisualRoot());
			var editorMousePosition = args.GetPosition(editor) + editor.TextArea.TextView.ScrollOffset;

			var textViewPosition = editor.TextArea.TextView.GetPositionFloor(new Point(editorMousePosition.X - editor.TextArea.LeftMargins.Sum(s => s.Bounds.Width), editorMousePosition.Y));

			tooltip.IsVisible = false;

			if (textViewPosition.HasValue && editor.TextArea.Document.TextLength > 0)
			{
				var position = Math.Constrain(editor.TextArea.TextView.Document.GetOffset(textViewPosition.Value.Line, textViewPosition.Value.Column) - 1, 0, editor.TextArea.Document.TextLength - 1);

				if (tooltip.Content is StackPanel mainPanel && !Char.IsWhiteSpace(editor.Document.GetCharAt(position)))
				{
					mainPanel.Children.Clear();

					var errors = compiler.GetErrors(id);
					await GenerateErrors(errors, position, editor.FontFamily, mainPanel);

					var info = compiler.GetInfo(id, position);
					await GenerateQuickInfo(info, editor.FontFamily, mainPanel);

					if (mainPanel.Children.Any())
					{
						tooltip.Margin = new Thickness(mousePosition.X, mousePosition.Y, 5, 5);
						tooltip.IsVisible = true;
					}
				}
			}
		};

		editor.PointerHoverStopped += delegate { tooltip.IsVisible = false; };

		editor.TextChanged += delegate
		{
			compiler.UpdateDocument(id, editor.Text);
			editor.TextArea.TextView.Redraw();
		};

		CompletionWindow? completionWindow = null;

		editor.TextArea.TextEntered += async (sender, args) =>
		{
			var completionChar = editor.Document.GetCharAt(editor.CaretOffset - 1);

			if (completionWindow?.IsOpen != true && !CommitCharacters(completionChar))
			{
				var results = compiler.GetCompletions(id, editor.CaretOffset);

				// Open code completion after the user has pressed dot:
				completionWindow = new CompletionWindow(editor.TextArea)
				{
					MinWidth = 300,
					CloseWhenCaretAtBeginning = true,
				};
				if (char.IsLetterOrDigit(completionChar))
				{
					completionWindow.StartOffset -= 1;
				}

				var data = completionWindow.CompletionList.CompletionData;

				await foreach (var completionItem in results)
				{
					data.Add(new CompletionData(completionItem, compiler, id));
				}

				completionWindow.CompletionList.SelectedItem = data.FirstOrDefault(f => f.Priority > 0);

				if (data.Any())
				{
					completionWindow.Closed += (o, _) => completionWindow = null;
					completionWindow.Show();
				}
				else
				{
					completionWindow = null;
				}
			}
		};

		editor.TextArea.TextEntering += (sender, args) =>
		{
			if (args.Text?.Length > 0 && completionWindow is not null)
			{
				if (CommitCharacters(args.Text[0]))
				{
					// Whenever no identifier letter is typed while the completion window is open,
					// insert the currently selected element.
					completionWindow.CompletionList.RequestInsertion(args);
				}
			}
		};

		bool CommitCharacters(char c)
		{
			return char.IsWhiteSpace(c) || c is ' ' or '{' or '}' or '[' or ']' or '(' or ')'
				or '.' or ',' or ':' or ';' or '+' or '-' or '*' or '/'
				or '%' or '&' or '|' or '^' or '!' or '~' or '=' or '<'
				or '>' or '?' or '@' or '#' or '\'' or '\"' or '\\' or '\n' or '\r';
		}
	}

	private static async Task GenerateQuickInfo(IAsyncEnumerable<QuickInfoGroupModel> info, FontFamily family, StackPanel mainPanel)
	{
		await foreach (var infoItem in info.ConfigureAwait(false))
		{
			if (infoItem.Infos is not null)
			{
				var stack = new StackPanel
				{
					Orientation = Orientation.Horizontal,
				};

				foreach (var model in infoItem.Infos)
				{
					if (model.Text is not "")
					{
						stack.Children.Add(new TextBlock
						{
							Text = model.Text,
							FontSize = 14,
							FontFamily = family,
							Foreground = model.Type.GetForeground(),
						});
					}
				}

				mainPanel.Children.Add(stack);
			}
		}
	}

	private static async Task GenerateErrors(IAsyncEnumerable<Diagnostic> errors, int position, FontFamily family, IPanel mainPanel)
	{
		await foreach (var diagnostic in errors.ConfigureAwait(false))
		{
			if (diagnostic.Location.SourceSpan.Contains(position) && diagnostic.Severity is DiagnosticSeverity.Error)
			{
				var stack = new StackPanel
				{
					Orientation = Orientation.Vertical,
				};

				stack.Children.Add(new TextBlock()
				{
					Text = diagnostic.GetMessage(),

					MaxWidth = 400,
					TextWrapping = TextWrapping.WrapWithOverflow,
					FontSize = 14,
					FontFamily = family,
				});

				mainPanel.Children.Add(stack);
			}
		}
	}
}

