// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using NuGet;

namespace PackageManagement.Tests.Helpers
{
	public class ExceptionThrowingPackageManagementSolution : FakePackageManagementSolution
	{
		public Exception ExceptionToThrowWhenGetActiveProjectCalled { get; set; }
		
		public override IPackageManagementProject GetActiveProject()
		{
			if (ExceptionToThrowWhenGetActiveProjectCalled != null) {
				throw ExceptionToThrowWhenGetActiveProjectCalled;
			}
			return base.GetActiveProject();
		}
	}
}
