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
