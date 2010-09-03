// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
