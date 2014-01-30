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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Analysis;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.Search;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Refactoring
{
	/// <summary>
	/// Language-independent finds references implementation.
	/// This call will call into the individual language bindings to perform the job.
	/// </summary>
	public static class FindReferenceService
	{
		#region FindReferences
		static IEnumerable<IProject> GetProjectsThatCouldReferenceEntity(ISymbol entity)
		{
			ISolution solution = ProjectService.OpenSolution;
			if (solution == null)
				yield break;
			foreach (IProject project in solution.Projects) {
				IProjectContent pc = project.ProjectContent;
				if (pc == null)
					continue;
				yield return project;
			}
		}
		
		static List<ISymbolSearch> PrepareSymbolSearch(ISymbol entity, CancellationToken cancellationToken, out double totalWorkAmount)
		{
			totalWorkAmount = 0;
			List<ISymbolSearch> symbolSearches = new List<ISymbolSearch>();
			foreach (IProject project in GetProjectsThatCouldReferenceEntity(entity)) {
				cancellationToken.ThrowIfCancellationRequested();
				ISymbolSearch symbolSearch = project.PrepareSymbolSearch(entity);
				if (symbolSearch != null) {
					symbolSearches.Add(symbolSearch);
					totalWorkAmount += symbolSearch.WorkAmount;
				}
			}
			if (totalWorkAmount < 1)
				totalWorkAmount = 1;
			return symbolSearches;
		}
		
		/// <summary>
		/// Finds all references to the specified entity.
		/// The results are reported using the callback.
		/// FindReferences may internally use parallelism, and may invoke the callback on multiple
		/// threads in parallel.
		/// </summary>
		public static async Task FindReferencesAsync(IEntity entity, IProgressMonitor progressMonitor, Action<SearchedFile> callback)
		{
			if (entity == null)
				throw new ArgumentNullException("entity");
			if (progressMonitor == null)
				throw new ArgumentNullException("progressMonitor");
			if (callback == null)
				throw new ArgumentNullException("callback");
			SD.MainThread.VerifyAccess();
			if (SD.ParserService.LoadSolutionProjectsThread.IsRunning) {
				progressMonitor.ShowingDialog = true;
				MessageService.ShowMessage("${res:SharpDevelop.Refactoring.LoadSolutionProjectsThreadRunning}");
				progressMonitor.ShowingDialog = false;
				return;
			}
			double totalWorkAmount;
			List<ISymbolSearch> symbolSearches = PrepareSymbolSearch(entity, progressMonitor.CancellationToken, out totalWorkAmount);
			double workDone = 0;
			ParseableFileContentFinder parseableFileContentFinder = new ParseableFileContentFinder();
			foreach (ISymbolSearch s in symbolSearches) {
				progressMonitor.CancellationToken.ThrowIfCancellationRequested();
				using (var childProgressMonitor = progressMonitor.CreateSubTask(s.WorkAmount / totalWorkAmount)) {
					await s.FindReferencesAsync(new SymbolSearchArgs(childProgressMonitor, parseableFileContentFinder), callback);
				}
				
				workDone += s.WorkAmount;
				progressMonitor.Progress = workDone / totalWorkAmount;
			}
		}
		
		public static IObservable<SearchedFile> FindReferences(IEntity entity, IProgressMonitor progressMonitor)
		{
			return ReactiveExtensions.CreateObservable<SearchedFile>(
				(monitor, callback) => FindReferencesAsync(entity, monitor, callback),
				progressMonitor);
		}
		
		/// <summary>
		/// Finds references to a local variable.
		/// </summary>
		public static async Task<SearchedFile> FindLocalReferencesAsync(IVariable variable, IProgressMonitor progressMonitor)
		{
			if (variable == null)
				throw new ArgumentNullException("variable");
			if (progressMonitor == null)
				throw new ArgumentNullException("progressMonitor");
			var fileName = FileName.Create(variable.Region.FileName);
			List<SearchResultMatch> references = new List<SearchResultMatch>();
			await SD.ParserService.FindLocalReferencesAsync(
				fileName, variable,
				r => { lock (references) references.Add(r); },
				cancellationToken: progressMonitor.CancellationToken);
			return new SearchedFile(fileName, references);
		}
		
		public static IObservable<SearchedFile> FindLocalReferences(IVariable variable, IProgressMonitor progressMonitor)
		{
			return ReactiveExtensions.CreateObservable<SearchedFile>(
				(monitor, callback) => FindLocalReferencesAsync(variable, monitor).ContinueWith(t => callback(t.Result)),
				progressMonitor);
		}
		#endregion
		
		#region FindDerivedTypes
		/// <summary>
		/// Finds all types that are derived from the given base type.
		/// </summary>
		public static IList<ITypeDefinition> FindDerivedTypes(ITypeDefinition baseType, bool directDerivationOnly)
		{
			if (baseType == null)
				throw new ArgumentNullException("baseType");
			baseType = baseType.GetDefinition(); // ensure we use the compound class
			
			List<ITypeDefinition> results = new List<ITypeDefinition>();
			
			var solutionSnapshot = GetSolutionSnapshot(baseType.Compilation);
			foreach (IProject project in GetProjectsThatCouldReferenceEntity(baseType)) {
				var compilation = solutionSnapshot.GetCompilation(project);
				var importedBaseType = compilation.Import(baseType);
				if (importedBaseType == null)
					continue;
				foreach (ITypeDefinition typeDef in compilation.MainAssembly.GetAllTypeDefinitions()) {
					bool isDerived;
					if (directDerivationOnly) {
						isDerived = typeDef.DirectBaseTypes.Select(t => t.GetDefinition()).Contains(importedBaseType);
					} else {
						isDerived = typeDef.IsDerivedFrom(importedBaseType);
					}
					if (isDerived)
						results.Add(typeDef);
				}
			}
			return results;
		}
		
		static ISolutionSnapshotWithProjectMapping GetSolutionSnapshot(ICompilation compilation)
		{
			var snapshot = compilation.SolutionSnapshot as ISolutionSnapshotWithProjectMapping;
			return snapshot ?? SD.ParserService.GetCurrentSolutionSnapshot();
		}
		
		
		/// <summary>
		/// Builds a graph of derived type definitions.
		/// </summary>
		public static TypeGraphNode BuildDerivedTypesGraph(ITypeDefinition baseType)
		{
			if (baseType == null)
				throw new ArgumentNullException("baseType");
			var solutionSnapshot = GetSolutionSnapshot(baseType.Compilation);
			var assemblies = GetProjectsThatCouldReferenceEntity(baseType).Select(p => solutionSnapshot.GetCompilation(p).MainAssembly);
			var graph = new TypeGraph(assemblies);
			var node = graph.GetNode(baseType);
			if (node != null) {
				// only derived types were requested, so don't return the base types
				// (this helps the GC to collect the unused parts of the graph more quickly)
				node.BaseTypes.Clear();
				return node;
			} else {
				return new TypeGraphNode(baseType);
			}
		}
		#endregion
		
		public static async Task RenameSymbolAsync(ISymbol symbol, string newName, IProgressMonitor progressMonitor, Action<Error> errorCallback)
		{
			if (symbol == null)
				throw new ArgumentNullException("symbol");
			if (progressMonitor == null)
				throw new ArgumentNullException("progressMonitor");
			SD.MainThread.VerifyAccess();
			if (SD.ParserService.LoadSolutionProjectsThread.IsRunning) {
				progressMonitor.ShowingDialog = true;
				MessageService.ShowMessage("${res:SharpDevelop.Refactoring.LoadSolutionProjectsThreadRunning}");
				progressMonitor.ShowingDialog = false;
				return;
			}
			double totalWorkAmount;
			List<ISymbolSearch> symbolSearches = PrepareSymbolSearch(symbol, progressMonitor.CancellationToken, out totalWorkAmount);
			double workDone = 0;
			ParseableFileContentFinder parseableFileContentFinder = new ParseableFileContentFinder();
			var errors = new List<Error>();
			var changes = new List<PatchedFile>();
			foreach (ISymbolSearch s in symbolSearches) {
				progressMonitor.CancellationToken.ThrowIfCancellationRequested();
				using (var childProgressMonitor = progressMonitor.CreateSubTask(s.WorkAmount / totalWorkAmount)) {
					var args = new SymbolRenameArgs(newName, childProgressMonitor, parseableFileContentFinder);
					args.ProvideHighlightedLine = false;
					await s.RenameAsync(args, file => changes.Add(file), error => errors.Add(error));
				}
				
				workDone += s.WorkAmount;
				progressMonitor.Progress = workDone / totalWorkAmount;
			}
			if (errors.Count == 0) {
				foreach (var file in changes) {
					ApplyChanges(file);
				}
			} else {
				foreach (var error in errors) {
					errorCallback(error);
				}
			}
		}
		
		public static IObservable<Error> RenameSymbol(ISymbol symbol, string newName, IProgressMonitor progressMonitor)
		{
			return ReactiveExtensions.CreateObservable<Error>(
				(monitor, callback) => RenameSymbolAsync(symbol, newName, monitor, callback),
				progressMonitor);
		}
		
		static void ApplyChanges(PatchedFile file)
		{
			var openedFile = SD.FileService.GetOpenedFile(file.FileName);
			if (openedFile == null) {
				SD.FileService.OpenFile(file.FileName, false);
				openedFile = SD.FileService.GetOpenedFile(file.FileName); //?
			}
			
			var provider = openedFile.CurrentView.GetService<IFileDocumentProvider>();
			if (provider != null) {
				var document = provider.GetDocumentForFile(openedFile);
				if (document == null)
					throw new InvalidOperationException("Editor/document not found!");
				file.Apply(document);
				openedFile.MakeDirty();
			}
		}
	}
	
	public class SymbolSearchArgs
	{
		public IProgressMonitor ProgressMonitor { get; private set; }
		
		public CancellationToken CancellationToken {
			get { return this.ProgressMonitor.CancellationToken; }
		}
		
		/// <summary>
		/// Specifies whether the symbol search should pass a HighlightedInlineBuilder
		/// for the matching line to the SearchResultMatch.
		/// The default value is <c>true</c>.
		/// </summary>
		public bool ProvideHighlightedLine { get; set; }
		
		public ParseableFileContentFinder ParseableFileContentFinder { get; private set; }
		
		public SymbolSearchArgs(IProgressMonitor progressMonitor, ParseableFileContentFinder parseableFileContentFinder)
		{
			if (progressMonitor == null)
				throw new ArgumentNullException("progressMonitor");
			if (parseableFileContentFinder == null)
				throw new ArgumentNullException("parseableFileContentFinder");
			this.ProgressMonitor = progressMonitor;
			this.ParseableFileContentFinder = parseableFileContentFinder;
			this.ProvideHighlightedLine = true;
		}
	}
	
	public class SymbolRenameArgs : SymbolSearchArgs
	{
		public SymbolRenameArgs(string newName, IProgressMonitor progressMonitor, ParseableFileContentFinder parseableFileContentFinder)
			: base(progressMonitor, parseableFileContentFinder)
		{
			if (newName == null)
				throw new ArgumentNullException("newName");
			NewName = newName;
		}
		
		public string NewName { get; private set; }
	}
	
	public interface ISymbolSearch
	{
		double WorkAmount { get; }
		
		Task FindReferencesAsync(SymbolSearchArgs searchArguments, Action<SearchedFile> callback);
		Task RenameAsync(SymbolRenameArgs renameArguments, Action<PatchedFile> callback, Action<Error> errorCallback);
	}
	
	public sealed class CompositeSymbolSearch : ISymbolSearch
	{
		IEnumerable<ISymbolSearch> symbolSearches;
		
		CompositeSymbolSearch(params ISymbolSearch[] symbolSearches)
		{
			this.symbolSearches = symbolSearches;
		}
		
		public static ISymbolSearch Create(ISymbolSearch symbolSearch1, ISymbolSearch symbolSearch2)
		{
			if (symbolSearch1 == null)
				return symbolSearch2;
			if (symbolSearch2 == null)
				return symbolSearch1;
			return new CompositeSymbolSearch(symbolSearch1, symbolSearch2);
		}
		
		public double WorkAmount {
			get { return symbolSearches.Sum(s => s.WorkAmount); }
		}
		
		public Task FindReferencesAsync(SymbolSearchArgs searchArguments, Action<SearchedFile> callback)
		{
			return Task.WhenAll(symbolSearches.Select(s => s.FindReferencesAsync(searchArguments, callback)));
		}
		
		public Task RenameAsync(SymbolRenameArgs renameArguments, Action<PatchedFile> callback, Action<Error> errorCallback)
		{
			return Task.WhenAll(symbolSearches.Select(s => s.RenameAsync(renameArguments, callback, errorCallback)));
		}
	}
}
