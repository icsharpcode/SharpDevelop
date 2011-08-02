// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using Antlr.Runtime;
using Antlr.Runtime.Tree;

namespace ICSharpCode.JavaScriptBinding
{
	public class JavaScriptAst
	{
		CommonTokenStream tokenStream;
		CommonTree tree;
		
		public JavaScriptAst(CommonTokenStream tokenStream, CommonTree tree)
		{
			this.tokenStream = tokenStream;
			this.tree = tree;
		}
		
		public CommonTree Tree {
			get { return tree; }
		}
		
		public bool IsValid {
			get { return tree != null; }
		}
		
		public IToken GetToken(int index)
		{
			return tokenStream.Get(index);
		}
		
		public IList<IToken> GetTokens()
		{
			return tokenStream.GetTokens();
		}
	}
}
