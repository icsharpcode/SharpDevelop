// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeModel : MarshalByRefObject
	{
		public CodeModel()
		{
		}
		
		public CodeElements CodeElements {
			get { throw new NotImplementedException(); }
		}
		
		public CodeType CodeTypeFromFullName(string name)
		{
			throw new NotImplementedException();
		}
	}
}
