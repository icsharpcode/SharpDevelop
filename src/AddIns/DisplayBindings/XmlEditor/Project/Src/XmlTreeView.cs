// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using System.Xml;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.WinForms;
using ICSharpCode.SharpDevelop.Workbench;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// The secondary view content that displays the XML document as a tree view.
	/// </summary>
	public class XmlTreeView : AbstractSecondaryViewContent, IClipboardHandler
	{
		XmlTreeViewContainerControl treeViewContainer;
		XmlSchemaCompletionCollection schemas;
		XmlSchemaCompletion defaultSchema;
		bool ignoreDirtyChange;
		
		public XmlTreeView(IViewContent parent, XmlSchemaCompletionCollection schemas, XmlSchemaCompletion defaultSchema)
			: base(parent)
		{
			this.schemas = schemas;
			this.defaultSchema = defaultSchema;
			
			this.TabPageText = "${res:ICSharpCode.XmlEditor.XmlTreeView.Title}";
			this.treeViewContainer = new XmlTreeViewContainerControl(schemas, defaultSchema);
			this.treeViewContainer.DirtyChanged += TreeViewContainerDirtyChanged;
			treeViewContainer.AttributesGrid.ContextMenuStrip = MenuService.CreateContextMenu(treeViewContainer, "/AddIns/XmlEditor/XmlTree/AttributesGrid/ContextMenu");
			treeViewContainer.TreeView.ContextMenuStrip = MenuService.CreateContextMenu(treeViewContainer, "/AddIns/XmlEditor/XmlTree/ContextMenu");
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
		
		public override object Control {
			get { return this.treeViewContainer; }
		}
		
		protected override void LoadFromPrimary()
		{
			IFileDocumentProvider provider = this.PrimaryViewContent.GetRequiredService<IFileDocumentProvider>();
			IDocument document = provider.GetDocumentForFile(this.PrimaryFile);
			treeViewContainer.LoadXml(document.Text);
			XmlView view = XmlView.ForFile(this.PrimaryFile);
			if (view != null) {
				XmlView.CheckIsWellFormed(view.TextEditor);
			}
		}
		
		protected override void SaveToPrimary()
		{
			// Do not modify text in the primary view if the data is not well-formed XML
			if (!treeViewContainer.IsErrorMessageTextBoxVisible && treeViewContainer.IsDirty) {
				XmlView view = XmlView.ForFile(this.PrimaryFile);
				if (view != null) {
					XmlView.ReplaceAll(treeViewContainer.Document.OuterXml, view.TextEditor);
					ignoreDirtyChange = true;
					treeViewContainer.IsDirty = false;
					ignoreDirtyChange = false;
				}
			}
		}
		
		public string XmlContent {
			get {
				StringWriter str = new StringWriter(CultureInfo.InvariantCulture);
				XmlTextWriter writer = new XmlTextWriter(str);
				
				writer.Formatting = Formatting.Indented;
				treeViewContainer.Document.WriteTo(writer);
				return str.ToString();
			}
		}
		
		public override bool SupportsSwitchFromThisWithoutSaveLoad(OpenedFile file, IViewContent newView)
		{
			return false;
		}
		
		public override bool SupportsSwitchToThisWithoutSaveLoad(OpenedFile file, IViewContent oldView)
		{
			return false;
		}
		
		void TreeViewContainerDirtyChanged(object source, EventArgs e)
		{
			if (!ignoreDirtyChange) {
				this.PrimaryFile.IsDirty = treeViewContainer.IsDirty;
			}
		}
		
		bool disposed;
		
		public override void Dispose()
		{
			LoggingService.Debug("XmlTreeView.Dispose");
			
			if (!disposed) {
				disposed = true;
				treeViewContainer.Dispose();
			}
			base.Dispose();
		}
	}
}
