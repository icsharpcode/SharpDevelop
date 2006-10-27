/*
 * User: dickon
 * Date: 23/09/2006
 * Time: 10:57
 * 
 */

using System;
using System.Data;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using SharpDbTools.Data;
using SharpServerTools.Forms;

namespace SharpDbTools.Forms
{
	/// <summary>
	/// specialisation of the TreeNode to add context menu and click handling
	/// to invoke the DescribeTable component for Oracle tables.
	/// </summary>
	
	public class TableTreeNode: TreeNode
	{
		string logicalConnectionName;
		
		public TableTreeNode(string objectName, string logicalConnectionName): base(objectName)
		{
			this.logicalConnectionName = logicalConnectionName;
			NodeAwareContextMenuStrip cMenu = new NodeAwareContextMenuStrip(this);
			ToolStripMenuItem invokeDescriberMenuItem = new ToolStripMenuItem("Describe");
			invokeDescriberMenuItem.Click += new EventHandler(DescribeTableClickHandler);
			cMenu.Items.Add(invokeDescriberMenuItem);
			this.ContextMenuStrip = cMenu;
		}
		
		public void DescribeTableClickHandler(object sender, EventArgs args)
		{
			ToolStripMenuItem item = sender as ToolStripMenuItem;
			NodeAwareContextMenuStrip s = item.Owner as NodeAwareContextMenuStrip;
			string tableName = s.TreeNode.Text;
			LoggingService.Debug("describe table clicked for: " + logicalConnectionName + " and table name: " + tableName);
			DataTable tableInfo = DbModelInfoService.GetTableInfo(logicalConnectionName, tableName);
			string invariantName = DbModelInfoService.GetDbModelInfo(logicalConnectionName).InvariantName;
			// TODO: get field names and column header names from factory
			FormsArtefactFactory factory = FormsArtefactFactories.GetFactory(invariantName);
			
			TableDescribeViewContent tableDescribeViewContent =
				new TableDescribeViewContent(tableInfo, tableName, factory.GetDescribeTableFieldNames(),
				                             factory.GetDescribeTableColumnHeaderNames());
			WorkbenchSingleton.Workbench.ShowView(tableDescribeViewContent);
		}
	}
}
