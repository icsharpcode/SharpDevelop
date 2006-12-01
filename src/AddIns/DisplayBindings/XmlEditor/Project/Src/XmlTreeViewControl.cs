// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// Displays a tree of XML elements. This is a separate control so it can
	/// be unit tested. It has no SharpDevelop specific parts, for example,
	/// the context menus are defined in the XmlTreeViewContainerControl.
	/// </summary>
	public class XmlTreeViewControl : ExtTreeView
	{			
		const string ViewStatePropertyName = "XmlTreeViewControl.ViewState";

		XmlElement documentElement;

		enum InsertionMode {
			Before = 0,
			After = 1
		}
		
		public XmlTreeViewControl()
		{
		}
		
		/// <summary>
		/// Gets or sets the root element currently being displayed.
		/// </summary>
		[Browsable(false)]
		public XmlElement DocumentElement {
			get {
				return documentElement;
			}
			set {
				documentElement = value;
				
				// Update display.
				BeginUpdate();
				try {
					ShowDocumentElement();
				} finally {
					EndUpdate();
				}
			}
		}
		
		/// <summary>
		/// Gets the selected element in the tree.
		/// </summary>
		public XmlElement SelectedElement {
			get {
				XmlElementTreeNode xmlElementTreeNode = SelectedElementNode;
				if (xmlElementTreeNode != null) {
					return xmlElementTreeNode.XmlElement;
				}
				return null;
			}
		}
		
		/// <summary>
		/// Determines whether an element is selected in the tree.
		/// </summary>
		public bool IsElementSelected {
			get {
				return SelectedElement != null;
			}
		}
		
		/// <summary>
		/// Gets the selected text node in the tree.
		/// </summary>
		public XmlText SelectedTextNode {
			get {				
				XmlTextTreeNode xmlTextTreeNode = SelectedNode as XmlTextTreeNode;
				if (xmlTextTreeNode != null) {
					return xmlTextTreeNode.XmlText;
				}

				return null;
			}
		}
		
		/// <summary>
		/// Determines whether a text node is selected in the tree.
		/// </summary>
		public bool IsTextNodeSelected {
			get {
				return SelectedTextNode != null;
			}
		}
		
		/// <summary>
		/// Saves the current state of the tree.
		/// </summary>
		public void SaveViewState(Properties properties)
		{
			properties.Set(ViewStatePropertyName, ExtTreeView.GetViewStateString(this));
		}
		
		/// <summary>
		/// Restores the node state of the tree.
		/// </summary>
		public void RestoreViewState(Properties properties)
		{
			ExtTreeView.ApplyViewStateString(properties.Get(ViewStatePropertyName, String.Empty), this);
		}
		
		/// <summary>
		/// Appends a new child element to the currently selected node.
		/// </summary>
		public void AppendChildElement(XmlElement element)
		{
			XmlElementTreeNode selectedNode = SelectedElementNode;
			if (selectedNode != null) {
				XmlElementTreeNode newNode = new XmlElementTreeNode(element);
				newNode.AddTo(selectedNode);
				selectedNode.Expand();
			}
		}
		
		/// <summary>
		/// Appends a new child text node to the currently selected element.
		/// </summary>
		public void AppendChildTextNode(XmlText textNode)
		{
			XmlElementTreeNode selectedNode = SelectedElementNode;
			if (selectedNode != null) {
				XmlTextTreeNode newNode = new XmlTextTreeNode(textNode);
				newNode.AddTo(selectedNode);
				selectedNode.Expand();
			}
		}
		
		/// <summary>
		/// Inserts a new element node before the currently selected
		/// node.
		/// </summary>
		public void InsertElementBefore(XmlElement element)
		{
			InsertElement(element, InsertionMode.Before);
		}
		
		/// <summary>
		/// Inserts a new element node after the currently selected
		/// node.
		/// </summary>
		public void InsertElementAfter(XmlElement element)
		{
			InsertElement(element, InsertionMode.After);
		}
		
		/// <summary>
		/// Removes the specified element from the tree.
		/// </summary>
		public void RemoveElement(XmlElement element)
		{
			XmlElementTreeNode selectedElementTreeNode = SelectedNode as XmlElementTreeNode;
			if (selectedElementTreeNode != null && selectedElementTreeNode.XmlElement == element) {
				// Remove selected tree node.
				selectedElementTreeNode.Remove();
			} else {
				XmlElementTreeNode elementTreeNode = FindElementNode(element, Nodes);
				if (elementTreeNode != null) {
					elementTreeNode.Remove();
				}
			}
		}
		
		/// <summary>
		/// Removes the specified text node from the tree.
		/// </summary>
		public void RemoveTextNode(XmlText textNode)
		{
			XmlTextTreeNode selectedTextTreeNode = SelectedNode as XmlTextTreeNode;
			if (selectedTextTreeNode != null && selectedTextTreeNode.XmlText == textNode) {
				selectedTextTreeNode.Remove();
			} else {
				XmlTextTreeNode textTreeNode = FindTextNode(textNode, Nodes);
				if (textTreeNode != null) {
					textTreeNode.Remove();
				}
			}
		}
		
		/// <summary>
		/// Inserts a text node before the currently selected
		/// node.
		/// </summary>
		public void InsertTextNodeBefore(XmlText textNode)
		{
			InsertTextNode(textNode, InsertionMode.Before);
		}
		
		/// <summary>
		/// Inserts a text node after the currently selected
		/// node.
		/// </summary>
		public void InsertTextNodeAfter(XmlText textNode)
		{
			InsertTextNode(textNode, InsertionMode.After);
		}
		
		/// <summary>
		/// Updates the corresponding tree node's text based on 
		/// the textNode's value.
		/// </summary>
		public void UpdateTextNode(XmlText textNode)
		{
			XmlTextTreeNode selectedTextTreeNode = SelectedNode as XmlTextTreeNode;
			if (selectedTextTreeNode != null && selectedTextTreeNode.XmlText == textNode) {
				selectedTextTreeNode.Update();
			} else {
				XmlTextTreeNode textTreeNode = FindTextNode(textNode, Nodes);
				if (textTreeNode != null) {
					textTreeNode.Update();
				}
			}
		}
		
		/// <summary>
		/// If no node is selected after a mouse click then we make 
		/// sure the AfterSelect event is fired. Standard behaviour is
		/// for the AfterSelect event not to be fired when the user
		/// deselects all tree nodes.
		/// </summary>
		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			if (SelectedNode == null) {
				this.OnAfterSelect(new TreeViewEventArgs(null, TreeViewAction.ByMouse));
			}
		}

		/// <summary>
		/// Displays the document in the xml tree.
		/// </summary>
		void ShowDocumentElement()
		{
			Nodes.Clear();
			if (documentElement != null) {
				XmlElementTreeNode node = new XmlElementTreeNode(documentElement);
				node.AddTo(this);
			}
		}
		
		/// <summary>
		/// Returns the selected xml element tree node.
		/// </summary>
		XmlElementTreeNode SelectedElementNode {
			get {
				return SelectedNode as XmlElementTreeNode;
			}
		}
		
		/// <summary>
		/// Inserts a new element node either before or after the 
		/// currently selected element node.
		/// </summary>
		void InsertElement(XmlElement element, InsertionMode insertionMode)
		{
			XmlElementTreeNode selectedNode = SelectedElementNode;
			if (selectedNode != null) {
				XmlElementTreeNode parentNode = (XmlElementTreeNode)selectedNode.Parent;
				XmlElementTreeNode newNode = new XmlElementTreeNode(element);
				int index = parentNode.Nodes.IndexOf(selectedNode);
				if (insertionMode == InsertionMode.After) {
					index++;
				}
				newNode.Insert(index, parentNode);
			}
		}
		
		/// <summary>
		/// Inserts a new text node either before or after the 
		/// currently selected node.
		/// </summary>
		void InsertTextNode(XmlText textNode, InsertionMode insertionMode)
		{
			ExtTreeNode selectedNode = (ExtTreeNode)SelectedNode;
			if (selectedNode != null) {
				XmlElementTreeNode parentNode = (XmlElementTreeNode)selectedNode.Parent;
				XmlTextTreeNode newNode = new XmlTextTreeNode(textNode);
				int index = parentNode.Nodes.IndexOf(selectedNode);
				if (insertionMode == InsertionMode.After) {
					index++;
				}
				newNode.Insert(index, parentNode);
			}
		}
		
		/// <summary>
		/// Looks at all the nodes in the tree view and returns the
		/// tree node that represents the specified element.
		/// </summary>
		XmlElementTreeNode FindElementNode(XmlElement element, TreeNodeCollection nodes)
		{
			foreach (ExtTreeNode node in nodes) {
				XmlElementTreeNode elementTreeNode = node as XmlElementTreeNode;
				if (elementTreeNode != null) {
					if (elementTreeNode.XmlElement == element) {
						return elementTreeNode;
					}
					
					// Look for a match in the element's child nodes.
					XmlElementTreeNode childElementTreeNode = FindElementNode(element, elementTreeNode.Nodes);
					if (childElementTreeNode != null) {
						return childElementTreeNode;
					}
				}
			}
			return null;
		}
		
		/// <summary>
		/// Looks at all the nodes in the tree view and returns the
		/// tree node that represents the specified text node.
		/// </summary>
		XmlTextTreeNode FindTextNode(XmlText textNode, TreeNodeCollection nodes)
		{
			foreach (ExtTreeNode node in nodes) {
				XmlTextTreeNode textTreeNode = node as XmlTextTreeNode;
				if (textTreeNode != null) {
					if (textTreeNode.XmlText == textNode) {
						return textTreeNode;
					}
				} else {
					// Look for a match in the node's child nodes.
					XmlTextTreeNode childTextTreeNode = FindTextNode(textNode, node.Nodes);
					if (childTextTreeNode != null) {
						return childTextTreeNode;
					}
				}
			}
			return null;
		}
	}
}
