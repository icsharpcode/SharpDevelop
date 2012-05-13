// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeTypeRef : MarshalByRefObject
	{
		public CodeTypeRef()
		{
		}
		
		public virtual string AsFullName {
			get { throw new NotImplementedException(); }
		}
		
		public virtual object Parent {
			get { throw new NotImplementedException(); }
		}
		
		public virtual CodeType CodeType {
			get { throw new NotImplementedException(); }
		}
	}
}
