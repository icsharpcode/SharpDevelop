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
using System.Xml;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor.Search;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Refactoring;

namespace ICSharpCode.XamlBinding
{
	/// <summary>
	/// Description of XamlSymbolSearch.
	/// </summary>
	public class XamlSymbolSearch : ISymbolSearch
	{
		ICompilation compilation;
		ISymbol entity;
		List<FileName> interestingFileNames;
		int workAmount;
		double workAmountInverse;
		
		public XamlSymbolSearch(IProject project, ISymbol entity)
		{
			compilation = SD.ParserService.GetCompilation(project);
			if (entity is IEntity)
				this.entity = compilation.Import((IEntity)entity);
			interestingFileNames = new List<FileName>();
			if (this.entity == null)
				return;
			foreach (var item in project.Items.OfType<FileProjectItem>().Where(i => i.FileName.HasExtension(".xaml")))
				interestingFileNames.Add(item.FileName);
			workAmount = interestingFileNames.Count;
			workAmountInverse = 1.0 / workAmount;
		}
		
		public double WorkAmount {
			get { return workAmount; }
		}
		
		public Task FindReferencesAsync(SymbolSearchArgs searchArguments, Action<SearchedFile> callback)
		{
			if (callback == null)
				throw new ArgumentNullException("callback");
			var cancellationToken = searchArguments.ProgressMonitor.CancellationToken;
			return Task.Run(
				() => {
					object progressLock = new object();
					Parallel.ForEach(
						interestingFileNames,
						new ParallelOptions {
							MaxDegreeOfParallelism = Environment.ProcessorCount,
							CancellationToken = cancellationToken
						},
						delegate (FileName fileName) {
							FindReferencesInFile(searchArguments, entity, fileName, callback, cancellationToken);
							lock (progressLock)
								searchArguments.ProgressMonitor.Progress += workAmountInverse;
						});
				}, cancellationToken
			);
		}
		
		void FindReferencesInFile(SymbolSearchArgs searchArguments, ISymbol entity, FileName fileName, Action<SearchedFile> callback, CancellationToken cancellationToken)
		{
			ITextSource textSource = searchArguments.ParseableFileContentFinder.Create(fileName);
			if (textSource == null)
				return;
			int offset = textSource.IndexOf(entity.Name, 0, textSource.TextLength, StringComparison.Ordinal);
			if (offset < 0)
				return;
			
			var parseInfo = SD.ParserService.Parse(fileName, textSource) as XamlFullParseInformation;
			if (parseInfo == null)
				return;
			ReadOnlyDocument document = null;
			IHighlighter highlighter = null;
			List<SearchResultMatch> results = new List<SearchResultMatch>();
			XamlAstResolver resolver = new XamlAstResolver(compilation, parseInfo);
			do {
				if (document == null) {
					document = new ReadOnlyDocument(textSource, fileName);
					highlighter = SD.EditorControlService.CreateHighlighter(document);
					highlighter.BeginHighlighting();
				}
				var result = resolver.ResolveAtLocation(document.GetLocation(offset + entity.Name.Length / 2 + 1), cancellationToken);
				int length = entity.Name.Length;
				if ((result is TypeResolveResult && ((TypeResolveResult)result).Type.Equals(entity)) || (result is MemberResolveResult && ((MemberResolveResult)result).Member.Equals(entity))) {
					var region = new DomRegion(fileName, document.GetLocation(offset), document.GetLocation(offset + length));
					var builder = SearchResultsPad.CreateInlineBuilder(region.Begin, region.End, document, highlighter);
					results.Add(new SearchResultMatch(fileName, document.GetLocation(offset), document.GetLocation(offset + length), offset, length, builder, highlighter.DefaultTextColor));
				}
				offset = textSource.IndexOf(entity.Name, offset + length, textSource.TextLength - offset - length, StringComparison.OrdinalIgnoreCase);
			} while (offset > 0);
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
					bool isNameValid = XmlReader.IsNameToken(args.NewName);
					object progressLock = new object();
					Parallel.ForEach(
						interestingFileNames,
						new ParallelOptions {
							MaxDegreeOfParallelism = Environment.ProcessorCount,
							CancellationToken = cancellationToken
						},
						delegate (FileName fileName) {
							RenameReferencesInFile(args, fileName, callback, errorCallback, isNameValid, cancellationToken);
							lock (progressLock)
								args.ProgressMonitor.Progress += workAmountInverse;
						});
				}, cancellationToken
			);
		}
		
		void RenameReferencesInFile(SymbolRenameArgs args, FileName fileName, Action<PatchedFile> callback, Action<Error> errorCallback, bool isNameValid, CancellationToken cancellationToken)
		{
			ITextSource textSource = args.ParseableFileContentFinder.Create(fileName);
			if (textSource == null)
				return;
			int offset = textSource.IndexOf(entity.Name, 0, textSource.TextLength, StringComparison.Ordinal);
			if (offset < 0)
				return;
			
			var parseInfo = SD.ParserService.Parse(fileName, textSource) as XamlFullParseInformation;
			if (parseInfo == null)
				return;
			ReadOnlyDocument document = null;
			IHighlighter highlighter = null;
			List<RenameResultMatch> results = new List<RenameResultMatch>();
			XamlAstResolver resolver = new XamlAstResolver(compilation, parseInfo);
			string newCode = args.NewName;
			do {
				if (document == null) {
					document = new ReadOnlyDocument(textSource, fileName);
					highlighter = SD.EditorControlService.CreateHighlighter(document);
					highlighter.BeginHighlighting();
				}
				var result = resolver.ResolveAtLocation(document.GetLocation(offset + entity.Name.Length / 2 + 1), cancellationToken);
				int length = entity.Name.Length;
				if ((result is TypeResolveResult && ((TypeResolveResult)result).Type.Equals(entity)) || (result is MemberResolveResult && ((MemberResolveResult)result).Member.Equals(entity))) {
					var region = new DomRegion(fileName, document.GetLocation(offset), document.GetLocation(offset + length));
					var builder = SearchResultsPad.CreateInlineBuilder(region.Begin, region.End, document, highlighter);
					results.Add(new RenameResultMatch(fileName, document.GetLocation(offset), document.GetLocation(offset + length), offset, length, newCode, builder, highlighter.DefaultTextColor));
				}
				offset = textSource.IndexOf(entity.Name, offset + length, textSource.TextLength - offset - length, StringComparison.OrdinalIgnoreCase);
			} while (offset > 0);
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
