// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class UpdatedPackageViewModel : PackageViewModel
	{
		IPackageManagementSolution solution;
		
		public UpdatedPackageViewModel(
			IPackageFromRepository package,
			IPackageManagementSolution solution,
			IPackageManagementEvents packageManagementEvents,
			ILogger logger)
			: base(package, solution, packageManagementEvents, logger)
		{
			this.solution = solution;
		}
		
		protected override void InstallPackage(
			IPackageFromRepository package,
			IEnumerable<PackageOperation> packageOperations)
		{
			var action = solution.CreateUpdatePackageAction();
			action.PackageRepository = package.Repository;
			action.Package = package;
			action.Operations = packageOperations;
			action.Execute();
		}
	}
}
