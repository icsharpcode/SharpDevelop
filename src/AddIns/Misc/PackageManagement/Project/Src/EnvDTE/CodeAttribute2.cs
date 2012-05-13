// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeAttribute2 : CodeAttribute
	{
		public CodeAttribute2()
		{
		}
		
		public virtual string FullName {
			get { throw new NotImplementedException(); }
		}
		
		public virtual CodeElements Arguments {
			get { throw new NotImplementedException(); }
		}
	}
}
