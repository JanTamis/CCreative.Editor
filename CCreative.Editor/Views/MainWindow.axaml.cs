using Avalonia;
using Avalonia.Markup.Xaml;

namespace CCreative.Editor.Views
{
	public partial class MainWindow : FluentWindow
	{
		public MainWindow()
		{
			InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
		}

		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);

			//var compiler = new Compiler.Compiler();

			// var editor = this.FindControl<RoslynCodeEditor>("Editor");
			//
			// editor.Initialize(compiler, new ClassificationHighlightColors(), Environment.CurrentDirectory, String.Empty ,SourceCodeKind.Regular);
		}
	}
}
