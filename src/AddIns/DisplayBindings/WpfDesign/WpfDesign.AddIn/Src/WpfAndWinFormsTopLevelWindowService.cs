// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media;

namespace ICSharpCode.WpfDesign.AddIn
{
	sealed class WpfAndWinFormsTopLevelWindowService : ITopLevelWindowService
	{
		public ITopLevelWindow GetTopLevelWindow(UIElement element)
		{
			Window window = Window.GetWindow(element);
			if (window != null) {
				return new WpfTopLevelWindow(window);
			}
			HwndSource hwndSource = PresentationSource.FromVisual(element) as HwndSource;
			if (hwndSource != null) {
				Control ctl = Control.FromChildHandle(hwndSource.Handle);
				if (ctl != null) {
					Form form = ctl.FindForm();
					if (form != null) {
						return new WindowsFormsTopLevelWindow(form);
					}
				}
			}
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
		
		sealed class WindowsFormsTopLevelWindow : ITopLevelWindow
		{
			Form form;
			
			public WindowsFormsTopLevelWindow(Form form)
			{
				this.form = form;
			}
			
			public void SetOwner(Window child)
			{
				(new WindowInteropHelper(child)).Owner = form.Handle;
			}
			
			public bool Activate()
			{
				return form.Focus();
			}
		}
	}
}
