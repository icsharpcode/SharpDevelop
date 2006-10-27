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
		
		public XmlElement SelectedElement {
			get {
				XmlElementTreeNode xmlElementTreeNode = SelectedElementNode;
				if (xmlElementTreeNode != null) {
					return xmlElementTreeNode.XmlElement;
				}
				return null;
			}
		}
		
		public bool IsElementSelected {
			get {
				return SelectedElement != null;
			}
		}
		
		public XmlText SelectedTextNode {
			get {				
				XmlTextTreeNode xmlTextTreeNode = SelectedNode as XmlTextTreeNode;
				if (xmlTextTreeNode != null) {
					return xmlTextTreeNode.XmlText;
				}

				return null;
			}
		}
		
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
		
		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			if (SelectedNode == null) {
				this.OnAfterSelect(new TreeViewEventArgs(null, TreeViewAction.ByMouse));
			}
		}

		void ShowDocumentElement()
		{
			Nodes.Clear();
			if (documentElement != null) {
				XmlElementTreeNode node = new XmlElementTreeNode(documentElement);
				node.AddTo(this);
			}
		}
		
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
	}
}
