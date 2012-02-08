// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.PackageManagement.Scripting;

namespace ICSharpCode.PackageManagement
{
	public interface IUpdatePackageActions
	{
		bool UpdateDependencies { get; set; }
		bool AllowPrereleaseVersions { get; set; }
		IPackageScriptRunner PackageScriptRunner { get; set; }
		
		IEnumerable<UpdatePackageAction> CreateActions();
	}
}
