// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeClass : CodeType
	{
		public virtual CodeElements ImplementedInterfaces {
			get { throw new NotImplementedException(); }
		}
		
		public virtual CodeVariable AddVariable(string name, object type, object Position = null, vsCMAccess Access = vsCMAccess.vsCMAccessPublic, object Location = null)
		{
			throw new NotImplementedException();
		}
	}
}
