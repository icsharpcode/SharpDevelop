// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Bookmarks;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.Search;
using ICSharpCode.SharpDevelop.Gui;

namespace SearchAndReplace
{
	public class SearchReplaceManager
	{
		public static SearchAndReplaceDialog SearchAndReplaceDialog = null;
		
		static Search find = new Search();
		
		static SearchReplaceManager()
		{
			find.TextIteratorBuilder = new ForwardTextIteratorBuilder();
		}
		
		static void SetSearchOptions(IProgressMonitor monitor)
		{
			find.SearchStrategy   = SearchReplaceUtilities.CreateSearchStrategy(SearchOptions.SearchStrategyType);
			find.DocumentIterator = SearchReplaceUtilities.CreateDocumentIterator(SearchOptions.DocumentIteratorType, monitor);
		}
		
		public static void Replace(IProgressMonitor monitor)
		{
			SetSearchOptions(monitor);
			if (lastResult != null) {
				ITextEditorProvider provider = WorkbenchSingleton.Workbench.ActiveViewContent as ITextEditorProvider;
				if (provider != null) {
					ITextEditor textarea = provider.TextEditor;
					
					if (textarea.SelectionStart == lastResult.Offset
					    && textarea.SelectionLength == lastResult.Length
					    && lastResult.FileName == textarea.FileName)
					{
						string replacePattern = lastResult.TransformReplacePattern(SearchOptions.ReplacePattern);
						
						using (textarea.Document.OpenUndoGroup()) {
							textarea.Document.Replace(lastResult.Offset, lastResult.Length, replacePattern);
							textarea.Select(lastResult.Offset + replacePattern.Length, 0); // clear selection and set caret position
						}
					}
				}
			}
			FindNext(monitor);
		}
		
		static TextSelection textSelection;
		
		public static void ReplaceFirstInSelection(int offset, int length, IProgressMonitor monitor)
		{
			SetSearchOptions(monitor);
			FindFirstInSelection(offset, length, monitor);
		}
		
		public static bool ReplaceNextInSelection(IProgressMonitor monitor)
		{
			if (lastResult != null) {
				ITextEditorProvider provider = WorkbenchSingleton.Workbench.ActiveViewContent as ITextEditorProvider;
				if (provider != null) {
					ITextEditor textarea = provider.TextEditor;
					
					if (textarea.SelectionStart == lastResult.Offset
					    && textarea.SelectionLength == lastResult.Length
					    && lastResult.FileName == textarea.FileName)
					{
						string replacePattern = lastResult.TransformReplacePattern(SearchOptions.ReplacePattern);
						
						using (textarea.Document.OpenUndoGroup()) {
							textarea.Document.Replace(lastResult.Offset, lastResult.Length, replacePattern);
							textarea.Select(lastResult.Offset + replacePattern.Length, 0); // clear selection and set caret position
						}
						
						textSelection.Length -= lastResult.Length - replacePattern.Length;
					}
				}
			}
			return FindNextInSelection(monitor);
		}
		
		public static void MarkAll(IProgressMonitor monitor)
		{
			SetSearchOptions(monitor);
			ClearSelection();
			find.Reset();
			if (!find.SearchStrategy.CompilePattern(monitor))
				return;
			List<ITextEditor> textAreas = new List<ITextEditor>();
			int count;
			for (count = 0;; count++) {
				SearchResultMatch result = SearchReplaceManager.find.FindNext(monitor);
				
				if (result == null) {
					break;
				} else {
					MarkResult(textAreas, result);
				}
			}
			find.Reset();
			ShowMarkDoneMessage(count, monitor);
		}
		
		public static void MarkAll(int offset, int length, IProgressMonitor monitor)
		{
			SetSearchOptions(monitor);
			find.Reset();
			
			if (!find.SearchStrategy.CompilePattern(monitor))
				return;
			
			List<ITextEditor> textAreas = new List<ITextEditor>();
			int count;
			for (count = 0;; count++) {
				SearchResultMatch result = find.FindNext(offset, length);
				if (result == null) {
					break;
				} else {
					MarkResult(textAreas, result);
				}
			}
			find.Reset();
			ShowMarkDoneMessage(count, monitor);
		}
		
