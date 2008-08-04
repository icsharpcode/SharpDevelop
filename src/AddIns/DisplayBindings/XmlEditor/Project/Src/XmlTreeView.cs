// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// The secondary view content that displays the XML document as a tree view.
	/// </summary>
	public class XmlTreeView : AbstractSecondaryViewContent, IClipboardHandler
	{
		XmlTreeViewContainerControl treeViewContainer = new XmlTreeViewContainerControl();
		XmlView xmlView;
		bool disposed;
		bool ignoreDirtyChange;
		
		public XmlTreeView(XmlView xmlView) : this(xmlView, null, null)
		{
			treeViewContainer.AttributesGrid.ContextMenuStrip = MenuService.CreateContextMenu(treeViewContainer, "/AddIns/XmlEditor/XmlTree/AttributesGrid/ContextMenu");
			treeViewContainer.TreeView.ContextMenuStrip = MenuService.CreateContextMenu(treeViewContainer, "/AddIns/XmlEditor/XmlTree/ContextMenu");
		}
		
		/// <summary>
		/// Creates an XmlTreeView with the specified context menu strips.
		/// This constructor is only used to test the XmlTreeView class.
		/// </summary>
		public XmlTreeView(XmlView xmlView, ContextMenuStrip attributesGridContextMenuStrip, ContextMenuStrip treeViewContextMenuStrip)
			: base(xmlView)
		{
			this.TabPageText = "${res:ICSharpCode.XmlEditor.XmlTreeView.Title}";
			
			this.xmlView = xmlView;
			treeViewContainer.DirtyChanged += TreeViewContainerDirtyChanged;
			treeViewContainer.AttributesGrid.ContextMenuStrip = attributesGridContextMenuStrip;
			treeViewContainer.TreeView.ContextMenuStrip = treeViewContextMenuStrip;
		}
		
		public override Control Control {
			get {
				return treeViewContainer;
			}
		}
		
		public override void Dispose()
		{
			LoggingService.Debug("XmlTreeView.Dispose");
			
			if (!disposed) {
				disposed = true;
				treeViewContainer.Dispose();
			}
			base.Dispose();
		}
		
		protected override void LoadFromPrimary()
		{
			LoggingService.Debug("XmlTreeView.LoadFromPrimary");
			
			XmlEditorControl xmlEditor = xmlView.XmlEditor;
			XmlCompletionDataProvider completionDataProvider = new XmlCompletionDataProvider(xmlEditor.SchemaCompletionDataItems, xmlEditor.DefaultSchemaCompletionData, xmlEditor.DefaultNamespacePrefix);
			treeViewContainer.LoadXml(xmlView.Text, completionDataProvider);
			xmlView.CheckIsWellFormed();
		}
		
		protected override void SaveToPrimary()
		{
			LoggingService.Debug("XmlTreeView.SaveToPrimary");
			
			if (treeViewContainer.IsDirty) {
				xmlView.ReplaceAll(treeViewContainer.Document.OuterXml);
				ignoreDirtyChange = true;
				treeViewContainer.IsDirty = false;
				ignoreDirtyChange = false;
			}
		}
		
		#region IClipboardHandler implementation
		
		/// <summary>
		/// Gets whether the edit menu's cut command should be enabled.
		/// </summary>
		public bool EnableCut {
			get {
				return treeViewContainer.EnableCut;
			}
		}
		
		/// <summary>
		/// Gets whether the edit menu's copy command should be enabled.
		/// </summary>
		public bool EnableCopy {
			get {
				return treeViewContainer.EnableCopy;
			}
		}
		
		/// <summary>
		/// Gets whether the edit menu's paste command should be enabled.
		/// </summary>
		public bool EnablePaste {
			get {
				return treeViewContainer.EnablePaste;
			}
		}
		
		/// <summary>
		/// Gets whether the edit menu's delete command should be enabled.
		/// </summary>
		public bool EnableDelete {
			get {
				return treeViewContainer.EnableDelete;
			}
		}
		
		/// <summary>
		/// Always returns false.
		/// </summary>
		public bool EnableSelectAll {
			get {
				return false;
			}
		}
		
		/// <summary>
		/// Cuts the selected tree node.
		/// </summary>
		public void Cut()
		{
			treeViewContainer.Cut();
		}
		
		/// <summary>
		/// Copies the selected tree node.
		/// </summary>
		public void Copy()
		{
			treeViewContainer.Copy();
		}
		
		/// <summary>
		/// Pastes the copied or cut node as a child of the selected tree node.
		/// </summary>
		public void Paste()
		{
			treeViewContainer.Paste();
		}
		
		/// <summary>
		/// Deletes the selected tree node.
		/// </summary>
		public void Delete()
		{
			treeViewContainer.Delete();
		}
		
		/// <summary>
		/// Select all is not currently supported.
		/// </summary>
		public void SelectAll()
		{
		}
		
		#endregion
		
		void TreeViewContainerDirtyChanged(object source, EventArgs e)
		{
			if (!ignoreDirtyChange) {
				this.PrimaryFile.IsDirty = treeViewContainer.IsDirty;
			}
		}
	}
}
