// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Specialized;
using System.Windows.Forms;
using System.Xml;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.WixBinding
{
	public class WixPackageFilesTreeView : ExtTreeView, IOwnerState
	{
		/// <summary>
		/// The possible states of the tree view.
		/// </summary>
		[Flags]
		public enum WixPackageFilesTreeViewState {
			None = 0,
			NoChildElementsAllowed = 0x1,
			ChildElementsAllowed = 0x2,
			NothingSelected = 0x4,
			NothingSelectedAndChildElementsAllowed = 0x8
		}
		
		StringCollection allowedChildElements = new StringCollection();
				
		/// <summary>
		/// Raised when the user selects another xml element in the tree view.
		/// </summary>
		public event EventHandler SelectedElementChanged;
		
		public WixPackageFilesTreeView()
		{
			ContextMenuStrip = MenuService.CreateContextMenu(this, "/AddIns/WixBinding/PackageFilesView/ContextMenu/TreeView");
		}
		
		/// <summary>
		/// Gets the "ownerstate" condition.
		/// </summary>
		public Enum InternalState {
			get {
				bool itemSelected = SelectedElement != null;
				bool childElementsAllowed = allowedChildElements.Count > 0;
				
				if (childElementsAllowed && itemSelected) {
					return WixPackageFilesTreeViewState.ChildElementsAllowed;
				} else if (childElementsAllowed) {
					return WixPackageFilesTreeViewState.NothingSelectedAndChildElementsAllowed;
				} else if (itemSelected) {
					return WixPackageFilesTreeViewState.NoChildElementsAllowed;
				}
				return WixPackageFilesTreeViewState.NothingSelected;
			}
		}
		
		public StringCollection AllowedChildElements {
			get {
				return allowedChildElements;
			}
		}
		
		public XmlElement SelectedElement {
			get {
				WixTreeNode selectedNode = (WixTreeNode)SelectedNode;
				if (selectedNode != null) {
					return selectedNode.XmlElement;
				}
				return null;
			}
			set {
				if (value == null) {
					SelectedNode = null;
				} else if (!IsSelectedNode(value)) {
					WixTreeNode node = FindNode(Nodes, value);
					if (node != null) {
						SelectedNode = node;
					}
				}
			}
		}
		
		/// <summary>
		/// Adds the directories to the tree view.
		/// </summary>
		public void AddDirectories(WixDirectoryElement[] directories)
		{
			foreach (WixDirectoryElement directory in directories) {
				WixTreeNodeBuilder.AddNode(this, directory);
			}
		}
		
		/// <summary>
		/// The selected element's attributes have changed so refresh the node.
		/// </summary>
		public void RefreshSelectedElement()
		{
			WixTreeNode selectedNode = (WixTreeNode)SelectedNode;
			if (selectedNode != null) {
				selectedNode.Refresh();
			}
		}
		
		/// <summary>
		/// Adds a new element to the tree.
		/// </summary>
		/// <remarks>If no node is currently selected this element is added as a 
		/// root node.</remarks>
		public void AddElement(XmlElement element)
		{
			WixTreeNode selectedNode = (WixTreeNode)SelectedNode;
			if (selectedNode == null) {
				 WixTreeNodeBuilder.AddNode(this, element);
			} else {
				if (selectedNode.IsInitialized) {
					 WixTreeNodeBuilder.AddNode(selectedNode, element);
				} else {
					// Initializing the node will add all the child elements.
					selectedNode.PerformInitialization();
				}
			}
		}
		
		/// <summary>
		/// Removes the element from the tree.
		/// </summary>
		public void RemoveElement(XmlElement element)
		{
			// Try selected node first.
			if (IsSelectedNode(element)) {
				SelectedNode.Remove();
			} else {
				ExtTreeNode node = FindNode(Nodes, element);
				if (node != null) {
					node.Remove();
				}
			}
		}
		
		protected override void OnAfterSelect(TreeViewEventArgs e)
		{
			base.OnAfterSelect(e);
			OnSelectedElementChanged();
		}
		
		/// <summary>
		/// Raises the SelectedElementChanged event if the user selects no node and
		/// previously there was a node selected. The standard TreeView.AfterSelect event
		/// never fires if the selected node is set to null.
		/// </summary>
		protected override void OnMouseDown(MouseEventArgs e)
		{
			TreeNode selectedNodeBefore = SelectedNode;
			base.OnMouseDown(e);
			TreeNode selectedNodeAfter = SelectedNode;
			if (selectedNodeAfter == null && selectedNodeBefore != selectedNodeAfter) {
				OnSelectedElementChanged();
			}
		}
	
		void OnSelectedElementChanged()
		{
			if (SelectedElementChanged != null) {
				SelectedElementChanged(this, new EventArgs());
			}
		}
		
		/// <summary>
		/// Returns whether the currently selected node contains the specified element.
		/// </summary>
		bool IsSelectedNode(XmlElement element)
		{
			return NodeHasElement(SelectedNode as WixTreeNode, element);
		}
	
		/// <summary>
		/// Finds the node that represents the specified xml element.
		/// </summary>
		/// <remarks>
		/// This currently looks in nodes that have been opened. Really it
		/// should explore the entire tree, but child nodes are only added if the
		/// node is expanded.
		/// </remarks>
		WixTreeNode FindNode(TreeNodeCollection nodes, XmlElement element)
		{
			foreach (ExtTreeNode node in nodes) {
				WixTreeNode wixTreeNode = node as WixTreeNode;
				if (wixTreeNode != null) {
					if (NodeHasElement(wixTreeNode, element)) {
						return wixTreeNode;
					} else {
						WixTreeNode foundNode = FindNode(wixTreeNode.Nodes, element);
						if (foundNode != null) {
							return foundNode;
						}
					}
				}
			}
			return null;
		}
		
		/// <summary>
		/// Checks whether the specified tree node represents the xml element.
		/// </summary>
		static bool NodeHasElement(WixTreeNode node, XmlElement element)
		{
			if (node != null) {
				return Object.ReferenceEquals(element, node.XmlElement);
			}
			return false;
		}
	}
}
