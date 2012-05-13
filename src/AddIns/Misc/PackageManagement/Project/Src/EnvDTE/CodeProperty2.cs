// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeProperty2 : CodeProperty
	{
		public CodeProperty2()
		{
		}
		
		public CodeElements Members {
			get { throw new NotImplementedException(); }
		}
		
		public vsCMPropertyKind ReadWrite { 
			get { throw new NotImplementedException(); }
		}
		
		public CodeElements Parameters {
			get { throw new NotImplementedException(); }
		}
	}
}
