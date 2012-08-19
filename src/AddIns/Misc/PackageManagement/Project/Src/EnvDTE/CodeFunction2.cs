// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeFunction2 : CodeFunction
	{
		public CodeFunction2()
		{
		}
		
		public virtual bool IsGeneric {
			get { throw new NotImplementedException(); }
		}
		
		public virtual vsCMOverrideKind OverrideKind {
			get { throw new NotImplementedException(); }
		}
	}
}
