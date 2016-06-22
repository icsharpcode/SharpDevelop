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
using System.Windows.Controls;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Workbench;
using ICSharpCode.WpfDesign.Designer.ThumbnailView;

namespace ICSharpCode.WpfDesign.AddIn
{
	public class ThumbnailViewPad : AbstractPadContent
	{
		ContentPresenter contentControl = new ContentPresenter();

		ThumbnailView thumbnailView = new ThumbnailView();

		TextBlock notAvailableTextBlock = new TextBlock {
			Text = StringParser.Parse("${res:ICSharpCode.SharpDevelop.Gui.OutlinePad.NotAvailable}"),
			TextWrapping = TextWrapping.Wrap
		};
		
		public ThumbnailViewPad()
		{
			SD.Workbench.ActiveViewContentChanged += WorkbenchActiveViewContentChanged;
			WorkbenchActiveViewContentChanged(null, null);
		}

		void WorkbenchActiveViewContentChanged(object sender, EventArgs e)
		{
			WpfViewContent wpfView = SD.Workbench.ActiveViewContent as WpfViewContent;
			if (wpfView != null)
			{
				thumbnailView.DesignSurface = wpfView.DesignSurface;
				SD.WinForms.SetContent(contentControl, thumbnailView);
			} else {
				SD.WinForms.SetContent(contentControl, notAvailableTextBlock);
			}
		}
		
		/// <summary>
		/// The <see cref="System.Windows.Forms.Control"/> representing the pad
		/// </summary>
		public override object Control {
			get {
				return contentControl;
			}
		}
	}
}
