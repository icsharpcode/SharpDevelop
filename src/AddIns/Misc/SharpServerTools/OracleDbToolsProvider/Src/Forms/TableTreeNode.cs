/*
 * User: dickon
 * Date: 23/09/2006
 * Time: 10:57
 * 
 */

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data;

using SharpDbTools.Forms;
using SharpDbTools.Data;

using SharpServerTools.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace SharpDbTools.Oracle.Forms
{
	/// <summary>
	/// specialisation of the TreeNode to add context menu and click handling
	/// to invoke the DescribeTable component for Oracle tables.
	/// Does not change any data or metadata content so does not need to be
	/// a ServerExplorerTreeNode
	/// </summary>
	
	class TableTreeNode: TreeNode
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
			TableDescribeViewContent tableDescribeViewContent = new TableDescribeViewContent(tableInfo, tableName);
			WorkbenchSingleton.Workbench.ShowView(tableDescribeViewContent);
		}
	}
}
