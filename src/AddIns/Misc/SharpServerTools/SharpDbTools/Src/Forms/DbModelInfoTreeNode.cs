// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Dickon Field" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.Data.Common;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

using SharpDbTools.Data;
using SharpServerTools.Forms;

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
		BackgroundWorker backgroundWorker;
		ProgressEllipsis progress;
		Timer timer;
		const string fileLoadMessage = ": loading from file";
		const string connectionLoadMessage = ": loading from connection";
		string message;
		
		public DbModelInfoTreeNode(string name): base(name)
		{
			// use tag to carry the logical connection name
			
			this.Tag = name;
			
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
			
			ToolStripMenuItem openSQLToolMenuItem = new ToolStripMenuItem("Open SQL Tool");	
			openSQLToolMenuItem.Click += new EventHandler(OpenSQLToolClickHandler);
			
			cMenu.Items.AddRange(new ToolStripMenuItem[] 
			                     {
			                     	setConnectionStringMenuItem,
			                     	loadMetadataFromConnectionMenuItem,
			                     	loadMetadataFromFileMenuItem,
			                     	openSQLToolMenuItem
			                     });
	
			this.ContextMenuStrip = cMenu;
			this.Nodes.Clear();
			TreeNode connectionPropsNode = CreateConnectionPropertiesNode(this.LogicalConnectionName);
			TreeNode dbNode = CreateMetaDataNode(this.LogicalConnectionName);
			this.Nodes.Add(connectionPropsNode);
			this.Nodes.Add(dbNode);
			
			timer = new Timer();
			timer.Interval = 1000;
			timer.Tick += new EventHandler(this.TimerClick);
			progress = new ProgressEllipsis(4);
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
			string connectionLogicalName = (string)this.Tag;
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
			this.backgroundWorker = new BackgroundWorker();
			backgroundWorker.DoWork += new DoWorkEventHandler(this.LoadMetadataFromFileDoWork);
			backgroundWorker.RunWorkerCompleted += 
				new RunWorkerCompletedEventHandler(this.LoadMetadataFinished);
			string logicalConnectionName = (string)this.Tag;
			this.message = logicalConnectionName + fileLoadMessage;
			this.ContextMenuStrip.Enabled = false;
			timer.Start();
			this.backgroundWorker.RunWorkerAsync(logicalConnectionName);
		}
		
		private void OpenSQLToolClickHandler(object sender, EventArgs e)
		{
			SQLToolViewContent sqlToolViewContent = new SQLToolViewContent((string)this.Tag);
			WorkbenchSingleton.Workbench.ShowView(sqlToolViewContent);
		}
		
		private void TimerClick(object sender, EventArgs eventArgs)
		{
			string ellipsis = progress.Text;
			progress.performStep();
			string displayMsg = this.message + ellipsis;
			SetText(displayMsg);
			
		}

		delegate void TextSetterDelegate(string text);
		
		public void SetText(string text)
		{
			if (this.TreeView.InvokeRequired) {
				this.TreeView.Invoke(new TextSetterDelegate(this.SetText), new object[] { text });
				return;
			}
			this.Text = text;
		}

		private void LoadMetadataFromFileDoWork(object sender, DoWorkEventArgs args)
		{
			string logicalConnectionName = args.Argument as string;
			if (logicalConnectionName != null) {
				DbModelInfoService.LoadFromFile(logicalConnectionName);	
			}
		}
		
		private void LoadMetadataFinished(object sender, RunWorkerCompletedEventArgs args)
		{
			if (this.TreeView.InvokeRequired) {
				this.TreeView.Invoke(new EventHandler<RunWorkerCompletedEventArgs>
				                     (this.LoadMetadataFinished));
				return;
			} 
			this.timer.Stop();
			this.Text = (string)this.Tag;
			this.ContextMenuStrip.Enabled = true;
			this.backgroundWorker.Dispose();
			this.backgroundWorker = null;
			this.FireRebuildRequired();
		}
		
		private void LoadMetadataFromConnectionClickHandler(object sender, EventArgs args)
		{
			LoggingService.Debug("load metadata from connection clicked");
			this.backgroundWorker = new BackgroundWorker();
			backgroundWorker.DoWork += new DoWorkEventHandler(this.LoadMetadataFromConnectionDoWork);
			backgroundWorker.RunWorkerCompleted += 
				new RunWorkerCompletedEventHandler(this.LoadMetadataFinished);
			string logicalConnectionName = (string)this.Tag;
			this.message = logicalConnectionName + connectionLoadMessage;
			this.ContextMenuStrip.Enabled = false;
			timer.Start();
			this.backgroundWorker.RunWorkerAsync(logicalConnectionName);
		}
		
		private void LoadMetadataFromConnectionDoWork(object sender, DoWorkEventArgs args)
		{
			string connectionLogicalName = args.Argument as string;
			if (connectionLogicalName != null) {
				try {	
					DbModelInfoService.LoadMetadataFromConnection(connectionLogicalName);
				}
				catch(DbException e) {
					MessageService.ShowError(e, 
					"An Exception was thrown while trying to connect to: " + connectionLogicalName);                       
				}
			}
		}
	}
}
