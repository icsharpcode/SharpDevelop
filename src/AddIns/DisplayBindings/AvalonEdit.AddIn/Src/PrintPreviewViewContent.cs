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
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;

using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Workbench;

namespace ICSharpCode.AvalonEdit.AddIn
{
	public class PrintPreviewViewContent : AbstractViewContent
	{
		static readonly PrintDialog printDialog = new PrintDialog();
		
		public static PrintDialog PrintDialog {
			get { return printDialog; }
		}
		
		Action cleanup;
		DocumentViewer viewer = new DocumentViewer();
		
		public PrintPreviewViewContent()
		{
			SetLocalizedTitle("${res:Global.Preview}");
			viewer.CommandBindings.Add(new CommandBinding(ApplicationCommands.Print, OnPrint));
		}
		
		/// <summary>
		/// Original (unconverted) document.
		/// </summary>
		IDocumentPaginatorSource originalDocument;
		
		public IDocumentPaginatorSource Document {
			get { return originalDocument; }
			set {
				if (cleanup != null) {
					cleanup();
					cleanup = null;
				}
				
				if (value is FlowDocument) {
					// DocumentViewer does not support FlowDocument, so we'll convert it
					MemoryStream xpsStream = new MemoryStream();
					Package package = Package.Open(xpsStream, FileMode.Create, FileAccess.ReadWrite);
					string packageUriString = "memorystream://data.xps";
					PackageStore.AddPackage(new Uri(packageUriString), package);
					
					XpsDocument xpsDocument = new XpsDocument(package, CompressionOption.Normal, packageUriString);
					XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(xpsDocument);
					
					writer.Write(value.DocumentPaginator);
					viewer.Document = xpsDocument.GetFixedDocumentSequence();
					cleanup = delegate {
						viewer.Document = null;
						xpsDocument.Close();
						package.Close();
						PackageStore.RemovePackage(new Uri(packageUriString));
					};
				} else {
					viewer.Document = value;
				}
				originalDocument = value;
			}
		}
		
		public override void Dispose()
		{
			base.Dispose();
			if (cleanup != null) {
				cleanup();
				cleanup = null;
			}
		}
		
		public string Description { get; set; }
		
		public override object Control {
			get { return viewer; }
		}
		
		void OnPrint(object sender, ExecutedRoutedEventArgs e)
		{
			if (originalDocument == null)
				return;
			e.Handled = true;
			if (printDialog.ShowDialog() == true) {
				// re-apply settings if changed
				if (originalDocument is FlowDocument)
					ApplySettingsToFlowDocument(printDialog, (FlowDocument)originalDocument);
				printDialog.PrintDocument(originalDocument.DocumentPaginator, this.Description);
			}
		}
		
		public static void ApplySettingsToFlowDocument(PrintDialog printDialog, FlowDocument flowDocument)
		{
			flowDocument.ColumnGap = 0;
			flowDocument.ColumnWidth = printDialog.PrintableAreaWidth;
			flowDocument.PageHeight = printDialog.PrintableAreaHeight;
			flowDocument.PageWidth = printDialog.PrintableAreaWidth;
		}
		
		public static void ShowDocument(IDocumentPaginatorSource document, string description)
		{
			PrintPreviewViewContent vc = SD.Workbench.ViewContentCollection.OfType<PrintPreviewViewContent>().FirstOrDefault();
			if (vc != null) {
				vc.WorkbenchWindow.SelectWindow();
			} else {
				vc = new PrintPreviewViewContent();
				SD.Workbench.ShowView(vc);
			}
			vc.Document = document;
			vc.Description = description;
		}
	}
}
