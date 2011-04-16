// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class UpdatedPackageViewModel : PackageViewModel
	{
		IPackageManagementService packageManagementService;
		
		public UpdatedPackageViewModel(
			IPackage package,
			IPackageRepository sourceRepository,
			IPackageManagementService packageManagementService,
			IPackageManagementEvents packageManagementEvents,
			ILogger logger)
			: base(package, sourceRepository, packageManagementService, packageManagementEvents, logger)
		{
			this.packageManagementService = packageManagementService;
		}
		
		protected override void InstallPackage(
			IPackageRepository sourcePackageRepository,
			IPackage package,
			IEnumerable<PackageOperation> packageOperations)
		{
			var action = packageManagementService.CreateUpdatePackageAction();
			action.PackageRepository = sourcePackageRepository;
			action.Package = package;
			action.Operations = packageOperations;
			action.Execute();
		}
	}
}
