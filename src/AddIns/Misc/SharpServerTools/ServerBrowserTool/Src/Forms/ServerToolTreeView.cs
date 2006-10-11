/*
 * User: dickon
 * Date: 23/09/2006
 * Time: 23:07
 * 
 */

using System;
using System.Windows.Forms;
using System.Collections;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace SharpServerTools.Forms
{
	/// <summary>
	/// Provides a tree-structured visual rendering of server instances.
	/// This class should not hold references to server data, but should render its
	/// content using data retrieved from business services responsible for
	/// maintaining this model/s of underlying services.
	/// </summary>
	public class ServerToolTreeView : TreeView, IRebuildable
	{		
		public ServerToolTreeView(): base()
		{
			// TODO: iterate through plugins retrieved from AddIn Tree
			Type dbExplorerType = Type.GetType("SharpDbTools.Forms.DatabaseExplorerTreeNode, SharpDbTools");
			TreeNode dbExplorerNode = (TreeNode)Activator.CreateInstance(dbExplorerType);
			IRequiresRebuildSource s = dbExplorerNode as IRequiresRebuildSource;
			s.RebuildRequiredEvent += new RebuildRequiredEventHandler(RebuildRequiredNotify);
			this.Nodes.Add(dbExplorerNode);
		}
		
		public void RebuildChildren(IEnumerable children)
		{
			// Rebuild each of the root nodes in the ServerToolTreeView
			// Currently this comprises the Database Explorer
			IEnumerable n = children;
			if (n == null) {
				n = this.Nodes;
			}

			this.BeginUpdate();
			foreach (object o in n) {
				IRebuildable se = (IRebuildable)o;
				se.Rebuild();
			}
			this.EndUpdate();
		}
		
		private void RebuildRequiredNotify(object sender, RebuildRequiredEventArgs e)
		{
			IEnumerable children = e.Nodes;
			if (this.InvokeRequired) {
				this.Invoke(new RebuildChildrenDelegate(RebuildChildren), new object[] {children});
			}
			else {
				RebuildChildren(children);
			}
			
		}
		
		public void Rebuild()
		{
			if (this.InvokeRequired) {
				this.Invoke(new RebuildChildrenDelegate(RebuildChildren), new object[] {null});	
			}
			else {
				this.RebuildChildren(null);
			}
		}
		
		
	}
	public delegate void RebuildChildrenDelegate(IEnumerable children);
}
