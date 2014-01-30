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
using System.Xml;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Workbench;

namespace ICSharpCode.WixBinding
{
	/// <summary>
	/// Displays the setup package files.
	/// </summary>
	public class PackageFilesView : AbstractViewContentWithoutFile, IWixDocumentWriter
	{
		IWixPackageFilesControl packageFilesControl;
		WixProject project;
		IWorkbench workbench;
		bool reload;
		WixDocumentWindow wixDocumentWindow;
		OpenTextEditors openTextEditors;
		WixTextWriter wixTextWriter;
		
		public PackageFilesView(WixProject project, IWorkbench workbench)
			: this(project, workbench, new WixPackageFilesControl())
		{
		}
		
		public PackageFilesView(WixProject project, IWorkbench workbench, IWixPackageFilesControl packageFilesControl)
			: this(project, workbench, packageFilesControl, null)
		{
			wixTextWriter = new WixTextWriter(GetTextEditorOptions());
		}
		
		public PackageFilesView(WixProject project, 
			IWorkbench workbench, 
			IWixPackageFilesControl packageFilesControl,
			WixTextWriter wixTextWriter)
		{
			this.packageFilesControl = packageFilesControl;
			packageFilesControl.DirtyChanged += delegate { base.RaiseIsDirtyChanged(); };
			SetLocalizedTitle("${res:ICSharpCode.WixBinding.PackageFilesView.Title}");
			this.project = project;
			
			this.workbench = workbench;
			wixDocumentWindow = new WixDocumentWindow(workbench);
			workbench.ActiveViewContentChanged += ActiveViewContentChanged;
			
			this.wixTextWriter = wixTextWriter;
			
			openTextEditors = new OpenTextEditors(workbench);
		}
		
		static ITextEditorOptions GetTextEditorOptions()
		{
			ICSharpCode.AvalonEdit.TextEditor editor = new ICSharpCode.AvalonEdit.TextEditor();
			AvalonEditTextEditorAdapter adapter = new AvalonEditTextEditorAdapter(editor);
			return adapter.Options;
		}
		
		public override object Control {
			get { return packageFilesControl; }
		}
		
		public bool IsActiveWindow {
			get { return Object.ReferenceEquals(workbench.ActiveViewContent, this); }
		}

		public bool IsForProject(WixProject project)
		{
			return this.project == project;
		}
		
		public override void Load()
		{
		}
		
		public override void Save()
		{
			packageFilesControl.Save();
		}
		
		public override bool IsDirty {
			get { return packageFilesControl.IsDirty; }
		}
		
		public override void Dispose()
		{
			if (packageFilesControl != null) {
				workbench.ActiveViewContentChanged -= ActiveViewContentChanged;
				packageFilesControl.Dispose();
				packageFilesControl = null;
			}
			base.Dispose();
		}
		
		public void Write(WixDocument document)
		{
			ITextEditor openTextEditor = openTextEditors.FindTextEditorForDocument(document);
			if (openTextEditor != null) {
				UpdateOpenTextEditor(openTextEditor, document);
			} else {
				using (XmlWriter xmlWriter = wixTextWriter.Create(document.FileName)) {
					document.Save(xmlWriter);
				}
				FileUtility.RaiseFileSaved(new FileNameEventArgs(document.FileName));
			}
			packageFilesControl.IsDirty = false;
		}
		
		/// <summary>
		/// Adds a new child element with the given name to the selected tree node.
		/// </summary>
		public void AddElement(string name)
		{
			packageFilesControl.AddElement(name);
		}
		
		/// <summary>
		/// Removes the selected element from the Wix document.
		/// </summary>
		public void RemoveSelectedElement()
		{
			packageFilesControl.RemoveSelectedElement();
		}
		
		/// <summary>
		/// Adds files to the selected Component element or Directory element tree node.
		/// </summary>
		public void AddFiles()
		{
			packageFilesControl.AddFiles();
		}
		
		public void ShowFiles()
		{
			packageFilesControl.ShowFiles(project, new WorkbenchTextFileReader(), this);
		}
		
		/// <summary>
		/// Adds a directory and all its contents to the selected Directory
		/// element tree node.
		/// </summary>
		public void AddDirectory()
		{
			packageFilesControl.AddDirectory();
		}
		
		public void CalculateDiff()
		{
			packageFilesControl.CalculateDiff();
		}
		
		public void HideDiff()
		{
			packageFilesControl.IsDiffVisible = false;
		}
		
		void UpdateOpenTextEditor(ITextEditor textEditor, WixDocument document)
		{
			if (document.HasProduct) {
				UpdateOpenTextEditorWithRootDirectoryChanges(textEditor, document);
			} else {
				UpdateOpenTextEditorWithRootDirectoryRefChanges(textEditor, document);
			}
		}
		
		/// <summary>
		/// When the user switches away from the package files view to the corresponding
		/// Wix document then we update the document's contents. When the user switches
		/// back we reload the view if the corresponding Wix document is open.
		/// </summary>
		void ActiveViewContentChanged(object source, EventArgs e)
		{
			if (IsWixDocumentWindowActive) {
				if (IsDirty) {
					// Set IsDirty to false first since we get another workbench window
					// changed event whilst updating the open file. The
					// DefaultDocument.Replace method triggers this.
					ITextEditor textEditor = openTextEditors.FindTextEditorForDocument(packageFilesControl.Document);
					if (textEditor != null) {
						UpdateOpenTextEditor(textEditor, packageFilesControl.Document);
						packageFilesControl.IsDirty = false;
					}
				}
				reload = true;
			} else if (reload && IsActiveWindow) {
				ShowFiles();
				reload = false;
			}
		}
		
		bool IsWixDocumentWindowActive {
			get { return wixDocumentWindow.IsActive(packageFilesControl.Document); }
		}
		
		void UpdateOpenTextEditorWithRootDirectoryChanges(ITextEditor textEditor, WixDocument document)
		{
			WixDirectoryElement rootDirectory = document.GetRootDirectory();
			string xml = rootDirectory.GetXml(wixTextWriter);
			
			WixDocumentEditor documentEditor = new WixDocumentEditor(textEditor);
			DomRegion region = documentEditor.ReplaceElement(rootDirectory.Id, WixDirectoryElement.DirectoryElementName, xml);
			if (!region.IsEmpty) {
				return;
			}
			
			TextLocation location = FindProductElementEndLocation(textEditor, document);
			if (!location.IsEmpty) {
				documentEditor.InsertIndented(location, String.Concat(xml, "\r\n"));
			}
		}
		
		void UpdateOpenTextEditorWithRootDirectoryRefChanges(ITextEditor textEditor, WixDocument document)
		{
			WixDirectoryRefElement rootDirectoryRef = document.GetRootDirectoryRef();
			string xml = rootDirectoryRef.GetXml(wixTextWriter);
			
			WixDocumentEditor documentEditor = new WixDocumentEditor(textEditor);
			documentEditor.ReplaceElement(rootDirectoryRef.Id, WixDirectoryRefElement.DirectoryRefElementName, xml);
		}
		
		TextLocation FindProductElementEndLocation(ITextEditor textEditor, WixDocument document)
		{
			XmlElement productElement = document.GetProduct();
			string productId = productElement.GetAttribute("Id");
			WixDocumentReader wixReader = new WixDocumentReader(textEditor.Document.Text);
			return wixReader.GetEndElementLocation("Product", productId);
		}
	}
}
