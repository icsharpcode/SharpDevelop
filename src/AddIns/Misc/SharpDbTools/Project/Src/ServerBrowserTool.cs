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
			//dbTree.Tag = "Connections";
			ContextMenuStrip cMenu = new ContextMenuStrip();
			ToolStripMenuItem menuItem = new ToolStripMenuItem("Save");
			cMenu.Items.AddRange(new ToolStripMenuItem[] {menuItem} );
			                                              
			TreeNode connection1 = new TreeNode("Test");
			connection1.ContextMenuStrip = cMenu;
			
			TreeNode[] childNodes = new TreeNode[1];
			childNodes[0] = connection1;
			TreeNode topNode = new TreeNode("Database Connections", childNodes);
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
/// DbModelInfo's
/// </summary>
class DatabaseServerNode : TreeNode
{
	
}


/// <summary>
/// Adds context menu behaviour to save/update etc the corresponding
/// DbModelInfo and to host a list of MetadataNodes for the DbModelInfo
/// </summary>
class ConnectionNode : TreeNode
{
	
}

/// <summary>
/// Adds behaviour to display a list of db objects corresponding to
/// its metadata type
/// </summary>
class MetadataNode : TreeNode
{
	
}
