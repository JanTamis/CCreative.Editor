using CCreative.Editor.Models;
using System.Collections.ObjectModel;
using CCreative.Compilers;

namespace CCreative.Editor.ViewModels
{
	public class MainWindowViewModel : ViewModelBase
	{
		private ObservableCollection<TabViewModel> Tabs { get; set; } = new()
		{
			new TabViewModel("Hello World"),
			new TabViewModel("Test"),
			new TabViewModel("Testing"),
		};

		public MainWindowViewModel()
		{
			using var compiler = new Compiler();

			var project = compiler.CreateProject("project", "Project");
		}
	}
}