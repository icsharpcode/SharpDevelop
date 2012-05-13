// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeInterface : CodeType
	{
		public CodeInterface()
		{
		}
		
		public CodeFunction AddFunction(string name, vsCMFunction kind, object type, object Position = null, vsCMAccess Access = vsCMAccess.vsCMAccessPublic)
		{
			throw new NotImplementedException();
		}
	}
}
