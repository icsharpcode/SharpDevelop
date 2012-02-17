// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Threading;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.CSharp.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Parser;

namespace CSharpBinding.Parser
{
	public class CSharpFullParseInformation : ParseInformation
	{
		readonly CompilationUnit compilationUnit;
		
		public CSharpFullParseInformation(CSharpParsedFile parsedFile, CompilationUnit compilationUnit)
			: base(parsedFile, isFullParseInformation: true)
		{
			if (parsedFile == null)
				throw new ArgumentNullException("parsedFile");
			if (compilationUnit == null)
				throw new ArgumentNullException("compilationUnit");
			this.compilationUnit = compilationUnit;
		}
		
		public new CSharpParsedFile ParsedFile {
			get { return (CSharpParsedFile)base.ParsedFile; }
		}
		
		public CompilationUnit CompilationUnit {
			get { return compilationUnit; }
		}
		
		public CSharpAstResolver GetResolver(ICompilation compilation)
		{
			return (CSharpAstResolver)compilation.CacheManager.GetOrAddShared(
				this, _ => new CSharpAstResolver(compilation, compilationUnit, ParsedFile)
			);
		}
	}
}
