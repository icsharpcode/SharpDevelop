// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace Microsoft.VisualStudio.ExtensionManager
{
	public class SVsExtensionManager
	{
		public object GetInstalledExtension(string identifier)
		{
			throw new NotInstalledException();
		}
	}
}
