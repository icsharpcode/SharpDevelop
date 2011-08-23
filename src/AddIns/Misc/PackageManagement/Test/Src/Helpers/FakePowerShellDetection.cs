// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.Scripting;

namespace PackageManagement.Tests.Helpers
{
	public class FakePowerShellDetection : IPowerShellDetection
	{
		public bool IsPowerShell2InstalledReturnValue = true;
		
		public bool IsPowerShell2Installed()
		{
			return IsPowerShell2InstalledReturnValue;
		}
	}
}
