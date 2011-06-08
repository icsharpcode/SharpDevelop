// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.PackageManagement.Scripting
{
	public class ClearPackageManagementConsoleHostCommand
	{
		IPackageManagementConsoleHost consoleHost;
		
		public ClearPackageManagementConsoleHostCommand(IPackageManagementConsoleHost consoleHost)
		{
			this.consoleHost = consoleHost;
		}
		
		public void ClearHost()
		{
			consoleHost.Clear();
		}
	}
}
