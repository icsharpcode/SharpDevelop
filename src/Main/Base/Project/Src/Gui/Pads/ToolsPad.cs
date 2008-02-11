// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;
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
		Control ToolsControl { get; }
	}
	
	/// <summary>
	/// A pad that shows a single child control determined by the document that currently has the focus.
	/// </summary>
	public class ToolsPad : AbstractPadContent
	{
		Panel panel = new Panel();
		Label noToolsAvailable = new Label();
		Control child;
		
		public override Control Control {
			get {
				return panel;
			}
		}
		
		public ToolsPad()
		{
			noToolsAvailable.Text = StringParser.Parse("${res:SharpDevelop.SideBar.NoToolsAvailableForCurrentDocument}");
			noToolsAvailable.Dock = DockStyle.Fill;
			panel.Controls.Add(noToolsAvailable);
			child = noToolsAvailable;
			
			WorkbenchSingleton.Workbench.ActiveViewContentChanged += WorkbenchActiveContentChanged;
			WorkbenchActiveContentChanged(null, null);
		}
		
		void SetChild(Control newChild)
		{
			if (child != newChild) {
				panel.Controls.Clear();
				newChild.Dock = DockStyle.Fill;
				panel.Controls.Add(newChild);
				child = newChild;
			}
		}
		
		void WorkbenchActiveContentChanged(object sender, EventArgs e)
		{
			IToolsHost th = WorkbenchSingleton.Workbench.ActiveViewContent as IToolsHost;
			if (th != null) {
				SetChild(th.ToolsControl ?? noToolsAvailable);
			} else {
				SetChild(noToolsAvailable);
			}
		}
	}
}
