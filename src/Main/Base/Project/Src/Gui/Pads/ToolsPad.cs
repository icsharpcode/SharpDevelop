// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

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
		ContentControl contentControl = new ContentControl();
		
		public override object Content {
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
				contentControl.SetContent(th.ToolsContent);
			} else {
				contentControl.SetContent(StringParser.Parse("${res:SharpDevelop.SideBar.NoToolsAvailableForCurrentDocument}"));
			}
		}
	}
}
