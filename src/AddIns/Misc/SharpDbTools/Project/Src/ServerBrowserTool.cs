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
using System.Data.Common;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using SharpDbTools.Model;
using SharpDbTools.Connection;

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
			dbNode.Text = "Database Explorer";
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
			
			// TODO: NEXT: put this and the Rebuild... behaviour into a view builder; 
			
			// this is the wrong place for this, but lets give it a go...
			DbModelInfoService.LoadFromFiles();
			
			this.BeginUpdate();
			this.Nodes.Add(dbNode);
			RebuildDbConnectionsNode();
			this.EndUpdate();
		}
		
		/// <summary>
		/// Rebuilds the database connection tree.
		/// Should only be called from a delegate via Invoke or BeginInvoke.
		/// </summary>
		public void RebuildDbConnectionsNode()
		{
			// Rebuild each of the root nodes in the ServerToolTreeView
			this.BeginUpdate();
			dbNode.Nodes.Clear();
			foreach (string name in DbModelInfoService.Names) {
				TreeNode dbModelInfoNode = CreateDbModelInfoNode(name);
				dbNode.Nodes.Add(dbModelInfoNode);
			}
			this.EndUpdate();
		}
		
		public TreeNode CreateDbModelInfoNode(string name)
		{
			TreeNode treeNode = new TreeNode(name);
			// create and add the menustrip for this node
			
			DbModelInfoContextMenuStrip cMenu = new DbModelInfoContextMenuStrip(treeNode);
			
			// create menu items
			ToolStripMenuItem setConnectionStringMenuItem = 
				new ToolStripMenuItem("Set Connection String");
			setConnectionStringMenuItem.Click += new EventHandler(SetConnectionStringOnDbModelInfoClickHandler);
			
			ToolStripMenuItem loadMetadataMenuItem =
				new ToolStripMenuItem("Load Metadata from Connection");
			loadMetadataMenuItem.Click += new EventHandler(LoadMetadataClickHandler);
			
			cMenu.Items.AddRange(new ToolStripMenuItem[] 
			                     {
			                     	setConnectionStringMenuItem,
			                     	loadMetadataMenuItem	
			                     });
	
			treeNode.ContextMenuStrip = cMenu;
			
			// create sub TreeNodes for the connection string and invariant name if they exist
			DbModelInfo modelInfo = DbModelInfoService.GetDbModelInfo(name);
			string connectionString = modelInfo.ConnectionString;
			string invariantName = modelInfo.InvariantName;
			
			TreeNode attributesNode = new TreeNode("Connection Properties");
			treeNode.Nodes.Add(attributesNode);
			
			if (connectionString != null) {
				TreeNode cstringNode = new TreeNode("Connection String: " + connectionString);
				attributesNode.Nodes.Add(cstringNode);
			}
			
			if (invariantName != null) {
				TreeNode invNameNode = new TreeNode("Invariant Name: " + invariantName);
				attributesNode.Nodes.Add(invNameNode);
			}
			
			return treeNode;
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
			this.BeginInvoke(new MethodInvoker(this.RebuildDbConnectionsNode));
		}
		
		public void DeleteDbConnectionClickHandler(object sender, EventArgs e)
		{
			LoggingService.Debug("delete connection clicked");
		}
		
		public void SaveDbModelInfoClickHandler(object sender, EventArgs e)
		{
			LoggingService.Debug("save all metadata clicked");
			DbModelInfoService.SaveAll();
		}
		
		private static string getConnectionName(object sender)
		{
			ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
			DbModelInfoContextMenuStrip toolStrip = menuItem.Owner as DbModelInfoContextMenuStrip;
			TreeNode node = toolStrip.TreeNode;
			string connectionLogicalName = node.Text;
			return connectionLogicalName;
		}
		
		public void SetConnectionStringOnDbModelInfoClickHandler(object sender, EventArgs e)
		{
			string connectionLogicalName = getConnectionName(sender);
			LoggingService.Debug("add connection string clicked for item with name: " + connectionLogicalName);
			
			// use the ConnectionStringDefinitionDialog to get a connection string and invariant name
			ConnectionStringDefinitionDialog definitionDialog = new ConnectionStringDefinitionDialog();
			DialogResult result = definitionDialog.ShowDialog();
			
			// if the dialog was cancelled then do nothing
			if (result == DialogResult.Cancel) {
				return;
			}
			
			// if the dialog was submitted and connection string has changed then clear the DbModelInfo metadata
			// note that is is not required for the Connection string to be valid - it may be work
			// in progress and a user might want to save a partially formed connection string

			DbModelInfo dbModelInfo = DbModelInfoService.GetDbModelInfo(connectionLogicalName);
			string connectionString = dbModelInfo.ConnectionString;
			string newConnectionString = definitionDialog.ConnectionString;
			
			if (newConnectionString == null) {
				return;
			}
			
			dbModelInfo.ConnectionString = newConnectionString;
			dbModelInfo.InvariantName = definitionDialog.InvariantName;
			
			// rebuild the database explorer node
			this.BeginInvoke(new MethodInvoker(this.RebuildDbConnectionsNode));
		}
		
		public void LoadMetadataClickHandler(object sender, EventArgs args)
		{
			string connectionLogicalName = getConnectionName(sender);
			LoggingService.Debug("load metadata from connection clicked for connection with name: "
			                     + connectionLogicalName);
			try {	
				DbModelInfoService.LoadMetadataFromConnection(connectionLogicalName);
			}
			catch(DbException e) {
				MessageService.ShowError(e, 
				"An Exception was thrown while trying to connect to: " + connectionLogicalName);                       
			}
		}
	}
	
	class DbModelInfoContextMenuStrip : ContextMenuStrip
	{
		TreeNode treeNodeAttached;
		
		public DbModelInfoContextMenuStrip(TreeNode treeNodeAttached) : base()
		{
			this.treeNodeAttached = treeNodeAttached;		
		}
		
		public TreeNode TreeNode {
			get {
				return treeNodeAttached;
			}
		}
	}
}





