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
		static IEnumerable<FileName> GenerateFileList(SearchTarget target, string baseDirectory = null, string filter = "*.*", bool searchSubdirs = false)
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
					foreach (ITextEditorProvider editor in WorkbenchSingleton.Workbench.ViewContentCollection.OfType<ITextEditorProvider>())
						files.Add(editor.TextEditor.FileName);
					break;
				case SearchTarget.WholeProject:
					if (ProjectService.CurrentProject == null)
						break;
					foreach (FileProjectItem item in ProjectService.CurrentProject.Items.OfType<FileProjectItem>())
						files.Add(new FileName(item.FileName));
					break;
				case SearchTarget.WholeSolution:
					if (ProjectService.OpenSolution == null)
						break;
					foreach (var item in ProjectService.OpenSolution.SolutionFolderContainers.Select(f => f.SolutionItems).SelectMany(si => si.Items))
						files.Add(new FileName(Path.Combine(ProjectService.OpenSolution.Directory, item.Location)));
					foreach (var item in ProjectService.OpenSolution.Projects.SelectMany(p => p.Items).OfType<FileProjectItem>())
						files.Add(new FileName(item.FileName));
					break;
				case SearchTarget.Directory:
					if (!Directory.Exists(baseDirectory))
						break;
					foreach (var name in FileUtility.SearchDirectory(baseDirectory, filter, searchSubdirs))
						files.Add(new FileName(name));
					break;
				default:
					throw new Exception("Invalid value for FileListType");
			}
			
			return files.Distinct();
		}
		
		public static IObservable<SearchResultMatch> FindAll(string pattern, bool ignoreCase, bool matchWholeWords, SearchMode mode,
		                                                     SearchTarget target, string baseDirectory = null, string filter = "*.*", bool searchSubdirs = false)
		{
			CancellationTokenSource cts = new CancellationTokenSource();
			var monitor = WorkbenchSingleton.Workbench.StatusBar.CreateProgressMonitor(cts.Token);
			monitor.TaskName = "Find All";
			monitor.Status = OperationStatus.Normal;
			var strategy = SearchStrategyFactory.Create(pattern, ignoreCase, matchWholeWords, mode);
			ParseableFileContentFinder fileFinder = new ParseableFileContentFinder();
			var fileList = GenerateFileList(target, baseDirectory, filter, searchSubdirs);
			return new SearchRun(strategy, fileFinder, fileList, monitor, cts);
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
				new System.Threading.Tasks.Task(delegate { Parallel.ForEach(fileList, fileName => SearchFile(fileFinder.Create(fileName).CreateSnapshot(), strategy, monitor.CancellationToken)); }).Start();
				return this;
			}
			
			public void Dispose()
			{
				if (!cts.IsCancellationRequested)
					cts.Cancel();
				monitor.Dispose();
			}
			
			void SearchFile(ITextBuffer buffer, ISearchStrategy strategy, CancellationToken ct)
			{
				if (!MimeTypeDetection.FindMimeType(buffer).StartsWith("text/"))
					return;
				var source = DocumentUtilitites.GetTextSource(buffer);
				foreach(var result in strategy.FindAll(source)) {
					ct.ThrowIfCancellationRequested();
					observer.OnNext(new SearchResultMatch(result.Offset, result.Length));
				}
				lock (monitor)
					monitor.Progress += 1 / fileList.Count();
			}
		}
	}
}
