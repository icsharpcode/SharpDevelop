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
using Antlr.Runtime.Tree;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.JavaScriptBinding
{
	public class JavaScriptFunctionDefinition
	{
		JavaScriptAst ast;
		CommonTree tree;
		
		public JavaScriptFunctionDefinition(JavaScriptAst ast, CommonTree tree)
		{
			this.ast = ast;
			this.tree = tree;
		}
		
		public void AddMethod(JavaScriptGlobalClass c)
		{
			string methodName = GetMethodName();
			DefaultMethod method = c.AddMethod(methodName);
			UpdateRegions(method);
		}
		
		string GetMethodName()
		{
			CommonTree child = tree.GetFirstChild();
			return child.Text;
		}
		
		void UpdateRegions(DefaultMethod method)
		{
			var methodRegion = new JavaScriptMethodRegion(ast, tree);
			method.Region = methodRegion.GetHeaderRegion();
			method.BodyRegion = methodRegion.GetBodyRegion();
		}
	}
}
