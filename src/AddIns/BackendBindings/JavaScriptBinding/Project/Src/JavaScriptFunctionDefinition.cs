// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
