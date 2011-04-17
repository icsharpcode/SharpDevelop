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
			IPackageFromRepository package,
			IPackageManagementService packageManagementService,
			IPackageManagementEvents packageManagementEvents,
			ILogger logger)
			: base(package, packageManagementService, packageManagementEvents, logger)
		{
			this.packageManagementService = packageManagementService;
		}
		
		protected override void InstallPackage(
			IPackageFromRepository package,
			IEnumerable<PackageOperation> packageOperations)
		{
			var action = packageManagementService.CreateUpdatePackageAction();
			action.PackageRepository = package.Repository;
			action.Package = package;
			action.Operations = packageOperations;
			action.Execute();
		}
	}
}
