// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using CSharpBinding.Parser;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.CSharp.TypeSystem;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.Utils;
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
		
		public CSharpSymbolSearch(IProject project, IEntity entity)
		{
			this.project = project;
			searchScopes = fr.GetSearchScopes(entity);
			compilation = ParserService.GetCompilation(project);
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
		
		public void FindReferences(SymbolSearchArgs args, Action<Reference> callback)
		{
			if (callback == null)
				throw new ArgumentNullException("callback");
			var cancellationToken = args.ProgressMonitor.CancellationToken;
			
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
						FindReferencesInFile(args, searchScope, FileName.Create(fileName), callback, cancellationToken);
						lock (progressLock)
							args.ProgressMonitor.Progress += workAmountInverse;
					});
			}
		}
		
		void FindReferencesInFile(SymbolSearchArgs args, IFindReferenceSearchScope searchScope, FileName fileName, Action<Reference> callback, CancellationToken cancellationToken)
		{
			ITextSource textSource = args.ParseableFileContentFinder.Create(fileName);
			if (textSource == null)
				return;
			if (searchScope.SearchTerm != null) {
				if (textSource.IndexOf(searchScope.SearchTerm, 0, textSource.TextLength, StringComparison.Ordinal) < 0)
					return;
			}
			
			var parseInfo = ParserService.Parse(fileName, textSource) as CSharpFullParseInformation;
			if (parseInfo == null)
				return;
			fr.FindReferencesInFile(
				searchScope, parseInfo.ParsedFile, parseInfo.CompilationUnit, compilation,
				delegate (AstNode node, ResolveResult result) {
					var region = new DomRegion(fileName, node.StartLocation, node.EndLocation);
					callback(new Reference(region, result));
				}, cancellationToken);
		}
	}
}
