// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.EnvDTE;
using ICSharpCode.PackageManagement.VisualStudio;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.ExtensionManager;
using Microsoft.VisualStudio.Shell.Interop;

namespace Microsoft.VisualStudio.Shell
{
	public abstract class Package
	{
		public static object GetGlobalService(Type serviceType)
		{
			if (serviceType == typeof(global::EnvDTE.DTE)) {
				return new DTE();
			} else if (serviceType == typeof(SVsExtensionManager)) {
				return new SVsExtensionManager();
			} else if (serviceType == typeof(IVsSolution)) {
				return new VsSolution();
			} else if (serviceType == typeof(SComponentModel)) {
				return new ComponentModel();
			}
			return null;
		}
	}
}
