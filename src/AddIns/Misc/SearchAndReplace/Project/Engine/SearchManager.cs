// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Search;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Bookmarks;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.AvalonEdit;
using ICSharpCode.SharpDevelop.Editor.Search;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace SearchAndReplace
{
	/// <summary>
	/// Provides all search actions: find next, parallel and sequential find all, mark all, mark result, interactive replace and replace all.
	/// </summary>
	public class SearchManager
	{
		#region FindAll
		public static IObservable<SearchedFile> FindAllParallel(ISearchStrategy strategy, SearchLocation location, IProgressMonitor progressMonitor)
		{
			currentSearchRegion = null;
			SearchableFileContentFinder fileFinder = new SearchableFileContentFinder();
			return new SearchRun(strategy, fileFinder, location.GenerateFileList(), progressMonitor) { Target = location.Target, Selection = location.Selection };
		}
		
		public static IEnumerable<SearchedFile> FindAll(ISearchStrategy strategy, SearchLocation location, IProgressMonitor progressMonitor)
		{
			currentSearchRegion = null;
			SearchableFileContentFinder fileFinder = new SearchableFileContentFinder();
			return new SearchRun(strategy, fileFinder, location.GenerateFileList(), progressMonitor) { Target = location.Target, Selection = location.Selection }.GetResults();
		}
		
		class SearchableFileContentFinder
		{
			FileName[] viewContentFileNamesCollection = WorkbenchSingleton.SafeThreadFunction(() => FileService.OpenedFiles.Select(f => f.FileName).ToArray());
			
			static ITextBuffer ReadFile(FileName fileName)
			{
				OpenedFile openedFile = FileService.GetOpenedFile(fileName);
				if (openedFile == null)
					return null;
				IFileDocumentProvider provider = FileService.GetOpenFile(fileName) as IFileDocumentProvider;
				if (provider == null)
					return null;
				IDocument doc = provider.GetDocumentForFile(openedFile);
				if (doc == null)
					return null;
				return doc.CreateSnapshot();
			}
			
			public ITextBuffer Create(FileName fileName)
			{
				try {
					foreach (FileName name in viewContentFileNamesCollection) {
						if (FileUtility.IsEqualFileName(name, fileName)) {
							ITextBuffer buffer = WorkbenchSingleton.SafeThreadFunction(ReadFile, fileName);
							if (buffer != null)
								return buffer;
						}
					}
					using (Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read)) {
						if (MimeTypeDetection.FindMimeType(stream).StartsWith("text/")) {
							stream.Position = 0;
							return new StringTextBuffer(ICSharpCode.AvalonEdit.Utils.FileReader.ReadFileContent(stream, Encoding.Default));
						}
					}
					return null;
				} catch (IOException) {
					return null;
				} catch (UnauthorizedAccessException) {
					return null;
				}
			}
		}
		
		class SearchRun : IObservable<SearchedFile>, IDisposable
		{
			ISearchStrategy strategy;
			SearchableFileContentFinder fileFinder;
			IEnumerable<FileName> fileList;
			IProgressMonitor monitor;
			CancellationTokenSource cts;
			
			public SearchTarget Target { get; set; }
			
			public ISegment Selection { get; set; }
			
			public SearchRun(ISearchStrategy strategy, SearchableFileContentFinder fileFinder, IEnumerable<FileName> fileList, IProgressMonitor monitor)
			{
				this.strategy = strategy;
				this.fileFinder = fileFinder;
				this.fileList = fileList;
				this.monitor = monitor;
				this.cts = new CancellationTokenSource();
			}
			
			public IDisposable Subscribe(IObserver<SearchedFile> observer)
			{
				LoggingService.Debug("Parallel FindAll starting");
				var task = new System.Threading.Tasks.Task(
					delegate {
						var list = fileList.ToList();
						ThrowIfCancellationRequested();
						SearchParallel(list, observer);
					}, TaskCreationOptions.LongRunning);
				task.ContinueWith(
					t => {
						LoggingService.Debug("Parallel FindAll finished " + (t.IsFaulted ? "with error" : "successfully"));
						if (t.Exception != null)
							observer.OnError(t.Exception);
						else
							observer.OnCompleted();
						this.Dispose();
					});
				task.Start();
				return this;
			}
			
			public IEnumerable<SearchedFile> GetResults()
			{
				var list = fileList.ToList();
				foreach (var file in list) {
					ThrowIfCancellationRequested();
					var results = SearchFile(file);
					if (results != null)
						yield return results;
					monitor.Progress += 1.0 / list.Count;
				}
			}
			
			void SearchParallel(List<FileName> files, IObserver<SearchedFile> observer)
			{
				int taskCount = 2 * Environment.ProcessorCount;
				Queue<Task<SearchedFile>> queue = new Queue<Task<SearchedFile>>(taskCount);
				List<Exception> exceptions = new List<Exception>();
				for (int i = 0; i < files.Count; i++) {
					if (i >= taskCount) {
						HandleResult(queue.Dequeue(), observer, exceptions, files);
					}
					if (exceptions.Count > 0) break;
					FileName file = files[i];
					queue.Enqueue(System.Threading.Tasks.Task.Factory.StartNew(() => SearchFile(file)));
				}
				while (queue.Count > 0) {
					HandleResult(queue.Dequeue(), observer, exceptions, files);
				}
				if (exceptions.Count != 0)
					throw new AggregateException(exceptions);
			}

			void HandleResult(Task<SearchedFile> task, IObserver<SearchedFile> observer, List<Exception> exceptions, List<FileName> files)
			{
				SearchedFile result;
				try {
					result = task.Result;
				} catch (AggregateException ex) {
					exceptions.AddRange(ex.InnerExceptions);
					return;
				}
				if (exceptions.Count == 0 && result != null)
					observer.OnNext(result);
				monitor.Progress += 1.0 / files.Count;
			}
			
			void ThrowIfCancellationRequested()
			{
				monitor.CancellationToken.ThrowIfCancellationRequested();
				cts.Token.ThrowIfCancellationRequested();
			}
			
			public void Dispose()
			{
				cts.Cancel();
				monitor.Dispose();
			}
			
			SearchedFile SearchFile(FileName fileName)
			{
				ITextBuffer buffer = fileFinder.Create(fileName);
				if (buffer == null)
					return null;
				
				ThrowIfCancellationRequested();
				
				var source = DocumentUtilitites.GetTextSource(buffer);
				TextDocument document = null;
				ISyntaxHighlighter highlighter = null;
				int offset = 0;
				int length = source.TextLength;
				if (Target == SearchTarget.CurrentSelection && Selection != null) {
					offset = Selection.Offset;
					length = Selection.Length;
				}
				List<SearchResultMatch> results = new List<SearchResultMatch>();
				foreach (var result in strategy.FindAll(source, offset, length)) {
					ThrowIfCancellationRequested();
					if (document == null) {
						document = new TextDocument(source);
						var highlighting = HighlightingManager.Instance.GetDefinitionByExtension(Path.GetExtension(fileName));
						highlighter = EditorControlService.Instance.CreateHighlighter(new AvalonEditDocumentAdapter(document, null), fileName);
					}
					var start = document.GetLocation(result.Offset).ToLocation();
					var end = document.GetLocation(result.Offset + result.Length).ToLocation();
					var builder = SearchResultsPad.CreateInlineBuilder(start, end, document, highlighter);
					results.Add(new AvalonEditSearchResultMatch(fileName, start, end, result.Offset, result.Length, builder, highlighter, result));
				}
				if (results.Count > 0)
					return new SearchedFile(fileName, results);
				else
					return null;
			}
		}
		#endregion
		
		#region FindNext
		static SearchRegion currentSearchRegion;
		
		public static SearchResultMatch FindNext(ISearchStrategy strategy, SearchLocation location)
		{
			var files = location.GenerateFileList().ToArray();
			if (files.Length == 0)
				return null;
			if (currentSearchRegion == null || !currentSearchRegion.IsSameState(files, strategy, location))
				currentSearchRegion = SearchRegion.CreateSearchRegion(files, strategy, location);
			if (currentSearchRegion == null)
				return null;
			var result = currentSearchRegion.FindNext();
			if (result == null)
				currentSearchRegion = null;
			return result;
		}
		
		class SearchRegion
		{
			FileName[] files;
			SearchLocation location;
			ISearchStrategy strategy;
			
			public SearchResultMatch FindNext()
			{
				// Setup search inside current or first file.
				SearchableFileContentFinder finder = new SearchableFileContentFinder();
				int index = GetCurrentFileIndex();
				int i = 0;
				int searchOffset = 0;

				SearchResultMatch result = null;
				
				if (index > -1) {
					ITextEditor editor = GetActiveTextEditor();
					if (editor.SelectionLength > 0)
						searchOffset = editor.SelectionStart + editor.SelectionLength;
					else
						searchOffset = editor.Caret.Offset + 1;
					var document = editor.Document;
					
					int length;
					// if (target == SearchTarget.CurrentSelection) selection will not be null
					// hence use the selection as search region.
					if (location.Selection != null) {
						searchOffset = Math.Max(location.Selection.Offset, searchOffset);
						length = location.Selection.EndOffset - searchOffset;
					} else {
						length = document.TextLength - searchOffset;
					}
					
					// try to find a result
					if (length > 0 && (searchOffset + length) <= document.TextLength)
						result = Find(editor.FileName, document, searchOffset, length);
				}
				
				// try the other files until we find something, or have processed all of them
				while (result == null && i < files.Length) {
					index = (index + 1) % files.Length;
					
					FileName file = files[index];
					ITextBuffer buffer = finder.Create(file);
					
					if (buffer == null)
						continue;
					int length;
					if (location.Selection != null) {
						searchOffset = location.Selection.Offset;
						length = location.Selection.Length;
					} else {
						searchOffset = 0;
						length = buffer.TextLength;
					}
					
					// try to find a result
					result = Find(file, DocumentUtilitites.LoadReadOnlyDocumentFromBuffer(buffer), searchOffset, length);
					
					i++;
				}
				
				return result;
			}

			SearchResultMatch Find(FileName file, IDocument document, int searchOffset, int length)
			{
				var result = strategy.FindNext(DocumentUtilitites.GetTextSource(document), searchOffset, length);
				if (result != null) {
					var start = document.OffsetToPosition(result.Offset);
					var end = document.OffsetToPosition(result.EndOffset);
					return new AvalonEditSearchResultMatch(file, start, end, result.Offset, result.Length, null, null, result);
				}
				return null;
			}
			
			int GetCurrentFileIndex()
			{
				var editor = GetActiveTextEditor();
				if (editor == null)
					return -1;
				return files.FindIndex(file => editor.FileName.Equals(file));
			}
			
			public static SearchRegion CreateSearchRegion(FileName[] files, ISearchStrategy strategy, SearchLocation location)
			{
				return new SearchRegion(files, strategy, location);
			}
			
			SearchRegion(FileName[] files, ISearchStrategy strategy, SearchLocation location)
			{
				if (files == null)
					throw new ArgumentNullException("files");
				this.files = files;
				this.strategy = strategy;
				this.location = location;
			}
			
			public bool IsSameState(FileName[] files, ISearchStrategy strategy, SearchLocation location)
			{
				return this.files.SequenceEqual(files) &&
					this.strategy.Equals(strategy) &&
					this.location.EqualsWithoutSelection(location);
			}
		}
		#endregion

		#region Mark & Replace(All)
		public static void MarkAll(IObservable<SearchedFile> results)
		{
			int count = 0;
			results.ObserveOnUIThread()
				.Subscribe(
					searchedFile => {
						count++;
						foreach (var m in searchedFile.Matches)
							MarkResult(m, false);
					},
					error => MessageService.ShowException(error),
					() => ShowMarkDoneMessage(count)
				);
		}
		
		public static int ReplaceAll(IEnumerable<SearchedFile> results, string replacement, CancellationToken ct)
		{
			int count = 0;
			foreach (var searchedFile in results) {
				ct.ThrowIfCancellationRequested();
				int difference = 0;
				ITextEditor textArea = OpenTextArea(searchedFile.FileName, false);
				if (textArea != null) {
					using (textArea.Document.OpenUndoGroup()) {
						foreach (var match in searchedFile.Matches) {
							ct.ThrowIfCancellationRequested();
							string newString = match.TransformReplacePattern(replacement);
							textArea.Document.Replace(match.StartOffset + difference, match.Length, newString);
							difference += newString.Length - match.Length;
							count++;
						}
					}
				}
			}
			
			return count;
		}
		
		public static void Replace(SearchResultMatch lastMatch, string replacement)
		{
			if (lastMatch == null)
				return;
			ITextEditor textArea = OpenTextArea(lastMatch.FileName);
			if (textArea != null)
				textArea.Document.Replace(lastMatch.StartOffset, lastMatch.Length, lastMatch.TransformReplacePattern(replacement));
		}

		static void MarkResult(SearchResultMatch result, bool switchToOpenedView = true)
		{
			ITextEditor textArea = OpenTextArea(result.FileName, switchToOpenedView);
			if (textArea != null) {
				if (switchToOpenedView)
					textArea.Caret.Position = result.StartLocation;

				foreach (var bookmark in BookmarkManager.GetBookmarks(result.FileName)) {
					if (bookmark.CanToggle && bookmark.LineNumber == result.StartLocation.Line) {
						// bookmark or breakpoint already exists at that line
						return;
					}
				}
				BookmarkManager.AddMark(new Bookmark(result.FileName, result.StartLocation));
			}
		}
		#endregion
		
		#region TextEditor helpers
		static ITextEditor OpenTextArea(string fileName, bool switchToOpenedView = true)
		{
			ITextEditorProvider textEditorProvider;
			if (fileName != null) {
				textEditorProvider = FileService.OpenFile(fileName, switchToOpenedView) as ITextEditorProvider;
			} else {
				textEditorProvider = WorkbenchSingleton.Workbench.ActiveViewContent as ITextEditorProvider;
			}

			if (textEditorProvider != null) {
				return textEditorProvider.TextEditor;
			}
			return null;
		}
		
		public static ITextEditor GetActiveTextEditor()
		{
			ITextEditorProvider provider = WorkbenchSingleton.Workbench.ActiveViewContent as ITextEditorProvider;
			if (provider != null) {
				return provider.TextEditor;
			} else {
				return null;
			}
		}
		
		public static ISegment GetActiveSelection(bool useAnchors)
		{
			ITextEditor editor = GetActiveTextEditor();
			if (editor != null) {
				if (useAnchors)
					return new AnchorSegment((TextDocument)editor.Document.GetService(typeof(TextDocument)), editor.SelectionStart, editor.SelectionLength);
				return new TextSegment { StartOffset = editor.SelectionStart, Length = editor.SelectionLength };
			}
			
			return null;
		}
		#endregion
		
		#region Show Messages
		public static void ShowReplaceDoneMessage(int count)
		{
			if (count == 0) {
				ShowNotFoundMessage();
			} else {
				MessageService.ShowMessage(
					StringParser.Parse("${res:ICSharpCode.TextEditor.Document.SearchReplaceManager.ReplaceAllDone}",
					                   new StringTagPair("Count", count.ToString())),
					"${res:Global.FinishedCaptionText}");
			}
		}
		
		static void ShowMarkDoneMessage(int count)
		{
			if (count == 0) {
				ShowNotFoundMessage();
			} else {
				MessageService.ShowMessage(
					StringParser.Parse("${res:ICSharpCode.TextEditor.Document.SearchReplaceManager.MarkAllDone}",
					                   new StringTagPair("Count", count.ToString())),
					"${res:Global.FinishedCaptionText}");
			}
		}

		static void ShowNotFoundMessage()
		{
			MessageService.ShowMessage("${res:Dialog.NewProject.SearchReplace.SearchStringNotFound}", "${res:Dialog.NewProject.SearchReplace.SearchStringNotFound.Title}");
		}
		#endregion

		#region Result display helpers
		public static void SelectResult(SearchResultMatch result)
		{
			if (result == null) {
				ShowNotFoundMessage();
				return;
			}
			var editor = OpenTextArea(result.FileName, true);
			if (editor != null) {
				var start = editor.Document.PositionToOffset(result.StartLocation.Line, result.StartLocation.Column);
				var end = editor.Document.PositionToOffset(result.EndLocation.Line, result.EndLocation.Column);
				editor.Caret.Offset = start;
				editor.Select(start, end - start);
			}
		}
		
		public static bool IsResultSelected(SearchResultMatch result)
		{
			if (result == null)
				return false;
			var editor = GetActiveTextEditor();
			if (editor == null)
				return false;
			return result.FileName.Equals(editor.FileName)
				&& result.StartOffset == editor.SelectionStart
				&& result.Length == editor.SelectionLength;
		}
		
		public static void ShowSearchResults(string pattern, IObservable<SearchedFile> results)
		{
			string title = StringParser.Parse("${res:MainWindow.Windows.SearchResultPanel.OccurrencesOf}",
			                                  new StringTagPair("Pattern", pattern));
			SearchResultsPad.Instance.ShowSearchResults(title, results);
			SearchResultsPad.Instance.BringToFront();
		}
		#endregion
	}
}
