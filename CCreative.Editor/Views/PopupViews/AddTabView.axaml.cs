using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace CCreative.Editor.Views.PopupViews;

public class AddTabView : UserControl
{
	public AddTabView()
	{
		InitializeComponent();
	}

	private void InitializeComponent()
	{
		AvaloniaXamlLoader.Load(this);
	}
}