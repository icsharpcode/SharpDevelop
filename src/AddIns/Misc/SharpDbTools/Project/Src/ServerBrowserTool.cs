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
	/// referenced therein.
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
			controller = new ServerBrowserToolController();
			TreeView dbTree = new TreeView();
			ctl = new Panel();
			ctl.Controls.Add(dbTree);
			
			// initialise browser tree
			
			dbTree.BeginUpdate();
			//dbTree.Tag = "Connections";
			TreeNode connection1 = new TreeNode("Test");
			TreeNode[] childNodes = new TreeNode[1];
			childNodes[0] = connection1;
			TreeNode topNode = new TreeNode("Connections", childNodes);
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
