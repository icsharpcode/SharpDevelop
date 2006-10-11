/*
 * User: dickon
 * Date: 23/09/2006
 * Time: 23:37
 * 
 */

using System;
using System.Windows.Forms;
using System.Data.Common;

using ICSharpCode.Core;

using SharpServerTools.Forms;

using SharpDbTools.Data;

namespace SharpDbTools.Forms
{
	/// <summary>
	/// Renders a view of the metadata and connection properties for a single
	/// database connection. It is an IRequiresRebuildSource and can emit 
	/// RequiresRebuildEvents when the metadata etc are changed, but the 
	/// DatabaseExplorerTreeNode disposes of these and constructs new ones 
	/// when this occurs, so it is not an IRebuildable
	/// </summary>
	public class DbModelInfoTreeNode: TreeNode, IRequiresRebuildSource
	{
		public DbModelInfoTreeNode(string name): base(name)
		{		
			// create and add the menustrip for this node
			
			NodeAwareContextMenuStrip cMenu = new NodeAwareContextMenuStrip(this);
			
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
	
			this.ContextMenuStrip = cMenu;
			this.Nodes.Clear();
			TreeNode connectionPropsNode = CreateConnectionPropertiesNode(this.LogicalConnectionName);
			TreeNode dbNode = CreateMetaDataNode(this.LogicalConnectionName);
			this.Nodes.Add(connectionPropsNode);
			this.Nodes.Add(dbNode);
		}
		
		public string LogicalConnectionName {
			get {
				return this.Text;
			}
		}
		
		public event RebuildRequiredEventHandler RebuildRequiredEvent;
		
		protected void FireRebuildRequired()
		{
			// HERE: the null eventargs indicates no desire to be rebuilt - is this correct?
			if (RebuildRequiredEvent != null) {
				// This object does not want to be rebuilt - it is discarded when there is
				// a change in the underlying model. So, an event is posted without a ref
				// to this object.
				RebuildRequiredEventArgs eventArgs = new RebuildRequiredEventArgs();
				RebuildRequiredEvent(this, eventArgs);
			}
		}
		
		private TreeNode CreateConnectionPropertiesNode(string name)
		{
			// create sub TreeNodes for the connection string and invariant name if they exist
			LoggingService.Debug("Looking for a Db Model Info for connection with name: " + name);
			DbModelInfo modelInfo = DbModelInfoService.GetDbModelInfo(name);
			
			if (modelInfo == null) {
				LoggingService.Error("could not find a logical connection named: " + name);
				throw new ArgumentException("this logical connection name is not defined: " + name);
			}
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
		
		private TreeNode CreateMetaDataNode(string name)
		{
			LoggingService.Debug("creating metadata tree for connection with name: " + name);
			TreeNode node = null;
			// get the invariant name from the name, then get the FormsArtefactFactory
			DbModelInfo modelInfo = DbModelInfoService.GetDbModelInfo(name);
			
			if (modelInfo == null) {
				LoggingService.Error("could not find a logical connection named: " + name);
				throw new ArgumentException("this logical connection name is not defined: " + name);
			}
			
			string invariantName = modelInfo.InvariantName;
			LoggingService.Debug("got invariant name: " + invariantName + " for connection name: " + name);

			try {
				LoggingService.Debug(this.GetType().ToString() 
			    	+ ": getting forms info for name: " 
			        + name + " and invariant name: " 
			        + invariantName);
				FormsArtefactFactory factory = FormsArtefactFactories.GetFactory(invariantName);
				node = factory.CreateMetaDataNode(name);	
			} catch(ArgumentException e) {
				LoggingService.Debug(this.GetType().ToString() 
				                     + " failed to create metadata node for connection: "
				                     + name + "\n" 
				                     + e.Message + "\n"
				                     + e.GetType().ToString());
				node = new TreeNode("No Metadata");
			}
			return node;
		}
		
		/// <summary>
		/// Uses a dialog to get the logical name of a new Connection then
		/// adds a new DbModelInfo for it to the cache and updates the DatabaseServer 
		/// Tree.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		
		private void SetConnectionStringOnDbModelInfoClickHandler(object sender, EventArgs e)
		{
			string connectionLogicalName = this.Text;
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
			this.FireRebuildRequired();
		}
		
		private void LoadMetadataFromFileClickHandler(object sender, EventArgs e)
		{
			LoggingService.Debug("load metadata from file clicked");
			string logicalConnectionName = this.Text;
			DbModelInfoService.LoadFromFile(logicalConnectionName);
			this.FireRebuildRequired();
		}
		
		private void LoadMetadataFromConnectionClickHandler(object sender, EventArgs args)
		{
			string connectionLogicalName = this.Text;
			LoggingService.Debug("load metadata from connection clicked for connection with name: "
			                     + connectionLogicalName);
			try {	
				DbModelInfoService.LoadMetadataFromConnection(connectionLogicalName);
			}
			catch(DbException e) {
				MessageService.ShowError(e, 
				"An Exception was thrown while trying to connect to: " + connectionLogicalName);                       
			}
			this.FireRebuildRequired();
		}
	}
}
