// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Russell Wilkins" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel.Design;
using System.Workflow.ComponentModel.Design;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace WorkflowDesigner
{
	/// <summary>
	/// Description of MenuCommandService.
	/// </summary>
	public class WorkflowMenuCommandService : System.ComponentModel.Design.MenuCommandService
	{
		
		public WorkflowMenuCommandService(IServiceProvider host) : base(host)
		{
			
		}
		
		public override void ShowContextMenu(CommandID menuID, int x, int y)
		{
			if (menuID == null)
				throw new ArgumentNullException("menuID");
			
			LoggingService.Debug("ShowContextMenu");
			
			if (menuID == WorkflowMenuCommands.DesignerActionsMenu)
			{
				ContextMenuStrip contextMenu = new ContextMenuStrip();
				
				ICollection collection = this.GetCommandList(menuID.Guid);
				foreach (System.ComponentModel.Design.MenuCommand menuCommand in collection)
				{
					// Only interested in the errors.
					if (menuCommand.CommandID.ID == 8342)
					{
						ToolStripMenuItem menuItem = new ToolStripMenuItem(menuCommand.Properties["Text"].ToString());
						menuItem.Click += ClickHandler;
						contextMenu.Items.Add(menuItem);
					}
				}

				WorkflowView workflowView = GetService(typeof(WorkflowView)) as WorkflowView;
				contextMenu.Show(workflowView , workflowView.PointToClient(new Point(x, y)));

			}
		}
		
		void ClickHandler(object sender, EventArgs e)
		{
			// TODO: Move focus to the property in the property pad.
			throw new NotImplementedException();
		}
		
	}
}