		static void MarkResult(List<ITextEditor> textAreas, SearchResultMatch result)
		{
			ITextEditor textArea = OpenTextArea(result.FileName);
			if (textArea != null) {
				if (!textAreas.Contains(textArea)) {
					textAreas.Add(textArea);
				}
				textArea.Caret.Offset = result.Offset;
				IDocumentLine segment = textArea.Document.GetLineForOffset(result.Offset);
				
				int lineNr = segment.LineNumber;
				
				foreach (var bookmark in BookmarkManager.GetBookmarks(result.FileName)) {
					if (bookmark.CanToggle && bookmark.LineNumber == lineNr) {
						// bookmark or breakpoint already exists at that line
						return;
					}
				}
				BookmarkManager.AddMark(new Bookmark(result.FileName, textArea.Document.OffsetToPosition(result.Offset)));
			}
		}
		
		static void ShowMarkDoneMessage(int count, IProgressMonitor monitor)
		{
			if (count == 0) {
				ShowNotFoundMessage(monitor);
			} else {
				if (monitor != null) monitor.ShowingDialog = true;
				MessageService.ShowMessage(StringParser.Parse("${res:ICSharpCode.TextEditor.Document.SearchReplaceManager.MarkAllDone}", new string[,]{{ "Count", count.ToString() }}),
				                           "${res:Global.FinishedCaptionText}");
				if (monitor != null) monitor.ShowingDialog = false;
			}
		}
		
		static void ShowReplaceDoneMessage(int count, IProgressMonitor monitor)
		{
			if (count == 0) {
				ShowNotFoundMessage(monitor);
			} else {
				if (monitor != null) monitor.ShowingDialog = true;
				MessageService.ShowMessage(StringParser.Parse("${res:ICSharpCode.TextEditor.Document.SearchReplaceManager.ReplaceAllDone}", new string[,]{{ "Count", count.ToString() }}),
				                           "${res:Global.FinishedCaptionText}");
				if (monitor != null) monitor.ShowingDialog = false;
			}
		}
		
		public static void ReplaceAll(IProgressMonitor monitor)
		{
			SetSearchOptions(monitor);
			ClearSelection();
			find.Reset();
			if (!find.SearchStrategy.CompilePattern(monitor))
				return;
			
			List<ITextEditor> textAreas = new List<ITextEditor>();
			ITextEditor textArea = null;
			for (int count = 0;; count++) {
				SearchResultMatch result = SearchReplaceManager.find.FindNext(monitor);
				
				if (result == null) {
					if (count != 0) {
						foreach (ITextEditor ta in textAreas) {
							ta.Document.EndUndoableAction();
						}
					}
					ShowReplaceDoneMessage(count, monitor);
					find.Reset();
					return;
				} else {
					if (textArea == null || textArea.FileName != result.FileName) {
						// we need to open another text area
						textArea = OpenTextArea(result.FileName);
						if (textArea != null) {
							if (!textAreas.Contains(textArea)) {
								textArea.Document.StartUndoableAction();
								textArea.Select(textArea.SelectionStart, 0);
								textAreas.Add(textArea);
							}
						}
					}
					if (textArea != null) {
						string transformedPattern = result.TransformReplacePattern(SearchOptions.ReplacePattern);
						find.Replace(result.Offset, result.Length, transformedPattern);
						if (find.CurrentDocumentInformation.IsDocumentCreatedFromTextBuffer) {
							textArea.Document.Replace(result.Offset, result.Length, transformedPattern);
						}
					} else {
						count--;
					}
				}
			}
		}
		
