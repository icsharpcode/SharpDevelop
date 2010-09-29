// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Dom;
using IronPython.Compiler.Ast;
using Microsoft.Scripting;

namespace ICSharpCode.PythonBinding
{
	public class PythonMethodDefinition
	{
		FunctionDefinition methodDefinition;
		
		public PythonMethodDefinition(FunctionDefinition methodDefinition)
		{
			this.methodDefinition = methodDefinition;
		}
		
		public PythonMethod CreateMethod(IClass c)
		{
			if (IsConstructor) {
				return new PythonConstructor(c, methodDefinition);
			}
			return new PythonMethod(c, methodDefinition);
		}
		
		bool IsConstructor {
			get { return methodDefinition.Name == "__init__"; }
		}
	}
}
