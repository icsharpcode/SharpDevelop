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
			
			if (menuID == WorkflowMenuCommands.DesignerActionsMenu) {
				ContextMenuStrip contextMenu = new ContextMenuStrip();
				
				Guid DesignerActionGuid = new Guid("3bd4a275-fccd-49f0-b617-765ce63b4340");
				
				ICollection collection = this.GetCommandList(menuID.Guid);
				foreach (System.ComponentModel.Design.MenuCommand menuCommand in collection) {
					// Only interested in the errors.
					if (menuCommand.CommandID.ID == 8342) {
						foreach (object o in menuCommand.Properties.Keys)
							LoggingService.DebugFormatted("{0} {1}", o.GetType(), o.ToString());
						foreach (object o in menuCommand.Properties.Values)
							LoggingService.DebugFormatted("{0} {1}", o.GetType(), o.ToString());
						ToolStripMenuItem menuItem = new ToolStripMenuItem(menuCommand.Properties["Text"].ToString());
						menuItem.Click += new EventHandler(ClickHandler);
						menuItem.Tag = menuCommand.Properties[DesignerActionGuid];
						contextMenu.Items.Add(menuItem);
					}
				}

				WorkflowView workflowView = GetService(typeof(WorkflowView)) as WorkflowView;
				contextMenu.Show(workflowView , workflowView.PointToClient(new Point(x, y)));

			}
		}
		
		void ClickHandler(object sender, EventArgs e)
		{
			DesignerAction designerAction = ((ToolStripMenuItem)sender).Tag as DesignerAction;
			if (designerAction == null)
				return;
			
			designerAction.Invoke();  // Will change the selectedObject in the designer
			
			if (!string.IsNullOrEmpty( designerAction.PropertyName))
			{
				// No easy way to search for a grid item so
				// find the root item in the grid, and search for items for the property.
				GridItem item = PropertyPad.Grid.SelectedGridItem;
				while (item.Parent != null) {
					item = item.Parent;
				}
				GridItem item2 = FindGridItem(item, designerAction.PropertyName);
				
				if (item2 != null) {
					PropertyPad.Grid.SelectedGridItem = item2;
					PropertyPad.Grid.Focus();
				}
			}
			
		}

		static GridItem FindGridItem(GridItem gridItem, string name)
		{
			foreach (GridItem item in gridItem.GridItems){
				if (item.Label == name)
					return item;
				
				GridItem item2 = FindGridItem(item, name);
				if (item2 != null)
					return item2;
			}
			
			return null;
		}
		
	}
}

