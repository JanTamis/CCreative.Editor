using CCreative.Editor.Models;
using System.Collections.ObjectModel;

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
	}
}
