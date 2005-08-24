// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// Description of ExtTreeView.
	/// </summary>
	public class ExtTreeView : TreeView
	{
		Dictionary<string, int> imageIndexTable = new Dictionary<string, int>();
		List<ExtTreeNode> cutNodes = new List<ExtTreeNode>();
		bool isSorted = true;
		
		/// <summary>
		/// Gets/Sets whether the ExtTreeView does its own sorting.
		/// </summary>
		public bool IsSorted {
			get {
				return isSorted;
			}
			set {
				isSorted = value;
			}
		}
		
		[Obsolete("Use IsSorted instead!")]
		public new bool Sorted {
			get {
				return base.Sorted;
			}
			set {
				base.Sorted = value;
			}
		}

		public List<ExtTreeNode> CutNodes {
			get {
				return cutNodes;
			}
		}
		
		// using TreeView.TreeViewNodeSorter will result in TreeNodeCollection
		// calling Sort() after every insertion. Therefore, we have to create
		// our own NodeSorter property.
		IComparer<TreeNode> nodeSorter = new ExtTreeViewComparer();

		public IComparer<TreeNode> NodeSorter {
			get {
				return nodeSorter;
			}
			set {
				nodeSorter = value;
			}
		}
		
		[Obsolete("Use NodeSorter instead!")]
		public new System.Collections.IComparer TreeViewNodeSorter {
			get {
				return base.TreeViewNodeSorter;
			}
			set {
				base.TreeViewNodeSorter = value;
			}
		}
		
		public ExtTreeView()
		{
			DrawMode      = TreeViewDrawMode.OwnerDrawText;
			HideSelection = false;
			AllowDrop     = true;
			ImageList newImageList = new ImageList();
			newImageList.ImageSize = new Size(16, 16);
			newImageList.ColorDepth = ColorDepth.Depth32Bit;
			this.ImageList = newImageList;
		}

		public new void Sort()
		{
			SortNodes(Nodes, true);
		}

		public void SortNodes(TreeNodeCollection nodes, bool recursive)
		{
			if (!isSorted) {
				return;
			}
			TreeNode[] nodeArray = new TreeNode[nodes.Count];
			nodes.CopyTo(nodeArray, 0);
			Array.Sort(nodeArray, nodeSorter);
			nodes.Clear();
			nodes.AddRange(nodeArray);

			if (recursive) {
				foreach (TreeNode childNode in nodeArray) {
					SortNodes(childNode.Nodes, true);
				}
			}
		}
		
		public void ClearCutNodes()
		{
			foreach (ExtTreeNode node in CutNodes) {
				node.DoPerformCut = false;
			}
			CutNodes.Clear();
		}
		
		public void Clear()
		{
			if (this.IsDisposed) {
				return;
			}
			TreeNode[] nodeArray = new TreeNode[Nodes.Count];
			Nodes.CopyTo(nodeArray, 0);
			Nodes.Clear();
			foreach (TreeNode node in nodeArray) {
				if (node is IDisposable) {
					((IDisposable)node).Dispose();
				}
			}
		}
		
		void ActivateSelectedItem()
		{
			ExtTreeNode node = SelectedNode as ExtTreeNode;
			if (node != null) {
				node.ActivateItem();
			}
		}
		#region label editing
		
		public void StartLabelEdit(ExtTreeNode node)
		{
			if (node == null) {
				return;
			}
			node.EnsureVisible();
			SelectedNode = node;
			LabelEdit = true;
			node.BeforeLabelEdit();
			node.BeginEdit();
		}
		
		protected override bool ProcessDialogKey(Keys keyData)
		{
			switch (keyData) {
				case Keys.F2:
					StartLabelEdit(SelectedNode as ExtTreeNode);
					break;
			}
			return base.ProcessDialogKey(keyData);
		}
		
		protected override void OnAfterLabelEdit(NodeLabelEditEventArgs e)
		{
			base.OnAfterLabelEdit(e);
			LabelEdit    = false;
			e.CancelEdit = true;
			
			ExtTreeNode node = e.Node as ExtTreeNode;
			if (node != null) {
				node.AfterLabelEdit(e.Label);
			}
			SortParentNodes(e.Node);
		}

		private void SortParentNodes(TreeNode treeNode)
		{
			TreeNode parent = treeNode.Parent;
			SortNodes((parent == null) ? Nodes : parent.Nodes, false);
		}
		#endregion
		bool inRefresh = false;
		protected override void OnBeforeExpand(TreeViewCancelEventArgs e)
		{
			base.OnBeforeExpand(e);
			if (e.Node == null)
				return;
			inRefresh = true;
			BeginUpdate();
			try {
				if (e.Node is ExtTreeNode) {
					((ExtTreeNode)e.Node).Expanding();
				}
				SortNodes(e.Node.Nodes, false);
			} catch (Exception ex) {
				// catch error to prevent corrupting the TreeView component
				MessageService.ShowError(ex);
			}
			if (e.Node.Nodes.Count == 0) {
				// when the node's subnodes have been removed by Expanding, AfterExpand is not called
				inRefresh = false;
				EndUpdate();
			}
		}
		
		protected override void OnAfterExpand(TreeViewEventArgs e)
		{
			base.OnAfterExpand(e);
			inRefresh = false;
			EndUpdate();
		}
		
		
		protected override void OnBeforeCollapse(TreeViewCancelEventArgs e)
		{
			base.OnBeforeCollapse(e);
			if (e.Node is ExtTreeNode) {
				((ExtTreeNode)e.Node).Collapsing();
			}
		}
		
		protected override void OnKeyPress(KeyPressEventArgs e)
		{
			base.OnKeyPress(e);
			if (e.KeyChar == '\r') {
				ActivateSelectedItem();
				e.Handled = true;
			}
		}
		
		protected override void OnDoubleClick(EventArgs e)
		{
			base.OnDoubleClick(e);
			ActivateSelectedItem();
		}
		
		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			TreeNode node = GetNodeAt(e.X, e.Y);
			if (node != null) {
				if (SelectedNode != node) {
					SelectedNode = node;
				}
			} else {
				SelectedNode = null;
				this.ContextMenuStrip = null;
			}
		}
		
		protected override void OnBeforeSelect(TreeViewCancelEventArgs e)
		{
			// setting the context menu must be done by BeforeSelect because
			// AfterSelect is not called for the selection changes when a node is being deleted.
			base.OnBeforeSelect(e);
			ExtTreeNode node = e.Node as ExtTreeNode;
			if (node != null) {
				this.ContextMenuStrip = MenuService.CreateContextMenu(e.Node, node.ContextmenuAddinTreePath);
			}
		}
		
		protected override void OnAfterCheck(TreeViewEventArgs e)
		{
			base.OnAfterCheck(e);
			ExtTreeNode node = e.Node as ExtTreeNode;
			if (node != null) {
				node.CheckedChanged();
			}
		}
		
		protected override void OnDrawNode(DrawTreeNodeEventArgs e)
		{
			if (!inRefresh) {
				ExtTreeNode node = e.Node as ExtTreeNode;
				if (node != null && !node.DrawDefault) {
					node.Draw(e);
					e.DrawDefault = false;
				} else {
					e.DrawDefault = true;
				}
			} else {
				e.DrawDefault = false;
			}
			base.OnDrawNode(e);
		}
		
		protected override void OnItemDrag(ItemDragEventArgs e)
		{
			base.OnItemDrag(e);
			ExtTreeNode node = e.Item as ExtTreeNode;
			if (node != null) {
				DataObject dataObject = node.DragDropDataObject;
				if (dataObject != null) {
					DoDragDrop(dataObject, DragDropEffects.All);
					SortParentNodes(node);
				}
			}
		}
		
		protected override void OnDragEnter(DragEventArgs e)
		{
			base.OnDragEnter(e);
			e.Effect = DragDropEffects.Move | DragDropEffects.Copy | DragDropEffects.None;
		}
		
		protected override void OnDragOver(DragEventArgs e)
		{
			base.OnDragOver(e);
			Point       clientcoordinate = PointToClient(new Point(e.X, e.Y));
			ExtTreeNode node             = GetNodeAt(clientcoordinate) as ExtTreeNode;
			
			if (node != null) {
				DragDropEffects effect = DragDropEffects.None;
				
				if ((e.KeyState & 8) > 0) { // CTRL key pressed.
					effect = DragDropEffects.Copy;
				} else {
					effect = DragDropEffects.Move;
				}
				e.Effect = node.GetDragDropEffect(e.Data, effect);
				
				if (e.Effect != DragDropEffects.None) {
					SelectedNode = node;
				}
			}
		}
		
		protected override void OnDragDrop(DragEventArgs e)
		{
			base.OnDragDrop(e);
			Point       clientcoordinate = PointToClient(new Point(e.X, e.Y));
			ExtTreeNode node             = GetNodeAt(clientcoordinate) as ExtTreeNode;
			
			if (node != null) {
				node.DoDragDrop(e.Data, e.Effect);
				SortParentNodes(node);
			}
		}
		
		public int GetImageIndexForImage(string image, bool performCutBitmap)
		{
			string imageKey = performCutBitmap ? (image + "_ghost") : image;
			if (!imageIndexTable.ContainsKey(imageKey)) {
				ImageList.Images.Add(performCutBitmap ? IconService.GetGhostBitmap(image) : IconService.GetBitmap(image));
				imageIndexTable[imageKey] = ImageList.Images.Count - 1;
				return ImageList.Images.Count - 1;
			}
			return imageIndexTable[imageKey];
		}
	}
}
