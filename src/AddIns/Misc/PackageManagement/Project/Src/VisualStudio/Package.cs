// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.EnvDTE;
using Microsoft.VisualStudio.ExtensionManager;

namespace Microsoft.VisualStudio.Shell
{
	public abstract class Package
	{
		public static object GetGlobalService(Type serviceType)
		{
			//typeof(IVsSolution)
			//typeof(SComponentModel) --> not used - console initializer.
			//typeof(SVsExtensionManager)
			if (serviceType == typeof(DTE)) {
				return new DTE();
			} else if (serviceType == typeof(SVsExtensionManager)) {
				return new SVsExtensionManager();
			}
			return null;
		}
	}
}
