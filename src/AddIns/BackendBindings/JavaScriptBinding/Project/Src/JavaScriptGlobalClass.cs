// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.JavaScriptBinding
{
	public class JavaScriptGlobalClass : DefaultClass
	{
		public JavaScriptGlobalClass(
			ICompilationUnit compilationUnit,
			string fullyQualifiedName)
			: base(compilationUnit, fullyQualifiedName)
		{
		}
		
		public DefaultMethod AddMethod(string name)
		{
			var method = new DefaultMethod(this, name);
			AddMethod(method);
			return method;
		}
		
		public void AddMethod(IMethod method)
		{
			Methods.Add(method);
		}
	}
}
