// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using ICSharpCode.NRefactory.Semantics;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeAttributeArgument : CodeElement, global::EnvDTE.CodeAttributeArgument
	{
		string name;
		string value;
		
		public CodeAttributeArgument()
		{
		}
		
		public CodeAttributeArgument(string name, string value)
		{
			this.name = name;
			this.value = value;
		}
		
		public CodeAttributeArgument(string name, ResolveResult value)
		{
			this.name = name;
			this.value = GetValue(value);
		}
		
		string GetValue(ResolveResult value)
		{
			var astBuilder = new TypeSystemAstBuilder();
			var ast = astBuilder.ConvertConstantValue(value);
			return ast.ToString();
		}
		
		public override string Name {
			get { return name; }
		}
		
		public virtual string Value {
			get { return value; }
		}
	}
}
