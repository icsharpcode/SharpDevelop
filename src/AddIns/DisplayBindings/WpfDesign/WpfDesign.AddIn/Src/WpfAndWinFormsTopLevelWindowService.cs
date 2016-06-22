// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
