// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Threading;
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
		readonly SyntaxTree compilationUnit;
		readonly ITextSourceVersion parsedVersion;
		
		public CSharpFullParseInformation(CSharpUnresolvedFile unresolvedFile, ITextSourceVersion parsedVersion, SyntaxTree compilationUnit)
			: base(unresolvedFile, isFullParseInformation: true)
		{
			if (unresolvedFile == null)
				throw new ArgumentNullException("unresolvedFile");
			if (compilationUnit == null)
				throw new ArgumentNullException("compilationUnit");
			this.parsedVersion = parsedVersion;
			this.compilationUnit = compilationUnit;
		}
		
		public new CSharpUnresolvedFile UnresolvedFile {
			get { return (CSharpUnresolvedFile)base.UnresolvedFile; }
		}
		
		public SyntaxTree SyntaxTree {
			get { return compilationUnit; }
		}
		
		public ITextSourceVersion ParsedVersion {
			get { return parsedVersion; }
		}
		
		public CSharpAstResolver GetResolver(ICompilation compilation)
		{
			return (CSharpAstResolver)compilation.CacheManager.GetOrAddShared(
				this, _ => new CSharpAstResolver(compilation, compilationUnit, UnresolvedFile)
			);
		}
	}
}
