// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using NuGet;

namespace ICSharpCode.PackageManagement.Scripting
{
	public interface IPackageScript
	{
		IPackageManagementProject Project { get; set; }
		IPackage Package { get; set; }
	
		bool Exists();
		void Run(IPackageScriptSession session);
	}
}
