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
using System.Windows.Controls;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Workbench;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// Implement this interface to make a view content display tools in the outline pad.
	/// </summary>
	[ViewContentService]
	public interface IOutlineContentHost
	{
		/// <summary>
		/// Gets the control to display in the outline pad.
		/// </summary>
		object OutlineContent { get; }
	}
	
	/// <summary>
	/// A pad that shows a single child control determined by the document that currently has the focus.
	/// </summary>
	public class OutlinePad : AbstractPadContent
	{
		ContentPresenter contentControl = new ContentPresenter();
		
		public override object Control {
			get {
				return contentControl;
			}
		}
		
		public OutlinePad()
		{
			SD.Workbench.ActiveViewContentChanged += WorkbenchActiveContentChanged;
			WorkbenchActiveContentChanged(null, null);
		}
		
		void WorkbenchActiveContentChanged(object sender, EventArgs e)
		{
			IViewContent view = SD.Workbench.ActiveViewContent;
			if (view != null) {
				IOutlineContentHost content = view.GetService(typeof(IOutlineContentHost)) as IOutlineContentHost;
				if (content != null) {
					SD.WinForms.SetContent(contentControl, content.OutlineContent, view);
					return;
				}
			}
			SD.WinForms.SetContent(contentControl, StringParser.Parse("${res:MainWindow.Windows.OutlinePad.NoContentAvailable}"));
		}
	}
}
