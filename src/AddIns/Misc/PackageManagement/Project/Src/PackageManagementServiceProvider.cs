// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using Microsoft.VisualStudio.Shell;

namespace ICSharpCode.PackageManagement
{
	public class PackageManagementServiceProvider : IServiceProvider
	{
		public object GetService(Type serviceType)
		{
			return Package.GetGlobalService(serviceType);
		}
	}
}
