// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
