// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PythonBinding
{
	public class PythonModule : DefaultClass
	{
		public PythonModule(ICompilationUnit compilationUnit)
			: base(compilationUnit, compilationUnit.UsingScope.NamespaceName)
		{
			compilationUnit.Classes.Add(this);
		}
	}
}
