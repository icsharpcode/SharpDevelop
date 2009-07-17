// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

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
			if (lastResult != null && WorkbenchSingleton.Workbench.ActiveWorkbenchWindow != null) {
				ITextEditorControlProvider provider = WorkbenchSingleton.Workbench.ActiveViewContent as ITextEditorControlProvider;
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
			if (lastResult != null && WorkbenchSingleton.Workbench.ActiveWorkbenchWindow != null) {
				ITextEditorControlProvider provider = WorkbenchSingleton.Workbench.ActiveViewContent as ITextEditorControlProvider;
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
			return FindNextInSelection(monitor);
		}
		
		public static void MarkAll(IProgressMonitor monitor)
		{
			SetSearchOptions(monitor);
			ClearSelection();
			find.Reset();
			if (!find.SearchStrategy.CompilePattern(monitor))
				return;
			List<TextEditorControl> textAreas = new List<TextEditorControl>();
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
			foreach (TextEditorControl ctl in textAreas) {
				ctl.Refresh();
			}
			ShowMarkDoneMessage(count, monitor);
		}
		
		public static void MarkAll(int offset, int length, IProgressMonitor monitor)
		{
			SetSearchOptions(monitor);
			find.Reset();
			
			if (!find.SearchStrategy.CompilePattern(monitor))
				return;
			
			List<TextEditorControl> textAreas = new List<TextEditorControl>();
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
			foreach (TextEditorControl ctl in textAreas) {
				ctl.Refresh();
			}
			ShowMarkDoneMessage(count, monitor);
		}
		
		static void MarkResult(List<TextEditorControl> textAreas, SearchResultMatch result)
		{
			TextEditorControl textArea = OpenTextArea(result.FileName);
			if (textArea != null) {
				if (!textAreas.Contains(textArea)) {
					textAreas.Add(textArea);
				}
				textArea.ActiveTextAreaControl.Caret.Position = textArea.Document.OffsetToPosition(result.Offset);
				LineSegment segment = textArea.Document.GetLineSegmentForOffset(result.Offset);
				
				int lineNr = segment.LineNumber;
				if (!textArea.Document.BookmarkManager.IsMarked(lineNr)) {
					textArea.Document.BookmarkManager.ToggleMarkAt(new TextLocation(result.Offset - segment.Offset, lineNr));
				}
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
			
			List<TextEditorControl> textAreas = new List<TextEditorControl>();
			TextEditorControl textArea = null;
			for (int count = 0;; count++) {
				SearchResultMatch result = SearchReplaceManager.find.FindNext(monitor);
				
				if (result == null) {
					if (count != 0) {
						foreach (TextEditorControl ta in textAreas) {
							ta.EndUpdate();
							ta.Refresh();
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
			
			TextEditorControl textArea = null;
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
			TextEditorControl textArea = null;
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
		
		static void ShowNotFoundMessage(IProgressMonitor monitor)
		{
			if (monitor != null && monitor.IsCancelled)
				return;
			if (monitor != null) monitor.ShowingDialog = true;
			MessageBox.Show(WorkbenchSingleton.MainForm,
			                ResourceService.GetString("Dialog.NewProject.SearchReplace.SearchStringNotFound"),
			                ResourceService.GetString("Dialog.NewProject.SearchReplace.SearchStringNotFound.Title"),
			                MessageBoxButtons.OK,
			                MessageBoxIcon.Information);
			if (monitor != null) monitor.ShowingDialog = false;
		}
		
		static TextEditorControl OpenTextArea(string fileName)
		{
			ITextEditorControlProvider textEditorProvider = null;
			if (fileName != null) {
				textEditorProvider = FileService.OpenFile(fileName) as ITextEditorControlProvider;
			} else {
				textEditorProvider = WorkbenchSingleton.Workbench.ActiveViewContent as ITextEditorControlProvider;
			}
			
			if (textEditorProvider != null) {
				return textEditorProvider.TextEditorControl;
			}
			return null;
		}
		
		static void ClearSelection()
		{
			if (WorkbenchSingleton.Workbench.ActiveWorkbenchWindow != null) {
				ITextEditorControlProvider provider = WorkbenchSingleton.Workbench.ActiveViewContent as ITextEditorControlProvider;
				if (provider != null) {
					provider.TextEditorControl.ActiveTextAreaControl.TextArea.SelectionManager.ClearSelection();
				}
			}
		}
	}
}
