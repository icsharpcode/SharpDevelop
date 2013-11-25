// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
			// TODO implement Rename for XAML
			return Task.Run(() => {});
		}
	}
}
