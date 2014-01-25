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
