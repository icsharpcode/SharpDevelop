// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using ICSharpCode.NRefactory.VB.Ast;
using ICSharpCode.NRefactory.VB.Parser;

namespace ICSharpCode.NRefactory.VB
{
	public enum SnippetType
	{
		None,
		CompilationUnit,
		Expression,
		Statements,
		TypeMembers
	}
	
	public class VBParser
	{
		public CompilationUnit ParseFile(string content)
		{
			throw new NotImplementedException();
		}
		
		public CompilationUnit ParseFile(TextReader reader)
		{
			throw new NotImplementedException();
		}
		
		public AstNode ParseSnippet(TextReader reader)
		{
			throw new NotImplementedException();
		}
	}
}