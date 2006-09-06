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
using System.Data;
using System.Data.Common;
using System.Collections.Generic;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using SharpDbTools.Model;
using SharpDbTools.Connection;
using SharpDbTools.Viewer;

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
		public ServerToolTreeView(): base()
		{				
			// this is the wrong place for this, but lets give it a go...
			DbModelInfoService.LoadNamesFromFiles();
			Rebuild();

		}
		
		/// <summary>
		/// Rebuilds the database connection tree.
		/// Should only be called from a delegate via Invoke or BeginInvoke.
		/// </summary>
		public void Rebuild()
		{
			this.BeginUpdate();
			// TODO: put the Rebuild... behaviour into a view builder;
			TreeNode dbNode = null;
			TreeNode[] dbNodes = this.Nodes.Find("DatabaseExplorer", true);
			
			// lets assume there is only one with this above name
			if (dbNodes.Length == 0) {
				LoggingService.Debug("could not find DatabaseExplorer Node, so creating it now");
				dbNode = new TreeNode();
				dbNode.Text = "Database Explorer";
				dbNode.Name = "DatabaseExplorer";
				this.Nodes.Add(dbNode);
			} else {
				dbNode = dbNodes[0];
			}
			
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

			// Rebuild each of the root nodes in the ServerToolTreeView
			dbNode.Nodes.Clear();
			foreach (string name in DbModelInfoService.Names) {
				TreeNode dbModelInfoNode = CreateDbModelInfoNode(name);
				TreeNode connectionNode = CreateConnectionPropertiesNode(name);
				TreeNode metadataNode = CreateMetaDataNode(name);
				dbModelInfoNode.Nodes.Add(connectionNode);
				dbModelInfoNode.Nodes.Add(metadataNode);
				dbNode.Nodes.Add(dbModelInfoNode);
			}
			this.EndUpdate();
		}
		
		public TreeNode CreateDbModelInfoNode(string name)
		{
			TreeNode treeNode = new TreeNode(name);
			treeNode.Tag = "ConnectionRoot";
			// create and add the menustrip for this node
			
			NodeAwareContextMenuStrip cMenu = new NodeAwareContextMenuStrip(treeNode);
			
			// create menu items
			ToolStripMenuItem setConnectionStringMenuItem = 
				new ToolStripMenuItem("Set Connection String");
			setConnectionStringMenuItem.Click += new EventHandler(SetConnectionStringOnDbModelInfoClickHandler);
			
			ToolStripMenuItem loadMetadataFromConnectionMenuItem =
				new ToolStripMenuItem("Load Metadata from Connection");
			loadMetadataFromConnectionMenuItem.Click += new EventHandler(LoadMetadataFromConnectionClickHandler);
			
			ToolStripMenuItem loadMetadataFromFileMenuItem =
				new ToolStripMenuItem("Load Metadata from File");
			loadMetadataFromFileMenuItem.Click += new EventHandler(LoadMetadataFromFileClickHandler);
			
			
			cMenu.Items.AddRange(new ToolStripMenuItem[] 
			                     {
			                     	setConnectionStringMenuItem,
			                     	loadMetadataFromConnectionMenuItem,
			                     	loadMetadataFromFileMenuItem
			                     });
	
			treeNode.ContextMenuStrip = cMenu;
			return treeNode;
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <returns>a TreeNode representation of the connection properties
		/// of the connection</returns>
		public TreeNode CreateConnectionPropertiesNode(string name)
		{
			// create sub TreeNodes for the connection string and invariant name if they exist
			DbModelInfo modelInfo = DbModelInfoService.GetDbModelInfo(name);
			if (name == null) throw new KeyNotFoundException();
			string connectionString = modelInfo.ConnectionString;
			string invariantName = modelInfo.InvariantName;
			
			TreeNode attributesNode = new TreeNode("Connection Properties");
				
			if (connectionString != null) {
				TreeNode cstringNode = new TreeNode("Connection String: " + connectionString);
				attributesNode.Nodes.Add(cstringNode);
			}
			
			if (invariantName != null) {
				TreeNode invNameNode = new TreeNode("Invariant Name: " + invariantName);
				attributesNode.Nodes.Add(invNameNode);
			}
			
			return attributesNode;
		}
		
		public TreeNode CreateMetaDataNode(string name)
		{
			TreeNode metaNode = new TreeNode("Db Objects");
			metaNode.Name = name + ":MetaData";
			DbModelInfo info = DbModelInfoService.GetDbModelInfo(name);
			DataTable metadataCollectionsTable = info.Tables[TableNames.MetaDataCollections];
			if (metadataCollectionsTable != null) {
				for (int i = 0; i < TableNames.PrimaryObjects.Length; i++) {
					string metadataCollectionName = TableNames.PrimaryObjects[i];
					DataTable metaCollectionTable = info.Tables[metadataCollectionName];
					LoggingService.Debug("found metadata collection: " + metadataCollectionName);
					TreeNode collectionNode = new TreeNode(metadataCollectionName);
					collectionNode.Name = name + ":Collection:" + metadataCollectionName;
					metaNode.Nodes.Add(collectionNode);
					foreach (DataRow dbObjectRow in metaCollectionTable.Rows) {
						TreeNode objectNode = null;
						if (dbObjectRow.ItemArray.Length > 1) {
							objectNode = new TreeNode((string)dbObjectRow[1]);
							objectNode.Name = name + ":Object:" + (string)dbObjectRow[1];
							// TODO: >>>>>>> NEXT: building Describe invocation will need to be somewhere around here
						} else {
							objectNode = new TreeNode((string)dbObjectRow[0]);
							objectNode.Name = name + ":Object:" + (string)dbObjectRow[0];
						}
						// HACK All this building stuff needs to be externalise I think						
						if (metadataCollectionName.Equals("Tables")) {
							// add the handler to invoke describer
							NodeAwareContextMenuStrip cMenu = new NodeAwareContextMenuStrip(objectNode);
							ToolStripMenuItem invokeDescriberMenuItem = new ToolStripMenuItem("Describe");
							invokeDescriberMenuItem.Click += new EventHandler(DescribeTableClickHandler);
							cMenu.Items.Add(invokeDescriberMenuItem);
							objectNode.ContextMenuStrip = cMenu;
						}
						    	
						    
						
//						TreeNode ownerNode = new TreeNode("Owner: " + (string)dbObjectRow["OWNER"]);
//						TreeNode typeNode = new TreeNode("Type: " + (string)dbObjectRow["TYPE"]);
//						// TODO: add fields to each Table
//						TreeNode fieldsNode = new TreeNode("Fields [TODO]");
//						objectNode.Nodes.AddRange(new TreeNode[] {ownerNode, typeNode, fieldsNode });
						collectionNode.Nodes.Add(objectNode);
					}
				}	
			}
			return metaNode;
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
			this.BeginInvoke(new MethodInvoker(this.Rebuild));
		}
		
		public void DeleteDbConnectionClickHandler(object sender, EventArgs e)
		{
			LoggingService.Debug("delete connection clicked");
		}
		
		public void SaveDbModelInfoClickHandler(object sender, EventArgs e)
		{
			// save each DbModelInfo separately, confirming overwrite where necessary
			LoggingService.Debug("save all metadata clicked - will iterate through each and attempt to save");
			IList<string> names = DbModelInfoService.Names;
			foreach (string name in names) {
				bool saved = DbModelInfoService.SaveToFile(name, false);
				if (!saved) {
					DialogResult result = MessageBox.Show("Overwrite existing file for connection: " + name + "?", 
					                "File exists for connection", MessageBoxButtons.YesNo,
					                MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
					if (result.Equals(DialogResult.Yes)) {
						DbModelInfoService.SaveToFile(name, true);
					}
				}
			}
		}
		
		public void LoadMetadataFromFileClickHandler(object sender, EventArgs e)
		{
			LoggingService.Debug("load metadata from file clicked");
			string logicalConnectionName = getConnectionName(sender);
			DbModelInfoService.LoadFromFile(logicalConnectionName);
			this.BeginInvoke(new MethodInvoker(this.Rebuild));
		}
		
		private static string getConnectionName(object sender)
		{
			ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
			NodeAwareContextMenuStrip toolStrip = menuItem.Owner as NodeAwareContextMenuStrip;
			TreeNode node = toolStrip.TreeNode;
			while ((node.Tag == null) || (!node.Tag.Equals("ConnectionRoot"))) {
				node = node.Parent;				       	
			}
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
			this.BeginInvoke(new MethodInvoker(this.Rebuild));
		}
		
		public void LoadMetadataFromConnectionClickHandler(object sender, EventArgs args)
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
		
		public void DescribeTableClickHandler(object sender, EventArgs args)
		{
			string logicalConnectionName = getConnectionName(sender);
			ToolStripMenuItem item = sender as ToolStripMenuItem;
			NodeAwareContextMenuStrip s = item.Owner as NodeAwareContextMenuStrip;
			string tableName = s.TreeNode.Text;
			LoggingService.Debug("describe table clicked for: " + logicalConnectionName + " and table name: " + tableName);
			DataTable tableInfo = DbModelInfoService.GetTableInfo(logicalConnectionName, tableName);
			//DataSet dbModelInfo = DbModelInfoService.GetDbModelInfo(logicalConnectionName);
//			TableDescribeForm describeForm = new TableDescribeForm(tableInfo);
//			describeForm.Show();
			TableDescribeViewContent tableDescribeViewContent = new TableDescribeViewContent(tableInfo, tableName);
			WorkbenchSingleton.Workbench.ShowView(tableDescribeViewContent);
		}
	}
	
	class NodeAwareContextMenuStrip : ContextMenuStrip
	{
		TreeNode treeNodeAttached;
		
		public NodeAwareContextMenuStrip(TreeNode treeNodeAttached) : base()
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





