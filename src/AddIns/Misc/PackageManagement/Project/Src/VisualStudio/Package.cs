// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using Microsoft.VisualStudio.ExtensionManager;

namespace Microsoft.VisualStudio.Shell
{
	public abstract class Package
	{
		public static object GetGlobalService(Type serviceType)
		{
			return new SVsExtensionManager();
		}
	}
}
