// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeAttributeArgument : MarshalByRefObject
	{
		public CodeAttributeArgument()
		{
		}
		
		public virtual string Name {
			get { throw new NotImplementedException(); }
		}
		
		public virtual string Value {
			get { throw new NotImplementedException(); }
		}
	}
}
