// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using ICSharpCode.SharpDevelop.Gui;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.Core;

namespace SearchAndReplace
{
	public class SearchReplaceManager
	{
		public static SearchAndReplaceDialog SearchAndReplaceDialog = null;
		
		static Search find                  = new Search();
		
		static SearchReplaceManager()
		{
			find.TextIteratorBuilder = new ForwardTextIteratorBuilder();
		}
		
		static void SetSearchOptions()
		{
			find.SearchStrategy   = SearchReplaceUtilities.CreateSearchStrategy(SearchOptions.SearchStrategyType);
			find.DocumentIterator = SearchReplaceUtilities.CreateDocumentIterator(SearchOptions.DocumentIteratorType);
		}
		
		public static void Replace()
		{
			SetSearchOptions();
			if (lastResult != null && WorkbenchSingleton.Workbench.ActiveWorkbenchWindow != null) {
				ITextEditorControlProvider provider = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent as ITextEditorControlProvider;
				if (provider != null) {
					TextEditorControl textarea = provider.TextEditorControl;
					SelectionManager selectionManager = textarea.ActiveTextAreaControl.TextArea.SelectionManager;
					
					if (selectionManager.SelectionCollection.Count == 1
					    && selectionManager.SelectionCollection[0].Offset == lastResult.Offset
					    && selectionManager.SelectionCollection[0].Length == lastResult.Length
					    && lastResult.FileName == textarea.FileName)
					{
						string replacePattern = lastResult.TransformReplacePattern(SearchOptions.ReplacePattern);
						
						textarea.BeginUpdate();
						selectionManager.ClearSelection();
						textarea.Document.Replace(lastResult.Offset, lastResult.Length, replacePattern);
						textarea.ActiveTextAreaControl.Caret.Position = textarea.Document.OffsetToPosition(lastResult.Offset + replacePattern.Length);
						textarea.EndUpdate();
					}
				}
			}
			FindNext();
		}
		
		public static void MarkAll()
		{
			SetSearchOptions();
			TextEditorControl textArea = null;
			if (WorkbenchSingleton.Workbench.ActiveWorkbenchWindow != null) {
				textArea = ((ITextEditorControlProvider)WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent).TextEditorControl;
				textArea.ActiveTextAreaControl.TextArea.SelectionManager.ClearSelection();
			}
			find.Reset();
			if (!find.SearchStrategy.CompilePattern())
				return;
			while (true) {
				SearchResult result = SearchReplaceManager.find.FindNext();
				
				if (result == null) {
					
					MessageService.ShowMessage("${res:ICSharpCode.TextEditor.Document.SearchReplaceManager.MarkAllDone}", "${res:Global.FinishedCaptionText}");
					find.Reset();
					return;
				} else {
					textArea = OpenTextArea(result.FileName);
					
					textArea.ActiveTextAreaControl.Caret.Position = textArea.Document.OffsetToPosition(result.Offset);
					int lineNr = textArea.Document.GetLineNumberForOffset(result.Offset);
					
					if (!textArea.Document.BookmarkManager.IsMarked(lineNr)) {
						textArea.Document.BookmarkManager.ToggleMarkAt(lineNr);
					}
				}
			}
		}
		
		public static void ReplaceAll()
		{
			SetSearchOptions();
			if (WorkbenchSingleton.Workbench.ActiveWorkbenchWindow != null) {
				ITextEditorControlProvider provider = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent as ITextEditorControlProvider;
				if (provider != null) {
					provider.TextEditorControl.ActiveTextAreaControl.TextArea.SelectionManager.ClearSelection();
				}
			}
			find.Reset();
			if (!find.SearchStrategy.CompilePattern())
				return;
			
			List<TextEditorControl> textAreas = new List<TextEditorControl>();
			while (true) {
				SearchResult result = SearchReplaceManager.find.FindNext();
				
				if (result == null) {
					foreach (TextEditorControl textArea in textAreas) {
						textArea.EndUpdate();
						textArea.Refresh();
					}
					MessageService.ShowMessage("${res:ICSharpCode.TextEditor.Document.SearchReplaceManager.ReplaceAllDone}", "${res:Global.FinishedCaptionText}");
					find.Reset();
					return;
				} else {
					TextEditorControl textArea = OpenTextArea(result.FileName);
					
					if (!textAreas.Contains(textArea)) {
						textArea.BeginUpdate();
						textArea.ActiveTextAreaControl.TextArea.SelectionManager.SelectionCollection.Clear();
						textAreas.Add(textArea);
					}
					
					string transformedPattern = result.TransformReplacePattern(SearchOptions.ReplacePattern);
					find.Replace(result.Offset,
					             result.Length,
					             transformedPattern);
				}
			}
		}
		
		static SearchResult lastResult = null;
		public static void FindNext()
		{
			SetSearchOptions();
			if (find == null ||
			    SearchOptions.FindPattern == null ||
			    SearchOptions.FindPattern.Length == 0) {
				return;
			}
			
			if (!find.SearchStrategy.CompilePattern()) {
				find.Reset();
				lastResult = null;
				return;
			}
			SearchResult result = find.FindNext();
			
			if (result == null) {
				MessageBox.Show((Form)WorkbenchSingleton.Workbench,
				                ResourceService.GetString("Dialog.NewProject.SearchReplace.SearchStringNotFound"),
				                "Not Found",
				                MessageBoxButtons.OK,
				                MessageBoxIcon.Information);
				find.Reset();
			} else {
				TextEditorControl textArea = OpenTextArea(result.FileName);
				
				if (lastResult != null  && lastResult.FileName == result.FileName &&
				    textArea.ActiveTextAreaControl.Caret.Offset != lastResult.Offset + lastResult.Length) {
					find.Reset();
				}
				int startPos = Math.Min(textArea.Document.TextLength, Math.Max(0, result.Offset));
				int endPos   = Math.Min(textArea.Document.TextLength, startPos + result.Length);
				
				textArea.ActiveTextAreaControl.Caret.Position = textArea.Document.OffsetToPosition(endPos);
				textArea.ActiveTextAreaControl.TextArea.SelectionManager.ClearSelection();
				textArea.ActiveTextAreaControl.TextArea.SelectionManager.SetSelection(new DefaultSelection(textArea.Document, textArea.Document.OffsetToPosition(startPos),
				                                                                                           textArea.Document.OffsetToPosition(endPos)));
				textArea.Refresh();
			}
			
			lastResult = result;
		}
		
		static TextEditorControl OpenTextArea(string fileName)
		{
			if (fileName != null) {
				return ((ITextEditorControlProvider)FileService.OpenFile(fileName).ViewContent).TextEditorControl;
			}
			
			return ((ITextEditorControlProvider)WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent).TextEditorControl;
		}
	}
}
