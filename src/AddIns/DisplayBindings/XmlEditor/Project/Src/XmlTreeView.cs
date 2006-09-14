// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using System;
using System.Windows.Forms;
using System.Xml;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// The secondary view content that displays the XML document as a tree view. 
	/// </summary>
	public class XmlTreeView : AbstractSecondaryViewContent
	{
		XmlTreeViewContainerControl treeViewContainer = new XmlTreeViewContainerControl();
		XmlView xmlView;
		bool disposed;
		
		public XmlTreeView(XmlView xmlView)
		{
			this.xmlView = xmlView;
			treeViewContainer.DirtyChanged += TreeViewContainerDirtyChanged;
		}
		
		public override Control Control {
			get {
				return treeViewContainer;
			}
		}
		
		public override string TabPageText {
			get {
				return "XML Tree";
			}
		}
		
		public override void Dispose()
		{
			if (!disposed) {
				disposed = true;
				treeViewContainer.Dispose();
			}
		}
		
		public override void Selected()
		{
			treeViewContainer.LoadXml(xmlView.Text);
			xmlView.CheckIsWellFormed();
		}
		
		public override void Deselecting()
		{
			if (!disposed) {
				if (treeViewContainer.IsDirty) {
					xmlView.ReplaceAll(treeViewContainer.Document.OuterXml);
					treeViewContainer.IsDirty = false;
				}
			}
		}
		
		public XmlElement DocumentElement {
			get {
				return treeViewContainer.TreeView.DocumentElement;
			}
			set {
				treeViewContainer.DocumentElement = value;
			}
		}
		
		void TreeViewContainerDirtyChanged(object source, EventArgs e)
		{
			xmlView.IsDirty = treeViewContainer.IsDirty;
		}
	}
}
