// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using Antlr.Runtime.Tree;
using ICSharpCode.SharpDevelop.Dom;
using Xebic.Parsers.ES3;

namespace ICSharpCode.JavaScriptBinding
{
	public class JavaScriptAstWalker
	{
		JavaScriptCompilationUnit compilationUnit;
		JavaScriptAst ast;
		
		public JavaScriptAstWalker(
			JavaScriptCompilationUnit compilationUnit,
			JavaScriptAst ast)
		{
			this.compilationUnit = compilationUnit;
			this.ast = ast;
		}
		
		public void Walk()
		{
			if (ast.IsValid) {
				Walk(ast.Tree);
				WalkRegions();
			}
		}
		
		void Walk(CommonTree tree)
		{
			if (tree.IsFunction()) {
				AddMethod(tree);
			}
			
			if (tree.HasChildren()) {
				WalkChildren(tree.Children);
			}
		}
		
		void AddMethod(CommonTree tree)
		{
			var functionDefinition = new JavaScriptFunctionDefinition(ast, tree);
			functionDefinition.AddMethod(compilationUnit.GlobalClass);
		}
		
		void WalkChildren(IEnumerable<ITree> children)
		{
			foreach (CommonTree child in children) {
				Walk(child);
			}
		}
		
		void WalkRegions()
		{
			var regionWalker = new JavaScriptRegionWalker(ast, compilationUnit);
			regionWalker.Walk();
		}
	}
}
