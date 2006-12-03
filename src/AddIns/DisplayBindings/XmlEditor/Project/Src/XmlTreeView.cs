// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;
using System.Xml;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

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
		bool ignoreDirtyChange;
		
		public XmlTreeView(XmlView xmlView)
		{
			this.xmlView = xmlView;
			treeViewContainer.DirtyChanged += TreeViewContainerDirtyChanged;
			treeViewContainer.AttributesGrid.ContextMenuStrip = MenuService.CreateContextMenu(treeViewContainer, "/AddIns/XmlEditor/XmlTree/AttributesGrid/ContextMenu");
			treeViewContainer.TreeView.ContextMenuStrip = MenuService.CreateContextMenu(treeViewContainer, "/AddIns/XmlEditor/XmlTree/ContextMenu");
		}
		
		public override Control Control {
			get {
				return treeViewContainer;
			}
		}
		
		public override string TabPageText {
			get {
				return StringParser.Parse("${res:ICSharpCode.XmlEditor.XmlTreeView.Title}");
			}
		}
		
		public override void NotifyBeforeSave()
		{
			Deselecting();
		}
		
		public override void NotifyAfterSave(bool successful)
		{
			if (!successful) {
				ignoreDirtyChange = true;
				treeViewContainer.IsDirty = xmlView.IsDirty;
				ignoreDirtyChange = false;
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
			XmlEditorControl xmlEditor = xmlView.XmlEditor;
			XmlCompletionDataProvider completionDataProvider = new XmlCompletionDataProvider(xmlEditor.SchemaCompletionDataItems, xmlEditor.DefaultSchemaCompletionData, xmlEditor.DefaultNamespacePrefix);
			treeViewContainer.LoadXml(xmlView.Text, completionDataProvider);
			xmlView.CheckIsWellFormed();
		}
		
		public override void Deselecting()
		{
			if (!disposed) {
				if (treeViewContainer.IsDirty) {
					xmlView.ReplaceAll(treeViewContainer.Document.OuterXml);
					ignoreDirtyChange = true;
					treeViewContainer.IsDirty = false;
					ignoreDirtyChange = false;
				}
			}
		}
		
		void TreeViewContainerDirtyChanged(object source, EventArgs e)
		{
			if (!ignoreDirtyChange) {
				xmlView.IsDirty = treeViewContainer.IsDirty;
			}
		}
	}
}
