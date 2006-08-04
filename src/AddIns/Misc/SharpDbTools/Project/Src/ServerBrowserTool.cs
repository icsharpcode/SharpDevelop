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
		/// 
		/// </summary>
		public ServerBrowserTool()
		{
			LoggingService.Debug("Loading ServerBrowserTool");
			controller = ServerBrowserToolController.GetInstance();
			TreeView dbTree = new TreeView();
			ctl = new Panel();
			ctl.Controls.Add(dbTree);
			
			dbTree.Dock = DockStyle.Fill;
			
			// initialise browser tree
			
			dbTree.BeginUpdate();
			DatabaseServerNode topNode = new DatabaseServerNode();
			dbTree.Nodes.Add(topNode);
			dbTree.EndUpdate();
			
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
		/// Refreshes the pad
		/// </summary>
		public override void RedrawContent()
		{
			// TODO: Refresh the whole pad control here, renew all resource strings whatever
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
}

/// <summary>
/// Hosts a list of ConnectionNodes corresponding to the available
/// DbModelInfo's and supports the addition and deletion of
/// ConnectionNodes.
/// </summary>
class DatabaseServerNode : TreeNode
{
	public DatabaseServerNode()
	{
		this.Text = "Database Connections";
		
		ContextMenuStrip cMenu = new ContextMenuStrip();
		ToolStripMenuItem addConnectionMenuItem = 
			new ToolStripMenuItem("Add Connection");
		addConnectionMenuItem.Click += new EventHandler(AddConnectionClickHandler);
		
		ToolStripMenuItem deleteConnectionMenuItem = 
			new ToolStripMenuItem("Delete Connection");
		deleteConnectionMenuItem.Click += new EventHandler(DeleteConnectionClickHandler);
		
		ToolStripMenuItem saveMetadataMenuItem =
			new ToolStripMenuItem("Save All");
		saveMetadataMenuItem.Click += new EventHandler(SaveMetadataClickHandler);
		
		
		cMenu.Items.AddRange(new ToolStripMenuItem[] 
		                     {	
		                     	addConnectionMenuItem,
		                     	deleteConnectionMenuItem,
		                     	saveMetadataMenuItem
		                     } 
		                    );
		this.ContextMenuStrip = cMenu;
	}
	
	/// <summary>
	/// Uses a dialog to get the logical name of a new Connection then
	/// adds a new DbModelInfo for it to the cache and updates the DatabaseServer 
	/// Tree.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void AddConnectionClickHandler(object sender, EventArgs e)
	{
		LoggingService.Debug("add connection clicked");
		
		// get the logical name of the new connection
		
		// add a new DbModelInfo to the cache
		
		// add a new Node below this one for the Connection
		
	}
	
	public void DeleteConnectionClickHandler(object sender, EventArgs e)
	{
		LoggingService.Debug("delete connection clicked");
	}
	
	public void SaveMetadataClickHandler(object sender, EventArgs e)
	{
		LoggingService.Debug("save all metadata clicked");
	}
	
}


/// <summary>
/// Adds context menu behaviour to save/update etc the corresponding
/// DbModelInfo and to host a list of MetadataNodes for the DbModelInfo
/// </summary>
class ConnectionNode : TreeNode
{	
	public ConnectionNode(string name)
	{
		this.Text = name;
		this.Tag = name;
	}
	
	/// <summary>
	/// Changes the logical name of this connection. This is just for the convenience
	/// of the user: the connection properties and metadata can remain the same
	/// but the DbModelInfoService needs to be notified to make changes to the cache
	/// and file for its DbModelInfo.
	/// </summary>
	public void ChangeName() {}
	
	/// <summary>
	/// Uses the ConnectionStringDefinitionDialog to change the properties
	/// of this connection.
	/// Note that by doing this the metadata for the connection must be cleared.
	/// </summary>
	public void ChangeConnectionProperties() {}
	
}

/// <summary>
/// Adds behaviour to display a list of db objects corresponding to
/// its metadata type
/// </summary>
class MetadataNode : TreeNode
{
	
}
