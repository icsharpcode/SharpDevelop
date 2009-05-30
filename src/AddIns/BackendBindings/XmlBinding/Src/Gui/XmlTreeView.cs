// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 4018 $</version>
// </file>

using System;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.XmlEditor.Gui;
using System.Xml;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// The secondary view content that displays the XML document as a tree view.
	/// </summary>
	public class XmlTreeView : AbstractSecondaryViewContent, IClipboardHandler
	{
		XmlTreeViewContainerControl treeViewContainer;
		
		public XmlTreeView(IViewContent parent)
			: base(parent)
		{
			this.TabPageText = "XML Tree";
			this.treeViewContainer = new XmlTreeViewContainerControl();
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
			get {
				return this.treeViewContainer;
			}
		}
		
		protected override void LoadFromPrimary()
		{
			var provider = this.PrimaryViewContent as ITextEditorProvider;
			treeViewContainer.LoadXml(provider.TextEditor.Document.Text, XmlView.GetProvider(Path.GetExtension(provider.TextEditor.FileName)));
		}
		
		protected override void SaveToPrimary()
		{
			// Do not modify text in the primary view if the data is not well-formed XML
			if (!treeViewContainer.IsErrorMessageTextBoxVisible) {
				ITextEditorProvider provider = this.PrimaryViewContent as ITextEditorProvider;
				StringWriter str = new StringWriter();
				XmlTextWriter writer = new XmlTextWriter(str);
				
				writer.Formatting = Formatting.Indented;
				treeViewContainer.Document.WriteTo(writer);
				provider.TextEditor.Document.Text = str.ToString();
			}
		}
		
		public string XmlContent {
			get {
				StringWriter str = new StringWriter();
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
	}
}
