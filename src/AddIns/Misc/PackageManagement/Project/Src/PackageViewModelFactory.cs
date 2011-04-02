// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class PackageViewModelFactory : IPackageViewModelFactory
	{
		public PackageViewModelFactory(
			IPackageManagementService packageManagementService,
			ILicenseAcceptanceService licenseAcceptanceService,
			IMessageReporter messageReporter)
		{
			this.PackageManagementService = packageManagementService;
			this.LicenseAcceptanceService = licenseAcceptanceService;
			this.MessageReporter = messageReporter;
		}
		
		public virtual PackageViewModel CreatePackageViewModel(IPackage package)
		{
			return new PackageViewModel(
				package, 
				PackageManagementService, 
				LicenseAcceptanceService,
				MessageReporter);
		}
		
		protected IPackageManagementService PackageManagementService { get; private set; }
		protected ILicenseAcceptanceService LicenseAcceptanceService { get; private set; }
		protected IMessageReporter MessageReporter { get; private set; }
	}
}
