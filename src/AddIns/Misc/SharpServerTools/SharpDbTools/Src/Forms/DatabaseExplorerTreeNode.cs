// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Dickon Field" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Windows.Forms;

using ICSharpCode.Core;
using SharpDbTools.Data;
using SharpServerTools.Forms;

namespace SharpDbTools.Forms
{
	/// <summary>
	/// Description of DatabaseExplorerNode.
	/// Hold minimal state - access state through the DbModelInfoService
	/// </summary>
	public class DatabaseExplorerTreeNode: TreeNode, IRebuildable, IRequiresRebuildSource
	{	
		public DatabaseExplorerTreeNode(): base("Database Explorer")
		{
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
			this.ContextMenuStrip = cMenu;
		}
		
		public void Rebuild()
		{
			this.Nodes.Clear();
			foreach (string name in DbModelInfoService.Names) {
				LoggingService.Debug(this.GetType().ToString() + " getting DbModelInfoTreeNode for node: " + name);
				DbModelInfoTreeNode dbModelInfoNode = CreateDbModelInfoNode(name);
				dbModelInfoNode.RebuildRequiredEvent += new RebuildRequiredEventHandler(RebuildRequiredNotify);
				this.Nodes.Add(dbModelInfoNode);
			}
		}
		
		public event RebuildRequiredEventHandler RebuildRequiredEvent;
		
		/// <summary>
		/// DatabaseExplorerTreeNode chucks away any existing Nodes and recreates its tree when it
		/// is triggered.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		private void RebuildRequiredNotify(object sender, RebuildRequiredEventArgs e) 
		{	
			// adding this node because it wants to be rebuilt.
			e.AddNode(this);
			this.FireRebuildRequired(this, e);
		}
		
		private void FireRebuildRequired(object sender, RebuildRequiredEventArgs e)
		{
			if (this.RebuildRequiredEvent != null) {
				RebuildRequiredEvent(this, e);
			}
		}
		
		private DbModelInfoTreeNode CreateDbModelInfoNode(string name)
		{
			return new DbModelInfoTreeNode(name);
		}
		
		/// <summary>
		/// Uses a dialog to get the logical name of a new Connection then
		/// adds a new DbModelInfo for it to the cache and updates the DatabaseServer 
		/// Tree.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		
		private void AddDbConnectionClickHandler(object sender, EventArgs e)
		{
			LoggingService.Debug("add connection clicked");
			
			// get the logical name of the new connection
			
			string logicalName = MessageService.ShowInputBox("Connection name", "Please provide the name for your db connection:", "");
			if (String.IsNullOrEmpty(logicalName)) return;
			
			LoggingService.Debug("name received is: " + logicalName);
			
			// add a new DbModelInfo to the cache
			
			DbModelInfoService.Add(logicalName, null, null);
			
			// rebuild the database server node
			
			RebuildRequiredEventArgs e1 = new RebuildRequiredEventArgs();
			e1.AddNode(this as IRebuildable);
			this.FireRebuildRequired(this, e1);
		}
		
		private void DeleteDbConnectionClickHandler(object sender, EventArgs e)
		{
			LoggingService.Debug("delete connection clicked");
		}
		
		private void SaveDbModelInfoClickHandler(object sender, EventArgs e)
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
	}
}
