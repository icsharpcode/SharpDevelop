// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;
using IronPython.Compiler.Ast;

namespace ICSharpCode.PythonBinding
{
	public class PythonConstructor : PythonMethod
	{
		public PythonConstructor(IClass declaringType, FunctionDefinition methodDefinition)
			: base(declaringType, methodDefinition, "#ctor")
		{
		}
	}
}
