// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

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
		IProject project;
		ICompilation compilation;
		FindReferences fr = new FindReferences();
		IList<IFindReferenceSearchScope> searchScopes;
		IList<string>[] interestingFileNames;
		int workAmount;
		double workAmountInverse;
		
		public CSharpSymbolSearch(IProject project, ISymbol entity)
		{
			this.project = project;
			searchScopes = fr.GetSearchScopes(entity);
			compilation = SD.ParserService.GetCompilation(project);
			interestingFileNames = new IList<string>[searchScopes.Count];
			for (int i = 0; i < searchScopes.Count; i++) {
				interestingFileNames[i] = fr.GetInterestingFiles(searchScopes[i], compilation).Select(f => f.FileName).ToList();
				workAmount += interestingFileNames[i].Count;
			}
			workAmountInverse = 1.0 / workAmount;
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
					for (int i = 0; i < searchScopes.Count; i++) {
						IFindReferenceSearchScope searchScope = searchScopes[i];
						object progressLock = new object();
						Parallel.ForEach(
							interestingFileNames[i],
							new ParallelOptions {
								MaxDegreeOfParallelism = Environment.ProcessorCount,
								CancellationToken = cancellationToken
							},
							delegate (string fileName) {
								try {
									FindReferencesInFile(args, searchScope, FileName.Create(fileName), callback, cancellationToken);
								} catch (OperationCanceledException) {
									throw;
								} catch (Exception ex) {
									throw new ApplicationException("Error searching in file '" + fileName + "'", ex);
								}
								lock (progressLock)
									args.ProgressMonitor.Progress += workAmountInverse;
							});
					}
				}, cancellationToken
			);
		}
		
		void FindReferencesInFile(SymbolSearchArgs args, IFindReferenceSearchScope searchScope, FileName fileName, Action<SearchedFile> callback, CancellationToken cancellationToken)
		{
			ITextSource textSource = args.ParseableFileContentFinder.Create(fileName);
			if (textSource == null)
				return;
			if (searchScope.SearchTerm != null) {
				if (textSource.IndexOf(searchScope.SearchTerm, 0, textSource.TextLength, StringComparison.Ordinal) < 0)
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
				searchScope, unresolvedFile, parseInfo.SyntaxTree, compilation,
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
			if (results.Count > 0)
				callback(new SearchedFile(fileName, results));
		}
		
		public Task RenameAsync(SymbolRenameArgs args, Action<PatchedFile> callback, Action<Error> errorCallback)
		{
			if (callback == null)
				throw new ArgumentNullException("callback");
			var cancellationToken = args.ProgressMonitor.CancellationToken;
			return Task.Run(
				() => {
					bool isNameValid = Mono.CSharp.Tokenizer.IsValidIdentifier(args.NewName);
					for (int i = 0; i < searchScopes.Count; i++) {
						IFindReferenceSearchScope searchScope = searchScopes[i];
						object progressLock = new object();
						Parallel.ForEach(
							interestingFileNames[i],
							new ParallelOptions {
								MaxDegreeOfParallelism = Environment.ProcessorCount,
								CancellationToken = cancellationToken
							},
							delegate (string fileName) {
								RenameReferencesInFile(args, searchScope, FileName.Create(fileName), callback, errorCallback, isNameValid, cancellationToken);
								lock (progressLock)
									args.ProgressMonitor.Progress += workAmountInverse;
							});
					}
				}, cancellationToken
			);
		}
		
		void RenameReferencesInFile(SymbolRenameArgs args, IFindReferenceSearchScope searchScope, FileName fileName, Action<PatchedFile> callback, Action<Error> errorCallback, bool isNameValid, CancellationToken cancellationToken)
		{
			ITextSource textSource = args.ParseableFileContentFinder.Create(fileName);
			if (textSource == null)
				return;
			if (searchScope.SearchTerm != null) {
				if (textSource.IndexOf(searchScope.SearchTerm, 0, textSource.TextLength, StringComparison.Ordinal) < 0)
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
				new[] { searchScope }, args.NewName, resolver,
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
				foreach (var result in results.OrderByDescending(m => m.StartOffset)) {
					changedDocument.Replace(result.StartOffset, result.Length, result.NewCode);
				}
				callback(new PatchedFile(fileName, results, oldVersion, changedDocument.Version));
			}
		}
	}
}
