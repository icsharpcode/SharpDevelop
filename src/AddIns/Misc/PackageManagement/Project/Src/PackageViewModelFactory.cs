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
				packageViewModelFactory.LicenseAcceptanceService,
				packageViewModelFactory.MessageReporter)
		{
		}
		
		public PackageViewModelFactory(
			IRegisteredPackageRepositories registeredPackageRepositories,
			IPackageManagementService packageManagementService,
			ILicenseAcceptanceService licenseAcceptanceService,
			IMessageReporter messageReporter)
		{
			this.RegisteredPackageRepositories = registeredPackageRepositories;
			this.PackageManagementService = packageManagementService;
			this.LicenseAcceptanceService = licenseAcceptanceService;
			this.MessageReporter = messageReporter;
		}
		
		public virtual PackageViewModel CreatePackageViewModel(IPackage package)
		{
			return new PackageViewModel(
				package,
				RegisteredPackageRepositories.ActiveRepository,
				PackageManagementService,
				LicenseAcceptanceService,
				MessageReporter);
		}
		
		public IRegisteredPackageRepositories RegisteredPackageRepositories { get; private set; }
		public IPackageManagementService PackageManagementService { get; private set; }
		public ILicenseAcceptanceService LicenseAcceptanceService { get; private set; }
		public IMessageReporter MessageReporter { get; private set; }
	}
}
