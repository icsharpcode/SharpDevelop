// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Controls;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// Implement this interface to make your view content display tools in the tool box.
	/// </summary>
	public interface IToolsHost
	{
		/// <summary>
		/// Gets the control to display in the tool box.
		/// </summary>
		object ToolsContent { get; }
	}
	
	/// <summary>
	/// A pad that shows a single child control determined by the document that currently has the focus.
	/// </summary>
	public class ToolsPad : AbstractPadContent
	{
		ContentPresenter contentControl = new ContentPresenter();
		
		public override object Control {
			get {
				return contentControl;
			}
		}
		
		public ToolsPad()
		{
			WorkbenchSingleton.Workbench.ActiveViewContentChanged += WorkbenchActiveContentChanged;
			WorkbenchActiveContentChanged(null, null);
		}
		
		void WorkbenchActiveContentChanged(object sender, EventArgs e)
		{
			IToolsHost th = WorkbenchSingleton.Workbench.ActiveViewContent as IToolsHost;
			if (th != null && th.ToolsContent != null) {
				contentControl.SetContent(th.ToolsContent, th);
			} else {
				contentControl.SetContent(StringParser.Parse("${res:SharpDevelop.SideBar.NoToolsAvailableForCurrentDocument}"));
			}
		}
	}
}
