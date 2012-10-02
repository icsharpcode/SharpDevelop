// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using CSharpBinding.Parser;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor.Search;
using ICSharpCode.SharpDevelop.Project;

namespace CSharpBinding.Refactoring
{
	public class SearchForIssuesCommand : SimpleCommand
	{
		public override void Execute(object parameter)
		{
			SearchForIssuesDialog dlg = new SearchForIssuesDialog();
			dlg.Owner = SD.Workbench.MainWindow;
			if (dlg.ShowDialog() == true) {
				var providers = dlg.SelectedProviders.ToList();
				var fileNames = GetFilesToSearch(dlg.Target).ToList();
				var monitor = SD.StatusBar.CreateProgressMonitor();
				var observable = ReactiveExtensions.CreateObservable<SearchedFile>(
					(m, c) => SearchForIssuesAsync(fileNames, providers, c, m),
					monitor);
				SearchResultsPad.Instance.ShowSearchResults("Issue Search", observable);
			}
		}
		
		IEnumerable<FileName> GetFilesToSearch(SearchForIssuesTarget target)
		{
			SD.MainThread.VerifyAccess();
			switch (target) {
				case SearchForIssuesTarget.CurrentDocument:
					if (SD.Workbench.ActiveViewContent != null) {
						FileName fileName = SD.Workbench.ActiveViewContent.PrimaryFileName;
						if (fileName != null)
							return new[] { fileName };
					}
					break;
				case SearchForIssuesTarget.WholeProject:
					return GetFilesFromProject(ProjectService.CurrentProject);
				case SearchForIssuesTarget.WholeSolution:
					if (ProjectService.OpenSolution != null) {
						return ProjectService.OpenSolution.Projects.SelectMany(GetFilesFromProject).Distinct();
					}
					break;
				default:
					throw new Exception("Invalid value for SearchForIssuesTarget");
			}
			return Enumerable.Empty<FileName>();
		}
		
		IEnumerable<FileName> GetFilesFromProject(IProject project)
		{
			if (project == null)
				return Enumerable.Empty<FileName>();
			return from item in project.GetItemsOfType(ItemType.Compile)
				where item.FileName.EndsWith(".cs", StringComparison.OrdinalIgnoreCase)
				select FileName.Create(item.FileName);
		}
		
		Task SearchForIssuesAsync(List<FileName> fileNames, IEnumerable<IssueManager.IssueProvider> providers, Action<SearchedFile> callback, IProgressMonitor monitor)
		{
			return Task.Run(() => SearchForIssues(fileNames, providers, callback, monitor));
		}
		
		void SearchForIssues(List<FileName> fileNames, IEnumerable<IssueManager.IssueProvider> providers, Action<SearchedFile> callback, IProgressMonitor monitor)
		{
			ParseableFileContentFinder contentFinder = new ParseableFileContentFinder();
			int filesProcessed = 0;
			Parallel.ForEach(
				fileNames,
				delegate (FileName fileName) {
					var fileContent = contentFinder.Create(fileName);
					var resultForFile = SearchForIssues(fileName, fileContent, providers, monitor.CancellationToken);
					if (resultForFile != null) {
						callback(resultForFile);
					}
					monitor.Progress = (double)Interlocked.Increment(ref filesProcessed) / fileNames.Count;
				});
		}
		
		SearchedFile SearchForIssues(FileName fileName, ITextSource fileContent, IEnumerable<IssueManager.IssueProvider> providers, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			var parseInfo = SD.ParserService.Parse(fileName, fileContent, cancellationToken: cancellationToken) as CSharpFullParseInformation;
			if (parseInfo == null)
				return null;
			var compilation = SD.ParserService.GetCompilationForFile(fileName);
			var resolver = parseInfo.GetResolver(compilation);
			var context = new SDRefactoringContext(fileContent, resolver, new TextLocation(0, 0), 0, 0, cancellationToken);
			ReadOnlyDocument document = null;
			IHighlighter highlighter = null;
			var results = new List<SearchResultMatch>();
			foreach (var provider in providers) {
				cancellationToken.ThrowIfCancellationRequested();
				foreach (var issue in provider.GetIssues(context)) {
					if (document == null) {
						document = new ReadOnlyDocument(fileContent, fileName);
						highlighter = SD.EditorControlService.CreateHighlighter(document);
					}
					results.Add(SearchResultMatch.Create(document, issue.Start, issue.End, highlighter));
				}
			}
			if (results.Count > 0)
				return new SearchedFile(fileName, results);
			else
				return null;
		}
	}
	
	public enum SearchForIssuesTarget
	{
		[Description("${res:Dialog.NewProject.SearchReplace.LookIn.CurrentDocument}")]
		CurrentDocument,
		//[Description("${res:Dialog.NewProject.SearchReplace.LookIn.AllOpenDocuments}")]
		//AllOpenFiles,
		[Description("${res:Dialog.NewProject.SearchReplace.LookIn.WholeProject}")]
		WholeProject,
		[Description("${res:Dialog.NewProject.SearchReplace.LookIn.WholeSolution}")]
		WholeSolution
	}
}
