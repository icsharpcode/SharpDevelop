/*
 * Created by SharpDevelop.
 * User: Siegfried
 * Date: 06.10.2011
 * Time: 21:33
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Search;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.Search;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace SearchAndReplace
{
	/// <summary>
	/// Description of SearchManager.
	/// </summary>
	public class SearchManager
	{
		static IEnumerable<FileName> GenerateFileList(SearchTarget target, string baseDirectory = null, string filter = "*.*", bool searchSubdirs = false, CancellationToken ct = default(CancellationToken))
		{
			List<FileName> files = new List<FileName>();
			
			switch (target) {
				case SearchTarget.CurrentDocument:
				case SearchTarget.CurrentSelection:
					ITextEditorProvider vc = WorkbenchSingleton.Workbench.ActiveViewContent as ITextEditorProvider;
					if (vc != null)
						files.Add(vc.TextEditor.FileName);
					break;
				case SearchTarget.AllOpenFiles:
					foreach (ITextEditorProvider editor in WorkbenchSingleton.Workbench.ViewContentCollection.OfType<ITextEditorProvider>()) {
						if (ct != default(CancellationToken))
							ct.ThrowIfCancellationRequested();
						files.Add(editor.TextEditor.FileName);
					}
					break;
				case SearchTarget.WholeProject:
					if (ProjectService.CurrentProject == null)
						break;
					foreach (FileProjectItem item in ProjectService.CurrentProject.Items.OfType<FileProjectItem>()) {
						if (ct != default(CancellationToken))
							ct.ThrowIfCancellationRequested();
						files.Add(new FileName(item.FileName));
					}
					break;
				case SearchTarget.WholeSolution:
					if (ProjectService.OpenSolution == null)
						break;
					foreach (var item in ProjectService.OpenSolution.SolutionFolderContainers.Select(f => f.SolutionItems).SelectMany(si => si.Items)) {
						if (ct != default(CancellationToken))
							ct.ThrowIfCancellationRequested();
						files.Add(new FileName(Path.Combine(ProjectService.OpenSolution.Directory, item.Location)));
					}
					foreach (var item in ProjectService.OpenSolution.Projects.SelectMany(p => p.Items).OfType<FileProjectItem>()) {
						if (ct != default(CancellationToken))
							ct.ThrowIfCancellationRequested();
						files.Add(new FileName(item.FileName));
					}
					break;
				case SearchTarget.Directory:
					if (!Directory.Exists(baseDirectory))
						break;
					return EnumerateDirectories(baseDirectory, filter, searchSubdirs, ct);
					break;
				default:
					throw new Exception("Invalid value for FileListType");
			}
			
			return files.Distinct();
		}

		static IEnumerable<FileName> EnumerateDirectories(string baseDirectory, string filter, bool searchSubdirs)
		{
			foreach (var name in FileUtility.SearchDirectory(baseDirectory, filter, searchSubdirs)) {
				yield return name;
			}
		}
		
		public static void ShowSearchResults(string pattern, IObservable<SearchResultMatch> results)
		{
			string title = StringParser.Parse("${res:MainWindow.Windows.SearchResultPanel.OccurrencesOf}",
			                                  new StringTagPair("Pattern", pattern));
			SearchResultsPad.Instance.ShowSearchResults(title, results);
			SearchResultsPad.Instance.BringToFront();
		}
		
		
		public static IObservable<SearchResultMatch> FindAll(string pattern, bool ignoreCase, bool matchWholeWords, SearchMode mode,
		                                                     SearchTarget target, string baseDirectory = null, string filter = "*.*", bool searchSubdirs = false)
		{
			CancellationTokenSource cts = new CancellationTokenSource();
			var monitor = WorkbenchSingleton.Workbench.StatusBar.CreateProgressMonitor(cts.Token);
			monitor.TaskName = "Find all occurrences of '" + pattern + "' in " + GetTargetDescription(target, baseDirectory);
			monitor.Status = OperationStatus.Normal;
			var strategy = SearchStrategyFactory.Create(pattern, ignoreCase, matchWholeWords, mode);
			ParseableFileContentFinder fileFinder = new ParseableFileContentFinder();
			IEnumerable<FileName> fileList;
			using (IProgressMonitor dialog = AsynchronousWaitDialog.ShowWaitDialog("Prepare search ...", true)) {
				dialog.Progress = double.NaN;
				fileList = GenerateFileList(target, baseDirectory, filter, searchSubdirs, dialog.CancellationToken);
			}
			return new SearchRun(strategy, fileFinder, fileList, monitor, cts);
		}
		
		static string GetTargetDescription(SearchTarget target, string baseDirectory = null)
		{
			switch (target) {
				case SearchTarget.CurrentDocument:
					return "the current document";
				case SearchTarget.CurrentSelection:
					return "the current selection";
				case SearchTarget.AllOpenFiles:
					return "all open files";
				case SearchTarget.WholeProject:
					return "the whole project";
				case SearchTarget.WholeSolution:
					return "the whole solution";
				case SearchTarget.Directory:
					return "the directory '" + baseDirectory + "'";
				default:
					throw new Exception("Invalid value for SearchTarget");
			}
		}
		
		class SearchRun : IObservable<SearchResultMatch>, IDisposable
		{
			IObserver<SearchResultMatch> observer;
			ISearchStrategy strategy;
			ParseableFileContentFinder fileFinder;
			IEnumerable<FileName> fileList;
			IProgressMonitor monitor;
			CancellationTokenSource cts;
			
			public SearchRun(ISearchStrategy strategy, ParseableFileContentFinder fileFinder, IEnumerable<FileName> fileList, IProgressMonitor monitor, CancellationTokenSource cts)
			{
				this.strategy = strategy;
				this.fileFinder = fileFinder;
				this.fileList = fileList;
				this.monitor = monitor;
				this.cts = cts;
			}
			
			public IDisposable Subscribe(IObserver<SearchResultMatch> observer)
			{
				this.observer = observer;
				var task = new System.Threading.Tasks.Task(delegate { Parallel.ForEach(fileList, new ParallelOptions { CancellationToken = monitor.CancellationToken, MaxDegreeOfParallelism = Environment.ProcessorCount }, fileName => SearchFile(fileName, strategy, monitor.CancellationToken)); });
				task.ContinueWith(t => { if (t.Exception != null) observer.OnError(t.Exception); else observer.OnCompleted(); this.Dispose(); });
				task.Start();
				return this;
			}
			
			public void Dispose()
			{
				if (!cts.IsCancellationRequested)
					cts.Cancel();
				monitor.Dispose();
			}
			
			void SearchFile(FileName fileName, ISearchStrategy strategy, CancellationToken ct)
			{
				ITextBuffer buffer = fileFinder.Create(fileName);
				if (buffer == null)
					return;
				buffer = buffer.CreateSnapshot();
				
				if (!MimeTypeDetection.FindMimeType(buffer).StartsWith("text/"))
					return;
				var source = DocumentUtilitites.GetTextSource(buffer);
				foreach(var result in strategy.FindAll(source)) {
					ct.ThrowIfCancellationRequested();
					lock (observer)
						observer.OnNext(new SearchResultMatch(new ProvidedDocumentInformation(buffer, fileName, 0), result.Offset, result.Length));
				}
				lock (monitor)
					monitor.Progress += 1.0 / fileList.Count();
			}
		}
	}
}
