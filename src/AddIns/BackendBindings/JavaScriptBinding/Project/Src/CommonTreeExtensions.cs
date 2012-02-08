// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using Antlr.Runtime.Tree;
using Xebic.Parsers.ES3;

namespace ICSharpCode.JavaScriptBinding
{
	public static class CommonTreeExtensions
	{
		public static bool IsFunction(this CommonTree tree)
		{
			return tree.Type == ES3Parser.FUNCTION;
		}
		
		public static bool HasChildren(this CommonTree tree)
		{
			return tree.ChildCount > 0;
		}
		
		public static CommonTree GetFirstChild(this CommonTree tree)
		{
			return tree.GetChild(0) as CommonTree;
		}
		
		public static CommonTree GetFirstChildArguments(this CommonTree tree)
		{
			return tree.GetFirstChildWithType(ES3Parser.ARGS) as CommonTree;
		}
		
		public static int BeginColumn(this CommonTree tree)
		{
			return tree.CharPositionInLine + 1;
		}
	}
}
