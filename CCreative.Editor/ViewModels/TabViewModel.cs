using AvaloniaEdit;
using Microsoft.CodeAnalysis;
using ReactiveUI;

namespace CCreative.Editor.ViewModels;

public class TabViewModel : ViewModelBase
{
	private string _title;
	private TextEditor _textEditor;


	public string Title
	{
		get => _title;
		set => this.RaiseAndSetIfChanged(ref _title, value);
	}

	public TextEditor TextEditor
	{
		get => _textEditor;
		set => this.RaiseAndSetIfChanged(ref _textEditor, value);
	}

	public DocumentId Id { get; }

	public TabViewModel(string title, TextEditor textEditor, DocumentId id)
	{
		Id = id;
		_title = title;
		TextEditor = textEditor;
	}
}