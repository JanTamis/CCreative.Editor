using Avalonia.Controls;
using Avalonia.Controls.Templates;
using CCreative.Editor.ViewModels;
using System;

namespace CCreative.Editor
{
	public class ViewLocator : IDataTemplate
	{
		public bool SupportsRecycling => false;

		public IControl Build(object data)
		{
			var name = data.GetType().FullName!.Replace("ViewModel", "View");
			var type = Type.GetType(name);

			if (type != null)
			{
				return (IControl)Activator.CreateInstance(type)!;
			}

			return new TextBlock { Text = $"Not Found: {name}" };
		}

		public bool Match(object data)
		{
			return data is ViewModelBase;
		}
	}
}