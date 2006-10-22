// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Xml;

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
				XmlElementTreeNode xmlElementTreeNode = SelectedNode as XmlElementTreeNode;
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

		void ShowDocumentElement()
		{
			Nodes.Clear();
			if (documentElement != null) {
				XmlElementTreeNode node = new XmlElementTreeNode(documentElement);
				node.AddTo(this);
			}
		}
		
		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			if (SelectedNode == null) {
				this.OnAfterSelect(new TreeViewEventArgs(null, TreeViewAction.ByMouse));
			}
		}
	}
}
