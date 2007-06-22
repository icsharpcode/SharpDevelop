// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;

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
	/// 
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
			noToolsAvailable.Text = "There are no tools available for the current document.";
			noToolsAvailable.Dock = DockStyle.Fill;
			panel.Controls.Add(noToolsAvailable);
			child = noToolsAvailable;
			
			WorkbenchSingleton.Workbench.ActiveWorkbenchWindowChanged += WorkbenchWindowChanged;
			WorkbenchWindowChanged(null, null);
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
		
		void WorkbenchWindowChanged(object sender, EventArgs e)
		{
			IToolsHost th = WorkbenchSingleton.Workbench.ActiveContent as IToolsHost;
			if (th == null) {
				IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
				if (window != null) {
					th = window.ActiveViewContent as IToolsHost;
				}
			}
			if (th != null) {
				SetChild(th.ToolsControl ?? noToolsAvailable);
			} else {
				SetChild(noToolsAvailable);
			}
		}
	}
}
