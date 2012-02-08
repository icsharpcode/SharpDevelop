// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using ICSharpCode.PackageManagement.Scripting;

namespace PackageManagement.Tests.Helpers
{
	public class FakeUpdatePackageActions : IUpdatePackageActions
	{
		public bool UpdateDependencies { get; set; }
		public bool AllowPrereleaseVersions { get; set; }
		public IPackageScriptRunner PackageScriptRunner { get; set; }
		
		public List<FakeUpdatePackageAction> FakeActions = new List<FakeUpdatePackageAction>();
		
		public IEnumerable<UpdatePackageAction> CreateActions()
		{
			return FakeActions;
		}
	}
}
