// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using NuGet;

namespace PackageManagement.Tests.Helpers
{
	public class FakePackageViewModelFactory : IPackageViewModelFactory
	{
		public FakeRegisteredPackageRepositories FakeRegisteredPackageRepositories = new FakeRegisteredPackageRepositories();
		public FakePackageManagementService FakePackageManagementService = new FakePackageManagementService();
		public FakePackageManagementEvents FakePackageManagementEvents = new FakePackageManagementEvents();
		public FakeLogger FakeLogger = new FakeLogger();
		
		public PackageViewModel CreatePackageViewModel(IPackage package)
		{
			return new PackageViewModel(
				package,
				FakeRegisteredPackageRepositories.FakeActiveRepository,
				FakePackageManagementService,
				FakePackageManagementEvents,
				FakeLogger);
		}
		
		public IRegisteredPackageRepositories RegisteredPackageRepositories {
			get { return FakeRegisteredPackageRepositories; }
		}
		
		public IPackageManagementService PackageManagementService { 
			get { return FakePackageManagementService; }
		}
		
		public IPackageManagementEvents PackageManagementEvents {
			get { return FakePackageManagementEvents; }
		}
	}
}
