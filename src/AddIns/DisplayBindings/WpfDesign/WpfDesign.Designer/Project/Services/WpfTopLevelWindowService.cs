// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 2667$</version>
// </file>

using System;
using System.Windows;

namespace ICSharpCode.WpfDesign.Designer.Services
{
	sealed class WpfTopLevelWindowService : ITopLevelWindowService
	{
		public ITopLevelWindow GetTopLevelWindow(System.Windows.UIElement element)
		{
			Window window = Window.GetWindow(element);
			if (window != null)
				return new WpfTopLevelWindow(window);
			else
				return null;
		}
		
		sealed class WpfTopLevelWindow : ITopLevelWindow
		{
			Window window;
			
			public WpfTopLevelWindow(Window window)
			{
				this.window = window;
			}
			
			public void SetOwner(Window child)
			{
				child.Owner = window;
			}
			
			public bool Activate()
			{
				return window.Activate();
			}
		}
	}
}