		public static void ReplaceAll(int offset, int length, IProgressMonitor monitor)
		{
			SetSearchOptions(monitor);
			find.Reset();
			
			if (!find.SearchStrategy.CompilePattern(monitor))
				return;
			
			for (int count = 0;; count++) {
				SearchResultMatch result = find.FindNext(offset, length);
				if (result == null) {
					ShowReplaceDoneMessage(count, monitor);
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
		
		static SearchResultMatch lastResult = null;
		
		public static void FindNext(IProgressMonitor monitor)
		{
			SetSearchOptions(monitor);
			if (find == null ||
			    SearchOptions.FindPattern == null ||
			    SearchOptions.FindPattern.Length == 0) {
				return;
			}
			
			if (!find.SearchStrategy.CompilePattern(monitor)) {
				find.Reset();
				lastResult = null;
				return;
			}
			
			ITextEditor textArea = null;
			while (textArea == null) {
				SearchResultMatch result = find.FindNext(monitor);
				if (result == null) {
					ShowNotFoundMessage(monitor);
					find.Reset();
					lastResult = null;
					return;
				} else {
					textArea = OpenTextArea(result.FileName);
					if (textArea != null) {
						if (lastResult != null  && lastResult.FileName == result.FileName &&
						    textArea.Caret.Offset != lastResult.Offset + lastResult.Length) {
							find.Reset();
						}
						int startPos = Math.Min(textArea.Document.TextLength, Math.Max(0, result.Offset));
						int endPos   = Math.Min(textArea.Document.TextLength, startPos + result.Length);
						
						textArea.Select(startPos, endPos - startPos);
						lastResult = result;
					}
				}
			}
		}
		
		static bool foundAtLeastOneItem = false;

		public static void FindFirstInSelection(int offset, int length, IProgressMonitor monitor)
		{
			foundAtLeastOneItem = false;
			textSelection = null;
			SetSearchOptions(monitor);
			
			if (find == null ||
			    SearchOptions.FindPattern == null ||
			    SearchOptions.FindPattern.Length == 0) {
				return;
			}
			
			if (!find.SearchStrategy.CompilePattern(monitor)) {
				find.Reset();
				lastResult = null;
				return;
			}
			
			textSelection = new TextSelection(offset, length);
			FindNextInSelection(monitor);
		}

		public static bool FindNextInSelection(IProgressMonitor monitor)
		{
			ITextEditor textArea = null;
			while (textArea == null) {
				SearchResultMatch result = find.FindNext(textSelection.Offset, textSelection.Length);
				if (result == null) {
					if (!foundAtLeastOneItem) {
						ShowNotFoundMessage(monitor);
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
						    textArea.Caret.Offset != lastResult.Offset + lastResult.Length) {
						}
						int startPos = Math.Min(textArea.Document.TextLength, Math.Max(0, result.Offset));
						int endPos   = Math.Min(textArea.Document.TextLength, startPos + result.Length);
						textArea.Select(startPos, endPos - startPos);
						lastResult = result;
					}
				}
			}
			return true;
		}
		
		static void ShowNotFoundMessage(IProgressMonitor monitor)
		{
			if (monitor != null && monitor.CancellationToken.IsCancellationRequested)
				return;
			if (monitor != null) monitor.ShowingDialog = true;
			MessageBox.Show(WorkbenchSingleton.MainWin32Window,
			                ResourceService.GetString("Dialog.NewProject.SearchReplace.SearchStringNotFound"),
			                ResourceService.GetString("Dialog.NewProject.SearchReplace.SearchStringNotFound.Title"),
			                MessageBoxButtons.OK,
			                MessageBoxIcon.Information);
			if (monitor != null) monitor.ShowingDialog = false;
		}
		
		static ITextEditor OpenTextArea(string fileName)
		{
			ITextEditorProvider textEditorProvider;
			if (fileName != null) {
				textEditorProvider = FileService.OpenFile(fileName) as ITextEditorProvider;
			} else {
				textEditorProvider = WorkbenchSingleton.Workbench.ActiveViewContent as ITextEditorProvider;
			}
			
			if (textEditorProvider != null) {
				return textEditorProvider.TextEditor;
			}
			return null;
		}
		
		static void ClearSelection()
		{
			ITextEditorProvider provider = WorkbenchSingleton.Workbench.ActiveViewContent as ITextEditorProvider;
			if (provider != null) {
				ITextEditor editor = provider.TextEditor;
				if (editor.SelectionLength > 0)
					editor.Select(editor.Caret.Offset, 0);
			}
		}
	}
}
