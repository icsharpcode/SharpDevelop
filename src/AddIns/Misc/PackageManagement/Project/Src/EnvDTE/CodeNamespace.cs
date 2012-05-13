// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeNamespace : CodeElement
	{
		public CodeNamespace()
		{
		}
		
		public string FullName {
			get { throw new NotImplementedException(); }
		}
		
		public CodeElements Members {
			get { throw new NotImplementedException(); }
		}
	}
}
