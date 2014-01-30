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
