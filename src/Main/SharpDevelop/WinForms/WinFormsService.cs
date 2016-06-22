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
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop.Widgets;
using ICSharpCode.SharpDevelop.Workbench;

namespace ICSharpCode.SharpDevelop.WinForms
{
	/// <summary>
	/// Allows printing using the IPrintable interface.
	/// </summary>
	sealed class WinFormsService : IWinFormsService
	{
		public void Print(IPrintable printable)
		{
			using (PrintDocument pdoc = printable.PrintDocument) {
				if (pdoc != null) {
					using (PrintDialog ppd = new PrintDialog()) {
						ppd.Document  = pdoc;
						ppd.AllowSomePages = true;
						if (ppd.ShowDialog(SD.WinForms.MainWin32Window) == DialogResult.OK) { // fixed by Roger Rubin
							pdoc.Print();
						}
					}
				} else {
					MessageService.ShowError("${res:ICSharpCode.SharpDevelop.Commands.Print.CreatePrintDocumentError}");
				}
			}
		}
		
		public void PrintPreview(IPrintable printable)
		{
			using (PrintDocument pdoc = printable.PrintDocument) {
				if (pdoc != null) {
					PrintPreviewDialog ppd = new PrintPreviewDialog();
					ppd.TopMost   = true;
					ppd.Document  = pdoc;
					ppd.Show(SD.WinForms.MainWin32Window);
				} else {
					MessageService.ShowError("${res:ICSharpCode.SharpDevelop.Commands.Print.CreatePrintDocumentError}");
				}
			}
		}
		
		public IWinFormsToolbarService ToolbarService {
			get {
				return SD.GetRequiredService<IWinFormsToolbarService>();
			}
		}
		
		public IWinFormsMenuService MenuService {
			get {
				return SD.GetRequiredService<IWinFormsMenuService>();
			}
		}
		
		public Font DefaultMonospacedFont {
			get {
				return WinFormsResourceService.DefaultMonospacedFont;
			}
		}
		
		public IWin32Window MainWin32Window {
			get {
				return (WpfWorkbench)SD.Workbench;
			}
		}
		
		public Font LoadDefaultMonospacedFont(FontStyle style)
		{
			return WinFormsResourceService.LoadDefaultMonospacedFont(style);
		}
		
		public Font LoadFont(Font baseFont, FontStyle newStyle)
		{
			return WinFormsResourceService.LoadFont(baseFont, newStyle);
		}
		
		public Font LoadFont(string fontName, int size, FontStyle style)
		{
			return WinFormsResourceService.LoadFont(fontName, size, style);
		}
		
		public Bitmap GetResourceServiceBitmap(string resourceName)
		{
			return WinFormsResourceService.GetBitmap(resourceName);
		}
		
		public Icon GetResourceServiceIcon(string resourceName)
		{
			return WinFormsResourceService.GetIcon(resourceName);
		}
		
		public Icon BitmapToIcon(Bitmap bitmap)
		{
			return WinFormsResourceService.BitmapToIcon(bitmap);
		}
		
		public void ApplyRightToLeftConverter(Control control, bool recurse)
		{
			if (recurse)
				RightToLeftConverter.ConvertRecursive(control);
			else
				RightToLeftConverter.Convert(control);
		}
		
		public void SetContent(System.Windows.Controls.ContentControl contentControl, object content, IServiceProvider serviceProvider)
		{
			if (contentControl == null)
				throw new ArgumentNullException("contentControl");
			// serviceObject = object implementing the old clipboard/undo interfaces
			// to allow WinForms AddIns to handle WPF commands
			
			var host = contentControl.Content as SDWindowsFormsHost;
			if (host != null) {
				if (host.Child == content) {
					host.ServiceProvider = serviceProvider;
					return;
				}
				host.Dispose();
			}
			if (content is System.Windows.Forms.Control) {
				contentControl.Content = new SDWindowsFormsHost {
					Child = (System.Windows.Forms.Control)content,
					ServiceProvider = serviceProvider,
					DisposeChild = false
				};
			} else if (content is string) {
				contentControl.Content = new System.Windows.Controls.TextBlock {
					Text = content.ToString(),
					TextWrapping = System.Windows.TextWrapping.Wrap
				};
			} else {
				contentControl.Content = content;
			}
		}
		
		public void SetContent(System.Windows.Controls.ContentPresenter contentControl, object content, IServiceProvider serviceProvider)
		{
			if (contentControl == null)
				throw new ArgumentNullException("contentControl");
			// serviceObject = object implementing the old clipboard/undo interfaces
			// to allow WinForms AddIns to handle WPF commands
			
			var host = contentControl.Content as SDWindowsFormsHost;
			if (host != null) {
				if (host.Child == content) {
					host.ServiceProvider = serviceProvider;
					return;
				}
				host.Dispose();
			}
			if (content is System.Windows.Forms.Control) {
				contentControl.Content = new SDWindowsFormsHost {
					Child = (System.Windows.Forms.Control)content,
					ServiceProvider = serviceProvider,
					DisposeChild = false
				};
			} else if (content is string) {
				contentControl.Content = new System.Windows.Controls.TextBlock {
					Text = content.ToString(),
					TextWrapping = System.Windows.TextWrapping.Wrap
				};
			} else {
				contentControl.Content = content;
			}
		}
		
		public CustomWindowsFormsHost CreateWindowsFormsHost(IServiceProvider serviceProvider = null, bool processShortcutsInWPF = false)
		{
			return new SDWindowsFormsHost(processShortcutsInWPF) {
				ServiceProvider = serviceProvider,
				DisposeChild = false
			};
		}

		public void InvalidateCommands()
		{
			System.Windows.Input.CommandManager.InvalidateRequerySuggested();
		}
	}
}
