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
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

using ICSharpCode.NRefactory.Analysis;
using CSharpBinding.Parser;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.CSharp.TypeSystem;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.Utils;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.Search;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Refactoring;

namespace CSharpBinding
{
	/// <summary>
	/// C# backend implementation for 'find references'.
	/// </summary>
	public class CSharpSymbolSearch : ISymbolSearch
	{
		readonly IProject project;
		readonly ICompilation compilation;
		readonly FindReferences fr = new FindReferences();
		readonly IList<IFindReferenceSearchScope> searchScopes;
		readonly Dictionary<string, IList<IFindReferenceSearchScope>> searchScopesPerFile;
		readonly int workAmount;
		readonly double workAmountInverse;
		
		public CSharpSymbolSearch(IProject project, ISymbol entity)
		{
			this.project = project;
			compilation = SD.ParserService.GetCompilation(project);
			var relatedSymbols = GetRelatedSymbols(entity);
			if ((relatedSymbols != null) && relatedSymbols.Any()) {
				searchScopes = relatedSymbols.SelectMany(e => fr.GetSearchScopes(e)).ToList();
			} else {
				searchScopes = fr.GetSearchScopes(entity);
			}
			
			searchScopesPerFile = new Dictionary<string, IList<IFindReferenceSearchScope>>();
			for (int i = 0; i < searchScopes.Count; i++) {
				var thisSearchScope = searchScopes[i];
				var interestingFiles = fr.GetInterestingFiles(thisSearchScope, compilation).Select(f => f.FileName);
				foreach (var file in interestingFiles) {
					if (!searchScopesPerFile.ContainsKey(file))
						searchScopesPerFile[file] = new List<IFindReferenceSearchScope>();
					searchScopesPerFile[file].Add(thisSearchScope);
					workAmount++;
				}
			}
			workAmountInverse = 1.0 / workAmount;
		}
		
		IEnumerable<ISymbol> GetRelatedSymbols(ISymbol entity)
		{
			var typeGraph = new Lazy<TypeGraph>(() => new TypeGraph(new [] { compilation.MainAssembly }));
			var symbolCollector = new SymbolCollector();
			return symbolCollector.GetRelatedSymbols(typeGraph, entity);
		}
		
		public double WorkAmount {
			get { return workAmount; }
		}
		
		public Task FindReferencesAsync(SymbolSearchArgs args, Action<SearchedFile> callback)
		{
			if (callback == null)
				throw new ArgumentNullException("callback");
			var cancellationToken = args.ProgressMonitor.CancellationToken;
			return Task.Run(
				() => {
					object progressLock = new object();
					Parallel.ForEach(
						searchScopesPerFile.Keys,
						new ParallelOptions {
							MaxDegreeOfParallelism = Environment.ProcessorCount,
							CancellationToken = cancellationToken
						},
						delegate (string fileName) {
							try {
								FindReferencesInFile(args, searchScopesPerFile[fileName], FileName.Create(fileName), callback, cancellationToken);
							} catch (OperationCanceledException) {
								throw;
							} catch (Exception ex) {
								throw new ApplicationException("Error searching in file '" + fileName + "'", ex);
							}
							lock (progressLock)
								args.ProgressMonitor.Progress += workAmountInverse;
						});
				}, cancellationToken
			);
		}
		
		void FindReferencesInFile(SymbolSearchArgs args, IList<IFindReferenceSearchScope> searchScopeList, FileName fileName, Action<SearchedFile> callback, CancellationToken cancellationToken)
		{
			ITextSource textSource = args.ParseableFileContentFinder.Create(fileName);
			if (textSource == null)
				return;
			if (searchScopeList != null) {
				if (!searchScopeList.DistinctBy(scope => scope.SearchTerm ?? String.Empty).Any(
					scope => (scope.SearchTerm == null) || (textSource.IndexOf(scope.SearchTerm, 0, textSource.TextLength, StringComparison.Ordinal) >= 0)))
					return;
			}
			
			var parseInfo = SD.ParserService.Parse(fileName, textSource) as CSharpFullParseInformation;
			if (parseInfo == null)
				return;
			ReadOnlyDocument document = null;
			IHighlighter highlighter = null;
			List<SearchResultMatch> results = new List<SearchResultMatch>();
			
			// Grab the unresolved file matching the compilation version
			// (this may differ from the version created by re-parsing the project)
			CSharpUnresolvedFile unresolvedFile = null;
			IProjectContent pc = compilation.MainAssembly.UnresolvedAssembly as IProjectContent;
			if (pc != null) {
				unresolvedFile = pc.GetFile(fileName) as CSharpUnresolvedFile;
			}
			
			fr.FindReferencesInFile(
				searchScopeList, unresolvedFile, parseInfo.SyntaxTree, compilation,
				delegate (AstNode node, ResolveResult result) {
					if (document == null) {
						document = new ReadOnlyDocument(textSource, fileName);
						highlighter = SD.EditorControlService.CreateHighlighter(document);
						highlighter.BeginHighlighting();
					}
					Identifier identifier = node.GetChildByRole(Roles.Identifier);
					if (!identifier.IsNull)
						node = identifier;
					var region = new DomRegion(fileName, node.StartLocation, node.EndLocation);
					int offset = document.GetOffset(node.StartLocation);
					int length = document.GetOffset(node.EndLocation) - offset;
					var builder = SearchResultsPad.CreateInlineBuilder(node.StartLocation, node.EndLocation, document, highlighter);
					var defaultTextColor = highlighter != null ? highlighter.DefaultTextColor : null;
					results.Add(new SearchResultMatch(fileName, node.StartLocation, node.EndLocation, offset, length, builder, defaultTextColor));
				}, cancellationToken);
			if (highlighter != null) {
				highlighter.Dispose();
			}
			if (results.Count > 0) {
				// Remove overlapping results
				List<SearchResultMatch> fixedResults = new List<SearchResultMatch>();
				int lastEndOffset = 0;
				foreach (var result in results.OrderBy(m => m.StartOffset)) {
					if (result.StartOffset >= lastEndOffset) {
						fixedResults.Add(result);
						lastEndOffset = result.EndOffset;
					}
				}
				callback(new SearchedFile(fileName, fixedResults));
			}
		}
		
