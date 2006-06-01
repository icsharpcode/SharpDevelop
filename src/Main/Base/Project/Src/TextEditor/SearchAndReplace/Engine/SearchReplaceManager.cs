// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
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
		
		static TextSelection textSelection;
		
		public static void ReplaceFirstInSelection(int offset, int length)
		{
			SetSearchOptions();
			FindFirstInSelection(offset, length);
		}
		
		public static bool ReplaceNextInSelection()
		{
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
						
						textSelection.Length -= lastResult.Length - replacePattern.Length;
					}
				}
			}
			return FindNextInSelection();
		}
		
		public static void MarkAll()
		{
			SetSearchOptions();
			ClearSelection();
			find.Reset();
			if (!find.SearchStrategy.CompilePattern())
				return;
			List<TextEditorControl> textAreas = new List<TextEditorControl>();
			int count;
			for (count = 0;; count++) {
				SearchResult result = SearchReplaceManager.find.FindNext();
				
				if (result == null) {
					break;
				} else {
					MarkResult(textAreas, result);
				}
			}
			find.Reset();
			foreach (TextEditorControl ctl in textAreas) {
				ctl.Refresh();
			}
			ShowMarkDoneMessage(count);
		}
		
		public static void MarkAll(int offset, int length)
		{
			SetSearchOptions();
			find.Reset();
			
			if (!find.SearchStrategy.CompilePattern())
				return;
			
			List<TextEditorControl> textAreas = new List<TextEditorControl>();
			int count;
			for (count = 0;; count++) {
				SearchResult result = find.FindNext(offset, length);
				if (result == null) {
					break;
				} else {
					MarkResult(textAreas, result);
				}
			}
			find.Reset();
			foreach (TextEditorControl ctl in textAreas) {
				ctl.Refresh();
			}
			ShowMarkDoneMessage(count);
		}
		
		static void MarkResult(List<TextEditorControl> textAreas, SearchResult result)
		{
			TextEditorControl textArea = OpenTextArea(result.FileName);
			if (textArea != null) {
				if (!textAreas.Contains(textArea)) {
					textAreas.Add(textArea);
				}
				textArea.ActiveTextAreaControl.Caret.Position = textArea.Document.OffsetToPosition(result.Offset);
				int lineNr = textArea.Document.GetLineNumberForOffset(result.Offset);
				
				if (!textArea.Document.BookmarkManager.IsMarked(lineNr)) {
					textArea.Document.BookmarkManager.ToggleMarkAt(lineNr);
				}
			}
		}
		
		static void ShowMarkDoneMessage(int count)
		{
			if (count == 0) {
				ShowNotFoundMessage();
			} else {
				MessageService.ShowMessage("${res:ICSharpCode.TextEditor.Document.SearchReplaceManager.MarkAllDone}", "${res:Global.FinishedCaptionText}");
			}
		}
		
		static void ShowReplaceDoneMessage(int count)
		{
			if (count == 0) {
				ShowNotFoundMessage();
			} else {
				MessageService.ShowMessage("${res:ICSharpCode.TextEditor.Document.SearchReplaceManager.ReplaceAllDone}", "${res:Global.FinishedCaptionText}");
			}
		}
		
		public static void ReplaceAll()
		{
			SetSearchOptions();
			ClearSelection();
			find.Reset();
			if (!find.SearchStrategy.CompilePattern())
				return;
			
			List<TextEditorControl> textAreas = new List<TextEditorControl>();
			TextEditorControl textArea = null;
			for (int count = 0;; count++) {
				SearchResult result = SearchReplaceManager.find.FindNext();
				
				if (result == null) {
					if (count != 0) {
						foreach (TextEditorControl ta in textAreas) {
							ta.EndUpdate();
							ta.Refresh();
						}
					}
					ShowReplaceDoneMessage(count);
					find.Reset();
					return;
				} else {
					if (textArea == null || textArea.FileName != result.FileName) {
						// we need to open another text area
						textArea = OpenTextArea(result.FileName);
						if (textArea != null) {
							if (!textAreas.Contains(textArea)) {
								textArea.BeginUpdate();
								textArea.ActiveTextAreaControl.TextArea.SelectionManager.SelectionCollection.Clear();
								textAreas.Add(textArea);
							}
						}
					}
					if (textArea != null) {
						string transformedPattern = result.TransformReplacePattern(SearchOptions.ReplacePattern);
						find.Replace(result.Offset, result.Length, transformedPattern);
						if (find.CurrentDocumentInformation.Document == null) {
							textArea.Document.Replace(result.Offset, result.Length, transformedPattern);
						}
					} else {
						count--;
					}
				}
			}
		}
		
		public static void ReplaceAll(int offset, int length)
		{
			SetSearchOptions();
			find.Reset();
			
			if (!find.SearchStrategy.CompilePattern())
				return;
			
			for (int count = 0;; count++) {
				SearchResult result = find.FindNext(offset, length);
				if (result == null) {
					ShowReplaceDoneMessage(count);
					return;
				}
				
				string replacement = result.TransformReplacePattern(SearchOptions.ReplacePattern);
				find.Replace(result.Offset,
				             result.Length,
				             replacement);
				length -= result.Length - replacement.Length;
				
				// HACK - Move the cursor to the correct offset - the caret gets
				// moved before the replace range if we replace a string with a
				// single character. The ProvidedDocInfo.Replace method assumes that
				// the current offset is at the end of the found text which it is not.
				find.CurrentDocumentInformation.CurrentOffset = result.Offset + replacement.Length - 1;
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
			
			TextEditorControl textArea = null;
			while (textArea == null) {
				SearchResult result = find.FindNext();
				if (result == null) {
					ShowNotFoundMessage();
					find.Reset();
					lastResult = null;
					return;
				} else {
					textArea = OpenTextArea(result.FileName);
					if (textArea != null) {
						if (lastResult != null  && lastResult.FileName == result.FileName &&
						    textArea.ActiveTextAreaControl.Caret.Offset != lastResult.Offset + lastResult.Length) {
							find.Reset();
						}
						int startPos = Math.Min(textArea.Document.TextLength, Math.Max(0, result.Offset));
						int endPos   = Math.Min(textArea.Document.TextLength, startPos + result.Length);
						
						SearchReplaceUtilities.SelectText(textArea, startPos, endPos);
						lastResult = result;
					}
				}
			}
		}
		
		static bool foundAtLeastOneItem = false;

		public static void FindFirstInSelection(int offset, int length)
		{
			foundAtLeastOneItem = false;
			textSelection = null;
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
			
			textSelection = new TextSelection(offset, length);
			FindNextInSelection();
		}

		public static bool FindNextInSelection()
		{
			TextEditorControl textArea = null;
			while (textArea == null) {
				SearchResult result = find.FindNext(textSelection.Offset, textSelection.Length);
				if (result == null) {
					if (!foundAtLeastOneItem) {
						ShowNotFoundMessage();
					}
					find.Reset();
					lastResult = null;
					foundAtLeastOneItem = false;
					return false;
				} else {
					textArea = OpenTextArea(result.FileName);
					if (textArea != null) {
						foundAtLeastOneItem = true;
						if (lastResult != null  && lastResult.FileName == result.FileName &&
						    textArea.ActiveTextAreaControl.Caret.Offset != lastResult.Offset + lastResult.Length) {
						}
						int startPos = Math.Min(textArea.Document.TextLength, Math.Max(0, result.Offset));
						int endPos   = Math.Min(textArea.Document.TextLength, startPos + result.Length);
						SearchReplaceUtilities.SelectText(textArea, startPos, endPos);
						lastResult = result;
					}
				}
			}
			return true;
		}
		
		static void ShowNotFoundMessage()
		{
			MessageBox.Show((Form)WorkbenchSingleton.Workbench,
			                ResourceService.GetString("Dialog.NewProject.SearchReplace.SearchStringNotFound"),
			                ResourceService.GetString("Dialog.NewProject.SearchReplace.SearchStringNotFound.Title"),
			                MessageBoxButtons.OK,
			                MessageBoxIcon.Information);
		}
		
		static TextEditorControl OpenTextArea(string fileName)
		{
			ITextEditorControlProvider textEditorProvider = null;
			if (fileName != null) {
				textEditorProvider = FileService.OpenFile(fileName).ViewContent as ITextEditorControlProvider;
			} else {
				textEditorProvider = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent as ITextEditorControlProvider;
			}
			
			if (textEditorProvider != null) {
				return textEditorProvider.TextEditorControl;
			}
			return null;
		}
		
		static void ClearSelection()
		{
			if (WorkbenchSingleton.Workbench.ActiveWorkbenchWindow != null) {
				ITextEditorControlProvider provider = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent as ITextEditorControlProvider;
				if (provider != null) {
					provider.TextEditorControl.ActiveTextAreaControl.TextArea.SelectionManager.ClearSelection();
				}
			}
		}
	}
}
