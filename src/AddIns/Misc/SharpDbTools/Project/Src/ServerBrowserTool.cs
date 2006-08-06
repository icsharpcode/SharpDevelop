// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Dickon Field" email=""/>
//     <version>$Revision$</version>
// </file>

/*
 * User: Dickon Field
 * Date: 12/06/2006
 * Time: 06:25
 */

using System;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using SharpDbTools.Model;

namespace SharpDbTools
{
	/// <summary>
	/// Enables a user to browse metadata associated with a db server and to open resources
	/// referenced therein. The intention is to extend this to other server processes over
	/// time.
	/// </summary>
	public class ServerBrowserTool : AbstractPadContent
	{
		Panel ctl;
		ServerBrowserToolController controller;
		
		/// <summary>
		/// ServerBrowserTool hosts one or more TreeViews providing views of types
		/// of server. Currently it shows only relational database servers.
		/// </summary>
		public ServerBrowserTool()
		{
			LoggingService.Debug("Loading ServerBrowserTool");
			controller = ServerBrowserToolController.GetInstance();
			ServerToolTreeView dbTree = new ServerToolTreeView();
			dbTree.Dock = DockStyle.Fill;
			ctl = new Panel();
			ctl.Controls.Add(dbTree);			
		}
		
		/// <summary>
		/// The <see cref="System.Windows.Forms.Control"/> representing the pad
		/// </summary>
		public override Control Control {
			get {
				return ctl;
			}
		}
		
		/// <summary>
		/// Rebuildes the pad
		/// </summary>
		public override void RedrawContent()
		{
			// TODO: Rebuild the whole pad control here, renew all resource strings whatever
			//       Note that you do not need to recreate the control.
		}
		
		/// <summary>
		/// Cleans up all used resources
		/// </summary>
		public override void Dispose()
		{
			ctl.Dispose();
		}
	}
	
	class ServerToolTreeView : TreeView
	{
		TreeNode dbNode;
		
		public ServerToolTreeView(): base()
		{
			dbNode = new TreeNode();
			dbNode.Text = "Database Connections";
			dbNode.Tag = "DatabaseConnections";
			
			// create the context menu for the database server node
			ContextMenuStrip cMenu = new ContextMenuStrip();
			ToolStripMenuItem addConnectionMenuItem = 
				new ToolStripMenuItem("Add Connection");
			addConnectionMenuItem.Click += new EventHandler(AddDbConnectionClickHandler);
			
			ToolStripMenuItem deleteConnectionMenuItem = 
				new ToolStripMenuItem("Delete Connection");
			deleteConnectionMenuItem.Click += new EventHandler(DeleteDbConnectionClickHandler);
			
			ToolStripMenuItem saveMetadataMenuItem =
				new ToolStripMenuItem("Save All");
			saveMetadataMenuItem.Click += new EventHandler(SaveDbModelInfoClickHandler);
			
			
			cMenu.Items.AddRange(new ToolStripMenuItem[] 
			                     {	
			                     	addConnectionMenuItem,
			                     	deleteConnectionMenuItem,
			                     	saveMetadataMenuItem
			                     } 
			                    );
			dbNode.ContextMenuStrip = cMenu;
			
			this.BeginUpdate();
			this.Nodes.Add(dbNode);
			this.EndUpdate();
		}
		
		/// <summary>
		/// Rebuilds the database connection tree.
		/// Should only be called from a delegate via Invoke or BeginInvoke.
		/// </summary>
		public void RebuildDbConnectionNode()
		{
			// Rebuild each of the root nodes in the ServerToolTreeView
			this.BeginUpdate();
			dbNode.Nodes.Clear();
			foreach (string name in DbModelInfoService.Names) {
				dbNode.Nodes.Add(new TreeNode(name));
			}
			this.EndUpdate();
		}
		
		/// <summary>
		/// Uses a dialog to get the logical name of a new Connection then
		/// adds a new DbModelInfo for it to the cache and updates the DatabaseServer 
		/// Tree.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void AddDbConnectionClickHandler(object sender, EventArgs e)
		{
			LoggingService.Debug("add connection clicked");
			
			// get the logical name of the new connection
			string logicalName =  null;			
			using (GetConnectionLogicalNameDialog dialog = new GetConnectionLogicalNameDialog()) {
				dialog.ShowDialog();
				logicalName = dialog.LogicalConnectionName;
			}
			if (logicalName.Equals("") || logicalName == null) return;
			
			LoggingService.Debug("name received is: " + logicalName);
			
			// add a new DbModelInfo to the cache			
			DbModelInfoService.Add(logicalName, null, null);
			
			// rebuild the database server node
			this.BeginInvoke(new MethodInvoker(this.RebuildDbConnectionNode));
		}
		
		public void DeleteDbConnectionClickHandler(object sender, EventArgs e)
		{
			LoggingService.Debug("delete connection clicked");
		}
		
		public void SaveDbModelInfoClickHandler(object sender, EventArgs e)
		{
			LoggingService.Debug("save all metadata clicked");
		}	

	}	
}