		public Task RenameAsync(SymbolRenameArgs args, Action<PatchedFile> callback, Action<Error> errorCallback)
		{
			if (callback == null)
				throw new ArgumentNullException("callback");
			var cancellationToken = args.ProgressMonitor.CancellationToken;
			return Task.Run(
				() => {
					bool isNameValid = ICSharpCode.NRefactory.MonoCSharp.Tokenizer.IsValidIdentifier(args.NewName);
					object progressLock = new object();
					Parallel.ForEach(
						searchScopesPerFile.Keys,
						new ParallelOptions {
							MaxDegreeOfParallelism = Environment.ProcessorCount,
							CancellationToken = cancellationToken
						},
						delegate (string fileName) {
							RenameReferencesInFile(args, searchScopesPerFile[fileName], FileName.Create(fileName), callback, errorCallback, isNameValid, cancellationToken);
							lock (progressLock)
								args.ProgressMonitor.Progress += workAmountInverse;
						});
				}, cancellationToken
			);
		}
		
		void RenameReferencesInFile(SymbolRenameArgs args, IList<IFindReferenceSearchScope> searchScopeList, FileName fileName, Action<PatchedFile> callback, Action<Error> errorCallback, bool isNameValid, CancellationToken cancellationToken)
		{
			ITextSource textSource = args.ParseableFileContentFinder.Create(fileName);
			if (textSource == null)
				return;
			if (searchScopeList != null) {
				if (!searchScopeList.DistinctBy(scope => scope.SearchTerm ?? String.Empty).Any(
					scope => (scope.SearchTerm == null) || (textSource.IndexOf(scope.SearchTerm, 0, textSource.TextLength, StringComparison.Ordinal) >= 0)))
					return;
			}
			
			var parseInfo = SD.ParserService.Parse(fileName, textSource) as CSharpFullParseInformation;
			if (parseInfo == null)
				return;
			ReadOnlyDocument document = null;
			IHighlighter highlighter = null;
			List<RenameResultMatch> results = new List<RenameResultMatch>();
			
			// Grab the unresolved file matching the compilation version
			// (this may differ from the version created by re-parsing the project)
			CSharpUnresolvedFile unresolvedFile = null;
			IProjectContent pc = compilation.MainAssembly.UnresolvedAssembly as IProjectContent;
			if (pc != null) {
				unresolvedFile = pc.GetFile(fileName) as CSharpUnresolvedFile;
			}
			
			CSharpAstResolver resolver = new CSharpAstResolver(compilation, parseInfo.SyntaxTree, unresolvedFile);
			
			fr.RenameReferencesInFile(
				searchScopeList, args.NewName, resolver,
				delegate (RenameCallbackArguments callbackArgs) {
					var node = callbackArgs.NodeToReplace;
					string newCode = callbackArgs.NewNode.ToString();
					if (document == null) {
						document = new ReadOnlyDocument(textSource, fileName);
						
						if (args.ProvideHighlightedLine) {
							highlighter = SD.EditorControlService.CreateHighlighter(document);
							highlighter.BeginHighlighting();
						}
					}
					var startLocation = node.StartLocation;
					var endLocation = node.EndLocation;
					int offset = document.GetOffset(startLocation);
					int length = document.GetOffset(endLocation) - offset;
					if (args.ProvideHighlightedLine) {
						var builder = SearchResultsPad.CreateInlineBuilder(node.StartLocation, node.EndLocation, document, highlighter);
						var defaultTextColor = highlighter != null ? highlighter.DefaultTextColor : null;
						results.Add(new RenameResultMatch(fileName, startLocation, endLocation, offset, length, newCode, builder, defaultTextColor));
					} else {
						results.Add(new RenameResultMatch(fileName, startLocation, endLocation, offset, length, newCode));
					}
				},
				errorCallback, cancellationToken);
			if (highlighter != null) {
				highlighter.Dispose();
			}
			if (results.Count > 0) {
				if (!isNameValid) {
					errorCallback(new Error(ErrorType.Error, string.Format("The name '{0}' is not valid in the current context!", args.NewName),
					                        new DomRegion(fileName, results[0].StartLocation)));
					return;
				}
				IDocument changedDocument = new TextDocument(document);
				var oldVersion = changedDocument.Version;
				List<SearchResultMatch> fixedResults = new List<SearchResultMatch>();
				int lastStartOffset = changedDocument.TextLength + 1;
				foreach (var result in results.OrderByDescending(m => m.StartOffset)) {
					if (result.EndOffset <= lastStartOffset) {
						changedDocument.Replace(result.StartOffset, result.Length, result.NewCode);
						fixedResults.Add(result);
						lastStartOffset = result.StartOffset;
					}
				}
				callback(new PatchedFile(fileName, fixedResults, oldVersion, changedDocument.Version));
			}
		}
	}
}
