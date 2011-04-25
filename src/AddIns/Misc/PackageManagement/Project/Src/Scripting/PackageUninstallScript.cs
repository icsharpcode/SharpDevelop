// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.PackageManagement.Scripting
{
	public class PackageUninstallScript : PackageScript
	{
		public PackageUninstallScript(
			PackageUninstallScriptFileName fileName,
			IPackageScriptSession session)
			: base(fileName, session)
		{
		}
	}
}
