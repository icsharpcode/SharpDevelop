// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.Scripting;

namespace ICSharpCode.PackageManagement
{
	public static class ServiceLocator
	{
		static readonly PackageManagementService packageManagementService = new PackageManagementService();
		static readonly PackageManagementConsoleHost packageManagementConsoleHost = new PackageManagementConsoleHost();
		
		public static IPackageManagementService PackageManagementService {
			get { return packageManagementService; }
		}
		
		public static IPackageManagementConsoleHost PackageManagementConsoleHost {
			get { return packageManagementConsoleHost; }
		}
	}
}
