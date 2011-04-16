// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class PackageViewModelFactory : IPackageViewModelFactory
	{
		public PackageViewModelFactory(IPackageViewModelFactory packageViewModelFactory)
			: this(
				packageViewModelFactory.RegisteredPackageRepositories,
				packageViewModelFactory.PackageManagementService,
				packageViewModelFactory.PackageManagementEvents)
		{
		}
		
		public PackageViewModelFactory(
			IRegisteredPackageRepositories registeredPackageRepositories,
			IPackageManagementService packageManagementService,
			IPackageManagementEvents packageManagementEvents)
		{
			this.RegisteredPackageRepositories = registeredPackageRepositories;
			this.PackageManagementService = packageManagementService;
			this.PackageManagementEvents = packageManagementEvents;
			this.Logger = new PackageManagementLogger(packageManagementEvents);
		}
		
		public virtual PackageViewModel CreatePackageViewModel(IPackage package)
		{
			return new PackageViewModel(
				package,
				RegisteredPackageRepositories.ActiveRepository,
				PackageManagementService,
				PackageManagementEvents,
				Logger);
		}
		
		public IRegisteredPackageRepositories RegisteredPackageRepositories { get; private set; }
		public IPackageManagementService PackageManagementService { get; private set; }
		public IPackageManagementEvents PackageManagementEvents { get; private set; }
		public ILogger Logger { get; private set; }
	}
}
