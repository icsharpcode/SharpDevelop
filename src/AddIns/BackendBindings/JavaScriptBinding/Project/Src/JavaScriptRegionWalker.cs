// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using Antlr.Runtime;
using Antlr.Runtime.Tree;
using ICSharpCode.SharpDevelop.Dom;
using Xebic.Parsers.ES3;

namespace ICSharpCode.JavaScriptBinding
{
	public class JavaScriptRegionWalker
	{
		JavaScriptAst ast;
		ICompilationUnit compilationUnit;
		Stack<JavaScriptRegionStart> regions;
		
		public JavaScriptRegionWalker(
			JavaScriptAst ast,
			ICompilationUnit compilationUnit)
		{
			this.ast = ast;
			this.compilationUnit = compilationUnit;
		}
		
		public void Walk()
		{
			regions = new Stack<JavaScriptRegionStart>();
			
			foreach (IToken token in ast.GetTokens()) {
				if (token.IsSingleLineComment()) {
					WalkComment(token);
				}
			}
		}
		
		void WalkComment(IToken token)
		{
			if (JavaScriptRegionStart.IsRegionStart(token)) {
				WalkRegionStart(token);
			} else if (JavaScriptRegionEnd.IsRegionEnd(token)) {
				WalkRegionEnd(token);
			}
		}
		
		void WalkRegionStart(IToken token)
		{
			var regionStart = new JavaScriptRegionStart(token);
			regions.Push(regionStart);
		}
		
		void WalkRegionEnd(IToken token)
		{
			if (regions.Count > 0) {
				JavaScriptRegionStart regionStart = regions.Pop();
				
				var regionEnd = new JavaScriptRegionEnd(token);
				var region = new JavaScriptRegion(regionStart, regionEnd);
				region.AddRegion(compilationUnit.FoldingRegions);
			}
		}
	}
}
