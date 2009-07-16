// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

namespace SearchAndReplace
{
	public sealed class SearchReplaceUtilities
	{
		public static bool IsTextAreaSelected {
			get {
				return WorkbenchSingleton.Workbench.ActiveViewContent != null &&
					WorkbenchSingleton.Workbench.ActiveViewContent is ITextEditorControlProvider;
			}
		}
		
		public static TextEditorControl GetActiveTextEditor()
		{
			IViewContent content = WorkbenchSingleton.Workbench.ActiveViewContent;
			if (content is ITextEditorControlProvider) {
				return ((ITextEditorControlProvider)content).TextEditorControl;
			}
			return null;
		}
		
		public static bool IsWholeWordAt(ITextBufferStrategy document, int offset, int length)
		{
			return (offset - 1 < 0 || Char.IsWhiteSpace(document.GetCharAt(offset - 1))) &&
				(offset + length + 1 >= document.Length || Char.IsWhiteSpace(document.GetCharAt(offset + length)));
		}
		
		public static ISearchStrategy CreateSearchStrategy(SearchStrategyType type)
		{
			switch (type) {
				case SearchStrategyType.Normal:
					return new BruteForceSearchStrategy(); // new KMPSearchStrategy();
				case SearchStrategyType.RegEx:
					return new RegExSearchStrategy();
				case SearchStrategyType.Wildcard:
					return new WildcardSearchStrategy();
				default:
					throw new System.NotImplementedException("CreateSearchStrategy for type " + type);
			}
		}
		
		public static IDocumentIterator CreateDocumentIterator(DocumentIteratorType type, IProgressMonitor monitor)
		{
			switch (type) {
				case DocumentIteratorType.CurrentDocument:
				case DocumentIteratorType.CurrentSelection:
					return new CurrentDocumentIterator();
				case DocumentIteratorType.Directory:
					try {
						if (!Directory.Exists(SearchOptions.LookIn)) {
							if (monitor != null) monitor.ShowingDialog = true;
							MessageService.ShowMessageFormatted("${res:Dialog.NewProject.SearchReplace.SearchStringNotFound.Title}", "${res:Dialog.NewProject.SearchReplace.LookIn.DirectoryNotFound}", FileUtility.NormalizePath(SearchOptions.LookIn));
							if (monitor != null) monitor.ShowingDialog = false;
							return new DummyDocumentIterator();
						}
					} catch (Exception ex) {
						if (monitor != null) monitor.ShowingDialog = true;
						MessageService.ShowMessage(ex.Message);
						if (monitor != null) monitor.ShowingDialog = false;
						return new DummyDocumentIterator();
					}
					return new DirectoryDocumentIterator(SearchOptions.LookIn,
					                                     SearchOptions.LookInFiletypes,
					                                     SearchOptions.IncludeSubdirectories);
				case DocumentIteratorType.AllOpenFiles:
					return new AllOpenDocumentIterator();
				case DocumentIteratorType.WholeProject:
					return new WholeProjectDocumentIterator();
				case DocumentIteratorType.WholeSolution:
					return new WholeSolutionDocumentIterator();
				default:
					throw new System.NotImplementedException("CreateDocumentIterator for type " + type);
			}
		}
		
		static List<string> excludedFileExtensions;
		
		public static bool IsSearchable(string fileName)
		{
			if (fileName == null)
				return false;
			
			if (excludedFileExtensions == null) {
				excludedFileExtensions = AddInTree.BuildItems<string>("/AddIns/DefaultTextEditor/Search/ExcludedFileExtensions", null, false);
			}
			string extension = Path.GetExtension(fileName);
			if (extension != null) {
				foreach (string excludedExtension in excludedFileExtensions) {
					if (String.Compare(excludedExtension, extension, true) == 0) {
						return false;
					}
				}
			}
			return true;
		}
		
		public static void SelectText(TextEditorControl textArea, int offset, int endOffset)
		{
			int textLength = textArea.ActiveTextAreaControl.Document.TextLength;
			if (textLength < endOffset) {
				endOffset = textLength - 1;
			}
			textArea.ActiveTextAreaControl.Caret.Position = textArea.Document.OffsetToPosition(endOffset);
			textArea.ActiveTextAreaControl.TextArea.SelectionManager.ClearSelection();
			textArea.ActiveTextAreaControl.TextArea.SelectionManager.SetSelection(new DefaultSelection(textArea.Document, textArea.Document.OffsetToPosition(offset),
			                                                                                           textArea.Document.OffsetToPosition(endOffset)));
			textArea.Refresh();
		}
	}
}
