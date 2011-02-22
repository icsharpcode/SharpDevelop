// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using ICSharpCode.SharpDevelop.Project;
using NuGet;

namespace PackageManagement.Tests.Helpers
{
	public class FakeProjectSystemFactory : ISharpDevelopProjectSystemFactory
	{
		public FakeProjectSystem FakeProjectSystem = new FakeProjectSystem();
		
		public IProjectSystem CreateProjectSystem(MSBuildBasedProject project)
		{
			return FakeProjectSystem;
		}
	}
}
