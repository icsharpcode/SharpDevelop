// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement;
using ICSharpCode.SharpDevelop.Project;
using NuGet;

namespace PackageManagement.Tests.Helpers
{
	public class FakePackageManagerFactory : IPackageManagerFactory
	{
		public FakePackageManager FakePackageManager = new FakePackageManager();
		public IPackageRepository PackageRepositoryPassedToCreatePackageManager;
		public MSBuildBasedProject ProjectPassedToCreateRepository;
		
		public ISharpDevelopPackageManager CreatePackageManager(IPackageRepository packageRepository, MSBuildBasedProject project)
		{
			PackageRepositoryPassedToCreatePackageManager = packageRepository;
			ProjectPassedToCreateRepository = project;
			return FakePackageManager;
		}
	}
}
