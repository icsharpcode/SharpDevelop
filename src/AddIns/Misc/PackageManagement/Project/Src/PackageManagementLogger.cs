// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class PackageManagementLogger : ILogger
	{
		IPackageManagementEvents packageManagementEvents;
		
		public PackageManagementLogger(IPackageManagementEvents packageManagementEvents)
		{
			this.packageManagementEvents = packageManagementEvents;
		}
		
		public void Log(MessageLevel level, string message, params object[] args)
		{
			packageManagementEvents.OnPackageOperationMessageLogged(level, message, args);
		}
	}
}
