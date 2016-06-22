// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CSharpBinding.Parser;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.Search;
using ICSharpCode.SharpDevelop.Gui;
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
				string title = "Issue Search";
				var providers = dlg.SelectedProviders.ToList();
				var fileNames = GetFilesToSearch(dlg.Target).ToList();
				if (dlg.FixIssues) {
					int fixedIssueCount = 0;
					IReadOnlyList<SearchResultMatch> remainingIssues = null;
					AsynchronousWaitDialog.RunInCancellableWaitDialog(
						title, null,
						monitor => {
							remainingIssues = FindAndFixIssues(fileNames, providers, monitor, out fixedIssueCount);
						});
					if (remainingIssues == null)
						return; // can happen if issue search is cancelled
					string message = string.Format(
						"{0} issues were fixed automatically." +
						"{1} issues are remaining (no automatic fix available).",
						fixedIssueCount, remainingIssues.Count);
					SearchResultsPad.Instance.ShowSearchResults(title, remainingIssues);
					MessageService.ShowMessage(message, title);
				} else if (Keyboard.Modifiers == (ModifierKeys.Control | ModifierKeys.Alt | ModifierKeys.Shift)) {
					// Ctrl+Alt+Shift => run issue search on main thread,
					// this helps debugging as exceptions don't get caught and passed from one thread to another
					List<SearchResultMatch> issues = new List<SearchResultMatch>();
					AsynchronousWaitDialog.RunInCancellableWaitDialog(
						title, null,
						monitor => SearchForIssues(fileNames, providers, f => issues.AddRange(f.Matches), monitor)
					);
					SearchResultsPad.Instance.ShowSearchResults(title, issues);
				} else {
					var monitor = SD.StatusBar.CreateProgressMonitor();
					var observable = ReactiveExtensions.CreateObservable<SearchedFile>(
						(m, c) => SearchForIssuesAsync(fileNames, providers, c, m),
						monitor);
					SearchResultsPad.Instance.ShowSearchResults(title, observable);
				}
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
				where item.FileName.HasExtension(".cs")
				select item.FileName;
		}
		
		Task SearchForIssuesAsync(List<FileName> fileNames, IEnumerable<IssueManager.IssueProvider> providers, Action<SearchedFile> callback, IProgressMonitor monitor)
		{
			return Task.Run(() => SearchForIssuesParallel(fileNames, providers, callback, monitor));
		}
		
		void SearchForIssuesParallel(List<FileName> fileNames, IEnumerable<IssueManager.IssueProvider> providers, Action<SearchedFile> callback, IProgressMonitor monitor)
		{
			ParseableFileContentFinder contentFinder = new ParseableFileContentFinder();
			int filesProcessed = 0;
			Parallel.ForEach(
				fileNames,
				new ParallelOptions {
					MaxDegreeOfParallelism = Environment.ProcessorCount,
					CancellationToken = monitor.CancellationToken
				},
				delegate (FileName fileName) {
					var fileContent = contentFinder.Create(fileName);
					try {
						var resultForFile = SearchForIssues(fileName, fileContent, providers, monitor.CancellationToken);
						if (resultForFile != null) {
							callback(resultForFile);
						}
					} catch (IOException) {
						// ignore IO exceptions (e.g. a file is missing)
					} catch (OperationCanceledException) {
						throw;
					} catch (Exception ex) {
						throw new ApplicationException("Exception while searching for issues in " + fileName, ex);
					}
					monitor.Progress = (double)Interlocked.Increment(ref filesProcessed) / fileNames.Count;
				});
		}
		
		void SearchForIssues(List<FileName> fileNames, IEnumerable<IssueManager.IssueProvider> providers, Action<SearchedFile> callback, IProgressMonitor monitor)
		{
			// used for debugging so that we can break in the crashing issue
			ParseableFileContentFinder contentFinder = new ParseableFileContentFinder();
			int filesProcessed = 0;
			foreach (FileName fileName in fileNames) {
				var fileContent = contentFinder.Create(fileName);
				var resultForFile = SearchForIssues(fileName, fileContent, providers, monitor.CancellationToken);
				if (resultForFile != null) {
					callback(resultForFile);
				}
				monitor.Progress = (double)(++filesProcessed) / fileNames.Count;
			}
		}
		
		SearchedFile SearchForIssues(FileName fileName, ITextSource fileContent, IEnumerable<IssueManager.IssueProvider> providers, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			var context = SDRefactoringContext.Create(fileName, fileContent, TextLocation.Empty, cancellationToken);
			ReadOnlyDocument document = null;
			IHighlighter highlighter = null;
			var results = new List<SearchResultMatch>();
			foreach (var provider in providers) {
				cancellationToken.ThrowIfCancellationRequested();
				foreach (var issue in provider.GetIssues(context)) {
					if (document == null) {
						document = new ReadOnlyDocument(fileContent, fileName);
						highlighter = SD.EditorControlService.CreateHighlighter(document);
						highlighter.BeginHighlighting();
					}
					results.Add(SearchResultMatch.Create(document, issue.Start, issue.End, highlighter));
				}
			}
			if (highlighter != null) {
				highlighter.Dispose();
			}
			if (results.Count > 0)
				return new SearchedFile(fileName, results);
			else
				return null;
		}
		
		IReadOnlyList<SearchResultMatch> FindAndFixIssues(List<FileName> fileNames, List<IssueManager.IssueProvider> providers, IProgressMonitor progress, out int fixedIssueCount)
		{
			fixedIssueCount = 0;
			List<SearchResultMatch> remainingIssues = new List<SearchResultMatch>();
			for (int i = 0; i < fileNames.Count; i++) {
				remainingIssues.AddRange(FindAndFixIssues(fileNames[i], providers, progress.CancellationToken, ref fixedIssueCount));
				progress.Report((double)i / fileNames.Count);
			}
			return remainingIssues;
		}
		
		IEnumerable<SearchResultMatch> FindAndFixIssues(FileName fileName, List<IssueManager.IssueProvider> providers, CancellationToken cancellationToken, ref int fixedIssueCount)
		{
			cancellationToken.ThrowIfCancellationRequested();
			var openedFile = SD.FileService.GetOpenedFile(fileName);
			bool documentWasLoadedFromDisk = false;
			IDocument document = null;
			if (openedFile != null && openedFile.CurrentView != null) {
				var provider = openedFile.CurrentView.GetService<IFileDocumentProvider>();
				if (provider != null)
					document = provider.GetDocumentForFile(openedFile);
			}
			if (document == null) {
				documentWasLoadedFromDisk = true;
				document = new TextDocument(SD.FileService.GetFileContent(fileName)) { FileName = fileName };
			}
			
			var context = SDRefactoringContext.Create(fileName, document, TextLocation.Empty, cancellationToken);
			List<CodeIssue> allIssues = new List<CodeIssue>();
			bool documentWasChanged = false;
			foreach (var provider in providers) {
				cancellationToken.ThrowIfCancellationRequested();
				var issues = provider.GetIssues(context).ToList();
				// Fix issues, if possible:
				if (provider.Attribute.SupportsAutoFix && issues.Any(i => i.Actions.Count > 0)) {
					using (var script = context.StartScript()) {
						foreach (var issue in issues) {
							if (issue.Actions.Count > 0) {
								fixedIssueCount++;
								issue.Actions[0].Run(script);
							}
						}
					}
					documentWasChanged = true;
					// Update context now that we've modified the document
					context = SDRefactoringContext.Create(fileName, document, TextLocation.Empty, cancellationToken);
					// Find remaining issues:
					allIssues.AddRange(provider.GetIssues(context));
				} else {
					allIssues.AddRange(issues);
				}
			}
			if (documentWasChanged && documentWasLoadedFromDisk) {
				// Save changes back to disk
				using (var writer = new StreamWriter(fileName, false, SD.FileService.DefaultFileEncoding)) {
					document.WriteTextTo(writer);
				}
			}
			if (allIssues.Count > 0) {
				using (var highlighter = SD.EditorControlService.CreateHighlighter(document)) {
					highlighter.BeginHighlighting();
					return allIssues.Select(issue => SearchResultMatch.Create(document, issue.Start, issue.End, highlighter)).ToList();
				}
			} else {
				return Enumerable.Empty<SearchResultMatch>();
			}
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
