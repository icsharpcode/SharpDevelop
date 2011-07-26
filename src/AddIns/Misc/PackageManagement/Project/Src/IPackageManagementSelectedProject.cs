// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public interface IPackageManagementSelectedProject
	{
		string Name { get; }
		bool IsSelected { get; set; }
		bool IsEnabled { get; set; }
		
		IPackageManagementProject Project { get; }
	}
}
