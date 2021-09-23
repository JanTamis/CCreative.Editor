using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Platform;
using Avalonia.Styling;
using System;

namespace CCreative.Editor.Views
{
	public class FluentWindow : Window, IStyleable
	{
		Type IStyleable.StyleKey => typeof(Window);

		public FluentWindow()
		{
			if (Environment.OSVersion.Platform != PlatformID.Unix)
			{
				ExtendClientAreaToDecorationsHint = true;
				ExtendClientAreaTitleBarHeightHint = -1;
			}
			
			SystemDecorations = SystemDecorations.Full;
			TransparencyLevelHint = WindowTransparencyLevel.AcrylicBlur; 

			if (ActualTransparencyLevel != WindowTransparencyLevel.AcrylicBlur)
			{
				TransparencyLevelHint = WindowTransparencyLevel.None;
			}

			this.GetObservable(WindowStateProperty)
					.Subscribe(x =>
					{
						PseudoClasses.Set(":maximized", x == WindowState.Maximized);
						PseudoClasses.Set(":fullscreen", x == WindowState.FullScreen);
					});
		}

		protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
		{
			base.OnApplyTemplate(e);
			ExtendClientAreaChromeHints = ExtendClientAreaChromeHints.PreferSystemChrome;
		}
	}
}