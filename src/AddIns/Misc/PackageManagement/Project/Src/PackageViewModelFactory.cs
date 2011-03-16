// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class PackageViewModelFactory : IPackageViewModelFactory
	{
		IPackageManagementService packageManagementService;
		ILicenseAcceptanceService licenseAcceptanceService;
		IMessageReporter messageReporter;
		
		public PackageViewModelFactory(
			IPackageManagementService packageManagementService,
			ILicenseAcceptanceService licenseAcceptanceService,
			IMessageReporter messageReport)
		{
			this.packageManagementService = packageManagementService;
			this.licenseAcceptanceService = licenseAcceptanceService;
			this.messageReporter = messageReport;
		}
		
		public PackageViewModel CreatePackageViewModel(IPackage package)
		{
			return new PackageViewModel(
				package, 
				packageManagementService, 
				licenseAcceptanceService,
				messageReporter);
		}
	}
}
