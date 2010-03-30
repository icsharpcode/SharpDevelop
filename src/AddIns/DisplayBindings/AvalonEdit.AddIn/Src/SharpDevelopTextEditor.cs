// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;

using ICSharpCode.AvalonEdit.AddIn.Options;
using ICSharpCode.AvalonEdit.AddIn.Snippets;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.AvalonEdit;
using ICSharpCode.SharpDevelop.Editor.Commands;

namespace ICSharpCode.AvalonEdit.AddIn
{
	/// <summary>
	/// The text editor used in SharpDevelop.
	/// Serves both as base class for CodeEditorView and as text editor control
	/// for editors used in other parts of SharpDevelop (e.g. all ConsolePad-based controls)
	/// </summary>
	public class SharpDevelopTextEditor : TextEditor
	{
		protected readonly CodeEditorOptions options;
		
		public SharpDevelopTextEditor()
		{
			AvalonEditDisplayBinding.RegisterAddInHighlightingDefinitions();
			
			this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Print, OnPrint));
			this.CommandBindings.Add(new CommandBinding(ApplicationCommands.PrintPreview, OnPrintPreview));
			
			options = ICSharpCode.AvalonEdit.AddIn.Options.CodeEditorOptions.Instance;
			options.BindToTextEditor(this);
		}
		
		protected virtual string FileName {
			get { return "untitled"; }
		}
		
		#region Printing
		void OnPrint(object sender, ExecutedRoutedEventArgs e)
		{
			PrintDialog printDialog = PrintPreviewViewContent.PrintDialog;
			if (printDialog.ShowDialog() == true) {
				FlowDocument fd = DocumentPrinter.CreateFlowDocumentForEditor(this);
				fd.ColumnGap = 0;
				fd.ColumnWidth = printDialog.PrintableAreaWidth;
				fd.PageHeight = printDialog.PrintableAreaHeight;
				fd.PageWidth = printDialog.PrintableAreaWidth;
				IDocumentPaginatorSource doc = fd;
				printDialog.PrintDocument(doc.DocumentPaginator, Path.GetFileName(this.FileName));
			}
		}
		
		void OnPrintPreview(object sender, ExecutedRoutedEventArgs e)
		{
			PrintDialog printDialog = PrintPreviewViewContent.PrintDialog;
			FlowDocument fd = DocumentPrinter.CreateFlowDocumentForEditor(this);
			fd.ColumnGap = 0;
			fd.ColumnWidth = printDialog.PrintableAreaWidth;
			fd.PageHeight = printDialog.PrintableAreaHeight;
			fd.PageWidth = printDialog.PrintableAreaWidth;
			PrintPreviewViewContent.ShowDocument(fd, Path.GetFileName(this.FileName));
		}
		#endregion
	}
}
