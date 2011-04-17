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
				packageViewModelFactory.PackageManagementService,
				packageViewModelFactory.PackageManagementEvents)
		{
		}
		
		public PackageViewModelFactory(
			IPackageManagementService packageManagementService,
			IPackageManagementEvents packageManagementEvents)
		{
			this.PackageManagementService = packageManagementService;
			this.PackageManagementEvents = packageManagementEvents;
			this.Logger = new PackageManagementLogger(packageManagementEvents);
		}
		
		public virtual PackageViewModel CreatePackageViewModel(IPackageFromRepository package)
		{
			return new PackageViewModel(
				package,
				PackageManagementService,
				PackageManagementEvents,
				Logger);
		}
		
		public IPackageManagementService PackageManagementService { get; private set; }
		public IPackageManagementEvents PackageManagementEvents { get; private set; }
		public ILogger Logger { get; private set; }
	}
}
