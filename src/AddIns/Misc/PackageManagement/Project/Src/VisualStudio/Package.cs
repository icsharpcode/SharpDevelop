// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.EnvDTE;
using ICSharpCode.PackageManagement.VisualStudio;
using Microsoft.VisualStudio.ExtensionManager;
using Microsoft.VisualStudio.Shell.Interop;

namespace Microsoft.VisualStudio.Shell
{
	public abstract class Package
	{
		public static object GetGlobalService(Type serviceType)
		{
			//typeof(SComponentModel) --> not used - console initializer.
			if (serviceType == typeof(DTE)) {
				return new DTE();
			} else if (serviceType == typeof(SVsExtensionManager)) {
				return new SVsExtensionManager();
			} else if (serviceType == typeof(IVsSolution)) {
				return new VsSolution();
			}
			return null;
		}
	}
}
