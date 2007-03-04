// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Dickon Field" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;

using ICSharpCode.Core;

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
		public const string SERVERTOOL_PATH = "/SharpServerTools/ServerTool";
		
		public ServerToolTreeView(): base()
		{
			
			AddInTreeNode node = 
			AddInTree.GetTreeNode(SERVERTOOL_PATH);
			List<Codon> codons = node.Codons;
			foreach (Codon codon in codons) {
				// create an instance of the relevant ServerTool TreeNode
				string id = codon.Id;
				TreeNode treeNode = (TreeNode)node.BuildChildItem(id, null, null);

				// a ServerTool plugin can register to be refreshed by the ServerToolTreeView
				// control by implementing the IRequiresRebuildSource interface
				
				IRequiresRebuildSource s = treeNode as IRequiresRebuildSource;
				
				if (s != null) {
					s.RebuildRequiredEvent += new RebuildRequiredEventHandler(RebuildRequiredNotify);
				}
				
				// a ServerTool plugin can also register to handle drag-n-drop if it implements
				// the required interface
				
				ISupportsDragDrop d = treeNode as ISupportsDragDrop;
				
				if (d != null) {
					this.MouseDown += new MouseEventHandler(d.HandleMouseDownEvent);
				}
				
				this.Nodes.Add(treeNode);
			}
			
//			Type dbExplorerType = Type.GetType("SharpDbTools.Forms.DatabaseExplorerTreeNode, SharpDbTools");
//			TreeNode dbExplorerNode = (TreeNode)Activator.CreateInstance(dbExplorerType);
//			IRequiresRebuildSource s = dbExplorerNode as IRequiresRebuildSource;
//			s.RebuildRequiredEvent += new RebuildRequiredEventHandler(RebuildRequiredNotify);
//			this.Nodes.Add(dbExplorerNode);
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
