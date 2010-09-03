// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PythonBinding
{
	public class PythonMethodGroupResolveResult : MethodGroupResolveResult
	{
		public PythonMethodGroupResolveResult(IClass containingClass, string methodName)
			: base(null, null, containingClass.DefaultReturnType, methodName)
		{
		}
	}
}
