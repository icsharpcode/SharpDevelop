// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Threading;

using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.CSharp.TypeSystem;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Parser;

namespace CSharpBinding.Parser
{
	public class CSharpFullParseInformation : ParseInformation
	{
		readonly SyntaxTree syntaxTree;
		internal List<NewFolding> newFoldings;
		
		public CSharpFullParseInformation(CSharpUnresolvedFile unresolvedFile, ITextSourceVersion parsedVersion, SyntaxTree compilationUnit)
			: base(unresolvedFile, parsedVersion, isFullParseInformation: true)
		{
			if (unresolvedFile == null)
				throw new ArgumentNullException("unresolvedFile");
			if (compilationUnit == null)
				throw new ArgumentNullException("compilationUnit");
			this.syntaxTree = compilationUnit;
		}
		
		public new CSharpUnresolvedFile UnresolvedFile {
			get { return (CSharpUnresolvedFile)base.UnresolvedFile; }
		}
		
		public SyntaxTree SyntaxTree {
			get { return syntaxTree; }
		}
		
		static readonly object ResolverCacheKey = new object();
		
		public CSharpAstResolver GetResolver(ICompilation compilation)
		{
			// Cache the resolver within the compilation.
			// (caching in the parse information could prevent the compilation from being garbage-collected)
			
			// Also, don't cache CSharpAstResolvers for every file - doing so would require too much memory,
			// and we usually only need to access the same file several times.
			// So we use a static key to get the resolver, and verify that it belongs to this parse information.
			var resolver = compilation.CacheManager.GetShared(ResolverCacheKey) as CSharpAstResolver;
			if (resolver == null || resolver.RootNode != syntaxTree || resolver.UnresolvedFile != UnresolvedFile) {
				resolver = new CSharpAstResolver(compilation, syntaxTree, UnresolvedFile);
				compilation.CacheManager.SetShared(ResolverCacheKey, resolver);
			}
			return resolver;
		}
		
		public override IEnumerable<NewFolding> GetFoldings(IDocument document, out int firstErrorOffset)
		{
			firstErrorOffset = -1;
			return newFoldings;
		}
	}
}
