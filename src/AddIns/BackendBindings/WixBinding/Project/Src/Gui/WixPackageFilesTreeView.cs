// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using System;
using System.Collections.Specialized;
using System.Windows.Forms;
using System.Xml;

namespace ICSharpCode.WixBinding
{
	public class WixPackageFilesTreeView : ExtTreeView, IOwnerState
	{
		/// <summary>
		/// The possible states of the tree view.
		/// </summary>
		public enum WixPackageFilesTreeViewState {
			NoChildElementsAllowed = 0,
			ChildElementsAllowed = 1
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
				if (allowedChildElements.Count > 0) {
					return WixPackageFilesTreeViewState.ChildElementsAllowed;
				}
				return WixPackageFilesTreeViewState.NoChildElementsAllowed;
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
				AddNode(null, directory);
			}
			ExpandAll();
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
			ExtTreeNode selectedNode = (ExtTreeNode)SelectedNode;
			ExtTreeNode addedNode = AddNode(selectedNode, element);
			addedNode.Expand();
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
		
		/// <summary>
		/// Adds a new tree node containing the xml element to the specified
		/// nodes collection.
		/// </summary>
		ExtTreeNode AddNode(ExtTreeNode parentNode, XmlElement element)
		{
			ExtTreeNode node = CreateNode(element);
			if (parentNode != null) {
				node.AddTo(parentNode);
			} else {
				node.AddTo(this);
			}
			AddNodes(node, element.ChildNodes);
			return node;
		}
		
		/// <summary>
		/// Adds all the elements.
		/// </summary>
		void AddNodes(ExtTreeNode parentNode, XmlNodeList nodes)
		{
			foreach (XmlNode childNode in nodes) {
				XmlElement childElement = childNode as XmlElement;
				if (childElement != null) {
					AddNode(parentNode, childElement);
				}
			}
		}
		
		/// <summary>
		/// Creates a tree node from the specified element.
		/// </summary>
		ExtTreeNode CreateNode(XmlElement element)
		{
			switch (element.LocalName) {
				case "Directory":
					return new WixDirectoryTreeNode((WixDirectoryElement)element);
				case "Component":
					return new WixComponentTreeNode((WixComponentElement)element);
				case "File":
					return new WixFileTreeNode((WixFileElement)element);
			}
			return new UnknownWixTreeNode(element);
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
		WixTreeNode FindNode(TreeNodeCollection nodes, XmlElement element)
		{
			foreach (WixTreeNode node in nodes) {
				if (NodeHasElement(node, element)) {
					return node;
				} else {
					WixTreeNode foundNode = FindNode(node.Nodes, element);
					if (foundNode != null) {
						return foundNode;
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
